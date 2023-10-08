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
        //public Dictionary<string, int> GleisListenVerzeichnis = new Dictionary<string, int>();
        //public List<PictureBox> GleisListe = new List<PictureBox>();
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

                        if(gleis.label != null)
                        {
                            Label neuesLabel = new Label
                            {
                                AutoSize = true,
                                Text = gleis.label.Text,
                                Location = new Point(gleis.PosX * 32 + gleis.label.X_Offeset, gleis.PosY * 32 + gleis.label.Y_Offeset)
                            };
                            if (gleis.label.Fett) neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.label.Groesse, FontStyle.Bold);
                            else neuesLabel.Font = new Font("Microsoft Sans Serif", gleis.label.Groesse, FontStyle.Regular);
                            neuesLabel.Name = gleis.Name + "Label";
                            if (gleis.label.Rahmen) neuesLabel.BorderStyle = BorderStyle.FixedSingle;
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
                                    if (gleis.Weiche_2nd.Equals("")) // Normale Weiche ohen zweiten Motor
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
                    
                    if (!gleis.Weiche.Equals(""))
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
                                GleisbildZeichnung.ZeichneSchaltbild(weiche, Picbox, true);
                            }
                        }
                    }                   
                }  
            }
        }

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
        
        
        
       
       

        #region Schattenbahnhof

        private void UpdateGleisbild_Schatten_Eingl(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_6);
        }
        private void UpdateGleisbild_Schatten_Eingl_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenEingl_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl1(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_6);
        }
        private void UpdateGleisbild_Schatten_Gl1_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl1_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl2(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_6);
        }
        private void UpdateGleisbild_Schatten_Gl2_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl2_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl3(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_6);
        }
        private void UpdateGleisbild_Schatten_Gl3_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl3_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl4(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_6);
        }
        private void UpdateGleisbild_Schatten_Gl4_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl4_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl5(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_6);
        }
        private void UpdateGleisbild_Schatten_Gl5_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl5_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl6(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_6);
        }
        private void UpdateGleisbild_Schatten_Gl6_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl6_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl7(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_6);
        }
        private void UpdateGleisbild_Schatten_Gl7_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl7_Halt_1);
        }

        #endregion


       
        #region Block 3
        
        private void UpdateGleisbild_Block3(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_5);
        }
        #endregion
        #region Block 4
        private void UpdateGleisbild_Block4(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_5);
        }
        
        #endregion


        
       
       

        

       
        

        #region Weichen Gleisfeldumgebung
       
       
       


       
       
       
        private void UpdateGleisbild_Weiche27()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche27");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche27.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche27);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche27_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche27_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche28()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche28");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche28.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche28);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche28_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche28_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche29()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche29");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche29.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche29);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche29_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche29_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche30()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche30");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche30.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche30);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche30_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche30_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche50()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche50");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche50.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche50);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche50a_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche50a_2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche50b_1);              
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche50a_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche50a_2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche50b_1);
            }
        }

        private void UpdateGleisbild_Tunnel1_Einf(bool besetzt, List<Fahrstrasse> Fahrstrasse_mit)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_mit, new List<Fahrstrasse>());
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel1_Einf);
        }
        private void UpdateGleisbild_Tunnel2_Einf(bool besetzt, List<Fahrstrasse> Fahrstrasse_mit)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_mit, new List<Fahrstrasse>());
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel2_Einf);
        }
        private void UpdateGleisbild_Tunnel1_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_mit)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_mit, new List<Fahrstrasse>());
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel1_Halt);
        }
        private void UpdateGleisbild_Tunnel2_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_mit)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_mit, new List<Fahrstrasse>());
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel2_Halt);
        }

        private void UpdateGleisbild_Tunnel(bool besetzt_Tunnel1, List<Fahrstrasse> Fahrstrasse_Tunnel1,
                                            bool besetzt_Tunnel2, List<Fahrstrasse> Fahrstrasse_Tunnel2)
        {
            MeldeZustand zustand_Tunnel1 = ErrechneZustand(besetzt_Tunnel1, Fahrstrasse_Tunnel1, new List<Fahrstrasse>());
            MeldeZustand zustand_Tunnel2 = ErrechneZustand(besetzt_Tunnel2, Fahrstrasse_Tunnel2, new List<Fahrstrasse>());

            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_9);

            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, zustand_Tunnel1, Tunnel2_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, zustand_Tunnel1, Tunnel2_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_5);
        }


        private void UpdateGleisbild_Weiche51()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche51");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche51.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche51);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51a_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51a_2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51a_3);

                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche51b_1);

            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51b_2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche51b_1);
            }
        }
       
       

        private void UpdateGleisbild_Weiche61()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche61");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche61.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche61);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche61_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche61_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche61_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche61_2);
            }
        }
        private void UpdateGleisbild_Weiche62()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche62");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche62.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche62);
            if (!weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche62_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche62_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche62_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche62_2);
            }
        }
        private void UpdateGleisbild_Weiche63()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche63");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche63.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche63);
            if (!weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche63_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche63_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche63_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche63_2);
            }
        }
        private void UpdateGleisbild_Weiche64()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche64");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche64.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche64);
            if (!weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche64_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche64_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche64_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche64_2);
            }
        }
        private void UpdateGleisbild_Weiche65()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche65");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche65.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche65);
            if (!weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche65_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche65_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche65_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche65_2);
            }
        }
        private void UpdateGleisbild_Weiche66()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche66");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche66.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche66);
            if (!weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche66_1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, Weiche66_1);
            }
        }
        private void UpdateGleisbild_Weiche67_68()
        {

            Weiche weiche67 = WeichenListe.GetWeiche("Weiche67");
            if (weiche67 == null) return;
            Weiche weiche68 = WeichenListe.GetWeiche("Weiche68");
            if (weiche68 == null) return;

            MeldeZustand meldeZustand67 = new MeldeZustand(weiche67, Weiche67.Tag.ToString().EndsWith("_Gegen"));
            MeldeZustand meldeZustand68 = new MeldeZustand(weiche68, Weiche68.Tag.ToString().EndsWith("_Gegen"));

            GleisbildZeichnung.ZeichneSchaltbild(weiche67, Weiche67);
            GleisbildZeichnung.ZeichneSchaltbild(weiche68, Weiche68);

            if ((weiche67.Abzweig == false) && (weiche68.Abzweig == false))
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand67, FreiesGleis, FreiesGleis, Weiche68_67);
            else if ((weiche67.Abzweig == true) && (weiche68.Abzweig == false))
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, meldeZustand67, Weiche68_67);
            else if ((weiche67.Abzweig == false) && (weiche68.Abzweig == true))
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand68, FreiesGleis, Weiche68_67);
            else
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand68, meldeZustand67, Weiche68_67);
            if (weiche68.Abzweig == true)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand68, Schatten0_Ausf1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand68, Schatten0_Ausf2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand68, Schatten0_Ausf3);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand68, Schatten0_Ausf4);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf4);
            }

        }
        private void UpdateGleisbild_Weiche70()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche70");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche70.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche70);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche70_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche70_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche70_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche70_2);
            }
        }
        private void UpdateGleisbild_Weiche71()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche71");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche71.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche71);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche71_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche71_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche71_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche71_2);
            }
        }
        private void UpdateGleisbild_Weiche72()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche72");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche72.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche72);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche72_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche72_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche72_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche72_2);
            }
        }
        private void UpdateGleisbild_Weiche73()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche73");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche73.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche73);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche73_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche73_2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche73_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche73_2);
            }
        }


        #endregion
       
        #endregion

        
        /// <summary>
        /// Weichenliste der Fahrstraße durchlaufen und mit aktueller Weichenstellung vergleichen
        /// </summary>
        /// <param name="fahrstrasse">Fahrstraße zum überprüfen</param>
        private void Fahrstrassenupdate(Fahrstrasse fahrstrasse)
        {
            bool SafeStatusAlt = fahrstrasse.Safe;
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
                        UpdateWeicheImGleisplan(weiche);  //Weichenbild aktualisieren
                    }

                    //Weichen zyklisch nochmal schalten um hängenbleiben zu vermeiden
                    if (Betriebsbereit) fahrstrasse.ControlSetFahrstrasse(z21Start);
                }
            }
            // Wechseln auf den sicheren Zustand
            if ((SafeStatusAlt == false) && (fahrstrasse.Safe == true))
            {
                // Signal nur Schalten wenn die Option Autoschaltung über Fahrstraße aktiv ist
                if (fahrstrasse.EinfahrtsSignal.Name != null)
                {
                    if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) AutoSignalUpdate(fahrstrasse.EinfahrtsSignal.Name);
                }
                
            }
        }
        /// <summary>
        /// Fahrstraßen im Bild zeichnen
        /// </summary>
        private void FahrstrasseBildUpdate()
        {
            UpdateWeichenBelegung();

            
            //Schattenbahnhof
            UpdateGleisbild_Schatten_Eingl(BelegtmelderListe.GetBelegtStatus("Eingleisen"));
            UpdateGleisbild_Schatten_Eingl_Halt(BelegtmelderListe.GetBelegtStatus("Eingleisen_Halt"));

            UpdateGleisbild_Schatten_Gl1(BelegtmelderListe.GetBelegtStatus("Schatten_Gl1"));
            UpdateGleisbild_Schatten_Gl1_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl1_Halt"));

            UpdateGleisbild_Schatten_Gl2(BelegtmelderListe.GetBelegtStatus("Schatten_Gl2"));
            UpdateGleisbild_Schatten_Gl2_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl2_Halt"));

            UpdateGleisbild_Schatten_Gl3(BelegtmelderListe.GetBelegtStatus("Schatten_Gl3"));
            UpdateGleisbild_Schatten_Gl3_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl3_Halt"));

            UpdateGleisbild_Schatten_Gl4(BelegtmelderListe.GetBelegtStatus("Schatten_Gl4"));
            UpdateGleisbild_Schatten_Gl4_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl4_Halt"));

            UpdateGleisbild_Schatten_Gl5(BelegtmelderListe.GetBelegtStatus("Schatten_Gl5"));
            UpdateGleisbild_Schatten_Gl5_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl5_Halt"));

            UpdateGleisbild_Schatten_Gl6(BelegtmelderListe.GetBelegtStatus("Schatten_Gl6"));
            UpdateGleisbild_Schatten_Gl6_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl6_Halt"));

            UpdateGleisbild_Schatten_Gl7(BelegtmelderListe.GetBelegtStatus("Schatten_Gl7"));
            UpdateGleisbild_Schatten_Gl7_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl7_Halt"));

           
            //Tunnel
            UpdateGleisbild_Tunnel1_Einf(
                BelegtmelderListe.GetBelegtStatus("Tunnel1_Einfahrt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen" }));
            UpdateGleisbild_Tunnel2_Einf(
                BelegtmelderListe.GetBelegtStatus("Tunnel2_Einfahrt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen" }));

            UpdateGleisbild_Tunnel(BelegtmelderListe.GetBelegtStatus("Tunnel1"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen" }),
                BelegtmelderListe.GetBelegtStatus("Tunnel2"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen" }));

            UpdateGleisbild_Tunnel1_Halt(
                BelegtmelderListe.GetBelegtStatus("Tunnel1_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen" }));
            
            UpdateGleisbild_Tunnel2_Halt(
                BelegtmelderListe.GetBelegtStatus("Tunnel2_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen" }));
            
            //Gleise im Block 3 aktualisieren
           UpdateGleisbild_Block3(BelegtmelderListe.GetBelegtStatus("Block3"), //Besetzt                             
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen", "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen"}),
                                    new List<Fahrstrasse>());
            //Gleise im Block 4 aktualisieren                        
            UpdateGleisbild_Block4(BelegtmelderListe.GetBelegtStatus("Block4"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis1", "TunnelAussen_nach_Gleis2", "TunnelAussen_nach_Gleis3", "TunnelAussen_nach_Gleis4", "TunnelAussen_nach_Gleis5", "TunnelAussen_nach_Gleis6", "TunnelInnen_nach_Gleis1", "TunnelInnen_nach_Gleis2", "TunnelInnen_nach_Gleis3", "TunnelInnen_nach_Gleis4", "TunnelInnen_nach_Gleis5", "TunnelInnen_nach_Gleis6" }),
                                    new List<Fahrstrasse>());     //nie nach rechts
           UpdateGleisbild_Weiche27();     //Umfeld um Weiche 27
            UpdateGleisbild_Weiche28();     //Umfeld um Weiche 28
            UpdateGleisbild_Weiche29();     //Umfeld um Weiche 29
            UpdateGleisbild_Weiche30();     //Umfeld um Weiche 30
            UpdateGleisbild_Weiche50();     //Umfeld um Weiche 50
            UpdateGleisbild_Weiche51();     //Umfeld um Weiche 51


            UpdateGleisbild_Weiche61();     //Umfeld um Weiche 61
            UpdateGleisbild_Weiche62();     //Umfeld um Weiche 62
            UpdateGleisbild_Weiche63();     //Umfeld um Weiche 63
            UpdateGleisbild_Weiche64();     //Umfeld um Weiche 64
            UpdateGleisbild_Weiche65();     //Umfeld um Weiche 65
            UpdateGleisbild_Weiche66();     //Umfeld um Weiche 66

            UpdateGleisbild_Weiche67_68();     //Umfeld um Weiche 67_68

            UpdateGleisbild_Weiche70();     //Umfeld um Weiche 70
            UpdateGleisbild_Weiche71();     //Umfeld um Weiche 71
            UpdateGleisbild_Weiche72();     //Umfeld um Weiche 72
            UpdateGleisbild_Weiche73();     //Umfeld um Weiche 73      
        }

        void UpdateWeichenBelegung()
        {
            foreach (Belegtmelder belegtmelder in BelegtmelderListe.Liste)
            {
                switch (belegtmelder.Name)
                {
                    case "Block2": SetzeWeichenBelegung("Weiche53", belegtmelder.IstBelegt()); break;
                    case "W60": SetzeWeichenBelegung("Weiche60", belegtmelder.IstBelegt()); break;
                    case "W1_W4": 
                        SetzeWeichenBelegung("Weiche1", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("Weiche4", belegtmelder.IstBelegt()); break;
                    case "W2_W3":
                        SetzeWeichenBelegung("Weiche2", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("Weiche3", belegtmelder.IstBelegt()); break;
                    case "W5": SetzeWeichenBelegung("Weiche5", belegtmelder.IstBelegt()); break;
                    case "W6": SetzeWeichenBelegung("Weiche6", belegtmelder.IstBelegt()); break;
                    case "DKW7_W8": 
                        SetzeWeichenBelegung("Weiche8", belegtmelder.IstBelegt()); 
                        SetzeWeichenBelegung("DKW7_1", belegtmelder.IstBelegt()); 
                        SetzeWeichenBelegung("DKW7_2", belegtmelder.IstBelegt()); break;
                    case "DKW9":
                        SetzeWeichenBelegung("DKW9_1", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("DKW9_2", belegtmelder.IstBelegt()); break;
                    case "KW22":
                        SetzeWeichenBelegung("KW22_1", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("KW22_2", belegtmelder.IstBelegt()); break;
                    case "DKW24_W23":
                        SetzeWeichenBelegung("Weiche23", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("DKW24_1", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("DKW24_2", belegtmelder.IstBelegt()); break;
                    case "W25": SetzeWeichenBelegung("Weiche25", belegtmelder.IstBelegt()); break;
                    case "W26": SetzeWeichenBelegung("Weiche26", belegtmelder.IstBelegt()); break;
                    case "W28_W29":
                        SetzeWeichenBelegung("Weiche28", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("Weiche29", belegtmelder.IstBelegt()); break;
                    case "W27_W30":
                        SetzeWeichenBelegung("Weiche27", belegtmelder.IstBelegt());
                        SetzeWeichenBelegung("Weiche30", belegtmelder.IstBelegt()); break;
                    case "W50": SetzeWeichenBelegung("Weiche50", belegtmelder.IstBelegt()); break;
                    case "W51": SetzeWeichenBelegung("Weiche51", belegtmelder.IstBelegt()); break;               
                    case "HBf6_Halt_R": SetzeWeichenBelegung("Weiche21", belegtmelder.IstBelegt()); break;
                    case "SchattenMitte2":
                        SetzeWeichenBelegung("Weiche71", false);
                        SetzeWeichenBelegung("Weiche72", false);
                        SetzeWeichenBelegung("Weiche73", false);
                        SetzeWeichenBelegung("Weiche74", false);
                        SetzeWeichenBelegung("Weiche75", false);
                        SetzeWeichenBelegung("Weiche76", false);

                        SetzeWeichenBelegung("Weiche70", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche70").Abzweig) break;
                        SetzeWeichenBelegung("Weiche71", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche71").Abzweig) break;
                        SetzeWeichenBelegung("Weiche72", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche72").Abzweig) break;
                        SetzeWeichenBelegung("Weiche73", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche73").Abzweig) break;
                        SetzeWeichenBelegung("Weiche74", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche74").Abzweig) break;
                        SetzeWeichenBelegung("Weiche75", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche75").Abzweig) break;
                        SetzeWeichenBelegung("Weiche76", belegtmelder.IstBelegt()); break;
                    case "SchattenAusfahrt":
                        SetzeWeichenBelegung("Weiche62", false);
                        SetzeWeichenBelegung("Weiche63", false);
                        SetzeWeichenBelegung("Weiche64", false);
                        SetzeWeichenBelegung("Weiche65", false);
                        SetzeWeichenBelegung("Weiche66", false);
                       

                        SetzeWeichenBelegung("Weiche61", belegtmelder.IstBelegt()); if (!WeichenListe.GetWeiche("Weiche61").Abzweig) break;
                        SetzeWeichenBelegung("Weiche62", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche62").Abzweig) break;
                        SetzeWeichenBelegung("Weiche63", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche63").Abzweig) break;
                        SetzeWeichenBelegung("Weiche64", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche64").Abzweig) break;
                        SetzeWeichenBelegung("Weiche65", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche65").Abzweig) break;
                        SetzeWeichenBelegung("Weiche66", belegtmelder.IstBelegt());
                        break;
                    case "W67_W68":
                        if (WeichenListe.GetWeiche("Weiche68").Abzweig && (!WeichenListe.GetWeiche("Weiche67").Abzweig))
                        {
                            SetzeWeichenBelegung("Weiche67", false);
                            SetzeWeichenBelegung("Weiche68", belegtmelder.IstBelegt());
                        }
                        else if ((!WeichenListe.GetWeiche("Weiche68").Abzweig) && WeichenListe.GetWeiche("Weiche67").Abzweig)
                        {
                            SetzeWeichenBelegung("Weiche67", belegtmelder.IstBelegt());
                            SetzeWeichenBelegung("Weiche68", false);
                        }
                        else
                        {
                            SetzeWeichenBelegung("Weiche67", belegtmelder.IstBelegt());
                            SetzeWeichenBelegung("Weiche68", belegtmelder.IstBelegt());
                        }

                        break;
                    default: break;
                }
            }
        }

        void SetzeWeichenBelegung(string Weichenname, bool besetzt)
        {
            Weiche weiche = WeichenListe.GetWeiche(Weichenname);
            if (weiche != null)
            {
                weiche.Besetzt = besetzt;
            }
        }

    }
}
