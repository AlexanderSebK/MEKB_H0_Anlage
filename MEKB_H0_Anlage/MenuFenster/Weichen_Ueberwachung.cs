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
    }
}
