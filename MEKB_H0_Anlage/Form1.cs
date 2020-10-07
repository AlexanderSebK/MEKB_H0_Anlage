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
    public partial class Form1 : Form
    {
        Z21 z21Start;
        public Form1()
        {
            InitializeComponent();
            z21Start = new Z21(this);
        }
        public void GetCallback(int type, dynamic data)
        {
            switch(type)
            {
                case 0x10:      //Serien Nummer
                    this.BeginInvoke((Action<string>)DataReceivedUI, data.ToString());
                    break;
                case 0x40:      //Versionen
                    break;
                case 0x51:      //Broadcast-Flags
                    break;
                case 0x60:      //Lok-Status
                    break;
                case 0x70:      //Fx-Decoder Status
                    break;
                case 0x80:      //Rückmelde-Bus
                    break;
                case 0x84:      //System-Status
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
                default:break;
            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            z21Start.Z21_IP = "192.168.0.111";
            z21Start.Z21_Port = 21105;
            z21Start.Connect_Z21();
        }
        public void DataReceivedUI(string data)
        {
            Antwort.Text = data;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            z21Start.Z21_GET_SERIAL_NUMBER();
           
        }
    }
}
