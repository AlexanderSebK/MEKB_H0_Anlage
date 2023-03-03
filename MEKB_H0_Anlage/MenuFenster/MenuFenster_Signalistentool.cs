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


            Routenzustand routenzustand = new Routenzustand();
            routenzustand.Weichenzustand.Add("Weiche1", true);
            routenzustand.Weichenzustand.Add("Weiche6", false);

            SignalListe.Liste[0].Routenzustandsliste.Add(routenzustand);
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

            if (SignalListe.Liste[index].Routenzustandsliste.Count > 0)
            {
                ToolRoutenIndex.Maximum = SignalListe.Liste[index].Routenzustandsliste.Count-1;
                int routenIndex = (int)ToolRoutenIndex.Value;
                Routenzustand routenzustand = SignalListe.Liste[index].Routenzustandsliste[routenIndex];

                
                foreach (KeyValuePair<string, bool> entry in routenzustand.Weichenzustand)
                {
                    ToolSignalWeichenListe.Rows.Add(entry.Key,entry.Value);
                }
            }
            else
            {
                ToolRoutenIndex.Minimum = 0;
                ToolRoutenIndex.Value = 0;
                ToolRoutenIndex.Maximum = 0;
            }
        }

        private void ToolSignalIndex_ValueChanged(object sender, EventArgs e)
        {
            ShowSignalData();
        }
    }
}
