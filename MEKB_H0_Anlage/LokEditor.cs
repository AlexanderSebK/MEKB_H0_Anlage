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
        private List<Lok> loks = new List<Lok>(); 
        public LokEditor()
        {
            InitializeComponent();
        }

        private void LokEditor_Load(object sender, EventArgs e)
        {
            LoadLokList();
        }

        private void LoadLokList()
        {
            LokDatein = Directory.GetFiles("LokArchiv");
            Lokliste.Items.Clear();
            foreach(string Datei in LokDatein)
            {
                Lokliste.Items.Add(Path.GetFileName(Datei));               
            }
        }

        private void LoadLok(XElement XMLFile)
        {
            loks.Clear();
            var list = XMLFile.Elements("Lok").ToList();             //Alle Elemente des Types Lok in eine Liste Umwandeln 

            foreach (XElement lok in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                Lok Lokomotive = new Lok();
                if (lok.Element("Adresse") == null)
                {
                    //Entry rot markieren
                }
                else
                {
                    Lokomotive.Adresse = Int16.Parse(lok.Element("Adresse").Value);     //LokAdresse des Elements auslesen
                }

                if (lok.Element("Name") == null) Lokomotive.Name = "";
                else Lokomotive.Name = lok.Element("Name").Value;                   //Lokname des Elements auslesen
                if (lok.Element("Gattung") == null) Lokomotive.Gattung = "";
                else Lokomotive.Gattung = lok.Element("Gattung").Value;             //StandardGattung Eintragen
                Lokomotive.Funktionen.Add("Licht");
                for (int i = 1; i <= 21; i++)
                {
                    string Label = String.Format("Funktion{0}", i);
                    if (lok.Element(Label) == null)
                    {
                        Lokomotive.Funktionen.Add(null);
                    }
                    else
                    {
                        Lokomotive.Funktionen.Add(lok.Element(Label).Value);
                    }
                }

                loks.Add(Lokomotive);
            }
        }
        private void SaveLok(string Filename)
        {

        }
    }
}
