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
                var XML_Belegtmelder = xml_abschnitt.Elements("Belegtmelder").ToList();
                foreach (XElement xml_Bloecke in XML_Belegtmelder)
                {
                    Abschnitt.Block neuerBlock = new Abschnitt.Block();
                    neuerBlock.Belegtmelder = xml_Bloecke.Attribute("Name").Value;

                    var XML_FahrstrassenMit = xml_Bloecke.Element("Fahrstrassen_Mit");
                    if (XML_FahrstrassenMit != null)
                    {
                        var XML_FahrstrassenMitList = XML_FahrstrassenMit.Elements("Fahrstrasse").ToList();
                        foreach (XElement StrasseMit in XML_FahrstrassenMitList)
                        {
                            neuerBlock.FahrstrassenMit.Add(StrasseMit.Value);
                        }
                    }
                    var XML_FahrstrassenGegen = xml_Bloecke.Element("Fahrstrassen_Gegen");
                    if (XML_FahrstrassenGegen != null)
                    {
                        var XML_FahrstrassenGegenList = XML_FahrstrassenGegen.Elements("Fahrstrasse").ToList();
                        foreach (XElement StrasseGegen in XML_FahrstrassenGegenList)
                        {
                            neuerBlock.FahrstrassenGegen.Add(StrasseGegen.Value);
                        }
                    }
                    var XML_Gleise = xml_Bloecke.Elements("Gleis").ToList();
                    foreach (XElement xml_Gleis in XML_Gleise)
                    {
                        Abschnitt.Block.GleisTyp gleisTyp = new Abschnitt.Block.GleisTyp();
                        gleisTyp.zustand = new MeldeZustand();
                        gleisTyp.Name = xml_Gleis.Attribute("Name").Value;
                        gleisTyp.PosX = int.Parse(xml_Gleis.Element("PosX").Value);
                        gleisTyp.PosY = int.Parse(xml_Gleis.Element("PosY").Value);
                        gleisTyp.Typ = xml_Gleis.Element("Typ").Value;

                        var XML_Bedingung = xml_Gleis.Elements().ToList();
                        foreach(XElement xml_Bedingung in XML_Bedingung)
                        {
                            int pos = int.Parse(xml_Bedingung.Attribute("Teil").Value);
                            gleisTyp.Weiche[pos] = xml_Bedingung.Element("Stellung").Attribute("Weiche").Value;
                            gleisTyp.Stellung[pos] = xml_Bedingung.Element("Stellung").Value;
                        }

                        neuerBlock.GleisTypen.Add(gleisTyp);
                    }
                    neuerAbschnitt.Blocks.Add(neuerBlock);
                }
                Abschnitte.Add(neuerAbschnitt);

            }

        }


        public class Abschnitt
        {
            public string Name { get; set; }
            public List<Block> Blocks { get; set; }

            public Abschnitt()
            {
                Blocks = new List<Block>();
            }

            public class Block
            {
                public Block()
                {
                    GleisTypen = new List<GleisTyp>();
                    FahrstrassenMit = new List<string>();
                    FahrstrassenGegen = new List<string>();
                }

                public List<string> FahrstrassenMit { get; set; }
                public List<string> FahrstrassenGegen { get; set; }

                public string Belegtmelder { get; set; }

                public List<GleisTyp> GleisTypen { get; set; }

                public class GleisTyp
                {
                    public GleisTyp()
                    {
                       
                    }

                    public int PosX { get; set; }
                    public int PosY { get; set; }
                    public string Name { get; set; }
                    public string Typ { get; set; }

                    public string[] Weiche = new string[3];
                    public string[] Stellung = new string[3];
                    public MeldeZustand zustand { get; set; }
                }
            }

        }
    }
}
