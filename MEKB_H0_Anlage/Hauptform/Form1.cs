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
        private InfoBox InfoBox;

        public List<Weiche> Weichenliste = new List<Weiche>();
        public List<Signal> Signalliste = new List<Signal>();
        public List<Lok> Lokliste = new List<Lok>();

        public Form1()
        {
            InitializeComponent();                      //Programminitialisieren

            z21Start = new Z21(this);                   //Neue Z21-Verbindung anlegen
            z21_Einstellung = new Z21_Einstellung();    //Neues Fenster: Einstellung der Z21 (Läuft im Hintergund)
            z21_Einstellung.Get_Z21_Instance(this);     //Z21-Verbindung dem neuen Fenster mitgeben

            ConnectStatus(false,false);                 //Verbindungsstatus auf getrennt setzen

            SetupWeichenListe();                        //Weichenliste aus Datei laden
            SetupFahrstrassen();                        //Fahstrassen festlegen
            SetupSignalListe();                         //Signalliste festlegen
            SetupLokListe();                            //Lok-Daten aus Dateien laden

            LokCtrl_LoklisteAusfuellen();               //Auswahlliste im Lok-Kontrollfenster ausfüllen
        }



        private void GetFirmware_Click(object sender, EventArgs e)
        {
            z21Start.Z21_GET_FIRMWARE_VERSION();
        }

        private void MenuZ21Eigenschaften_Click(object sender, EventArgs e)
        {
            if (z21_Einstellung.IsDisposed) z21_Einstellung = new Z21_Einstellung();
            z21_Einstellung.Show();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Gleisplan = this.CreateGraphics();
        }

        private static System.Timers.Timer FlagTimer;
        private static System.Timers.Timer WeichenTimer;

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

            if (Config.ReadConfig("Auto_Connect").Equals("true")) z21Start.Connect_Z21();   //Wenn "Auto_Connect" gesetzt ist: Verbinden


        }
        private void Z21_Heartbeat(Object source, ElapsedEventArgs e)
        {
            //Nur ausführen, wenn Verbindung aufgebaut ist
            if (z21Start.Verbunden())
            {
                if (!z21_Einstellung.IsDisposed) //Fenster Z21-Einstellung nläuft immer noch im Hintergrund
                {
                    z21Start.Z21_SET_BROADCASTFLAGS(z21_Einstellung.Get_Flag_Config()); //Flags neu setzen 
                }
            }

        }
        
        private int Pointer_Weichenliste = 0;
        private int Pointer_Signalliste = 0;
        private bool Signal_Init;
        private bool Weichen_Init;
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
                if(Weichen_Init & Signal_Init) SetConnect(true, true); //Initialisierung abgeschlossen
                //
                Fahrstrassenupdate(Gleis1_nach_links);
                Fahrstrassenupdate(Gleis2_nach_links);
                Fahrstrassenupdate(Gleis3_nach_links);
                Fahrstrassenupdate(Gleis4_nach_links);
                Fahrstrassenupdate(Gleis5_nach_links);
                Fahrstrassenupdate(Gleis6_nach_links);

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

                FahrstrasseBildUpdate();
            }

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

        private void Weiche1_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche1");
        }
        private void Weiche2_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche2");
        }
        private void Weiche3_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche3");
        }
        private void Weiche4_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche4");
        }
        private void Weiche5_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche5");
        }
        private void Weiche6_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche6");
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
        private void Weiche8_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche8");
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
        private void Weiche21_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche21");
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
        private void Weiche23_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche23");
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
        private void Weiche25_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche25");
        }
        private void Weiche26_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche26");
        }
        private void Weiche27_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche27");
        }
        private void Weiche28_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche28");
        }
        private void Weiche29_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche29");
        }
        private void Weiche30_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche30");
        }
        private void Weiche50_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche50");
        }
        private void Weiche51_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche51");
        }
        private void Weiche52_Click(object sender, EventArgs e)
        {
            SetWeiche("Weiche52", true);        //Nur Abzweig, da noch nicht vorhanden
        }
        private void Weiche53_Click(object sender, EventArgs e)
        {
            SetWeiche("Weiche53", true);        //Nur Abzweig, da noch nicht vorhanden
        }
        private void Weiche60_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche60");
        }
        private void Weiche61_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche61");
        }
        private void Weiche62_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche62");
        }
        private void Weiche63_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche63");
        }
        private void Weiche64_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche64");
        }
        private void Weiche70_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche70");
        }
        private void Weiche71_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche71");
        }
        private void Weiche72_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche72");
        }
        private void Weiche73_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche73");
        }
        private void Weiche74_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche74");
        }

        private void Fahrstr_GL1_links_Click(object sender, EventArgs e)
        {
            if (Gleis1_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if(!Gleis2_nach_links.GetGesetztStatus() &&
                   !Gleis3_nach_links.GetGesetztStatus() &&
                   !Gleis4_nach_links.GetGesetztStatus() &&
                   !Gleis5_nach_links.GetGesetztStatus() && 
                   !Gleis6_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis1_nach_links);
                }
            }
        }
        private void Fahrstr_GL2_links_Click(object sender, EventArgs e)
        {
            if (Gleis2_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_links.GetGesetztStatus() &&
                   !Gleis3_nach_links.GetGesetztStatus() &&
                   !Gleis4_nach_links.GetGesetztStatus() &&
                   !Gleis5_nach_links.GetGesetztStatus() &&
                   !Gleis6_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis2_nach_links);
                }
            }
        }
        private void Fahrstr_GL3_links_Click(object sender, EventArgs e)
        {
            if (Gleis3_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_links.GetGesetztStatus() &&
                   !Gleis2_nach_links.GetGesetztStatus() &&
                   !Gleis4_nach_links.GetGesetztStatus() &&
                   !Gleis5_nach_links.GetGesetztStatus() &&
                   !Gleis6_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus() &&
                   !Block2_nach_Gleis3.GetGesetztStatus() &&
                   !Block2_nach_Gleis4.GetGesetztStatus() &&
                   !Block2_nach_Gleis5.GetGesetztStatus() &&
                   !Block2_nach_Gleis6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis3_nach_links);
                }
            }
        }
        private void Fahrstr_GL4_links_Click(object sender, EventArgs e)
        {
            if (Gleis4_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_links.GetGesetztStatus() &&
                   !Gleis2_nach_links.GetGesetztStatus() &&
                   !Gleis3_nach_links.GetGesetztStatus() &&
                   !Gleis5_nach_links.GetGesetztStatus() &&
                   !Gleis6_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus() &&
                   !Block2_nach_Gleis3.GetGesetztStatus() &&
                   !Block2_nach_Gleis4.GetGesetztStatus() &&
                   !Block2_nach_Gleis5.GetGesetztStatus() &&
                   !Block2_nach_Gleis6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis4_nach_links);
                }
            }
        }
        private void Fahrstr_GL5_links_Click(object sender, EventArgs e)
        {
            if (Gleis5_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_links.GetGesetztStatus() &&
                   !Gleis2_nach_links.GetGesetztStatus() &&
                   !Gleis3_nach_links.GetGesetztStatus() &&
                   !Gleis4_nach_links.GetGesetztStatus() &&
                   !Gleis6_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus() &&
                   !Block2_nach_Gleis3.GetGesetztStatus() &&
                   !Block2_nach_Gleis4.GetGesetztStatus() &&
                   !Block2_nach_Gleis5.GetGesetztStatus() &&
                   !Block2_nach_Gleis6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis5_nach_links);
                }
            }
        }
        private void Fahrstr_GL6_links_Click(object sender, EventArgs e)
        {
            if (Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_links);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_links.GetGesetztStatus() &&
                   !Gleis2_nach_links.GetGesetztStatus() &&
                   !Gleis3_nach_links.GetGesetztStatus() &&
                   !Gleis4_nach_links.GetGesetztStatus() &&
                   !Gleis5_nach_links.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus() &&
                   !Block2_nach_Gleis3.GetGesetztStatus() &&
                   !Block2_nach_Gleis4.GetGesetztStatus() &&
                   !Block2_nach_Gleis5.GetGesetztStatus() &&
                   !Block2_nach_Gleis6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis6_nach_links);
                }
            }
        }

        private void Block2_Einfahrt_Click(object sender, EventArgs e)
        {   //Einer der Block2-Fahrstrassen aktiv
            if(Block2_nach_Gleis1.GetGesetztStatus() ||
               Block2_nach_Gleis2.GetGesetztStatus() ||
               Block2_nach_Gleis3.GetGesetztStatus() ||
               Block2_nach_Gleis4.GetGesetztStatus() ||
               Block2_nach_Gleis5.GetGesetztStatus() ||
               Block2_nach_Gleis6.GetGesetztStatus()) 
            {   //Aktive Fahrstrasse ausschalten
                if (Block2_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis1);
                if (Block2_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis2);
                if (Block2_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis3);
                if (Block2_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis4);
                if (Block2_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis5);
                if (Block2_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Block2_Auswahl.Visible) Block2_Auswahl.Visible = false;
                else Block2_Auswahl.Visible = true;
            }
        }
        private void Block2_Einfaht_GL1_Click(object sender, EventArgs e)
        {
            if (!Gleis1_nach_links.GetGesetztStatus() &&
                !Gleis2_nach_links.GetGesetztStatus() &&
                !Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis1);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL2_Click(object sender, EventArgs e)
        {
            if (!Gleis1_nach_links.GetGesetztStatus() &&
                !Gleis2_nach_links.GetGesetztStatus() &&
                !Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis2);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL3_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis3);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL4_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis4);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL5_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis5);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL6_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_links.GetGesetztStatus() &&
                !Gleis4_nach_links.GetGesetztStatus() &&
                !Gleis5_nach_links.GetGesetztStatus() &&
                !Gleis6_nach_links.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis6);
                Block2_Auswahl.Visible = false;
            }
        }
        
        private void Fahrstr_GL6_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl6-rechts-Fahrstrassen aktiv
            if (Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis6_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_rechts1);
                if (Gleis6_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl6_rechts_Auswahl.Visible) Gl6_rechts_Auswahl.Visible = false;
                else Gl6_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl6_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_rechts1);
                Gl6_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl6_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_rechts2);
                Gl6_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_GL5_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl5-rechts Fahrstrassen aktiv
            if (Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis5_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis5_nach_rechts1);
                if (Gleis5_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis5_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl5_rechts_Auswahl.Visible) Gl5_rechts_Auswahl.Visible = false;
                else Gl5_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl5_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_rechts1);
                Gl5_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl5_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_rechts2);
                Gl5_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_GL4_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl4-rechts Fahrstrassen aktiv
            if (Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis4_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis4_nach_rechts1);
                if (Gleis4_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis4_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl4_rechts_Auswahl.Visible) Gl4_rechts_Auswahl.Visible = false;
                else Gl4_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl4_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_rechts1);
                Gl4_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl4_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_rechts2);
                Gl4_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_GL3_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl3-rechts Fahrstrassen aktiv
            if (Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis3_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis3_nach_rechts1);
                if (Gleis3_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis3_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl3_rechts_Auswahl.Visible) Gl3_rechts_Auswahl.Visible = false;
                else Gl3_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl3_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_rechts1);
                Gl3_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl3_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_rechts2);
                Gl3_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_GL2_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl2-rechts Fahrstrassen aktiv
            if (Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis2_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis2_nach_rechts1);
                if (Gleis2_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis2_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl2_rechts_Auswahl.Visible) Gl2_rechts_Auswahl.Visible = false;
                else Gl2_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl2_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_rechts1);
                Gl2_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl2_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_rechts2);
                Gl2_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_GL1_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis1_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis1_nach_rechts1);
                if (Gleis1_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis1_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl1_rechts_Auswahl.Visible) Gl1_rechts_Auswahl.Visible = false;
                else Gl1_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl1_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_rechts1);
                Gl1_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl1_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_rechts2);
                Gl1_rechts_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_Rechts1_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Rechts1_nach_Gleis1.GetGesetztStatus() ||
                Rechts1_nach_Gleis2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Rechts1_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis1);
                if (Rechts1_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis2);
                if (Rechts1_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis3);
                if (Rechts1_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis4);
                if (Rechts1_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis5);
                if (Rechts1_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Rechts1_Auswahl.Visible) Rechts1_Auswahl.Visible = false;
                else Rechts1_Auswahl.Visible = true;
            }
        }
        private void Rechts1_Einfahrt_Gl1_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis1);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl2_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis2);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl3_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis3);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl4_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis4);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl5_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis5);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl6_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis6);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Fahrstr_Rechts2_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Rechts2_nach_Gleis1.GetGesetztStatus() ||
                Rechts2_nach_Gleis2.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Rechts2_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis1);
                if (Rechts2_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis2);
                if (Rechts2_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis3);
                if (Rechts2_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis4);
                if (Rechts2_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis5);
                if (Rechts2_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Rechts2_Auswahl.Visible) Rechts2_Auswahl.Visible = false;
                else Rechts2_Auswahl.Visible = true;
            }
        }
        private void Rechts2_Einfahrt_Gl1_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis1);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl2_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis2);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl3_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis3);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl4_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis4);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl5_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis5);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl6_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis6);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void LokCtrl1_Strg_Typ_Click(object sender, EventArgs e)
        {
            if(LokCtrl1_Strg_Typ.Text == "Manuell")
            {
                LokCtrl1_Strg_Typ.Text = "Automatik";
                LokCtrl1_Strg_Typ.BackColor = Color.FromArgb(128,0,0);
            }
            else
            {
                LokCtrl1_Strg_Typ.Text = "Manuell";
                LokCtrl1_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            }

        }
        private void DemoFahrt_Scroll(object sender, EventArgs e)
        {
            int Richtung;
            int Fahrstufe;
            int Geschwindigkeit;
            int Adresse = (int)LokCtrl1_Adr.Value;

            if (DemoFahrt.Value < 0)
            {
                Richtung = LokFahrstufen.Rueckwaerts;
                Geschwindigkeit = -DemoFahrt.Value;
            }
            else
            {
                Richtung = LokFahrstufen.Vorwaerts;
                Geschwindigkeit = DemoFahrt.Value;
            }
            switch (DemoFahrstufen.Text)
            {
                case "FS14": Fahrstufe = LokFahrstufen.Fahstufe14;break;
                case "FS28": Fahrstufe = LokFahrstufen.Fahstufe28; break;
                case "FS128": Fahrstufe = LokFahrstufen.Fahstufe128; break;
                default: Fahrstufe = LokFahrstufen.Fahstufe128; break;
            }

            
            z21Start.Z21_SET_LOCO_DRIVE(Adresse, Geschwindigkeit, Richtung, Fahrstufe);
            DemoGesch.Text = DemoFahrt.Value.ToString();


        }

        private void DemoFahrstufen_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (DemoFahrstufen.Text)
            {
                case "FS14":
                    DemoFahrt.TickFrequency = 14;
                    DemoFahrt.Maximum = 14;
                    DemoFahrt.Minimum = -14;
                    break;
                case "FS28":
                    DemoFahrt.TickFrequency = 28;
                    DemoFahrt.Maximum = 28;
                    DemoFahrt.Minimum = -28;
                    break;
                case "FS128":
                    DemoFahrt.TickFrequency = 128;
                    DemoFahrt.Maximum = 128;
                    DemoFahrt.Minimum = -128;
                    break;
                default:break;
            }
            
        }

        bool Sperrung;
        private void Sperr_GL1_links_Click(object sender, EventArgs e)
        {
            if(Sperrung)
            {
                Sperr_GL1_links.Image = MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv;
                Sperrung = false;
            }
            else
            {
                Sperr_GL1_links.Image = MEKB_H0_Anlage.Properties.Resources.SH_2;
                Sperrung = true;
            }
        }

        private void UeberSteuerprogrammToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoBox = new InfoBox();
            InfoBox.Show();
        }

        private void LokCtrl1_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl1_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl1_Adr.Value = Adresse;      
        }
        private void LokCtrl2_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl2_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl2_Adr.Value = Adresse;
        }
        private void LokCtrl3_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl3_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl3_Adr.Value = Adresse;
        }
        private void LokCtrl4_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl4_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl4_Adr.Value = Adresse;
        }
        private void LokCtrl5_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl5_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl5_Adr.Value = Adresse;
        }
        private void LokCtrl6_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl6_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl6_Adr.Value = Adresse;
        }
        private void LokCtrl7_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl7_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl7_Adr.Value = Adresse;
        }
        private void LokCtrl8_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl8_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl8_Adr.Value = Adresse;
        }
        private void LokCtrl9_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl9_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl9_Adr.Value = Adresse;
        }
        private void LokCtrl10_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ListID = Lokliste.IndexOf(new Lok() { Name = LokCtrl10_Name.Text });
            if (ListID == -1) return;
            int Adresse = Lokliste[ListID].Adresse;
            LokCtrl10_Adr.Value = Adresse;
        }

        private void LokCtrl1_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl1_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl1_Name.Text = String.Format("Lok: {0}", LokCtrl1_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl1_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern1(sender, e);
        }
        private void LokCtrl2_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl2_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl2_Name.Text = String.Format("Lok: {0}", LokCtrl2_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl2_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern2(sender, e);
        }
        private void LokCtrl3_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl3_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl3_Name.Text = String.Format("Lok: {0}", LokCtrl3_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl3_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern3(sender, e);
        }
        private void LokCtrl4_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl4_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl4_Name.Text = String.Format("Lok: {0}", LokCtrl4_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl4_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern4(sender, e);
        }
        private void LokCtrl5_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl5_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl5_Name.Text = String.Format("Lok: {0}", LokCtrl5_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl5_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern5(sender, e);
        }
        private void LokCtrl6_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl6_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl6_Name.Text = String.Format("Lok: {0}", LokCtrl6_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl6_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern6(sender, e);
        }
        private void LokCtrl7_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl7_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl7_Name.Text = String.Format("Lok: {0}", LokCtrl7_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl7_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern7(sender, e);
        }
        private void LokCtrl8_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl8_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl8_Name.Text = String.Format("Lok: {0}", LokCtrl8_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl8_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern8(sender, e);
        }
        private void LokCtrl9_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl9_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl9_Name.Text = String.Format("Lok: {0}", LokCtrl9_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl9_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern9(sender, e);
        }
        private void LokCtrl10_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl10_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1) LokCtrl10_Name.Text = String.Format("Lok: {0}", LokCtrl10_Adr.Value);    //Lok nicht gefunden in der Liste
            else LokCtrl10_Name.Text = Lokliste[index].Name;

            //Update Rest
            Update_Rufnummern10(sender, e);
        }
        private void Update_Rufnummern1(object sender, EventArgs e)
        {
            if (LokCtrl1_Adr.Value != 0) LokCtrl1_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl1_Gattung.Text) + LokCtrl1_Adr.Value.ToString();
            else LokCtrl1_Ruf.Text = "";
        }
        private void Update_Rufnummern2(object sender, EventArgs e)
        {
            if (LokCtrl2_Adr.Value != 0) LokCtrl2_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl2_Gattung.Text) + LokCtrl2_Adr.Value.ToString();
            else LokCtrl2_Ruf.Text = "";
        }
        private void Update_Rufnummern3(object sender, EventArgs e)
        {
            if (LokCtrl3_Adr.Value != 0) LokCtrl3_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl3_Gattung.Text) + LokCtrl3_Adr.Value.ToString();
            else LokCtrl3_Ruf.Text = "";
        }
        private void Update_Rufnummern4(object sender, EventArgs e)
        {
            if (LokCtrl4_Adr.Value != 0) LokCtrl4_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl4_Gattung.Text) + LokCtrl4_Adr.Value.ToString();
            else LokCtrl4_Ruf.Text = "";
        }
        private void Update_Rufnummern5(object sender, EventArgs e)
        {
            if (LokCtrl5_Adr.Value != 0) LokCtrl5_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl5_Gattung.Text) + LokCtrl5_Adr.Value.ToString();
            else LokCtrl5_Ruf.Text = "";
        }
        private void Update_Rufnummern6(object sender, EventArgs e)
        {
            if (LokCtrl6_Adr.Value != 0) LokCtrl6_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl6_Gattung.Text) + LokCtrl6_Adr.Value.ToString();
            else LokCtrl6_Ruf.Text = "";
        }
        private void Update_Rufnummern7(object sender, EventArgs e)
        {
            if (LokCtrl7_Adr.Value != 0) LokCtrl7_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl7_Gattung.Text) + LokCtrl7_Adr.Value.ToString();
            else LokCtrl7_Ruf.Text = "";
        }
        private void Update_Rufnummern8(object sender, EventArgs e)
        {
            if (LokCtrl8_Adr.Value != 0) LokCtrl8_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl8_Gattung.Text) + LokCtrl8_Adr.Value.ToString();
            else LokCtrl8_Ruf.Text = "";
        }
        private void Update_Rufnummern9(object sender, EventArgs e)
        {
            if (LokCtrl9_Adr.Value != 0) LokCtrl9_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl9_Gattung.Text) + LokCtrl9_Adr.Value.ToString();
            else LokCtrl9_Ruf.Text = "";
        }
        private void Update_Rufnummern10(object sender, EventArgs e)
        {
            if (LokCtrl10_Adr.Value != 0) LokCtrl10_Ruf.Text = LokKontrolle.Abkuerzung(LokCtrl10_Gattung.Text) + LokCtrl10_Adr.Value.ToString();
            else LokCtrl10_Ruf.Text = "";
        }

        private void Signal_Ausfahrt_L1_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_L2_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L2" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Ausfahrt_L3_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L3" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Ausfahrt_L4_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L4" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_L5_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L5" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_L6_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L6" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_R1_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R1" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_R2_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R2" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Ausfahrt_R3_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R3" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Ausfahrt_R4_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R4" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_R5_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R5" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_Ausfahrt_R6_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R6" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_RTunnel_1_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_RTunnel_1" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 2) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
        }

        private void Signal_RTunnel_2_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_RTunnel_2" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Tunnel_L1_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Tunnel_L1" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand == 1) Signalliste[ListID].Schalten(0, z21Start);
            else if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
        }

        private void Signal_Einfahrt_L_Click(object sender, EventArgs e)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Einfahrt_L" });
            if (ListID == -1) return;

            if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(2, z21Start);
            }
            else
            {
                if (Signalliste[ListID].Zustand == 0) Signalliste[ListID].Schalten(1, z21Start);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Alle Autozüge Stop
            //Alle Signale Rot
        }
        ZugSteuerpult Fahrpult2 = new ZugSteuerpult();
        private void OpenFahrpult2_Click(object sender, EventArgs e)
        {
            if(!Fahrpult2.IsDisposed) Fahrpult2.Dispose();
            Fahrpult2 = new ZugSteuerpult();
            Fahrpult2.Show();
        }
    }
}
