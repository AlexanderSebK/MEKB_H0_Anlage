using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public class Gleisplan
    {
        public List<Abschnitt> Abschnitte;

        MeldeZustand FreiesGleis = new MeldeZustand(false);
        private Control.ControlCollection Controls;

        private GleisbildZeichnung GleisbildZeichnung = GleisbildZeichnung.Instance;
        private SignalListe SignalListe = SignalListe.Instance;
        private WeichenListe WeichenListe = WeichenListe.Instance;
        private FahrstrassenListe FahrstrassenListe = FahrstrassenListe.Instance;
        private BelegtmelderListe BelegtmelderListe = BelegtmelderListe.Instance;
        private Systemzustand Systemzustand = Systemzustand.Instance;

        private Einstellungen Einstellungen = Einstellungen.Instance;

        private List<string> SperrButtons = new List<string>();

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
            var Kategory = XMLFile.Element("Gleisplan");
            var XML_Abschnitte = Kategory.Elements("Abschnitt").ToList();
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
                        gleisTyp.Gleislabel = new Abschnitt.GleisTyp.Label
                        {
                            Text = xml_Label.Element("Text").Value,
                            Rahmen = xml_Label.Element("Typ").Value.Equals("Rahmen"),
                            Groesse = int.Parse(xml_Label.Element("Schrift").Value),
                            Fett = xml_Label.Element("Schrift").Attribute("Fett").Value.Equals("true"),
                            X_Offeset = int.Parse(xml_Label.Element("XOffset").Value),
                            Y_Offeset = int.Parse(xml_Label.Element("YOffset").Value)
                        };
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

                    if ((xml_Gleis.Element("SperrButton") != null) && gleisTyp.Weiche.Equals("") && gleisTyp.Signal.Equals("") && gleisTyp.FahrstrassenButton.Equals("")) // Keine anderen Optionen und SperrButton
                    {
                        gleisTyp.SperrButton = true;
                        var XML_SperrStrassen = xml_Gleis.Element("SperrButton").Elements("Fahrstrasse").ToList();
                        foreach (XElement XML_sperrung in XML_SperrStrassen)
                        {
                            gleisTyp.GesperrteFahrstrassen.Add(XML_sperrung.Value);
                        }
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
                    Abschnitt.Bilder neuesBild = new Abschnitt.Bilder
                    {
                        Name = xml_Bilder.Element("Name").Value,
                        PosX = int.Parse(xml_Bilder.Element("PosX").Value),
                        PosY = int.Parse(xml_Bilder.Element("PosY").Value),
                        Base64String = xml_Bilder.Element("BildString").Value
                    };

                    neuerAbschnitt.BilderListe.Add(neuesBild);
                }
                Abschnitte.Add(neuerAbschnitt);

            }

        }

        public void ControlsZuweisen(Control.ControlCollection controls)
        {
            Controls = controls;
        }


        /// <summary>
        /// Initialen Gleisplan zeichnen. Anschließend nur noch funktion GleisplanZeichen() für updates verwenden
        /// </summary>
        public void ZeichnenInitial()
        {
            foreach (Abschnitt abschnitt in Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.Bilder bild in abschnitt.BilderListe)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        // Convert Image to byte[]
                        Properties.Resources.Drehscheibe.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        //byte[] imageBytes = ms.ToArray();

                        // Convert byte[] to base 64 string
                        string base64String = Convert.ToBase64String(ms.ToArray());
                    }




                    // Convert base 64 string to byte[]
                    byte[] imageBytes = Convert.FromBase64String(bild.Base64String);
                    // Convert byte[] to Image
                    using (var ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        PictureBox neuesBild = new PictureBox()
                        {
                            Name = bild.Name,
                            Size = new Size(image.Width, image.Height),
                            Location = new Point(bild.PosX * 32, bild.PosY * 32),
                            Image = image
                        };
                        Controls.Add(neuesBild);
                    }
                }

                foreach (Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (!Controls.ContainsKey(gleis.Name))
                    {
                        PictureBox neuesGleis = new PictureBox()
                        {
                            Name = gleis.Name,
                            Tag = gleis.Typ,
                            Size = new Size(32, 32),
                            Location = new Point(gleis.PosX * 32, gleis.PosY * 32),
                            Image = new Bitmap(GleisbildZeichnung.Katalog[36][7]),
                        };
                        if (!gleis.Weiche.Equals(""))
                        {
                            if (gleis.Typ.StartsWith("DKW")) neuesGleis.Click += new System.EventHandler(this.DKW_Click);
                            else if (gleis.Typ.StartsWith("KW")) neuesGleis.Click += new System.EventHandler(this.KW_Click);
                            //else if (gleis.Typ.StartsWith("Dreiweg")) neuesGleis.Click += new System.EventHandler(this.Dreiweg_Click);
                            else neuesGleis.Click += new System.EventHandler(this.Weiche_Click);
                        }
                        else if (!gleis.Signal.Equals(""))
                        {
                            neuesGleis.Click += new System.EventHandler(this.Signal_Click);
                        }

                        Controls.Add(neuesGleis);

                        if (gleis.Gleislabel != null)
                        {
                            Label neuesLabel = new Label
                            {
                                AutoSize = true,
                                Text = gleis.Gleislabel.Text,
                                Location = new Point(gleis.PosX * 32 + gleis.Gleislabel.X_Offeset, gleis.PosY * 32 + gleis.Gleislabel.Y_Offeset)
                            };
                            if (gleis.Gleislabel.Fett) neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.Gleislabel.Groesse, FontStyle.Bold);
                            else neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.Gleislabel.Groesse, FontStyle.Regular);
                            neuesLabel.Name = gleis.Name + "Label";
                            if (gleis.Gleislabel.Rahmen) neuesLabel.BorderStyle = BorderStyle.FixedSingle;
                            Controls.Add(neuesLabel);
                            neuesLabel.BringToFront();
                        }

                        if (!gleis.FahrstrassenButton.Equals(""))
                        {
                            Button button = new Button
                            {
                                Name = gleis.FahrstrassenButton + "_Button",
                                Size = new System.Drawing.Size(16, 16),
                                BackColor = System.Drawing.Color.Yellow,
                                Margin = new System.Windows.Forms.Padding(0),
                                BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center,
                                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                                ForeColor = System.Drawing.SystemColors.ControlText,
                                Tag = gleis.FahrstrassenButton,
                                UseVisualStyleBackColor = false,
                            };
                            button.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                            button.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
                            button.FlatAppearance.BorderSize = 0;
                            button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
                            button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;

                            

                            bool Buttonzeichen = true;
                            string Winkel = GetButtonWinkel(gleis.Typ, gleis.ButtonDrehen);

                            if (Winkel.Equals("0"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_oben;
                                button.BackgroundImage.Tag = "oben";
                                button.Location = new Point(gleis.PosX * 32 + 8, gleis.PosY * 32 + 16);
                            }
                            else if (Winkel.Equals("90"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_links;
                                button.BackgroundImage.Tag = "links";
                                button.Location = new Point(gleis.PosX * 32 + 16, gleis.PosY * 32 + 8);
                            }
                            else if (Winkel.Equals("180"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_unten;
                                button.BackgroundImage.Tag = "unten";
                                button.Location = new Point(gleis.PosX * 32 + 8, gleis.PosY * 32 + 0);
                            }
                            else if (Winkel.Equals("270"))
                            {
                                button.BackgroundImage = global::MEKB_H0_Anlage.Properties.Resources.Fahrstrasse_rechts;
                                button.BackgroundImage.Tag = "rechts";
                                button.Location = new Point(gleis.PosX * 32 + 0, gleis.PosY * 32 + 8);
                            }
                            else
                            {
                                Buttonzeichen = false; // Button nicht setzen;
                            }

                            //TODO: Untermenü für Buttons hier einfügen
                            Fahrstrasse fahrstrasse = FahrstrassenListe.GetFahrstrasse(gleis.FahrstrassenButton);
                            if(fahrstrasse != null)
                            {
                                if (fahrstrasse.Fahrstr_GleicherEingang.Count >= 1)
                                {
                                    ContextMenu SubMenu = new ContextMenu();
                                    foreach (string strasse in fahrstrasse.Fahrstr_GleicherEingang)
                                    {
                                        MenuItem item = new MenuItem();

                                        //Nur letzten Teil (Ziel) übernehmen
                                        string[] text = strasse.Split('_');
                                        item.Text = text[2];

                                        item.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                                        item.Tag = strasse;

                                        SubMenu.MenuItems.Add(item);
                                    }
                                    button.ContextMenu = SubMenu;
                                }
                            }
                            

                            if (Buttonzeichen)
                            {
                                Controls.Add(button);
                                button.BringToFront();
                            }
                        }

                        if (gleis.SperrButton)
                        {
                            CheckBox SHO_Sperrung = new CheckBox
                            {
                                Name = gleis.Name + "_SHO",
                                Appearance = System.Windows.Forms.Appearance.Button,
                                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                                Image = global::MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv,
                                Location = new System.Drawing.Point(gleis.PosX * 32 + 0, gleis.PosY * 32 + 8),
                                Size = new System.Drawing.Size(32, 16),
                                UseVisualStyleBackColor = true
                            };
                            SHO_Sperrung.CheckedChanged += new System.EventHandler(this.SperrungSh2_CheckedChanged);
                            SHO_Sperrung.Tag = String.Join("+", gleis.GesperrteFahrstrassen);
                            Controls.Add(SHO_Sperrung);
                            SHO_Sperrung.BringToFront();
                            SperrButtons.Add(SHO_Sperrung.Name);
                        }

                        if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, FreiesGleis, neuesGleis);
                        }
                        else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, neuesGleis);
                        }
                        else if (gleis.Bedingung[0] != 0)
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, neuesGleis);
                        }
                        else if (!gleis.Weiche.Equals(""))
                        {
                            Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                            if (gleis.Weiche_2nd.Equals(""))
                            {
                                if (gleis.Bedingung[1] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, neuesGleis);
                                }
                                else if (gleis.Bedingung[2] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, FreiesGleis, neuesGleis);
                                }
                            }
                            else
                            {
                                Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, neuesGleis);
                            }
                        }
                        if (!gleis.Signal.Equals(""))
                        {
                            GleisbildZeichnung.ZeichneSchaltbild(SignalListe.GetSignal(gleis.Signal), neuesGleis);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Alle Gleise zeichnen. Inklusive abfrage ob neuzeichnen notwendig ist
        /// </summary>
        public void GleisplanZeichnen()
        {
            foreach (Gleisplan.Abschnitt abschnitt in Abschnitte)
            {
                // Aktueller Zustand der Gleise in diesem Abschnitt
                Dictionary<int, MeldeZustand> aktZustand = new Dictionary<int, MeldeZustand>();

                // Aktuellen Zustand berechnen
                foreach (Gleisplan.Abschnitt.StatusBedingung bedingung in abschnitt.Bedingungen)
                {
                    bool BelegtmelderStatus = false;
                    if (bedingung.Aktiv.Count > 0)
                    {
                        foreach (Dictionary<string, bool> entry in bedingung.Aktiv)
                        {
                            bool AND_bedingung = true;
                            foreach (KeyValuePair<string, bool> Weichenentry in entry)
                            {
                                bool WeichenAbzweig = WeichenListe.GetWeiche(Weichenentry.Key).Abzweig;
                                if (WeichenAbzweig != Weichenentry.Value) AND_bedingung = false;
                            }
                            if (AND_bedingung) BelegtmelderStatus = true;
                        }
                    }
                    else
                    {
                        BelegtmelderStatus = true; // Keine Bedingung - immer anzeigen
                    }
                    // Wenn Belegtmelderstatus gewünscht ist
                    if (BelegtmelderStatus) BelegtmelderStatus = BelegtmelderListe.GetBelegtStatus(bedingung.Belegtmelder);

                    aktZustand.Add(bedingung.Nummer, ErrechneZustand(
                            BelegtmelderStatus,
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenMit.ToArray()),
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenGegen.ToArray())));
                }



                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    bool update = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (gleis.Bedingung[i] == 0) continue; // Keine Bedingung
                        if (!gleis.Zustand[i].Equals(aktZustand[gleis.Bedingung[i]])) update = true; //Gezeichnete Bedingung mit aktueller vergleichen
                        if (!gleis.Signal.Equals("")) //Signal vorhanden
                        {
                            if (!SignalListe.GetSignal(gleis.Signal).Zustand.Equals(gleis.SignalZustand)) update = true;
                            else update = true;
                        }
                    }

                    if (update) //Nur neu Zeichnen wenn Unterschied vorhanden
                    {
                        var Fund = Controls.Find(gleis.Name, true); //Bildelement zum Gleis finden
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox) // Element gefunden
                            {
                                switch (GetGleisBezeichnung(gleis))
                                {
                                    case GleistypBezeichnung.Gleis:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.GleisSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Gleis1Ecke:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.Gleis1EckeSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Gleis2Ecken:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            aktZustand[gleis.Bedingung[2]],
                                            Picbox);
                                        break;
                                    case GleistypBezeichnung.Gleis2EckenSignal:
                                        GleisbildZeichnung.ZeichneSchaltbild(
                                            aktZustand[gleis.Bedingung[0]],
                                            aktZustand[gleis.Bedingung[1]],
                                            aktZustand[gleis.Bedingung[2]],
                                            SignalListe.GetSignal(gleis.Signal),
                                            Picbox, true);
                                        gleis.SignalZustand = SignalListe.GetSignal(gleis.Signal).Zustand;
                                        break;
                                    case GleistypBezeichnung.Weiche1Motor:
                                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                                        if (!gleis.WeichenBelegtmelder.Equals(""))
                                        {
                                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                                            {
                                                weiche.Besetzt = aktZustand[index].Besetzt;
                                            }
                                        }
                                        if (gleis.Bedingung[1] == 0)
                                        {
                                            GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox);
                                        }
                                        else if (gleis.Bedingung[2] == 0)
                                        {
                                            GleisbildZeichnung.ZeichneSchaltbild(weiche, aktZustand[gleis.Bedingung[1]], Picbox, true);
                                        }
                                        break;
                                    case GleistypBezeichnung.Weiche2Motoren:
                                        Weiche weiche1 = WeichenListe.GetWeiche(gleis.Weiche);
                                        Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                        if (!gleis.WeichenBelegtmelder.Equals(""))
                                        {
                                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                                            {
                                                weiche1.Besetzt = aktZustand[index].Besetzt;
                                                weiche2.Besetzt = aktZustand[index].Besetzt;
                                            }
                                        }

                                        GleisbildZeichnung.ZeichneSchaltbild(weiche1, weiche2, Picbox);
                                        break;
                                    default: break;
                                }
                                // Aktuellen Zustand als gezeichneten Zustand übernehmen
                                for (int i = 0; i < 3; i++)
                                {
                                    if (gleis.Bedingung[i] == 0) continue; // Keine Bedingung
                                    gleis.Zustand[i] = aktZustand[gleis.Bedingung[i]];
                                }

                            } // if picturebox
                        } // foreach Control
                    } // if(update)

                    if (!gleis.Weiche.Equals("") && gleis.Weiche_2nd.Equals(""))
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        if (!gleis.WeichenBelegtmelder.Equals(""))
                        {
                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                            {
                                weiche.Besetzt = aktZustand[index].Besetzt;
                            }
                        }
                        var Fund = this.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                if (gleis.Bedingung[1] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox, true);
                                }
                                else if (gleis.Bedingung[2] == 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, aktZustand[gleis.Bedingung[1]], Picbox, true);
                                }
                            }
                        }
                    }
                    else if (!gleis.Weiche.Equals("") && !gleis.Weiche_2nd.Equals(""))
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                        if (!gleis.WeichenBelegtmelder.Equals(""))
                        {
                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                            {
                                weiche.Besetzt = aktZustand[index].Besetzt;
                                weiche2.Besetzt = aktZustand[index].Besetzt;
                            }
                        }
                        var Fund = this.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, Picbox, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subfunktion: Winkel des Buttons an hand des Gleistyps berechnen
        /// </summary>
        /// <param name="GleisTyp">Gleistyp</param>
        /// <param name="drehen">Auf welcher Seite des Gleises ist der Button</param>
        /// <returns>Winkel des Buttons oder "nicht erlaubt" wenn unzulässig</returns>
        public string GetButtonWinkel(string GleisTyp, bool drehen)
        {
            string Typ = GleisTyp.Split('+').First();
            string Winkel = Typ.Split('_').Last();
            Typ = Typ.Split('_').First();

            if (Typ.Equals("KurveL") && drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "nicht erlaubt"; break;
                    case "45": Winkel = "270"; break;
                    case "90": Winkel = "nicht erlaubt"; break;
                    case "135": Winkel = "0"; break;
                    case "180": Winkel = "nicht erlaubt"; break;
                    case "225": Winkel = "90"; break;
                    case "270": Winkel = "nicht erlaubt"; break;
                    case "315": Winkel = "180"; break;
                    default: Winkel = "nicht erlaubt"; break;
                }
            }
            else if (Typ.Equals("KurveR") && drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "nicht erlaubt"; break;
                    case "45": Winkel = "180"; break;
                    case "90": Winkel = "nicht erlaubt"; break;
                    case "135": Winkel = "270"; break;
                    case "180": Winkel = "nicht erlaubt"; break;
                    case "225": Winkel = "0"; break;
                    case "270": Winkel = "nicht erlaubt"; break;
                    case "315": Winkel = "90"; break;
                    default: Winkel = "nicht erlaubt"; break;
                }
            }


            else if (drehen)
            {
                switch (Winkel)
                {
                    case "0": Winkel = "180"; break;
                    case "90": Winkel = "270"; break;
                    case "180": Winkel = "0"; break;
                    case "270": Winkel = "90"; break;
                    default: break;
                }
            }

            return Winkel;
        }

        /// <summary>
        /// Gleis anhand Name heraussuchen
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gleistyp ermitteln
        /// </summary>
        /// <param name="gleis">Gleis</param>
        /// <returns>Typ des Gleis</returns>
        private GleistypBezeichnung GetGleisBezeichnung(Gleisplan.Abschnitt.GleisTyp gleis)
        {
            if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.Gleis2EckenSignal;
                else return GleistypBezeichnung.Gleis2Ecken;
            }
            else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.Gleis1EckeSignal;
                else return GleistypBezeichnung.Gleis1Ecke;
            }
            else if (gleis.Bedingung[0] != 0)
            {
                if (!gleis.Signal.Equals("")) return GleistypBezeichnung.GleisSignal;
                else return GleistypBezeichnung.Gleis;
            }
            else if (!gleis.Weiche.Equals("") && gleis.Weiche_2nd.Equals("")) return GleistypBezeichnung.Weiche1Motor;
            else if (!gleis.Weiche.Equals("") && !gleis.Weiche_2nd.Equals("")) return GleistypBezeichnung.Weiche2Motoren;

            return GleistypBezeichnung.Error;
        }

        /// <summary>
        /// Gleisobjekt anhand des Namens suchen
        /// </summary>
        /// <param name="name">Name des Gleises was gesucht werden soll</param>
        /// <param name="Gleis">Output: Instanz des Gleises</param>
        /// <returns>wahr wenn Gleis gefunden wurde</returns>
        private bool GetGleisObjekt(string name, out Gleisplan.Abschnitt.GleisTyp Gleis)
        {
            foreach (Gleisplan.Abschnitt abschnitt in Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (gleis.Name.Equals(name))
                    {
                        Gleis = gleis;
                        return true;
                    }
                }
            }
            Gleis = null;
            return false;
        }

        /// <summary>
        /// Gleistypen
        /// </summary>
        enum GleistypBezeichnung
        {
            Gleis,
            GleisSignal,
            Gleis1Ecke,
            Gleis1EckeSignal,
            Gleis2Ecken,
            Gleis2EckenSignal,
            Weiche1Motor,
            Weiche2Motoren,
            Error
        }

        #region Weichensteuerung
        /// <summary>
        /// Schalten der Weiche bei Klicken auf das Symbol
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void Weiche_Click(object sender, EventArgs e)
        {
            if (!Systemzustand.Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    //Weiche nochmal neu schalten
                    WeichenListe.SetzeWeiche(weichenElement.Name, WeichenListe.GetWeiche(weichenElement.Name).Abzweig);
                }
                else
                {
                    WeichenListe.ToggleWeiche(weichenElement.Name);
                }
            }
        }
        /// <summary>
        /// Doppelkreuzungsweiche schalten
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void DKW_Click(object sender, EventArgs e)
        {
            if (!Systemzustand.Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                Gleisplan.Abschnitt.GleisTyp gleis = SucheGleis(weichenElement.Name);
                if (gleis != null)
                {
                    String[] tags = gleis.Typ.Split('_');
                    MouseEventArgs e2 = (MouseEventArgs)e;
                    switch (tags[1])
                    {
                        case "0":
                        case "45":
                            // Unterer Häfte ist 1. Weiche
                            if (e2.Y > (weichenElement.Height / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "90":
                        case "135":
                            // Linke Hälfte ist 1. Weiche
                            if (e2.X > (weichenElement.Width / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "180":
                        case "225":
                            // Obere Hälfte ist 1. Weiche
                            if (e2.Y <= (weichenElement.Height / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        case "270":
                        case "315":
                            // Rechte Hälfte ist 1. Weiche
                            if (e2.X <= (weichenElement.Width / 2))
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche);
                            }
                            else
                            {
                                WeichenListe.ToggleWeiche(gleis.Weiche_2nd);
                            }
                            break;
                        default: break;
                    }
                }
            }
        }
        /// <summary>
        /// Kreuzungsweiche schalten
        /// </summary>
        /// <param name="sender">Objekt, das diese Funktion ausführt</param>
        /// <param name="e">Argumente der Ausführung</param>
        private void KW_Click(object sender, EventArgs e)
        {
            if (!Systemzustand.Betriebsbereit) return;
            if (sender is PictureBox weichenElement)
            {
                Gleisplan.Abschnitt.GleisTyp gleis = SucheGleis(weichenElement.Name);
                if (gleis != null)
                {
                    MouseEventArgs e2 = (MouseEventArgs)e;
                    if (e2.X > 16)       //Auf rechte Hälfte der Weiche geklickt
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                        if (weiche == null) return;
                        if (weiche.Abzweig) WeichenListe.ToggleWeiche(gleis.Weiche);     //Nur Schalten wenn andere Zunge auf Abzweig
                    }
                    else                //Auf linke Hälfte der Weiche geklickt
                    {
                        Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                        if (weiche == null) return;
                        if (!weiche.Abzweig) WeichenListe.ToggleWeiche(gleis.Weiche_2nd);     //Nur Schalten wenn andere Zunge nicht auf Abzweig
                    }
                }
            }
        }
        #endregion

        #region SignalSteuerung
        /// <summary>
        /// Button Click: Signal schalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Signal_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox PicBox)
            {
                bool Modus = Config.ReadConfig("AutoSignalFahrstrasse").Equals("true");
                if (GetGleisObjekt(PicBox.Name, out Gleisplan.Abschnitt.GleisTyp Gleis))
                {
                    Signal signal = SignalListe.GetSignal(Gleis.Signal);
                    // SHIFT-Taste während des Klickens gedrückt -> Schalten auf HP2 (langsame Fahrt)
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        if (signal.Zustand == SignalZustand.HP2) // Signal bereits auf diesem Zustand  -> auf HP0 schalten
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if (Einstellungen.AutoSignal) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }

                        // HP2 erlaubt
                        if (signal.StellungErlaubt(SignalZustand.HP2, Modus))
                        {
                            signal.Schalten(SignalZustand.HP2);
                            signal.AutoSperre = false; //Signal wieder im Normalen Modus
                            return;
                        }
                        // HP2 nicht erlaubt, prüfen ob HP1 möglich
                        else if (signal.StellungErlaubt(SignalZustand.HP1, Modus))
                        {
                            signal.Schalten(SignalZustand.HP1);
                            signal.AutoSperre = false; //Signal wieder im Normalen Modus
                            return;
                        }
                        else // Weder HP2 noch HP1 erlaubt -> Strecke gesperrt
                        {
                            // Signal auf Rot-Schalten, wenn nicht bereits in diesem Zustand
                            if (signal.Zustand != SignalZustand.HP0)
                                signal.Schalten(SignalZustand.HP0);
                            return;
                        }
                    }
                    // CTRL-Taste während des Klickens gedrückt -> Schalten auf SH1 (Rangier Fahrt)
                    else if (Control.ModifierKeys == Keys.Control)
                    {
                        if (signal.Zustand == SignalZustand.SH1) // Signal bereits auf diesem Zustand  -> auf HP0 schalten
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if (Einstellungen.AutoSignal) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }
                        // SH1 erlaubt
                        if (signal.StellungErlaubt(SignalZustand.SH1, Modus))
                        {
                            signal.Schalten(SignalZustand.SH1);
                            return;
                        }
                        else //Nicht erlaubt -> Strecke gesperrt
                        {
                            // Signal auf Rot-Schalten, wenn nicht bereits in diesem Zustand
                            if (signal.Zustand != SignalZustand.HP0)
                                signal.Schalten(SignalZustand.HP0);
                            return;
                        }
                    }
                    else
                    {
                        // Signal is auf Rot -> Schalten in ein anderes 
                        if (signal.Zustand == SignalZustand.HP0)
                        {
                            if (signal.StellungErlaubt(SignalZustand.HP1, Modus))
                            {
                                signal.Schalten(SignalZustand.HP1);
                                signal.AutoSperre = false; //Signal wieder im Normalen Modus
                                return;
                            }
                            else if (signal.StellungErlaubt(SignalZustand.HP2, Modus))
                            {
                                signal.Schalten(SignalZustand.HP2);
                                signal.AutoSperre = false; //Signal wieder im Normalen Modus
                                return;
                            }
                            else // Weder HP2 noch HP1 erlaubt -> Strecke gesperrt
                            {
                                return; // Schalten nicht erlauben
                            }
                        }
                        else // Zurückschalten auf HP0
                        {
                            signal.Schalten(SignalZustand.HP0);
                            if (Einstellungen.AutoSignal) signal.AutoSperre = true; //Signal nicht wieder auf grün schalten lassen
                            return;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Button Click: SH2 Signal schalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SperrungSh2_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked == true)
                {
                    checkBox.Image = MEKB_H0_Anlage.Properties.Resources.SH_2;
                }
                else
                {
                    checkBox.Image = MEKB_H0_Anlage.Properties.Resources.SH_2_inaktiv;
                }
                UpdateFahrstrassenSchalter(1);
            }
        }
        #endregion

        #region Fahrstrassen

        /// <summary>
        /// Generiere Statuskonstrukt aus Fahrstraßen und Belegtmeldung
        /// </summary>
        /// <param name="besetzt">Belegtstatus des Abschnitts</param>
        /// <param name="Fahrstrassen_west">Fahrstraßen, die diesen Abschnitt nach westen(links) belegen </param>
        /// <param name="Fahrstrassen_ost">Fahrstraßen, die diesen Abschnitt nach osten(rechts) belegen</param>
        /// <returns></returns>
        private MeldeZustand ErrechneZustand(bool besetzt, List<Fahrstrasse> Fahrstrassen_west, List<Fahrstrasse> Fahrstrassen_ost)
        {
            //Zählvariablen für die Fahrstraßen
            int aktiv_west = 0; //Anzahl aktive Fahrstraßen nach westen
            int aktiv_ost = 0; //Anzahl aktive Fahrstraßen nach osten
            int safe_west = 0; //Anzahl sichere Fahrstraßen nach westen
            int safe_ost = 0; //Anzahl sichere Fahrstraßen nach osten

            //Zwischenvariablen Zustände
            bool richtung = false;
            bool fahrstrasseAktiv = false;
            bool sicher = false;

            //Zählen der Fahrstraßen
            foreach (Fahrstrasse fahrstrasse in Fahrstrassen_west)
            {
                if (fahrstrasse.GetAktivStatus()) aktiv_west++;
                if (fahrstrasse.Safe) safe_west++;
            }
            foreach (Fahrstrasse fahrstrasse in Fahrstrassen_ost)
            {
                if (fahrstrasse.GetAktivStatus()) aktiv_ost++;
                if (fahrstrasse.Safe) safe_ost++;
            }

            //Mehr als eine Fahrstraße aktiv -> Fehler
            if (aktiv_west > 1) return new MeldeZustand(false);
            if (aktiv_ost > 1) return new MeldeZustand(false);
            if (safe_west > 1) return new MeldeZustand(false);
            if (safe_ost > 1) return new MeldeZustand(false);
            if ((aktiv_west == 1) && (aktiv_ost == 1)) return new MeldeZustand(false);

            //Fahrstraße nach westen aktiv
            if (aktiv_west == 1)
            {
                richtung = true; //In Fahrtrichtung
                fahrstrasseAktiv = true;
                if (safe_west == 1) sicher = true; //Fahrstraße sicher?
            }
            //Fahrstraße nach osten aktiv
            if (aktiv_ost == 1)
            {
                richtung = false;  //Gegenfahrtrichtung
                fahrstrasseAktiv = true;
                if (safe_ost == 1) sicher = true; //Fahrstraße sicher?
            }

            //Generiere Statuskonstrukt
            return new MeldeZustand(besetzt, fahrstrasseAktiv, sicher, richtung);
        }


        /// <summary>
        /// Fahrstraße aktivieren/deaktivieren
        /// </summary>
        /// <param name="fahrstrasse">Fahrstraße zum Schalten</param>
        private void ToggleFahrstrasse(Fahrstrasse fahrstrasse)
        {
            //Fahrstraße gesetzt
            if (fahrstrasse.GetGesetztStatus())
            {
                //Fahrstraße deaktivieren
                fahrstrasse.DeleteFahrstrasse(WeichenListe.Liste);
            }
            else
            {
                //Fahrstraße aktivieren
                if (Systemzustand.Betriebsbereit) fahrstrasse.StarteFahrstrasse();
            }
            //Alle Fahrstraßen/Buttons aktualisieren
            UpdateFahrstrassenSchalter(1);
        }

        /// <summary>
        /// Button Click: Fahrstrasse an und ausschalten
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        public void FahrstrassenButton_Click(object sender, EventArgs e)
        {
            //Prüfen ob Funktion von einem Button ausgelöst wurde
            if (sender is Button button)
            {
                //An den Button gebundene Fahrstrasse auslesen
                if (button.Tag == null) return;
                string ZielFahrstrasse = button.Tag.ToString();

                //Fahrstrasse aus der Liste finden mit Namen
                Fahrstrasse fahrstrasse = FahrstrassenListe.GetFahrstrasse(ZielFahrstrasse);

                //Keine Fahrstraße gefunden -> Funktion abbrechen
                if (fahrstrasse == null) return;

                if (fahrstrasse.Fahrstr_GleicherEingang.Count >= 1)
                {
                    //Ist eine der Fahrstrassen mit gleichen Ausgang bereits aktiv?
                    if (FahrstrassenListe.FahrstrasseGleicheGesetzt(fahrstrasse.Name))
                    {
                        foreach (string GruppenItem in fahrstrasse.Fahrstr_GleicherEingang)
                        {
                            Fahrstrasse GruppenFahrstrasse = FahrstrassenListe.GetFahrstrasse(GruppenItem);
                            if (GruppenFahrstrasse.GetGesetztStatus())
                            {
                                ToggleFahrstrasse(GruppenFahrstrasse);
                            }
                        }
                    }
                    //Keine der Fahrstrassen mit gleichen Ausgang aktiv
                    else
                    {
                        foreach(MenuItem menuItem in button.ContextMenu.MenuItems)
                        {
                            if(FahrstrassenListe.FahrstrasseBlockiert(menuItem.Tag.ToString()))
                            {
                                menuItem.Enabled = false;
                            }
                            else
                            {
                                menuItem.Enabled= true;
                            }
                        }

                        button.ContextMenu.Show(button, new Point(16, 8));
                    }
                }
                else //Normale Fahrstrasse ohne gleiche Eingaenge
                {
                    //Fahrstrasse aktiv?
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        ToggleFahrstrasse(fahrstrasse);  //Aktiv? auschalten
                    }
                    else
                    {
                        //Kontrollieren 
                        if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                        {
                            //Keine Sperrende Fahstraße aktiv
                            ToggleFahrstrasse(fahrstrasse); //Fahrstrasse aktivieren
                        }
                    }
                }
            }
            else if (sender is MenuItem menuButton)
            {
                //An den Button gebundene Fahrstrasse auslesen
                if (menuButton.Tag == null) return;
                string ZielFahrstrasse = menuButton.Tag.ToString();

                //Fahrstrasse aus der Liste finden mit Namen
                Fahrstrasse fahrstrasse = FahrstrassenListe.GetFahrstrasse(ZielFahrstrasse);

                //Keine Fahrstraße gefunden -> Funktion abbrechen
                if (fahrstrasse == null) return;

                //Fahrstrasse aktiv?
                if (fahrstrasse.GetGesetztStatus())
                {
                    ToggleFahrstrasse(fahrstrasse);  //Aktiv? auschalten
                }
                else
                {
                    //Kontrollieren 
                    if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                    {
                        //Keine Sperrende Fahstraße aktiv
                        ToggleFahrstrasse(fahrstrasse); //Fahrstrasse aktivieren
                    }
                }

            }
        }
        
        /// <summary>
        /// Alle Fahrstrassen-Buttons aktualisieren. (Deaktivieren der Buttons bei gesperrten Fahrstrassen)
        /// </summary>
        /// <param name="dummy"></param>
        private void UpdateFahrstrassenSchalter(int dummy)
        {
            UpdateSperrungen();
            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                var Fund = Controls.Find(fahrstrasse.Name + "_Button", true);
                foreach (Control control in Fund)
                {
                    if (control is Button button)
                    {
                        if (FahrstrassenListe.FahrstrasseAlleGleicheBlockiert(fahrstrasse))
                        {
                            if (button.Enabled == true)
                            {
                                button.Enabled = false;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben_deakt;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten_deakt;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else
                                {
                                    break;
                                }
                                if (fahrstrasse.EinfahrtsSignal.Zustand != SignalZustand.HP0) fahrstrasse.EinfahrtsSignal.Schalten(SignalZustand.HP0);
                            }
                        }
                        else
                        {
                            if (button.Enabled == false)
                            {
                                button.Enabled = true;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else { }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fahrstrassensperrung anhand von SH2 und Belegtmeldung errechnen
        /// </summary>
        private void UpdateSperrungen()
        {
            List<string> Aenderungen = new List<string>();
            foreach (string ButtonName in SperrButtons)
            {
                var Fund = Controls.Find(ButtonName, true);
                foreach (Control control in Fund)
                {
                    if (control is CheckBox checkBox)
                    {
                        if (checkBox.Checked)
                        {
                            Aenderungen.AddRange(checkBox.Tag.ToString().Split('+'));
                        }
                    }
                }
            }

            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                if (fahrstrasse.IstFahrstrasseBelegt(BelegtmelderListe.Liste))
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = true; // Fahrstrasse ist gesperrt
                }
                else if (Aenderungen.Contains(fahrstrasse.Name))
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = true; // Fahrstrasse ist gesperrt
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        //Fahrstraße deaktivieren wenn gesetzt
                        fahrstrasse.DeleteFahrstrasse(WeichenListe.Liste);
                    }
                }
                else
                {
                    FahrstrassenListe.GesperrteFahrstrassen[fahrstrasse.Name] = false; // Fahrstrasse ist nicht (oder nicht mehr) gesperrt
                }
            }

        }
        #endregion
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
                    SperrButton = false;
                    GesperrteFahrstrassen = new List<string>();
                }

                public int PosX { get; set; }
                public int PosY { get; set; }
                public string Name { get; set; }
                public string Typ { get; set; }

                public Label Gleislabel { get; set; }

                public string Weiche { get; set; }
                public string Weiche_2nd { get; set; }
                public string WeichenBelegtmelder { get; set; }

                public string FahrstrassenButton { get; set; }
                public bool ButtonDrehen { get; set; }

                public bool SperrButton { get; set; }
                public List<string> GesperrteFahrstrassen { get; set; }

                public string Signal { get; set; }
                public SignalZustand SignalZustand { get; set; }

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
