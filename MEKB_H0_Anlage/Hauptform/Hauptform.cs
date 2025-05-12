/***************************************************************
  Datei:      Hauptform.cs
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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
//using static System.Net.Mime.MediaTypeNames;


namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Hauptform
    /// </summary>
    public partial class Hauptform : Form
    {
        #region Instanzen
        public Z21 z21Start;
        public GleisbildZeichnung GleisbildZeichnung = GleisbildZeichnung.Instance;

        public AktiveLokomotiven AktiveLokomotiven = AktiveLokomotiven.Instance;
        public Bahnhofsansage Bahnhofsansage = new Bahnhofsansage();
        public Fehlermeldung Fehlermeldungen = Fehlermeldung.Instance;
        public Einstellungen Einstellungen = Einstellungen.Instance;
        private Logger Log { set; get; }
        #endregion

        #region Fenster
        private Z21_Einstellung z21_Einstellung;
        private Signal_Einstellungen signal_Einstellungen;
        private Belegtmelder_Ueberwachung belegtmelder_Ueberwachung;
        private Weichen_Ueberwachung weichen_Ueberwachung;
        private InfoBox InfoBox;
        private MenuFenster_Signalistentool signaltool;
        private Zugmenue ZugmenueFenster = new Zugmenue();
        #endregion

        #region Listen
        public Gleisplan Plan = new Gleisplan();
        public WeichenListe WeichenListe = WeichenListe.Instance;
        public SignalListe SignalListe = SignalListe.Instance;
        public BelegtmelderListe BelegtmelderListe = BelegtmelderListe.Instance;
        public FahrstrassenListe FahrstrassenListe = FahrstrassenListe.Instance;
        public LokomotivenVerwaltung LokomotivenArchiv = new LokomotivenVerwaltung("LokArchiv");

        public Systemzustand Systemzustand = Systemzustand.Instance;

        public ZuganzeigerListe ZuganzeigerListe = ZuganzeigerListe.Instance;

        private List<string> SperrButtons = new List<string>();
        #endregion

        #region Threads
        #endregion

        #region Timer
        private static System.Timers.Timer HeartbeatTimer;
        private static System.Timers.Timer WeichenTimer;
        private static System.Timers.Timer BelegtmelderCoolDown;
        private static System.Timers.Timer UpdateLokStatus;
        #endregion





        private bool InitIsRunning = false; //Thread für Inittialisierung läuft aber noch nicht abgeschlossen. Verhindert Doppelte Ausführung
        
        public bool Z21_Initialisiert;

        public readonly int Max_Loks = 12;

        public readonly int Timer_BelegtMeldung_ms = 200;
        public readonly int Timer_WeichenUpdate_ms = 50;



        #region Hauptform Funktionen
        public Hauptform()
        {
            Log = new Logger(String.Format("log/log{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

            InitializeComponent();                      //Programminitialisieren
            Plan.ControlsZuweisen(this.GleisplanAnzeige.Controls);
            Einstellungen.LadeEinstellungen();
            Z21_Initialisieren();
            GleisbildZeichnung.ImportiereZeichenDesign("Standard.png");


            if (Einstellungen.AutoSignal) SetCheckBoxColor(AutoSignale, "Blau");  
            else SetCheckBoxColor(AutoSignale, "Grau");
            AutoSignale.Checked = Einstellungen.AutoSignal;

            if (Einstellungen.AutoFahrdienstleister) SetCheckBoxColor(AutoFahrdienstleiter, "Orange");
            else SetCheckBoxColor(AutoFahrdienstleiter, "Grau");
            AutoFahrdienstleiter.Checked = Einstellungen.AutoFahrdienstleister;

            if (Einstellungen.AutoFahrplan) SetCheckBoxColor(AutoFahrplan, "Grün");
            else SetCheckBoxColor(AutoFahrplan, "Grau");
            AutoFahrplan.Checked = Einstellungen.AutoFahrplan;

            if (Einstellungen.Bahnhofsansagen) SetCheckBoxColor(AutoBahnhofsansagen, "Lila");
            else SetCheckBoxColor(AutoBahnhofsansagen, "Grau");
            AutoBahnhofsansagen.Checked = Einstellungen.Bahnhofsansagen;

            if (Einstellungen.AutoNotbremse) SetCheckBoxColor(LokKontrolle, "Cyan");
            else SetCheckBoxColor(LokKontrolle, "Grau");
            LokKontrolle.Checked = Einstellungen.AutoNotbremse;




            if (!Config.ReadConfig("LetzteAnlage").Equals("Not Found"))
            {
                Gleisplan_Laden(Config.ReadConfig("LetzteAnlage"));
            }
            Fehlermeldungen.Register_Fehlermelden(FehlerMelden);
            Fehlermeldungen.Register_Fehlerentfernen(FehlerEntfernen);
            Fehlermeldungen.Register_FehlertextEntfernen(FehlertextEntfernen);

            LokListe_Laden();

            ZugmenueFenster = new Zugmenue(z21Start, LokomotivenArchiv, Bahnhofsansage, AktiveLokomotiven.Liste, BelegtmelderListe);

            
        }
        private void Form1_Shown(object sender, EventArgs e)
        {

            // 5 Sekunden Timer einrichten (Lebenspuls für die Verbindung)
            HeartbeatTimer = new System.Timers.Timer(5000);
            // Timer mit Funktion "Z21_Heartbeat" Verbinden
            HeartbeatTimer.Elapsed += Z21_Heartbeat;
            HeartbeatTimer.AutoReset = true;


            // 100 MilliSekunden Timer: Weichen und Fahrstraßen Update.
            WeichenTimer = new System.Timers.Timer(50);
            // Timer mit Funktion "OnTimedWeichenEvent" Verbinden
            WeichenTimer.Elapsed += OnTimedWeichenEvent;
            WeichenTimer.AutoReset = true;


            // 250 MilliSekunden Timer: Deaktivieren der Weichenmotoren.
            BelegtmelderCoolDown = new System.Timers.Timer(Timer_BelegtMeldung_ms);
            // Timer mit Funktion "WeichenCooldown" Verbinden
            BelegtmelderCoolDown.Elapsed += BelegtmelderCooldown;
            BelegtmelderCoolDown.AutoReset = true;

            // 100 MilliSekunden Timer: StatusUpdateLok.
            UpdateLokStatus = new System.Timers.Timer(200);
            // Timer mit Funktion "OnStatusUpdate" Verbinden
            UpdateLokStatus.Elapsed += OnStatusUpdate;
            UpdateLokStatus.AutoReset = true;

            //Gleisplan zeichnen
            Plan.ZeichnenInitial();
            ZuganzeigerListe.ZeichneZuganzeigen(this.GleisplanAnzeige.Controls);

            //Sofort verbinden wenn Optionen das erlauben
            if (Config.ReadConfig("Auto_Connect").Equals("true"))
            {
                z21Start.Connect_Z21();   //Wenn "Auto_Connect" gesetzt ist: Verbinden
                // Background-Prozess für Initialisierung starten
                Thread trd = new Thread(new ThreadStart(this.WeichenSignalInit))
                {
                    IsBackground = true
                };
                trd.Start();
            }

            // Timer aktivieren
            HeartbeatTimer.Enabled = true;
            WeichenTimer.Enabled = true;
            BelegtmelderCoolDown.Enabled = true;
            UpdateLokStatus.Enabled = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HeartbeatTimer.Stop();
            WeichenTimer.Stop();
            BelegtmelderCoolDown.Stop();

            StopAlle_Click(sender, e);
            LokListe_Speichern();
            foreach (Signal signal in SignalListe.Liste)
            {
                signal.Schalten(SignalZustand.HP0); //Alle Signale Rot
            }
            z21Start.DisConnect_Z21();
        }
        private void Hauptform_SizeChanged(object sender, EventArgs e)
        {
            GleisplanAnzeige.Size = new Size(GleisplanAnzeige.Size.Width, this.Size.Height - 200);
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
            Thread trd = new Thread(new ThreadStart(this.WeichenSignalInit))
            {
                IsBackground = true
            };
            trd.Start();
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
        private void SignalsteuergungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signal_Einstellungen = new Signal_Einstellungen();
            signal_Einstellungen.Show();
        }
        private void BelegtmeldungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (belegtmelder_Ueberwachung == null) belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(BelegtmelderListe);
            if (belegtmelder_Ueberwachung.IsDisposed) belegtmelder_Ueberwachung = new Belegtmelder_Ueberwachung(BelegtmelderListe);
            belegtmelder_Ueberwachung.Show();
            belegtmelder_Ueberwachung.BringToFront();
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

        #endregion

        #region Timer Funktionen
        private void Z21_Heartbeat(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                z21Start.Z21_SET_BROADCASTFLAGS(Einstellungen.Z21Flags);

                // Wenn noch nicht initialisiert: Thread für Initialisierung starten
                if (!Z21_Initialisiert)
                {
                    if (InitIsRunning) return; //Läuft bereits, nicht ausführen
                    Thread trd = new Thread(new ThreadStart(this.WeichenSignalInit))
                    {
                        IsBackground = true
                    };
                    trd.Start();
                }
            }

        }

        private void UpdateRegisterState(System.Windows.Forms.TextBox Box, string text)
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

                if (z21Start.Verbunden())
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    FahrstrassenListe.Fahrstrassenupdate();

                    if (Systemzustand.Betriebsbereit && Einstellungen.AutoSignal) 
                        SignalListe.AutoSignal(Einstellungen.AutoSignal, Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"),(int)timer.Interval);

                    SignalListe.VorsignaleSchalten();
                    try
                    {
                        Plan.GleisplanZeichnen();
                    }
                    catch
                    {
                        stopWatch.Stop();
                        return;
                    }
                    
                    
                    //GleisplanUpdateSignal();
                    stopWatch.Stop();
                    return;
                    //Messung vor Verbesserung: 100~120ms => 2ms
                }
            }

        }

        int BelegtmelderGruppenIndex = 0;
        /// <summary>
        /// Cooldown Timer für Belegtmelder. Zeit in der gemessen wird ob das Signal stabil ist, erst dann wird der Wert übernommen
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void BelegtmelderCooldown(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                BelegtmelderListe.CoolDownUpdate(Timer_BelegtMeldung_ms);
                //if (BelegtmelderGruppenIndex == 0)  { z21Start.LAN_RMBUS_GETDATA(0x00); BelegtmelderGruppenIndex = 1; }
                //else                                { z21Start.LAN_RMBUS_GETDATA(0x01); BelegtmelderGruppenIndex = 0; }
            }
        }

        private void OnStatusUpdate(Object source, ElapsedEventArgs e)
        {
            if (!Systemzustand.Betriebsbereit) return;
            

            if (source is System.Timers.Timer timer)
            {
                this.BeginInvoke((Action<bool>) ZuganzeigenAktualisieren, false);

                // Lokstatus abfragen
                if (AktiveLokomotiven.Liste.Count == 0) return;
                List<int> StatusAdressen = new List<int>();
                foreach (Lokomotive lokomotive in AktiveLokomotiven.Liste)
                {
                    lokomotive.BlockVerfolgung(LokKontrolle.Checked, true);

                    if (LokKontrolle.Checked) lokomotive.NotBremseHandeln((int)timer.Interval);
                    StatusAdressen.Add(lokomotive.Adresse);
                }

                z21Start.Z21_GET_LOCO_INFO(StatusAdressen);
            }
        }
        private void ZuganzeigenAktualisieren(bool ErzwingeUpdate = false)
        {
            ZuganzeigerListe.ZuganzeigenAktualisieren(this.GleisplanAnzeige.Controls, ErzwingeUpdate);
        }


        #endregion

        #region Lok Steuerung
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Lok-Geschwindigkeit setzen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        /// <param name="Fahrstufe">Aktuelle Fahrstufe</param>
        /// <param name="Richtung">Fahrtrichtung</param>
        /// <param name="Fahstrufeninfo">14,28 oder 128 Fahrstufen</param>
        private void Setze_Lok_Fahrt(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            z21Start.Z21_SET_LOCO_DRIVE(Adresse, Fahrstufe, Richtung, Fahstrufeninfo);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Lok-Funktion setzen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        /// <param name="Zustand">Funktion an oder ausschalten</param>
        /// <param name="FunktionsNr">Funktionsnummer</param>
        private void Setze_Lok_Funktion(int Adresse, byte Zustand, byte FunktionsNr)
        {
            z21Start.Z21_SET_LOCO_FUNCTION(Adresse, Zustand, FunktionsNr);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Lok-Status abrufen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        private void Setze_Lok_Status(int Adresse)
        {
            z21Start.Z21_GET_LOCO_INFO(Adresse);
        }

        /// <summary>
        /// Alle Lokomotiven anhalten
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void StopAlle_Click(object sender, EventArgs e)
        {
            foreach (Lokomotive lok in AktiveLokomotiven.Liste)
            {
                if (lok.Adresse != 0)
                {
                    Setze_Lok_Fahrt(lok.Adresse, 255, lok.Richtung, lok.FahrstufenInfo);
                }
            }
        }
        #endregion

        #region Unterfunktionen
        /// <summary>
        /// Initialisierungs Routine für Signale und Weichen. 
        /// Alle Weichen werden abgefragt
        /// Alle Signale werdne auf HP0 gesetzt
        /// Belegtmeldergroupen werden abgefragt
        /// </summary>
        private void WeichenSignalInit()
        {
            if (InitIsRunning) return; //Läuft bereits, nicht ausführen
            InitIsRunning = true;

            // Max. Wartezeit bis sich die Z21 verbunden hat in [ms]
            int timeoutVerbinden = 1000;

            while (!z21Start.Verbunden())
            {
                Thread.Sleep(100);
                timeoutVerbinden -= 100;
                if (timeoutVerbinden <= 0)
                {
                    InitIsRunning = false;
                    Fehlermeldungen.FehlerMelden("Z21-Zentrale nicht gefunden", "Error");
                    return; // Hat sich nicht verbunden -> Task beenden
                }
            }
            Fehlermeldungen.FehlerEntfernen("Z21-Zentrale nicht gefunden");
            // Z21 ist verbunden
            if (z21Start.Verbunden())
            { 
                // Broadcast-Nachrichten setzen
                z21Start.Z21_SET_BROADCASTFLAGS(Einstellungen.Z21Flags);

                Thread.Sleep(100);
                // Alle Weichen abfragen
                WeichenListe.WeichenStatus("Alle");
                Thread.Sleep(100);

                // Alle Belegtmeldergruppen abfragen
                BelegtmelderListe.StatusAnfordernBelegtmelder(z21Start, 0);
                Thread.Sleep(100);
                BelegtmelderListe.StatusAnfordernBelegtmelder(z21Start, 1);
                Thread.Sleep(100);

                // Alle Signale auf HP0 setzen
                foreach (Signal signal in SignalListe.Liste)
                {
                    signal.Schalten(SignalZustand.HP0);
                    Thread.Sleep(100);
                }
                SetConnect(true, true); //Initialisierung abgeschlossen
                Systemzustand.Betriebsbereit = true;
            }
            InitIsRunning = false; // Prozess beendet
        }

        /// <summary>
        /// Z21 Verbindung initialisieren
        /// Callbackfunktionen zuornden
        /// Instanzzugriffe setzen
        /// Einstellungen übernehmen
        /// </summary>
        private void Z21_Initialisieren()
        {
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

            z21Start.SetQMode(true); //Queue Mode bei Weichen aktivieren

            
            z21Start.SetIP_Z21(Einstellungen.Z21_IP,Einstellungen.Z21_Port);

            z21_Einstellung = new Z21_Einstellung();    //Neues Fenster: Einstellung der Z21 (Läuft im Hintergund)
            z21_Einstellung.Get_Z21_Instance(z21Start);     //Z21-Verbindung dem neuen Fenster mitgeben

            ConnectStatus(false, false);                 //Verbindungsstatus initialisieren
            Systemzustand.Betriebsbereit = false;
        }

        private void LokListe_Laden()
        {
            for (int i = 0; i < Max_Loks; i++)
            {
                string LokAdresse = Config.ReadConfig(String.Format("LokListe{0}", i));
                if (int.TryParse(LokAdresse, out int DigitAdresse))
                {
                    if (DigitAdresse == 0) continue;
                    if(!LokomotivenArchiv.SucheDurchAdresse(DigitAdresse, out Lokomotive lokomotive))
                    {
                        lokomotive = new Lokomotive() //Blanke Lok anlegen mit dieser Adresse
                        {
                            Adresse = DigitAdresse,
                            Name = String.Format("Lok: {0}", DigitAdresse)
                        };
                    }
                    
                    
                    lokomotive.Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                    lokomotive.Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
                    lokomotive.Register_CMD_LOKSTATUS(Setze_Lok_Status);

                    lokomotive.VorherigerBlock = "";
                    lokomotive.AktuellerBlock = "";

                    string VorherigePosition = Config.ReadConfig(String.Format("LokPosVor{0}", i));
                    Belegtmelder VorBlock = BelegtmelderListe.GetBelegtmelder(VorherigePosition);
                    if (VorBlock != null)
                    {
                        VorBlock.Registriert = "Deregistriert";
                        lokomotive.VorherigerBlock = VorherigePosition;
                    }
                        
                    string Position = Config.ReadConfig(String.Format("LokPos{0}", i));
                    Belegtmelder AktPosition = BelegtmelderListe.GetBelegtmelder(Position);
                    if (AktPosition != null)
                    {
                        lokomotive.AktuellerBlock = Position;
                        AktPosition.Registriert = lokomotive.Name;
                    }
                    else
                    {
                        lokomotive.AktuellerBlock = "";
                        lokomotive.VorherigerBlock = "";
                    }

                    AktiveLokomotiven.Liste.Add(lokomotive);
                    
                }
            }
        }

        private void LokListe_Speichern()
        {
            for(int i = 0;i < Max_Loks; i++)
            {
                if(i <  AktiveLokomotiven.Liste.Count)
                {
                    Config.WriteConfig(String.Format("LokListe{0}", i), AktiveLokomotiven.Liste[i].Adresse.ToString());
                    Config.WriteConfig(String.Format("LokPos{0}",i), AktiveLokomotiven.Liste[i].AktuellerBlock.ToString());
                    Config.WriteConfig(String.Format("LokPosVor{0}",i), AktiveLokomotiven.Liste[i].VorherigerBlock.ToString());
                }
                else
                {
                    Config.WriteConfig(String.Format("LokListe{0}", i), "0");
                    Config.WriteConfig(String.Format("LokPos{0}", i), "");
                    Config.WriteConfig(String.Format("LokPosVor{0}", i), "");
                }
            }
        }
        
        private void Gleisplan_Laden(string Dateiname)
        {
            GleisbildZeichnung.GleisZustand.Clear();
            Plan.DateiImportieren(Dateiname);
            WeichenListe.DateiImportieren(Dateiname);
            SignalListe.DateiImportieren(Dateiname);
            BelegtmelderListe.DateiImportieren(Dateiname);
            FahrstrassenListe.DateiImportieren(Dateiname, WeichenListe, SignalListe);
            ZuganzeigerListe.DateiImportieren(Dateiname);


            SignalListe.ListenZugriff(FahrstrassenListe, BelegtmelderListe, WeichenListe);
            BelegtmelderListe.SignalZugriff(SignalListe);
            ZuganzeigerListe.BelegtmelderVerknuepfen(BelegtmelderListe);



            // Instanzzugriffe auf Zentrale
            WeichenListe.DigitalzentraleZugriff(z21Start);
            SignalListe.DigitalzentraleZugriff(z21Start);
        }

        private void Gleisplan_Loeschen()
        {
            Plan = new Gleisplan();
            Plan.ControlsZuweisen(this.GleisplanAnzeige.Controls);
            WeichenListe.Clear();
            SignalListe.Clear();
            BelegtmelderListe = new BelegtmelderListe();
            FahrstrassenListe.Clear();
            GleisbildZeichnung.GleisZustand.Clear();
        }

       
        #endregion

        #region Schnellzugriff (obere Zeile)
        /// <summary>
        /// Farbe bei aktivierung anpassen (blau)
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void AutoSignale_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                Einstellungen.AutoSignal = checkBox.Checked;
                if (checkBox.Checked == true) SetCheckBoxColor(checkBox, "Blau");
                else SetCheckBoxColor(checkBox, "Grau");
                Einstellungen.WriteBoolToConfig("AutoSignal", Einstellungen.AutoSignal);
            }
        }
        /// <summary>
        /// Farbe bei aktivierung anpassen (orange)
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void AutoFahrdienstleiter_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                Einstellungen.AutoFahrdienstleister = checkBox.Checked;
                if (checkBox.Checked == true) SetCheckBoxColor(checkBox, "Orange");
                else SetCheckBoxColor(checkBox, "Grau");
                Einstellungen.WriteBoolToConfig("AutoFahrdienstleister", Einstellungen.AutoFahrdienstleister);
            }
        }
        /// <summary>
        /// Farbe bei aktivierung anpassen (grün)
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void AutoFahrplan_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                Einstellungen.AutoFahrplan = checkBox.Checked;
                if (checkBox.Checked == true) SetCheckBoxColor(checkBox, "Grün");
                else SetCheckBoxColor(checkBox, "Grau");
                Einstellungen.WriteBoolToConfig("AutoFahrplan", Einstellungen.AutoFahrplan);
            }
        }
        /// <summary>
        /// Farbe bei aktivierung anpassen (lila)
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void AutoBahnhofsansagen_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                Einstellungen.Bahnhofsansagen = checkBox.Checked;
                if (checkBox.Checked == true) SetCheckBoxColor(checkBox, "Lila");
                else SetCheckBoxColor(checkBox, "Grau");
                Einstellungen.WriteBoolToConfig("Bahnhofsansagen", Einstellungen.Bahnhofsansagen);
            }
        }

        private void LokKontrolle_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                Einstellungen.AutoNotbremse = checkBox.Checked;
                if (checkBox.Checked == true) SetCheckBoxColor(checkBox, "Cyan");
                else SetCheckBoxColor(checkBox, "Grau");
                Einstellungen.WriteBoolToConfig("AutoNotbremse", Einstellungen.AutoNotbremse);
            }
        }

        private void SetCheckBoxColor(CheckBox checkBox, string farbe)
        {
            switch (farbe)
            {
                case "Blau":
                    checkBox.BackColor = Color.FromArgb(0, 0, 255);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
                case "Grün":
                    checkBox.BackColor = Color.FromArgb(0, 128, 0);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
                case "Lila":
                    checkBox.BackColor = Color.FromArgb(128, 0, 128);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
                case "Orange":
                    checkBox.BackColor = Color.FromArgb(255, 128, 0);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
                case "Cyan":
                    checkBox.BackColor = Color.FromArgb(0, 128, 128);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
                case "Grau":
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Fahrzeugliste aufrufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Fahrzeuge_Click(object sender, EventArgs e)
        {
            

            if (!ZugmenueFenster.IsDisposed)
            {
                ZugmenueFenster.Show();
                ZugmenueFenster.BringToFront();
            }
            else
            {
                ZugmenueFenster = new Zugmenue(z21Start, LokomotivenArchiv, Bahnhofsansage, AktiveLokomotiven.Liste, BelegtmelderListe);
                ZugmenueFenster.Show();
            }
        }





        #endregion

        #region Z21 CallBacks
        /// <summary>
        /// Aufruf bei Fehler in der Nachricht
        /// </summary>
        /// <param name="FehlerCode">FehlerCode</param>
        public void CallBack_Fehler(int FehlerCode)
        {
            this.BeginInvoke((Action<int>)ShowErrorCode, FehlerCode);
        }
        /// <summary>
        /// Änderung des Verbindungsstatus
        /// </summary>
        /// <param name="Status">Neuer Status (true = verbunden)</param>
        public void SetConnect(bool Status, bool Init)
        {
            this.BeginInvoke((Action<bool, bool>)ConnectStatus, Status, Init);
        }
        /// <summary>
        /// CallBack Funktion: Seriennummer 
        /// Wird aufgerufen sobald die SerienNummer von der Z21 empfangen wurde
        /// </summary>
        /// <param name="sn">Seriennummer als Zahl</param>
        public void CallBack_GET_SERIAL_NUMBER(int sn)
        {
            this.BeginInvoke((Action<string>)Set_SerienNummer, sn.ToString());
        }
        /// <summary>
        /// CallBack Funktion: Z21 Firmware
        /// Wird aufgerufen sobald eine Nachricht über die Firmware erhalten wurde
        /// </summary>
        public void CallBack_LAN_X_GET_FIRMWARE_VERSION(double firmware)
        {
            this.BeginInvoke((Action<double>)ShowFirmware, firmware);
        }
        /// <summary>
        /// CallBack Funktion: Z21_Status
        /// Wird aufgerufen sobald eine Statusantwort von der Z21 bezüglich Weichen empfangen wurde
        /// </summary>
        public void CallBack_LAN_X_TURNOUT_INFO(int Adresse, byte Zustand)
        {
            this.BeginInvoke((Action<int, int>)UpdateWeiche, Adresse, Zustand);
        }
        /// <summary>
        /// CallBack Funktion: Z21_Status
        /// Wird aufgerufen sobald eine Braodcast_Flag-Nachricht von der Z21 empfangen wurde
        /// Die Flags geben an welche Änderungen von der Z21 automatisch (ohne anfrage gesendet werden)
        /// </summary>
        /// <param name="newFlags">Struktur mit Flags</param>
        public void CallBack_Z21_Broadcast_Flags(int flags)
        {
            Flags newFlags = new Flags(flags);
            this.BeginInvoke((Action<Flags>)Set_Flags, newFlags);
        }
        /// <summary>
        /// CallBack Funktion: Z21_Status
        /// Wird aufgerufen, sobald eine Statusnachricht von der Z21 empfangen wurde
        /// </summary>
        /// <param name="MainCurrent"></param>
        /// <param name="ProgCurrent"></param>
        /// <param name="MainCurrentFilter"></param>
        /// <param name="Temperatur"></param>
        /// <param name="VersorgungSpg"></param>
        /// <param name="GleisSpg"></param>
        /// <param name="ZentralenStatus"></param>
        /// <param name="ZentralenStatusGrund"></param>
        public void CallBack_Z21_System_Status(int MainCurrent, int ProgCurrent, int MainCurrentFilter, int Temperatur,
                    int VersorgungSpg, int GleisSpg, byte ZentralenStatus, byte ZentralenStatusGrund)
        {
            this.BeginInvoke((Action<int, int, int>)Set_Z21_Strom, MainCurrent, ProgCurrent, MainCurrentFilter);
            this.BeginInvoke((Action<int, int>)Set_Z21_Spannung, VersorgungSpg, GleisSpg);
            this.BeginInvoke((Action<int>)Set_Z21_Temperatur, Temperatur);
            this.BeginInvoke((Action<int, int>)Set_Gleistatus, ZentralenStatus, ZentralenStatusGrund);
        }

        public void CallBack_Z21_LokUpdate(int ParamterCount, int Addresse, bool Besetzt, byte FahrstufenInfo, bool Richtung,
                                             byte Fahrstufe, bool Doppeltraktio, bool Smartsearch, bool[] Funktionen)
        {
            this.BeginInvoke((Action<int, int, bool, byte, bool, byte, bool, bool, bool[]>)UpdateLok, ParamterCount, Addresse, Besetzt, FahrstufenInfo, Richtung,
                                              Fahrstufe, Doppeltraktio, Smartsearch, Funktionen);
        }

        public void CallBack_Z21_TrackStatus(byte status)
        {

        }
        /// <summary>
        /// CallBack-Funktion 
        /// Wird gesendet, wenn sich der Belegtmelderstatus geändert hat
        /// </summary>
        /// <param name="GruppenIndex">0 oder 1 (Gruppen Index)</param>
        /// <param name="RMStatus">Bit Array Belegtmeldungen</param>
        public void CallBack_LAN_RMBUS_DATACHANGED(byte GruppenIndex, byte[] RMStatus)
        {
            this.BeginInvoke((Action<byte, byte[]>)UpdateBelegtmeldung, GruppenIndex, RMStatus);
        }
        #region Invokes
        private void Set_SerienNummer(string data)
        {
            z21_Einstellung.Set_SerienNummer(data);
        }
        private void ShowFirmware(double Firmware)
        {
            if (!z21_Einstellung.IsDisposed)
            {
                z21_Einstellung.SetFirmware(Firmware.ToString("F2", CultureInfo.CreateSpecificCulture("en-GB")));
            }
        }
        private void Set_Flags(Flags flags)
        {
            if (!z21_Einstellung.IsDisposed)
            {
                z21_Einstellung.SetFlags(flags);
            }
        }
        public void ShowErrorCode(int Code)
        {
            switch (Code)
            {
                case 1:
                //FehlerCode.Text = "Falsche Länge"; break;
                case 3:
                //FehlerCode.Text = "Falsche CheckSumme"; break;
                default: break;
            }

        }
        /// <summary>
        /// Aktuellen Verbindungsstatus festlegen und anziegen
        /// </summary>
        /// <param name="status">WAHR: Z21 ist verbunden</param>
        /// <param name="init">WAHR: Z21 wurde bereits initialisiert</param>
        public void ConnectStatus(bool status, bool init)
        {
            Menu_Trennen.Enabled = status;
            Menu_Verbinden.Enabled = !status;
            if (status)
            {
                if (init)
                {
                    Z21_Initialisiert = true;
                    HauptStatusbar.Text = "Z21: Verbunden";
                    HauptStatusbar.BackColor = Color.ForestGreen;
                    HauptStatusbar.ForeColor = Color.White;
                }
                else
                {
                    Z21_Initialisiert = false;
                    HauptStatusbar.Text = "Z21: Initialisieren";
                    HauptStatusbar.BackColor = Color.Gold;
                    HauptStatusbar.ForeColor = Color.Black;
                }
            }
            else
            {
                HauptStatusbar.Text = "Z21: Getrennt";
                HauptStatusbar.BackColor = Color.Red;
                HauptStatusbar.ForeColor = Color.White;
            }
            z21_Einstellung.ConnectStatus(status);
        }
        private void Set_Z21_Strom(int Main, int Prog, int MainFilter)
        {
            StatusBarStrom.Text = String.Format("Stromverbauch: {0} mA", MainFilter);
        }
        /// <summary>
        /// Spannungsversorgung Z21 
        /// </summary>
        /// <param name="Versorgung">Versorgungsspannung Z21</param>
        /// <param name="Gleis">Versgungspannung Gleisspannung (Ausgang Z21)</param>
        private void Set_Z21_Spannung(int Versorgung, int Gleis)
        {
            StatusBarSpg.Text = String.Format("Gleisspannung: {0} mV", Gleis);
        }
        private void Set_Z21_Temperatur(int Temperatur)
        {

        }
        private void Set_Gleistatus(int Status, int Grund)
        {
            if (Status == 0x00)
            {
                TrackStatus.Text = "Strecke: In Betrieb";
                TrackStatus.BackColor = Color.ForestGreen;
                TrackStatus.ForeColor = Color.White;
                Systemzustand.Betriebsbereit = true;
                Fehlermeldungen.FehlerEntfernen("Kein Strom");
                Fehlermeldungen.FehlerEntfernen("Stoptaste wurde gedrückt");
                Fehlermeldungen.FehlerEntfernen("Kurzschluss auf der Strecke");
                Fehlermeldungen.FehlerEntfernen("Zentrale in Programmiermodus");
            }
            if ((Status & 0x02) == 0x02)
            {
                TrackStatus.Text = "Strecke: Kein Strom";
                TrackStatus.BackColor = Color.Gold;
                TrackStatus.ForeColor = Color.Black;
                Systemzustand.Betriebsbereit = false;
                Fehlermeldungen.FehlerMelden("Kein Strom", "Warning");
            }
            if ((Status & 0x01) == 0x01)
            {
                TrackStatus.Text = "Strecke: Nothalt";
                TrackStatus.BackColor = Color.Orange;
                TrackStatus.ForeColor = Color.Black;
                Systemzustand.Betriebsbereit = false;
                Fehlermeldungen.FehlerMelden("Stoptaste wurde gedrückt", "Warning");
            }
            if ((Status & 0x04) == 0x04)
            {
                TrackStatus.Text = "Strecke: Kurzschluss";
                TrackStatus.BackColor = Color.Red;
                TrackStatus.ForeColor = Color.White;
                Systemzustand.Betriebsbereit = false;
                Fehlermeldungen.FehlerMelden("Kurzschluss auf der Strecke", "Error");
            }
            if ((Status & 0x20) == 0x20)
            {
                TrackStatus.Text = "Programmiermodus";
                TrackStatus.BackColor = Color.Blue;
                TrackStatus.ForeColor = Color.White;
                Systemzustand.Betriebsbereit = false;
                Fehlermeldungen.FehlerMelden("Zentrale in Programmiermodus", "Hint");
            }
        }
        /// <summary>
        /// Invoke-Funktion
        /// Ausgeführt, wenn neues Packet mit Signal/Weichen Status empfangen wird
        /// </summary>
        /// <param name="Adresse">Adresse des Betroffenen Signals/Weiche</param>
        /// <param name="Status">neuer Status</param>
        private void UpdateWeiche(int Adresse, int Status)
        {
            Weiche weiche = WeichenListe.GetWeiche(Adresse); //Finde Weiche mit dieser Adresse 
            if (weiche != null)//Weiche gefunden in der Liste
            {
                weiche.StatusUpdate(Status);
            }
            else
            {
                // Versuche Signalzustand zu aktualisieren
                if (SignalListe.UpdateSignalZustand(Adresse, Status))
                {
                    // Update erfolgreich (inkl. Signal gefunden)
                    SignalListe.GetSignal(Adresse).UpdateNoetig = true;
                }
            }
        }

        private void UpdateLok(int ParamterCount, int Adresse, bool Besetzt, byte FahrstufenInfo, bool Richtung,
                                             byte Fahrstufe, bool Doppeltraktio, bool Smartsearch, bool[] Funktionen)
        {

            int ListID = AktiveLokomotiven.Liste.FindIndex(x => x.Adresse == Adresse); //Finde Lok mit dieser Adresse 
            if (ListID == -1)//Lok nicht gefunden in der Liste
            {
                return;
            }

            AktiveLokomotiven.Liste[ListID].UpdateZ21Data(ParamterCount, FahrstufenInfo, Richtung, Fahrstufe, Funktionen);

        }
        private void UpdateBelegtmeldung(byte GruppenIndex, byte[] RMStatus)
        {
            BelegtmelderListe.UpdateBelegtmelder(GruppenIndex, RMStatus);
            Plan.UpdateFahrstrassenSchalter(1);
        }
        #endregion
        #endregion

        #region Fehlermeldung

        private void FehlerMeldenInvoke(string text, string typ)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (FehlerListe.FindItemWithText(text) != null) return; //Fehlermeldung bereits vorhanden

            if (string.IsNullOrEmpty(typ)) typ = "Error";
            int ImageIndex = 0;

            switch (typ)
            {
                case "Error": ImageIndex = 0; break;
                case "Warning": ImageIndex = 1; break;
                default: ImageIndex = 0; break;
            }

            FehlerListe.Items.Add(text, ImageIndex);
        }
        public void FehlerMelden(string text, string typ)
        {
            this.BeginInvoke((Action<string,string>)FehlerMeldenInvoke,text,typ);  
        }

        private void FehlerEntfernenInvoke(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            ListViewItem FehlermeldungEintrag = FehlerListe.FindItemWithText(text);
            if (FehlermeldungEintrag != null)
            {
                FehlerListe.Items.Remove(FehlermeldungEintrag);
            }
        }
        public void FehlerEntfernen(string text)
        {
            this.BeginInvoke((Action<string>)FehlerEntfernenInvoke, text);
        }

        private void FehlertextEntfernenInvoke(string lokname)
        {
            if (string.IsNullOrEmpty(lokname)) return;
            foreach (ListViewItem listViewItem in FehlerListe.Items)
            {
                if(listViewItem.Text.Contains(lokname))
                {
                    FehlerListe.Items.Remove(listViewItem);
                }
            }
        }
        public void FehlertextEntfernen(string lokname)
        {
            this.BeginInvoke((Action<string>)FehlertextEntfernenInvoke, lokname);
        }


        #endregion

        private void weichenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (weichen_Ueberwachung == null) weichen_Ueberwachung = new Weichen_Ueberwachung(WeichenListe.Liste);
            if (weichen_Ueberwachung.IsDisposed) weichen_Ueberwachung = new Weichen_Ueberwachung(WeichenListe.Liste);
            weichen_Ueberwachung.Show();
            weichen_Ueberwachung.BringToFront();
        }

        
        /// <summary>
        /// Neue Anlage laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gleisplanLadenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Anlagen (*.xml) |*.xml";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Timer deaktivieren
                    WeichenTimer.Enabled = false;
                    BelegtmelderCoolDown.Enabled = false;
                    UpdateLokStatus.Enabled = false;
                    //Gleisplan löschen
                    Gleisplan_Loeschen();
                    this.GleisplanAnzeige.Controls.Clear();
                    this.GleisplanAnzeige.Refresh();

                    string datei = openFileDialog.FileName;
                    Gleisplan_Laden(datei);

                    Config.WriteConfig("LetzteAnlage", datei);

                    //Gleisplan zeichnen
                    Plan.ZeichnenInitial();
                    ZuganzeigerListe.ZeichneZuganzeigen(this.GleisplanAnzeige.Controls);

                    // Timer aktivieren
                    WeichenTimer.Enabled = true;
                    BelegtmelderCoolDown.Enabled = true;
                    UpdateLokStatus.Enabled = true;
                    Thread trd = new Thread(new ThreadStart(this.WeichenSignalInit))
                    {
                        IsBackground = true
                    };
                    trd.Start();
                }
            }
            
        }
    }
}
