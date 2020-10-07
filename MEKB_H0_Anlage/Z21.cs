using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;



namespace MEKB_H0_Anlage
{
    public enum Z21_Error : int
    {
        SUCCESS = 0,
        FALSE_LENGTH = 1,
        WRONG_HEADER = 2,
        FALSE_CHECK = 3,
    }
    class Z21
    {
        //Referenz Zugriff Hauptform
        private readonly Form1 form;
        public Z21(Form1 form)
        {
            this.form = form;   //Referenz zuweisen
        }

        //UPD-Verbindung zur Z21 als Klassen-global
        UdpClient Client = new UdpClient();

        //Mit Z21 verbinden
        public void Connect_Z21()
        {
            IPEndPoint Z21_Adr = new IPEndPoint(IPAddress.Parse(Z21_IP), Z21_Port);     //Adressdaten in IPEndPoint-Datentyp umwandeln
            Client.Connect(Z21_Adr);                                                    //UPD-Verbindung aufbauen
            Client.BeginReceive(DataReceived, null);                                    //Interupt/Callback-funktion wenn neue Daten von Z21 empfangen wurden
        }
        public string Z21_IP { get; set; }      //IP-Adresse Z21
        public UInt16 Z21_Port { get; set; }    //Port Z21 : 21105 (Primär)  / Alt. 21106

        private void DataReceived(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Z21_IP), Z21_Port);
            byte[] data;
            try
            {
                data = Client.EndReceive(ar, ref ip);

                if (data.Length == 0)
                    return; // No more to receive
                Client.BeginReceive(DataReceived, null);
            }
            catch (ObjectDisposedException)
            {
                return; // Connection closed
            }

            SolveZ21Msg(data);
        }
        private Z21_Error SolveZ21Msg(byte[] data)
        {
            int length = data[0] + data[1] * 256;
            length = length - 4;
            switch(data[2])
            {
                case 0x10:      //Serien Nummer
                    if (length != 4) return Z21_Error.FALSE_LENGTH;
                    form.CallBack_GET_SERIAL_NUMBER(data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
                    break;
                case 0x40:      //Versionen
                    if (length == 3)
                    {
                        if(data[6] != (data[4] ^ data[5])) return Z21_Error.FALSE_CHECK;
                        byte[] para_db0 = { data[4], data[5] };
                        form.GetCallback(0x40, para_db0);
                    }
                    else if (length == 4)
                    {
                        if (data[7] != (data[4] ^ data[5] ^data[6])) return Z21_Error.FALSE_CHECK;
                        byte[] para_db1 = { data[4], data[5], data[6] };
                        form.GetCallback(0x40, para_db1);
                    }
                    else if (length == 5)
                    {
                        if (data[8] != (data[4] ^ data[5] ^ data[6] ^ data[7] )) return Z21_Error.FALSE_CHECK;
                        byte[] para_db2 = { data[4], data[5], data[6], data[7] };
                        form.GetCallback(0x40, para_db2);
                    }
                    else if (length == 6)
                    {
                        if (data[9] != (data[4] ^ data[5] ^ data[6] ^ data[7] ^ data[8])) return Z21_Error.FALSE_CHECK;
                        byte[] para_db3 = { data[4], data[5], data[6], data[7], data[8] };
                        form.GetCallback(0x40, para_db3);
                    }
                    else { return Z21_Error.WRONG_HEADER; }
                    break;
                case 0x51:      //Broadcast-Flags
                    if (length != 16) return Z21_Error.FALSE_LENGTH;
                    form.GetCallback(0x51, data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24));
                    break;
                case 0x60:      //Lok-Status
                    break;
                case 0x70:      //Fx-Decoder Status
                    break;
                case 0x80:      //Rückmelde-Bus
                    break;
                case 0x84:      //System-Status
                    if (length != 16) return Z21_Error.FALSE_LENGTH;
                    byte[] para_16 = { data[4],  data[5],  data[6],  data[7],
                                       data[8],  data[9],  data[10], data[11],
                                       data[12], data[13], data[14], data[15],
                                       data[16], data[17], data[18], data[19]};
                    form.GetCallback(0x84, para_16);
                    break;
                case 0x88:      //Railcom
                    break;
                case 0xA0:      //LocoNet Rx
                    break;
                case 0xA1:      //LocoNet Tx
                    break;
                case 0xA2:      //LocoNet LAN
                    break;
                case 0xA3:      //LocoNet Adresse
                    break;
                case 0xA4:      //LocoNet Rückmelder
                    break;
                case 0xC4:      //CAN-Rückmelder
                    break;
                default: return Z21_Error.WRONG_HEADER;  
            }
            return 0;
        }

        
        //Befehle
        public void Z21_GET_SERIAL_NUMBER()                 //Daten senden: Seriennummer Abfragen
        {
            byte[] SendBytes = { 0x04, 0x00, 0x10, 0x00 };
            Client.Send(SendBytes, 4);
        }

        public void Z21_SET_LOCO_DRIVE(int Adresse, int Geschwindigkeit, int Richtung, int Fahrstufe)
        {

        }

        //Schreiben auf Window
    }
}
