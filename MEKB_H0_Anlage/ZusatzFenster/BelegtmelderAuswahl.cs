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
        public string vorherigerBlock;
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
                vorherigerBlock = comboBox1.Text;
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

        private void Tabelle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            comboBox1.Items.Clear();
            Auswahl = Tabelle.CurrentRow.DataBoundItem as Belegtmelder;
            foreach (NachbarBlock nachbarBlock in Auswahl.NachbarBlocks)
            {
                comboBox1.Items.Add(nachbarBlock.KommeVon);
            }
            comboBox1.SelectedIndex = 0;
        }
    }
}
