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

        Dictionary<string, MeldeZustand> GleisZustand;

        public GleisbildZeichnung()
        {
            GleisZustand = new Dictionary<string, MeldeZustand>();
        }

        public void ZeichneSchaltbild(MeldeZustand Zustand, PictureBox picBox)
        {
            if (picBox.Tag == null) return; //Kein Typdefiniert
            if (GleisZustand.ContainsKey(picBox.Name)) 
            {
                if (GleisZustand[picBox.Name].Equals(Zustand)) return; //Nicht nötig neu zu Zeichnen
            }
            else //Element nicht gefunden
            {
                GleisZustand.Add(picBox.Name, Zustand); //Element hinzufügen
            }
            //Grundgleisbild
            Bitmap bild = BasisSchiene(picBox.Tag.ToString()); //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei())
            {
                DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
                GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
                return;
            }

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = BelegtMarkierung(picBox.Tag.ToString());
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);               
            }
            else
            {
                zeichenmuster = FahrstrassenMarkierung(picBox.Tag.ToString(), Zustand.Richtung);
                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            DisplayPicture(bild, picBox); //Zeichne Bild und beende Funktion
            GleisZustand[picBox.Name] = Zustand; //Neuen Zustand übernehmen
            return;
        }


        private void ZeichneFahrstraße(ref Graphics gleisbild, Image Type, Color Farbe, Color TargetColor)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 32;
            int height = 32;
            ColorMap colorMap = new ColorMap
            {
                OldColor = TargetColor, // Color.FromArgb(255, 255, 255, 0),  // original gelb
                NewColor = Farbe  // opaque blue
            };
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);


            gleisbild.DrawImage(Type, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);

        }

        private Bitmap BasisSchiene(string Gleistyp)
        {
            switch(Gleistyp)
            {
                case "Gerade0": return MEKB_H0_Anlage.Properties.Resources.Gerade0;
                case "Gerade45": return MEKB_H0_Anlage.Properties.Resources.Gerade45;
                case "Gerade90": return MEKB_H0_Anlage.Properties.Resources.Gerade90;
                case "Gerade135": return MEKB_H0_Anlage.Properties.Resources.Gerade135;
                case "Gerade180": return MEKB_H0_Anlage.Properties.Resources.Gerade0;
                case "Gerade225": return MEKB_H0_Anlage.Properties.Resources.Gerade45;
                case "Gerade270": return MEKB_H0_Anlage.Properties.Resources.Gerade90;
                case "Gerade315": return MEKB_H0_Anlage.Properties.Resources.Gerade135;
                default: return null;
            }
        }
        private Bitmap BelegtMarkierung(string Gleistyp)
        {
            switch (Gleistyp)
            {
                case "Gerade0": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                case "Gerade45": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                case "Gerade90": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                case "Gerade135": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                case "Gerade180": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                case "Gerade225": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                case "Gerade270": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                case "Gerade315": return MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                default: return null;
            }
        }

        private Bitmap FahrstrassenMarkierung(string Gleistyp, bool Richtung)
        {
            if (Richtung == true) // Nach Westen
            {
                switch (Gleistyp)
                {
                    case "Gerade0": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;
                    case "Gerade45": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                    case "Gerade90": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                    case "Gerade135": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                    case "Gerade180": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                    case "Gerade225": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                    case "Gerade270": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                    case "Gerade315": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts;
                    default: return null;
                }
            }
            else
            {
                switch (Gleistyp)
                {
                    case "Gerade0": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                    case "Gerade45": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                    case "Gerade90": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                    case "Gerade135": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                    case "Gerade180": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;
                    case "Gerade225": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                    case "Gerade270": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                    case "Gerade315": return MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                    default: return null;
                }
            }
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

}
