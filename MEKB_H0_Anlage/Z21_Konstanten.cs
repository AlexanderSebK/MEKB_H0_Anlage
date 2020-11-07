using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Z21 FehlerCode
    /// Enthält Zahlencode und Fehlerbeschreibungen der Rückmeldungen der Z21 Klasse
    /// </summary>
    class Z21_ErrorCode
    {
        /// <summary>
        /// Paket wurde erfolgreich empfangen
        /// </summary>
        public const int SUCCESS = 0;
        /// <summary>
        /// Paket Längenangabe stimmt nicht überein
        /// </summary>
        public const int FALSE_LENGTH = 1;
        /// <summary>
        /// Header-Code passt nicht zum Nachrichten inhalt
        /// </summary>
        public const int WRONG_HEADER = 2;
        /// <summary>
        /// Checksumme ist falsch
        /// </summary>
        public const int FALSE_CHECK = 3;
        /// <summary>
        /// Wandelt Fehlerzahlencode in lesbare Fehlerbeschreibung um
        /// </summary>
        /// <param name="code">Fehlercode</param>
        /// <returns>Fehlerbeschreibung</returns>
        public string ToStirng(int code)
        {
            switch(code)
            {
                case 0: return "Kein Fehler";
                case 1: return "Falsche Paketlänge";
                case 2: return "Falsche ID";
                case 3: return "Falsche Checksumme";
                default: return "Unbekannt";
            }
        }
    }
    
    /// <summary>
    /// Packettyp wenn das X_BUS_TUNNEL (Z21-Header ID 0x40) verwendet wird
    /// </summary>
    class Z21_XBus_Header
    {
        public const int GET_FIRMWARE = 0xF3;
        public const int Weichen_INFO = 0x43;
    }
    /// <summary>
    /// Struktur für die Rückmelde-Optionen inkl. Umwandlung in sende-bares Datenpacket
    /// </summary>
    public struct Flags
    {
        /// <summary>
        /// Construktor - Erstellen einer Flag-Variable
        /// </summary>
        /// <param name="bitmap">16-Bit Integer des Datenpackets</param>
        public Flags(int bitmap)
        {
            Fahren_Schalten =   (0x00000001 & bitmap) == 0x00000001;
            RM_Bus =            (0x00000002 & bitmap) == 0x00000002;
            Railcom =           (0x00000004 & bitmap) == 0x00000004;
            System_Status =     (0x00000100 & bitmap) == 0x00000100;
            Alle_Lok_Info =     (0x00010000 & bitmap) == 0x00010000;
            LOCONET_Basic =     (0x01000000 & bitmap) == 0x01000000;
            LOCONET_Lok =       (0x02000000 & bitmap) == 0x02000000;
            LOCONET_Weichen =   (0x04000000 & bitmap) == 0x04000000;
            LOCONET_Detect =    (0x08000000 & bitmap) == 0x08000000;
            Alle_Railcom =      (0x00040000 & bitmap) == 0x00040000;
            CAN_Detect =        (0x00080000 & bitmap) == 0x00080000;
        }
        /// <summary>
        /// Option: automatische Rückantwort der Z21, wenn eigen gesteuerte Lokomotiven / Weichen geändert wurden
        /// </summary>
        public bool Fahren_Schalten { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21 von geänderten Daten auf dem Rückmelde Bus
        /// </summary>
        public bool RM_Bus { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21 wenn es Änderungen bei der Railcom-Auslesung der eigenen Lokomotiven gibt
        /// </summary>
        public bool Railcom { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21, wenn Bertiebsparameter sich ändern (Spannungen, Ströme, etc.)
        /// </summary>
        public bool System_Status { set; get; }
        /// <summary>
        /// Option: automatische Rückantwort der Z21, wenn Lokomotiven / Weichen geändert wurden
        /// </summary>
        public bool Alle_Lok_Info { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21, wenn neue Basis-Daten auf dem LocoNet empfangen wurden
        /// </summary>
        public bool LOCONET_Basic { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21, wenn neue Lok-Daten auf dem LocoNet empfangen wurden
        /// </summary>
        public bool LOCONET_Lok { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21, wenn neue Weichen-Daten auf dem LocoNet empfangen wurden
        /// </summary>
        public bool LOCONET_Weichen { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21, wenn neue Rückmelde-Daten auf dem LocoNet empfangen wurden
        /// </summary>
        public bool LOCONET_Detect { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21 wenn es Änderungen bei der Railcom-Auslesung gibt
        /// </summary>
        public bool Alle_Railcom { set; get; }
        /// <summary>
        /// Option: automatisches Senden der Z21 wenn neue Daten auf dem CAN-Bus empfangen wurden
        /// </summary>
        public bool CAN_Detect { set; get; }
        /// <summary>
        /// Umwandlung der Flags in ein sendbares Datenpaket
        /// </summary>
        /// <returns>4 Bytes - Datenpaket auf Z21-Protokoll Basis</returns>
        public byte[] GetAsBytes()
        {
            byte[] data = { 0x00, 0x00, 0x00, 0x00 };
            if (Fahren_Schalten) data[0] += 0x01;
            if (RM_Bus) data[0] += 0x02;
            if (Railcom) data[0] += 0x04;
            if (System_Status) data[1] += 0x01;
            if (Alle_Lok_Info) data[2] += 0x01;
            if (LOCONET_Basic) data[3] += 0x01;
            if (LOCONET_Lok) data[3] += 0x02;
            if (LOCONET_Weichen) data[3] += 0x04;
            if (LOCONET_Detect) data[3] += 0x08;
            if (Alle_Railcom) data[2] += 0x04;
            if (CAN_Detect) data[2] += 0x08;

            return data;
        }
    }
    public class LokFahrstufen
    {
        /// <summary>
        /// ID für 14 Fahrstufen
        /// </summary>
        public const int Fahstufe14 = 0;
        /// <summary>
        /// ID für 28 Fahrstufen
        /// </summary>
        public const int Fahstufe28 = 1;
        /// <summary>
        /// ID für 128 Fahrstufen
        /// </summary>
        public const int Fahstufe128 = 2;
        /// <summary>
        /// ID für Vorwärtsfahrt
        /// </summary>
        public const int Vorwaerts = 0x80;
        /// <summary>
        /// ID für Rückwärtsfahrt
        /// </summary>
        public const int Rueckwaerts = 0x00;
        /// <summary>
        /// Fahrstufen in Datenpaket umwandeln
        /// </summary>
        /// <param name="Geschw">Fahrstufe: Je nach Anzahl 0..126 </param>
        /// <param name="Richtung">Für Vorwärts benutze <see cref="Vorwaerts"/> | für Rückwärts benutze <see cref="Rueckwaerts"/></param>
        /// <param name="Fahrstufen"><see cref="Fahstufe14"/>, <see cref="Fahstufe28"/> oder <see cref="Fahstufe128"/></param>
        /// <returns></returns>
        public static byte LookUpFahrstufe(int Geschw, int Richtung, int Fahrstufen)
        {
            byte temp = 0;
            switch(Fahrstufen)
            {
                case Fahstufe14: 
                    if (Geschw != 0)
                    {
                        if (Geschw > 14) Geschw = 14;
                        temp = (byte)(Geschw + 1);
                    }
                    else
                    {
                        temp = 0;
                    }
                    break;
                case Fahstufe28:
                    if (Geschw != 0)
                    {
                        if (Geschw > 28) Geschw = 28;
                        temp = (byte)(Geschw + 3);

                        if ((temp & 0x01) == 0x01)
                        {
                            temp = (byte)(temp >> 1);
                            temp += 0x10;
                        }
                        else 
                        {
                            temp = (byte)(temp >> 1);
                        }
                    }
                    else
                    {
                        temp = 0x10;
                    }
                    break;
                case Fahstufe128:
                    if (Geschw != 0)
                    {
                        if (Geschw > 126) Geschw = 126;
                        temp = (byte)(Geschw + 1);
                    }
                    else
                    {
                        temp = 0;
                    }
                    break;
            }
            return (byte)(temp + Richtung);
        }
        public static byte Addr_Low(int Addresse)
        {
            return (byte)(Addresse & 0xFF);
        }
        public static byte Addr_High(int Addresse)
        {
            byte temp = (byte)((Addresse >> 8) & 0xFF);
            if (Addresse >= 128) temp |= 0xC0;
            return temp;
        }
    }
    
}
