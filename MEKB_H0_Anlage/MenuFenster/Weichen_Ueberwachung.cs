using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace MEKB_H0_Anlage
{
    public partial class Weichen_Ueberwachung : Form
    {
        #region Timer
        private static System.Timers.Timer HeartbeatTimer;
        #endregion

        public Weichen_Ueberwachung()
        {
            InitializeComponent();
        }
        public Weichen_Ueberwachung(List<Weiche> weichen)
        {
            InitializeComponent();
            WeichenFenster.DataSource = weichen;
        }

        private void Refresh_Heartbeat(Object source, ElapsedEventArgs e)
        {
            RefrechDataGrid(WeichenFenster);
        }

        private void RefrechDataGrid(System.Windows.Forms.DataGridView Grid)
        {
            Grid.Invoke((MethodInvoker)(() =>
            {
                Grid.Update();
            }));
        }

        private void Weichen_Ueberwachung_Shown(object sender, EventArgs e)
        {
            // 5 Sekunden Timer einrichten (Lebenspuls für die Verbindung)
            HeartbeatTimer = new System.Timers.Timer(2000);
            // Timer mit Funktion "Refresh_Heartbeat" Verbinden
            HeartbeatTimer.Elapsed += Refresh_Heartbeat;
            HeartbeatTimer.AutoReset = true;
            HeartbeatTimer.Enabled = true;
        }

        private void Weichen_Ueberwachung_FormClosing(object sender, FormClosingEventArgs e)
        {
            HeartbeatTimer.Enabled = false;
        }
    }
}
