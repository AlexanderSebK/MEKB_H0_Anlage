using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Signal_Einstellungen : Form
    {
        public Signal_Einstellungen()
        {
            InitializeComponent();
        }

        private void SignalEinstellungenSchliesen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Signal_Einstellungen_Shown(object sender, EventArgs e)
        {
            if(Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"))
            {
                AutoSigStellung.Checked = false;
                AutoSigStrasse.Checked = true;
            }
            else
            {
                AutoSigStellung.Checked = true;
                AutoSigStrasse.Checked = false;
            }
            if (Config.ReadConfig("AutoSignalFahrt").Equals("true"))
            {
                AutoFahrt.Checked = true;
            }
            else
            {
                AutoFahrt.Checked = false;
            }
        }

        private void AutoSigStellung_CheckedChanged(object sender, EventArgs e)
        {
            if(AutoSigStellung.Checked) Config.WriteConfig("AutoSignalFahrstrasse", "false");
        }

        private void AutoSigStrasse_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoSigStrasse.Checked) Config.WriteConfig("AutoSignalFahrstrasse", "true");
        }

        private void AutoFahrt_CheckedChanged(object sender, EventArgs e)
        {
            if(AutoFahrt.Checked) Config.WriteConfig("AutoSignalFahrt", "true");
            else Config.WriteConfig("AutoSignalFahrt", "false");
        }
    }
}
