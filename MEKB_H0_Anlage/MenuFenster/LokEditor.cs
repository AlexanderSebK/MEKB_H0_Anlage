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
            loks.Clear();
            LokDatein = Directory.GetFiles("LokArchiv", "*.xml", SearchOption.AllDirectories);
            Lokliste.Items.Clear();
            foreach (string Datei in LokDatein)
            {

                LoadLokValues(XElement.Load(Datei));
            }
            foreach (Lok lokomotive in loks)
            {
                Lokliste.Items.Add(lokomotive.Name);
            }
        }

        private void LoadLokValues(XElement XMLFile)
        {

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

        private void Laden_Click(object sender, EventArgs e)
        {
            string FileToOpen = "LokArchiv\\" + Lokliste.SelectedItem.ToString();

            int ListID = loks.IndexOf(new Lok() { Name = Lokliste.SelectedItem.ToString() }); //Lok mit diesem Namen in der Liste suchen
            if (ListID == -1) return;                                               //Lok nicht vorhanden, Funktion abbrechen
            DisplayLokValues(loks[ListID]);                                                   //Lokdaten anzeigen lassen
        }

        private void DisplayLokValues(Lok lok)
        {
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

        private void SpeichernUnter_Click(object sender, EventArgs e)
        {
            int ListID = loks.IndexOf(new Lok() { Name = Lokliste.SelectedItem.ToString() }); //Lok mit diesem Namen in der Liste suchen
            if (ListID == -1) return;                                               //Lok nicht vorhanden, Funktion abbrechen
            XElement ExportData = ExportLokData();                                                   //Lokdaten in XElement verwandeln;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "LokArchiv (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportData.Save(saveFileDialog1.FileName);
            }


            
        }

        private XElement ExportLokData()
        {
            XElement LokXMLData = new XElement("Lokliste",
                new XElement("Lok", 
                    new XElement("Name", DisplayLokName.Text),
                    new XElement("Adresse", DisplayLokAdresse.Value.ToString()),
                    new XElement("Gattung", DisplayLokGattung.Text),
                    new XElement("Funktion1", DisplayLokFkt1.Text),
                    new XElement("Funktion2", DisplayLokFkt2.Text),
                    new XElement("Funktion3", DisplayLokFkt3.Text),
                    new XElement("Funktion4", DisplayLokFkt4.Text),
                    new XElement("Funktion5", DisplayLokFkt5.Text),
                    new XElement("Funktion6", DisplayLokFkt6.Text),
                    new XElement("Funktion7", DisplayLokFkt7.Text),
                    new XElement("Funktion8", DisplayLokFkt8.Text),
                    new XElement("Funktion9", DisplayLokFkt9.Text),
                    new XElement("Funktion10", DisplayLokFkt10.Text),
                    new XElement("Funktion11", DisplayLokFkt11.Text),
                    new XElement("Funktion12", DisplayLokFkt12.Text),
                    new XElement("Funktion13", DisplayLokFkt13.Text),
                    new XElement("Funktion14", DisplayLokFkt14.Text),
                    new XElement("Funktion15", DisplayLokFkt15.Text),
                    new XElement("Funktion16", DisplayLokFkt16.Text),
                    new XElement("Funktion17", DisplayLokFkt17.Text),
                    new XElement("Funktion18", DisplayLokFkt18.Text),
                    new XElement("Funktion19", DisplayLokFkt19.Text),
                    new XElement("Funktion20", DisplayLokFkt20.Text)
                )
            );

            return LokXMLData;
        }
    }
}
