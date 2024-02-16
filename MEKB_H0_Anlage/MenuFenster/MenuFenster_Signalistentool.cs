using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public partial class MenuFenster_Signalistentool : Form
    {
        public SignalListe SignalListe;
        public MenuFenster_Signalistentool()
        {
            InitializeComponent();
        }

        public MenuFenster_Signalistentool(string XML_Datei)
        {
            InitializeComponent();
            SignalListe = new SignalListe(XML_Datei);
            ToolSignalIndex.Maximum = SignalListe.Liste.Count - 1;
            ShowSignalData();
        }

        public void ShowSignalData()
        {
            int index = (int)ToolSignalIndex.Value;

            ToolSignalName.Text = SignalListe.Liste[index].Name;
            ToolAdr1.Value = SignalListe.Liste[index].Adresse;
            ToolAdr2.Value = SignalListe.Liste[index].Adresse2;
            if(SignalListe.Liste[index].Adresse2 == 0)
            {
                ToolAdr2S0.Enabled = false;
                ToolAdr2S1.Enabled = false;
            }
            else
            {
                ToolAdr2S0.Enabled = true;
                ToolAdr2S1.Enabled = true;
            }

            ToolAdr1S0.Text = SignalListe.Liste[index].Adr1_1.ToString();
            ToolAdr1S1.Text = SignalListe.Liste[index].Adr1_2.ToString();
            ToolAdr2S0.Text = SignalListe.Liste[index].Adr2_1.ToString();
            ToolAdr2S1.Text = SignalListe.Liste[index].Adr2_2.ToString();
        }

        private void ToolSignalIndex_ValueChanged(object sender, EventArgs e)
        {
            ShowSignalData();
        }

        private void ExportNewXML_Click(object sender, EventArgs e)
        {
            XElement ExportData = new XElement("Signalliste",
                from SignalEntry in SignalListe.Liste
                select
                    SignalEntry.SignalZuXML()
                    );

            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "SignalData (*.xml)|*.xml",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportData.Save(saveFileDialog1.FileName);
            }
        }

        private void ToolAdr1S0_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            Enum.TryParse<SignalZustand>(ToolAdr1S0.Text, out SignalZustand Zustand);
            SignalListe.Liste[index].Adr1_1 = Zustand;
        }

        private void ToolAdr1S1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            Enum.TryParse<SignalZustand>(ToolAdr1S1.Text, out SignalZustand Zustand);
            SignalListe.Liste[index].Adr1_2 = Zustand;
        }

        private void ToolAdr2S0_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            Enum.TryParse<SignalZustand>(ToolAdr2S0.Text, out SignalZustand Zustand);
            SignalListe.Liste[index].Adr2_1 = Zustand;
        }

        private void ToolAdr2S1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            Enum.TryParse<SignalZustand>(ToolAdr2S1.Text, out SignalZustand Zustand);
            SignalListe.Liste[index].Adr2_2 = Zustand;
        }

    }
}
