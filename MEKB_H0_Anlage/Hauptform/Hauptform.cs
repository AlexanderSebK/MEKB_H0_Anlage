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
using System.Globalization;
using System.Runtime.InteropServices;


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
        //public Lokomotive[] AktiveLoks = new Lokomotive[12];
        public List<Lokomotive> LokListe = new List<Lokomotive>();
        public Bahnhofsansage Bahnhofsansage = new Bahnhofsansage();
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
        public Gleisplan Plan = new Gleisplan("Gleisplan.xml");
        public WeichenListe WeichenListe = new WeichenListe("Weichenliste.xml");
        public SignalListe SignalListe = new SignalListe("Signalliste.xml");
        public BelegtmelderListe BelegtmelderListe = new BelegtmelderListe("Belegtmelderliste.xml");
        public FahrstrassenListe FahrstrassenListe = new FahrstrassenListe();
        public LokomotivenVerwaltung LokomotivenArchiv = new LokomotivenVerwaltung("LokArchiv");

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
        public bool Betriebsbereit;
        public bool Z21_Initialisiert;

        public readonly int Max_Loks = 12;

        public readonly int Timer_BelegtMeldung_ms = 200;
        public readonly int Timer_WeichenUpdate_ms = 50;



        #region Hauptform Funktionen
        public Hauptform()
        {
            Log = new Logger(String.Format("log/log{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

            InitializeComponent();                      //Programminitialisieren
            Z21_Initialisieren();

            // Instanzen Zugriffe festlegen
            SetupFahrstrassen();                        //Fahstrassen festlegen              
            SignalListe.ListenZugriff(FahrstrassenListe, BelegtmelderListe, WeichenListe);
            BelegtmelderListe.SignalZugriff(SignalListe);
            LokListe_Laden();

            ZugmenueFenster = new Zugmenue(z21Start, LokomotivenArchiv, Bahnhofsansage, LokListe, BelegtmelderListe);

            
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
            GleisplanZeichnenInitial();

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
                if (!z21_Einstellung.IsDisposed) //Fenster Z21-Einstellung nläuft immer noch im Hintergrund
                {
                    Flags temp = z21_Einstellung.Get_Flag_Config();
                    z21Start.Z21_SET_BROADCASTFLAGS(temp); //Flags neu setzen 
                }
                if (!Z21_Initialisiert)
                {
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

        //private int GroupIndex = 0;
        private void OnTimedWeichenEvent(Object source, ElapsedEventArgs e)
        {
            if (source is System.Timers.Timer)
            {

                if (z21Start.Verbunden())
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
                    {
                        Fahrstrassenupdate(fahrstrasse);
                    }

                    if (Betriebsbereit && AutoSignale.Checked) 
                        SignalListe.AutoSignal(Config.ReadConfig("AutoSignalFahrt").Equals("true"), Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"),50);

                    SignalListe.VorsignaleSchalten();
                    try
                    {
                        GleisplanZeichnen();
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
                if (BelegtmelderGruppenIndex == 0)  { z21Start.LAN_RMBUS_GETDATA(0x00); BelegtmelderGruppenIndex = 1; }
                else                                { z21Start.LAN_RMBUS_GETDATA(0x01); BelegtmelderGruppenIndex = 0; }
            }
        }

        int LokStatusTimerIndex = 0;
        private void OnStatusUpdate(Object source, ElapsedEventArgs e)
        {
            if (!Betriebsbereit) return;
            // Lokstatus abfragen
            if (LokListe.Count == 0) return;

            if (!LokKontrolle.Checked) return;

            foreach(Lokomotive lokomotive in LokListe)
            {
                lokomotive.BlockVerfolgung(BelegtmelderListe, WeichenListe);
                lokomotive.NotBremseHandeln();
            }
            if(!(LokStatusTimerIndex < LokListe.Count)) LokStatusTimerIndex = 0;
            Setze_Lok_Status(LokListe[LokStatusTimerIndex].Adresse);
            LokStatusTimerIndex++;
        }



        #endregion

        #region Gleisplan

        #region Gleisplan Zeichnen
        /// <summary>
        /// Initialen Gleisplan zeichnen. Anschließend nur noch funktion GleisplanZeichen() für updates verwenden
        /// </summary>
        private void GleisplanZeichnenInitial()
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.Bilder bild in abschnitt.BilderListe)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        // Convert Image to byte[]
                        Properties.Resources.Drehscheibe.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        //byte[] imageBytes = ms.ToArray();

                        // Convert byte[] to base 64 string
                        string base64String = Convert.ToBase64String(ms.ToArray());
                    }




                    // Convert base 64 string to byte[]
                    byte[] imageBytes = Convert.FromBase64String(bild.Base64String);
                    // Convert byte[] to Image
                    using (var ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        PictureBox neuesBild = new PictureBox()
                        {
                            Name = bild.Name,
                            Size = new Size(image.Width, image.Height),
                            Location = new Point(bild.PosX * 32, bild.PosY * 32),
                            Image = image
                        };
                        this.GleisplanAnzeige.Controls.Add(neuesBild);
                    }
                }

                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (!this.Controls.ContainsKey(gleis.Name))
                    {
                        PictureBox neuesGleis = new PictureBox()
                        {
                            Name = gleis.Name,
                            Tag = gleis.Typ,
                            Size = new Size(32, 32),
                            Location = new Point(gleis.PosX * 32, gleis.PosY * 32),
                            Image = new Bitmap(GleisbildZeichnung.Katalog[36][7]),
                        };
                        if (!gleis.Weiche.Equals(""))
                        {
                            if (gleis.Typ.StartsWith("DKW")) neuesGleis.Click += new System.EventHandler(this.DKW_Click);
                            else if (gleis.Typ.StartsWith("KW")) neuesGleis.Click += new System.EventHandler(this.KW_Click);
                            //else if (gleis.Typ.StartsWith("Dreiweg")) neuesGleis.Click += new System.EventHandler(this.Dreiweg_Click);
                            else neuesGleis.Click += new System.EventHandler(this.Weiche_Click);
                        }
                        else if (!gleis.Signal.Equals(""))
                        {
                            neuesGleis.Click += new System.EventHandler(this.Signal_Click);
                        }

                        this.GleisplanAnzeige.Controls.Add(neuesGleis);

                        if (gleis.Gleislabel != null)
                        {
                            Label neuesLabel = new Label
                            {
                                AutoSize = true,
                                Text = gleis.Gleislabel.Text,
                                Location = new Point(gleis.PosX * 32 + gleis.Gleislabel.X_Offeset, gleis.PosY * 32 + gleis.Gleislabel.Y_Offeset)
                            };
                            if (gleis.Gleislabel.Fett) neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.Gleislabel.Groesse, FontStyle.Bold);
                            else neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.Gleislabel.Groesse, FontStyle.Regular);
                            neuesLabel.Name = gleis.Name + "Label";
                            if (gleis.Gleislabel.Rahmen) neuesLabel.BorderStyle = BorderStyle.FixedSingle;
                            this.GleisplanAnzeige.Controls.Add(neuesLabel);
                            neuesLabel.BringToFront();
                        }

                        if (!gleis.FahrstrassenButton.Equals(""))
                        {
                            Button button = new Button
                            {
                                Name = gleis.FahrstrassenButton + "_Button",
                                Size = new System.Drawing.Size(16, 16),
                                BackColor = System.Drawing.Color.Yellow,
                                Margin = new System.Windows.Forms.Padding(0),
                                BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center,
                                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                                ForeColor = System.Drawing.SystemColors.ControlText,
                                Tag = gleis.FahrstrassenButton,
                                UseVisualStyleBackColor = false,
                            };
                            button.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                            button.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
                            button.FlatAppearance.BorderSize = 0;
                            button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
                            button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;

                            bool Buttonzeichen = true;
                            string Winkel = GetButtonWinkel(gleis.Typ, gleis.ButtonDrehen);

                            if (Winkel.Equals("0"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_oben;
                                button.BackgroundImage.Tag = "oben";
                                button.Location = new Point(gleis.PosX * 32 + 8, gleis.PosY * 32 + 16);
                            }
                            else if (Winkel.Equals("90"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_links;
                                button.BackgroundImage.Tag = "links";
                                button.Location = new Point(gleis.PosX * 32 + 16, gleis.PosY * 32 + 8);
                            }
                            else if (Winkel.Equals("180"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_unten;
                                button.BackgroundImage.Tag = "unten";
                                button.Location = new Point(gleis.PosX * 32 + 8, gleis.PosY * 32 + 0);
                            }
                            else if (Winkel.Equals("270"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_rechts;
                                button.BackgroundImage.Tag = "rechts";
                                button.Location = new Point(gleis.PosX * 32 + 0, gleis.PosY * 32 + 8);
                            }
                            else
                            {
                                Buttonzeichen = false; // Button nicht setzen;
                            }
                            if (Buttonzeichen)
                            {
                                this.GleisplanAnzeige.Controls.Add(button);
                                button.BringToFront();
                            }
                        }

                        if (gleis.SperrButton)
                        {
                            CheckBox SHO_Sperrung = new CheckBox
                            {
                                Name = gleis.Name + "_SHO",
                                Appearance = System.Windows.Forms.Appearance.Button,
                                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                                Image = global::MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv,
                                Location = new System.Drawing.Point(gleis.PosX * 32 + 0, gleis.PosY * 32 + 8),
                                Size = new System.Drawing.Size(32, 16),
                                UseVisualStyleBackColor = true
                            };
                            SHO_Sperrung.CheckedChanged += new System.EventHandler(this.SperrungSh2_CheckedChanged);
                            SHO_Sperrung.Tag = String.Join("+", gleis.GesperrteFahrstrassen);
                            this.GleisplanAnzeige.Controls.Add(SHO_Sperrung);
                            SHO_Sperrung.BringToFront();
                            SperrButtons.Add(SHO_Sperrung.Name);
                        }

                        if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, FreiesGleis, neuesGleis);
                        }
                        else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, neuesGleis);
                        }
                        else if (gleis.Bedingung[0] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, neuesGleis);
                        }
                        else if (!gleis.Weiche.Equals(""))
                        {
                            Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                            if (gleis.Weiche_2nd.Equals(""))
                            {
                                if (gleis.Bedingung[1] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, neuesGleis);
                                }
                                else if (gleis.Bedingung[2] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, FreiesGleis, neuesGleis);
                                }
                            }
                            else
                            {
                                Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, neuesGleis);
                            }
                        }
                        if (!gleis.Signal.Equals(""))
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(SignalListe.GetSignal(gleis.Signal), neuesGleis);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subfunktion: Winkel des Buttons an hand des Gleistyps berechnen
        /// </summary>
        /// <param name="GleisTyp">Gleistyp</param>
        /// <param name="drehen">Auf welcher Seite des Gleises ist der Button</param>
        /// <returns>Winkel des Buttons oder "nicht erlaubt" wenn unzulässig</returns>
        private string GetButtonWinkel(string GleisTyp, bool drehen)
        {
            string Typ = GleisTyp.Split('+').First();
            string Winkel = Typ.Split('_').Last();
            Typ = Typ.Split('_').First();

            if (Typ.Equals("KurveL") && drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "nicht erlaubt"; break;
                    case "45": Winkel = "270"; break;
                    case "90": Winkel = "nicht erlaubt"; break;
                    case "135": Winkel = "0"; break;
                    case "180": Winkel = "nicht erlaubt"; break;
                    case "225": Winkel = "90"; break;
                    case "270": Winkel = "nicht erlaubt"; break;
                    case "315": Winkel = "180"; break;
                    default: Winkel = "nicht erlaubt"; break;
                }
            }
            else if (Typ.Equals("KurveR") && drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "nicht erlaubt"; break;
                    case "45": Winkel = "180"; break;
                    case "90": Winkel = "nicht erlaubt"; break;
                    case "135": Winkel = "270"; break;
                    case "180": Winkel = "nicht erlaubt"; break;
                    case "225": Winkel = "0"; break;
                    case "270": Winkel = "nicht erlaubt"; break;
                    case "315": Winkel = "90"; break;
                    default: Winkel = "nicht erlaubt"; break;
                }
            }


            else if (drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "180"; break;
                    case "90": Winkel = "270"; break;
                    case "180": Winkel = "0"; break;
                    case "270": Winkel = "90"; break;
                    default: break;
                }
            }

            return Winkel;
        }

        /// <summary>
        /// Alle Gleise zeichnen. Inklusive abfrage ob neuzeichnen notwendig ist
        /// </summary>
        private void GleisplanZeichnen()
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                // Aktueller Zustand der Gleise in diesem Abschnitt
                Dictionary<int, MeldeZustand> aktZustand = new Dictionary<int, MeldeZustand>();

                // Aktuellen Zustand berechnen
                foreach (Gleisplan.Abschnitt.StatusBedingung bedingung in abschnitt.Bedingungen)
                {
                    bool BelegtmelderStatus = false;
                    if (bedingung.Aktiv.Count > 0)
                    {
                        foreach (Dictionary<string, bool> entry in bedingung.Aktiv)
                        {
                            bool AND_bedingung = true;
                            foreach (KeyValuePair<string, bool> Weichenentry in entry)
                            {
                                bool WeichenAbzweig = WeichenListe.GetWeiche(Weichenentry.Key).Abzweig;
                                if (WeichenAbzweig != Weichenentry.Value) AND_bedingung = false;
                            }
                            if (AND_bedingung) BelegtmelderStatus = true;
                        }
                    }
                    else
                    {
                        BelegtmelderStatus = true; // Keine Bedingung - immer anzeigen
                    }
                    // Wenn Belegtmelderstatus gewünscht ist
                    if (BelegtmelderStatus) BelegtmelderStatus = BelegtmelderListe.GetBelegtStatus(bedingung.Belegtmelder);

                    aktZustand.Add(bedingung.Nummer, ErrechneZustand(
                            BelegtmelderStatus,
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenMit.ToArray()),
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenGegen.ToArray())));
                }



                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    bool update = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (gleis.Bedingung[i] == 0) continue; // Keine Bedingung
                        if (!gleis.Zustand[i].Equals(aktZustand[gleis.Bedingung[i]])) update = true; //Gezeichnete Bedingung mit aktueller vergleichen
                        if (!gleis.Signal.Equals("")) //Signal vorhanden
                        {
                            if (!SignalListe.GetSignal(gleis.Signal).Zustand.Equals(gleis.SignalZustand)) update = true;
                            else update = true;
                        }
                    }

                    if (update) //Nur neu Zeichnen wenn Unterschied vorhanden
                    {
                        var Fund = this.GleisplanAnzeige.Controls.Find(gleis.Name, true); //Bildelement zum Gleis finden
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox) // Element gefunden
                            {
                                switch (GetGleisBezeichnung(gleis))
                                {
                                    case GleistypBezeichnung.Gleis:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.GleisSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Gleis1Ecke:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.Gleis1EckeSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Gleis2Ecken:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            aktZustand[gleis.Bedingung[2]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.Gleis2EckenSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            aktZustand[gleis.Bedingung[2]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Weiche1Motor:
                                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                                        if (!gleis.WeichenBelegtmelder.Equals(""))
                                        {
                                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                                            {
                                                weiche.Besetzt = aktZustand[index].Besetzt;
                                            }
                                        }
                                        if (gleis.Bedingung[1] == 0)
                                        {
                                            GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox);
                                        }
                                        else if (gleis.Bedingung[2] == 0)
                                        {
                                            GleisbildZeichnung.ZeichneSchaltbild(weiche, aktZustand[gleis.Bedingung[1]], Picbox, true);
                                        }
                                        break;
                                    case GleistypBezeichnung.Weiche2Motoren:
                                        Weiche weiche1 = WeichenListe.GetWeiche(gleis.Weiche);
                                        Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                        if (!gleis.WeichenBelegtmelder.Equals(""))
                                        {
                                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                                            {
                                                weiche1.Besetzt = aktZustand[index].Besetzt;
                                                weiche2.Besetzt = aktZustand[index].Besetzt;
                                            }
                                        }

                                        GleisbildZeichnung.ZeichneSchaltbild(weiche1, weiche2, Picbox);
                                        break;
                                    default: break;
                                }
                                // Aktuellen Zustand als gezeichneten Zustand übernehmen
                                for (int i = 0; i < 3; i++)
                                {
                                    if (gleis.Bedingung[i] == 0) continue; // Keine Bedingung
                                    gleis.Zustand[i] = aktZustand[gleis.Bedingung[i]];
                                }

                            } // if picturebox
                        } // foreach Control
                    } // if(update)

                    if (!gleis.Weiche.Equals("") && gleis.Weiche_2nd.Equals(""))
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        if (!gleis.WeichenBelegtmelder.Equals(""))
                        {
                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                            {
                                weiche.Besetzt = aktZustand[index].Besetzt;
                            }
                        }
                        var Fund = this.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                if (gleis.Bedingung[1] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox, true);
                                }
                                else if (gleis.Bedingung[2] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, aktZustand[gleis.Bedingung[1]], Picbox, true);
                                }
                            }
                        }
                    }
                    else if (!gleis.Weiche.Equals("") && !gleis.Weiche_2nd.Equals(""))
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                        if (!gleis.WeichenBelegtmelder.Equals(""))
                        {
                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                            {
                                weiche.Besetzt = aktZustand[index].Besetzt;
                                weiche2.Besetzt = aktZustand[index].Besetzt;
                            }
                        }
                        var Fund = this.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, Picbox, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gleisobjekt anhand des Namens suchen
        /// </summary>
        /// <param name="name">Name des Gleises was gesucht werden soll</param>
        /// <param name="Gleis">Output: Instanz des Gleises</param>
        /// <returns>wahr wenn Gleis gefunden wurde</returns>
        private bool GetGleisObjekt(string name, out Gleisplan.Abschnitt.GleisTyp Gleis)
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (gleis.Name.Equals(name))
                    {
                        Gleis = gleis;
                        return true;
                    }
                }
            }
            Gleis = null;
            return false;
        }


        /// <summary>
        /// Gleistypen
        /// </summary>
        enum GleistypBezeichnung
        {
            Gleis,
            GleisSignal,
            Gleis1Ecke,
            Gleis1EckeSignal,
            Gleis2Ecken,
            Gleis2EckenSignal,
            Weiche1Motor,
            Weiche2Motoren,
            Error
        }
        /// <summary>
        /// Gleistyp ermitteln
        /// </summary>
        /// <param name="gleis">Gleis</param>
        /// <returns>Typ des Gleis</returns>
        private GleistypBezeichnung GetGleisBezeichnung(Gleisplan.Abschnitt.GleisTyp gleis)
        {
            if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.Gleis2EckenSignal;
                else return GleistypBezeichnung.Gleis2Ecken;
            }
            else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.Gleis1EckeSignal;
                else return GleistypBezeichnung.Gleis1Ecke;
            }
            else if (gleis.Bedingung[0] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.GleisSignal;
                else return GleistypBezeichnung.Gleis;
            }
            else if (!gleis.Weiche.Equals("") && gleis.Weiche_2nd.Equals("")) return GleistypBezeichnung.Weiche1Motor;
            else if (!gleis.Weiche.Equals("") && !gleis.Weiche_2nd.Equals("")) return GleistypBezeichnung.Weiche2Motoren;

            return GleistypBezeichnung.Error;
        }



        MeldeZustand FreiesGleis = new MeldeZustand(false);
        #endregion

        #region Fahrstraßen bestimmen
        /// <summary>
        /// Generiere Statuskonstrukt aus Fahrstraßen und Belegtmeldung
        /// </summary>
        /// <param name="besetzt">Belegtstatus des Abschnitts</param>
        /// <param name="Fahrstrassen_west">Fahrstraßen, die diesen Abschnitt nach westen(links) belegen </param>
        /// <param name="Fahrstrassen_ost">Fahrstraßen, die diesen Abschnitt nach osten(rechts) belegen</param>
        /// <returns></returns>
        private MeldeZustand ErrechneZustand(bool besetzt, List<Fahrstrasse> Fahrstrassen_west, List<Fahrstrasse> Fahrstrassen_ost)
        {
            //Zählvariablen für die Fahrstraßen
            int aktiv_west = 0; //Anzahl aktive Fahrstraßen nach westen
            int aktiv_ost = 0; //Anzahl aktive Fahrstraßen nach osten
            int safe_west = 0; //Anzahl sichere Fahrstraßen nach westen
            int safe_ost = 0; //Anzahl sichere Fahrstraßen nach osten

            //Zwischenvariablen Zustände
            bool richtung = false;
            bool fahrstrasseAktiv = false;
            bool sicher = false;

            //Zählen der Fahrstraßen
            foreach (Fahrstrasse fahrstrasse in Fahrstrassen_west)
            {
                if (fahrstrasse.GetAktivStatus()) aktiv_west++;
                if (fahrstrasse.Safe) safe_west++;
            }
            foreach (Fahrstrasse fahrstrasse in Fahrstrassen_ost)
            {
                if (fahrstrasse.GetAktivStatus()) aktiv_ost++;
                if (fahrstrasse.Safe) safe_ost++;
            }

            //Mehr als eine Fahrstraße aktiv -> Fehler
            if (aktiv_west > 1) return new MeldeZustand(false);
            if (aktiv_ost > 1) return new MeldeZustand(false);
            if (safe_west > 1) return new MeldeZustand(false);
            if (safe_ost > 1) return new MeldeZustand(false);
            if ((aktiv_west == 1) && (aktiv_ost == 1)) return new MeldeZustand(false);

            //Fahrstraße nach westen aktiv
            if (aktiv_west == 1)
            {
                richtung = true; //In Fahrtrichtung
                fahrstrasseAktiv = true;
                if (safe_west == 1) sicher = true; //Fahrstraße sicher?
            }
            //Fahrstraße nach osten aktiv
            if (aktiv_ost == 1)
            {
                richtung = false;  //Gegenfahrtrichtung
                fahrstrasseAktiv = true;
                if (safe_ost == 1) sicher = true; //Fahrstraße sicher?
            }

            //Generiere Statuskonstrukt
            return new MeldeZustand(besetzt, fahrstrasseAktiv, sicher, richtung);
        }

        /// <summary>
        /// Weichenliste der Fahrstraße durchlaufen und mit aktueller Weichenstellung vergleichen
        /// </summary>
        /// <param name="fahrstrasse">Fahrstraße zum überprüfen</param>
        private void Fahrstrassenupdate(Fahrstrasse fahrstrasse)
        {
            if (fahrstrasse.GetGesetztStatus())    //Fahrstraße wurde gesetzt
            {
                fahrstrasse.SetFahrstrasseRichtung();
                //Prüfen ob alle Weichen der Fahrstraßen richtig geschaltet sind
                if (fahrstrasse.CheckFahrstrassePos() == false) //Noch nicht alle Weichen gestellt
                {
                    if (Betriebsbereit) fahrstrasse.SetFahrstrasse();
                }
                else //Alle Weichen in richtiger Stellung
                {
                    //Fahrstraße als aktiviert kennzeichnen
                    fahrstrasse.AktiviereFahrstasse();
                    //Jede Weiche in der Fahrstraßenliste durchlaufen
                    /*
                    foreach (Weiche Fahrstrassenweiche in fahrstrasse.Fahrstr_Weichenliste)
                    {
                        Weiche weiche = WeichenListe.GetWeiche(Fahrstrassenweiche.Name);   //Weiche in Globale Liste suchen
                        if (weiche == null) return;   //Weichen nicht gefunden - Funktion abbrechen
                        GleisplanUpdateWeiche(weiche);  //Weichenbild aktualisieren
                    }
                    */
                    //Weichen zyklisch nochmal schalten um hängenbleiben zu vermeiden
                    if (Betriebsbereit) fahrstrasse.ControlSetFahrstrasse(z21Start);
                }
            }

        }
        #endregion

        #region Weichensteuerung
        /// <summary>
        /// Schalten der Weiche bei Klicken auf das Symbol
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void Weiche_Click(object sender, EventArgs e)
        {
            if (!Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    //Weiche nochmal neu schalten
                    WeichenListe.SetzeWeiche(weichenElement.Name, WeichenListe.GetWeiche(weichenElement.Name).Abzweig);
                }
                else
                {
                    WeichenListe.ToggleWeiche(weichenElement.Name);
                }
            }
        }
        /// <summary>
        /// Doppelkreuzungsweiche schalten
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
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
                            if (e2.Y > (weichenElement.Height / 2))
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
        /// <summary>
        /// Kreuzungsweiche schalten
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
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
        /// <summary>
        /// Button Click: Signal schalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                            if (AutoSignale.Checked) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
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
        /// <summary>
        /// Button Click: SH2 Signal schalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                UpdateFahrstrassenSchalter(1);
            }
        }

        #region Fahrstrassen

        #endregion

        #endregion

        #region Fahrstrassen
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            //Fahrstrassen Importieren
            FahrstrassenListe = new FahrstrassenListe("Fahrstrassenliste.xml", WeichenListe, SignalListe);
        }
        /// <summary>
        /// Fahrstraße aktivieren/deaktivieren
        /// </summary>
        /// <param name="fahrstrasse">Fahrstraße zum Schalten</param>
        private void ToggleFahrstrasse(Fahrstrasse fahrstrasse)
        {
            //Fahrstraße gesetzt
            if (fahrstrasse.GetGesetztStatus())
            {
                //Fahrstraße deaktivieren
                fahrstrasse.DeleteFahrstrasse(WeichenListe.Liste);
            }
            else
            {
                //Fahrstraße aktivieren
                if (Betriebsbereit) fahrstrasse.StarteFahrstrasse();
            }
            //Alle Fahrstraßen/Buttons aktualisieren
            UpdateFahrstrassenSchalter(1);
        }

        /// <summary>
        /// Button Click: Fahrstrasse an und ausschalten
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void FahrstrassenButton_Click(object sender, EventArgs e)
        {
            //Neue Fahrstrasse anlegen
            Fahrstrasse fahrstrasse;
            //Flag ob neue Buttons unter den Fahrstrassen-Button erstellt werden (False -> Werden erstellt)
            bool Abbau = false;

            //Prüfen ob Funktion von einem Button ausgelöst wurde
            if (sender is Button button)
            {
                //Button muss mit dem Namen der Fahrstraße als Tag ausgestattet sein
                if (button.Tag == null) return;
                string ZielFahrstrasse = button.Tag.ToString();

                //Tag endet mit '-' -> Es wurde ein generierter Button gedrückt
                if (ZielFahrstrasse.EndsWith("-"))
                {
                    Abbau = true; //Abbau der Generierten Buttons
                    ZielFahrstrasse = ZielFahrstrasse.Substring(0, ZielFahrstrasse.Length - 1); //'-' entfernen
                }

                //Fahrstrasse aus der Liste finden mit Namen
                fahrstrasse = FahrstrassenListe.GetFahrstrasse(ZielFahrstrasse);

                //Keine Fahrstraße gefunden -> Funktion abbrechen
                if (fahrstrasse == null) return;

                //Wenn Fahrstrassen gleichen Ausgangspunkt haben
                if (fahrstrasse.Fahrstr_GleicherEingang.Count >= 1)
                {
                    //Ist eine der Fahrstrassen mit gleichen Ausgang bereits aktiv?
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
                    //Keine der Fahrstrassen mit gleichen Ausgang aktiv
                    else
                    {
                        // Untergruppe mit Buttons soll gelöscht werden
                        if (Abbau)
                        {
                            //Fahrstrasse ist nicht blockiert mit anderen Fahrstrassen
                            if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                            {
                                //Fahrstrasse einschalten
                                ToggleFahrstrasse(fahrstrasse);
                                //Unterbuttons löschen
                                LoescheButtons(fahrstrasse.Fahrstr_GleicherEingang);

                                //Button freigeben 
                                Button clickedButton = button;
                                clickedButton.Dispose();
                            }
                        }
                        else //Buttons sollen generiert werden (Erster Klick auf Buttons mit mehreren Ausgängen
                        {
                            GeneriereButtons(fahrstrasse.Fahrstr_GleicherEingang, button.Location.X, button.Location.Y);
                        }
                    }
                }
                //Einzelne Fahrstrasse
                else
                {
                    //Fahrstrasse aktiv?
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
        /// <summary>
        /// Untergruppe an Buttons generieren
        /// </summary>
        /// <param name="Fahrstrassen">Liste an Fahrstrassennamen</param>
        /// <param name="X">Position des Ursprungsbuttons (X-Koordinate)</param>
        /// <param name="Y">Position des Ursprungsbuttons (Y-Koordinate)</param>
        private void GeneriereButtons(List<string> Fahrstrassen, int X, int Y)
        {
            //Offset von 20 Punkten nach links
            X += 20;

            //Wenn Buttons schon existieren -> löschen
            Control Modul = this.GleisplanAnzeige.Controls[Fahrstrassen[0] + "_Auswahl"];
            if (Modul is Button button)
            {
                LoescheButtons(Fahrstrassen);
                return;
            }
            //Für jede Fahrstrasse einen Button anlegen
            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                //Neuen Button erstellen
                Button newButton = new Button
                {
                    Name = Fahrstrassenname + "_Auswahl",
                    Tag = Fahrstrassenname + "-",
                    Size = new Size(100, 20),
                    Location = new Point(X, Y),
                    Enabled = !FahrstrassenListe.FahrstrasseBlockiert(Fahrstrassenname)
                };
                //Offset um 20 Punkte nach unten
                Y += 20;
                //Funktion an neuen Button zurodnen
                newButton.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                newButton.BringToFront();

                //Fahrstrassenname beinhaltet einen Unterstrich
                if (Fahrstrassenname.Contains('_'))
                {
                    //Nur letzten Teil übernehmen
                    string[] text = Fahrstrassenname.Split('_');
                    newButton.Text = text[2];
                }
                //Fahrstrassenname als Text übernehmen
                else
                {
                    newButton.Text = Fahrstrassenname;
                }
                //Button hinzufügen
                this.GleisplanAnzeige.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        /// <summary>
        /// Buttons löschen
        /// </summary>
        /// <param name="Fahrstrassen">Liste mit dem Namen der Fahrstrassen</param>
        private void LoescheButtons(List<string> Fahrstrassen)
        {
            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                //Name mit Auswahl erweitern
                Control Modul = this.GleisplanAnzeige.Controls[Fahrstrassenname + "_Auswahl"];
                if (Modul is Button)
                {
                    this.GleisplanAnzeige.Controls.Remove(Modul);
                }
            }

        }

        
        /// <summary>
        /// Alle Fahrstrassen-Buttons aktualisieren. (Deaktivieren der Buttons bei gesperrten Fahrstrassen)
        /// </summary>
        /// <param name="dummy"></param>
        private void UpdateFahrstrassenSchalter(int dummy)
        {
            UpdateSperrungen();
            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                var Fund = this.GleisplanAnzeige.Controls.Find(fahrstrasse.Name + "_Button", true);
                foreach (Control control in Fund)
                {
                    if (control is Button button)
                    {
                        if (FahrstrassenListe.FahrstrasseAlleGleicheBlockiert(fahrstrasse))
                        {
                            if (button.Enabled == true)
                            {
                                button.Enabled = false;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben_deakt;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten_deakt;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else
                                {
                                    break;
                                }
                                if (fahrstrasse.EinfahrtsSignal.Zustand != SignalZustand.HP0) fahrstrasse.EinfahrtsSignal.Schalten(SignalZustand.HP0);
                            }
                        }
                        else
                        {
                            if (button.Enabled == false)
                            {
                                button.Enabled = true;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else { }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fahrstrassensperrung anhand von SH2 und Belegtmeldung errechnen
        /// </summary>
        private void UpdateSperrungen()
        {
            List<string> Aenderungen = new List<string>();
            foreach (string ButtonName in SperrButtons)
            {
                var Fund = this.GleisplanAnzeige.Controls.Find(ButtonName, true);
                foreach (Control control in Fund)
                {
                    if (control is CheckBox checkBox)
                    {
                        if (checkBox.Checked)
                        {
                            Aenderungen.AddRange(checkBox.Tag.ToString().Split('+'));
                        }
                    }
                }
            }

            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                if (fahrstrasse.IstFahrstrasseBelegt(BelegtmelderListe.Liste))
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = true; // Fahrstrasse ist gesperrt
                }
                else if (Aenderungen.Contains(fahrstrasse.Name))
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = true; // Fahrstrasse ist gesperrt
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        //Fahrstraße deaktivieren wenn gesetzt
                        fahrstrasse.DeleteFahrstrasse(WeichenListe.Liste);
                    }
                }
                else
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = false; // Fahrstrasse ist nicht (oder nicht mehr) gesperrt
                }
            }

        }
        #endregion

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
            foreach (Lokomotive lok in LokListe)
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
                    return; // Hat sich nicht verbunden -> Task beenden
                }
            }

            // Z21 ist verbunden
            if (z21Start.Verbunden())
            {
                if (!z21_Einstellung.IsDisposed) // Fenster Z21-Einstellung läuft immer noch im Hintergrund
                {
                    Flags temp = z21_Einstellung.Get_Flag_Config();
                    z21Start.Z21_SET_BROADCASTFLAGS(temp); // Flags neu setzen 
                }
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
                Betriebsbereit = true;
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

            z21Start.SetQMode(true);

            // Instanzzugriffe auf Zentrale
            WeichenListe.DigitalzentraleZugriff(z21Start);
            SignalListe.DigitalzentraleZugriff(z21Start);

            z21_Einstellung = new Z21_Einstellung();    //Neues Fenster: Einstellung der Z21 (Läuft im Hintergund)
            z21_Einstellung.Get_Z21_Instance(this);     //Z21-Verbindung dem neuen Fenster mitgeben

            ConnectStatus(false, false);                 //Verbindungsstatus initialisieren
            Betriebsbereit = false;
        }

        private void LokListe_Laden()
        {
            for (int i = 0; i < Max_Loks; i++)
            {
                string LokAdresse = Config.ReadConfig(String.Format("LokListe{0}", i));
                if (int.TryParse(LokAdresse, out int DigitAdresse))
                {
                    if (LokomotivenArchiv.SucheDurchAdresse(DigitAdresse, out Lokomotive lokomotive))
                    {
                        lokomotive.Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                        lokomotive.Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
                        lokomotive.Register_CMD_LOKSTATUS(Setze_Lok_Status);

                        lokomotive.VorherigerBlock = "";
                        lokomotive.AktuellerBlock = "";

                        string VorherigePosition = Config.ReadConfig(String.Format("LokPosVor{0}", i));
                        if (BelegtmelderListe.GetBelegtmelder(VorherigePosition) != null)
                        {
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

                        LokListe.Add(lokomotive);
                    }
                }
            }
        }

        private void LokListe_Speichern()
        {
            for(int i = 0;i < Max_Loks; i++)
            {
                if(i <  LokListe.Count)
                {
                    Config.WriteConfig(String.Format("LokListe{0}", i), LokListe[i].Adresse.ToString());
                    Config.WriteConfig(String.Format("LokPos{0}",i), LokListe[i].AktuellerBlock.ToString());
                    Config.WriteConfig(String.Format("LokPosVor{0}",i), LokListe[i].VorherigerBlock.ToString());
                }
                else
                {
                    Config.WriteConfig(String.Format("LokListe{0}", i), "0");
                    Config.WriteConfig(String.Format("LokPos{0}", i), "");
                    Config.WriteConfig(String.Format("LokPosVor{0}", i), "");
                }
            }
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
                if (checkBox.Checked == true)
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
        /// <summary>
        /// Farbe bei aktivierung anpassen (orange)
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void AutoFahrdienstleiter_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(255, 128, 0);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                }
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
                if (checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(0, 128, 0);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                }
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
                if (checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(128, 0, 128);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                }
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
            }
            else
            {
                ZugmenueFenster = new Zugmenue(z21Start, LokomotivenArchiv, Bahnhofsansage, LokListe, BelegtmelderListe);
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

        public void CallBack_LAN_RMBUS_DATACHANGED(byte GruppenIndex, byte[] RMStatus)
        {
            this.BeginInvoke((Action<byte, byte[]>)UpdateBelegtmeldung, GruppenIndex, RMStatus);
            this.BeginInvoke((Action<int>)UpdateFahrstrassenSchalter, 1);
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
                Betriebsbereit = true;
            }
            if ((Status & 0x02) == 0x02)
            {
                TrackStatus.Text = "Strecke: Kein Strom";
                TrackStatus.BackColor = Color.Gold;
                TrackStatus.ForeColor = Color.Black;
                Betriebsbereit = false;
            }
            if ((Status & 0x01) == 0x01)
            {
                TrackStatus.Text = "Strecke: Nothalt";
                TrackStatus.BackColor = Color.Orange;
                TrackStatus.ForeColor = Color.Black;
                Betriebsbereit = false;
            }
            if ((Status & 0x04) == 0x04)
            {
                TrackStatus.Text = "Strecke: Kurzschluss";
                TrackStatus.BackColor = Color.Red;
                TrackStatus.ForeColor = Color.White;
                Betriebsbereit = false;
            }
            if ((Status & 0x20) == 0x20)
            {
                TrackStatus.Text = "Programmiermodus";
                TrackStatus.BackColor = Color.Blue;
                TrackStatus.ForeColor = Color.White;
                Betriebsbereit = false;
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

            int ListID = LokListe.FindIndex(x => x.Adresse == Adresse); //Finde Lok mit dieser Adresse 
            if (ListID == -1)//Lok nicht gefunden in der Liste
            {
                return;
            }

            LokListe[ListID].UpdateZ21Data(ParamterCount, FahrstufenInfo, Richtung, Fahrstufe, Funktionen);

        }
        private void UpdateBelegtmeldung(byte GruppenIndex, byte[] RMStatus)
        {
            BelegtmelderListe.UpdateBelegtmelder(GruppenIndex, RMStatus);
        }
        #endregion
        #endregion

        private void weichenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (weichen_Ueberwachung == null) weichen_Ueberwachung = new Weichen_Ueberwachung(WeichenListe.Liste);
            if (weichen_Ueberwachung.IsDisposed) weichen_Ueberwachung = new Weichen_Ueberwachung(WeichenListe.Liste);
            weichen_Ueberwachung.Show();
            weichen_Ueberwachung.BringToFront();
        }

        private void LokKontrolle_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.BackColor = Color.FromArgb(128, 0, 128);
                    checkBox.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    checkBox.BackColor = Color.FromArgb(64, 64, 64);
                    checkBox.ForeColor = Color.FromArgb(192, 192, 192);
                }
            }
        }
    }
}
