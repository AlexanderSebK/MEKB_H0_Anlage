﻿using System;
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
        public const int Fahstufe28 = 2;
        /// <summary>
        /// ID für 128 Fahrstufen
        /// </summary>
        public const int Fahstufe128 = 4;
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
        /// <param name="Geschw">Fahrstufe: Je nach Anzahl 0..126 / -1 (oder Außerhalb) = Nothalt </param>
        /// <param name="Richtung">Für Vorwärts benutze <see cref="Vorwaerts"/> | für Rückwärts benutze <see cref="Rueckwaerts"/></param>
        /// <param name="Fahrstufen"><see cref="Fahstufe14"/>, <see cref="Fahstufe28"/> oder <see cref="Fahstufe128"/></param>
        /// <returns></returns>
        public static byte FahrstufeToProtokol(int Geschw, int Richtung, int Fahrstufen)
        {
            byte temp = 0;
            switch(Fahrstufen)
            {
                case Fahstufe14: 
                    switch (Geschw)
                    {
                        case 0: temp = 0x00; break;
                        case 1: temp = 0x02; break;
                        case 2: temp = 0x03; break;
                        case 3: temp = 0x04; break;
                        case 4: temp = 0x05; break;
                        case 5: temp = 0x06; break;
                        case 6: temp = 0x07; break;
                        case 7: temp = 0x08; break;
                        case 8: temp = 0x09; break;
                        case 9: temp = 0x0A; break;
                        case 10: temp = 0x0B; break;
                        case 11: temp = 0x0C; break;
                        case 12: temp = 0x0D; break;
                        case 13: temp = 0x0E; break;
                        case 14: temp = 0x0F; break;
                        default: temp = 0x01;break;
                    }
                    break;
                case Fahstufe28:
                    switch (Geschw)
                    {
                        case 0: temp =  0x00; break;
                        case 1: temp =  0x02; break;
                        case 2: temp =  0x12; break;
                        case 3: temp =  0x03; break;
                        case 4: temp =  0x13; break;
                        case 5: temp =  0x04; break;
                        case 6: temp =  0x14; break;
                        case 7: temp =  0x05; break;
                        case 8: temp =  0x15; break;
                        case 9: temp =  0x06; break;
                        case 10: temp = 0x16; break;
                        case 11: temp = 0x07; break;
                        case 12: temp = 0x17; break;
                        case 13: temp = 0x08; break;
                        case 14: temp = 0x18; break;
                        case 15: temp = 0x09; break;
                        case 16: temp = 0x19; break;
                        case 17: temp = 0x0A; break;
                        case 18: temp = 0x1A; break;
                        case 19: temp = 0x0B; break;
                        case 20: temp = 0x1B; break;
                        case 21: temp = 0x0C; break;
                        case 22: temp = 0x1C; break;
                        case 23: temp = 0x0D; break;
                        case 24: temp = 0x1D; break;
                        case 25: temp = 0x0E; break;
                        case 26: temp = 0x1E; break;
                        case 27: temp = 0x0F; break;
                        case 28: temp = 0x1F; break;
                        default: temp = 0x01; break;
                    }
                    break;
                case Fahstufe128:
                    if (Geschw != 0)
                    {
                        if (Geschw > 126) return (byte)(0x01 + Richtung);
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
        public static byte ProtokolToFahrstufe(int Geschw, int FahrstufenInfo)
        {
            Geschw = Geschw & 0x7F; //Entfernen des MSB bit (Richtung)
            switch (FahrstufenInfo)
            {
                case Fahstufe14:
                    switch (Geschw)
                    {
                        case 0x00: return 0;
                        case 0x02: return 1;
                        case 0x03: return 2;
                        case 0x04: return 3;
                        case 0x05: return 4;
                        case 0x06: return 5;
                        case 0x07: return 6;
                        case 0x08: return 7;
                        case 0x09: return 8;
                        case 0x0A: return 9;
                        case 0x0B: return 10;
                        case 0x0C: return 11;
                        case 0x0D: return 12;
                        case 0x0E: return 13;
                        case 0x0F: return 14;
                        default: return 0;
                    }
                case Fahstufe28:
                    switch (Geschw)
                    {
                        case 0x00: return 0;
                        case 0x02: return 1;
                        case 0x12: return 2;
                        case 0x03: return 3;
                        case 0x13: return 4;
                        case 0x04: return 5;
                        case 0x14: return 6;
                        case 0x05: return 7;
                        case 0x15: return 8;
                        case 0x06: return 9;
                        case 0x16: return 10;
                        case 0x07: return 11;
                        case 0x17: return 12;
                        case 0x08: return 13;
                        case 0x18: return 14;
                        case 0x09: return 15;
                        case 0x19: return 16;
                        case 0x0A: return 17;
                        case 0x1A: return 18;
                        case 0x0B: return 19;
                        case 0x1B: return 20;
                        case 0x0C: return 21;
                        case 0x1C: return 22;
                        case 0x0D: return 23;
                        case 0x1D: return 24;
                        case 0x0E: return 25;
                        case 0x1E: return 26;
                        case 0x0F: return 27;
                        case 0x1F: return 28;
                        default: return 0;
                    }
                case Fahstufe128:
                    switch (Geschw)
                    {
                        case 0x00: return 0;
                        case 0x01: return 0;
                        default: return (byte)(Geschw - 1);
                    }
                default: return 0;
            }             
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
