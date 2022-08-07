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
    public partial class Form1 : Form
    {

        MeldeZustand FreiesGleis = new MeldeZustand(false);       
        
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
                if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) AutoSignalUpdate(fahrstrasse.Fahrstr_Sig.Name);
            }
            else
            {
                //Fahrstraße aktivieren
                if (Betriebsbereit) fahrstrasse.StarteFahrstrasse(WeichenListe.Liste);
            }
            //Weichenliste der Fahrstraßen übernehmen
            List<Weiche> FahrstrassenWeichen = fahrstrasse.GetFahrstrassenListe();
            //Weichenliste durchgehen
            foreach (Weiche Fahrstrassenweiche in FahrstrassenWeichen)
            {
                Weiche weiche = WeichenListe.GetWeiche(Fahrstrassenweiche.Name);
                if (weiche != null)
                {
                    UpdateWeicheImGleisplan(weiche); //Weiche im Gleisplan aktualisieren
                }
            }
            //Alle Fahrstraßen/Buttons aktualisieren
            
            FahrstrasseBildUpdate();
            UpdateSchalter();
        }



        #region Fahrstarßen Setup
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            //Fahrstrassen Importieren
            FahrstrassenListe = new FahrstrassenListe("Fahrstrassenliste.xml", WeichenListe, SignalListe);
        }



        #endregion
        #region Fahrstraßen bestimmen
        /// <summary>
        /// Generiere Statuskonstrukt aus Fahrstraßen und Belegtmeldung
        /// </summary>
        /// <param name="besetzt">Belegtstatus des Abschnitts</param>
        /// <param name="Fahrstrassen_west">Fahrstraßen, die diesen Abschnitt nach westen(links) belegen </param>
        /// <param name="Fahrstrassen_ost">Fahrstraßen, die diesen Abschnitt nach osten(rechts) belegen</param>
        /// <returns></returns>
        MeldeZustand ErrechneZustand(bool besetzt, List<Fahrstrasse> Fahrstrassen_west, List<Fahrstrasse> Fahrstrassen_ost)
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
        
        

        #region Bahnhofsausfahrt links
        private void UpdateGleisbild_GL1_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_mit, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_mit, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_3);
        }
        private void UpdateGleisbild_GL2_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_3);
        }
        private void UpdateGleisbild_GL3_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_3);
        }
        private void UpdateGleisbild_GL4_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_2);
        }
        private void UpdateGleisbild_GL5_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL5_links_0);

        }
        private void UpdateGleisbild_GL6_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL6_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL6_links_1);
        }
        #endregion
        #region Bahnhofsausfahrt rechts
        private void UpdateGleisbild_GL1_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_5);
        }
        private void UpdateGleisbild_GL2_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_3);
        }
        private void UpdateGleisbild_GL3_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_3);
        }
        private void UpdateGleisbild_GL4_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_2);
        }       
        private void UpdateGleisbild_GL5_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL5_rechts_0);
        }
        private void UpdateGleisbild_GL6_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL6_rechts_0);
        }
        #endregion
        #region Bahnhofseinfahrt links
        private void UpdateGleisbild_Weiche5(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche5_Gleis2);
        }
        #endregion
        #region Bahnhof
        private void UpdateGleisbild_Gl1_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_L3);
        }
        private void UpdateGleisbild_Gl1_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_Halt_R3);
        }
        private void UpdateGleisbild_Gl1_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl1_2);
        }

        private void UpdateGleisbild_Gl2_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_L3);
        }
        private void UpdateGleisbild_Gl2_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_Halt_R3);
        }
        private void UpdateGleisbild_Gl2_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl2_2);
        }

        private void UpdateGleisbild_Gl3_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_L3);
        }
        private void UpdateGleisbild_Gl3_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_Halt_R3);
        }
        private void UpdateGleisbild_Gl3_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl3_2);
        }

        private void UpdateGleisbild_Gl4_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_L3);
        }
        private void UpdateGleisbild_Gl4_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_Halt_R3);
        }
        private void UpdateGleisbild_Gl4_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl4_2);
        }

        private void UpdateGleisbild_Gl5_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_L3);
        }
        private void UpdateGleisbild_Gl5_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_Halt_R3);
        }
        private void UpdateGleisbild_Gl5_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl5_2);
        }

        private void UpdateGleisbild_Gl6_Halt_links(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_L1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_L2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_L3);
        }
        private void UpdateGleisbild_Gl6_Halt_rechts(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_R1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_R2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_Halt_R3);
        }
        private void UpdateGleisbild_Gl6_Zentrum(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Bhf_Gl6_2);
        }
        #endregion
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

        private void UpdateGleisbild_Schatten_Gl8(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_6);
        }
        private void UpdateGleisbild_Schatten_Gl8_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl8_Halt_1);
        }

        private void UpdateGleisbild_Schatten_Gl9(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_6);
        }
        private void UpdateGleisbild_Schatten_Gl9_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl9_Halt_2);
        }

        private void UpdateGleisbild_Schatten_Gl10(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_6);
        }
        private void UpdateGleisbild_Schatten_Gl10_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl10_Halt_2);
        }

        private void UpdateGleisbild_Schatten_Gl11(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_6);
        }
        private void UpdateGleisbild_Schatten_Gl11_Halt(bool besetzt)
        {
            MeldeZustand zustand = new MeldeZustand(besetzt, false, false, false);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, SchattenGl11_Halt_2);
        }

        #endregion

        #region Block 1
        private void UpdateGleisbild_Weiche6(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche6_Gleis4);
        }
        private void UpdateGleisbild_Block1a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1a_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1a_2);
        }
        private void UpdateGleisbild_Block1b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_7);
        }
        private void UpdateGleisbild_Block1_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1_Halt_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1_Halt_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1_Halt_4);
        }
        #endregion
        #region Block 2
        private void UpdateGleisbild_Block2(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_10);
        }
        private void UpdateGleisbild_Block2_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_Halt_2);
        }
        #endregion
        #region Block 3
        private void UpdateGleisbild_Weiche25(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche25_Gleis2);
        }
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
        private void UpdateGleisbild_Weiche26(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche26_Gleis2);
        }
        #endregion
        #region Block 5
        private void UpdateGleisbild_Block5a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {           
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_4);
        }
        private void UpdateGleisbild_Block5b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_9);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_10);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_11);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_12);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_13);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_14);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_15);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_16);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_17);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5b_18);
        }
        private void UpdateGleisbild_Block5_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5_Halt_2);
        }
        #endregion
        #region Block 6
        private void UpdateGleisbild_Block6(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_9);
        }
        private void UpdateGleisbild_Block6_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_Halt1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_Halt2);
        }
        #endregion
        #region Block 7
        private void UpdateGleisbild_Block7(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand,   Block7_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_9);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_10);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_11);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_12);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_13);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_14);

        }
        #endregion
        #region Block 8
        private void UpdateGleisbild_Block8(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_9);        
        }
        private void UpdateGleisbild_Block8_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_Halt_2);
        }
        #endregion
        #region Block 9
        private void UpdateGleisbild_Block9(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_9);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_10);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_11);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_12);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_13);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_14);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_15);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_16);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_17);
        }
        private void UpdateGleisbild_Block9_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_gegen)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_gegen);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block9_Halt_2);
        }
        #endregion

        private void UpdateKreuzung(bool besetzt, List<Fahrstrasse> Fahrstrasse_Block8, List<Fahrstrasse> Fahrstrasse_Block9)
        {
            MeldeZustand zustand8 = ErrechneZustand(besetzt, new List<Fahrstrasse>(), Fahrstrasse_Block8); //Gleis8
            MeldeZustand zustand9 = ErrechneZustand(besetzt, Fahrstrasse_Block9, new List<Fahrstrasse>()); //Gleis9

            if (zustand9.Fahrstrasse == true)
            {
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, FreiesGleis, Kreuzung1_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Kreuzung1_2);
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, FreiesGleis, Kreuzung1_3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Kreuzung1_4);
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, FreiesGleis, Kreuzung1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Kreuzung1_5);
            }
            else if (zustand8.Fahrstrasse == true)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, zustand8, Kreuzung1_1);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, zustand8, Kreuzung1_3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, zustand8, Kreuzung1);  
                zustand8.Richtung = !zustand8.Richtung;
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_4);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_5);

            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, FreiesGleis, Kreuzung1_1);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_2);
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, FreiesGleis, Kreuzung1_3);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_4);
                GleisbildZeichnung.ZeichneSchaltbild(zustand9, zustand8, Kreuzung1);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_5);
            }
       
        }

        #region Schattenbahnhof
        private void UpdateGleisbild_SchattenkleinAusf(bool besetzt)
        {
            bool besetzt8 = false;
            bool besetzt9 = false;
            bool besetzt10 = false;
            bool besetzt11 = false;

            if (WeichenListe.GetWeiche("Weiche80").Abzweig) besetzt8 = besetzt;
            else if(WeichenListe.GetWeiche("Weiche81").Abzweig) besetzt9 = besetzt;
            else if (WeichenListe.GetWeiche("Weiche82").Abzweig) besetzt10 = besetzt;
            else besetzt11 = besetzt;


            MeldeZustand zustand11 = ErrechneZustand(besetzt11, FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten11_nach_Eingleisen", "Schatten11_nach_Schatten1", "Schatten11_nach_Schatten2", "Schatten11_nach_Schatten3", "Schatten11_nach_Schatten4", "Schatten11_nach_Schatten5", "Schatten11_nach_Schatten6", "Schatten11_nach_Schatten7" }), new List<Fahrstrasse>()); //Gleis8
            MeldeZustand zustand10 = ErrechneZustand(besetzt10, FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten10_nach_Eingleisen", "Schatten10_nach_Schatten1", "Schatten10_nach_Schatten2", "Schatten10_nach_Schatten3", "Schatten10_nach_Schatten4", "Schatten10_nach_Schatten5", "Schatten10_nach_Schatten6", "Schatten10_nach_Schatten7" }), new List<Fahrstrasse>()); //Gleis9
            MeldeZustand zustand9 = ErrechneZustand(besetzt9, FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten9_nach_Eingleisen", "Schatten9_nach_Schatten1", "Schatten9_nach_Schatten2", "Schatten9_nach_Schatten3", "Schatten9_nach_Schatten4", "Schatten9_nach_Schatten5", "Schatten9_nach_Schatten6", "Schatten9_nach_Schatten7" }), new List<Fahrstrasse>()); //Gleis10
            MeldeZustand zustand8 = ErrechneZustand(besetzt8, FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten8_nach_Eingleisen", "Schatten8_nach_Schatten1", "Schatten8_nach_Schatten2", "Schatten8_nach_Schatten3", "Schatten8_nach_Schatten4", "Schatten8_nach_Schatten5", "Schatten8_nach_Schatten6", "Schatten8_nach_Schatten7" }), new List<Fahrstrasse>()); //Gleis11

            GleisbildZeichnung.ZeichneSchaltbild(zustand8, Schatten8_Ausf1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand8, zustand9, Schatten8_Ausf2);

            GleisbildZeichnung.ZeichneSchaltbild(zustand9, zustand10, Schatten9_Ausf1);

            GleisbildZeichnung.ZeichneSchaltbild(zustand10, Schatten10_Ausf1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand10, zustand11, Schatten10_Ausf2);

            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand11, zustand10, Schatten11_Ausf5);
        }
        
        #endregion
        #region Block (sonder)
        private void UpdateGleisbild_Block5_Block9(bool besetzt_Block5, List<Fahrstrasse> Fahrstrasse_links_Block5, List<Fahrstrasse> Fahrstrasse_gegen_Block5, 
                                                   bool besetzt_Block9, List<Fahrstrasse> Fahrstrasse_links_Block9, List<Fahrstrasse> Fahrstrasse_gegen_Block9)
        {
            MeldeZustand Zustand_Block5 = ErrechneZustand(besetzt_Block5, Fahrstrasse_links_Block5, Fahrstrasse_gegen_Block5);
            MeldeZustand Zustand_Block9 = ErrechneZustand(besetzt_Block9, Fahrstrasse_links_Block9, Fahrstrasse_gegen_Block9);
            GleisbildZeichnung.ZeichneSchaltbild(Zustand_Block5, Zustand_Block9,Block5_Block9);
        }
        #endregion

        #region Weichen Gleisfeldumgebung
        private void UpdateGleisbild_Weiche1()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche1.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche1);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche1_Gl1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche1_Gl1);
            }

        }
        private void UpdateGleisbild_Weiche2()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche2.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche2);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche2_Gleis);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche2_Gleis);               
            }

        }
        private void UpdateGleisbild_Weiche3()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche3");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche3.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche3);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche3_Gleis);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche3_Gleis);
            }
        }
        private void UpdateGleisbild_Weiche4()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche4");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche4.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche4);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche4_Gl1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche4_Gl1);
            }

        }
        private void UpdateGleisbild_Weiche5()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche5");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche5.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche5);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche5_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche5_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche6()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche6");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche6.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche6);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche6_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche6_Gleis3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche6_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche6_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche6_Gleis3);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche6_Gleis1);
            }

        }
        private void UpdateGleisbild_Weiche8()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche8");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche8.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche8);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche8_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche8_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche21()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche21");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche21.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche21);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche21_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche21_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche23()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche23");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche23.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche23);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche23_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche23_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche25()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche25");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche25.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche25);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche25_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche25_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche26()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche26");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche26.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche26);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche26_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche26_Gleis1);
            }
        }
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
        private void UpdateGleisbild_Weiche52()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche52");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche52.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche52); 
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche52_Gleis3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche52_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche52_Gleis3);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche52_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche53()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche53");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche53.Tag.ToString().EndsWith("_Gegen")); 
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche53);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche53_Gleis0);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche53_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche53_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche53_Gleis3);

            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche53_Gleis0);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche53_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche53_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche53_Gleis3);
            }
        }
        private void UpdateGleisbild_Weiche60()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche60");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche60.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche60);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche60_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche60_Gleis1);
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
        private void UpdateGleisbild_Weiche74()
        {
            Weiche weiche74 = WeichenListe.GetWeiche("Weiche74");
            if (weiche74 == null) return;
            Weiche weiche92 = WeichenListe.GetWeiche("Weiche92");
            if (weiche92 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche74, Weiche74.Tag.ToString().EndsWith("_Gegen"));
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche92, Weiche92.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche74, Weiche74);
            if (weiche74.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche74_1);
                if(weiche92.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, WeicheEcke74_92);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand2, WeicheEcke74_92);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche74_1);
                if (weiche92.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, WeicheEcke74_92);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, meldeZustand2, WeicheEcke74_92);
            }
        }
        private void UpdateGleisbild_Weiche75()
        {
            Weiche weiche75 = WeichenListe.GetWeiche("Weiche75");
            if (weiche75 == null) return;
            Weiche weiche91 = WeichenListe.GetWeiche("Weiche91");
            if (weiche91 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche75, Weiche75.Tag.ToString().EndsWith("_Gegen"));
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche91, Weiche91.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche75, Weiche75);
            if (weiche75.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche75_1);
                if (weiche91.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, WeicheEcke75_91);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand2, WeicheEcke75_91);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche75_1);
                if (weiche91.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, WeicheEcke75_91);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, meldeZustand2, WeicheEcke75_91);
            }
        }
        private void UpdateGleisbild_Weiche76()
        {
            Weiche weiche76 = WeichenListe.GetWeiche("Weiche76");
            if (weiche76 == null) return;
            Weiche weiche90 = WeichenListe.GetWeiche("Weiche90");
            if (weiche90 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche76, Weiche76.Tag.ToString().EndsWith("_Gegen"));
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche90, Weiche90.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche76, Weiche76);
            if (weiche76.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche76_1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche76_2);
                if (!weiche90.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, FreiesGleis, WeicheEcke76_90);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand2, WeicheEcke76_90);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche76_1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche76_2);
                if (!weiche90.Abzweig)
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, WeicheEcke76_90);
                else
                    GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, meldeZustand2, WeicheEcke76_90);
            }
        }
        private void UpdateGleisbild_Weiche80()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche80");
            if (weiche == null) return;
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche80);
        }

        private void UpdateGleisbild_Weiche81()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche81");
            if (weiche == null) return;
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche81);

            Weiche weiche80 = WeichenListe.GetWeiche("Weiche80");
            Weiche weiche81 = WeichenListe.GetWeiche("Weiche81");
            if (weiche80 == null) return;
            if (weiche81 == null) return;
            if (weiche81.Abzweig)
            {
                MeldeZustand meldeZustand = new MeldeZustand(weiche80, Weiche80.Tag.ToString().EndsWith("_Gegen"));
                GleisbildZeichnung.ZeichneSchaltbild(weiche81, meldeZustand, Weiche81);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(weiche81, FreiesGleis, Weiche81);
            }
        }

        private void UpdateGleisbild_Weiche82()
        {
            Weiche weiche82 = WeichenListe.GetWeiche("Weiche82");
            Weiche weiche81 = WeichenListe.GetWeiche("Weiche81");
            if (weiche82 == null) return;
            if (weiche81 == null) return;
            if (weiche81.Abzweig)
            {
                MeldeZustand meldeZustand = new MeldeZustand(weiche81, Weiche81.Tag.ToString().EndsWith("_Gegen"));
                GleisbildZeichnung.ZeichneSchaltbild(weiche82, meldeZustand, Weiche82);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(weiche82, FreiesGleis, Weiche82);
            }
        }

        private void UpdateGleisbild_Weiche90()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche90");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche90.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche90);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche90_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche90_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche91()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche91");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche91.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche91);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche91_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche91_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche92()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche92");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, Weiche92.Tag.ToString().EndsWith("_Gegen"));
            GleisbildZeichnung.ZeichneSchaltbild(weiche, Weiche92);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche92_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche92_Gleis2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche92_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche92_Gleis2);
            }
        }

        #endregion
        #region DKW Gleisfeldumgebung
        private void UpdateGleisbild_WeicheDKW7_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW7_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW7.Tag.ToString().EndsWith("_Gegen"));
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, DKW7);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche7_Gleis2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche7_Gleis2);
            }
        }
        private void UpdateGleisbild_WeicheDKW7_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW7_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW7.Tag.ToString().EndsWith("_Gegen"));
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche7_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche7_Gleis1);
            }
        }     
        private void UpdateGleisbild_WeicheDKW9_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW9_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW9.Tag.ToString().EndsWith("_Gegen"));
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, DKW9);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche9_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche9_Gleis4);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche9_Gleis1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche9_Gleis4);
            }

        }
        private void UpdateGleisbild_WeicheDKW9_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW9_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW9.Tag.ToString().EndsWith("_Gegen"));
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, Weiche9_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche9_Gleis3);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, Weiche9_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche9_Gleis3);
            }
        }       
        private void UpdateGleisbild_KW22_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("KW22_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, KW22.Tag.ToString().EndsWith("_Gegen"));
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, KW22);
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, FreiesGleis, KW22_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, KW22_Gleis3);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, FreiesGleis, KW22_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, KW22_Gleis3);
            }

        }
        private void UpdateGleisbild_KW22_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("KW22_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, KW22.Tag.ToString().EndsWith("_Gegen"));
            
            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, KW22_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, KW22_Gleis1);
            }
        }       
        private void UpdateGleisbild_DKW24_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW24_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW24.Tag.ToString().EndsWith("_Gegen"));
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            GleisbildZeichnung.ZeichneSchaltbild(weiche, DKW_2nd, DKW24);

            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, DKW24_Gleis2);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, DKW24_Gleis2);
            }
        }
        private void UpdateGleisbild_DKW24_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW24_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, DKW24.Tag.ToString().EndsWith("_Gegen"));

            if (weiche.Abzweig)
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, FreiesGleis, DKW24_Gleis1);
            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, meldeZustand, DKW24_Gleis1);
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
                fahrstrasse.SetFahrstrasseRichtung(WeichenListe.Liste);
                //Prüfen ob alle Weichen der Fahrstraßen richtig geschaltet sind
                if (fahrstrasse.CheckFahrstrassePos(WeichenListe.Liste) == false) //Noch nicht alle Weichen gestellt
                {
                    if (Betriebsbereit) fahrstrasse.SetFahrstrasse(WeichenListe.Liste, z21Start);
                }
                else //Alle Weichen in richtiger Stellung
                {
                    //Fahrstraße als aktiviert kennzeichnen
                    fahrstrasse.AktiviereFahrstasse(WeichenListe.Liste);
                    //Jede Weiche in der Fahrstraßenliste durchlaufen
                    foreach (Weiche Fahrstrassenweiche in fahrstrasse.Fahrstr_Weichenliste)
                    {
                        Weiche weiche = WeichenListe.GetWeiche(Fahrstrassenweiche.Name);   //Weiche in Globale Liste suchen
                        if (weiche == null) return;   //Weichen nicht gefunden - Funktion abbrechen
                        UpdateWeicheImGleisplan(weiche);  //Weichenbild aktualisieren
                    }

                    //Weichen zyklisch nochmal schalten um hängenbleiben zu vermeiden
                    if (Betriebsbereit) fahrstrasse.ControlSetFahrstrasse(WeichenListe.Liste, z21Start);
                }
            }
            // Wechseln auf den sicheren Zustand
            if ((SafeStatusAlt == false) && (fahrstrasse.Safe == true))
            {
                // Signal nur Schalten wenn die Option Autoschaltung über Fahrstraße aktiv ist
                if (fahrstrasse.Fahrstr_Sig.Name != null)
                {
                    if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) AutoSignalUpdate(fahrstrasse.Fahrstr_Sig.Name);
                }
                
            }
        }
        /// <summary>
        /// Fahrstraßen im Bild zeichnen
        /// </summary>
        private void FahrstrasseBildUpdate()
        {
            UpdateWeichenBelegung();

            #region Hbf
            //Hauptbahnhof - Linker Teil der Gleise
            UpdateGleisbild_GL1_links(BelegtmelderListe.GetBelegtStatus("W6") && WeichenListe.GetWeiche("Weiche6").Abzweig,  FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_Block1" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis1" }));
            UpdateGleisbild_GL2_links(BelegtmelderListe.GetBelegtStatus("W6") && !WeichenListe.GetWeiche("Weiche6").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis2_nach_Block1" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis2" }));
            UpdateGleisbild_GL3_links(BelegtmelderListe.GetBelegtStatus("W5") && !WeichenListe.GetWeiche("Weiche5").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis3" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis3_nach_Block1" }));
            UpdateGleisbild_GL4_links(BelegtmelderListe.GetBelegtStatus("DKW7_W8") && !WeichenListe.GetWeiche("DKW7_2").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis4" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis4_nach_Block1" }));
            UpdateGleisbild_GL5_links(BelegtmelderListe.GetBelegtStatus("DKW9") && !WeichenListe.GetWeiche("DKW9_2").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis5" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis5_nach_Block1" }));
            UpdateGleisbild_GL6_links(BelegtmelderListe.GetBelegtStatus("DKW9") && WeichenListe.GetWeiche("DKW9_2").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis6" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis6_nach_Block1" }));

            //Hauptbahnhof - Zentrum
            UpdateGleisbild_Gl1_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf1_Halt_L"));
            UpdateGleisbild_Gl1_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf1_Halt_R"));
            UpdateGleisbild_Gl1_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf1"));

            UpdateGleisbild_Gl2_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf2_Halt_L"));
            UpdateGleisbild_Gl2_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf2_Halt_R"));
            UpdateGleisbild_Gl2_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf2"));

            UpdateGleisbild_Gl3_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf3_Halt_L"));
            UpdateGleisbild_Gl3_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf3_Halt_R"));
            UpdateGleisbild_Gl3_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf3"));

            UpdateGleisbild_Gl4_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf4_Halt_L"));
            UpdateGleisbild_Gl4_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf4_Halt_R"));
            UpdateGleisbild_Gl4_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf4"));

            UpdateGleisbild_Gl5_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf5_Halt_L"));
            UpdateGleisbild_Gl5_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf5_Halt_R"));
            UpdateGleisbild_Gl5_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf5"));

            UpdateGleisbild_Gl6_Halt_links(BelegtmelderListe.GetBelegtStatus("HBf6_Halt_L"));
            UpdateGleisbild_Gl6_Halt_rechts(BelegtmelderListe.GetBelegtStatus("HBf6_Halt_R"));
            UpdateGleisbild_Gl6_Zentrum(BelegtmelderListe.GetBelegtStatus("HBf6"));

            //Hauptbahnhof - Rechter Teil der Gleise
            UpdateGleisbild_GL1_rechts(BelegtmelderListe.GetBelegtStatus("W26") && WeichenListe.GetWeiche("Weiche26").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis1", "TunnelInnen_nach_Gleis1" }), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen" , "Gleis1_nach_TunnelInnen" }));
            UpdateGleisbild_GL2_rechts(BelegtmelderListe.GetBelegtStatus("W26") && !WeichenListe.GetWeiche("Weiche26").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis2", "TunnelInnen_nach_Gleis2" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis2_nach_TunnelAussen" , "Gleis2_nach_TunnelInnen" }));
            UpdateGleisbild_GL3_rechts(BelegtmelderListe.GetBelegtStatus("W25") && !WeichenListe.GetWeiche("Weiche25").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis3_nach_TunnelAussen", "Gleis3_nach_TunnelInnen" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis3" , "TunnelInnen_nach_Gleis3" }));
            UpdateGleisbild_GL4_rechts(BelegtmelderListe.GetBelegtStatus("DKW24_W23") && !WeichenListe.GetWeiche("DKW24_2").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis4_nach_TunnelAussen", "Gleis4_nach_TunnelInnen" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis4" , "TunnelInnen_nach_Gleis4" }));
            UpdateGleisbild_GL5_rechts(BelegtmelderListe.GetBelegtStatus("KW22") && !WeichenListe.GetWeiche("KW22_2").Abzweig, FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis5_nach_TunnelAussen", "Gleis5_nach_TunnelInnen" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis5" , "TunnelInnen_nach_Gleis5" }));
            UpdateGleisbild_GL6_rechts(BelegtmelderListe.GetBelegtStatus("HBf6_Halt_R"), FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis6_nach_TunnelAussen", "Gleis6_nach_TunnelInnen" }) , FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis6" , "TunnelInnen_nach_Gleis6" }));
            #endregion
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

            UpdateGleisbild_Schatten_Gl8(BelegtmelderListe.GetBelegtStatus("Schatten_Gl8"));
            UpdateGleisbild_Schatten_Gl8_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl8_Halt"));

            UpdateGleisbild_Schatten_Gl9(BelegtmelderListe.GetBelegtStatus("Schatten_Gl9"));
            UpdateGleisbild_Schatten_Gl9_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl9_Halt"));

            UpdateGleisbild_Schatten_Gl10(BelegtmelderListe.GetBelegtStatus("Schatten_Gl10"));
            UpdateGleisbild_Schatten_Gl10_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl10_Halt"));

            UpdateGleisbild_Schatten_Gl11(BelegtmelderListe.GetBelegtStatus("Schatten_Gl11"));
            UpdateGleisbild_Schatten_Gl11_Halt(BelegtmelderListe.GetBelegtStatus("Schatten_Gl11_Halt"));

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


            //Gleise im Block 1 aktualisieren
            UpdateGleisbild_Weiche6(BelegtmelderListe.GetBelegtStatus("W6"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_Block1", "Gleis2_nach_Block1" }),//Mit Fahrtrichtung
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis1", "Block2_nach_Gleis2" }));//Gegen Fahrtrichtung

            UpdateGleisbild_Block1a(BelegtmelderListe.GetBelegtStatus("Block1_a"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_Block1", "Gleis2_nach_Block1", "Gleis3_nach_Block1", //Mit Fahrtrichtung
                                                                                    "Gleis4_nach_Block1", "Gleis5_nach_Block1", "Gleis6_nach_Block1" }),
                                    new List<Fahrstrasse>()); //Nie gegen Fahrtrichtung

            UpdateGleisbild_Block1b(BelegtmelderListe.GetBelegtStatus("Block1_b"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_Block1", "Gleis2_nach_Block1", "Gleis3_nach_Block1", //Mit Fahrtrichtung
                                                                                    "Gleis4_nach_Block1", "Gleis5_nach_Block1", "Gleis6_nach_Block1" }),
                                    new List<Fahrstrasse>()); //Nie gegen Fahrtrichtung
            UpdateGleisbild_Block1_Halt(BelegtmelderListe.GetBelegtStatus("Block1_Halt"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_Block1", "Gleis2_nach_Block1", "Gleis3_nach_Block1", //Nach links
                                                                                    "Gleis4_nach_Block1", "Gleis5_nach_Block1", "Gleis6_nach_Block1" }),
                                    new List<Fahrstrasse>()); //Nie gegen Fahrtrichtung          
            UpdateGleisbild_Block2(BelegtmelderListe.GetBelegtStatus("Block2"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Block1_nach_Block2", "Block9_nach_Block2" }), //Nach Links
                                    new List<Fahrstrasse>()); //Nie gegen Fahrtrichtung
            UpdateGleisbild_Block2_Halt(BelegtmelderListe.GetBelegtStatus("Block2_Halt"), //Besetzt
                                    FahrstrassenListe.GetFahrstrasse(new string[] { "Block1_nach_Block2", "Block9_nach_Block2" }), //Nach Links
                                    new List<Fahrstrasse>()); //Nie gegen Fahrtrichtung
            //Gleise im Block 2 aktualisieren
            UpdateGleisbild_Weiche5(BelegtmelderListe.GetBelegtStatus("W5"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block2_nach_Gleis3", "Block2_nach_Gleis4", "Block2_nach_Gleis5", "Block2_nach_Gleis6"}),
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis3_nach_Block1", "Gleis4_nach_Block1", "Gleis5_nach_Block1", "Gleis6_nach_Block1"}));
            //Gleise im Block 3 aktualisieren
            UpdateGleisbild_Weiche25(BelegtmelderListe.GetBelegtStatus("W25"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen" }),
                FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis3", "TunnelAussen_nach_Gleis4", "TunnelAussen_nach_Gleis5", "TunnelAussen_nach_Gleis6", "TunnelInnen_nach_Gleis3", "TunnelInnen_nach_Gleis4", "TunnelInnen_nach_Gleis5", "TunnelInnen_nach_Gleis6" }));
            UpdateGleisbild_Block3(BelegtmelderListe.GetBelegtStatus("Block3"), //Besetzt                             
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis3_nach_TunnelAussen", "Gleis4_nach_TunnelAussen", "Gleis5_nach_TunnelAussen", "Gleis6_nach_TunnelAussen", "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen", "Gleis3_nach_TunnelInnen", "Gleis4_nach_TunnelInnen", "Gleis5_nach_TunnelInnen", "Gleis6_nach_TunnelInnen"}),
                                    new List<Fahrstrasse>());
            //Gleise im Block 4 aktualisieren                        
            UpdateGleisbild_Block4(BelegtmelderListe.GetBelegtStatus("Block4"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis1", "TunnelAussen_nach_Gleis2", "TunnelAussen_nach_Gleis3", "TunnelAussen_nach_Gleis4", "TunnelAussen_nach_Gleis5", "TunnelAussen_nach_Gleis6", "TunnelInnen_nach_Gleis1", "TunnelInnen_nach_Gleis2", "TunnelInnen_nach_Gleis3", "TunnelInnen_nach_Gleis4", "TunnelInnen_nach_Gleis5", "TunnelInnen_nach_Gleis6" }),
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Weiche26(BelegtmelderListe.GetBelegtStatus("W26"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "TunnelAussen_nach_Gleis1", "TunnelAussen_nach_Gleis2", "TunnelInnen_nach_Gleis1", "TunnelInnen_nach_Gleis2" }), //Nach links
                FahrstrassenListe.GetFahrstrasse(new string[] { "Gleis1_nach_TunnelAussen", "Gleis2_nach_TunnelAussen", "Gleis1_nach_TunnelInnen", "Gleis2_nach_TunnelInnen" }));//Nach rechts
            UpdateGleisbild_Block5a(BelegtmelderListe.GetBelegtStatus("Block5_a"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block1_nach_Block5" }) ,  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block5b(BelegtmelderListe.GetBelegtStatus("Block5_b"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block1_nach_Block5" }),  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block5_Halt(BelegtmelderListe.GetBelegtStatus("Block5_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block1_nach_Block5" }),  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block6(BelegtmelderListe.GetBelegtStatus("Block6"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block5_nach_Block6", "Block8_nach_Block6" }),  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block6_Halt(BelegtmelderListe.GetBelegtStatus("Block6_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Block5_nach_Block6", "Block8_nach_Block6" }),  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block7(BelegtmelderListe.GetBelegtStatus("Block7"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten11_nach_Eingleisen", "Schatten11_nach_Schatten1", "Schatten11_nach_Schatten2", "Schatten11_nach_Schatten3", "Schatten11_nach_Schatten4", "Schatten11_nach_Schatten5", "Schatten11_nach_Schatten6", "Schatten11_nach_Schatten7",
                                                                "Schatten10_nach_Eingleisen", "Schatten10_nach_Schatten1", "Schatten10_nach_Schatten2", "Schatten10_nach_Schatten3", "Schatten10_nach_Schatten4", "Schatten10_nach_Schatten5", "Schatten10_nach_Schatten6", "Schatten10_nach_Schatten7",
                                                                "Schatten9_nach_Eingleisen", "Schatten9_nach_Schatten1", "Schatten9_nach_Schatten2", "Schatten9_nach_Schatten3", "Schatten9_nach_Schatten4", "Schatten9_nach_Schatten5", "Schatten9_nach_Schatten6", "Schatten9_nach_Schatten7",
                                                                "Schatten8_nach_Eingleisen", "Schatten8_nach_Schatten1", "Schatten8_nach_Schatten2", "Schatten8_nach_Schatten3", "Schatten8_nach_Schatten4", "Schatten8_nach_Schatten5", "Schatten8_nach_Schatten6", "Schatten8_nach_Schatten7"}),  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block8(BelegtmelderListe.GetBelegtStatus("Block8"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Eingleisen_nach_Block8", "Schatten1_nach_Block8" }),  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block8_Halt(BelegtmelderListe.GetBelegtStatus("Block8_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Eingleisen_nach_Block8", "Schatten1_nach_Block8" }),  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block9(BelegtmelderListe.GetBelegtStatus("Block9"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten1_nach_Block9", "Schatten2_nach_Block9", "Schatten3_nach_Block9", "Schatten4_nach_Block9", "Schatten5_nach_Block9", "Schatten6_nach_Block9", "Schatten7_nach_Block9"}),//nach rechts
                                    new List<Fahrstrasse>());  //nie nach rechts
            UpdateGleisbild_Block9_Halt(BelegtmelderListe.GetBelegtStatus("Block9_Halt"), //Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten1_nach_Block9", "Schatten2_nach_Block9", "Schatten3_nach_Block9", "Schatten4_nach_Block9", "Schatten5_nach_Block9", "Schatten6_nach_Block9", "Schatten7_nach_Block9"}),//nach rechts
                                    new List<Fahrstrasse>());  //nie nach rechts
                                     


            UpdateGleisbild_Weiche1();  //Umfeld um Weiche 1
            UpdateGleisbild_Weiche2();  //Umfeld um Weiche 2
            UpdateGleisbild_Weiche3();  //Umfeld um Weiche 3
            UpdateGleisbild_Weiche4();  //Umfeld um Weiche 4
            UpdateGleisbild_Weiche5();  //Umfeld um Weiche 5
            UpdateGleisbild_Weiche6();  //Umfeld um Weiche 6
            UpdateGleisbild_WeicheDKW7_1();//Umfeld um Doppelkreuzungweiche 7
            UpdateGleisbild_WeicheDKW7_2();
            UpdateGleisbild_Weiche8();  //Umfeld um Weiche 8
            UpdateGleisbild_WeicheDKW9_1();//Umfeld um Doppelkreuzungweiche 7
            UpdateGleisbild_WeicheDKW9_2();

            UpdateGleisbild_Weiche21();     //Umfeld um Weiche 21
            UpdateGleisbild_KW22_1();       //Umfeld um Kreuzungweiche 7
            UpdateGleisbild_KW22_2();
            UpdateGleisbild_Weiche23();     //Umfeld um Weiche 23
            UpdateGleisbild_DKW24_1();      //Umfeld um Doppelkreuzungweiche 7
            UpdateGleisbild_DKW24_2();
            UpdateGleisbild_Weiche25();     //Umfeld um Weiche 25
            UpdateGleisbild_Weiche26();     //Umfeld um Weiche 26
            UpdateGleisbild_Weiche27();     //Umfeld um Weiche 27
            UpdateGleisbild_Weiche28();     //Umfeld um Weiche 28
            UpdateGleisbild_Weiche29();     //Umfeld um Weiche 29
            UpdateGleisbild_Weiche30();     //Umfeld um Weiche 30
            UpdateGleisbild_Weiche50();     //Umfeld um Weiche 50
            UpdateGleisbild_Weiche51();     //Umfeld um Weiche 51
            UpdateGleisbild_Weiche52();     //Umfeld um Weiche 52
            UpdateGleisbild_Weiche53();     //Umfeld um Weiche 53

            UpdateGleisbild_Weiche60();     //Umfeld um Weiche 60
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
            UpdateGleisbild_Weiche74();     //Umfeld um Weiche 74
            UpdateGleisbild_Weiche75();     //Umfeld um Weiche 75
            UpdateGleisbild_Weiche76();     //Umfeld um Weiche 76

            UpdateGleisbild_Weiche80();     //Umfeld um Weiche 80
            UpdateGleisbild_Weiche81();     //Umfeld um Weiche 81
            UpdateGleisbild_Weiche82();     //Umfeld um Weiche 82

            UpdateGleisbild_Weiche90();     //Umfeld um Weiche 90
            UpdateGleisbild_Weiche91();     //Umfeld um Weiche 91
            UpdateGleisbild_Weiche92();     //Umfeld um Weiche 92

            UpdateKreuzung(BelegtmelderListe.GetBelegtStatus("SchattenAusfahrt"), //Besetzt
                            new List<Fahrstrasse> { FahrstrassenListe.GetFahrstrasse("Block8_nach_Block6") },  //Block8
                FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten1_nach_Block9", "Schatten2_nach_Block9", "Schatten3_nach_Block9", "Schatten4_nach_Block9", "Schatten5_nach_Block9", "Schatten6_nach_Block9", "Schatten7_nach_Block9"})); //Block9

            UpdateGleisbild_Block5_Block9(BelegtmelderListe.GetBelegtStatus("Block5_b"), //Block5: Besetzt
                        new List<Fahrstrasse> { FahrstrassenListe.GetFahrstrasse("Block1_nach_Block5") },  //Block6: Nach links
                        new List<Fahrstrasse>(), //Block5: nie nach rechts
                        BelegtmelderListe.GetBelegtStatus("Block9"), //Block9: Besetzt
                FahrstrassenListe.GetFahrstrasse(new string[] { "Schatten1_nach_Block9", "Schatten2_nach_Block9", "Schatten3_nach_Block9", "Schatten4_nach_Block9", "Schatten5_nach_Block9", "Schatten6_nach_Block9", "Schatten7_nach_Block9"}),//Block9: nie nach rechts
                        new List<Fahrstrasse>());  //Block9: Nach rechts

            UpdateGleisbild_SchattenkleinAusf(BelegtmelderListe.GetBelegtStatus("SchattenMitte1"));

            
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
                    case "W52": SetzeWeichenBelegung("Weiche52", belegtmelder.IstBelegt()); break;
                    case "HBf6_Halt_R": SetzeWeichenBelegung("Weiche21", belegtmelder.IstBelegt()); break;
                    case "SchattenEinfahrt":
                        SetzeWeichenBelegung("Weiche91", false);
                        SetzeWeichenBelegung("Weiche92", false);

                        SetzeWeichenBelegung("Weiche90", belegtmelder.IstBelegt()); if (!WeichenListe.GetWeiche("Weiche90").Abzweig) break;
                        SetzeWeichenBelegung("Weiche91", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche91").Abzweig) break;
                        SetzeWeichenBelegung("Weiche92", belegtmelder.IstBelegt()); break;

                    case "SchattenMitte1":
                        SetzeWeichenBelegung("Weiche81", false);
                        SetzeWeichenBelegung("Weiche82", false);

                        SetzeWeichenBelegung("Weiche80", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche80").Abzweig) break;
                        SetzeWeichenBelegung("Weiche81", belegtmelder.IstBelegt()); if (WeichenListe.GetWeiche("Weiche81").Abzweig) break;
                        SetzeWeichenBelegung("Weiche82", belegtmelder.IstBelegt()); break;

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
