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
            TURNOUT_INFO = 0x43,
            PROGRAMMING = 0x61,
            STATUS_CHANGE = 0x62,
            X_VERSION = 0x63,
            CV_RESULT = 0x64,
            BC_STOPPED = 0x81,
            LOCO_INFO = 0xEF,
            GET_FIRMWARE = 0xF3,
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

        private Logger _log;

        private bool QMode { get; set; }    // Beim Schalten Queue Modus ein aus

       


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
        public delegate void LAN_X_GET_VERSION(double XBusVersion, byte ID);
        public delegate void LAN_X_CV_RESULT(int CVAdresse, byte Wert);
        public delegate void LAN_X_BC_STOPPED();
        public delegate void LAN_X_LOCO_INFO(int ParamterCount, 
                                             int Addresse, 
                                             bool Besetzt, 
                                             byte FahrstufenInfo, 
                                             bool Richtung, 
                                             byte Fahrstufe, 
                                             bool Doppeltraktio,
                                             bool Smartsearch, 
                                             bool[] Funktionen);
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
        public delegate void LAN_CAN_DETECTOR(int CANID, int ModulAdresse, byte Port, byte Typ, int Value1,int Value2);

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


        public void SetLog(Logger logger)
        {
            _log = logger;
        }

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
        /// Queue-Modus aktivieren. 
        /// Wenn aktiv Z21 wartet mit dem Befehl bis Schienentransfer es erlaubt und sendet es 4mal hintereinander. 
        /// Wenn aus Befehl muss direkt gesendet werden. Es kann aber nur eine Weiche/Signal auf einmal angesteuert werden
        /// </summary>
        /// <param name="on_off"></param>
        public void SetQMode(bool on_off)
        {
            QMode = on_off;
        }
        /// <summary>
        /// Interrupt-Funktion
        /// Wird aufgerufen sobald eine Nachricht über UDP empfangen wurde
        /// </summary>
        /// <param name="ar">Asynchron Results</param>
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
            catch //(ObjectDisposedException)
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
                    _log.ReceivedData("SERIAL_NUMBER", data);
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    call_LAN_GET_SERIAL_NUMBER?.Invoke(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
                    break;
                case Z21_Header.CODE_STATUS:
                    _log.ReceivedData("CODE_STATUS", data);
                    if (length != 1) return Z21_ErrorCode.FALSE_LENGTH;
                    call_LAN_GET_CODE?.Invoke(data[4]);
                    break;
                case Z21_Header.Z21_VERSION:
                    _log.ReceivedData("Z21_VERSION", data);
                    if (length != 8) return Z21_ErrorCode.FALSE_LENGTH;
                    int major = (data[9] & 0x0F) + ((data[9] >> 4) * 10);       //Umwandeln DBC-Format
                    int major100 = (data[10] & 0x0F) + ((data[10] >> 4) * 10);       //Umwandeln DBC-Format
                    int major10000 = (data[11] & 0x0F) + ((data[11] >> 4) * 10);       //Umwandeln DBC-Format
                    int minor = (data[8] & 0x0F) + ((data[8] >> 4) * 10);       //Umwandeln DBC-Format
                    double FW_Version = (minor * 0.01) + (major) + (major100 * 100) + (major10000 * 1000);
                    int HWTYpe = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);
                    call_LAN_GET_HWINFO?.Invoke(HWTYpe, FW_Version);
                    break;
                case Z21_Header.X_BUS_TUNNEL:
                    byte XOR_Check = 0x00;
                    for(int i=4;i<data.Count();i++)
                    {
                        XOR_Check = (byte)(XOR_Check ^ data[i]);
                    }
                    if(XOR_Check != 0x00) return Z21_ErrorCode.FALSE_CHECK;

                    byte[] para_db = data.Skip(5).ToArray();
                    para_db = para_db.Take(para_db.Count() - 1).ToArray();
                    if (data.Length <= 4) return Z21_ErrorCode.FALSE_LENGTH;
                    SolveXTunnel((Z21_XBus_Header)data[4], para_db);

                    break;
                case Z21_Header.BROADCAST_FLAGS:      //Broadcast-Flags
                    _log.ReceivedData("BROADCAST_FLAGS", data);
                    if (length != 4) return Z21_ErrorCode.FALSE_LENGTH;
                    int flags = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);
                    call_LAN_GET_BROADCASTFLAGS?.Invoke(flags);
                    break;
                case Z21_Header.GET_LOK_MODE:      //Lok-Status
                    _log.ReceivedData("LOK_MODE", data);
                    if (length != 3) return Z21_ErrorCode.FALSE_LENGTH;
                    int Adresse = data[4] + (data[5] << 8) + 1;
                    call_LAN_GET_LOCOMODE?.Invoke(Adresse, data[6]);
                    break;
                case Z21_Header.GET_FKT_DEC_MODE:      //Fx-Decoder Status
                    _log.ReceivedData("FKT_DEC_MODE", data);
                    if (length != 3) return Z21_ErrorCode.FALSE_LENGTH;
                    int FxAdresse = data[4] + (data[5] << 8) + 1;
                    call_LAN_GET_TURNOUTMODE?.Invoke(FxAdresse, data[6]);
                    break;
                case Z21_Header.RM_BUS:      //Rückmelde-Bus
                    _log.ReceivedData("RM_BUS", data);
                    //data[6] = 0xFF; //Debug Testing, da kein Belegtmelder vorhanden
                    if (length != 11) return Z21_ErrorCode.FALSE_LENGTH;
                    byte GruppenIndex = data[4];
                    byte[] RMStatus = data.Skip(5).ToArray();
                    call_LAN_RMBUS_DATACHANGED?.Invoke(GruppenIndex, RMStatus);
                    break;
                case Z21_Header.SYSTEM_STATE:      //System-Status
                    _log.ReceivedData("SYSTEM_STATE", data);
                    if (length != 16) return Z21_ErrorCode.FALSE_LENGTH;
                    int MainCurrent = BitConverter.ToInt16(data, 4);
                    int ProgCurrent = BitConverter.ToInt16(data, 6);
                    int MainCurrentFilter = BitConverter.ToInt16(data, 8);
                    int Temperatur = BitConverter.ToInt16(data, 10);
                    int VersorgungSpg = BitConverter.ToUInt16(data, 12);
                    int GleisSpg = BitConverter.ToUInt16(data, 14);
                    call_LAN_SYSTEMSTATE_DATACHANGED?.Invoke(MainCurrent, ProgCurrent, MainCurrentFilter, Temperatur, VersorgungSpg, GleisSpg, data[16], data[17]);
                    break;
                case Z21_Header.RAILCOM:      //Railcom
                    _log.ReceivedData("RAILCOM", data);
                    if (length != 13) return Z21_ErrorCode.FALSE_LENGTH;
                    int LocoAdress = BitConverter.ToUInt16(data, 4);
                    int ReceiveCounter = (int)BitConverter.ToUInt32(data, 6);
                    int ErrorCounter = BitConverter.ToUInt16(data, 10);
                    byte Options = data[13];
                    byte Speed = data[14];
                    byte QoS = data[15];
                    call_LAN_RAILCOM_DATACHANGED?.Invoke(LocoAdress, ReceiveCounter, ErrorCounter, Options, Speed, QoS);
                    break;
                case Z21_Header.LOCONET_RX:      //LocoNet Rx
                    _log.ReceivedData("LOCONET_RX", data);
                    byte[] LocoNet_RX = data.Skip(5).ToArray();
                    call_LAN_LOCONET_Z21_RX?.Invoke(LocoNet_RX);
                    break;
                case Z21_Header.LOCONET_TX:      //LocoNet Tx
                    _log.ReceivedData("LOCONET_TX", data);
                    byte[] LocoNet_TX = data.Skip(5).ToArray();
                    call_LAN_LOCONET_Z21_TX?.Invoke(LocoNet_TX);
                    break;
                case Z21_Header.LOCONET_LAN:      //LocoNet LAN
                    _log.ReceivedData("LOCONET_LAN", data);
                    byte[] LocoNet_LAN = data.Skip(5).ToArray();
                    call_LAN_LOCONET_FROM_LAN?.Invoke(LocoNet_LAN);
                    break;
                case Z21_Header.LOCONET_ADDR:      //LocoNet Adresse
                    _log.ReceivedData("LOCONET_ADDR", data);
                    if (length != 3) return Z21_ErrorCode.FALSE_LENGTH;
                    int LOCONet_Adresse = data[4] + (data[5] << 8) + 1;
                    call_LAN_LOCONET_DISPATCH_ADDR?.Invoke(LOCONet_Adresse, data[6]);
                    break;
                case Z21_Header.LOCONET_DETECTOR:      //LocoNet Rückmelder
                    _log.ReceivedData("LOCONET_DETECTOR", data);
                    if (length != 3) return Z21_ErrorCode.FALSE_LENGTH;
                    int Reporter_Adresse = data[5] + (data[6] << 8) + 1;
                    call_LAN_LOCONET_DETECTOR?.Invoke(data[4], Reporter_Adresse);
                    break;
                case Z21_Header.CAN_DETECTOR:      //CAN-Rückmelder
                    _log.ReceivedData("CAN_DETECTOR", data);
                    if (length != 10) return Z21_ErrorCode.FALSE_LENGTH;
                    int CANID = BitConverter.ToUInt16(data, 4);
                    int Addr = BitConverter.ToUInt16(data, 6);
                    byte Port = data[8];
                    byte Typ = data[9];
                    int Value1 = BitConverter.ToUInt16(data, 10);
                    int Value2 = BitConverter.ToUInt16(data, 12);
                    call_LAN_CAN_DETECTOR?.Invoke(CANID,Addr,Port,Typ,Value1,Value2);
                    break;
                default:
                    _log.ReceivedData("Wrong Data", data);
                    return Z21_ErrorCode.WRONG_HEADER;  
            }
            return 0;
        }

        private void SolveXTunnel(Z21_XBus_Header header, byte[] db)
        {
            switch(header)
            {
                case Z21_XBus_Header.TURNOUT_INFO:
                    _log.ReceivedData("TURNOUT_INFO", db);
                    if (db.Length == 3)
                    {
                        int addr = (db[0] << 8) + db[1] + 1;
                        call_LAN_X_TURNOUT_INFO?.Invoke(addr, db[2]);
                    }                      
                    break;
                case Z21_XBus_Header.PROGRAMMING:
                    _log.ReceivedData("PROGRAMMING", db);
                    if (db.Length == 1)
                    {
                        if(db[0] == 0x00) call_LAN_X_BC_TRACK_POWER_OFF?.Invoke();
                        else if (db[0] == 0x01) call_LAN_X_BC_TRACK_POWER_ON?.Invoke();
                        else if (db[0] == 0x02) call_LAN_X_PROGRAMMING_MODE?.Invoke();
                        else if (db[0] == 0x08) call_LAN_X_BC_TRACK_SHORT_CIRCUIT?.Invoke();
                        else if (db[0] == 0x12) call_LAN_X_CV_NACK_SC?.Invoke();
                        else if (db[0] == 0x13) call_LAN_X_CV_NACK?.Invoke();
                        else if (db[0] == 0x82) call_LAN_X_UNKNOWN_COMMAND?.Invoke();
                    }
                    break;
                case Z21_XBus_Header.GET_FIRMWARE:
                    _log.ReceivedData("GET_FIRMWARE", db);
                    if (db.Length == 3)
                    {
                        int major = (db[1] & 0x0F) + ((db[1] >> 4) * 10);       //Umwandeln DBC-Format
                        int minor = (db[2] & 0x0F) + ((db[2] >> 4) * 10);       //Umwandeln DBC-Format
                        double FW = major + (minor * 0.01);
                        if (db[0] == 0x0A) call_LAN_X_GET_FIRMWARE_VERSION?.Invoke(FW);
                    }
                    break;
                case Z21_XBus_Header.STATUS_CHANGE:
                    _log.ReceivedData("STATUS_CHANGE", db);
                    if (db.Length == 2)
                    {
                        if (db[0] == 0x22)call_LAN_X_STATUS_CHANGED?.Invoke(db[1]);
                    }
                    break;
                case Z21_XBus_Header.X_VERSION:
                    _log.ReceivedData("X_VERSION", db);
                    if (db.Length == 3)
                    {
                        int major = (db[1] & 0x0F) + ((db[1] >> 4) * 10);       //Umwandeln DBC-Format
                        int minor = (db[2] & 0x0F) + ((db[2] >> 4) * 10);       //Umwandeln DBC-Format
                        double Version = major + (minor * 0.01);
                        if (db[0] == 0x21) call_LAN_X_GET_VERSION?.Invoke(Version, db[2]);
                    }
                    break;
                case Z21_XBus_Header.CV_RESULT:
                    _log.ReceivedData("CV_RESULT", db);
                    if (db.Length == 4)
                    {
                        int addr = (db[0] << 8) + db[1] + 1;
                        if (db[0] == 0x14) call_LAN_X_CV_RESULT?.Invoke(addr, db[3]);
                    }
                    break;
                case Z21_XBus_Header.BC_STOPPED:
                    _log.ReceivedData("BC_STOPPED", db);
                    if (db.Length == 1)
                    {
                        if (db[0] == 0x00) call_LAN_X_BC_STOPPED?.Invoke();
                    }
                    break;
                case Z21_XBus_Header.LOCO_INFO:
                    _log.ReceivedData("LOCO_INFO", db);
                    int ParameterCount = 0;
                    int Adr = 0;
                    bool besetzt = false;
                    byte FahrstufeInfo = 0;
                    bool Richtung = false;
                    byte Fahrstufe = 0;
                    bool Doppeltraktion = false;
                    bool SmartSearch = false;
                    List<bool> Funktionen = new List<bool>();


                    if (db.Length >=2)
                    {
                        ParameterCount = 1;
                        Adr = (((db[0] & 0x3F) << 8) + db[1]);
                    }
                    if(db.Length >=3)
                    {
                        ParameterCount = 3;
                        if ((db[2] & 0x08) == 0x08) besetzt = true;
                        else besetzt = false;
                        FahrstufeInfo = (byte)(db[2] & 0x07);
                    }
                    if (db.Length >= 4)
                    {
                        ParameterCount = 5;
                        if ((db[3] & 0x80) == 0x80) Richtung = true;
                        else Richtung = false;
                        Fahrstufe = (byte)(db[3] & 0x7F);
                    }
                    if (db.Length >= 5)
                    {
                        ParameterCount = 8;
                        if ((db[4] & 0x40) == 0x40) Doppeltraktion = true;
                        else Doppeltraktion = false;
                        if ((db[4] & 0x20) == 0x20) SmartSearch = true;
                        else SmartSearch = false;
                        if ((db[4] & 0x10) == 0x10) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[4] & 0x01) == 0x01) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[4] & 0x02) == 0x02) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[4] & 0x04) == 0x04) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[4] & 0x08) == 0x08) Funktionen.Add(true);
                        else Funktionen.Add(false);
                    }
                    if (db.Length >= 6)
                    {
                        if ((db[5] & 0x01) == 0x01) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x02) == 0x02) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x04) == 0x04) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x08) == 0x08) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x10) == 0x10) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x20) == 0x20) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x40) == 0x40) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[5] & 0x80) == 0x80) Funktionen.Add(true);
                        else Funktionen.Add(false);
                    }
                    if (db.Length >= 7)
                    {
                        if ((db[6] & 0x01) == 0x01) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x02) == 0x02) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x04) == 0x04) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x08) == 0x08) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x10) == 0x10) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x20) == 0x20) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x40) == 0x40) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[6] & 0x80) == 0x80) Funktionen.Add(true);
                        else Funktionen.Add(false);
                    }
                    if (db.Length >= 8)
                    {
                        if ((db[7] & 0x01) == 0x01) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x02) == 0x02) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x04) == 0x04) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x08) == 0x08) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x10) == 0x10) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x20) == 0x20) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x40) == 0x40) Funktionen.Add(true);
                        else Funktionen.Add(false);
                        if ((db[7] & 0x80) == 0x80) Funktionen.Add(true);
                        else Funktionen.Add(false);
                    }

                    call_LAN_X_LOCO_INFO?.Invoke(ParameterCount, Adr, besetzt, FahrstufeInfo, Richtung, Fahrstufe, Doppeltraktion, SmartSearch, Funktionen.ToArray());
                    break;
            }
        }

        private void SendCommand(byte[] Data, int size)
        {
            if (Connected)
            {
                Client.Send(Data, size);
            }
        }

        
        //Befehle
        public void GET_SERIAL_NUMBER()                 //Daten senden: Seriennummer Abfragen
        {
            byte[] SendBytes = { 0x04, 0x00, 0x10, 0x00 };
            _log.SendData("GET_SERIAL_NUMBER", SendBytes);
            SendCommand(SendBytes, 4);
            
        }
        public void LOGOFF()
        {
            byte[] SendBytes = { 0x04, 0x00, 0x30, 0x00 };
            _log.SendData("LOGOFF", SendBytes);
            SendCommand(SendBytes, 4);
        }
        public void GET_FIRMWARE_VERSION()
        {
            byte[] SendBytes = { 0x07, 0x00, 0x40, 0x00, 0xF1, 0x0A, 0xFB };
            _log.SendData("GET_FIRMWARE_VERSION", SendBytes);
            SendCommand(SendBytes, 7);           
        }
        public void GET_BROADCASTFLAGS()
        {
            byte[] SendBytes = { 0x04, 0x00, 0x51, 0x00 };
            _log.SendData("GET_BROADCASTFLAGS", SendBytes);
            SendCommand(SendBytes, 4);
        }
        public void Z21_SET_BROADCASTFLAGS(Flags flags)
        {
            byte[] tempdata = flags.GetAsBytes();
            byte[] SendBytes = { 0x08, 0x00, 0x50, 0x00, tempdata[0], tempdata[1], tempdata[2], tempdata[3] };
            _log.SendData("SET_BROADCASTFLAGS", SendBytes);
            SendCommand(SendBytes, 8);
        }
        public void Z21_GET_STATUS()
        {
            byte[] SendBytes = { 0x07, 0x00, 0x40, 0x00, 0x21, 0x24, 0x05 };
            _log.SendData("GET_STATUS", SendBytes);
            SendCommand(SendBytes, 7);
        }
        public void Z21_GET_LOCO_INFO(int Adresse)
        {
            byte Header = 0xE3;
            byte DB0 = 0xF0;
            byte DB1 = LokFahrstufen.Addr_High(Adresse);
            byte DB2 = LokFahrstufen.Addr_Low(Adresse);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            _log.SendData("GET_LOCO_INFO", SendBytes);
            SendCommand(SendBytes, 9);
        }
        public void Z21_SET_LOCO_DRIVE(int Adresse, int Geschwindigkeit, int Richtung, int Fahrstufe)
        {
            int SendeFahrStufe = Fahrstufe;
            if (SendeFahrStufe == 4) SendeFahrStufe = 3;  //Umwandeln der Fahrstufe in Sendeformat
            byte Header = 0xE4;
            byte DB0 = (byte)(0x10 + SendeFahrStufe);
            byte DB1 = LokFahrstufen.Addr_High(Adresse);
            byte DB2 = LokFahrstufen.Addr_Low(Adresse);
            byte DB3 = LokFahrstufen.FahrstufeToProtokol(Geschwindigkeit, Richtung, Fahrstufe);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2 ^ DB3);
            byte[] SendBytes = { 0x0A, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, DB3, XOR };
            _log.SendData("SET_LOCO_DRIVE", SendBytes);
            SendCommand(SendBytes, 10);
        }
        public void Z21_SET_LOCO_FUNCTION(int Adresse, byte Zustand, byte Funktion)
        {
            Zustand = (byte)(Zustand & 0x3);
            Funktion = (byte)(Funktion & 0x3F);
            byte Header = 0xE4;
            byte DB0 = 0xF8;
            byte DB1 = LokFahrstufen.Addr_High(Adresse);
            byte DB2 = LokFahrstufen.Addr_Low(Adresse);
            byte DB3 = (byte)((Zustand << 6)+Funktion);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2 ^ DB3);
            byte[] SendBytes = { 0x0A, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, DB3, XOR };
            _log.SendData("SET_LOCO_FUNCTION", SendBytes);
            SendCommand(SendBytes, 10);
        }
        public void Z21_GET_SYSTEMSTATE()
        {
            byte[] SendBytes = { 0x04, 0x00, 0x85, 0x00 };
            _log.SendData("GET_SYSTEMSTATE", SendBytes);
            SendCommand(SendBytes, 4);
        }

        private byte GetDB2(bool aktivieren, bool Ausgang)
        {
            //DB2 BYte               10Q0A00P 
            byte DB2             = 0b10000000;
            if (QMode) DB2      |= 0b00100000; // Queue-Modus aktivieren: Befehl wird in ein FiFo eingereiht und dann 4 mal an Gleis gesendet
            if (aktivieren) DB2 |= 0b00001000; // Ausgang aktivieren
            if (Ausgang) DB2    |= 0b00000001; // Schaltausgang wählen
            return DB2;
        }
        public void LAN_X_SET_TURNOUT(int Adresse, bool Abzweig, bool Q_Modus, bool aktivieren)
        {
            Adresse--;
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = GetDB2(aktivieren, !Abzweig);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            _log.SendData("SET_TURNOUT", SendBytes);
            SendCommand(SendBytes, 9);
        }
        public void LAN_X_SET_SIGNAL(int Adresse, bool Zustand)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht schalten, da Adresse 0
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = GetDB2(true, Zustand);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            _log.SendData("SET_SIGNAL", SendBytes);
            SendCommand(SendBytes, 9);
        }
        
        public void LAN_X_SET_SIGNAL_OFF(int Adresse, bool Zustand)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht schalten, da Adresse 0
            byte Header = 0x53;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte DB2 = GetDB2(false, Zustand);
            byte XOR = (byte)(Header ^ DB0 ^ DB1 ^ DB2);
            byte[] SendBytes = { 0x09, 0x00, 0x40, 0x00, Header, DB0, DB1, DB2, XOR };
            _log.SendData("SET_SIGNAL_OFF", SendBytes);
            SendCommand(SendBytes, 9);
        }
        public void LAN_X_GET_TURNOUT_INFO(int Adresse)
        {
            Adresse--;
            if (Adresse < 0) return;//Nicht anfragen, da Adresse 0
            byte Header = 0x43;
            byte DB0 = (byte)(Adresse >> 8);
            byte DB1 = (byte)(Adresse & 0xFF);
            byte XOR = (byte)(Header ^ DB0 ^ DB1);
            byte[] SendBytes = { 0x08, 0x00, 0x40, 0x00, Header, DB0, DB1, XOR };
            _log.SendData("GET_TURNOUT_INFO", SendBytes);
            SendCommand(SendBytes, 8);
        }

        public void LAN_X_GET_TURNOUT_INFO(List<int> Adressen)
        {
            List<byte> SendBytes = new List<byte>();
            foreach (int Adresse in Adressen)
            {
                if (Adresse < 1) continue;//Nicht anfragen, da Adresse 0
                List<byte> Packet = new List<byte>
                {
                    0x08,
                    0x00,
                    0x40,
                    0x00,
                    0x43, //Header
                    (byte)((Adresse - 1) >> 8),
                    (byte)((Adresse - 1) & 0xFF)
                };
                Packet.Add((byte)(Packet[4] ^ Packet[5] ^ Packet[6])); // XOR Checksumme
                _log.SendData("GET_TURNOUT_INFO", Packet.ToArray());

                SendBytes.AddRange(Packet);
            }
            SendCommand(SendBytes.ToArray(), SendBytes.Count);
        }

        public void LAN_RMBUS_GETDATA(byte GroupIndex)
        {
            byte[] SendBytes = { 0x05, 0x00, 0x81, 0x00, GroupIndex };
            _log.SendData("RMBUS_GETDATA", SendBytes);
            SendCommand(SendBytes, 5);
        }

        //Schreiben auf Window
    }
}
