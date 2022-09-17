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
using System.Diagnostics;


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

        public GleisbildZeichnung GleisbildZeichnung = new GleisbildZeichnung("Standard.png");

        public WeichenListe WeichenListe = new WeichenListe("Weichenliste.xml");
        public SignalListe SignalListe = new SignalListe("Signalliste.xml");      
        public BelegtmelderListe BelegtmelderListe = new BelegtmelderListe("Belegtmelderliste.xml");
        public FahrstrassenListe FahrstrassenListe = new FahrstrassenListe();
        public LokomotivenListe LokomotivenListe = new LokomotivenListe("LokArchiv");


        public List<Lokomotive> Lokliste = new List<Lokomotive>();
        public Lokomotive[] AktiveLoks = new Lokomotive[12];
        
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
            z21Start.Register_LAN_RMBUS_DATACHANGED(CallBack_LAN_RMBUS_DATACHANGED);

            z21_Einstellung = new Z21_Einstellung();    //Neues Fenster: Einstellung der Z21 (Läuft im Hintergund)
            z21_Einstellung.Get_Z21_Instance(this);     //Z21-Verbindung dem neuen Fenster mitgeben

            WeichenListe.DigitalzentraleVerknuepfen(z21Start);

            ConnectStatus(false, false);                 //Verbindungsstatus auf getrennt setzen

            SetupFahrstrassen();                        //Fahstrassen festlegen          
            SetupLokListe();                            //Lok-Daten aus Dateien laden

            Betriebsbereit = false;

            LokCtrl_LoklisteAusfuellen();               //Auswahlliste im Lok-Kontrollfenster ausfüllen
            for (int i = 0; i < AktiveLoks.Length; i++)
            {
                AktiveLoks[i] = new Lokomotive();
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
            

            Pointer_Weichenliste = WeichenListe.Liste.Count() - 1;
            Pointer_Signalliste = SignalListe.Liste.Count() - 1;
            Signal_Init = false;
            Weichen_Init = false;

            // 5 Sekunden Timer einrichten (Lebenspuls für die Verbindung)
            FlagTimer = new System.Timers.Timer(5000);
            // Timer mit Funktion "Z21_Heartbeat" Verbinden
            FlagTimer.Elapsed += Z21_Heartbeat;
            FlagTimer.AutoReset = true;
            

            // 100 MilliSekunden Timer: Weichen und Fahrstraßen Update.
            WeichenTimer = new System.Timers.Timer(50);
            // Timer mit Funktion "OnTimedWeichenEvent" Verbinden
            WeichenTimer.Elapsed += OnTimedWeichenEvent;
            WeichenTimer.AutoReset = true;
            

            // 100 MilliSekunden Timer: Deaktivieren der Weichenmotoren.
            CooldownTimer = new System.Timers.Timer(100);
            // Timer mit Funktion "WeichenCooldown" Verbinden
            CooldownTimer.Elapsed += WeichenCooldown;
            CooldownTimer.AutoReset = true;
            

            // 250 MilliSekunden Timer: Deaktivieren der Weichenmotoren.
            BelegtmelderCoolDown = new System.Timers.Timer(250);
            // Timer mit Funktion "WeichenCooldown" Verbinden
            BelegtmelderCoolDown.Elapsed += BelegtmelderCooldown;
            BelegtmelderCoolDown.AutoReset = true;
            

            if (Config.ReadConfig("Auto_Connect").Equals("true")) z21Start.Connect_Z21();   //Wenn "Auto_Connect" gesetzt ist: Verbinden

            FlagTimer.Enabled = true;
            WeichenTimer.Enabled = true;
            CooldownTimer.Enabled = true;
            BelegtmelderCoolDown.Enabled = true;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAlle_Click(sender, e);
            foreach (Signal signal in SignalListe.Liste)
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
            Pointer_Weichenliste = WeichenListe.Liste.Count() - 1;
            Pointer_Signalliste = SignalListe.Liste.Count() - 1;
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
        public delegate void InvokeDelegate();


        private void updateRegisterState(TextBox Box, string text)
        {
            Box.Invoke((MethodInvoker)(() =>
            {
                Box.Text = text;
            }));
        }

        private void OnTimedWeichenEvent(Object source, ElapsedEventArgs e)
        {
            if (source is System.Timers.Timer timer)
            {

                Belegtmelder belegtmelder = BelegtmelderListe.GetBelegtmelder(Block.Text);
                if(belegtmelder != null)
                {
                    updateRegisterState(NextBlock, belegtmelder.NaechsterBlock(true, WeichenListe, KommeVon.Text));
                    updateRegisterState(VorBlock, belegtmelder.NaechsterBlock(false, WeichenListe, KommeVon.Text));

                }


                if (z21Start.Verbunden())
                {
                    timer.Stop();
                    WeichenListe.WeichenStatus(WeichenListe.Liste[Pointer_Weichenliste].Name);
                    if (Pointer_Weichenliste <= 0)
                    {
                        Pointer_Weichenliste = WeichenListe.Liste.Count() - 1;
                        Weichen_Init = true;
                    }
                    else
                    {
                        Pointer_Weichenliste--;
                    }

                    GetSignalStatus_Z21(SignalListe.Liste[Pointer_Signalliste].Name);
                    if (Pointer_Signalliste <= 0)
                    {
                        Pointer_Signalliste = SignalListe.Liste.Count() - 1;
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
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    foreach(Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
                    {
                        Fahrstrassenupdate(fahrstrasse);
                    }
                    
                    FahrstrasseBildUpdate();
                    BelegtmelderListe.StatusAnfordernBelegtmelder(z21Start, 0);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    timer.Start();
                    //Messung vor Verbesserung: 100~120ms => 2ms
                }
            }

        }
        private void WeichenCooldown(Object source, ElapsedEventArgs e)
        {
            if (z21Start.Verbunden())
            {
                if (Betriebsbereit)
                {
                    WeichenListe.WeichenschaltungsUeberwachung(100);
                }
            }
        }

        private void BelegtmelderCooldown(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                BelegtmelderListe.CoolDownUpdate(250);
            }
        }

        

        #endregion

        #region Weichensteuerung
        private void Weiche_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                WeichenListe.ToggleWeiche(weichenElement.Name);
            }
        }     
        private void DKW7_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW7_2");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW7_1");
            }

        }
        private void DKW9_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW9_2");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW9_1");
            }

        }
        private void KW22_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                Weiche weiche = WeichenListe.GetWeiche("KW22_2");
                if (weiche == null) return;
                if (weiche.Abzweig) WeichenListe.ToggleWeiche("KW22_1");     //Nur Schalten wenn andere Zunge auf Abzweig
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                Weiche weiche = WeichenListe.GetWeiche("KW22_1");
                if (weiche == null) return;
                if (!weiche.Abzweig) WeichenListe.ToggleWeiche("KW22_2");     //Nur Schalten wenn andere Zunge nicht auf Abzweig
            }
        }
        private void DKW24_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW24_1");
            }
            else                //Auf linke Hälfte der Weiche geklickt
            {
                WeichenListe.ToggleWeiche("DKW24_2");
            }
        }
        #endregion

        #region SignalSteuerung
        private void Signal_HP0_HP1(object sender, EventArgs e)
        {
            if (sender is PictureBox SignalElement)
            {
                Signal signal = SignalListe.GetSignal(SignalElement.Name);
                if (signal == null) return;

                SignalZustand ErlaubteSignalstellung = AllowedSignalPos(SignalElement.Name);

                if (signal.Zustand == SignalZustand.HP1)
                {
                    signal.Schalten(SignalZustand.HP0, z21Start);
                }
                else if (signal.Zustand == SignalZustand.HP0)
                {
                    if (ErlaubteSignalstellung == SignalZustand.HP0) return; //Keine Schalterlaubnis, solange für Signal nur HP0 erlaubt ist.
                    signal.Schalten(SignalZustand.HP1, z21Start);
                }
            }
        }
        private void Signal_HP0_HP2(object sender, EventArgs e)
        {
            if (sender is PictureBox SignalElement)
            {
                Signal signal = SignalListe.GetSignal(SignalElement.Name);
                if (signal == null) return;

                SignalZustand ErlaubteSignalstellung = AllowedSignalPos(SignalElement.Name);

                if (signal.Zustand == SignalZustand.HP2)
                {
                    signal.Schalten(SignalZustand.HP0, z21Start);
                }
                else if (signal.Zustand == 0)
                {
                    if (ErlaubteSignalstellung == SignalZustand.HP0) return; //Keine Schalterlaubnis, solange für Signal nur HP0 erlaubt ist.
                    signal.Schalten(SignalZustand.HP2, z21Start);
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
                int ListID = Lokliste.IndexOf(new Lokomotive() { Name = Namensfeld.Text });
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
                    AktiveLoks[index] = new Lokomotive() { Adresse = (int)Adressfeld.Value };
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
            foreach(Lokomotive lok in AktiveLoks)
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

        private void SignalsteuergungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signal_Einstellungen = new Signal_Einstellungen();
            signal_Einstellungen.Show();
        }

        private void BelegtmeldungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(BelegtmelderListe);
            belegtmelder_Ueberwachung.Show();
        }

        private void FahrstrassenButton_Click(object sender, EventArgs e)
        {
            Fahrstrasse fahrstrasse;
            bool Abbau = false;
            if (sender is Button button)
            {
                if (button.Tag == null) return;
                string ZielFahrstrasse = button.Tag.ToString();
                if (ZielFahrstrasse.EndsWith("-"))
                {
                    Abbau = true;
                    ZielFahrstrasse = ZielFahrstrasse.Substring(0, ZielFahrstrasse.Length - 1);
                }
                fahrstrasse = FahrstrassenListe.GetFahrstrasse(ZielFahrstrasse);

                if (fahrstrasse == null) return;
                if (fahrstrasse.Fahrstr_GleicherEingang.Count >= 1)
                {
                    if (FahrstrassenListe.FahrstrasseGleicheGesetzt(fahrstrasse.Name))
                    {
                        foreach (string GruppenItem in fahrstrasse.Fahrstr_GleicherEingang)
                        {
                            Fahrstrasse GruppenFahrstrasse = FahrstrassenListe.GetFahrstrasse(GruppenItem);
                            if (GruppenFahrstrasse.GetGesetztStatus())
                            {
                                ToggleFahrstrasse(GruppenFahrstrasse);
                            }
                        }
                    }
                    else
                    {
                        if (Abbau)
                        {
                            if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                            {
                                ToggleFahrstrasse(fahrstrasse);
                                LoescheButtons(fahrstrasse.Fahrstr_GleicherEingang);
                                Button clickedButton = button;
                                clickedButton.Dispose();
                            }
                        }
                        else
                        {
                            GeneriereButtons(fahrstrasse.Fahrstr_GleicherEingang, button.Location.X, button.Location.Y);
                        }
                    }
                }
                else
                {
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        ToggleFahrstrasse(fahrstrasse);  //Aktiv? auschalten
                    }
                    else
                    {
                        //Keine Sperrende Fahstraße aktiv
                        if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                        {
                            ToggleFahrstrasse(fahrstrasse);
                        }
                    }
                }
            }
        }
        private void GeneriereButtons(List<string> Fahrstrassen, int X, int Y)
        {
            X += 20;

            //Wenn Buttons schon existieren -> löschen
            Control Modul = this.Controls[Fahrstrassen[0] + "_Auswahl"];
            if (Modul is Button button)
            {
                LoescheButtons(Fahrstrassen);
                return;
            }

            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                Button newButton = new Button
                {
                    Name = Fahrstrassenname + "_Auswahl",
                    Tag = Fahrstrassenname + "-",
                    Size = new Size(100, 20),
                    Location = new Point(X, Y),
                    Enabled = !FahrstrassenListe.FahrstrasseBlockiert(Fahrstrassenname)
                };
                Y += 20;
                newButton.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                newButton.BringToFront();

                if (Fahrstrassenname.Contains('_'))
                {
                    string[] text = Fahrstrassenname.Split('_');
                    newButton.Text = text[2];
                }
                else
                {
                    newButton.Text = Fahrstrassenname;
                }

                this.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }
        private void LoescheButtons(List<string> Fahrstrassen)
        {
            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                Control Modul = this.Controls[Fahrstrassenname + "_Auswahl"];
                if (Modul is Button)
                {
                    this.Controls.Remove(Modul);
                }
            }

        }


    }
}
