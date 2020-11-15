using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
        private void SetupLokListe()
        {
            string[] fileEntries = Directory.GetFiles("LokArchiv");
            string KeineLokAdr = "Folgende Dateien besitzen keine LokAdressen und werden ignoriert:\n\n";
            bool FehlendeAdr = false;
            foreach (string fileName in fileEntries)
            {

                XElement XMLFile = XElement.Load(fileName);              //XML-Datei öffnen
                var list = XMLFile.Elements("Lok").ToList();             //Alle Elemente des Types Lok in eine Liste Umwandeln 

                foreach (XElement lok in list)                            //Alle Elemente der Liste einzeln durchlaufen
                {
                    Lok Lokomotive = new Lok();
                    if (lok.Element("Adresse") == null)
                    {
                        KeineLokAdr += String.Format("- {0} \n", fileName);
                        FehlendeAdr = true;
                        continue;
                    }
                    Lokomotive.Adresse = Int16.Parse(lok.Element("Adresse").Value);     //LokAdresse des Elements auslesen

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

                    Lokliste.Add(Lokomotive);                                       //Lokomotive zur Lokliste hinzufügen
                }
            }
            if (FehlendeAdr) MessageBox.Show(KeineLokAdr, "Keine Adressen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (!Lokliste.GroupBy(x => x.Adresse).All(g => g.Count() == 1))
            {
                List<int> DoppelAdressen = Lokliste.GroupBy(x => x.Adresse)
                                        .Where(g => g.Count() > 1)
                                        .Select(y => y.Key)
                                        .ToList();

                List<Lok> Ausschuss = new List<Lok>();

                foreach (int adr in DoppelAdressen)
                {
                    var Subliste = Lokliste.FindAll(Lok => Lok.Adresse == adr);
                    Ausschuss.AddRange(Subliste);
                }
                string nachricht = "Loks mit gleichen Adressen gefunden: \n\n";

                foreach (Lok lok in Ausschuss)
                {
                    nachricht += String.Format("{0} - {1}\n", lok.Adresse, lok.Name);
                }

                MessageBox.Show(nachricht, "Mehrdeutige Adresse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void LokCtrl_LoklisteAusfuellen()
        {
            foreach (Lok Lokomtive in Lokliste)
            {
                LokCtrl1_Name.Items.Add(Lokomtive.Name);
                LokCtrl2_Name.Items.Add(Lokomtive.Name);
                LokCtrl3_Name.Items.Add(Lokomtive.Name);
                LokCtrl4_Name.Items.Add(Lokomtive.Name);
                LokCtrl5_Name.Items.Add(Lokomtive.Name);
                LokCtrl6_Name.Items.Add(Lokomtive.Name);
                LokCtrl7_Name.Items.Add(Lokomtive.Name);
                LokCtrl8_Name.Items.Add(Lokomtive.Name);
                LokCtrl9_Name.Items.Add(Lokomtive.Name);
                LokCtrl10_Name.Items.Add(Lokomtive.Name);
            }
        }  
        private void Setze_Lok_Fahrt(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            z21Start.Z21_SET_LOCO_DRIVE(Adresse, Fahrstufe, Richtung, Fahstrufeninfo);
        }
        private void Setze_Lok_Funktion(int Adresse, byte Zustand, byte FunktionsNr)
        {
            z21Start.Z21_SET_LOCO_FUNCTION(Adresse, Zustand, FunktionsNr);
        }
    }
}
