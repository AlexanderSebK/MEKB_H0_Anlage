using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{

    public class ZuganzeigerListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Zuganzeige> Liste;

        private Z21 Z21 { get; set; }

        public ZuganzeigerListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Zuganzeige>();
        }

        public ZuganzeigerListe(string Dateiname)
        {
            DateiImportieren(Dateiname);
        }

        public void DateiImportieren(string Dateiname)
        {
            Liste = new List<Zuganzeige>();
            Verzeichnis = new Dictionary<string, int>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen
            var Kategory = XMLFile.Element("Anzeigeliste");
            var list = Kategory.Elements("Anzeige").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach (XElement anzeige in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                int PosX = Int16.Parse(anzeige.Element("PosX").Value);
                int PosY = Int16.Parse(anzeige.Element("PosY").Value);
                int XMLBreite = Int16.Parse(anzeige.Element("Breite").Value);

                string XMLName = anzeige.Attribute("Name").Value;
                string XMLAnzeigeName = anzeige.Element("AnzeigeName").Value;
                string XMLRichtung = anzeige.Element("Richtung").Value;

                Dictionary<int, string> MelderListe = new Dictionary<int, string>();
                var melderListe = anzeige.Element("Melderliste").Elements("Belegtmelder").ToList();
                foreach(XElement element in melderListe)
                {
                    MelderListe.Add(Int16.Parse(element.Attribute("Pos").Value), element.Value);
                }

                Liste.Add(
                    new Zuganzeige() 
                    { 
                        Name = XMLName, 
                        AnzeigeName = XMLAnzeigeName, 
                        X = PosX, 
                        Y = PosY, 
                        Breite = XMLBreite, 
                        Richtung = XMLRichtung, 
                        Meldername = MelderListe
                    });  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        public Zuganzeige GetZuganzeige(string Name)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Name, out ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

    }


    public class Zuganzeige
    {
        // Konstruktor
        public Zuganzeige() { }

        public int X { set; get; }
        public int Y { set; get; }

        public int Breite { set; get; }

        public string Name { set; get; }

        public string AnzeigeName { set; get; }

        // In welcher richtung ist die allgemeine Ausrichtung
        public string Richtung { set; get; }

        public Dictionary<int, string> Meldername { set; get; }

        public Dictionary<int, Belegtmelder> Melder { set; get; }


        public void ZeichneAnzeige(out TextBox Zugname, out PictureBox Zugtyp, out PictureBox FahrtVor, out PictureBox Fahrtrueck)
        {
            Zugname = new TextBox()
            {
                Name = Name,
                Width = Breite,
                MinimumSize = new Size(32, 20),
                Height = 20,
                TextAlign = HorizontalAlignment.Center,
            };
            Zugtyp = new PictureBox()
            {
                Name = Name + "_Typ",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(32, 20),
                Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.TypUnbesetzt),

            };
            FahrtVor = new PictureBox()
            {
                Name = Name + "_VFahrt",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(15, 18),
                Location = new Point(X * 32, Y * 32 + 8),
                Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksAktiv),

            };
            Fahrtrueck = new PictureBox()
            {
                Name = Name + "_RFahrt",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(15, 18),
                Location = new Point(X * 32 + 48 + Breite, Y * 32 + 8),
                Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv),

            };
            switch(Richtung)
            {
                case "rechts": 
                    Zugname.Location = new Point(X * 32 + 48, Y * 32 + 6);
                    Zugtyp.Location = new Point(X * 32 + 16, Y * 32 + 6);
                    FahrtVor.Location = new Point(X * 32, Y * 32 + 8);
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksAktiv);
                    Fahrtrueck.Location = new Point(X * 32 + 48 + Breite, Y * 32 + 8);
                    Fahrtrueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    Zugname.Location = new Point(X * 32 + 16, Y * 32 + 6);
                    Zugtyp.Location = new Point(X * 32 + 16 + Breite, Y * 32 + 6);
                    FahrtVor.Location = new Point(X * 32 + 48 + Breite, Y * 32 + 8);
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsAktiv);
                    Fahrtrueck.Location = new Point(X * 32, Y * 32 + 8);
                    Fahrtrueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default:break;
            }
        }
    }
}
