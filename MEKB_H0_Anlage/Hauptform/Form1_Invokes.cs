using System;
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
using System.Globalization;


namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
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
        public void ConnectStatus(bool status,bool init)
        {
            Menu_Trennen.Enabled = status;
            Menu_Verbinden.Enabled = !status;
            if (status)
            {
                if (init)
                {
                    HauptStatusbar.Text = "Z21: Verbunden";
                    HauptStatusbar.BackColor = Color.ForestGreen;
                    HauptStatusbar.ForeColor = Color.White;
                }
                else
                {
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
        private void UpdateWeiche(int Adresse, int Status)
        {
            Weiche weiche = WeichenListe.GetWeiche(Adresse); //Finde Weiche mit dieser Adresse 
            if (weiche != null)//Weiche gefunden in der Liste
            {
                bool aenderung = weiche.Schalten(Status);
                UpdateWeicheImGleisplan(weiche, aenderung); //Mit Signal-Update
            }
            else
            {
                Signal signal = SignalListe.GetSignalErsteAdresse(Adresse);
                if (signal != null)//Signal gefunden in der 1. Adressen
                {
                    if (!signal.Letzte_Adresswahl)
                    {
                        signal.MaskenSetzen(Status);
                        UpdateSignalImGleisplan(signal);
                    }
                }
                else
                {
                    signal = SignalListe.GetSignalZweiteAdresse(Adresse);
                    if (signal != null)//Signal gefunden in der 2. Adressen
                    {
                        if (signal.Letzte_Adresswahl)
                        {
                            signal.MaskenSetzen(Status + 4);
                            UpdateSignalImGleisplan(signal);
                        }
                    }
                }
            }
        }
        private void UpdateWeicheImGleisplan(Weiche weiche, bool signalUpdate = false)
        {
            
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);     //Zweite Weiche bei DKWs und KWs DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche30_Gleis1);
            try
            {
                switch (weiche.Name)
                {
                    case "Weiche1":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche1, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_L1");
                            AutoSignalUpdate("Signal_Ausfahrt_L2");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "Weiche2":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche2, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "Weiche3":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche3, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "Weiche4":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche4, true) ;
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche3").Abzweig) AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L1");
                            AutoSignalUpdate("Signal_Ausfahrt_L2");
                        }
                        break;
                    case "Weiche5":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche5, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "Weiche6":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche6, true) ;
                        if (signalUpdate)
                        {
                            if(WeichenListe.GetWeiche("Weiche3").Abzweig) AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L1");
                            AutoSignalUpdate("Signal_Ausfahrt_L2");
                        }
                        break;
                    case "Weiche8":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche8, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;

                    case "Weiche21":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche21, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche23":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche23, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche25":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche25, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R3");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "Weiche26":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche26, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche27":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche27, true);
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            AutoSignalUpdate("Signal_Ausfahrt_R3");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "Weiche28":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche28, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            AutoSignalUpdate("Signal_RTunnel_1");
                            AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche29":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche29, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            AutoSignalUpdate("Signal_RTunnel_1");
                            AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche30":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche30, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            AutoSignalUpdate("Signal_Ausfahrt_R3");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "Weiche50":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche50, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Ausfahrt_R1");
                            AutoSignalUpdate("Signal_Ausfahrt_R2");
                            AutoSignalUpdate("Signal_Ausfahrt_R3");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "Weiche51":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche51, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_RTunnel_1");
                            AutoSignalUpdate("Signal_RTunnel_2");
                        }
                        break;
                    case "Weiche52":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche52, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Block5");
                        }
                        break;
                    case "Weiche53":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche53, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Block2");
                            if (WeichenListe.GetWeiche("Weiche52").Abzweig) AutoSignalUpdate("Signal_Block5");
                        }
                        break;

                    case "Weiche60":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche60, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Block6");
                            AutoSignalUpdate("Signal_Block8");
                        }
                        break;
                    case "Weiche61":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche61, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                            AutoSignalUpdate("Signal_Schatten3");
                            AutoSignalUpdate("Signal_Schatten4");
                            AutoSignalUpdate("Signal_Schatten5");
                            AutoSignalUpdate("Signal_Schatten6");
                            AutoSignalUpdate("Signal_Schatten7");
                        }
                        break;
                    case "Weiche62":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche62, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                            AutoSignalUpdate("Signal_Schatten3");
                            AutoSignalUpdate("Signal_Schatten4");
                            AutoSignalUpdate("Signal_Schatten5");
                            AutoSignalUpdate("Signal_Schatten6");
                        }
                        break;
                    case "Weiche63":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche63, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                            AutoSignalUpdate("Signal_Schatten3");
                            AutoSignalUpdate("Signal_Schatten4");
                            AutoSignalUpdate("Signal_Schatten5");
                        }
                        break;
                    case "Weiche64":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche64, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                            AutoSignalUpdate("Signal_Schatten3");
                            AutoSignalUpdate("Signal_Schatten4");
                        }
                        break;
                    case "Weiche65":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche65, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                            AutoSignalUpdate("Signal_Schatten3");
                        }
                        break;
                    case "Weiche66":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche66, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                            AutoSignalUpdate("Signal_Schatten2");
                        }
                        break;
                    case "Weiche67":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche67, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten1");
                        }
                        break;
                    case "Weiche68":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche68, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten0");
                            AutoSignalUpdate("Signal_Schatten1");
                        }
                        break;

                    case "Weiche70":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche70, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche71":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche71, true);
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche70").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break; 
                    case "Weiche72":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche72, true);
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche71").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche73":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche73, true);
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche72").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche74":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche74, true);
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche73").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche75":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche75, true);
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche74").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche76":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche76, true); 
                        if (signalUpdate && !WeichenListe.GetWeiche("Weiche75").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche80":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche80, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten8");
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche81":
                        Weiche weiche80 = WeichenListe.GetWeiche("Weiche80");
                        if (weiche80.Abzweig)
                        {
                            MeldeZustand meldeZustand = new MeldeZustand(weiche80, Weiche80.Tag.ToString().EndsWith("_Gegen"));
                            GleisbildZeichnung.ZeichneSchaltbild(weiche, meldeZustand, Weiche81, true);
                        }
                        else
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(weiche, FreiesGleis, Weiche81, true);
                        }
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten9");
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;
                    case "Weiche82":
                        Weiche weiche81 = WeichenListe.GetWeiche("Weiche81");
                        if (weiche81.Abzweig)
                        {
                            MeldeZustand meldeZustand = new MeldeZustand(weiche81, Weiche81.Tag.ToString().EndsWith("_Gegen"));
                            GleisbildZeichnung.ZeichneSchaltbild(weiche, meldeZustand, Weiche82, true);
                        }
                        else
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(weiche, FreiesGleis, Weiche82, true);
                        }
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten10");
                            AutoSignalUpdate("Signal_Schatten11");
                        }
                        break;

                    case "Weiche90":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche90, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Schatten_Einf");
                        }
                        break;
                    case "Weiche91":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche91, true);
                        if (signalUpdate && WeichenListe.GetWeiche("Weiche90").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten_Einf");
                        }
                        break;
                    case "Weiche92":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche92, true);
                        if (signalUpdate && WeichenListe.GetWeiche("Weiche90").Abzweig && !WeichenListe.GetWeiche("Weiche91").Abzweig)
                        {
                            AutoSignalUpdate("Signal_Schatten_Einf");
                        }
                        break;

                    case "DKW7_1":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd,DKW7, true);
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "DKW7_2":
                        GleisbildZeichnung.ZeichneSchaltbild(DKW_2nd, weiche, DKW7, true);//Spiegelverkehrt, da der zweite Weichenteil die aktuelle zu Schaltene ist   
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L3");
                            AutoSignalUpdate("Signal_Ausfahrt_L4");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;   
                        
                    case "DKW9_1":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, DKW9, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;
                    case "DKW9_2":
                        GleisbildZeichnung.ZeichneSchaltbild(DKW_2nd, weiche, DKW9, true) ;
                        if (signalUpdate)
                        {
                            AutoSignalUpdate("Signal_Einfahrt_L");
                            AutoSignalUpdate("Signal_Ausfahrt_L5");
                            AutoSignalUpdate("Signal_Ausfahrt_L6");
                        }
                        break;  
                        
                    case "KW22_1":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, KW22, true);
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "KW22_2":
                        GleisbildZeichnung.ZeichneSchaltbild(DKW_2nd, weiche, KW22, true);
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;

                    case "DKW24_1":
                        GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, DKW24, true);
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    case "DKW24_2":
                        GleisbildZeichnung.ZeichneSchaltbild(DKW_2nd, weiche, DKW24, true);
                        if (signalUpdate)
                        {
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_1");
                            if (WeichenListe.GetWeiche("Weiche28").Abzweig) AutoSignalUpdate("Signal_RTunnel_2");
                            AutoSignalUpdate("Signal_Ausfahrt_R4");
                            AutoSignalUpdate("Signal_Ausfahrt_R5");
                            AutoSignalUpdate("Signal_Ausfahrt_R6");
                        }
                        break;
                    default: break;
                }
            }
            catch
            {

            }
        }      
        private Weiche GetDWK_2nd(String name)
        {
            Weiche DKW_2nd;
            switch(name)
            {
                case "DKW7_1": DKW_2nd = WeichenListe.GetWeiche("DKW7_2"); break;
                case "DKW7_2": DKW_2nd = WeichenListe.GetWeiche("DKW7_1"); break;
                case "DKW9_1": DKW_2nd = WeichenListe.GetWeiche("DKW9_2"); break;
                case "DKW9_2": DKW_2nd = WeichenListe.GetWeiche("DKW9_1"); break;
                case "KW22_1": DKW_2nd = WeichenListe.GetWeiche("KW22_2"); break;
                case "KW22_2": DKW_2nd = WeichenListe.GetWeiche("KW22_1"); break;
                case "DKW24_1": DKW_2nd = WeichenListe.GetWeiche("DKW24_2"); break;
                case "DKW24_2": DKW_2nd = WeichenListe.GetWeiche("DKW24_1"); break;
                default: DKW_2nd = new Weiche() { Status_Error = true, Name = "Fehler" }; break;
            }
            return DKW_2nd;
        }
        private void UpdateSignalImGleisplan(Signal signal)
        {
            try
            {
                //Passendes Signal heraussuchen
                PictureBox SignalBild = (PictureBox)this.Controls.Find(signal.Name, true)[0];
                if (SignalBild == null) return; //Nicht gefunden: Abbrechen

                GetSignalSchaltbild(signal, SignalBild);               
            }
            catch
            {

            }
        }
        private void UpdateLok(int ParamterCount, int Adresse, bool Besetzt, byte FahrstufenInfo, bool Richtung,
                                             byte Fahrstufe, bool Doppeltraktio, bool Smartsearch, bool[] Funktionen)
        {
            int index = -1;
            for (int i = 0; i<AktiveLoks.Length;i++)
            {
                if(AktiveLoks[i] != null)
                {
                    if (AktiveLoks[i].Adresse == Adresse) index = i;
                }
            }
            if (index == -1) return;

            if (ParamterCount >= 3)
            {
                AktiveLoks[index].FahrstufenInfo = FahrstufenInfo;
            }
            if (ParamterCount >= 4)
            {
                int FahrRichtung = LokFahrstufen.Vorwaerts;
                if ((Richtung == true) && (AktiveLoks[index].LokUmgedreht == false)) FahrRichtung = LokFahrstufen.Vorwaerts;
                if ((Richtung == true) && (AktiveLoks[index].LokUmgedreht == true)) FahrRichtung = LokFahrstufen.Rueckwaerts;
                if ((Richtung == false) && (AktiveLoks[index].LokUmgedreht == false)) FahrRichtung = LokFahrstufen.Rueckwaerts;
                if ((Richtung == false) && (AktiveLoks[index].LokUmgedreht == true)) FahrRichtung = LokFahrstufen.Vorwaerts;
                AktiveLoks[index].Richtung = FahrRichtung;
                AktiveLoks[index].Fahrstufe = LokFahrstufen.ProtokolToFahrstufe(Fahrstufe, FahrstufenInfo);
            }
            if (ParamterCount >= 5)
            {
                AktiveLoks[index].AktiveFunktion[0] = Funktionen[0];
                AktiveLoks[index].AktiveFunktion[1] = Funktionen[1];
                AktiveLoks[index].AktiveFunktion[2] = Funktionen[2];
                AktiveLoks[index].AktiveFunktion[3] = Funktionen[3];
                AktiveLoks[index].AktiveFunktion[4] = Funktionen[4];
            }
            if (ParamterCount >= 6)
            {
                AktiveLoks[index].AktiveFunktion[5] = Funktionen[5];
                AktiveLoks[index].AktiveFunktion[6] = Funktionen[6];
                AktiveLoks[index].AktiveFunktion[7] = Funktionen[7];
                AktiveLoks[index].AktiveFunktion[8] = Funktionen[8];
                AktiveLoks[index].AktiveFunktion[9] = Funktionen[9];
                AktiveLoks[index].AktiveFunktion[10] = Funktionen[10];
                AktiveLoks[index].AktiveFunktion[11] = Funktionen[11];
                AktiveLoks[index].AktiveFunktion[12] = Funktionen[12];
            }
            if (ParamterCount >= 7)
            {
                AktiveLoks[index].AktiveFunktion[13] = Funktionen[13];
                AktiveLoks[index].AktiveFunktion[14] = Funktionen[14];
                AktiveLoks[index].AktiveFunktion[15] = Funktionen[15];
                AktiveLoks[index].AktiveFunktion[16] = Funktionen[16];
                AktiveLoks[index].AktiveFunktion[17] = Funktionen[17];
                AktiveLoks[index].AktiveFunktion[18] = Funktionen[18];
                AktiveLoks[index].AktiveFunktion[19] = Funktionen[19];
                AktiveLoks[index].AktiveFunktion[20] = Funktionen[20];
            }
            if (ParamterCount >= 8)
            {
                AktiveLoks[index].AktiveFunktion[21] = Funktionen[21];
                AktiveLoks[index].AktiveFunktion[22] = Funktionen[22];
                AktiveLoks[index].AktiveFunktion[23] = Funktionen[23];
                AktiveLoks[index].AktiveFunktion[24] = Funktionen[24];
                AktiveLoks[index].AktiveFunktion[25] = Funktionen[25];
                AktiveLoks[index].AktiveFunktion[26] = Funktionen[26];
                AktiveLoks[index].AktiveFunktion[27] = Funktionen[27];
                AktiveLoks[index].AktiveFunktion[28] = Funktionen[28];
            }
            if (!AktiveLoks[index].Steuerpult.IsDisposed)
            {
                AktiveLoks[index].Steuerpult.UpdateLokDaten();
            }
            //int index = Lokliste.FindIndex(x => x.Adresse == Adresse);

        }
        private void UpdateBelegtmeldung(byte GruppenIndex, byte[] RMStatus)
        {
            BelegtmelderListe.UpdateBelegtmelder(GruppenIndex, RMStatus);   
        }
    }
}
