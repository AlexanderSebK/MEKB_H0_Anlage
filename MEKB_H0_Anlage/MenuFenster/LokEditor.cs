using System;
using System.IO;
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
    public partial class LokEditor : Form
    {
        private string[] LokDatein;

        private string PfadAktuelleLok;
        Lokomotive AktuelleLok;

        public LokEditor()
        {
            InitializeComponent();
        }

        private void LokEditor_Load(object sender, EventArgs e)
        {
            LoadLokList("LokArchiv");
        }

        private void LoadLokList(string ArchivOrdner)
        {
            ArchivBaum.Nodes.Clear();

            ArchivBaum.Nodes.Add("root", "Lokarchiv");

            LokDatein = Directory.GetFiles(ArchivOrdner, "*.xml", SearchOption.AllDirectories);

            string path;
            foreach (string Datei in LokDatein)
            {
                path = Path.GetDirectoryName(Datei);
                if (path.Equals(ArchivOrdner))
                {
                    path = "";
                }
                else
                {
                    path = path.Substring(ArchivOrdner.Length + 1);
                }
                
                string[] Folders = path.Split('\\');
                string subpath = "";

                ArchivBaum.SelectedNode = ArchivBaum.Nodes[0];               

                foreach (string Folder in Folders)
                {
                    if (Folder.Equals("")) continue;
                    subpath += Folder;

                    if (ArchivBaum.SelectedNode.Nodes.ContainsKey(subpath) == false)
                    {
                        ArchivBaum.SelectedNode.Nodes.Add(subpath, Folder);
                    }
                    ArchivBaum.SelectedNode = ArchivBaum.Nodes.Find(subpath, true)[0];
                }
                ArchivBaum.SelectedNode.Nodes.Add(Datei, Path.GetFileName(Datei));                
            }
            ArchivBaum.TreeViewNodeSorter = new NodeSorter();
            ArchivBaum.Sort();
        }

        private void Laden_Click(object sender, EventArgs e)
        {
            if( ArchivBaum.SelectedNode == null ) return; // Nichts angewählt: Abbrechen
            if (ArchivBaum.SelectedNode.Nodes.Count != 0) return; // Wenn Untergruppen -> Abbrechen Ordner angewählt
            PfadAktuelleLok = ArchivBaum.SelectedNode.Name;
            PfadLabel.Text = "Geladen: " + PfadAktuelleLok;
            AktuelleLok = new Lokomotive(ArchivBaum.SelectedNode.Name);
            DisplayLokValues(AktuelleLok);
            Speichern.Enabled = true;   
        }

        private void DisplayLokValues(Lokomotive lok)
        {
            if (lok == null) return;
            if (lok.Adresse == 0) return;
            DisplayLokName.Text = lok.Name;
            try
            {
                DisplayLokAdresse.Value = lok.Adresse;
            }
            catch
            {
                DisplayLokAdresse.Value = 3;
            }
            DisplayLokGattung.Text = lok.Gattung;

            switch(lok.Epoche)
            {
                case 1:
                    DisplayLokEpoche.Text = "I";break;
                case 2:
                    DisplayLokEpoche.Text = "II"; break;
                case 3:
                    DisplayLokEpoche.Text = "III"; break;
                case 4:
                    DisplayLokEpoche.Text = "IV"; break;
                case 5:
                    DisplayLokEpoche.Text = "V"; break;
                case 6:
                    DisplayLokEpoche.Text = "VI"; break;
                default:
                    DisplayLokEpoche.Text = ""; break;
            }
            DisplayLokHersteller.Text = lok.Hersteller;
            DisplayLokVerwaltung.Text = lok.Verwaltung;
            DisplayLokTyp.Text = lok.Typ;

            DisplayLokFkt1.Text = lok.Funktionen[1];
            DisplayLokFkt2.Text = lok.Funktionen[2];
            DisplayLokFkt3.Text = lok.Funktionen[3];
            DisplayLokFkt4.Text = lok.Funktionen[4];
            DisplayLokFkt5.Text = lok.Funktionen[5];
            DisplayLokFkt6.Text = lok.Funktionen[6];
            DisplayLokFkt7.Text = lok.Funktionen[7];
            DisplayLokFkt8.Text = lok.Funktionen[8];
            DisplayLokFkt9.Text = lok.Funktionen[9];
            DisplayLokFkt10.Text = lok.Funktionen[10];
            DisplayLokFkt11.Text = lok.Funktionen[11];
            DisplayLokFkt12.Text = lok.Funktionen[12];
            DisplayLokFkt13.Text = lok.Funktionen[13];
            DisplayLokFkt14.Text = lok.Funktionen[14];
            DisplayLokFkt15.Text = lok.Funktionen[15];
            DisplayLokFkt16.Text = lok.Funktionen[16];
            DisplayLokFkt17.Text = lok.Funktionen[17];
            DisplayLokFkt18.Text = lok.Funktionen[18];
            DisplayLokFkt19.Text = lok.Funktionen[19];
            DisplayLokFkt20.Text = lok.Funktionen[20];
        }

        private void DisplayDatenLesen()
        {
            AktuelleLok = new Lokomotive();
            AktuelleLok.Adresse = (int)DisplayLokAdresse.Value;
            AktuelleLok.Name = DisplayLokName.Text;
            AktuelleLok.Gattung = DisplayLokGattung.Text;
            AktuelleLok.Hersteller = DisplayLokHersteller.Text;
            AktuelleLok.Typ = DisplayLokTyp.Text;
            if (DisplayLokEpoche.Text.Equals("I")) AktuelleLok.Epoche = 1;
            if (DisplayLokEpoche.Text.Equals("II")) AktuelleLok.Epoche = 2;
            if (DisplayLokEpoche.Text.Equals("III")) AktuelleLok.Epoche = 3;
            if (DisplayLokEpoche.Text.Equals("IV")) AktuelleLok.Epoche = 4;
            if (DisplayLokEpoche.Text.Equals("V")) AktuelleLok.Epoche = 5;
            if (DisplayLokEpoche.Text.Equals("VI")) AktuelleLok.Epoche = 6;
            AktuelleLok.Verwaltung = DisplayLokVerwaltung.Text;

            AktuelleLok.Funktionen[1] = DisplayLokFkt1.Text;
            AktuelleLok.Funktionen[2] = DisplayLokFkt2.Text;
            AktuelleLok.Funktionen[3] = DisplayLokFkt3.Text;
            AktuelleLok.Funktionen[4] = DisplayLokFkt4.Text;
            AktuelleLok.Funktionen[5] = DisplayLokFkt5.Text;
            AktuelleLok.Funktionen[6] = DisplayLokFkt6.Text;
            AktuelleLok.Funktionen[7] = DisplayLokFkt7.Text;
            AktuelleLok.Funktionen[8] = DisplayLokFkt8.Text;
            AktuelleLok.Funktionen[9] = DisplayLokFkt9.Text;
            AktuelleLok.Funktionen[10] = DisplayLokFkt10.Text;
            AktuelleLok.Funktionen[11] = DisplayLokFkt11.Text;
            AktuelleLok.Funktionen[12] = DisplayLokFkt12.Text;
            AktuelleLok.Funktionen[13] = DisplayLokFkt13.Text;
            AktuelleLok.Funktionen[14] = DisplayLokFkt14.Text;
            AktuelleLok.Funktionen[15] = DisplayLokFkt15.Text;
            AktuelleLok.Funktionen[16] = DisplayLokFkt16.Text;
            AktuelleLok.Funktionen[17] = DisplayLokFkt17.Text;
            AktuelleLok.Funktionen[18] = DisplayLokFkt18.Text;
            AktuelleLok.Funktionen[19] = DisplayLokFkt19.Text;
            AktuelleLok.Funktionen[20] = DisplayLokFkt20.Text;

        }

        private void SpeichernUnter_Click(object sender, EventArgs e)
        {
            DisplayDatenLesen();
            XElement ExportData = AktuelleLok.ExportLokData();                                                   //Lokdaten in XElement verwandeln;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "LokArchiv (*.xml)|*.xml",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportData.Save(saveFileDialog1.FileName);
            }
            PfadAktuelleLok = saveFileDialog1.FileName;
            PfadLabel.Text = "Geladen: " + PfadAktuelleLok;
            Speichern.Enabled = true;
        }



        

        public class NodeSorter : System.Collections.IComparer
        {
            public NodeSorter() { }

            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;

                if (tx.Nodes.Count != 0 && ty.Nodes.Count != 0) return tx.Text.CompareTo(ty.Text);
                if (tx.Nodes.Count == 0 && ty.Nodes.Count == 0) return tx.Text.CompareTo(ty.Text);
                if (tx.Nodes.Count == 0 && ty.Nodes.Count != 0) return 1;
                if (tx.Nodes.Count != 0 && ty.Nodes.Count == 0) return -1;
                return tx.Text.CompareTo(ty.Text);
            }
        }

        private void NeueLok_Click(object sender, EventArgs e)
        {
            PfadAktuelleLok = "";
            PfadLabel.Text = "Geladen: -";
            AktuelleLok = new Lokomotive() { Adresse = 3};
            DisplayLokValues(AktuelleLok);
            Speichern.Enabled = false;
        }

        private void Speichern_Click(object sender, EventArgs e)
        {
            if(PfadAktuelleLok.Equals(""))
            {
                Speichern.Enabled=false;
                return;
            }
            DisplayDatenLesen();
            XElement ExportData = AktuelleLok.ExportLokData();                                                   //Lokdaten in XElement verwandeln;
            ExportData.Save(PfadAktuelleLok);
        }
    }
}
