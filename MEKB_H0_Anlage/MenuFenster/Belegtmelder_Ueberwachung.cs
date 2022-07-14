using System;
using System.Timers;
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
    public partial class Belegtmelder_Ueberwachung : Form
    {
        private static System.Timers.Timer UpdateTimer;

        public BelegtmelderListe belegtmelderListe;

        public Belegtmelder_Ueberwachung(BelegtmelderListe liste)
        {
            InitializeComponent();

            belegtmelderListe = liste;
            // 100 MilliSekunden Timer: CooldownTimer
            UpdateTimer = new System.Timers.Timer(100);
            // Timer mit Funktion "BelegtLEDUpdate" Verbinden
            UpdateTimer.Elapsed += BelegtLEDUpdate;
            UpdateTimer.AutoReset = true;
            UpdateTimer.Enabled = true;

            foreach (Belegtmelder belegtmelder in belegtmelderListe.Liste)
            {
                Control Modul = this.Controls["Modul_" + belegtmelder.Name];
                if (Modul is TextBox ModulTextBox)
                {
                    ModulTextBox.Text = belegtmelder.Modulnummer.ToString();
                }
                Control Port = this.Controls["Port_" + belegtmelder.Name];
                if (Port is TextBox PortTextBox)
                {
                    PortTextBox.Text = belegtmelder.Portnummer.ToString();
                }
            }

        }

        private void BelegtLEDUpdate(Object source, ElapsedEventArgs e)
        {
            this.BeginInvoke((Action<int>)UpdateLightMatrix,1);          
        }

        private void UpdateLightMatrix(int i)
        {
            foreach(Belegtmelder belegtmelder in belegtmelderListe.Liste)
            {
                Control ctn = this.Controls[belegtmelder.Name];
                if(ctn is CheckBox checkBox)
                {
                    checkBox.Checked = belegtmelder.IstBelegt();
                }
            }
        }

        private void Melder_CheckedChanged(object sender, EventArgs e)
        {
            if(sender is CheckBox box)
            {
                if (box.Checked == false) box.BackColor = Color.DarkRed;
                else box.BackColor = Color.Red;
            }
        }

        private void PinChange(object sender, EventArgs e)
        {
            if (sender is CheckBox box)
            {
                string Meldeabschnitt = box.Name;
                Meldeabschnitt = Meldeabschnitt.Remove(0, 4); //Remove Pin_*

                Belegtmelder belegtmelder = belegtmelderListe.GetBelegtmelder(Meldeabschnitt);
                if(belegtmelder != null)
                {
                    belegtmelder.MeldeBesetzt(box.Checked);
                }
                
            }
        }


        private void Belegtmelder_Ueberwachung_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateTimer.Stop();
            UpdateTimer.Close();
        }
    }
}
