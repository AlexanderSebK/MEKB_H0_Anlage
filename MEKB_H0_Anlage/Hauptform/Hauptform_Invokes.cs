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
    public partial class Hauptform : Form
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
        /// <summary>
        /// Aktuellen Verbindungsstatus festlegen und anziegen
        /// </summary>
        /// <param name="status">WAHR: Z21 ist verbunden</param>
        /// <param name="init">WAHR: Z21 wurde bereits initialisiert</param>
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
        private void UpdateWeiche(int Adresse, int Status)
        {
            Weiche weiche = WeichenListe.GetWeiche(Adresse); //Finde Weiche mit dieser Adresse 
            if (weiche != null)//Weiche gefunden in der Liste
            {
                weiche.StatusUpdate(Status);
                GleisplanUpdateWeiche(weiche);       
            }
            else
            {
                // Versuche Signalzustand zu aktualisieren
                if(SignalListe.UpdateSignalZustand(Adresse, Status))
                {
                    // Update erfolgreich (inkl. Signal gefunden)
                    GleisplanUpdateSignal(SignalListe.GetSignal(Adresse));               
                }
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
