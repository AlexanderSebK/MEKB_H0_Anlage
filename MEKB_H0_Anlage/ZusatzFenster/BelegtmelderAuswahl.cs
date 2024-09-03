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
    public partial class BelegtmelderAuswahl : Form
    {
        public Belegtmelder Auswahl;
        public BelegtmelderAuswahl(List<Belegtmelder> list)
        {
            InitializeComponent();
            Tabelle.DataSource = list;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (Tabelle.SelectedCells.Count > 0)
            {
                Auswahl = Tabelle.CurrentRow.DataBoundItem as Belegtmelder;  
            }
            else
            {
                Auswahl = null;
                DialogResult = DialogResult.Cancel;
            }
        }

        private void Abbruch_Click(object sender, EventArgs e)
        {

        }
    }
}
