using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{

    public sealed class ZuganzeigerListe
    {
        //Lazy Instance
        private static readonly Lazy<ZuganzeigerListe> lazy =
        new Lazy<ZuganzeigerListe>(() => new ZuganzeigerListe());

        public static ZuganzeigerListe Instance { get { return lazy.Value; } }

        // Liste und Verzeichnis
        private Dictionary<string, int> Verzeichnis;
        public List<Zuganzeige> Liste;


        public Fehlermeldung Fehlermeldungen = Fehlermeldung.Instance;

        public ZuganzeigerListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Zuganzeige>();
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
                    });  //Mit den Werten eine neue Zuganzeíge in die Liste hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        public Zuganzeige GetZuganzeige(string Name)
        {
            if (Verzeichnis.TryGetValue(Name, out int ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

        public void ZeichneZuganzeigen(Control.ControlCollection Controls)
        {
            foreach (Zuganzeige zuganzeige in Liste)
            {
                zuganzeige.ZeichneAnzeige(out TextBox Anzeige, out PictureBox Typ, out PictureBox VFahrt, out PictureBox RFahrt);

                Anzeige.Text = zuganzeige.AnzeigeName;

                Controls.Add(Anzeige);
                Anzeige.BringToFront();

                Controls.Add(Typ);
                Typ.BringToFront();

                Controls.Add(VFahrt);
                VFahrt.BringToFront();

                Controls.Add(RFahrt);
                RFahrt.BringToFront();
            }
        }

        public void ZuganzeigenAktualisieren(Control.ControlCollection Controls, bool ErzwingeUpdate = false)
        {
            foreach (Zuganzeige zuganzeige in Liste)
            {
                zuganzeige.ZuganzeigenAktualisieren(Controls, ErzwingeUpdate);
            }
        }

        public void BelegtmelderVerknuepfen(BelegtmelderListe belegtmelderListe)
        {
            foreach (Zuganzeige zuganzeige in Liste)
            {
                foreach(KeyValuePair<int, string> entry in zuganzeige.Meldername)
                {
                    Belegtmelder belegtmelder = belegtmelderListe.GetBelegtmelder(entry.Value);
                    if (belegtmelder != null)
                    {
                        zuganzeige.Melder.Add(entry.Key, belegtmelder);
                    }
                    else
                    {
                        Fehlermeldungen.FehlerMelden(String.Format("Belegtmelder: {0} nicht gefunden", entry.Value), "Error");
                    }
                }
            }
        }

        

    }


    public class Zuganzeige
    {
        // Konstruktor
        public Zuganzeige() 
        {
            Melder = new Dictionary<int, Belegtmelder>();
            Meldername = new Dictionary<int, string>();
            aktLokname = "";
        }

        private AktiveLokomotiven AktiveLokomotiven = AktiveLokomotiven.Instance;

        public int X { set; get; }
        public int Y { set; get; }

        public int Breite { set; get; }

        public string Name { set; get; }

        public string AnzeigeName { set; get; }

        // In welcher richtung ist die allgemeine Ausrichtung
        public string Richtung { set; get; }

        public Dictionary<int, string> Meldername { set; get; }

        public Dictionary<int, Belegtmelder> Melder { set; get; }

        private bool istBelegt;
        private bool unbekannteLok;
        private bool fehler;
        private string aktLokname;

        public bool NeedUpdate()
        {
            ErrechneStatus(out bool Belegt, out bool UnbekannteLok, out bool Fehler, out string Lokname);

            // Änderung gegenüber vorheriger Abfrage
            if((istBelegt == Belegt) && (unbekannteLok == UnbekannteLok) && (fehler == Fehler) && aktLokname.Equals(Lokname))
            {
                return false;
            }
            else
            {
                istBelegt = Belegt;
                unbekannteLok = UnbekannteLok;
                fehler = Fehler;
                aktLokname = Lokname;
                return true; 
            }

        }

        public void ErrechneStatus(out bool Belegt, out bool UnbekannteLok, out bool Fehler, out string Lokname)
        {
            Belegt = false;
            UnbekannteLok = true;
            Fehler = false;
            Lokname = "";
            foreach (KeyValuePair<int, Belegtmelder> entry in Melder)
            {
                if (entry.Value.IstBelegt())
                {
                    Belegt = true;
                }
                if (!entry.Value.Registriert.Equals("Deregistriert"))
                {
                    if (!entry.Value.Registriert.Equals(""))
                    {
                        if (Lokname.Equals("")) { Lokname = entry.Value.Registriert; UnbekannteLok = false; }
                        else Fehler = true; // Zwei Loks im Bereich
                    }
                }
            }
        }

        public bool ErrechneRichtung(string aktuellerBlock, string vorherigerBlock, out bool Richtung)
        {
            int PosAktuell = 0;
            int PosVorherige = 0;
            foreach (KeyValuePair<int, string> entry in Meldername)
            {
                if(aktuellerBlock.Equals(entry.Value)) PosAktuell = entry.Key;
                if(vorherigerBlock.Equals(entry.Value)) PosVorherige = entry.Key;
            }
            if (PosAktuell == 0) { Richtung = false; return false; }

            if (PosVorherige == 0)
            {
                if (PosAktuell == 1) { Richtung = false; return true; }
                if (PosAktuell == Meldername.Count) { Richtung = true; return true; }
            }
            if (PosAktuell < PosVorherige) { Richtung = true; return true; }
            if (PosAktuell > PosVorherige) { Richtung = false; return true;}

            Richtung = false;
            return false;
        }

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
                Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv),

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
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    FahrtVor.Tag = "links";
                    Fahrtrueck.Location = new Point(X * 32 + 48 + Breite, Y * 32 + 8);
                    Fahrtrueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    Fahrtrueck.Tag = "rechts";
                    break;
                case "links":
                    Zugname.Location = new Point(X * 32 + 16, Y * 32 + 6);
                    Zugtyp.Location = new Point(X * 32 + 16 + Breite, Y * 32 + 6);
                    FahrtVor.Location = new Point(X * 32 + 48 + Breite, Y * 32 + 8);
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    FahrtVor.Tag = "rechts";
                    Fahrtrueck.Location = new Point(X * 32, Y * 32 + 8);
                    Fahrtrueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    Fahrtrueck.Tag = "links";
                    break;
                default:break;
            }
        }

        public void ZuganzeigenAktualisieren(Control.ControlCollection Controls, bool ErzwingeUpdate = false)
        {
            if (NeedUpdate() || ErzwingeUpdate)
            {
                ErrechneStatus(out bool Belegt, out bool UnbekannteLok, out bool Fehler, out string Lokname);
                if (Fehler) { ZuganzeigeDoppelbelegung(Controls); }
                else
                {
                    if (Belegt)
                    {
                        if (UnbekannteLok) { ZuganzeigeUnbekannteLok(Controls); }
                        else
                        {
                            Lokomotive lokomotive = AktiveLokomotiven.GetLokomotive(Lokname); //Finde Lok mit diesem Name
                            if(lokomotive == null)
                            {
                                ZuganzeigeUnbekannteLok(Controls); return;
                            }
                            if (ErrechneRichtung(lokomotive.AktuellerBlock, lokomotive.VorherigerBlock, out bool Richtung))
                                ZuganzeigeBelegt(Controls, Richtung, lokomotive);
                        }
                    }
                    else
                    {
                        ZuganzeigeUnbelegt(Controls);
                    }
                }
            }           
        }

        private void ZuganzeigeDoppelbelegung(Control.ControlCollection Controls)
        {
            PictureBox FahrtVor = (PictureBox)Controls.Find(Name + "_VFahrt", true).First();
            if (FahrtVor == null) return; //Nicht gefunden: Abbrechen
            PictureBox FahrtRueck = (PictureBox)Controls.Find(Name + "_RFahrt", true).First();
            if (FahrtRueck == null) return; //Nicht gefunden: Abbrechen
            PictureBox ZugTyp = (PictureBox)Controls.Find(Name + "_Typ", true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen
            TextBox AnzeigeName = (TextBox)Controls.Find(Name, true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen

            AnzeigeName.Text = "Doppelbelegung";
            AnzeigeName.BackColor = Color.RosyBrown;
            AnzeigeName.ForeColor = Color.Black;
            ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.TypUnbekannt);
            switch ((string)FahrtRueck.Tag)
            {
                case "rechts":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
            switch ((string)FahrtVor.Tag)
            {
                case "rechts":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
        }

        private void ZuganzeigeUnbelegt(Control.ControlCollection Controls)
        {
            PictureBox FahrtVor = (PictureBox)Controls.Find(Name + "_VFahrt", true).First();
            if (FahrtVor == null) return; //Nicht gefunden: Abbrechen
            PictureBox FahrtRueck = (PictureBox)Controls.Find(Name + "_RFahrt", true).First();
            if (FahrtRueck == null) return; //Nicht gefunden: Abbrechen
            PictureBox ZugTyp = (PictureBox)Controls.Find(Name + "_Typ", true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen
            TextBox AnzeigeName = (TextBox)Controls.Find(Name, true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen

            AnzeigeName.Text = Name;
            AnzeigeName.BackColor = Color.Gray;
            AnzeigeName.ForeColor = Color.DarkGray;
            ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.TypUnbesetzt);
            switch ((string)FahrtRueck.Tag)
            {
                case "rechts":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
            switch ((string)FahrtVor.Tag)
            {
                case "rechts":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
        }

        private void ZuganzeigeUnbekannteLok(Control.ControlCollection Controls)
        {
            PictureBox FahrtVor = (PictureBox)Controls.Find(Name + "_VFahrt", true).First();
            if (FahrtVor == null) return; //Nicht gefunden: Abbrechen
            PictureBox FahrtRueck = (PictureBox)Controls.Find(Name + "_RFahrt", true).First();
            if (FahrtRueck == null) return; //Nicht gefunden: Abbrechen
            PictureBox ZugTyp = (PictureBox)Controls.Find(Name + "_Typ", true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen
            TextBox AnzeigeName = (TextBox)Controls.Find(Name, true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen

            AnzeigeName.Text = "Unbekannte Lok";
            AnzeigeName.BackColor = Color.RosyBrown;
            AnzeigeName.ForeColor = Color.Black;
            ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.TypUnbekannt);
            switch ((string)FahrtRueck.Tag)
            {
                case "rechts":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
            switch ((string)FahrtVor.Tag)
            {
                case "rechts":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
        }

        private void ZuganzeigeBelegt(Control.ControlCollection Controls, bool Richtung, Lokomotive Lokname)
        {
            PictureBox FahrtVor = (PictureBox)Controls.Find(Name + "_VFahrt", true).First();
            if (FahrtVor == null) return; //Nicht gefunden: Abbrechen
            PictureBox FahrtRueck = (PictureBox)Controls.Find(Name + "_RFahrt", true).First();
            if (FahrtRueck == null) return; //Nicht gefunden: Abbrechen
            PictureBox ZugTyp = (PictureBox)Controls.Find(Name + "_Typ", true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen
            TextBox AnzeigeName = (TextBox)Controls.Find(Name, true).First();
            if (ZugTyp == null) return; //Nicht gefunden: Abbrechen

            AnzeigeName.Text = Lokname.Name;
            AnzeigeName.BackColor = Color.White;
            AnzeigeName.ForeColor = Color.Black;

            switch (Lokname.Gattung)
            {
                case "InterCityExpress": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.ICE); break;
                case "InterCity": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.IC); break;
                case "InterRegioExpress": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.IRE); break;
                case "InterRegio": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.IR); break;
                case "RegionalExpress": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RE); break;
                case "RegionalBahn": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RB); break;
                case "S-Bahn": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.S_Bahn); break;
                case "U-Bahn": ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.U_Bahn); break;
                default: ZugTyp.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.TypUnbekannt); break;
            }


            switch ((string)FahrtRueck.Tag)
            {
                case "rechts":
                    if (Richtung) FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    else FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsAktiv);
                    break;
                case "links":
                    if (Richtung) FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    else FahrtRueck.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksAktiv);
                    break;
                default: break;
            }
            switch ((string)FahrtVor.Tag)
            {
                case "rechts":
                    if (Richtung) FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsAktiv);
                    else FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.RechtsInaktiv);
                    break;
                case "links":
                    if (Richtung) FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksAktiv);
                    else FahrtVor.Image = new Bitmap(global::MEKB_H0_Anlage.Properties.Resources.LinksInaktiv);
                    break;
                default: break;
            }
        }
    }
}
