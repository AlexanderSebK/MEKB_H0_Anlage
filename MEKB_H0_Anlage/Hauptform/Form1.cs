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
        public Lok[] AktiveLoks = new Lok[12];

        public bool Betriebsbereit;

        public Form1()
        {
            InitializeComponent();                      //Programminitialisieren

            z21Start = new Z21();                   //Neue Z21-Verbindung anlegen
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

            ConnectStatus(false,false);                 //Verbindungsstatus auf getrennt setzen

            SetupWeichenListe();                        //Weichenliste aus Datei laden
            SetupFahrstrassen();                        //Fahstrassen festlegen
            SetupSignalListe();                         //Signalliste festlegen
            SetupLokListe();                            //Lok-Daten aus Dateien laden

            Betriebsbereit = false;

            LokCtrl_LoklisteAusfuellen();               //Auswahlliste im Lok-Kontrollfenster ausfüllen
            for(int i = 0; i<AktiveLoks.Length;i++)
            {
                AktiveLoks[i] = new Lok();
                AktiveLoks[i].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                AktiveLoks[i].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            }
        }
        private void GetFirmware_Click(object sender, EventArgs e)
        {
            z21Start.GET_FIRMWARE_VERSION();
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
        private static System.Timers.Timer CooldownTimer;

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

            if (Config.ReadConfig("Auto_Connect").Equals("true")) z21Start.Connect_Z21();   //Wenn "Auto_Connect" gesetzt ist: Verbinden
        }
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
                Fahrstrassenupdate(Gleis1_nach_Block2);
                Fahrstrassenupdate(Gleis2_nach_Block2);
                Fahrstrassenupdate(Gleis3_nach_Block2);
                Fahrstrassenupdate(Gleis4_nach_Block2);
                Fahrstrassenupdate(Gleis5_nach_Block2);
                Fahrstrassenupdate(Gleis6_nach_Block2);
                Fahrstrassenupdate(Gleis1_nach_Block5);
                Fahrstrassenupdate(Gleis2_nach_Block5);
                Fahrstrassenupdate(Gleis3_nach_Block5);
                Fahrstrassenupdate(Gleis4_nach_Block5);
                Fahrstrassenupdate(Gleis5_nach_Block5);
                Fahrstrassenupdate(Gleis6_nach_Block5);

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
                            this.Invoke(new Action<string>(WriteTextBox), new object[] { weiche.ZeitAktiv.ToString() + weiche.Name });

                        if (weiche.ZeitAktiv <= 0)
                        {
                            weiche.ZeitAktiv = 0;                         
                            bool Abzweig = weiche.Abzweig;
                            if (weiche.Spiegeln) Abzweig = !Abzweig;
                            if (Betriebsbereit)
                            {
                                z21Start.Z21_SET_TURNOUT(weiche.Adresse, Abzweig, true, false); //Q-Modus aktiviert, Schaltausgang inaktiv
                                    this.Invoke(new Action<string>(WriteTextBox), new object[] { weiche.ZeitAktiv.ToString() + weiche.Name });
                            }
                        }
                    }
                }
            }
        }

        private void WriteTextBox(String text)
        {
            LokCtrl1_Ort.Text = text;
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

        #region Weichensteuerung
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
            ToggleWeiche("Weiche52");        
        }
        private void Weiche53_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche53");        
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
        private void Weiche65_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche65");
        }
        private void Weiche66_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche66");
        }
        private void Weiche67_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche67");
        }
        private void Weiche68_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche68");
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
        private void Weiche75_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche75");
        }
        private void Weiche76_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche76");
        }
        private void Weiche80_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche80");
        }
        private void Weiche81_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche81");
        }
        private void Weiche82_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche82");
        }
        private void Weiche90_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche90");
        }
        private void Weiche91_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche91");
        }
        private void Weiche92_Click(object sender, EventArgs e)
        {
            ToggleWeiche("Weiche92");
        }
        #endregion

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

        #region Signalsteuerung
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
        #endregion
        private void ProgrammBeenden_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAlle_Click(sender, e);
            foreach(Signal signal in Signalliste)
            {
                signal.Schalten(0, z21Start);//Alle Signale Rot
            }
            z21Start.DisConnect_Z21();
        }

        #region Lok Namensauswahl
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
        #endregion
        #region Lok Adresse ändern
        private void LokCtrl1_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl1_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl1_Name.Text = String.Format("Lok: {0}", LokCtrl1_Adr.Value);
                AktiveLoks[0] = new Lok() { Adresse = (int)LokCtrl1_Adr.Value };
            }
            else
            {
                AktiveLoks[0] = Lokliste[index];
                LokCtrl1_Name.Text = AktiveLoks[0].Name;
                LokCtrl1_Gattung.Text = AktiveLoks[0].Gattung;
            }
            AktiveLoks[0].Automatik = false;
            LokCtrl1_Strg_Typ.Text = "Manuell";
            LokCtrl1_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);

            //Update Rest
            Update_Rufnummern1(sender, e);
        }
        private void LokCtrl2_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl2_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl2_Name.Text = String.Format("Lok: {0}", LokCtrl2_Adr.Value);
                AktiveLoks[1] = new Lok() { Adresse = (int)LokCtrl2_Adr.Value };
            }
            else
            {
                AktiveLoks[1] = Lokliste[index];
                LokCtrl2_Name.Text = AktiveLoks[1].Name;
                LokCtrl2_Gattung.Text = AktiveLoks[1].Gattung;
            }
            AktiveLoks[1].Automatik = false;
            LokCtrl2_Strg_Typ.Text = "Manuell";
            LokCtrl2_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern2(sender, e);
        }
        private void LokCtrl3_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl3_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl3_Name.Text = String.Format("Lok: {0}", LokCtrl3_Adr.Value);
                AktiveLoks[2] = new Lok() { Adresse = (int)LokCtrl3_Adr.Value };
            }
            else
            {
                AktiveLoks[2] = Lokliste[index];
                LokCtrl3_Name.Text = AktiveLoks[2].Name;
                LokCtrl3_Gattung.Text = AktiveLoks[2].Gattung;
            }
            AktiveLoks[2].Automatik = false;
            LokCtrl3_Strg_Typ.Text = "Manuell";
            LokCtrl3_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern3(sender, e);
        }
        private void LokCtrl4_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl4_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl4_Name.Text = String.Format("Lok: {0}", LokCtrl4_Adr.Value);
                AktiveLoks[3] = new Lok() { Adresse = (int)LokCtrl4_Adr.Value };
            }
            else
            {
                AktiveLoks[3] = Lokliste[index];
                LokCtrl4_Name.Text = AktiveLoks[3].Name;
                LokCtrl4_Gattung.Text = AktiveLoks[3].Gattung;
            }
            AktiveLoks[3].Automatik = false;
            LokCtrl4_Strg_Typ.Text = "Manuell";
            LokCtrl4_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern4(sender, e);
        }
        private void LokCtrl5_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl5_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl5_Name.Text = String.Format("Lok: {0}", LokCtrl5_Adr.Value);
                AktiveLoks[4] = new Lok() { Adresse = (int)LokCtrl5_Adr.Value };
            }
            else
            {
                AktiveLoks[4] = Lokliste[index];
                LokCtrl5_Name.Text = AktiveLoks[4].Name;
                LokCtrl5_Gattung.Text = AktiveLoks[4].Gattung;
            }
            AktiveLoks[4].Automatik = false;
            LokCtrl5_Strg_Typ.Text = "Manuell";
            LokCtrl5_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern5(sender, e);
        }
        private void LokCtrl6_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl6_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl6_Name.Text = String.Format("Lok: {0}", LokCtrl6_Adr.Value);
                AktiveLoks[5] = new Lok() { Adresse = (int)LokCtrl6_Adr.Value };
            }
            else
            {
                AktiveLoks[5] = Lokliste[index];
                LokCtrl6_Name.Text = AktiveLoks[5].Name;
                LokCtrl6_Gattung.Text = AktiveLoks[5].Gattung;
            }
            AktiveLoks[5].Automatik = false;
            LokCtrl6_Strg_Typ.Text = "Manuell";
            LokCtrl6_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern6(sender, e);
        }
        private void LokCtrl7_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl7_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl7_Name.Text = String.Format("Lok: {0}", LokCtrl7_Adr.Value);
                AktiveLoks[6] = new Lok() { Adresse = (int)LokCtrl7_Adr.Value };
            }
            else
            {
                AktiveLoks[6] = Lokliste[index];
                LokCtrl7_Name.Text = AktiveLoks[6].Name;
                LokCtrl7_Gattung.Text = AktiveLoks[6].Gattung;
            }
            AktiveLoks[6].Automatik = false;
            LokCtrl7_Strg_Typ.Text = "Manuell";
            LokCtrl7_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern7(sender, e);
        }
        private void LokCtrl8_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl8_Adr.Value);                   //Finde Lok mit dieser Adresse                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl8_Name.Text = String.Format("Lok: {0}", LokCtrl8_Adr.Value);
                AktiveLoks[7] = new Lok() { Adresse = (int)LokCtrl8_Adr.Value };
            }
            else
            {
                AktiveLoks[7] = Lokliste[index];
                LokCtrl8_Name.Text = AktiveLoks[7].Name;
                LokCtrl8_Gattung.Text = AktiveLoks[7].Gattung;
            }
            AktiveLoks[7].Automatik = false;
            LokCtrl8_Strg_Typ.Text = "Manuell";
            LokCtrl8_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern8(sender, e);
        }
        private void LokCtrl9_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl9_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl9_Name.Text = String.Format("Lok: {0}", LokCtrl9_Adr.Value);
                AktiveLoks[8] = new Lok() { Adresse = (int)LokCtrl9_Adr.Value };
            }
            else
            {
                AktiveLoks[8] = Lokliste[index];
                LokCtrl9_Name.Text = AktiveLoks[8].Name;
                LokCtrl9_Gattung.Text = AktiveLoks[8].Gattung;
            }
            AktiveLoks[8].Automatik = false;
            LokCtrl9_Strg_Typ.Text = "Manuell";
            LokCtrl9_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern9(sender, e);
        }
        private void LokCtrl10_Adr_ValueChanged(object sender, EventArgs e)
        {
            int index = Lokliste.FindIndex(x => x.Adresse == LokCtrl10_Adr.Value);                   //Finde Lok mit dieser Adresse 
            if (index == -1)//Lok nicht gefunden in der Liste
            {
                LokCtrl10_Name.Text = String.Format("Lok: {0}", LokCtrl10_Adr.Value);
                AktiveLoks[9] = new Lok() { Adresse = (int)LokCtrl10_Adr.Value };
            }
            else
            {
                AktiveLoks[9] = Lokliste[index];
                LokCtrl10_Name.Text = AktiveLoks[9].Name;
                LokCtrl10_Gattung.Text = AktiveLoks[9].Gattung;
            }
            AktiveLoks[9].Automatik = false;
            LokCtrl10_Strg_Typ.Text = "Manuell";
            LokCtrl10_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
            //Update Rest
            Update_Rufnummern10(sender, e);
        }
        #endregion
        #region Lok Rufnummern ändern
        private void Update_Rufnummern1(object sender, EventArgs e)
        {
            AktiveLoks[0].Gattung = LokCtrl1_Gattung.Text;
            if (AktiveLoks[0].Adresse != 0) LokCtrl1_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[0].Gattung) + AktiveLoks[0].Adresse.ToString();
            else LokCtrl1_Ruf.Text = "";
        }
        private void Update_Rufnummern2(object sender, EventArgs e)
        {
            AktiveLoks[1].Gattung = LokCtrl2_Gattung.Text;
            if (AktiveLoks[1].Adresse != 0) LokCtrl2_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[1].Gattung) + AktiveLoks[1].Adresse.ToString();
            else LokCtrl2_Ruf.Text = "";
        }
        private void Update_Rufnummern3(object sender, EventArgs e)
        {
            AktiveLoks[2].Gattung = LokCtrl3_Gattung.Text;
            if (AktiveLoks[2].Adresse != 0) LokCtrl3_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[2].Gattung) + AktiveLoks[2].Adresse.ToString();
            else LokCtrl3_Ruf.Text = "";
        }
        private void Update_Rufnummern4(object sender, EventArgs e)
        {
            AktiveLoks[3].Gattung = LokCtrl4_Gattung.Text;
            if (AktiveLoks[3].Adresse != 0) LokCtrl4_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[3].Gattung) + AktiveLoks[3].Adresse.ToString();
            else LokCtrl4_Ruf.Text = "";
        }
        private void Update_Rufnummern5(object sender, EventArgs e)
        {
            AktiveLoks[4].Gattung = LokCtrl5_Gattung.Text;
            if (AktiveLoks[4].Adresse != 0) LokCtrl5_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[4].Gattung) + AktiveLoks[4].Adresse.ToString();
            else LokCtrl5_Ruf.Text = "";
        }
        private void Update_Rufnummern6(object sender, EventArgs e)
        {
            AktiveLoks[5].Gattung = LokCtrl6_Gattung.Text;
            if (AktiveLoks[5].Adresse != 0) LokCtrl6_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[5].Gattung) + AktiveLoks[5].Adresse.ToString();
            else LokCtrl6_Ruf.Text = "";
        }
        private void Update_Rufnummern7(object sender, EventArgs e)
        {
            AktiveLoks[6].Gattung = LokCtrl7_Gattung.Text;
            if (AktiveLoks[6].Adresse != 0) LokCtrl7_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[6].Gattung) + AktiveLoks[6].Adresse.ToString();
            else LokCtrl7_Ruf.Text = "";
        }
        private void Update_Rufnummern8(object sender, EventArgs e)
        {
            AktiveLoks[7].Gattung = LokCtrl8_Gattung.Text;
            if (AktiveLoks[7].Adresse != 0) LokCtrl8_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[7].Gattung) + AktiveLoks[7].Adresse.ToString();
            else LokCtrl8_Ruf.Text = "";
        }
        private void Update_Rufnummern9(object sender, EventArgs e)
        {
            AktiveLoks[8].Gattung = LokCtrl9_Gattung.Text;
            if (AktiveLoks[8].Adresse != 0) LokCtrl9_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[8].Gattung) + AktiveLoks[8].Adresse.ToString();
            else LokCtrl9_Ruf.Text = "";
        }
        private void Update_Rufnummern10(object sender, EventArgs e)
        {
            AktiveLoks[9].Gattung = LokCtrl10_Gattung.Text;
            if (AktiveLoks[9].Adresse != 0) LokCtrl10_Ruf.Text = LokKontrolle.Abkuerzung(AktiveLoks[9].Gattung) + AktiveLoks[9].Adresse.ToString();
            else LokCtrl10_Ruf.Text = "";
        }
        #endregion
        #region Lok Fahrpulte öffnen
        private void OpenFahrpult1_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl1_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[0].Steuerpult.IsDisposed)
            {
                AktiveLoks[0].Steuerpult.Dispose();
            }
            AktiveLoks[0].Steuerpult = new ZugSteuerpult(AktiveLoks[0]);
            AktiveLoks[0].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[0].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[0].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[0].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[0].Adresse);

        }
        private void OpenFahrpult2_Click(object sender, EventArgs e)
        {
            if(! (Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            if(LokCtrl2_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[1].Steuerpult.IsDisposed)
            {
                AktiveLoks[1].Steuerpult.Dispose();
            }
            AktiveLoks[1].Steuerpult = new ZugSteuerpult(AktiveLoks[1]);
            AktiveLoks[1].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[1].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[1].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[1].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[1].Adresse);
        }
        private void OpenFahrpult3_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl3_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[2].Steuerpult.IsDisposed)
            {
                AktiveLoks[2].Steuerpult.Dispose();
            }
            AktiveLoks[2].Steuerpult = new ZugSteuerpult(AktiveLoks[2]);
            AktiveLoks[2].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[2].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[2].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[2].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[2].Adresse);
        }
        private void OpenFahrpult4_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl4_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[3].Steuerpult.IsDisposed)
            {
                AktiveLoks[3].Steuerpult.Dispose();
            }
            AktiveLoks[3].Steuerpult = new ZugSteuerpult(AktiveLoks[3]);
            AktiveLoks[3].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[3].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[3].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[3].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[3].Adresse);
        }
        private void OpenFahrpult5_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl5_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[4].Steuerpult.IsDisposed)
            {
                AktiveLoks[4].Steuerpult.Dispose();
            }
            AktiveLoks[4].Steuerpult = new ZugSteuerpult(AktiveLoks[4]);
            AktiveLoks[4].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[4].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[4].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[4].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[4].Adresse);
        }
        private void OpenFahrpult6_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl6_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[5].Steuerpult.IsDisposed)
            {
                AktiveLoks[5].Steuerpult.Dispose();
            }
            AktiveLoks[5].Steuerpult = new ZugSteuerpult(AktiveLoks[5]);
            AktiveLoks[5].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[5].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[5].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[5].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[5].Adresse);
        }
        private void OpenFahrpult7_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl7_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[6].Steuerpult.IsDisposed)
            {
                AktiveLoks[6].Steuerpult.Dispose();
            }
            AktiveLoks[6].Steuerpult = new ZugSteuerpult(AktiveLoks[6]);
            AktiveLoks[6].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[6].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[6].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[6].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[6].Adresse);
        }
        private void OpenFahrpult8_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl8_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[7].Steuerpult.IsDisposed)
            {
                AktiveLoks[7].Steuerpult.Dispose();
            }
            AktiveLoks[7].Steuerpult = new ZugSteuerpult(AktiveLoks[7]);
            AktiveLoks[7].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[7].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[7].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[7].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[7].Adresse);
        }
        private void OpenFahrpult9_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl9_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[8].Steuerpult.IsDisposed)
            {
                AktiveLoks[8].Steuerpult.Dispose();
            }
            AktiveLoks[8].Steuerpult = new ZugSteuerpult(AktiveLoks[8]);
            AktiveLoks[8].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[8].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[8].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[8].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[8].Adresse);
        }
        private void OpenFahrpult10_Click(object sender, EventArgs e)
        {
            if (!(Weichen_Init & Signal_Init))
            {
                MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LokCtrl10_Adr.Value == 0)
            {
                MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!AktiveLoks[9].Steuerpult.IsDisposed)
            {
                AktiveLoks[9].Steuerpult.Dispose();
            }
            AktiveLoks[9].Steuerpult = new ZugSteuerpult(AktiveLoks[9]);
            AktiveLoks[9].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            AktiveLoks[9].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks[9].Register_CMD_LOKSTATUS(Setze_Lok_Status);
            AktiveLoks[9].Steuerpult.Show();
            z21Start.Z21_GET_LOCO_INFO(AktiveLoks[9].Adresse);
        }
        #endregion
        #region Lok Nothalte
        private void LokCtrlStop1_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[0].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[0].Adresse, 255, AktiveLoks[0].Richtung, AktiveLoks[0].FahrstufenInfo);
            }
        }
        private void LokCtrlStop2_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[1].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[1].Adresse, 255, AktiveLoks[1].Richtung, AktiveLoks[1].FahrstufenInfo);
            }
        }
        private void LokCtrlStop3_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[2].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[2].Adresse, 255, AktiveLoks[2].Richtung, AktiveLoks[2].FahrstufenInfo);
            }
        }
        private void LokCtrlStop4_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[3].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[3].Adresse, 255, AktiveLoks[3].Richtung, AktiveLoks[3].FahrstufenInfo);
            }
        }
        private void LokCtrlStop5_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[4].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[4].Adresse, 255, AktiveLoks[4].Richtung, AktiveLoks[4].FahrstufenInfo);
            }
        }
        private void LokCtrlStop6_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[5].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[5].Adresse, 255, AktiveLoks[5].Richtung, AktiveLoks[5].FahrstufenInfo);
            }
        }
        private void LokCtrlStop7_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[6].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[6].Adresse, 255, AktiveLoks[6].Richtung, AktiveLoks[6].FahrstufenInfo);
            }
        }
        private void LokCtrlStop8_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[7].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[7].Adresse, 255, AktiveLoks[7].Richtung, AktiveLoks[7].FahrstufenInfo);
            }
        }
        private void LokCtrlStop9_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[8].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[8].Adresse, 255, AktiveLoks[8].Richtung, AktiveLoks[8].FahrstufenInfo);
            }
        }
        private void LokCtrlStop10_Click(object sender, EventArgs e)
        {
            if (AktiveLoks[9].Adresse != 0)
            {
                Setze_Lok_Fahrt(AktiveLoks[9].Adresse, 255, AktiveLoks[9].Richtung, AktiveLoks[9].FahrstufenInfo);
            }
        }
        private void StopAlle_Click(object sender, EventArgs e)
        {
            LokCtrlStop1_Click(sender, e);
            LokCtrlStop2_Click(sender, e);
            LokCtrlStop3_Click(sender, e);
            LokCtrlStop4_Click(sender, e);
            LokCtrlStop5_Click(sender, e);
            LokCtrlStop6_Click(sender, e);
            LokCtrlStop7_Click(sender, e);
            LokCtrlStop8_Click(sender, e);
            LokCtrlStop9_Click(sender, e);
            LokCtrlStop10_Click(sender, e);
        }
        #endregion
        #region Lok Auto- / Manuellsteuerung
        private void LokCtrl1_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[0].Automatik)
            {
                LokCtrl1_Strg_Typ.Text = "Automatik";
                LokCtrl1_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[0].Automatik = true;
            }
            else
            {
                LokCtrl1_Strg_Typ.Text = "Manuell";
                LokCtrl1_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[0].Automatik = false;
            }
        }
        private void LokCtrl2_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[1].Automatik)
            {
                LokCtrl2_Strg_Typ.Text = "Automatik";
                LokCtrl2_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[1].Automatik = true;
            }
            else
            {
                LokCtrl2_Strg_Typ.Text = "Manuell";
                LokCtrl2_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[1].Automatik = false;
            }
        }
        private void LokCtrl3_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[2].Automatik)
            {
                LokCtrl3_Strg_Typ.Text = "Automatik";
                LokCtrl3_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[2].Automatik = true;
            }
            else
            {
                LokCtrl3_Strg_Typ.Text = "Manuell";
                LokCtrl3_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[2].Automatik = false;
            }
        }
        private void LokCtrl4_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[3].Automatik)
            {
                LokCtrl4_Strg_Typ.Text = "Automatik";
                LokCtrl4_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[3].Automatik = true;
            }
            else
            {
                LokCtrl4_Strg_Typ.Text = "Manuell";
                LokCtrl4_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[3].Automatik = false;
            }
        }
        private void LokCtrl5_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[4].Automatik)
            {
                LokCtrl5_Strg_Typ.Text = "Automatik";
                LokCtrl5_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[4].Automatik = true;
            }
            else
            {
                LokCtrl5_Strg_Typ.Text = "Manuell";
                LokCtrl5_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[4].Automatik = false;
            }
        }
        private void LokCtrl6_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[5].Automatik)
            {
                LokCtrl6_Strg_Typ.Text = "Automatik";
                LokCtrl6_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[5].Automatik = true;
            }
            else
            {
                LokCtrl6_Strg_Typ.Text = "Manuell";
                LokCtrl6_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[5].Automatik = false;
            }
        }
        private void LokCtrl7_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[6].Automatik)
            {
                LokCtrl7_Strg_Typ.Text = "Automatik";
                LokCtrl7_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[6].Automatik = true;
            }
            else
            {
                LokCtrl7_Strg_Typ.Text = "Manuell";
                LokCtrl7_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[6].Automatik = false;
            }
        }
        private void LokCtrl8_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[7].Automatik)
            {
                LokCtrl8_Strg_Typ.Text = "Automatik";
                LokCtrl8_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[7].Automatik = true;
            }
            else
            {
                LokCtrl8_Strg_Typ.Text = "Manuell";
                LokCtrl8_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[7].Automatik = false;
            }
        }
        private void LokCtrl9_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[8].Automatik)
            {
                LokCtrl9_Strg_Typ.Text = "Automatik";
                LokCtrl9_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[8].Automatik = true;
            }
            else
            {
                LokCtrl9_Strg_Typ.Text = "Manuell";
                LokCtrl9_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[8].Automatik = false;
            }
        }
        private void LokCtrl10_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (!AktiveLoks[9].Automatik)
            {
                LokCtrl10_Strg_Typ.Text = "Automatik";
                LokCtrl10_Strg_Typ.BackColor = Color.FromArgb(128, 0, 0);
                AktiveLoks[9].Automatik = true;
            }
            else
            {
                LokCtrl10_Strg_Typ.Text = "Manuell";
                LokCtrl10_Strg_Typ.BackColor = Color.FromArgb(0, 128, 0);
                AktiveLoks[9].Automatik = false;
            }
        }
        #endregion
        private void LokEditorOpen_Click(object sender, EventArgs e)
        {
            LokEditor lokEditor = new LokEditor();
            lokEditor.Show();
        }

        

        

        
    }
}
