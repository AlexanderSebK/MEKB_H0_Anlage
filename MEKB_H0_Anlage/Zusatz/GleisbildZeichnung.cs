using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{

    public class GleisbildZeichnung
    {

        #region Design-Position
        private const int Gerade = 0;
        private const int KurveR = 1;
        private const int KurveL = 2;
        private const int Prellbock = 3;
        private const int Ecke = 4;
        private const int WeicheR = 5;
        private const int WeicheL = 6;
        private const int Dreiweg = 7;
        private const int Kreuzung = 8;
        private const int KW = 9;
        private const int DKW = 10;
        private const int Zunge_G = 11;
        private const int Zunge_R = 12;
        private const int Zunge_L = 13;
        private const int ZungeAktiv_G = 14;
        private const int ZungeAktiv_R = 15;
        private const int ZungeAktiv_L = 16;
        private const int Belegt_Gerade = 17;
        private const int Belegt_KurveR = 18;
        private const int Belegt_KurveL = 19;
        private const int FahrstrRes_Gerade = 20;
        private const int FahrstrRes_KurveR = 21;
        private const int FahrstrRes_KurveL = 22;
        private const int FahrstrRes_Ecke = 23;
        private const int FahrstrBelegt_Gerade = 24;
        private const int FahrstrBelegt_KurveR = 25;
        private const int FahrstrBelegt_KurveL = 26;
        private const int FahrstrBelegt_Ecke = 27;
        private const int FahrstrRangier_Gerade = 28;
        private const int FahrstrRangier_KurveR = 29;
        private const int FahrstrRangier_KurveL = 30;
        private const int FahrstrRangier_Ecke = 31;
        private const int FahrstrSicher_Gerade = 32;
        private const int FahrstrSicher_KurveR = 33;
        private const int FahrstrSicher_KurveL = 34;
        private const int FahrstrSicher_Ecke = 35;

        private const int Sonder = 36;
        private const int Fehler = 0;
        private const int Unbekannt = 1;
        private const int Error = 3;
        private const int EmptyImage = 7;

        #endregion

        public List<List<Bitmap>> Katalog;


        public Dictionary<string, MeldeZustand> GleisZustand;

        public GleisbildZeichnung()
        {
            GleisZustand = new Dictionary<string, MeldeZustand>();
            Katalog = new List<List<Bitmap>>();
        }

        public GleisbildZeichnung(string Dateipfad)
        {
            GleisZustand = new Dictionary<string, MeldeZustand>();
            Katalog = new List<List<Bitmap>>();
            ImportiereZeichenDesign(Dateipfad);
        }

        public void ImportiereZeichenDesign(string Dateienpfad)
        {
            // Lade Bitmap von Datei
            Bitmap Design = new Bitmap(Dateienpfad);

            // Clone a portion of the Bitmap object.
            RectangleF cloneRect;
            System.Drawing.Imaging.PixelFormat format =  Design.PixelFormat;

            Katalog = new List<List<Bitmap>>();

            for (int reihe = 0; reihe < (Design.Height/32); reihe++)
            {
                List<Bitmap> Typ = new List<Bitmap>();
                for (int spalte = 0; spalte < 8; spalte++)
                {
                    cloneRect = new RectangleF(spalte * 32, reihe * 32, 32, 32);
                    Bitmap section = Design.Clone(cloneRect, format);
                    Typ.Add(section);
                }
                Katalog.Add(Typ);
            }
            //Reset der Zeichnung
            GleisZustand = new Dictionary<string, MeldeZustand>();
        }

        private bool ZeichneGleis(MeldeZustand Zustand, string Typ, out Bitmap bild)
        {
            
            ////////////////////////////////////////////////////
            // Grundgelisbild laden
            ////////////////////////////////////////////////////
            try
            {
                bild = new Bitmap(BasisSchiene(Typ)); //Gleisbild   
            }
            catch
            {
                bild = new Bitmap(32,32);
                return false;
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
            if (Zustand.IstFrei())
            {
                return true;
            }

            ////////////////////////////////////////////////////
            // Belegtmeldung / Fahrstrasse zeichnen
            ////////////////////////////////////////////////////
            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                BildHinzufuegen(ref gleis, BelegtMarkierung(Typ));
            }
            else
            {
                if (Zustand.Fahrstrasse) BildHinzufuegen(ref gleis, FahrstrassenMarkierung(Typ, Zustand.Richtung, 0));
                if (Zustand.Sicher) BildHinzufuegen(ref gleis, FahrstrassenMarkierung(Typ, Zustand.Richtung, 3));
                if (Zustand.Besetzt) BildHinzufuegen(ref gleis, FahrstrassenMarkierung(Typ, Zustand.Richtung, 1));
            }
            return true;
        }

        private bool ZeichneWeiche(Weiche weiche, string Typ, out Bitmap bild)
        {
            List<String> ErlaubteTags = new List<string>() { "Weiche", "DKW", "KW", "DreiwegWeiche" };
            bool TagVorhanden = false;
            foreach (String Tag in ErlaubteTags)
            {
                if (Typ.StartsWith(Tag)) TagVorhanden = true;
            }
            if (!TagVorhanden)
            {
                bild = Katalog[Sonder][Error];
                return false;
            }

            try
            {
                bild = new Bitmap(BasisSchiene(Typ)); //Gleisbild   
            }
            catch
            {
                bild = new Bitmap(32, 32);
                return false;
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            ////////////////////////////////////////////////////
            // Zunge zeichnen
            ////////////////////////////////////////////////////

            if (weiche.Status_Error)
            {
                BildHinzufuegen(ref gleis, Katalog[Sonder][Fehler]);
                return true;
            }
            if (weiche.Status_Unbekannt)
            {
                BildHinzufuegen(ref gleis, Katalog[Sonder][Unbekannt]);
                return true; 
            }
            BildHinzufuegen(ref gleis, WeichenZunge(Typ, weiche.Abzweig, weiche.ZeitAktiv > 0));

            MeldeZustand Zustand = new MeldeZustand(weiche);
            if (Zustand.IstFrei())
            {
                return true;
            }

            ////////////////////////////////////////////////////
            // Belegtmeldung / Fahrstrasse zeichnen
            ////////////////////////////////////////////////////
            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                BildHinzufuegen(ref gleis, WeicheBelegtMarkierung(Typ, weiche.Abzweig));
            }
            else
            {
                if (Zustand.Fahrstrasse) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(Typ, Zustand.Richtung, 0, weiche.Abzweig));
                if (Zustand.Sicher) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(Typ, Zustand.Richtung, 3, weiche.Abzweig));
                if (Zustand.Besetzt) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(Typ, Zustand.Richtung, 1, weiche.Abzweig));
            }
            return true;

        }


        public void ZeichneSchaltbild(MeldeZustand Zustand, PictureBox picBox, bool ErzwingeZeichnen = false)
        {
            if (picBox.Tag == null) return; // Kein Typdefiniert
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) && GleisZustand[picBox.Name].Equals(Zustand)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
            }
            if (picBox.Tag.ToString().Contains('+')) return;
            if (!ZeichneGleis(Zustand, picBox.Tag.ToString(), out Bitmap bild))
            {
                return; //Bild nicht Zeichnen, da Fehler
            }
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen

            DisplayPicture(bild, picBox); //Zeichne Bild
        }
        public void ZeichneSchaltbild(MeldeZustand Zustand, MeldeZustand Zustand2, PictureBox picBox, bool ErzwingeZeichnen = false)
        {

            if (picBox.Tag == null) return; // Kein Typdefiniert
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) &&
                    GleisZustand[picBox.Name].Equals(Zustand) &&
                    GleisZustand[picBox.Name + "Zustand2"].Equals(Zustand2)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
                GleisZustand.Add(picBox.Name + "Zustand2", Zustand2); // Element hinzufügen
            }


            if (!picBox.Tag.ToString().Contains('+')) return;
            string[] Gleistypen = picBox.Tag.ToString().Split('+');

            if(!ZeichneGleis(Zustand, Gleistypen[0], out Bitmap bild)) // Erstes Gleisbild zeichnen
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen

            if(!ZeichneGleis(Zustand2, Gleistypen[1], out Bitmap bild2))
            {
                return; //Bild nicht Zeichnen, da Fehler
            }
            BildHinzufuegen(ref gleis, bild2); // Zweites Gleisbild darüberzeichnen
            GleisZustand[picBox.Name + "Zustand2"] = Zustand2; //Neuen Zustand übernehmen

            DisplayPicture(bild, picBox); //Zeichne Bild            
        }
        public void ZeichneSchaltbild(MeldeZustand Zustand, MeldeZustand Zustand2, MeldeZustand Zustand3, PictureBox picBox, bool ErzwingeZeichnen = false)
        {
            if (picBox.Tag == null) return; // Kein Typdefiniert
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) &&
                    GleisZustand[picBox.Name].Equals(Zustand) &&
                    GleisZustand[picBox.Name + "Zustand2"].Equals(Zustand2) &&
                    GleisZustand[picBox.Name + "Zustand3"].Equals(Zustand3)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
                GleisZustand.Add(picBox.Name + "Zustand2", Zustand2); // Element hinzufügen
                GleisZustand.Add(picBox.Name + "Zustand3", Zustand2); // Element hinzufügen
            }
            if (!picBox.Tag.ToString().Contains('+')) return;

            string[] Gleistypen = picBox.Tag.ToString().Split('+');
            if (Gleistypen.Length != 3) return;

            if (!ZeichneGleis(Zustand, Gleistypen[0], out Bitmap bild)) // Erstes Gleisbild zeichnen
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen

            if (!ZeichneGleis(Zustand2, Gleistypen[1], out Bitmap bild2)) // Zweites Gleisbild zeichnen
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            BildHinzufuegen(ref gleis, bild2); // Zweites Gleisbild darüberzeichnen
            GleisZustand[picBox.Name + "Zustand2"] = Zustand2; //Neuen Zustand übernehmen

            if (!ZeichneGleis(Zustand3, Gleistypen[2], out Bitmap bild3)) // Zweites Gleisbild zeichnen
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            BildHinzufuegen(ref gleis, bild3); // Drittes Gleisbild darüberzeichnen
            GleisZustand[picBox.Name + "Zustand3"] = Zustand3; //Neuen Zustand übernehmen

            DisplayPicture(bild, picBox); //Zeichne Bild            
        }

        public void ZeichneSchaltbild(Weiche weiche, PictureBox picBox, bool ErzwingeZeichnen = false)
        {
            ////////////////////////////////////////////////////
            // Prüfen ob Element aktualisiert werden muss / kann
            ////////////////////////////////////////////////////
            if (picBox.Tag == null) return; // Kein Typdefiniert

            MeldeZustand Zustand = new MeldeZustand(weiche);
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) && GleisZustand[picBox.Name].Equals(Zustand)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
            }
            if (weiche.Status_Error || weiche.Status_Error) Zustand.UpdateNoetig = true;
            if (picBox.Tag.ToString().Contains('+')) return;

            if (!ZeichneWeiche(weiche, picBox.Tag.ToString(), out Bitmap bild))
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen

            DisplayPicture(bild, picBox); //Zeichne Bild
        }
        public void ZeichneSchaltbild(Weiche weiche, MeldeZustand Zustand2, PictureBox picBox, bool ErzwingeZeichnen = false)
        {
            ////////////////////////////////////////////////////
            // Prüfen ob Element aktualisiert werden muss / kann
            ////////////////////////////////////////////////////
            if (picBox.Tag == null) return; // Kein Typdefiniert

            MeldeZustand Zustand = new MeldeZustand(weiche);
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) && GleisZustand[picBox.Name].Equals(Zustand)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
            }
            if (weiche.Status_Error || weiche.Status_Error) Zustand.UpdateNoetig = true;
            if (!picBox.Tag.ToString().Contains('+')) return;

            string[] Gleistypen = picBox.Tag.ToString().Split('+');


            if (!ZeichneWeiche(weiche, Gleistypen[0], out Bitmap bild))
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen

            if (!ZeichneGleis(Zustand2, Gleistypen[1], out Bitmap bild2)) // Zweites Gleisbild zeichnen
            {
                return;  //Bild nicht Zeichnen, da Fehler
            }
            BildHinzufuegen(ref gleis, bild2); // Zweites Gleisbild darüberzeichnen
            GleisZustand[picBox.Name + "Zustand2"] = Zustand2; //Neuen Zustand übernehmen
            DisplayPicture(bild, picBox); //Zeichne Bild
        }

        public void ZeichneSchaltbild1(Weiche weiche, PictureBox picBox, bool ErzwingeZeichnen = false)
        {
            ////////////////////////////////////////////////////
            // Prüfen ob Element aktualisiert werden muss / kann
            ////////////////////////////////////////////////////
            if (picBox.Tag == null) return; // Kein Typdefiniert

            // Tag startet mit dieser Bezeichnung
            List<String> ErlaubteTags = new List<string>(){ "Weiche", "DKW", "KW", "DreiwegWeiche" };
            bool TagVorhanden = false;
            foreach(String Tag in ErlaubteTags)
            {
                if (picBox.Tag.ToString().StartsWith(Tag)) TagVorhanden = true;
            }
            if (!TagVorhanden) return;

            MeldeZustand Zustand = new MeldeZustand(weiche);
            if (GleisZustand.ContainsKey(picBox.Name))
            {
                if ((!ErzwingeZeichnen) && GleisZustand[picBox.Name].Equals(Zustand)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
            }

            ////////////////////////////////////////////////////
            // Grundgleisbild laden
            ////////////////////////////////////////////////////
            Bitmap bild;
            try
            {
                bild = new Bitmap(BasisSchiene(picBox.Tag.ToString())); //Gleisbild   
            }
            catch
            {
                return;
            }
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            ////////////////////////////////////////////////////
            // Zunge zeichnen
            ////////////////////////////////////////////////////

            if (weiche.Status_Error)
            {
                BildHinzufuegen(ref gleis, Katalog[Sonder][Fehler]);
                DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
                Zustand.UpdateNoetig = true;
                GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
                return;
            }
            if (weiche.Status_Unbekannt)
            {
                BildHinzufuegen(ref gleis, Katalog[Sonder][Unbekannt]);
                DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
                Zustand.UpdateNoetig = true;
                GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
                return;
            }
            BildHinzufuegen(ref gleis, WeichenZunge(picBox.Tag.ToString(), weiche.Abzweig, weiche.ZeitAktiv > 0));
             

            if (Zustand.IstFrei())
            {
                DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
                GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
                return;
            }

            ////////////////////////////////////////////////////
            // Belegtmeldung / Fahrstrasse zeichnen
            ////////////////////////////////////////////////////
            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                BildHinzufuegen(ref gleis, WeicheBelegtMarkierung(picBox.Tag.ToString(), weiche.Abzweig));
            }
            else
            {
                if (Zustand.Fahrstrasse) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 0, weiche.Abzweig));
                if (Zustand.Sicher) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 3, weiche.Abzweig));
                if (Zustand.Besetzt) BildHinzufuegen(ref gleis, WeicheFahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 1, weiche.Abzweig));
            }
            DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
            return;
        }

        private void BildHinzufuegen(ref Graphics gleisbild, Image Type)
        {
            if (Type == null) Type = Katalog[Sonder][Error];
            gleisbild.DrawImage(Type, new Rectangle(0, 0, 32, 32));
        }

        private Bitmap BasisSchiene(string Gleistyp)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }

            List<Bitmap> Winkelauswahl;
            if (Gleistyp.StartsWith("Gerade")) Winkelauswahl = Katalog[Gerade];
            else if (Gleistyp.StartsWith("KurveR")) Winkelauswahl = Katalog[KurveR];
            else if (Gleistyp.StartsWith("KurveL")) Winkelauswahl = Katalog[KurveL];
            else if (Gleistyp.StartsWith("Prellbock")) Winkelauswahl = Katalog[Prellbock];
            else if (Gleistyp.StartsWith("Ecke")) Winkelauswahl = Katalog[Ecke];
            else if (Gleistyp.StartsWith("WeicheR")) Winkelauswahl = Katalog[WeicheR];
            else if (Gleistyp.StartsWith("WeicheL")) Winkelauswahl = Katalog[WeicheL];
            else
            {
                return Katalog[Sonder][Error];
            }

            if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
            else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
            else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
            else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
            else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
            else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
            else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
            else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];

            else if (Gleistyp.EndsWith("_OR")) return Winkelauswahl[0];
            else if (Gleistyp.EndsWith("_UR")) return Winkelauswahl[1];
            else if (Gleistyp.EndsWith("_UL")) return Winkelauswahl[2];
            else if (Gleistyp.EndsWith("_OL")) return Winkelauswahl[3];
            else if (Gleistyp.EndsWith("_OR_SP")) return Winkelauswahl[4];
            else if (Gleistyp.EndsWith("_UR_SP")) return Winkelauswahl[5];
            else if (Gleistyp.EndsWith("_UL_SP")) return Winkelauswahl[6];
            else if (Gleistyp.EndsWith("_OL_SP")) return Winkelauswahl[7];
            else
            {
                return Katalog[Sonder][Error];
            }

        }  
        private Bitmap BelegtMarkierung(string Gleistyp)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }
            List<Bitmap> Winkelauswahl;
            if (Gleistyp.StartsWith("Gerade")) Winkelauswahl = Katalog[Belegt_Gerade];
            else if (Gleistyp.StartsWith("KurveR")) Winkelauswahl = Katalog[Belegt_KurveR];
            else if (Gleistyp.StartsWith("KurveL")) Winkelauswahl = Katalog[Belegt_KurveL];
            else if (Gleistyp.StartsWith("Prellbock")) return Katalog[Sonder][EmptyImage]; //Return Empty
            else if (Gleistyp.StartsWith("Ecke")) return Katalog[Sonder][EmptyImage];
            else
            {
                return Katalog[Sonder][Error];
            }

            if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
            else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
            else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
            else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
            else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
            else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
            else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
            else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
          
            else
            {
                return Katalog[Sonder][Error];
            }
        }

        private Bitmap WeichenZunge(string Gleistyp, bool Abzweig, bool Aktiv)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }
            List<Bitmap> Winkelauswahl;
            if(Abzweig)
            {
                if (Gleistyp.StartsWith("WeicheR"))
                {
                    if (Aktiv) Winkelauswahl = Katalog[ZungeAktiv_R];
                    else Winkelauswahl = Katalog[Zunge_R];
                }
                else if (Gleistyp.StartsWith("WeicheL"))
                {
                    if (Aktiv) Winkelauswahl = Katalog[ZungeAktiv_L];
                    else Winkelauswahl = Katalog[Zunge_L];
                }
                else
                {
                    return Katalog[Sonder][Error];
                }
            }
            else
            {
                if (Gleistyp.StartsWith("WeicheR"))
                {
                    if (Aktiv) Winkelauswahl = Katalog[ZungeAktiv_G];
                    else Winkelauswahl = Katalog[Zunge_G];
                }
                else if (Gleistyp.StartsWith("WeicheL"))
                {
                    if (Aktiv) Winkelauswahl = Katalog[ZungeAktiv_G];
                    else Winkelauswahl = Katalog[Zunge_G];
                }
                else
                {
                    return Katalog[Sonder][Error];
                }
            }
            if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
            else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
            else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
            else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
            else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
            else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
            else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
            else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];

            else
            {
                return Katalog[Sonder][Error];
            }
        }

        private Bitmap WeicheBelegtMarkierung(string Gleistyp, bool Abzweig)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }
            List<Bitmap> Winkelauswahl;
            if (Abzweig)
            {
                if (Gleistyp.StartsWith("WeicheR")) Winkelauswahl = Katalog[Belegt_KurveR];
                else if (Gleistyp.StartsWith("WeicheL")) Winkelauswahl = Katalog[Belegt_KurveL];
                else
                {
                    return Katalog[Sonder][Error];
                }
            }
            else
            {
                if (Gleistyp.StartsWith("WeicheR")) Winkelauswahl = Katalog[Belegt_Gerade];
                else if (Gleistyp.StartsWith("WeicheL")) Winkelauswahl = Katalog[Belegt_Gerade];
                else
                {
                    return Katalog[Sonder][Error];
                }
            }
            if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
            else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
            else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
            else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
            else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
            else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
            else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
            else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];

            else
            {
                return Katalog[Sonder][Error];
            }
        }

        /// <summary>
        /// Fahrstrasse(reserviert) aus dem Katalog suchen
        /// </summary>
        /// <param name="Gleistyp">GleisTyp</param>
        /// <param name="Richtung">true = Mit normale Fahrrichtung</param>
        /// <param name="typ">0 = Reserviert; 1 = Belegt; 2 = Rangier; 3 = Sicher</param>
        /// <returns>Bild aus der Datei</returns>
        private Bitmap FahrstrassenMarkierung(string Gleistyp, bool Richtung, int typ = 0)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }
            List<Bitmap> Winkelauswahl;
            if (Gleistyp.StartsWith("Gerade"))
            {
                if(typ == 0) Winkelauswahl = Katalog[FahrstrRes_Gerade];
                else if(typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_Gerade];
                else if(typ == 2) Winkelauswahl = Katalog[FahrstrRangier_Gerade];
                else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_Gerade];
                else return Katalog[Sonder][Error];
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[3];
                }
            }
            else if (Gleistyp.StartsWith("KurveR"))
            {
                if (Richtung)
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveR];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveR];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveR];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveR];
                    else return Katalog[Sonder][Error];
                }
                else
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveL];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveL];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveL];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveL];
                    else return Katalog[Sonder][Error];
                }
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[2];
                }
            }
            else if (Gleistyp.StartsWith("KurveL"))
            {
                if (Richtung)
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveL];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveL];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveL];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveL];
                    else return Katalog[Sonder][Error];
                }
                else
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveR];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveR];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveR];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveR];
                    else return Katalog[Sonder][Error];
                }
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[4];
                }
            }
            else if (Gleistyp.StartsWith("Prellbock")) return Katalog[Sonder][EmptyImage]; //Return Empty
            else if (Gleistyp.StartsWith("Ecke"))
            {
                if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_Ecke];
                else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_Ecke];
                else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_Ecke];
                else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_Ecke];
                else return Katalog[Sonder][Error];
                
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_OR")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_UR")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_UL")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_OL")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_OR_SP")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_UR_SP")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_UL_SP")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_OL_SP")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_OR")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_UR")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_UL")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_OL")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_OR_SP")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_UR_SP")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_UL_SP")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_OL_SP")) return Winkelauswahl[3];
                }
            }
            else
            {
                return Katalog[Sonder][Error];
            }

            return Katalog[Sonder][Error];
        }

        /// <summary>
        /// Fahrstrasse(reserviert) aus dem Katalog suchen
        /// </summary>
        /// <param name="Gleistyp">GleisTyp</param>
        /// <param name="Richtung">true = Mit normale Fahrrichtung</param>
        /// <param name="typ">0 = Reserviert; 1 = Belegt; 2 = Rangier; 3 = Sicher</param>
        /// <returns>Bild aus der Datei</returns>
        private Bitmap WeicheFahrstrassenMarkierung(string Gleistyp, bool Richtung, int typ, bool Abzweig)
        {
            if (Gleistyp.EndsWith("_Gegen"))
            {
                Gleistyp = Gleistyp.Substring(0, Gleistyp.Length - 6);
            }
            List<Bitmap> Winkelauswahl;
            if (!Abzweig)
            {
                if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_Gerade];
                else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_Gerade];
                else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_Gerade];
                else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_Gerade];
                else return Katalog[Sonder][Error];
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[3];
                }
            }
            else if (Gleistyp.StartsWith("WeicheR"))
            {
                if (Richtung)
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveR];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveR];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveR];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveR];
                    else return Katalog[Sonder][Error];
                }
                else
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveL];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveL];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveL];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveL];
                    else return Katalog[Sonder][Error];
                }
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[2];
                }
            }
            else if (Gleistyp.StartsWith("WeicheL"))
            {
                if (Richtung)
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveL];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveL];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveL];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveL];
                    else return Katalog[Sonder][Error];
                }
                else
                {
                    if (typ == 0) Winkelauswahl = Katalog[FahrstrRes_KurveR];
                    else if (typ == 1) Winkelauswahl = Katalog[FahrstrBelegt_KurveR];
                    else if (typ == 2) Winkelauswahl = Katalog[FahrstrRangier_KurveR];
                    else if (typ == 3) Winkelauswahl = Katalog[FahrstrSicher_KurveR];
                    else return Katalog[Sonder][Error];
                }
                if (Richtung)
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[4];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[7];
                }
                else
                {
                    if (Gleistyp.EndsWith("_0")) return Winkelauswahl[5];
                    else if (Gleistyp.EndsWith("_45")) return Winkelauswahl[6];
                    else if (Gleistyp.EndsWith("_90")) return Winkelauswahl[7];
                    else if (Gleistyp.EndsWith("_135")) return Winkelauswahl[0];
                    else if (Gleistyp.EndsWith("_180")) return Winkelauswahl[1];
                    else if (Gleistyp.EndsWith("_225")) return Winkelauswahl[2];
                    else if (Gleistyp.EndsWith("_270")) return Winkelauswahl[3];
                    else if (Gleistyp.EndsWith("_315")) return Winkelauswahl[4];
                }
            }
            
            else
            {
                return Katalog[Sonder][Error];
            }

            return Katalog[Sonder][Error];
        }


        /// <summary>
        /// Involke-Funktion. Verhindert Fehlermeldung beim gleichzeitigen Zugreifen auf ein Bild
        /// </summary>
        /// <param name="img">Neues Bild zum Anzeiegen</param>
        /// <param name="picBox">Instanz der PictureBox</param>
        private void DisplayPicture(Bitmap img, PictureBox picBox)
        {
            picBox.Invoke(new EventHandler(delegate
            {
                picBox.Image = img;
            }));
        }


    }

    public struct MeldeZustand
    {
        public MeldeZustand(bool besetzt, bool fahrstrasse, bool sicher, bool richtung)
        {
            Besetzt = besetzt;
            Fahrstrasse = fahrstrasse;
            Sicher = sicher;
            Richtung = richtung;
            UpdateNoetig = false;
        }

        public MeldeZustand(Weiche weiche, bool richtung=false)
        {
            Besetzt = weiche.Besetzt;
            Fahrstrasse = weiche.FahrstrasseAktive;
            Sicher = weiche.FahrstrasseSicher;
            Richtung = weiche.FahrstrasseRichtung_vonZunge ^ richtung;
            UpdateNoetig = false;
        }

        public MeldeZustand(bool StatusALL)
        {
            Besetzt = StatusALL;
            Fahrstrasse = StatusALL;
            Sicher = StatusALL;
            Richtung = StatusALL;
            UpdateNoetig = false;
        }

        public bool Besetzt { get; set; }
        public bool Fahrstrasse { get; set; }
        public bool Sicher { get; set; }
        public bool Richtung { get; set; }
        public bool UpdateNoetig { get; set; }

        public bool IstFrei()
        {
            return !(Besetzt || Fahrstrasse);
        }
    }

}
