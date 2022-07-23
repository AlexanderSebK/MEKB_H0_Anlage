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
        #region Farben
        private readonly Color Farbe_Gelb = Color.FromArgb(255, 255, 255, 0);
        private readonly Color Farbe_Gruen = Color.FromArgb(255, 0, 255, 0);
        private readonly Color Farbe_Rot = Color.FromArgb(255, 255, 0, 0);
        private readonly Color Farbe_Grau = Color.FromArgb(255, 128, 128, 128);
        private readonly Color Farbe_Weis = Color.FromArgb(255, 255, 255, 255);
        #endregion

        #region Design-Position
        private const int Gerade = 0;
        private const int KurveR = 1;
        private const int KurveL = 2;
        private const int Prellbock = 3;
        private const int Ecke = 4;

        private const int Belegt_Gerade = 14;
        private const int Belegt_KurveR = 15;
        private const int Belegt_KurveL = 16;
        private const int FahrstrRes_Gerade = 17;
        private const int FahrstrRes_KurveR = 18;
        private const int FahrstrRes_KurveL = 19;
        private const int FahrstrRes_Ecke = 20;
        private const int FahrstrBelegt_Gerade = 21;
        private const int FahrstrBelegt_KurveR = 22;
        private const int FahrstrBelegt_KurveL = 23;
        private const int FahrstrBelegt_Ecke = 24;
        private const int FahrstrRangier_Gerade = 25;
        private const int FahrstrRangier_KurveR = 26;
        private const int FahrstrRangier_KurveL = 27;
        private const int FahrstrRangier_Ecke = 28;
        private const int FahrstrSicher_Gerade = 29;
        private const int FahrstrSicher_KurveR = 30;
        private const int FahrstrSicher_KurveL = 31;
        private const int FahrstrSicher_Ecke = 32;

        private const int Sonder = 33;
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
            RectangleF cloneRect = new RectangleF(0, 0, 32, 32);
            System.Drawing.Imaging.PixelFormat format =  Design.PixelFormat;

            Katalog = new List<List<Bitmap>>();

            for (int reihe = 0; reihe < 34; reihe++)
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

        public void ZeichneSchaltbild(MeldeZustand Zustand, PictureBox picBox)
        {
            ////////////////////////////////////////////////////
            // Prüfen ob Element aktualisiert werden muss / kann
            ////////////////////////////////////////////////////
            if (picBox.Tag == null) return; // Kein Typdefiniert
            if (GleisZustand.ContainsKey(picBox.Name)) 
            {
                if (GleisZustand[picBox.Name].Equals(Zustand)) return; // Nicht nötig neu zu Zeichnen
            }
            else // Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); // Element hinzufügen
            }

            ////////////////////////////////////////////////////
            // Grundgelisbild laden
            ////////////////////////////////////////////////////
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            Bitmap bild = new Bitmap(BasisSchiene(picBox.Tag.ToString())); //Gleisbild
            Graphics gleis  = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

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
               ZeichneFahrstraße(ref gleis, BelegtMarkierung(picBox.Tag.ToString())); 
            }
            else
            {
                zeichenmuster = FahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung);
                if (Zustand.Fahrstrasse) ZeichneFahrstraße(ref gleis, FahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 0));
                if (Zustand.Sicher) ZeichneFahrstraße(ref gleis, FahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 3));
                if (Zustand.Besetzt) ZeichneFahrstraße(ref gleis, FahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung, 1));
            }
            DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
            return;
        }


        private void ZeichneFahrstraße(ref Graphics gleisbild, Image Type)
        {
            if (Type == null) Type = Katalog[Sonder][Error];
            gleisbild.DrawImage(Type, new Rectangle(0, 0, 32, 32));
        }

        private Bitmap BasisSchiene(string Gleistyp)
        {
            List<Bitmap> Winkelauswahl;
            if (Gleistyp.StartsWith("Gerade")) Winkelauswahl = Katalog[Gerade];
            else if (Gleistyp.StartsWith("KurveR")) Winkelauswahl = Katalog[KurveR];
            else if (Gleistyp.StartsWith("KurveL")) Winkelauswahl = Katalog[KurveL];
            else if (Gleistyp.StartsWith("Prellbock")) Winkelauswahl = Katalog[Prellbock];
            else if (Gleistyp.StartsWith("Ecke")) Winkelauswahl = Katalog[Ecke];
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
        
        /// <summary>
        /// Fahrstrasse(reserviert) aus dem Katalog suchen
        /// </summary>
        /// <param name="Gleistyp">GleisTyp</param>
        /// <param name="Richtung">true = Mit normale Fahrrichtung</param>
        /// <param name="typ">0 = Reserviert; 1 = Belegt; 2 = Rangier; 3 = Sicher</param>
        /// <returns>Bild aus der Datei</returns>
        private Bitmap FahrstrassenMarkierung(string Gleistyp, bool Richtung, int typ = 0)
        {
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
        }

        public MeldeZustand(Weiche weiche, bool richtung)
        {
            Besetzt = weiche.Besetzt;
            Fahrstrasse = weiche.FahrstrasseAktive;
            Sicher = weiche.FahrstrasseSicher;
            Richtung = weiche.FahrstrasseRichtung_vonZunge ^ richtung;
        }

        public MeldeZustand(bool StatusALL)
        {
            Besetzt = StatusALL;
            Fahrstrasse = StatusALL;
            Sicher = StatusALL;
            Richtung = StatusALL;
        }

        public bool Besetzt { get; set; }
        public bool Fahrstrasse { get; set; }
        public bool Sicher { get; set; }
        public bool Richtung { get; set; }

        public bool IstFrei()
        {
            return !(Besetzt || Fahrstrasse);
        }
    }

}
