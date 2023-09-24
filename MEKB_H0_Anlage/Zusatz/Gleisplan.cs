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
                Abschnitt neuerAbschnitt = new Abschnitt();
                neuerAbschnitt.Name = xml_abschnitt.Attribute("Name").Value;
                var XML_StatusBedingung = xml_abschnitt.Elements("StatusBedingungen").ToList();
                foreach (XElement xml_SBedingung in XML_StatusBedingung)
                {
                    Abschnitt.StatusBedingung Bedingung = new Abschnitt.StatusBedingung();
                    Bedingung.Nummer = int.Parse(xml_SBedingung.Attribute("Nummer").Value);
                    Bedingung.Belegtmelder = xml_SBedingung.Element("Belegtmelder").Value;
                    foreach (XElement StrasseMit in xml_SBedingung.Element("Fahrstrassen_Mit").Elements("Fahrstrasse").ToList())
                    {
                        Bedingung.FahrstrassenMit.Add(StrasseMit.Value);
                    }
                    foreach (XElement StrasseGegen in xml_SBedingung.Element("Fahrstrassen_Gegen").Elements("Fahrstrasse").ToList())
                    {
                        Bedingung.FahrstrassenGegen.Add(StrasseGegen.Value);
                    }
                    neuerAbschnitt.Bedingungen.Add(Bedingung);
                }
                var XML_Gleise = xml_abschnitt.Elements("Gleis").ToList();
                foreach (XElement xml_Gleis in XML_Gleise)
                {
                    Abschnitt.GleisTyp gleisTyp = new Abschnitt.GleisTyp();

                    gleisTyp.Name = xml_Gleis.Attribute("Name").Value;
                    gleisTyp.PosX = int.Parse(xml_Gleis.Element("PosX").Value);
                    gleisTyp.PosY = int.Parse(xml_Gleis.Element("PosY").Value);
                    gleisTyp.Typ = xml_Gleis.Element("Typ").Value;

                    if(xml_Gleis.Element("Weiche") != null)
                    {
                        gleisTyp.Weiche = xml_Gleis.Element("Weiche").Value;
                        if(xml_Gleis.Element("WeichenBelegtmelder") != null)
                        {
                            gleisTyp.WeichenBelegtmelder = xml_Gleis.Element("WeichenBelegtmelder").Value;
                        }
                        else
                        {
                            gleisTyp.WeichenBelegtmelder ="";
                        }
                    }
                    else
                    {
                        gleisTyp.Weiche = "";
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
                Abschnitte.Add(neuerAbschnitt);

            }

        }


        public class Abschnitt
        {
            public string Name { get; set; }
            public List<StatusBedingung> Bedingungen { get; set; }
        
            public List<GleisTyp> Gleise { get; set; }
            public Abschnitt()
            {
                Bedingungen = new List<StatusBedingung>();
                Gleise = new List<GleisTyp>();
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
                }

                public int PosX { get; set; }
                public int PosY { get; set; }
                public string Name { get; set; }
                public string Typ { get; set; }

                public string Weiche { get; set; }
                public string WeichenBelegtmelder { get; set; }

                public int[] Bedingung { get; set; }

                public MeldeZustand[] Zustand { get; set; }
            }


            public class StatusBedingung
            {
                public StatusBedingung()
                {
                    FahrstrassenGegen = new List<string>();
                    FahrstrassenMit = new List<string>();
                }
                public int Nummer { get; set; }
                public string Belegtmelder { get; set; }
                public List<string> FahrstrassenMit { get; set; }
                public List<string> FahrstrassenGegen { get; set; }
            }

        }
    }
}
