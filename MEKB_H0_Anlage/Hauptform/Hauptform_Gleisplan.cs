using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Hauptform
    /// </summary>
    public partial class Hauptform : Form
    {
        #region Gleisplan Zeichnen
        /// <summary>
        /// Initialen Gleisplan zeichnen. Anschließend nur noch funktion GleisplanZeichen() für updates verwenden
        /// </summary>
        private void GleisplanZeichnenInitial()
        {
            foreach(Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                foreach(Gleisplan.Abschnitt.Bilder bild in abschnitt.BilderListe)
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
                        this.GleisplanAnzeige.Controls.Add(neuesBild);
                    }                   
                }

                foreach(Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (!this.Controls.ContainsKey(gleis.Name))
                    {
                        PictureBox neuesGleis = new PictureBox()
                        {
                            Name = gleis.Name,
                            Tag = gleis.Typ,
                            Size = new Size(32, 32),
                            Location = new Point(gleis.PosX*32, gleis.PosY*32),
                            Image = new Bitmap(GleisbildZeichnung.Katalog[36][7]),
                        };
                        if (!gleis.Weiche.Equals(""))
                        {
                            if (gleis.Typ.StartsWith("DKW")) neuesGleis.Click += new System.EventHandler(this.DKW_Click);
                            else if (gleis.Typ.StartsWith("KW")) neuesGleis.Click += new System.EventHandler(this.KW_Click);
                            //else if (gleis.Typ.StartsWith("Dreiweg")) neuesGleis.Click += new System.EventHandler(this.Dreiweg_Click);
                            else neuesGleis.Click += new System.EventHandler(this.Weiche_Click);
                        }
                        else if(!gleis.Signal.Equals(""))
                        {
                            neuesGleis.Click += new System.EventHandler(this.Signal_Click);
                        }

                        this.GleisplanAnzeige.Controls.Add(neuesGleis);

                        if(gleis.Gleislabel != null)
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
                            this.GleisplanAnzeige.Controls.Add(neuesLabel);
                            neuesLabel.BringToFront();
                        }

                        if(!gleis.FahrstrassenButton.Equals(""))
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
                            if (Buttonzeichen)
                            {
                                this.GleisplanAnzeige.Controls.Add(button);
                                button.BringToFront();
                            }
                        }

                        if(gleis.SperrButton)
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
                            this.GleisplanAnzeige.Controls.Add(SHO_Sperrung);
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
                        else if(!gleis.Weiche.Equals(""))
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
        /// Subfunktion: Winkel des Buttons an hand des Gleistyps berechnen
        /// </summary>
        /// <param name="GleisTyp">Gleistyp</param>
        /// <param name="drehen">Auf welcher Seite des Gleises ist der Button</param>
        /// <returns>Winkel des Buttons oder "nicht erlaubt" wenn unzulässig</returns>
        private string GetButtonWinkel(string GleisTyp, bool drehen)
        {
            string Typ = GleisTyp.Split('+').First();
            string Winkel = Typ.Split('_').Last();
            Typ = Typ.Split('_').First();

            if(Typ.Equals("KurveL") && drehen)
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
        /// Alle Gleise zeichnen. Inklusive abfrage ob neuzeichnen notwendig ist
        /// </summary>
        private void GleisplanZeichnen()
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                Dictionary<int, MeldeZustand> aktZustand = new Dictionary<int, MeldeZustand>();
                foreach(Gleisplan.Abschnitt.StatusBedingung bedingung in abschnitt.Bedingungen)
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
                    if(BelegtmelderStatus) BelegtmelderStatus = BelegtmelderListe.GetBelegtStatus(bedingung.Belegtmelder);

                    aktZustand.Add(bedingung.Nummer, ErrechneZustand(
                            BelegtmelderStatus,
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenMit.ToArray()),
                            FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenGegen.ToArray())));
                }



                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    bool update = false;
                    for(int i = 0; i <3; i++)
                    {
                        if (gleis.Bedingung[i] == 0) continue; // Keine Bedingung
                        if(!gleis.Zustand[i].Equals(aktZustand[gleis.Bedingung[i]])) update = true;
                    }

                    if (update)
                    {
                        var Fund = this.GleisplanAnzeige.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(
                                        aktZustand[gleis.Bedingung[0]],
                                        aktZustand[gleis.Bedingung[1]],
                                        aktZustand[gleis.Bedingung[2]],
                                        Picbox);
                                }
                                else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(
                                        aktZustand[gleis.Bedingung[0]],
                                        aktZustand[gleis.Bedingung[1]],
                                        Picbox);
                                }
                                else if (gleis.Bedingung[0] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(
                                        aktZustand[gleis.Bedingung[0]],
                                        Picbox);
                                }
                                else if (!gleis.Weiche.Equals(""))
                                {
                                    Weiche weiche = WeichenListe.GetWeiche(gleis.Weiche);
                                    if (gleis.Weiche_2nd.Equals("")) // Normale Weiche ohne zweiten Motor
                                    {
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
                                    }
                                    else
                                    {
                                        Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                        if (!gleis.WeichenBelegtmelder.Equals(""))
                                        {
                                            if (int.TryParse(gleis.WeichenBelegtmelder, out int index))
                                            {
                                                weiche.Besetzt = aktZustand[index].Besetzt;
                                                weiche2.Besetzt = aktZustand[index].Besetzt;
                                            }
                                        }

                                        GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, Picbox);
                                    }
                                }
                                if (!gleis.Signal.Equals(""))
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(SignalListe.GetSignal(gleis.Signal), Picbox);
                                }

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
                    else if(!gleis.Weiche.Equals("") && !gleis.Weiche_2nd.Equals(""))
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
        /// Aktuellen Zustand der Weiche im Gleisplan zeichnen
        /// </summary>
        /// <param name="weiche">Weiche die neu gezeichnet werden soll</param>
        private void GleisplanUpdateWeiche(Weiche weiche)
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if(gleis.Weiche.Equals(weiche.Name))
                    {
                        var Fund = this.GleisplanAnzeige.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                if (gleis.Weiche_2nd.Equals("")) //Kein Zweiter Weichenmotor
                                {
                                    if (gleis.Bedingung[1] == 0)
                                    {
                                        GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox);
                                    }
                                    else if (gleis.Bedingung[2] == 0)
                                    {
                                        Dictionary<int, MeldeZustand> aktZustand = new Dictionary<int, MeldeZustand>();
                                        foreach (Gleisplan.Abschnitt.StatusBedingung bedingung in abschnitt.Bedingungen)
                                        {
                                            if (bedingung.Nummer == gleis.Bedingung[1])
                                            {
                                                bool BelegtmelderStatus = false;
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
                                                // Wenn Belegtmelderstatus gewünscht ist
                                                if (BelegtmelderStatus) BelegtmelderStatus = BelegtmelderListe.GetBelegtStatus(bedingung.Belegtmelder);

                                                aktZustand.Add(bedingung.Nummer, ErrechneZustand(
                                                        BelegtmelderStatus,
                                                        FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenMit.ToArray()),
                                                        FahrstrassenListe.GetFahrstrasse(bedingung.FahrstrassenGegen.ToArray())));
                                            }
                                        }
                                        GleisbildZeichnung.ZeichneSchaltbild(weiche, aktZustand[gleis.Bedingung[1]], Picbox, true);
                                    }
                                }
                                else // Zweiter Weichenmotor
                                {
                                    Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche_2nd);
                                    GleisbildZeichnung.ZeichneSchaltbild(weiche, weiche2, Picbox, true);
                                }
                            }
                        }                      
                    }
                    else if(gleis.Weiche_2nd.Equals(weiche.Name))
                    {
                        var Fund = this.GleisplanAnzeige.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                Weiche weiche2 = WeichenListe.GetWeiche(gleis.Weiche);
                                GleisbildZeichnung.ZeichneSchaltbild(weiche2, weiche, Picbox, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Aktuellen Zustand des Signals im Gleisplan zeichnen
        /// </summary>
        /// <param name="signal">Signal das neu gezeichnet werden soll</param>
        private void GleisplanUpdateSignal(Signal signal)
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
            {
                foreach (Gleisplan.Abschnitt.GleisTyp gleis in abschnitt.Gleise)
                {
                    if (gleis.Signal.Equals(signal.Name))
                    {
                        var Fund = this.GleisplanAnzeige.Controls.Find(gleis.Name, true);
                        foreach (Control control in Fund)
                        {
                            if (control is PictureBox Picbox)
                            {
                                if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0 && gleis.Bedingung[2] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(gleis.Zustand[0], gleis.Zustand[1], gleis.Zustand[2], Picbox, true);
                                }
                                else if (gleis.Bedingung[0] != 0 && gleis.Bedingung[1] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(gleis.Zustand[0], gleis.Zustand[1], Picbox, true);
                                }
                                else if (gleis.Bedingung[0] != 0)
                                {
                                    GleisbildZeichnung.ZeichneSchaltbild(gleis.Zustand[0], Picbox, true);
                                }
                                GleisbildZeichnung.ZeichneSchaltbild(signal, Picbox);    
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gleisobjekt anhand des Namens suchen
        /// </summary>
        /// <param name="name">Name des Gleises was gesucht werden soll</param>
        /// <param name="Gleis">Output: Instanz des Gleises</param>
        /// <returns>wahr wenn Gleis gefunden wurde</returns>
        private bool GetGleisObjekt(string name, out Gleisplan.Abschnitt.GleisTyp Gleis)
        {
            foreach (Gleisplan.Abschnitt abschnitt in Plan.Abschnitte)
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

        MeldeZustand FreiesGleis = new MeldeZustand(false);
        #endregion
        #region Fahrstraßen bestimmen
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
        /// Weichenliste der Fahrstraße durchlaufen und mit aktueller Weichenstellung vergleichen
        /// </summary>
        /// <param name="fahrstrasse">Fahrstraße zum überprüfen</param>
        private void Fahrstrassenupdate(Fahrstrasse fahrstrasse)
        {
            if (fahrstrasse.GetGesetztStatus())    //Fahrstraße wurde gesetzt
            {
                fahrstrasse.SetFahrstrasseRichtung();
                //Prüfen ob alle Weichen der Fahrstraßen richtig geschaltet sind
                if (fahrstrasse.CheckFahrstrassePos() == false) //Noch nicht alle Weichen gestellt
                {
                    if (Betriebsbereit) fahrstrasse.SetFahrstrasse(z21Start);
                }
                else //Alle Weichen in richtiger Stellung
                {
                    //Fahrstraße als aktiviert kennzeichnen
                    fahrstrasse.AktiviereFahrstasse();
                    //Jede Weiche in der Fahrstraßenliste durchlaufen
                    foreach (Weiche Fahrstrassenweiche in fahrstrasse.Fahrstr_Weichenliste)
                    {
                        Weiche weiche = WeichenListe.GetWeiche(Fahrstrassenweiche.Name);   //Weiche in Globale Liste suchen
                        if (weiche == null) return;   //Weichen nicht gefunden - Funktion abbrechen
                        GleisplanUpdateWeiche(weiche);  //Weichenbild aktualisieren
                    }

                    //Weichen zyklisch nochmal schalten um hängenbleiben zu vermeiden
                    if (Betriebsbereit) fahrstrasse.ControlSetFahrstrasse(z21Start);
                }
            }
           
        }
        #endregion
    }
}
