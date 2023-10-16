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
        #region Instanzen
        public Z21 z21Start;
        public GleisbildZeichnung GleisbildZeichnung = new GleisbildZeichnung("Standard.png");
        public Lokomotive[] AktiveLoks = new Lokomotive[12];
        private Logger Log { set; get; }
        #endregion

        #region Fenster
        private Z21_Einstellung z21_Einstellung;
        private Signal_Einstellungen signal_Einstellungen;
        private Belegtmelder_Ueberwachung belegtmelder_Ueberwachung;
        private InfoBox InfoBox;
        private MenuFenster_Signalistentool signaltool;
        #endregion

        #region Listen
        public Gleisplan Plan = new Gleisplan("Gleisplan.xml");
        public WeichenListe WeichenListe = new WeichenListe("Weichenliste.xml");
        public SignalListe SignalListe = new SignalListe("Signalliste.xml");      
        public BelegtmelderListe BelegtmelderListe = new BelegtmelderListe("Belegtmelderliste.xml");
        public FahrstrassenListe FahrstrassenListe = new FahrstrassenListe();
        public LokomotivenVerwaltung LokomotivenArchiv = new LokomotivenVerwaltung("LokArchiv");
        #endregion

        #region Threads
        public Thread ThreadLoksuche;
        #endregion

        #region Timer
        private static System.Timers.Timer FlagTimer;
        private static System.Timers.Timer WeichenTimer;
        private static System.Timers.Timer CooldownTimer;
        private static System.Timers.Timer BelegtmelderCoolDown;
        #endregion




        

        public bool Betriebsbereit;

        

        private int Pointer_Weichenliste = 0;
        private int Pointer_Signalliste = 0;
        private bool Signal_Init;
        private bool Weichen_Init;

        

        #region Hauptform Funktionen
        public Hauptform()
        {
            Log = new Logger(String.Format("log/log{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

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

            // Instanzen Zugriffe festlegen
            SetupFahrstrassen();                        //Fahstrassen festlegen    
            WeichenListe.DigitalzentraleZugriff(z21Start);
            SignalListe.DigitalzentraleZugriff(z21Start);
            SignalListe.ListenZugriff(FahrstrassenListe, BelegtmelderListe);

            ConnectStatus(false, false);                 //Verbindungsstatus auf getrennt setzen
      

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

            GleisplanZeichnenInitial();

            


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
                signal.Schalten(SignalZustand.HP0); //Alle Signale Rot
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


        private void UpdateRegisterState(TextBox Box, string text)
        {
            Box.Invoke((MethodInvoker)(() =>
            {
                Box.Text = text;
            }));
        }

        private int GroupIndex = 0;
        private void OnTimedWeichenEvent(Object source, ElapsedEventArgs e)
        {
            if (source is System.Timers.Timer timer)
            {

                Belegtmelder belegtmelder = BelegtmelderListe.GetBelegtmelder(Block.Text); //Debug
                if(belegtmelder != null)
                {
                    UpdateRegisterState(NextBlock, belegtmelder.NaechsterBlock(VorBlock.Text, WeichenListe));
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
                    if (Weichen_Init && Signal_Init)
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
                    
                    if (GroupIndex == 0)
                    {
                        //BelegtmelderListe.StatusAnfordernBelegtmelder(z21Start, 0);
                        GroupIndex = 1;
                    }
                    else
                    {
                        //BelegtmelderListe.StatusAnfordernBelegtmelder(z21Start, 1);
                        GroupIndex = 0;
                    }

                    GleisplanZeichnen();
                    if(Betriebsbereit && AutoSignale.Checked) SignalListe.AutoSignal(Config.ReadConfig("AutoSignalFahrt").Equals("true"), Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"));
                    stopWatch.Stop();
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

        private void DKW_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                Gleisplan.Abschnitt.GleisTyp gleis = Plan.SucheGleis(weichenElement.Name);
                if (gleis != null)
                {
                    String[] tags = gleis.Typ.Split('_');
                    MouseEventArgs e2 = (MouseEventArgs)e;
                    switch (tags[1])
                    {
                        case "0":
                        case "45":
                            // Unterer Häfte ist 1. Weiche
                            if(e2.Y > (weichenElement.Height / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "90":
                        case "135":
                            // Linke Hälfte ist 1. Weiche
                            if (e2.X > (weichenElement.Width / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "180":
                        case "225":
                            // Obere Hälfte ist 1. Weiche
                            if (e2.Y <= (weichenElement.Height / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "270":
                        case "315":
                            // Rechte Hälfte ist 1. Weiche
                            if (e2.X <= (weichenElement.Width / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        default: break;
                    }
                }    
            }            
        }

        private void KW_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                Gleisplan.Abschnitt.GleisTyp gleis = Plan.SucheGleis(weichenElement.Name);
                if (gleis != null)
                {
                    MouseEventArgs e2 = (MouseEventArgs)e;
                    if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                        if (weiche == null) return;
                        if (weiche.Abzweig) WeichenListe.ToggleWeiche(gleis.Weiche);     //Nur Schalten wenn andere Zunge auf Abzweig
                    }
                    else                //Auf linke Hälfte der Weiche geklickt
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        if (weiche == null) return;
                        if (!weiche.Abzweig) WeichenListe.ToggleWeiche(gleis.Weiche_2nd);     //Nur Schalten wenn andere Zunge nicht auf Abzweig
                    }
                }
            }
        }


        
        #endregion

        #region SignalSteuerung
        private void Signal_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox PicBox)
            {
                bool Modus = Config.ReadConfig("AutoSignalFahrstrasse").Equals("true");
                if (GetGleisObjekt(PicBox.Name, out Gleisplan.Abschnitt.GleisTyp Gleis))
                {
                    Signal signal = SignalListe.GetSignal(Gleis.Signal);
                    // SHIFT-Taste während des Klickens gedrückt -> Schalten auf HP2 (langsame Fahrt)
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        if (signal.Zustand == SignalZustand.HP2) // Signal bereits auf diesem Zustand  -> auf HP0 schalten
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if(AutoSignale.Checked) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }

                        // HP2 erlaubt
                        if (signal.StellungErlaubt(SignalZustand.HP2, Modus))
                        {
                            signal.Schalten(SignalZustand.HP2);
                            signal.AutoSperre = false; //Signal wieder im Normalen Modus
                            return;
                        }
                        // HP2 nicht erlaubt, prüfen ob HP1 möglich
                        else if (signal.StellungErlaubt(SignalZustand.HP1, Modus))
                        {
                            signal.Schalten(SignalZustand.HP1);
                            signal.AutoSperre = false; //Signal wieder im Normalen Modus
                            return;
                        }
                        else // Weder HP2 noch HP1 erlaubt -> Strecke gesperrt
                        {
                            // Signal auf Rot-Schalten, wenn nicht bereits in diesem Zustand
                            if (signal.Zustand != SignalZustand.HP0)
                                signal.Schalten(SignalZustand.HP0);
                            return;
                        }
                    }
                    // CTRL-Taste während des Klickens gedrückt -> Schalten auf SH1 (Rangier Fahrt)
                    else if (Control.ModifierKeys == Keys.Control)
                    {
                        if (signal.Zustand == SignalZustand.SH1) // Signal bereits auf diesem Zustand  -> auf HP0 schalten
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if (AutoSignale.Checked) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }
                        // SH1 erlaubt
                        if (signal.StellungErlaubt(SignalZustand.SH1, Modus))
                        {
                            signal.Schalten(SignalZustand.SH1);
                            return;
                        }
                        else //Nicht erlaubt -> Strecke gesperrt
                        {
                            // Signal auf Rot-Schalten, wenn nicht bereits in diesem Zustand
                            if (signal.Zustand != SignalZustand.HP0)
                                signal.Schalten(SignalZustand.HP0);
                            return;
                        }
                    }
                    else
                    {
                        // Signal is auf Rot -> Schalten in ein anderes 
                        if (signal.Zustand == SignalZustand.HP0)
                        {
                            if (signal.StellungErlaubt(SignalZustand.HP1, Modus))
                            {
                                signal.Schalten(SignalZustand.HP1);
                                signal.AutoSperre = false; //Signal wieder im Normalen Modus
                                return;
                            }
                            else if (signal.StellungErlaubt(SignalZustand.HP2, Modus))
                            {
                                signal.Schalten(SignalZustand.HP2);
                                signal.AutoSperre = false; //Signal wieder im Normalen Modus
                                return;
                            }
                            else // Weder HP2 noch HP1 erlaubt -> Strecke gesperrt
                            {
                                return; // Schalten nicht erlauben
                            }
                        }
                        else // Zurückschalten auf HP0
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if (AutoSignale.Checked) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }
                    }
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
                    //TODO: aktive Fahrstrasse ausschalten
                }
                else
                {
                    checkBox.Image = MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv;
                }
                UpdateFahrstrassenSchalter();
            }
        }

        private void SignalsteuergungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signal_Einstellungen = new Signal_Einstellungen();
            signal_Einstellungen.Show();
        }

        private void BelegtmeldungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(belegtmelder_Ueberwachung == null) belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(BelegtmelderListe);
            if(belegtmelder_Ueberwachung.IsDisposed) belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(BelegtmelderListe);
            belegtmelder_Ueberwachung.Show();
            belegtmelder_Ueberwachung.BringToFront();
        }

       

        private void Button1_Click(object sender, EventArgs e)
        {
            string naechsterBlock = NextBlock.Text;
            string aktuellerBlock = Block.Text;
            Block.Text = naechsterBlock;
            VorBlock.Text = aktuellerBlock;
        }

        private void SignaleEditierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signaltool = new MenuFenster_Signalistentool("Signalliste.xml");
            signaltool.Show();
        }

        private void LokomotivenNeuLadenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LokomotivenArchiv = new LokomotivenVerwaltung("LokArchiv");
        }

        private void Hauptform_SizeChanged(object sender, EventArgs e)
        {
            GleisplanAnzeige.Size = new Size(GleisplanAnzeige.Size.Width, this.Size.Height - 300);
        }
    }
}
