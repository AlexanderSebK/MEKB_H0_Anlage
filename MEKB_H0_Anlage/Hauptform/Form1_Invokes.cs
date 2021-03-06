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

        }
        private void UpdateWeiche(int Adresse, int Status)
        {
            int index = Weichenliste.FindIndex(x => x.Adresse == Adresse); //Finde Weiche mit dieser Adresse 
            if (index != -1)//Weiche gefunden in der Liste
            {
                Weichenliste[index].Schalten(Status);
                UpdateWeicheImGleisplan(Weichenliste[index]);
            }
            else
            {
                index = Signalliste.FindIndex(x => x.Adresse == Adresse);
                if (index != -1)//Signal gefunden in der 1. Adressen
                {
                    if (!Signalliste[index].Letzte_Adresswahl)
                    {
                        Signalliste[index].MaskenSetzen(Status);
                        UpdateSignalImGleisplan(Signalliste[index]);
                    }
                }
                else
                {
                    index = Signalliste.FindIndex(x => x.Adresse2 == Adresse);
                    if (index != -1)//Signal gefunden in der 2. Adressen
                    {
                        if (Signalliste[index].Letzte_Adresswahl)
                        {
                            Signalliste[index].MaskenSetzen(Status + 4);
                            UpdateSignalImGleisplan(Signalliste[index]);
                        }
                    }
                }

            }
        }
        private void UpdateWeicheImGleisplan(Weiche weiche)
        {
            
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);     //Zweite Weiche bei DKWs und KWs DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche30_Gleis1);
            try
            {
                switch (weiche.Name)
                {
                    case "Weiche1":  DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche1) ; break;
                    case "Weiche2":  DisplayPicture(GetSchaltbildWeicheR270(weiche),Weiche2) ; break;
                    case "Weiche3":  DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche3) ; break;
                    case "Weiche4":  DisplayPicture(GetSchaltbildWeicheL270(weiche),Weiche4) ; break;
                    case "Weiche5":  DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche5) ; break;
                    case "Weiche6":  DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche6) ; break;
                    case "Weiche8":  DisplayPicture(GetSchaltbildWeicheL315(weiche),Weiche8) ; break;
                    case "Weiche21": DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche21); break;
                    case "Weiche23": DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche23); break;
                    case "Weiche25": DisplayPicture(GetSchaltbildWeicheL270(weiche),Weiche25); break;
                    case "Weiche26": DisplayPicture(GetSchaltbildWeicheR270(weiche),Weiche26); break;
                    case "Weiche27": DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche27); break;
                    case "Weiche28": DisplayPicture(GetSchaltbildWeicheL270(weiche),Weiche28); break;
                    case "Weiche29": DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche29); break;
                    case "Weiche30": DisplayPicture(GetSchaltbildWeicheR270(weiche),Weiche30); break;
                    case "Weiche50": DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche50); break;
                    case "Weiche51": DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche51); break;
                    case "Weiche52": DisplayPicture(GetSchaltbildWeicheL180(weiche),Weiche52); break;
                    case "Weiche53": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche53); break;
                    case "Weiche60": DisplayPicture(GetSchaltbildWeicheR270(weiche),Weiche60); break;
                    case "Weiche61": DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche61); break;
                    case "Weiche62": DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche62); break;
                    case "Weiche63": DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche63); break;
                    case "Weiche64": DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche64); break;
                    case "Weiche70": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche70); break;
                    case "Weiche71": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche71); break;
                    case "Weiche72": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche72); break;
                    case "Weiche73": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche73); break;
                    case "Weiche74": DisplayPicture(GetSchaltbildWeicheR225(weiche),Weiche74); break;
                    case "DKW7_1":   DisplayPicture(GetSchaltbildDKW90_135(weiche, DKW_2nd),DKW7) ;break;
                    case "DKW7_2":   DisplayPicture(GetSchaltbildDKW90_135(DKW_2nd, weiche),DKW7) ;break; //Spiegelverkehrt, da der zweite Weichenteil die aktuelle zu Schaltene ist     
                        
                    case "DKW9_1":   DisplayPicture(GetSchaltbildDKW90_135(weiche, DKW_2nd),DKW9) ;break;
                    case "DKW9_2":   DisplayPicture(GetSchaltbildDKW90_135(DKW_2nd, weiche),DKW9) ;break;  
                        
                    case "KW22_1":   DisplayPicture(GetSchaltbildKW90_45(weiche, DKW_2nd)  ,KW22) ;break;
                    case "KW22_2":   DisplayPicture(GetSchaltbildKW90_45(DKW_2nd, weiche)  ,KW22) ;break;

                    case "DKW24_1":  DisplayPicture(GetSchaltbildDKW90_45(weiche, DKW_2nd) ,DKW24); break;
                    case "DKW24_2":  DisplayPicture(GetSchaltbildDKW90_45(DKW_2nd, weiche), DKW24); break;
                    default: break;
                }
            }
            catch
            {

            }
        }      
        private Weiche GetDWK_2nd(String name)
        {
            int ListID;
            switch(name)
            {
                case "DKW7_1": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" }); break;
                case "DKW7_2": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" }); break;
                case "DKW9_1": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" }); break;
                case "DKW9_2": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" }); break;
                case "KW22_1": ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" }); break;
                case "KW22_2": ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" }); break;
                case "DKW24_1": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" }); break;
                case "DKW24_2": ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" }); break;
                default: ListID = -1;break;
            }
            if(ListID == -1)
            {
                Weiche Fehler = new Weiche() { Status_Error = true, Name = "Fehler" };
                return Fehler;
            }
            else
            {
                return Weichenliste[ListID];
            }

        }
        private void UpdateSignalImGleisplan(Signal signal)
        {
            try
            {
                switch (signal.Name)
                {
                    case "Signal_Ausfahrt_L1": GetSignalSchaltbild(signal, Signal_Ausfahrt_L1); break;
                    case "Signal_Ausfahrt_L2": GetSignalSchaltbild(signal, Signal_Ausfahrt_L2); break;
                    case "Signal_Ausfahrt_L3": GetSignalSchaltbild(signal, Signal_Ausfahrt_L3); break;
                    case "Signal_Ausfahrt_L4": GetSignalSchaltbild(signal, Signal_Ausfahrt_L4); break;
                    case "Signal_Ausfahrt_L5": GetSignalSchaltbild(signal, Signal_Ausfahrt_L5); break;
                    case "Signal_Ausfahrt_L6": GetSignalSchaltbild(signal, Signal_Ausfahrt_L6); break;
                    case "Signal_Ausfahrt_R1": GetSignalSchaltbild(signal, Signal_Ausfahrt_R1); break;
                    case "Signal_Ausfahrt_R2": GetSignalSchaltbild(signal, Signal_Ausfahrt_R2); break;
                    case "Signal_Ausfahrt_R3": GetSignalSchaltbild(signal, Signal_Ausfahrt_R3); break;
                    case "Signal_Ausfahrt_R4": GetSignalSchaltbild(signal, Signal_Ausfahrt_R4); break;
                    case "Signal_Ausfahrt_R5": GetSignalSchaltbild(signal, Signal_Ausfahrt_R5); break;
                    case "Signal_Ausfahrt_R6": GetSignalSchaltbild(signal, Signal_Ausfahrt_R6); break;
                    case "Signal_RTunnel_1":   GetSignalSchaltbild(signal, Signal_RTunnel_1);   break;
                    case "Signal_RTunnel_2":   GetSignalSchaltbild(signal, Signal_RTunnel_2);   break;
                    case "Signal_Einfahrt_L":  GetSignalSchaltbild(signal, Signal_Einfahrt_L);  break;
                    case "Signal_Tunnel_L1":   GetSignalSchaltbild(signal, Signal_Tunnel_L1);   break;
                    default: break;
                }
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
    }
}
