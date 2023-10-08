using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public class Gleisplan
    {
        public List<Abschnitt> Abschnitte;

        public Gleisplan()
        {
            Abschnitte = new List<Abschnitt>();
        }

        public Gleisplan(string Dateiname)
        {
            DateiImportieren(Dateiname);
        }

        public void DateiImportieren(string Dateiname)
        {
            Abschnitte = new List<Abschnitt>();

            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen
            var XML_Abschnitte = XMLFile.Elements("Abschnitt").ToList();
            foreach (XElement xml_abschnitt in XML_Abschnitte)
            {
                Abschnitt neuerAbschnitt = new Abschnitt
                {
                    Name = xml_abschnitt.Attribute("Name").Value
                };
                var XML_StatusBedingung = xml_abschnitt.Elements("StatusBedingungen").ToList();
                foreach (XElement xml_SBedingung in XML_StatusBedingung)
                {
                    Abschnitt.StatusBedingung Bedingung = new Abschnitt.StatusBedingung
                    {
                        Nummer = int.Parse(xml_SBedingung.Attribute("Nummer").Value),
                        Belegtmelder = xml_SBedingung.Element("Belegtmelder").Value
                    };
                    foreach (XElement StrasseMit in xml_SBedingung.Element("Fahrstrassen_Mit").Elements("Fahrstrasse").ToList())
                    {
                        Bedingung.FahrstrassenMit.Add(StrasseMit.Value);
                    }
                    foreach (XElement StrasseGegen in xml_SBedingung.Element("Fahrstrassen_Gegen").Elements("Fahrstrasse").ToList())
                    {
                        Bedingung.FahrstrassenGegen.Add(StrasseGegen.Value);
                    }
                    var XML_Weichenbedingung = xml_SBedingung.Elements("WeichenBedingung").ToList();
                    foreach (XElement xml_WBedingung in XML_Weichenbedingung)
                    {
                        Dictionary<string, bool> AktiveWeichen = new Dictionary<string, bool>();
                        var XML_Aktiv = xml_WBedingung.Elements("Aktiv").ToList();
                        foreach (XElement xml_aktiv in XML_Aktiv)
                        {
                            string Weichennamen = xml_aktiv.Attribute("Weiche").Value;
                            bool Abzweig = xml_aktiv.Value.Equals("Abzweig");
                            AktiveWeichen.Add(Weichennamen, Abzweig);
                        }
                            
                        Bedingung.Aktiv.Add(AktiveWeichen);
                     }
                    neuerAbschnitt.Bedingungen.Add(Bedingung);
                }
                
                

                var XML_Gleise = xml_abschnitt.Elements("Gleis").ToList();
                foreach (XElement xml_Gleis in XML_Gleise)
                {
                    Abschnitt.GleisTyp gleisTyp = new Abschnitt.GleisTyp
                    {
                        Name = xml_Gleis.Attribute("Name").Value,
                        PosX = int.Parse(xml_Gleis.Element("PosX").Value),
                        PosY = int.Parse(xml_Gleis.Element("PosY").Value),
                        Typ = xml_Gleis.Element("Typ").Value
                    };
                    XElement xml_Label = xml_Gleis.Element("Label");
                    if (xml_Label != null)
                    {
                        gleisTyp.label = new Abschnitt.GleisTyp.Label();
                        gleisTyp.label.Text = xml_Label.Element("Text").Value;
                        gleisTyp.label.Rahmen = xml_Label.Element("Typ").Value.Equals("Rahmen");
                        gleisTyp.label.Groesse = int.Parse(xml_Label.Element("Schrift").Value);
                        gleisTyp.label.Fett = xml_Label.Element("Schrift").Attribute("Fett").Value.Equals("true");
                        gleisTyp.label.X_Offeset = int.Parse(xml_Label.Element("XOffset").Value);
                        gleisTyp.label.Y_Offeset = int.Parse(xml_Label.Element("YOffset").Value);
                    }

                    if (xml_Gleis.Element("Weiche") != null)
                    {
                        gleisTyp.Weiche = xml_Gleis.Element("Weiche").Value;
                        if (xml_Gleis.Element("Weiche2") != null) gleisTyp.Weiche_2nd = xml_Gleis.Element("Weiche2").Value;
                        else gleisTyp.Weiche_2nd = "";

                        if (xml_Gleis.Element("WeichenBelegtmelder") != null)
                        {
                            gleisTyp.WeichenBelegtmelder = xml_Gleis.Element("WeichenBelegtmelder").Value;                          
                        }
                        else
                        {
                            gleisTyp.WeichenBelegtmelder = "";
                        }
                    }
                    else
                    {
                        gleisTyp.Weiche = "";
                    }

                    if ((xml_Gleis.Element("Signal") != null) && gleisTyp.Weiche.Equals("")) // Signal vorhanden und keine Weiche
                    {
                        gleisTyp.Signal = xml_Gleis.Element("Signal").Value;
                    }
                    else
                    {
                        gleisTyp.Signal = "";
                    }

                    if ((xml_Gleis.Element("FahrstrassenButton") != null) && gleisTyp.Weiche.Equals("")) // Button für Fahrstrasse vorhanden und keine Weiche
                    {
                        gleisTyp.FahrstrassenButton = xml_Gleis.Element("FahrstrassenButton").Value;
                        if (xml_Gleis.Element("FahrstrassenButton").Attribute("Drehen") == null)
                        {
                            gleisTyp.ButtonDrehen = false;
                        }
                        else
                        {
                            if (xml_Gleis.Element("FahrstrassenButton").Attribute("Drehen").Value.Equals("true")) gleisTyp.ButtonDrehen = true;
                            else gleisTyp.ButtonDrehen = false;
                        }
                    }
                    else
                    {
                        gleisTyp.FahrstrassenButton = "";
                    }

                    var XML_Bedingung = xml_Gleis.Elements("Bedingung").ToList();
                    foreach(XElement xml_Bedingung in XML_Bedingung)
                    {
                        if(xml_Bedingung.Attribute("Teil") != null)
                        {
                            int teil = int.Parse(xml_Bedingung.Attribute("Teil").Value);
                            if (teil > 0 && teil < 4)
                            {
                                teil--;
                                if (xml_Bedingung.Value != null)
                                {
                                    int nummer = int.Parse(xml_Bedingung.Value);
                                    if (nummer > 0)
                                    {
                                        gleisTyp.Bedingung[teil] = nummer;
                                    }
                                }
                            }
                        }
                    }                     
                    neuerAbschnitt.Gleise.Add(gleisTyp);
                }

                var XML_Bilder = xml_abschnitt.Elements("Bild").ToList();
                foreach (XElement xml_Bilder in XML_Bilder)
                {
                    Abschnitt.Bilder neuesBild = new Abschnitt.Bilder();
                    neuesBild.Name = xml_Bilder.Element("Name").Value;
                    neuesBild.PosX = int.Parse(xml_Bilder.Element("PosX").Value);
                    neuesBild.PosY = int.Parse(xml_Bilder.Element("PosY").Value);
                    neuesBild.Base64String = xml_Bilder.Element("BildString").Value;
                    
                    neuerAbschnitt.BilderListe.Add(neuesBild);
                }
                Abschnitte.Add(neuerAbschnitt);

            }

        }


        public Abschnitt.GleisTyp SucheGleis(string Name)
        {
            foreach(Abschnitt abschnitt in Abschnitte)
            {
                foreach(Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (gleis.Name.Equals(Name)) return gleis;
                }
            }
            return null;
        }

        public class Abschnitt
        {
            public string Name { get; set; }
            public List<StatusBedingung> Bedingungen { get; set; }
            
            public List<Bilder> BilderListe { get; set; }
            public List<GleisTyp> Gleise { get; set; }
            public Abschnitt()
            {
                Bedingungen = new List<StatusBedingung>();
                Gleise = new List<GleisTyp>();
                BilderListe = new List<Bilder>();
            }
            public class GleisTyp
            {
                public GleisTyp()
                {
                    Bedingung = new int[3];
                    Zustand = new MeldeZustand[3];
                    Zustand[0] = new MeldeZustand(false);
                    Zustand[1] = new MeldeZustand(false);
                    Zustand[2] = new MeldeZustand(false);
                    Weiche_2nd = "";
                    Signal = "";
                }

                public int PosX { get; set; }
                public int PosY { get; set; }
                public string Name { get; set; }
                public string Typ { get; set; }

                public Label label { get; set; }

                public string Weiche { get; set; }
                public string Weiche_2nd { get; set; }
                public string WeichenBelegtmelder { get; set; }

                public string FahrstrassenButton { get; set; }
                public bool ButtonDrehen { get; set; }

                public string Signal { get; set; }

                public int[] Bedingung { get; set; }

                public MeldeZustand[] Zustand { get; set; }

                public class Label
                {
                    public Label()
                    {
                        Text = "";
                    }
                    public string Text { get; set; }
                    public bool Rahmen { get; set; }
                    public int Groesse { get; set; }
                    public bool Fett { get; set; }

                    public int X_Offeset { get; set; }
                    public int Y_Offeset { get; set; }

                }
            }

            public class Bilder
            {
                public string Name { get; set;}
                public int PosX { get; set; }
                public int PosY { get; set; }
                public string Base64String { get; set; }
            }

            public class StatusBedingung
            {
                public StatusBedingung()
                {
                    FahrstrassenGegen = new List<string>();
                    FahrstrassenMit = new List<string>();
                    Aktiv = new List<Dictionary<string, bool>>();
                }
                /// <summary>
                /// Sonderbedingungen das Belegtmelder angezeigt werden soll
                /// </summary>
                public List<Dictionary<string,bool>> Aktiv { set; get; }
                public int Nummer { get; set; }
                public string Belegtmelder { get; set; }
                public List<string> FahrstrassenMit { get; set; }
                public List<string> FahrstrassenGegen { get; set; }
            }

        }
    }
}
