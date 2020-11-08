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
        enum Z21_Header : byte
        {
           SERIAL_NUMBER = 0x10,
           CODE_STATUS = 0x18,
           Z21_VERSION = 0x1A,
           X_BUS_TUNNEL = 0x40,
           BROADCAST_FLAGS = 0x51,
           GET_LOK_MODE = 0x60,
           GET_FKT_DEC_MODE = 0x70,
           RM_BUS = 0x80,
           SYSTEM_STATE = 0x84,
           RAILCOM = 0x88,
           LOCONET_RX = 0xA0,
           LOCONET_TX = 0xA1,
           LOCONET_LAN = 0xA2,
           LOCONET_ADDR = 0xA3,
           LOCONET_DETECTOR = 0xA4,
           CAN_DETECTOR = 0xC4,
        }

        enum Z21_XBus_Header : byte
        {
            GET_FIRMWARE = 0xF3,
            Weichen_INFO = 0x43,
        }


        //Referenz Zugriff Hauptform
        //private readonly Form1 form;
        public Z21()
        {
            Connected = false;
        }

        //UPD-Verbindung zur Z21 als Klassen-global
        private UdpClient Client = new UdpClient();
        public string Z21_IP { get; set; }      //IP-Adresse Z21
        public UInt16 Z21_Port { get; set; }    //Port Z21 : 21105 (Primär)  / Alt. 21106
        private bool Connected { get; set; }    //Status ob die Z21 Verbunden ist

        //Delegates
        public delegate void LAN_ERROR(int ErrorCode);
        public delegate void LAN_CONNECT_STATUS(bool Status, bool Init);
        
        public delegate void LAN_GET_SERIAL_NUMBER(int Serial);
        public delegate void LAN_GET_CODE(byte Code);
        public delegate void LAN_GET_HWINFO(int HWTYpe, double FW_Version);        
        public delegate void LAN_X_TURNOUT_INFO(int Adresse, byte Zustand);
        public delegate void LAN_X_BC_TRACK_POWER_OFF();
        public delegate void LAN_X_BC_TRACK_POWER_ON();
        public delegate void LAN_X_PROGRAMMING_MODE();
        public delegate void LAN_X_BC_TRACK_SHORT_CIRCUIT();
        public delegate void LAN_X_CV_NACK_SC();
        public delegate void LAN_X_CV_NACK();
        public delegate void LAN_X_UNKNOWN_COMMAND();
        public delegate void LAN_X_STATUS_CHANGED(byte Status);
        public delegate void LAN_X_GET_VERSION(byte XBusVersion, byte ID);
        public delegate void LAN_X_CV_RESULT(int CVAdresse, byte Wert);
        public delegate void LAN_X_BC_STOPPED();
        public delegate void LAN_X_LOCO_INFO(bool Besetzt, byte FahrstufenInfo, bool Richtung, byte Fahrstufe, bool Doppeltraktio, bool Smartsearch, bool[] Funktionen);
        public delegate void LAN_X_GET_FIRMWARE_VERSION(double FW_Version);
        public delegate void LAN_GET_BROADCASTFLAGS(int BroadcastFlags);
        public delegate void LAN_GET_LOCOMODE(int Adresse, byte Modus);
        public delegate void LAN_GET_TURNOUTMODE(int Adresse, byte Modus);
        public delegate void LAN_RMBUS_DATACHANGED(byte GruppenIndex, byte[] RMStatus);
        public delegate void LAN_SYSTEMSTATE_DATACHANGED(int  MainCurrent,
                                                         int ProgCurrent,
                                                         int FilteredMainCurrent,
                                                         int Temperature,
                                                         int SupplyVoltage,
                                                         int VCCVoltage,
                                                         byte CentralState,
                                                         byte CentralStateEx);
        public delegate void LAN_RAILCOM_DATACHANGED(int LocoAddress,
                                                     int ReceiveCounter,
                                                     int ErrorCounter,
                                                     byte Options,
                                                     byte Speed,
                                                     byte QoS);
        public delegate void LAN_LOCONET_Z21_RX(byte[] LocoNet);
        public delegate void LAN_LOCONET_Z21_TX(byte[] LocoNet);
        public delegate void LAN_LOCONET_FROM_LAN(byte[] LocoNet);
        public delegate void LAN_LOCONET_DISPATCH_ADDR(int Addresse, byte Ergebnis);
        public delegate void LAN_LOCONET_DETECTOR(byte Typ, int Reportadresse);
        public delegate void LAN_CAN_DETECTOR(byte Typ, int CANID);

        private LAN_ERROR call_LAN_ERROR;
        private LAN_CONNECT_STATUS call_LAN_CONNECT_STATUS;
        
        private LAN_GET_SERIAL_NUMBER call_LAN_GET_SERIAL_NUMBER;
        private LAN_GET_CODE call_LAN_GET_CODE;
        private LAN_GET_HWINFO call_LAN_GET_HWINFO;
        private LAN_X_TURNOUT_INFO call_LAN_X_TURNOUT_INFO;
        private LAN_X_BC_TRACK_POWER_OFF call_LAN_X_BC_TRACK_POWER_OFF;
        private LAN_X_BC_TRACK_POWER_ON call_LAN_X_BC_TRACK_POWER_ON;
        private LAN_X_PROGRAMMING_MODE call_LAN_X_PROGRAMMING_MODE;
        private LAN_X_BC_TRACK_SHORT_CIRCUIT call_LAN_X_BC_TRACK_SHORT_CIRCUIT;
        private LAN_X_CV_NACK_SC call_LAN_X_CV_NACK_SC;
        private LAN_X_CV_NACK call_LAN_X_CV_NACK;
        private LAN_X_UNKNOWN_COMMAND call_LAN_X_UNKNOWN_COMMAND;
        private LAN_X_STATUS_CHANGED call_LAN_X_STATUS_CHANGED;
        private LAN_X_GET_VERSION call_LAN_X_GET_VERSION;
        private LAN_X_CV_RESULT call_LAN_X_CV_RESULT;
        private LAN_X_BC_STOPPED call_LAN_X_BC_STOPPED;
        private LAN_X_LOCO_INFO call_LAN_X_LOCO_INFO;
        private LAN_X_GET_FIRMWARE_VERSION call_LAN_X_GET_FIRMWARE_VERSION;
        private LAN_GET_BROADCASTFLAGS call_LAN_GET_BROADCASTFLAGS;
        private LAN_GET_LOCOMODE call_LAN_GET_LOCOMODE;
        private LAN_GET_TURNOUTMODE call_LAN_GET_TURNOUTMODE;
        private LAN_RMBUS_DATACHANGED call_LAN_RMBUS_DATACHANGED;
        private LAN_SYSTEMSTATE_DATACHANGED call_LAN_SYSTEMSTATE_DATACHANGED;
        private LAN_RAILCOM_DATACHANGED call_LAN_RAILCOM_DATACHANGED;
        private LAN_LOCONET_Z21_RX call_LAN_LOCONET_Z21_RX;
        private LAN_LOCONET_Z21_TX call_LAN_LOCONET_Z21_TX;
        private LAN_LOCONET_FROM_LAN call_LAN_LOCONET_FROM_LAN;
        private LAN_LOCONET_DISPATCH_ADDR call_LAN_LOCONET_DISPATCH_ADDR;
        private LAN_LOCONET_DETECTOR call_LAN_LOCONET_DETECTOR;
        private LAN_CAN_DETECTOR call_LAN_CAN_DETECTOR;

        public void Register_LAN_ERROR(LAN_ERROR function){call_LAN_ERROR = function;}
        public void Register_LAN_CONNECT_STATUS(LAN_CONNECT_STATUS function){ call_LAN_CONNECT_STATUS = function; }
        public void Register_LAN_GET_SERIAL_NUMBER(LAN_GET_SERIAL_NUMBER function){call_LAN_GET_SERIAL_NUMBER = function; }
        public void Register_LAN_GET_CODE(LAN_GET_CODE function){call_LAN_GET_CODE = function; }
        public void Register_LAN_GET_HWINFO(LAN_GET_HWINFO function){call_LAN_GET_HWINFO = function;}
        public void Register_LAN_X_TURNOUT_INFO(LAN_X_TURNOUT_INFO function){call_LAN_X_TURNOUT_INFO = function;}
        public void Register_LAN_X_BC_TRACK_POWER_OFF(LAN_X_BC_TRACK_POWER_OFF function){call_LAN_X_BC_TRACK_POWER_OFF = function;}
        public void Register_LAN_X_BC_TRACK_POWER_ON(LAN_X_BC_TRACK_POWER_ON function) { call_LAN_X_BC_TRACK_POWER_ON = function; }
        public void Register_LAN_X_PROGRAMMING_MODE(LAN_X_PROGRAMMING_MODE function) { call_LAN_X_PROGRAMMING_MODE = function; }
        public void Register_LAN_X_BC_TRACK_SHORT_CIRCUIT(LAN_X_BC_TRACK_SHORT_CIRCUIT function) { call_LAN_X_BC_TRACK_SHORT_CIRCUIT = function; }
        public void Register_LAN_X_CV_NACK_SC(LAN_X_CV_NACK_SC function) { call_LAN_X_CV_NACK_SC = function; }
        public void Register_LAN_X_CV_NACK(LAN_X_CV_NACK function) { call_LAN_X_CV_NACK = function; }
        public void Register_LAN_X_UNKNOWN_COMMAND(LAN_X_UNKNOWN_COMMAND function) { call_LAN_X_UNKNOWN_COMMAND = function; }
        public void Register_LAN_X_STATUS_CHANGED(LAN_X_STATUS_CHANGED function) { call_LAN_X_STATUS_CHANGED = function; }
        public void Register_LAN_X_GET_VERSION(LAN_X_GET_VERSION function) { call_LAN_X_GET_VERSION = function; }
        public void Register_LAN_X_CV_RESULT(LAN_X_CV_RESULT function) { call_LAN_X_CV_RESULT = function; }
        public void Register_LAN_X_BC_STOPPED(LAN_X_BC_STOPPED function) { call_LAN_X_BC_STOPPED = function; }
        public void Register_LAN_X_LOCO_INFO(LAN_X_LOCO_INFO function) { call_LAN_X_LOCO_INFO = function; }
        public void Register_LAN_X_GET_FIRMWARE_VERSION(LAN_X_GET_FIRMWARE_VERSION function) { call_LAN_X_GET_FIRMWARE_VERSION = function; }
        public void Register_LAN_GET_BROADCASTFLAGS(LAN_GET_BROADCASTFLAGS function) { call_LAN_GET_BROADCASTFLAGS = function; }
        public void Register_LAN_GET_LOCOMODE(LAN_GET_LOCOMODE function) { call_LAN_GET_LOCOMODE = function; }
        public void Register_LAN_GET_TURNOUTMODE(LAN_GET_TURNOUTMODE function) { call_LAN_GET_TURNOUTMODE = function; }
        public void Register_LAN_RMBUS_DATACHANGED(LAN_RMBUS_DATACHANGED function) { call_LAN_RMBUS_DATACHANGED = function; }
        public void Register_LAN_SYSTEMSTATE_DATACHANGED(LAN_SYSTEMSTATE_DATACHANGED function) { call_LAN_SYSTEMSTATE_DATACHANGED = function; }
        public void Register_LAN_RAILCOM_DATACHANGED(LAN_RAILCOM_DATACHANGED function) { call_LAN_RAILCOM_DATACHANGED = function; }
        public void Register_LAN_LOCONET_Z21_RX(LAN_LOCONET_Z21_RX function) { call_LAN_LOCONET_Z21_RX = function; }
        public void Register_LAN_LOCONET_Z21_TX(LAN_LOCONET_Z21_TX function) { call_LAN_LOCONET_Z21_TX = function; }
        public void Register_LAN_LOCONET_FROM_LAN(LAN_LOCONET_FROM_LAN function) { call_LAN_LOCONET_FROM_LAN = function; }
        public void Register_LAN_LOCONET_DISPATCH_ADDR(LAN_LOCONET_DISPATCH_ADDR function) { call_LAN_LOCONET_DISPATCH_ADDR = function; }
        public void Register_LAN_LOCONET_DETECTOR(LAN_LOCONET_DETECTOR function) { call_LAN_LOCONET_DETECTOR = function; }
        public void Register_LAN_CAN_DETECTOR(LAN_CAN_DETECTOR function) { call_LAN_CAN_DETECTOR = function; }


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
            LOGOFF();
            Client.Dispose();                                                    //UPD-Verbindung beenden
            Connected = false;
            call_LAN_CONNECT_STATUS?.Invoke(false,false);
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
                    call_LAN_CONNECT_STATUS?.Invoke(false,false);
                    return; // No more to receive
                }
                if (Connected == false) call_LAN_CONNECT_STATUS?.Invoke(true,false);
                Connected = true;
                Client.BeginReceive(DataReceived, null);
            }
            catch (ObjectDisposedException)
            {
                Connected = false;
                call_LAN_CONNECT_STATUS?.Invoke(false, false);
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
                if (FehlerCode != 0) call_LAN_ERROR?.Invoke(FehlerCode);//   form.CallBack_Fehler(FehlerCode);

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

            switch((Z21_Header)data[2])
            {
                case Z21_Header.SERIAL_NUMBER:      
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    //Call LAN_GET_SERIAL_NUMBER if available
                    call_LAN_GET_SERIAL_NUMBER?.Invoke(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
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

                    SolveXTunnel((Z21_XBus_Header)data[4], para_db);

                    break;
                case Z21_Header.BROADCAST_FLAGS:      //Broadcast-Flags
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    int flags = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);
                    call_LAN_GET_BROADCASTFLAGS?.Invoke(flags);

                    //Flags new_flags = new Flags(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));                   
                    //form.CallBack_Z21_Broadcast_Flags(new_flags);

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
                    call_LAN_SYSTEMSTATE_DATACHANGED?.Invoke(MainCurrent, ProgCurrent, MainCurrentFilter, Temperatur, VersorgungSpg, GleisSpg, data[16], data[17]);
                    //form.CallBack_Z21_System_Status(MainCurrent, ProgCurrent, MainCurrentFilter, Temperatur, VersorgungSpg, GleisSpg, data[16],data[17]);
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

        private void SolveXTunnel(Z21_XBus_Header header, byte[] db)
        {
            switch(header)
            {
                case Z21_XBus_Header.Weichen_INFO:
                    if (db.Length == 3)
                    {
                        int addr = (db[0] << 8) + db[1] + 1;
                        if (db.Length == 3) call_LAN_X_TURNOUT_INFO?.Invoke(addr, db[2]);
                    }                      
                    break;

            }
        }

        
        //Befehle
        public void GET_SERIAL_NUMBER()                 //Daten senden: Seriennummer Abfragen
        {
            byte[] SendBytes = { 0x04, 0x00, 0x10, 0x00 };
            if(Connected) Client.Send(SendBytes, 4);
        }
        public void LOGOFF()                
        {
            byte[] SendBytes = { 0x04, 0x00, 0x30, 0x00 };
            if (Connected) Client.Send(SendBytes, 4);
        }

        public void GET_FIRMWARE_VERSION()
        {
            byte[] SendBytes = { 0x07, 0x00, 0x40, 0x00, 0xF1, 0x0A, 0xFB };
            if (Connected) Client.Send(SendBytes, 7);
        }

        public void GET_BROADCASTFLAGS()
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
