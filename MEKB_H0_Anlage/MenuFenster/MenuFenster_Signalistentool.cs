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
            SignalListe = new SignalListe("Signalliste.xml");
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

            ToolSignalWeichenListe.Rows.Clear();
            ToolSignalDataGridBelegtmelder.Rows.Clear();
            

            if (SignalListe.Liste[index].Routenzustandsliste.Count > 0)
            {
                ToolRoutenIndex.Maximum = SignalListe.Liste[index].Routenzustandsliste.Count-1;
                int routenIndex = (int)ToolRoutenIndex.Value;
                Routenzustand routenzustand = SignalListe.Liste[index].Routenzustandsliste[routenIndex];
                ToolSignalNachestesSignal.Text = routenzustand.NaechstesSignal;
                ToolSignalSH0.Text = routenzustand.SH0_Sperre;
                
                foreach (KeyValuePair<string, bool> entry in routenzustand.Weichenzustand)
                {
                    ToolSignalWeichenListe.Rows.Add(entry.Key,entry.Value);
                }
                foreach (string Melder in routenzustand.Belegtmeldungen)
                {
                    ToolSignalDataGridBelegtmelder.Rows.Add(Melder);
                }
            }
            else
            {
                ToolRoutenIndex.Minimum = 0;
                ToolRoutenIndex.Value = 0;
                ToolRoutenIndex.Maximum = 0;
                ToolSignalNachestesSignal.Text = "";
                ToolSignalSH0.Text = "";
            }
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

        private void ToolSignalWeichenListe_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            int RouteIndex = (int)ToolRoutenIndex.Value;

            if (SignalListe == null) return;
            if (SignalListe.Liste[index].Routenzustandsliste.Count <= 0) SignalListe.Liste[index].Routenzustandsliste.Add(new Routenzustand());

            SignalListe.Liste[index].Routenzustandsliste[RouteIndex].Weichenzustand = new Dictionary<string, bool>();

            foreach (DataGridViewRow row in ToolSignalWeichenListe.Rows)
            {
                if (row.Cells["DataGridWeichenName"].Value == null) continue;
                string Weichenname = row.Cells["DataGridWeichenName"].Value.ToString();
                DataGridViewCheckBoxCell Abzweig = row.Cells["DataGridAbzweig"] as DataGridViewCheckBoxCell;

                SignalListe.Liste[index].Routenzustandsliste[RouteIndex].Weichenzustand.Add(Weichenname, Convert.ToBoolean(Abzweig.Value));
            }
        }

        private void NRoute_Click(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            SignalListe.Liste[index].Routenzustandsliste.Add(new Routenzustand());
            ToolRoutenIndex.Maximum++;
        }

        private void ToolRoutenIndex_ValueChanged(object sender, EventArgs e)
        {
            ShowSignalData();
        }

        private void ToolSignalNachestesSignal_TextChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            int RouteIndex = (int)ToolRoutenIndex.Value;

            if (SignalListe == null) return;

            if (SignalListe.Liste[index].Routenzustandsliste.Count <= 0) SignalListe.Liste[index].Routenzustandsliste.Add(new Routenzustand());

            SignalListe.Liste[index].Routenzustandsliste[RouteIndex].NaechstesSignal = ToolSignalNachestesSignal.Text;
        }

        private void ToolSignalSH0_TextChanged(object sender, EventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            int RouteIndex = (int)ToolRoutenIndex.Value;

            if (SignalListe == null) return;

            if(SignalListe.Liste[index].Routenzustandsliste.Count <= 0) SignalListe.Liste[index].Routenzustandsliste.Add(new Routenzustand());

            SignalListe.Liste[index].Routenzustandsliste[RouteIndex].SH0_Sperre = ToolSignalSH0.Text;
        }

        private void ToolSignalDataGridBelegtmelder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int index = (int)ToolSignalIndex.Value;
            int RouteIndex = (int)ToolRoutenIndex.Value;

            if (SignalListe == null) return;

            if (SignalListe.Liste[index].Routenzustandsliste.Count <= 0) SignalListe.Liste[index].Routenzustandsliste.Add(new Routenzustand());

            SignalListe.Liste[index].Routenzustandsliste[RouteIndex].Belegtmeldungen.Clear();

            foreach (DataGridViewRow row in ToolSignalDataGridBelegtmelder.Rows)
            {
                if (row.Cells["DataGridBelegtmelder"].Value == null) continue;
                string Belegtmelder = row.Cells["DataGridBelegtmelder"].Value.ToString();

                SignalListe.Liste[index].Routenzustandsliste[RouteIndex].Belegtmeldungen.Add(Belegtmelder);
            }
        }
    }
}
