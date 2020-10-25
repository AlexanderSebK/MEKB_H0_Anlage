using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;



namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Klasse für die Kommunikation mit der Z21.
    /// </summary>
    public partial class Z21
    {
        //Referenz Zugriff Hauptform
        private readonly Form1 form;
        public Z21(Form1 form)
        {
            this.form = form;   //Referenz zuweisen
            Connected = false;
        }

        //UPD-Verbindung zur Z21 als Klassen-global
        private UdpClient Client = new UdpClient();
        public string Z21_IP { get; set; }      //IP-Adresse Z21
        public UInt16 Z21_Port { get; set; }    //Port Z21 : 21105 (Primär)  / Alt. 21106
        private bool Connected { get; set; }    //Status mit dem 

        /// <summary>
        /// Starten einer UDP-Verbindung
        /// </summary>
        public void Connect_Z21()
        {
            Client = new UdpClient();
            Z21_IP = Config.ReadConfig("Z21_IP");
            if (Z21_IP.Equals("Not Found")) return;
            if (Z21_IP.Equals("Error")) return;
            Z21_Port = UInt16.Parse(Config.ReadConfig("Z21_Port"));
            if (Z21_Port.Equals("Not Found")) return;
            if (Z21_Port.Equals("Error")) return;
            IPEndPoint Z21_Adr = new IPEndPoint(IPAddress.Parse(Z21_IP), Z21_Port);     //Adressdaten in IPEndPoint-Datentyp umwandeln
            Client.Connect(Z21_Adr);                                                    //UPD-Verbindung aufbauen
            Client.BeginReceive(DataReceived, null);                                    //Interupt/Callback-funktion wenn neue Daten von Z21 empfangen wurden
            byte[] SendBytes = { 0x04, 0x00, 0x10, 0x00 };
            Client.Send(SendBytes, 4);
        }
        /// <summary>
        /// Beenden der UDP-Verbindung inkl. Abmeldung von der Z21
        /// </summary>
        public void DisConnect_Z21()
        {
            Z21_LOGOFF();
            Client.Dispose();                                                    //UPD-Verbindung beenden
            Connected = false;
            form.SetConnect(false);
        }
        /// <summary>
        /// Rückmeldung des Status der aktuellen Verbindung. Wird als Verbindung gewertet sobald die ersten Nachrichten von der Z21 empfangen wurden
        /// </summary>
        /// <returns>true - Verbunden / false - Nicht verunden</returns>
        public bool Verbunden()
        {
            return Connected;
        }
        /// <summary>
        /// Interrupt-Funktion
        /// Wird aufgerufen sobald eine Nachricht über UDP empfangen wurde
        /// </summary>
        /// <param name="ar"></param>
        private void DataReceived(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Z21_IP), Z21_Port);
            byte[] data;
            try
            {
                data = Client.EndReceive(ar, ref ip);

                if (data.Length == 0)
                {
                    Connected = false;
                    form.SetConnect(false);
                    return; // No more to receive
                }
                if (Connected == false) form.SetConnect(true);
                Connected = true;
                Client.BeginReceive(DataReceived, null);
            }
            catch (ObjectDisposedException)
            {
                Connected = false;
                form.SetConnect(false);
                return; // Connection closed
            }

            int length;
            byte[] msgdata;
            int FehlerCode;

            while (data.Length > 0)
            {
                length = data[0] + data[1] * 256;
                msgdata = data.Take(length).ToArray();

                FehlerCode = SolveZ21Msg(msgdata);
                if (FehlerCode != 0) form.CallBack_Fehler(FehlerCode);

                data = data.Skip(length).ToArray();
            }  
        }
        /// <summary>
        /// Empfangene Nachricht auswerten, kontrollieren und die entsprechenden CallBack Funktionen ausrufen
        /// </summary>
        /// <param name="data">Byte-Array der empfangen Daten</param>
        /// <returns>ErrorCode</returns>
        private int SolveZ21Msg(byte[] data)
        {
            int length = data[0] + data[1] * 256;
            if(length != data.Length)return Z21_ErrorCode.FALSE_LENGTH;

            length -= 4;
            switch(data[2])
            {
                case Z21_Header.SERIAL_NUMBER:      
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    form.CallBack_GET_SERIAL_NUMBER(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
                    break;
                case Z21_Header.CODE_STATUS: break;
                case Z21_Header.Z21_VERSION: break;
                case Z21_Header.X_BUS_TUNNEL:
                    byte XOR_Check = 0x00;
                    for(int i=4;i<data.Count();i++)
                    {
                        XOR_Check = (byte)(XOR_Check ^ data[i]);
                    }
                    if(XOR_Check != 0x00) return Z21_ErrorCode.FALSE_CHECK;

                    byte[] para_db = data.Skip(5).ToArray();
                    para_db = para_db.Take(para_db.Count() - 1).ToArray();

                    form.CallBack_X_BUS_TUNNEL(data[4], para_db, para_db.Count());
                    break;
                case Z21_Header.BROADCAST_FLAGS:      //Broadcast-Flags
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    Flags new_flags = new Flags(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
                    form.CallBack_Z21_Broadcast_Flags(new_flags);
                    break;
                case Z21_Header.GET_LOK_MODE:      //Lok-Status
                    break;
                case Z21_Header.GET_FKT_DEC_MODE:      //Fx-Decoder Status
                    break;
                case Z21_Header.RM_BUS:      //Rückmelde-Bus
                    break;
                case Z21_Header.SYSTEM_STATE:      //System-Status
                    if (length != 16) return Z21_ErrorCode.FALSE_LENGTH;

                    int MainCurrent = BitConverter.ToInt16(data, 4);
                    int ProgCurrent = BitConverter.ToInt16(data, 6);
                    int MainCurrentFilter = BitConverter.ToInt16(data, 8);
                    int Temperatur = BitConverter.ToInt16(data, 10);
                    int VersorgungSpg = BitConverter.ToUInt16(data, 12);
                    int GleisSpg = BitConverter.ToUInt16(data, 14);
                    form.CallBack_Z21_System_Status(MainCurrent, ProgCurrent, MainCurrentFilter, Temperatur, VersorgungSpg, GleisSpg, data[16],data[17]);
                    break;
                case Z21_Header.RAILCOM:      //Railcom
                    break;
                case Z21_Header.LOCONET_RX:      //LocoNet Rx
                    break;
                case Z21_Header.LOCONET_TX:      //LocoNet Tx
                    break;
                case Z21_Header.LOCONET_LAN:      //LocoNet LAN
                    break;
                case Z21_Header.LOCONET_ADDR:      //LocoNet Adresse
                    break;
                case Z21_Header.LOCONET_DETECTOR:      //LocoNet Rückmelder
                    break;
                case Z21_Header.CAN_DETECTOR:      //CAN-Rückmelder
                    break;
                default: return Z21_ErrorCode.WRONG_HEADER;  
            }
            return 0;
        }

        
        //Befehle
        public void Z21_GET_SERIAL_NUMBER()                 //Daten senden: Seriennummer Abfragen
        {
            byte[] SendBytes = { 0x04, 0x00, 0x10, 0x00 };
            if(Connected) Client.Send(SendBytes, 4);
        }
        public void Z21_LOGOFF()                
        {
            byte[] SendBytes = { 0x04, 0x00, 0x30, 0x00 };
            if (Connected) Client.Send(SendBytes, 4);
        }

        public void Z21_GET_FIRMWARE_VERSION()
        {
            byte[] SendBytes = { 0x07, 0x00, 0x40, 0x00, 0xF1, 0x0A, 0xFB };
            if (Connected) Client.Send(SendBytes, 7);
        }

        public void Z21_GET_BROADCASTFLAGS()
        {
            byte[] SendBytes = { 0x04, 0x00, 0x51, 0x00 };
            if (Connected) Client.Send(SendBytes, 4);
        }
        public void Z21_SET_BROADCASTFLAGS(Flags flags)
        {
            byte[] tempdata = flags.GetAsBytes();
            byte[] SendBytes = { 0x08, 0x00, 0x50, 0x00, tempdata[0], tempdata[1], tempdata[2], tempdata[3] };
            if (Connected) Client.Send(SendBytes, 8);
        }

        public void Z21_SET_LOCO_DRIVE(int Adresse, int Geschwindigkeit, int Richtung, int Fahrstufe)
        {
            byte Header = 0xE4;
            byte DB0 = (byte)(0x10 + Fahrstufe);
            byte DB1 = LokFahrstufen.Addr_High(Adresse);
            byte DB2 = LokFahrstufen.Addr_Low(Adresse);
            byte DB3 = LokFahrstufen.LookUpFahrstufe(Geschwindigkeit, Richtung, Fahrstufe);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2 ^ DB3);
            byte[] SendBytes = { 0x0A, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, DB3, XOR };
            if (Connected) Client.Send(SendBytes, 10);
        }

        public void Z21_GET_SYSTEMSTATE()
        {
            byte[] SendBytes = { 0x04, 0x00, 0x85, 0x00 };
            if (Connected) Client.Send(SendBytes, 4);
        }

        public async Task Z21_SET_WEICHEAsync(int Adresse, bool Abzweig, bool Q_Modus, int time, bool deaktivieren)
        {
            Adresse--;
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2;
            if (Q_Modus)
            {
                DB2 = 0xA9;
                if (Abzweig) DB2 = 0xA8;
            }
            else
            {
                DB2 = 0x89;
                if (Abzweig) DB2 = 0x88;
            }
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            if (Connected) Client.Send(SendBytes, 9);
            if (deaktivieren)
            {
                await Task.Delay(time);
                if (Q_Modus)
                {
                    DB2 = 0xA1;
                    if (Abzweig) DB2 = 0xA0;
                }
                else
                {
                    DB2 = 0x81;
                    if (Abzweig) DB2 = 0x80;
                }


                XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
                byte[] SendBytes2 = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
                if (Connected) Client.Send(SendBytes2, 9);
            }
            /*
            Adresse--;
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = 0xA9;
            if (Abzweig) DB2 = 0xA8;
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            if (Connected) Client.Send(SendBytes, 9);
            await Task.Delay(500);
            DB2 = 0xA1;
            if (Abzweig) DB2 = 0xA0;
            XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes2 = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            if (Connected) Client.Send(SendBytes2, 9);
            */
        }

        public void Z21_SET_SIGNAL(int Adresse, bool Zustand)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht schalten, da Adresse 0
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = 0xA8;
            if (Zustand) DB2 = 0xA9;
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            if (Connected) Client.Send(SendBytes, 9);
        }
        public void Z21_SET_SIGNAL_OFF(int Adresse)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht schalten, da Adresse 0
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = 0xA0;
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            if (Connected) Client.Send(SendBytes, 9);
        }

        public void Z21_GET_WEICHE(int Adresse)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht anfragen, da Adresse 0
            byte Header = 0x43;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte XOR = (byte)(Header ^ DB0 ^ DB1);
            byte[] SendBytes = { 0x08, 0x00, 0x40, 0x00, Header, DB0, DB1, XOR };
            if (Connected) Client.Send(SendBytes, 8);
        }

        //Schreiben auf Window
    }
}
