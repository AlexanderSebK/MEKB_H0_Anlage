

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
        /// <summary>
        /// CallBack Funktion: Seriennummer 
        /// Wird aufgerufen sobald die SerienNummer von der Z21 empfangen wurde
        /// </summary>
        /// <param name="data"></param>
        public void CallBack_GET_SERIAL_NUMBER(int sn)
        {
            this.BeginInvoke((Action<string>)DataReceivedUI, sn.ToString());
        }
    }
}
