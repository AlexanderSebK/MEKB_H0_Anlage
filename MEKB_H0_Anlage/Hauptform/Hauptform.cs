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
using System.Threading;


namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Hauptform
    /// </summary>
    public partial class Hauptform : Form
    {
        public Z21 z21Start;
        private Z21_Einstellung z21_Einstellung;
        private Signal_Einstellungen signal_Einstellungen;
        private Belegtmelder_Ueberwachung belegtmelder_Ueberwachung;
        private InfoBox InfoBox;
        private MenuFenster_Signalistentool signaltool;

        public GleisbildZeichnung GleisbildZeichnung = new GleisbildZeichnung("Standard.png");

        public WeichenListe WeichenListe = new WeichenListe("Weichenliste.xml");
        public SignalListe SignalListe = new SignalListe("Signalliste.xml");      
        public BelegtmelderListe BelegtmelderListe = new BelegtmelderListe("Belegtmelderliste.xml");
        public FahrstrassenListe FahrstrassenListe = new FahrstrassenListe();
        public LokomotivenVerwaltung LokomotivenArchiv = new LokomotivenVerwaltung("LokArchiv");


        public Lokomotive[] AktiveLoks = new Lokomotive[12];
        public Thread ThreadLoksuche;

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
        public Hauptform()
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

            Betriebsbereit = false;

            ThreadLoksuche = new Thread(() => DialogHandhabungLokSuche(""));

            for (int i = 0; i < AktiveLoks.Length; i++)
            {
                AktiveLoks[i] = new Lokomotive();
                AktiveLoks[i].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                AktiveLoks[i].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            }
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
            FlagTimer.Stop();
            WeichenTimer.Stop();
            CooldownTimer.Stop();
            BelegtmelderCoolDown.Stop();

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
                    updateRegisterState(NextBlock, belegtmelder.NaechsterBlock(VorBlock.Text, WeichenListe));
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

       

        private void button1_Click(object sender, EventArgs e)
        {
            string naechsterBlock = NextBlock.Text;
            string aktuellerBlock = Block.Text;
            Block.Text = naechsterBlock;
            VorBlock.Text = aktuellerBlock;
        }

        private void signaleEditierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signaltool = new MenuFenster_Signalistentool("Signalliste.xml");
            signaltool.Show();
        }
    }
}
