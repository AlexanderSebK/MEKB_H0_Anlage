

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


namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Hauptform
    /// </summary>
    public partial class Hauptform : Form
    {
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
            this.BeginInvoke((Action<bool,bool>)ConnectStatus, Status, Init);
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
            this.BeginInvoke((Action<int, int, bool, byte, bool, byte, bool, bool, bool[]>)UpdateLok, ParamterCount,  Addresse,  Besetzt,  FahrstufenInfo,  Richtung,
                                              Fahrstufe,  Doppeltraktio,  Smartsearch, Funktionen);
        }

        public void CallBack_Z21_TrackStatus(byte status)
        {

        }

        public void CallBack_LAN_RMBUS_DATACHANGED(byte GruppenIndex, byte[] RMStatus)
        {
            this.BeginInvoke((Action<byte, byte[]>)UpdateBelegtmeldung, GruppenIndex, RMStatus);
        }
    }
}
