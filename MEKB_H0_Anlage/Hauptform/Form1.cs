/***************************************************************
  Datei:      Form1.cs
  Author:     Alexander Kühne
  Copyright:  MEBK - All rights reserved.
****************************************************************
  Beschreibung:
              Funktionen zu dem Buttons zum Steuern der Bedienoberfläche

****************************************************************/

using System;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Hauptform
    /// </summary>
    public partial class Form1 : Form
    {
        public Z21 z21Start;
        private Z21_Einstellung z21_Einstellung;
        private Signal_Einstellungen signal_Einstellungen;
        private Belegtmelder_Ueberwachung belegtmelder_Ueberwachung;
        private InfoBox InfoBox;

        public List<Weiche> Weichenliste = new List<Weiche>();
        public List<Signal> Signalliste = new List<Signal>();
        public List<Lok> Lokliste = new List<Lok>();
        public List<Belegtmelder> Belegtmelderliste = new List<Belegtmelder>();
        public Lok[] AktiveLoks = new Lok[12];

        public bool Betriebsbereit;

        private static System.Timers.Timer FlagTimer;
        private static System.Timers.Timer WeichenTimer;
        private static System.Timers.Timer CooldownTimer;
        private static System.Timers.Timer BelegtmelderCoolDown;

        private int Pointer_Weichenliste = 0;
        private int Pointer_Signalliste = 0;
        private bool Signal_Init;
        private bool Weichen_Init;

        private Logger Log { set; get; }

        #region Hauptform Funktionen
        public Form1()
        {
            Log = new Logger("log.txt");

            InitializeComponent();                      //Programminitialisieren
            
            z21Start = new Z21();                   //Neue Z21-Verbindung anlegen
            z21Start.SetLog(Log);
            //Callback Funktionen registrieren
            z21Start.Register_LAN_CONNECT_STATUS(SetConnect);
            z21Start.Register_LAN_GET_SERIAL_NUMBER(CallBack_GET_SERIAL_NUMBER);
            z21Start.Register_LAN_X_TURNOUT_INFO(CallBack_LAN_X_TURNOUT_INFO);
            z21Start.Register_LAN_X_GET_FIRMWARE_VERSION(CallBack_LAN_X_GET_FIRMWARE_VERSION);
            z21Start.Register_LAN_SYSTEMSTATE_DATACHANGED(CallBack_Z21_System_Status);
            z21Start.Register_LAN_GET_BROADCASTFLAGS(CallBack_Z21_Broadcast_Flags);
            z21Start.Register_LAN_X_LOCO_INFO(CallBack_Z21_LokUpdate);


            z21_Einstellung = new Z21_Einstellung();    //Neues Fenster: Einstellung der Z21 (Läuft im Hintergund)
            z21_Einstellung.Get_Z21_Instance(this);     //Z21-Verbindung dem neuen Fenster mitgeben

            ConnectStatus(false, false);                 //Verbindungsstatus auf getrennt setzen

            SetupWeichenListe();                        //Weichenliste aus Datei laden
            SetupSignalListe();                         //Signalliste festlegen
            SetupFahrstrassen();                        //Fahstrassen festlegen          
            SetupLokListe();                            //Lok-Daten aus Dateien laden
            SetupBelegtmelderListe();                   //Belegtmelderliste aus Datei laden

            Betriebsbereit = false;

            LokCtrl_LoklisteAusfuellen();               //Auswahlliste im Lok-Kontrollfenster ausfüllen
            for (int i = 0; i < AktiveLoks.Length; i++)
            {
                AktiveLoks[i] = new Lok();
                AktiveLoks[i].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                AktiveLoks[i].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Gleisplan = this.CreateGraphics();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Pointer_Weichenliste = Weichenliste.Count() - 1;
            Pointer_Signalliste = Signalliste.Count() - 1;
            Signal_Init = false;
            Weichen_Init = false;

            // 5 Sekunden Timer einrichten (Lebenspuls für die Verbindung)
            FlagTimer = new System.Timers.Timer(5000);
            // Timer mit Funktion "Z21_Heartbeat" Verbinden
            FlagTimer.Elapsed += Z21_Heartbeat;
            FlagTimer.AutoReset = true;
            FlagTimer.Enabled = true;

            // 100 MilliSekunden Timer: Weichen und Fahrstraßen Update.
            WeichenTimer = new System.Timers.Timer(100);
            // Timer mit Funktion "OnTimedWeichenEvent" Verbinden
            WeichenTimer.Elapsed += OnTimedWeichenEvent;
            WeichenTimer.AutoReset = true;
            WeichenTimer.Enabled = true;

            // 100 MilliSekunden Timer: Deaktivieren der Weichenmotoren.
            CooldownTimer = new System.Timers.Timer(100);
            // Timer mit Funktion "WeichenCooldown" Verbinden
            CooldownTimer.Elapsed += WeichenCooldown;
            CooldownTimer.AutoReset = true;
            CooldownTimer.Enabled = true;

            // 250 MilliSekunden Timer: Deaktivieren der Weichenmotoren.
            BelegtmelderCoolDown = new System.Timers.Timer(250);
            // Timer mit Funktion "WeichenCooldown" Verbinden
            BelegtmelderCoolDown.Elapsed += BelegtmelderCooldown;
            BelegtmelderCoolDown.AutoReset = true;
            BelegtmelderCoolDown.Enabled = true;

            if (Config.ReadConfig("Auto_Connect").Equals("true")) z21Start.Connect_Z21();   //Wenn "Auto_Connect" gesetzt ist: Verbinden
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAlle_Click(sender, e);
            foreach (Signal signal in Signalliste)
            {
                signal.Schalten(0, z21Start);//Alle Signale Rot
            }
            z21Start.DisConnect_Z21();
        }
        #endregion

        #region Menue-Funktionen
        private void MenuZ21Eigenschaften_Click(object sender, EventArgs e)
        {
            if (z21_Einstellung.IsDisposed) z21_Einstellung = new Z21_Einstellung();
            z21_Einstellung.Show();
        }
        /// <summary>
        /// Menü Zentrale -> Verbinden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Verbinden_Click(object sender, EventArgs e)
        {
            z21Start.Connect_Z21();
            Pointer_Weichenliste = Weichenliste.Count() - 1;
            Pointer_Signalliste = Signalliste.Count() - 1;
            Signal_Init = false;
            Weichen_Init = false;
        }
        private void Menu_Trennen_Click(object sender, EventArgs e)
        {
            z21Start.DisConnect_Z21();
        }
        private void UeberSteuerprogrammToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoBox = new InfoBox();
            InfoBox.Show();
        }
        private void ProgrammBeenden_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void LokEditorOpen_Click(object sender, EventArgs e)
        {
            LokEditor lokEditor = new LokEditor();
            lokEditor.Show();
        }
        #endregion

        #region Timer Funktionen
        private void Z21_Heartbeat(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                if (!z21_Einstellung.IsDisposed) //Fenster Z21-Einstellung nläuft immer noch im Hintergrund
                {
                    Flags temp = z21_Einstellung.Get_Flag_Config();
                    z21Start.Z21_SET_BROADCASTFLAGS(temp); //Flags neu setzen 
                }
            }

        }
        private void OnTimedWeichenEvent(Object source, ElapsedEventArgs e)
        {

            if (z21Start.Verbunden())
            {
                GetWeichenStatus(Weichenliste[Pointer_Weichenliste].Name);
                if (Pointer_Weichenliste <= 0)
                {
                    Pointer_Weichenliste = Weichenliste.Count() - 1;
                    Weichen_Init = true;
                }
                else
                {
                    Pointer_Weichenliste--;
                }
                

                GetSignalStatus(Signalliste[Pointer_Signalliste].Name);
                if (Pointer_Signalliste <= 0)
                {
                    Pointer_Signalliste = Signalliste.Count() - 1;
                    Signal_Init = true;
                }
                else
                {
                    Pointer_Signalliste--;
                }
                if (Weichen_Init & Signal_Init)
                {
                    SetConnect(true, true); //Initialisierung abgeschlossen
                    Betriebsbereit = true;
                }
                //
                Fahrstrassenupdate(Gleis1_nach_Block1);
                Fahrstrassenupdate(Gleis2_nach_Block1);
                Fahrstrassenupdate(Gleis3_nach_Block1);
                Fahrstrassenupdate(Gleis4_nach_Block1);
                Fahrstrassenupdate(Gleis5_nach_Block1);
                Fahrstrassenupdate(Gleis6_nach_Block1);

                Fahrstrassenupdate(Block2_nach_Gleis1);
                Fahrstrassenupdate(Block2_nach_Gleis2);
                Fahrstrassenupdate(Block2_nach_Gleis3);
                Fahrstrassenupdate(Block2_nach_Gleis4);
                Fahrstrassenupdate(Block2_nach_Gleis5);
                Fahrstrassenupdate(Block2_nach_Gleis6);

                Fahrstrassenupdate(Gleis1_nach_rechts1);
                Fahrstrassenupdate(Gleis2_nach_rechts1);
                Fahrstrassenupdate(Gleis3_nach_rechts1);
                Fahrstrassenupdate(Gleis4_nach_rechts1);
                Fahrstrassenupdate(Gleis5_nach_rechts1);
                Fahrstrassenupdate(Gleis6_nach_rechts1);

                Fahrstrassenupdate(Gleis1_nach_rechts2);
                Fahrstrassenupdate(Gleis2_nach_rechts2);
                Fahrstrassenupdate(Gleis3_nach_rechts2);
                Fahrstrassenupdate(Gleis4_nach_rechts2);
                Fahrstrassenupdate(Gleis5_nach_rechts2);
                Fahrstrassenupdate(Gleis6_nach_rechts2);

                Fahrstrassenupdate(Rechts1_nach_Gleis1);
                Fahrstrassenupdate(Rechts1_nach_Gleis2);
                Fahrstrassenupdate(Rechts1_nach_Gleis3);
                Fahrstrassenupdate(Rechts1_nach_Gleis4);
                Fahrstrassenupdate(Rechts1_nach_Gleis5);
                Fahrstrassenupdate(Rechts1_nach_Gleis6);

                Fahrstrassenupdate(Rechts2_nach_Gleis1);
                Fahrstrassenupdate(Rechts2_nach_Gleis2);
                Fahrstrassenupdate(Rechts2_nach_Gleis3);
                Fahrstrassenupdate(Rechts2_nach_Gleis4);
                Fahrstrassenupdate(Rechts2_nach_Gleis5);
                Fahrstrassenupdate(Rechts2_nach_Gleis6);

                Fahrstrassenupdate(Block1_nach_Block2);
                Fahrstrassenupdate(Block1_nach_Block5);
                Fahrstrassenupdate(Block5_nach_Block6);
                Fahrstrassenupdate(Block8_nach_Block6);
                Fahrstrassenupdate(Block9_nach_Block2);

                Fahrstrassenupdate(Block6_nach_Schatten8);
                Fahrstrassenupdate(Block6_nach_Schatten9);
                Fahrstrassenupdate(Block6_nach_Schatten10);
                Fahrstrassenupdate(Block6_nach_Schatten11);

                Fahrstrassenupdate(Schatten8_nach_Block7);
                Fahrstrassenupdate(Schatten9_nach_Block7);
                Fahrstrassenupdate(Schatten10_nach_Block7);
                Fahrstrassenupdate(Schatten11_nach_Block7);

                Fahrstrassenupdate(Block7_nach_Schatten0);
                Fahrstrassenupdate(Block7_nach_Schatten1);
                Fahrstrassenupdate(Block7_nach_Schatten2);
                Fahrstrassenupdate(Block7_nach_Schatten3);
                Fahrstrassenupdate(Block7_nach_Schatten4);
                Fahrstrassenupdate(Block7_nach_Schatten5);
                Fahrstrassenupdate(Block7_nach_Schatten6);
                Fahrstrassenupdate(Block7_nach_Schatten7);

                Fahrstrassenupdate(Schatten0_nach_Block8);
                Fahrstrassenupdate(Schatten1_nach_Block8);
                Fahrstrassenupdate(Schatten1_nach_Block9);
                Fahrstrassenupdate(Schatten2_nach_Block9);
                Fahrstrassenupdate(Schatten3_nach_Block9);
                Fahrstrassenupdate(Schatten4_nach_Block9);
                Fahrstrassenupdate(Schatten5_nach_Block9);
                Fahrstrassenupdate(Schatten6_nach_Block9);
                Fahrstrassenupdate(Schatten7_nach_Block9);

                FahrstrasseBildUpdate();
                //if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) AutoSignalUpdate();
            }

        }
        private void WeichenCooldown(Object source, ElapsedEventArgs e)
        {
            if (z21Start.Verbunden())
            {
                foreach (Weiche weiche in Weichenliste)
                {
                    if (weiche.ZeitAktiv > 0)
                    {
                        weiche.ZeitAktiv -= 100;
                        bool Abzweig = weiche.Abzweig;
                        if (weiche.Spiegeln) Abzweig = !Abzweig;
                        z21Start.Z21_SET_TURNOUT(weiche.Adresse, Abzweig, true, true);

                        if (weiche.ZeitAktiv <= 0)
                        {
                            weiche.ZeitAktiv = 0;   
                            
                            
                            if (Betriebsbereit)
                            {
                                Log.Info("Deaktiviere Weichenausgang");
                                z21Start.Z21_SET_TURNOUT(weiche.Adresse, Abzweig, true, false); //Q-Modus aktiviert, Schaltausgang inaktiv
                            }
                        }
                    }
                }
            }
        }

        private void BelegtmelderCooldown(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                foreach(Belegtmelder belegtmelder in Belegtmelderliste)
                {
                    belegtmelder.CoolDown(250);
                }
            }
        }

        

        #endregion

        #region Weichensteuerung
        private void Weiche_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox weichenElement)
            {
                ToggleWeiche(weichenElement.Name);
            }
        }     
        private void DKW7_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW7_2");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW7_1");
            }

        }
        private void DKW9_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW9_2");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW9_1");
            }

        }
        private void KW22_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
                if (ListID == -1) return;
                if (!Weichenliste[ListID].Abzweig) ToggleWeiche("KW22_1");     //Nur Schalten wenn andere Zunge nicht auf Abzweig
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
                if (ListID == -1) return;
                if (Weichenliste[ListID].Abzweig) ToggleWeiche("KW22_2");     //Nur Schalten wenn andere Zunge nicht auf Abzweig
            }
        }
        private void DKW24_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW24_1");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                ToggleWeiche("DKW24_2");
            }
        }
        #endregion

        #region SignalSteuerung
        private void Signal_HP0_HP1(object sender, EventArgs e)
        {
            if (sender is PictureBox SignalElement)
            {
                int ListID = Signalliste.IndexOf(new Signal() { Name = SignalElement.Name });
                if (ListID == -1) return;

                int ErlaubteSignalstellung = AllowedSignalPos(SignalElement.Name);

                if (Signalliste[ListID].Zustand == Signal.HP1)
                {
                    Signalliste[ListID].Schalten(Signal.HP0, z21Start);
                }
                else if (Signalliste[ListID].Zustand == Signal.HP0)
                {
                    if (ErlaubteSignalstellung == Signal.HP0) return; //Keine Schalterlaubnis, solange für Signal nur HP0 erlaubt ist.
                    Signalliste[ListID].Schalten(Signal.HP1, z21Start);
                }
            }
        }
        private void Signal_HP0_HP2(object sender, EventArgs e)
        {
            if (sender is PictureBox SignalElement)
            {
                int ListID = Signalliste.IndexOf(new Signal() { Name = SignalElement.Name });
                if (ListID == -1) return;

                int ErlaubteSignalstellung = AllowedSignalPos(SignalElement.Name);

                if (Signalliste[ListID].Zustand == 2)
                {
                    Signalliste[ListID].Schalten(Signal.HP0, z21Start);
                }
                else if (Signalliste[ListID].Zustand == 0)
                {
                    if (ErlaubteSignalstellung == Signal.HP0) return; //Keine Schalterlaubnis, solange für Signal nur HP0 erlaubt ist.
                    Signalliste[ListID].Schalten(Signal.HP2, z21Start);
                }
            }
        }
        #endregion

        #region Lok Kontrolle
        private void LokKontroll_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox Namensfeld)
            {
                //Name der Instanz finden
                string name = Namensfeld.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Name") return; //Muss auf _Name enden

                //Passendes Adressfeld heraussuchen
                NumericUpDown Adressfeld = (NumericUpDown)this.Controls.Find(subs[0] + "_Adr", true)[0];
                if (Adressfeld == null) return; //Nicht gefunden: Abbrechen

                //Ausgewählte Lok heraussuchen
                int ListID = Lokliste.IndexOf(new Lok() { Name = Namensfeld.Text });
                if (ListID == -1) return;
                int Adresse = Lokliste[ListID].Adresse;

                Adressfeld.Value = Adresse;
            }
        }
        private void LokKontroll_Adr_ValueChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown Adressfeld)
            {
                //Name der Instanz finden
                string name = Adressfeld.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Adr") return; //Muss auf _Adr enden

                //Passendes Namensfeld finden
                ComboBox Namensfeld = (ComboBox)this.Controls.Find(subs[0] + "_Name", true)[0];
                if (Namensfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Gattungsfeld finden
                ComboBox Gattungsfeld = (ComboBox)this.Controls.Find(subs[0] + "_Gattung", true)[0];
                if (Gattungsfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Steuertypfeld finden
                Button Steuertypfeld = (Button)this.Controls.Find(subs[0] + "_Strg_Typ", true)[0];
                if (Steuertypfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Ruffeld finden
                TextBox Ruffeld = (TextBox)this.Controls.Find(subs[0] + "_Ruf", true)[0];
                if (Ruffeld == null) return; //Nicht gefunden: Abbrechen

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
                }
                else
                {
                    return;
                }

                int LokListenIndex = Lokliste.FindIndex(x => x.Adresse == Adressfeld.Value);                   //Finde Lok mit dieser Adresse 
                if (LokListenIndex == -1)//Lok nicht gefunden in der Liste
                {
                    Namensfeld.Text = String.Format("Lok: {0}", Adressfeld.Value);
                    AktiveLoks[index] = new Lok() { Adresse = (int)Adressfeld.Value };
                }
                else
                {
                    AktiveLoks[index] = Lokliste[LokListenIndex];
                    Namensfeld.Text = AktiveLoks[index].Name;
                    Gattungsfeld.Text = AktiveLoks[index].Gattung;
                }
                AktiveLoks[index].Automatik = false;
                Steuertypfeld.Text = "Manuell";
                Steuertypfeld.BackColor = Color.FromArgb(0, 128, 0);

                if (AktiveLoks[index].Adresse != 0) Ruffeld.Text = LokKontrolle.Abkuerzung(Gattungsfeld.Text) + Adressfeld.Value.ToString();
                else Ruffeld.Text = "";
            }
        }
        private void LokKontroll_Update_Rufnummern(object sender, EventArgs e)
        {
            if (sender is ComboBox Gattungsfeld)
            {
                //Name der Instanz finden
                string name = Gattungsfeld.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Gattung") return; //Muss auf _Gattung enden

                //Passendes Ruffeld finden
                TextBox Ruffeld = (TextBox)this.Controls.Find(subs[0] + "_Ruf", true)[0];
                if (Ruffeld == null) return; //Nicht gefunden: Abbrechen

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
                }
                else
                {
                    return;
                }

                AktiveLoks[index].Gattung = Gattungsfeld.Text;
                if (AktiveLoks[index].Adresse != 0) Ruffeld.Text = LokKontrolle.Abkuerzung(AktiveLoks[index].Gattung) + AktiveLoks[index].Adresse.ToString();
                else Ruffeld.Text = "";
            }
        }
        private void LokKontroll_OpenFahrpult_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (!(Weichen_Init & Signal_Init))
                {
                    MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //Name der Instanz finden
                string name = button.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "OpenFahrpult") return; //Muss auf _Name enden

                //Passendes Adressfeld heraussuchen
                NumericUpDown Adressfeld = (NumericUpDown)this.Controls.Find(subs[0] + "_Adr", true)[0];
                if (Adressfeld == null) return; //Nicht gefunden: Abbrechen

                if (Adressfeld.Value == 0)
                {
                    MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
                }
                else
                {
                    return;
                }
                if (!AktiveLoks[index].Steuerpult.IsDisposed)
                {
                    AktiveLoks[index].Steuerpult.Dispose();
                }
                AktiveLoks[index].Steuerpult = new ZugSteuerpult(AktiveLoks[index]);
                AktiveLoks[index].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
                AktiveLoks[index].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                AktiveLoks[index].Register_CMD_LOKSTATUS(Setze_Lok_Status);
                AktiveLoks[index].Steuerpult.Show();
                z21Start.Z21_GET_LOCO_INFO(AktiveLoks[index].Adresse);

            }
        }
        private void LokKontroll_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (sender is Button steuertyp)
            {
                //Name der Instanz finden
                string name = steuertyp.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Strg") return; //Muss auf _Strg an zweiter Position haben

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
                }
                else
                {
                    return;
                }

                if (!AktiveLoks[index].Automatik)
                {
                    steuertyp.Text = "Automatik";
                    steuertyp.BackColor = Color.FromArgb(0, 0, 255);
                    AktiveLoks[index].Automatik = true;
                }
                else
                {
                    steuertyp.Text = "Manuell";
                    steuertyp.BackColor = Color.FromArgb(0, 128, 0);
                    AktiveLoks[index].Automatik = false;
                }
            }
        }
        private void LokKontroll_Stop_Click(object sender, EventArgs e)
        {
            if (sender is Button stop)
            {
                //Name der Instanz finden
                string name = stop.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Stop") return; //Muss auf _Strg an zweiter Position haben

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
                }
                else
                {
                    return;
                }
                if (AktiveLoks[index].Adresse != 0)
                {
                    Setze_Lok_Fahrt(AktiveLoks[index].Adresse, 255, AktiveLoks[index].Richtung, AktiveLoks[index].FahrstufenInfo);
                }
            }
        }
        #endregion

        

        private void StopAlle_Click(object sender, EventArgs e)
        {
            foreach(Lok lok in AktiveLoks)
            {
                if (lok.Adresse != 0)
                {
                    Setze_Lok_Fahrt(lok.Adresse, 255, lok.Richtung, lok.FahrstufenInfo);
                }
            }
        }      
        private void AutoSignale_CheckedChanged(object sender, EventArgs e)
        {
            if(sender is CheckBox checkBox)
            {
                if(checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(0, 0, 255);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                }
            }
        }
        private void SperrungSh2_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.Image = MEKB_H0_Anlage.Properties.Resources.SH_2;
                }
                else
                {
                    checkBox.Image = MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv;
                }
            }
        }

        private void SignalFahrstrasse_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(0, 0, 255);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    checkBox.Text = "über Fahrstrasse";
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(0, 64, 0);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    checkBox.Text = "über Weichen";
                }
            }
        }

        private void signalsteuergungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signal_Einstellungen = new Signal_Einstellungen();
            signal_Einstellungen.Show();
        }

        private void belegtmeldungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(Belegtmelderliste);
            belegtmelder_Ueberwachung.Show();
        }
    }
}
