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
        const bool WEST = true;
        const bool OST = false;
        MeldeZustand FreiesGleis = new MeldeZustand(false);
        #region Farben
        private readonly Color Farbe_Gelb = Color.FromArgb(255, 255, 255, 0);
        private readonly Color Farbe_Gruen = Color.FromArgb(255, 0, 255, 0);
        private readonly Color Farbe_Rot = Color.FromArgb(255, 255, 0, 0);
        private readonly Color Farbe_Grau = Color.FromArgb(255, 128, 128, 128);
        private readonly Color Farbe_Weis = Color.FromArgb(255, 255, 255, 255);
        #endregion
        #region Fahrstraßen Instanzen
        private Fahrstrasse Gleis1_nach_Block1 { set; get; }
        private Fahrstrasse Gleis2_nach_Block1 { set; get; }
        private Fahrstrasse Gleis3_nach_Block1 { set; get; }
        private Fahrstrasse Gleis4_nach_Block1 { set; get; }
        private Fahrstrasse Gleis5_nach_Block1 { set; get; }
        private Fahrstrasse Gleis6_nach_Block1 { set; get; }

        private Fahrstrasse Block2_nach_Gleis1 { set; get; }
        private Fahrstrasse Block2_nach_Gleis2 { set; get; }
        private Fahrstrasse Block2_nach_Gleis3 { set; get; }
        private Fahrstrasse Block2_nach_Gleis4 { set; get; }
        private Fahrstrasse Block2_nach_Gleis5 { set; get; }
        private Fahrstrasse Block2_nach_Gleis6 { set; get; }
        private Fahrstrasse Gleis1_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis2_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis3_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis4_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis5_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis6_nach_rechts1 { set; get; }
        private Fahrstrasse Gleis1_nach_rechts2 { set; get; }
        private Fahrstrasse Gleis2_nach_rechts2 { set; get; }
        private Fahrstrasse Gleis3_nach_rechts2 { set; get; }
        private Fahrstrasse Gleis4_nach_rechts2 { set; get; }
        private Fahrstrasse Gleis5_nach_rechts2 { set; get; }
        private Fahrstrasse Gleis6_nach_rechts2 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis1 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis2 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis3 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis4 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis5 { set; get; }
        private Fahrstrasse Rechts1_nach_Gleis6 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis1 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis2 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis3 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis4 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis5 { set; get; }
        private Fahrstrasse Rechts2_nach_Gleis6 { set; get; }

        private Fahrstrasse Block1_nach_Schatten { set; get; }
        private Fahrstrasse Block1_nach_Block2 { set; get; }

        private Fahrstrasse Block1_nach_Block5 { set; get; }
        private Fahrstrasse Block5_nach_Block6 { set; get; }
        private Fahrstrasse Block8_nach_Block6 { set; get; }
        private Fahrstrasse Block9_nach_Block2 { set; get; }

        private Fahrstrasse Block6_nach_Schatten8 { set; get; }
        private Fahrstrasse Block6_nach_Schatten9 { set; get; }
        private Fahrstrasse Block6_nach_Schatten10 { set; get; }
        private Fahrstrasse Block6_nach_Schatten11 { set; get; }

        private Fahrstrasse Schatten8_nach_Block7 { set; get; }
        private Fahrstrasse Schatten9_nach_Block7 { set; get; }
        private Fahrstrasse Schatten10_nach_Block7 { set; get; }
        private Fahrstrasse Schatten11_nach_Block7 { set; get; }

        private Fahrstrasse Block7_nach_Schatten0 { set; get; }
        private Fahrstrasse Block7_nach_Schatten1 { set; get; }
        private Fahrstrasse Block7_nach_Schatten2 { set; get; }
        private Fahrstrasse Block7_nach_Schatten3 { set; get; }
        private Fahrstrasse Block7_nach_Schatten4 { set; get; }
        private Fahrstrasse Block7_nach_Schatten5 { set; get; }
        private Fahrstrasse Block7_nach_Schatten6 { set; get; }
        private Fahrstrasse Block7_nach_Schatten7 { set; get; }

        private Fahrstrasse Schatten0_nach_Block8 { set; get; }
        private Fahrstrasse Schatten1_nach_Block8 { set; get; }
        private Fahrstrasse Schatten1_nach_Block9 { set; get; }
        private Fahrstrasse Schatten2_nach_Block9 { set; get; }
        private Fahrstrasse Schatten3_nach_Block9 { set; get; }
        private Fahrstrasse Schatten4_nach_Block9 { set; get; }
        private Fahrstrasse Schatten5_nach_Block9 { set; get; }
        private Fahrstrasse Schatten6_nach_Block9 { set; get; }
        private Fahrstrasse Schatten7_nach_Block9 { set; get; }

        #endregion
        
        
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
            UpdateSchalter();
            FahrstrasseBildUpdate();
        }
        #region Fahrstarßen Setup
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            SetupGleis1_nach_Block1();
            SetupGleis2_nach_Block1();
            SetupGleis3_nach_Block1();
            SetupGleis4_nach_Block1();
            SetupGleis5_nach_Block1();
            SetupGleis6_nach_Block1();

            SetupBlock2_nach_Gleis1();
            SetupBlock2_nach_Gleis2();
            SetupBlock2_nach_Gleis3();
            SetupBlock2_nach_Gleis4();
            SetupBlock2_nach_Gleis5();
            SetupBlock2_nach_Gleis6();

            SetupGleis1_nach_rechts1();
            SetupGleis2_nach_rechts1();
            SetupGleis3_nach_rechts1();
            SetupGleis4_nach_rechts1();
            SetupGleis5_nach_rechts1();
            SetupGleis6_nach_rechts1();
            SetupGleis1_nach_rechts2();
            SetupGleis2_nach_rechts2();
            SetupGleis3_nach_rechts2();
            SetupGleis4_nach_rechts2();
            SetupGleis5_nach_rechts2();
            SetupGleis6_nach_rechts2();

            SetupRechts1_nach_Gleis1();
            SetupRechts1_nach_Gleis2();
            SetupRechts1_nach_Gleis3();
            SetupRechts1_nach_Gleis4();
            SetupRechts1_nach_Gleis5();
            SetupRechts1_nach_Gleis6();
            SetupRechts2_nach_Gleis1();
            SetupRechts2_nach_Gleis2();
            SetupRechts2_nach_Gleis3();
            SetupRechts2_nach_Gleis4();
            SetupRechts2_nach_Gleis5();
            SetupRechts2_nach_Gleis6();

            SetupBlock1_nach_Block5();
            SetupBlock1_nach_Block2();
            SetupBlock5_nach_Block6();
            SetupBlock8_nach_Block6();
            SetupBlock9_nach_Block2();

            SetupBlock6_nach_Schatten8();
            SetupBlock6_nach_Schatten9();
            SetupBlock6_nach_Schatten10();
            SetupBlock6_nach_Schatten11();

            SetupSchatten8_nach_Block7();
            SetupSchatten9_nach_Block7();
            SetupSchatten10_nach_Block7();
            SetupSchatten11_nach_Block7();

            SetupBlock7_nach_Schatten0();
            SetupBlock7_nach_Schatten1();
            SetupBlock7_nach_Schatten2();
            SetupBlock7_nach_Schatten3();
            SetupBlock7_nach_Schatten4();
            SetupBlock7_nach_Schatten5();
            SetupBlock7_nach_Schatten6();
            SetupBlock7_nach_Schatten7();

            SetupSchatten0_nachBlock8();
            SetupSchatten1_nachBlock8();
            SetupSchatten1_nachBlock9();
            SetupSchatten2_nachBlock9();
            SetupSchatten3_nachBlock9();
            SetupSchatten4_nachBlock9();
            SetupSchatten5_nachBlock9();
            SetupSchatten6_nachBlock9();
            SetupSchatten7_nachBlock9();
        }
        #region Bahnhof Ausfahrt links
        private void SetupGleis1_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis1_nach_links /////////
            Gleis1_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L1"); 
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis1_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche4").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche6").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupGleis2_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis2_nach_links /////////
            Gleis2_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis2_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche4").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche6").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupGleis3_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis3_nach_links /////////
            Gleis3_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L3");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis3_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_Block1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis4_nach_links /////////
            Gleis4_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L4");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis4_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupGleis5_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis5_nach_links /////////
            Gleis5_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis5_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5");
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche8").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_Block1()
        {
            Weiche weiche;
            ////////// Gleis6_nach_links /////////
            Gleis6_nach_Block1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L6");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_L6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis6_nach_Block1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche8").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block1.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Einfahrt links
        private void SetupBlock2_nach_Gleis1()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis1 /////////
            Block2_nach_Gleis1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche4").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche6").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis2()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis2 /////////
            Block2_nach_Gleis2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche4").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche6").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis3()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis3 /////////
            Block2_nach_Gleis3 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis3.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis4()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis4 /////////
            Block2_nach_Gleis4 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis4.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis5()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis5 /////////
            Block2_nach_Gleis5 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis5.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche8").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis6()
        {
            Weiche weiche;
            ////////// Block2_nach_Gleis6 /////////
            Block2_nach_Gleis6 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Einfahrt_L nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block2_nach_Gleis6.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche3").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche5").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_1").Copy();

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW7_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche8").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW9_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Ausfahrt rechts
        private void SetupGleis1_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis1_nach_rechts1 /////////
            Gleis1_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis1_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis2_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis2_nach_rechts1 /////////
            Gleis2_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis2_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis3_nach_rechts1 /////////
            Gleis3_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R3");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis3_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis4_nach_rechts1 /////////
            Gleis4_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R4");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis4_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis5_nach_rechts1 /////////
            Gleis5_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis5_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_rechts1()
        {
            Weiche weiche;
            ////////// Gleis6_nach_rechts1 /////////
            Gleis6_nach_rechts1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R6");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis6_nach_rechts1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche21").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis1_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis1_nach_rechts2 /////////
            Gleis1_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis1_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis2_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis2_nach_rechts2 /////////
            Gleis2_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis2_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis3_nach_rechts2 /////////
            Gleis3_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R3");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis3_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis4_nach_rechts2 /////////
            Gleis4_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R4");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis4_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis5_nach_rechts2 /////////
            Gleis5_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis5_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_rechts2()
        {
            Weiche weiche;
            ////////// Gleis6_nach_rechts2 /////////
            Gleis6_nach_rechts2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R6");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Ausfahrt_R6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Gleis6_nach_rechts2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche21").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche30").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche50").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Einfahrt rechts
        private void SetupRechts1_nach_Gleis1()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis1 /////////
            Rechts1_nach_Gleis1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis2()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis2 /////////
            Rechts1_nach_Gleis2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis3()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis3 /////////
            Rechts1_nach_Gleis3 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis3.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis4()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis4 /////////
            Rechts1_nach_Gleis4 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis4.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis5()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis5 /////////
            Rechts1_nach_Gleis5 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis5.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis6()
        {
            Weiche weiche;
            ////////// rechts1_nach_Gleis6 /////////
            Rechts1_nach_Gleis6 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts1_nach_Gleis6.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche21").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis1()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis1 /////////
            Rechts2_nach_Gleis1 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis1.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis2()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis2 /////////
            Rechts2_nach_Gleis2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche26").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis3()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis3 /////////
            Rechts2_nach_Gleis3 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis3.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis4()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis4 /////////
            Rechts2_nach_Gleis4 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis4.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis5()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis5 /////////
            Rechts2_nach_Gleis5 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis5.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis6()
        {
            Weiche weiche;
            ////////// rechts2_nach_Gleis6 /////////
            Rechts2_nach_Gleis6 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_RTunnel_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Rechts2_nach_Gleis6.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche51").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche29").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche28").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche27").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche25").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("DKW24_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche23").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_1").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("KW22_2").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche21").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Blockfahrt
        private void SetupBlock1_nach_Block5()
        {
            Weiche weiche;
            ////////// Block1_nach_Block5 /////////
            Block1_nach_Block5 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Block5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Block5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block1_nach_Block5.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche52").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block1_nach_Block5.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock1_nach_Block2()
        {
            Weiche weiche;
            ////////// Block1_nach_Block2 /////////
            Block1_nach_Block2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Block5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Block5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block1_nach_Block2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche52").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche53").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock5_nach_Block6()
        {
            Weiche weiche;
            ////////// Block5_nach_Block6 /////////
            Block5_nach_Block6 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Block6");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Block6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block5_nach_Block6.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche60").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche60 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block5_nach_Block6.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock8_nach_Block6()
        {
            Weiche weiche;
            ////////// Block8_nach_Block6 /////////
            Block8_nach_Block6 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Block8");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Block8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block8_nach_Block6.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche60").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche60 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block8_nach_Block6.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock9_nach_Block2()
        {
            Weiche weiche;
            ////////// Block9_nach_Block2 /////////
            Block9_nach_Block2 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Block2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Block2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block9_nach_Block2.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche53").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block9_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Schatten Einfahrt
        private void SetupBlock6_nach_Schatten8()
        {
            Weiche weiche;
            ////////// Block6_nach Schattenbahnhof Gl. 8 /////////
            Block6_nach_Schatten8 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten_Einf");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten_Einf nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block6_nach_Schatten8.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche90").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche91").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche92").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche92 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten9()
        {
            Weiche weiche;
            ////////// Block6_nach Schattenbahnhof Gl. 9 /////////
            Block6_nach_Schatten9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten_Einf");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten_Einf nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block6_nach_Schatten9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche90").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche91").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche92").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche92 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten10()
        {
            Weiche weiche;
            ////////// Block6_nach Schattenbahnhof Gl. 10 /////////
            Block6_nach_Schatten10 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten_Einf");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten_Einf nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block6_nach_Schatten10.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche90").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten10.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche91").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten10.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten11()
        {
            Weiche weiche;
            ////////// Block6_nach Schattenbahnhof Gl. 11 /////////
            Block6_nach_Schatten11 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten_Einf");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten_Einf nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Block6_nach_Schatten11.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche90").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten11.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Schatten Ausfahrt intern
        private void SetupSchatten11_nach_Block7()
        {
            Weiche weiche;
            ////////// Schattenbahnhof Gl. 11 Ausf/////////
            Schatten11_nach_Block7 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten11");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten11 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten11_nach_Block7.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche80").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche81").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche82").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche82 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten10_nach_Block7()
        {
            Weiche weiche;
            ////////// Schattenbahnhof Gl. 10 Ausf/////////
            Schatten10_nach_Block7 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten10");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten10 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten10_nach_Block7.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche80").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche81").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche82").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche82 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten9_nach_Block7()
        {
            Weiche weiche;
            ////////// Schattenbahnhof Gl. 9 Ausf/////////
            Schatten9_nach_Block7 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten9");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten9 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten9_nach_Block7.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche80").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten9_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche81").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten9_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten8_nach_Block7()
        {
            Weiche weiche;
            ////////// Schattenbahnhof Gl. 9 Ausf/////////
            Schatten8_nach_Block7 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten8");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten8_nach_Block7.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche80").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten8_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

        }
        #endregion
        #region Schatten Einfahrt intern
        private void SetupBlock7_nach_Schatten0()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten0 /////////
            Block7_nach_Schatten0 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten0.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock7_nach_Schatten1()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten1 /////////
            Block7_nach_Schatten1 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten1.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten1.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock7_nach_Schatten2()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten2 /////////
            Block7_nach_Schatten2 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten3()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten3 /////////
            Block7_nach_Schatten3 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche73").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten4()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten4 /////////
            Block7_nach_Schatten4 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche73").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche74").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten5()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten5 /////////
            Block7_nach_Schatten5 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche73").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche74").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche75").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche75 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten6()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten6 /////////
            Block7_nach_Schatten6 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche73").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche74").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche75").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche75 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche76").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche76 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten6.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten7()
        {
            Weiche weiche;
            ////////// Block7_nach_Schatten7 /////////
            Block7_nach_Schatten7 = new Fahrstrasse();
            weiche = WeichenListe.GetWeiche("Weiche70").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche71").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche72").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche73").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche74").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche75").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche75 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche76").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche76 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten7.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Schatten Ausfahrt
        private void SetupSchatten0_nachBlock8()
        {
            Weiche weiche;
            ////////// Schatten0_nachBlock8 /////////
            Schatten0_nach_Block8 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten0");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten0 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten0_nach_Block8.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche68").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche68 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten0_nach_Block8.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupSchatten1_nachBlock8()
        {
            Weiche weiche;
            ////////// Schatten1_nachBlock8 /////////
            Schatten1_nach_Block8 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten1_nach_Block8.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche68").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche68 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block8.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche67").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche67 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block8.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupSchatten1_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten1_nachBlock9 /////////
            Schatten1_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten1");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten1_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche67").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche67 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche66").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche66 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche65").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche65 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche64").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche64 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche63").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche63 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten1_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten2_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten2_nachBlock9 /////////
            Schatten2_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten2");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten2_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche66").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche66 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche65").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche65 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche64").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche64 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche63").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche63 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten2_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten3_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten3_nachBlock9 /////////
            Schatten3_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten3");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten3_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche65").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche65 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten3_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche64").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche64 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten3_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche63").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche63 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten3_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten3_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten3_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten4_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten4_nachBlock9 /////////
            Schatten4_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten4");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten4_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche64").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche64 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten4_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche63").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche63 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten4_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten4_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten4_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten5_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten4_nachBlock9 /////////
            Schatten5_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten5");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten5_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche63").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche63 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten5_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten5_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten5_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten6_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten4_nachBlock9 /////////
            Schatten6_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten6");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten6_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche62").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche62 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten6_nach_Block9.Fahrstr_Weichenliste.Add(weiche);

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten6_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten7_nachBlock9()
        {
            Weiche weiche;
            ////////// Schatten4_nachBlock9 /////////
            Schatten7_nach_Block9 = new Fahrstrasse();
            Signal signal = SignalListe.GetSignal("Signal_Schatten7");
            if (signal == null) { MessageBox.Show("Schwerer Error: Signal_Schatten7 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Schatten7_nach_Block9.Fahrstr_Sig = signal;

            weiche = WeichenListe.GetWeiche("Weiche61").Copy();
            if (weiche == null) { MessageBox.Show("Schwerer Error: Weiche61 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten7_nach_Block9.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion

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
                richtung = WEST;
                fahrstrasseAktiv = true;
                if (safe_west == 1) sicher = true; //Fahrstraße sicher?
            }
            //Fahrstraße nach osten aktiv
            if (aktiv_ost == 1) 
            { 
                richtung = OST;
                fahrstrasseAktiv = true;
                if (safe_ost == 1) sicher = true; //Fahrstraße sicher?
            }

            //Generiere Statuskonstrukt
            return new MeldeZustand(besetzt, fahrstrasseAktiv, sicher, richtung);
        }
        
        
        /// <summary>
        /// Involke-Funktion. Verhindert Fehlermeldung beim gleichzeitigen Zugreifen auf ein Bild
        /// </summary>
        /// <param name="img">Neues Bild zum Anzeiegen</param>
        /// <param name="picBox">Instanz der PictureBox</param>
        public void DisplayPicture(Bitmap img, PictureBox picBox)
        {
            picBox.Invoke(new EventHandler(delegate
            {
                picBox.Image = img;
            }));
        }
        #region Bahnhofsausfahrt links
        private void UpdateGleisbild_GL1_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_links_3);
        }
        private void UpdateGleisbild_GL2_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_links_3);
        }
        private void UpdateGleisbild_GL3_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_links_3);
        }
        private void UpdateGleisbild_GL4_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_links_2);
        }
        private void UpdateGleisbild_GL5_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL5_links_0);

        }
        private void UpdateGleisbild_GL6_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL6_links_0);
            DisplayPicture(GetSchaltbildKurve270R(zustand), GL6_links_1);
        }
        #endregion
        #region Bahnhofsausfahrt rechts
        private void UpdateGleisbild_GL1_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL1_rechts_3);
            DisplayPicture(GetSchaltbildKurve90R(zustand), GL1_rechts_4);
            DisplayPicture(GetSchaltbildEckeUL(zustand), GL1_rechts_5);
        }
        private void UpdateGleisbild_GL2_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL2_rechts_3);
        }
        private void UpdateGleisbild_GL3_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL3_rechts_3);
        }
        private void UpdateGleisbild_GL4_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_0);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL4_rechts_2);
        }       
        private void UpdateGleisbild_GL5_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL5_rechts_0);
        }
        private void UpdateGleisbild_GL6_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, GL6_rechts_0);
        }
        #endregion
        #region Bahnhofseinfahrt links
        private void UpdateGleisbild_Weiche5(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
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
        private void UpdateGleisbild_Weiche6(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche6_Gleis4);
        }
        private void UpdateGleisbild_Block1a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1a_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1a_2);
        }
        private void UpdateGleisbild_Block1b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_5);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block1b_7);
        }
        private void UpdateGleisbild_Block1_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildKurve270L(zustand), Block1_Halt_1);
            DisplayPicture(GetSchaltbildEckeUR(zustand), Block1_Halt_2);
            DisplayPicture(GetSchaltbildEckeOL(zustand), Block1_Halt_3);
            DisplayPicture(GetSchaltbildKurve225L(zustand), Block1_Halt_4);
        }
        #endregion
        #region Block 2
        private void UpdateGleisbild_Block2(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildEckeOL_Sp(zustand), Block2_1);
            DisplayPicture(GetSchaltbildEckeUR_Sp(zustand), Block2_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_3);
            DisplayPicture(GetSchaltbildEckeOL_Sp(zustand), Block2_4);
            DisplayPicture(GetSchaltbildEckeUR_Sp(zustand), Block2_5);
            DisplayPicture(GetSchaltbildKurve225R(zustand), Block2_6);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_10);
        }
        private void UpdateGleisbild_Block2_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block2_Halt_2);
        }
        #endregion
        #region Block 3
        private void UpdateGleisbild_Weiche25(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche25_Gleis2);
        }
        private void UpdateGleisbild_Block3(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block3_5);
        }
        #endregion
        #region Block 4
        private void UpdateGleisbild_Block4(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block4_5);
        }
        private void UpdateGleisbild_Weiche26(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Weiche26_Gleis2);
        }
        #endregion
        #region Block 5
        private void UpdateGleisbild_Block5a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {           
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5a_4);
        }
        private void UpdateGleisbild_Block5b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
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
            DisplayPicture(GetSchaltbildKurve180L(zustand), Block5b_16);
            DisplayPicture(GetSchaltbildEckeOR_Sp(zustand), Block5b_17);
            DisplayPicture(GetSchaltbildKurve135L(zustand), Block5b_18);
        }
        private void UpdateGleisbild_Block5_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block5_Halt_2);
        }
        #endregion
        #region Block 6
        private void UpdateGleisbild_Block6(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
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
        private void UpdateGleisbild_Block6_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_Halt1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block6_Halt2);
        }
        #endregion
        #region Block 7
        private void UpdateGleisbild_Block7(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_3);
            DisplayPicture(GetSchaltbildKurve0L(zustand),   Block7_4);
            DisplayPicture(GetSchaltbildEckeOR_Sp(zustand), Block7_5);
            DisplayPicture(GetSchaltbildEckeUL_Sp(zustand), Block7_6);
            DisplayPicture(GetSchaltbildKurve315L(zustand), Block7_7);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_9);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_10);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block7_11);
            DisplayPicture(GetSchaltbildKurve225R(zustand), Block7_12);
            DisplayPicture(GetSchaltbildEckeUR_Sp(zustand), Block7_13);
            DisplayPicture(GetSchaltbildEckeOL_Sp(zustand), Block7_14);

        }
        #endregion
        #region Block 8
        private void UpdateGleisbild_Block8(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_2);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_3);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_5);
            DisplayPicture(GetSchaltbildKurve270L(zustand), Block8_6);
            DisplayPicture(GetSchaltbildEckeUR(zustand), Block8_7);
            DisplayPicture(GetSchaltbildEckeOL(zustand), Block8_8);
            DisplayPicture(GetSchaltbildKurve0R(zustand), Block8_9);        
        }
        private void UpdateGleisbild_Block8_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_Halt_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Block8_Halt_2);
        }
        #endregion
        #region Block 9
        private void UpdateGleisbild_Block9(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildKurve135L(zustand), Block9_1);
            DisplayPicture(GetSchaltbildEckeUL_Sp(zustand), Block9_2);
            DisplayPicture(GetSchaltbildKurve180L(zustand), Block9_3);
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
        private void UpdateGleisbild_Block9_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
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
                DisplayPicture(GetSchaltbildGerade90_EckeUL(zustand9, FreiesGleis), Kreuzung1_1);
                DisplayPicture(GetSchaltbildEckeUL(FreiesGleis), Kreuzung1_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOR(zustand9, FreiesGleis), Kreuzung1_3);
                DisplayPicture(GetSchaltbildKurve180L(FreiesGleis), Kreuzung1_4);
                DisplayPicture(GetSchaltbildKreuzung90_135(zustand9, FreiesGleis), Kreuzung1);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Kreuzung1_5);
            }
            else if (zustand8.Fahrstrasse == true)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, zustand8), Kreuzung1_1);
                DisplayPicture(GetSchaltbildEckeUL(zustand8), Kreuzung1_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, zustand8), Kreuzung1_3);
                DisplayPicture(GetSchaltbildKreuzung90_135(FreiesGleis, zustand8), Kreuzung1);  
                zustand8.Richtung = !zustand8.Richtung;
                DisplayPicture(GetSchaltbildKurve180L(zustand8), Kreuzung1_4);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_5);

            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(zustand9, FreiesGleis), Kreuzung1_1);
                DisplayPicture(GetSchaltbildEckeUL(zustand8), Kreuzung1_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOR(zustand9, FreiesGleis), Kreuzung1_3);
                DisplayPicture(GetSchaltbildKurve180L(zustand8), Kreuzung1_4);
                DisplayPicture(GetSchaltbildKreuzung90_135(zustand9, zustand8), Kreuzung1);
                GleisbildZeichnung.ZeichneSchaltbild(zustand8, Kreuzung1_5);
            }
       
        }

        #region Schattenbahnhof
        private void UpdateGleisbild_SchattenkleinAusf(bool besetzt, List<Fahrstrasse> Fahrstrassen)
        {
            bool besetzt8 = false;
            bool besetzt9 = false;
            bool besetzt10 = false;
            bool besetzt11 = false;

            if (WeichenListe.GetWeiche("Weiche80").Abzweig) besetzt8 = besetzt;
            else if(WeichenListe.GetWeiche("Weiche81").Abzweig) besetzt9 = besetzt;
            else if (WeichenListe.GetWeiche("Weiche82").Abzweig) besetzt10 = besetzt;
            else besetzt11 = besetzt;

            MeldeZustand zustand8 = ErrechneZustand(besetzt8, new List<Fahrstrasse>(), new List<Fahrstrasse> { Fahrstrassen[0] }); //Gleis8
            MeldeZustand zustand9 = ErrechneZustand(besetzt9, new List<Fahrstrasse>(), new List<Fahrstrasse> { Fahrstrassen[1] }); //Gleis9
            MeldeZustand zustand10 = ErrechneZustand(besetzt10, new List<Fahrstrasse>(), new List<Fahrstrasse> { Fahrstrassen[2] }); //Gleis10
            MeldeZustand zustand11 = ErrechneZustand(besetzt11, new List<Fahrstrasse>(), new List<Fahrstrasse> { Fahrstrassen[3] }); //Gleis11

            DisplayPicture(GetSchaltbildEckeUR(zustand8), Schatten8_Ausf1);
            DisplayPicture(GetSchaltbildKurve90L_EckeUR(zustand8, zustand9), Schatten8_Ausf2);
            DisplayPicture(GetSchaltbildEckeOL(zustand8), Schatten8_Ausf3);

            DisplayPicture(GetSchaltbildKurve90L_EckeUR(zustand9, zustand10), Schatten9_Ausf1);
            DisplayPicture(GetSchaltbildEckeOL(zustand9), Schatten9_Ausf2);

            GleisbildZeichnung.ZeichneSchaltbild(zustand10, Schatten10_Ausf1);
            DisplayPicture(GetSchaltbildKurve90L_EckeUR(zustand10, zustand11), Schatten10_Ausf2);

            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand11, Schatten11_Ausf2);
            DisplayPicture(GetSchaltbildKurve90L(zustand11), Schatten11_Ausf3);
            DisplayPicture(GetSchaltbildEckeOL(zustand11), Schatten11_Ausf4);
            DisplayPicture(GetSchaltbildKurve180R_EckeOL(zustand11, zustand10), Schatten11_Ausf5);
        }
        private void UpdateGleisbild_Schatten0Ausf(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            if((WeichenListe.GetWeiche("Weiche68").Abzweig) || zustand.Fahrstrasse == true)
            {
                DisplayPicture(GetSchaltbildEckeUR(zustand), Schatten0_Ausf1);
                DisplayPicture(GetSchaltbildKurve270L(zustand), Schatten0_Ausf2);
                GleisbildZeichnung.ZeichneSchaltbild(zustand, Schatten0_Ausf3);
            }
            else if(zustand.Besetzt == false)
            {

            }
        }
        #endregion
        #region Block (sonder)
        private void UpdateGleisbild_Block5_Block9(bool besetzt_Block5, List<Fahrstrasse> Fahrstrasse_links_Block5, List<Fahrstrasse> Fahrstrasse_rechts_Block5, 
                                                   bool besetzt_Block9, List<Fahrstrasse> Fahrstrasse_links_Block9, List<Fahrstrasse> Fahrstrasse_rechts_Block9)
        {
            MeldeZustand Zustand_Block5 = ErrechneZustand(besetzt_Block5, Fahrstrasse_links_Block5, Fahrstrasse_rechts_Block5);
            MeldeZustand Zustand_Block9 = ErrechneZustand(besetzt_Block9, Fahrstrasse_links_Block9, Fahrstrasse_rechts_Block9);
            DisplayPicture(GetSchaltbildEckeUL_OR(Zustand_Block5, Zustand_Block9),Block5_Block9);
        }
        #endregion

        #region Weichen Gleisfeldumgebung
        private void UpdateGleisbild_Weiche1()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche1);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche1_Gl1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche1_Gl1);
            }

        }
        private void UpdateGleisbild_Weiche2()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR270(weiche), Weiche2);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, meldeZustand), Weiche2_Gleis);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(meldeZustand, FreiesGleis), Weiche2_Gleis);               
            }

        }
        private void UpdateGleisbild_Weiche3()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche3");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche3);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche3_Gleis);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche3_Gleis);
            }
        }
        private void UpdateGleisbild_Weiche4()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche4");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheL270(weiche), Weiche4);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche4_Gl1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche4_Gl1);
            }

        }
        private void UpdateGleisbild_Weiche5()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche5");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche5);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche5_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche5_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche6()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche6");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche6);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve270L(meldeZustand), Weiche6_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche6_Gleis3);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche6_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildKurve270L(FreiesGleis), Weiche6_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche6_Gleis3);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche6_Gleis1);
            }

        }
        private void UpdateGleisbild_Weiche8()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche8");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheL315(weiche), Weiche8);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(meldeZustand, FreiesGleis), Weiche8_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, meldeZustand), Weiche8_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche21()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche21");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche21);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche21_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche21_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche23()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche23");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche23);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche23_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche23_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche25()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche25");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheL270(weiche), Weiche25);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche25_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche25_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche26()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche26");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR270(weiche), Weiche26);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, meldeZustand), Weiche26_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(meldeZustand, FreiesGleis), Weiche26_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche27()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche27");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche27);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche27_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche27_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche28()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche28");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheL270(weiche), Weiche28);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche28_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche28_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche29()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche29");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche29);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche29_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche29_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche30()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche30");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR270(weiche), Weiche30);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, meldeZustand), Weiche30_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(meldeZustand, FreiesGleis), Weiche30_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche50()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche50");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR90(weiche), Weiche50);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeOR(meldeZustand),   Weiche50a_1);
                DisplayPicture(GetSchaltbildKurve270R(meldeZustand),Weiche50a_2);
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche50b_1);              
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche50b_1);
            }
        }

        private void UpdateGleisbild_Tunnel1_Einf(bool besetzt, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, new List<Fahrstrasse>(), Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel1_Einf);
        }
        private void UpdateGleisbild_Tunnel2_Einf(bool besetzt, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, new List<Fahrstrasse>(), Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel2_Einf);
        }
        private void UpdateGleisbild_Tunnel1_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, new List<Fahrstrasse>(), Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel1_Halt);
        }
        private void UpdateGleisbild_Tunnel2_Halt(bool besetzt, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, new List<Fahrstrasse>(), Fahrstrasse_rechts);
            GleisbildZeichnung.ZeichneSchaltbild(zustand, Tunnel2_Halt);
        }

        private void UpdateGleisbild_Tunnel(bool besetzt_Tunnel1, List<Fahrstrasse> Fahrstrasse_Tunnel1,
                                            bool besetzt_Tunnel2, List<Fahrstrasse> Fahrstrasse_Tunnel2)
        {
            MeldeZustand zustand_Tunnel1 = ErrechneZustand(besetzt_Tunnel1, new List<Fahrstrasse>(), Fahrstrasse_Tunnel1);
            MeldeZustand zustand_Tunnel2 = ErrechneZustand(besetzt_Tunnel2, new List<Fahrstrasse>(), Fahrstrasse_Tunnel2);

            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_2);
            DisplayPicture(GetSchaltbildKurve90L( zustand_Tunnel1), Tunnel1_3);
            DisplayPicture(GetSchaltbildEckeOL(   zustand_Tunnel1), Tunnel1_4);
            DisplayPicture(GetSchaltbildKurve180R(zustand_Tunnel1), Tunnel1_5);
            DisplayPicture(GetSchaltbildKurve0L(  zustand_Tunnel1), Tunnel1_6);
            DisplayPicture(GetSchaltbildEckeUL_Sp(zustand_Tunnel1), Tunnel1_7);
            DisplayPicture(GetSchaltbildKurve315L(zustand_Tunnel1), Tunnel1_8);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel1, Tunnel1_9);

            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_1);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_2);
            DisplayPicture(GetSchaltbildKurve90L_EckeUR(zustand_Tunnel2, zustand_Tunnel1), Tunnel2_3);
            DisplayPicture(GetSchaltbildKurve180L_EckeOR(zustand_Tunnel2, zustand_Tunnel1), Tunnel2_4);
            GleisbildZeichnung.ZeichneSchaltbild(zustand_Tunnel2, Tunnel2_5);
        }


        private void UpdateGleisbild_Weiche51()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche51");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche51);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche51a_1);
                DisplayPicture(GetSchaltbildKurve270L(meldeZustand), Weiche51a_2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51a_3);
                
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche51b_1);

            }
            else
            {
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche51b_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche51b_1);
            }
        }
        private void UpdateGleisbild_Weiche52()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche52");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheL180(weiche), Weiche52);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUL_Sp(meldeZustand), Weiche52_Gleis3);
                DisplayPicture(GetSchaltbildGerade180_EckeOR(FreiesGleis, meldeZustand), Weiche52_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUL_Sp(FreiesGleis), Weiche52_Gleis3);
                DisplayPicture(GetSchaltbildGerade180_EckeOR(meldeZustand, FreiesGleis), Weiche52_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche53()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche53");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche), Weiche53);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve0R(FreiesGleis), Weiche53_Gleis0);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche53_Gleis1);
                DisplayPicture(GetSchaltbildKurve225L_EckeUR(meldeZustand, FreiesGleis), Weiche53_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Weiche53_Gleis3);

            }
            else
            {
                DisplayPicture(GetSchaltbildKurve0R(meldeZustand), Weiche53_Gleis0);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche53_Gleis1);
                DisplayPicture(GetSchaltbildKurve225L_EckeUR(FreiesGleis, meldeZustand), Weiche53_Gleis2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand, Weiche53_Gleis3);
            }
        }
        private void UpdateGleisbild_Weiche60()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche60");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR270(weiche), Weiche60);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade270_EckeOR(FreiesGleis, meldeZustand), Weiche60_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade270_EckeOR(meldeZustand, FreiesGleis), Weiche60_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche61()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche61");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheL90(weiche), Weiche61);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche61_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche61_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche61_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche61_2);
            }
        }
        private void UpdateGleisbild_Weiche62()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche62");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche62);
            if (!weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche62_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche62_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche62_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche62_2);
            }
        }
        private void UpdateGleisbild_Weiche63()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche63");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche63);
            if (!weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche63_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche63_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche63_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche63_2);
            }
        }
        private void UpdateGleisbild_Weiche64()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche64");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche64);
            if (!weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche64_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche64_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche64_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche64_2);
            }
        }
        private void UpdateGleisbild_Weiche65()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche65");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche65);
            if (!weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche65_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche65_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Weiche65_1);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche65_2);
            }
        }
        private void UpdateGleisbild_Weiche66()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche66");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            DisplayPicture(GetSchaltbildWeicheR45(weiche), Weiche66);
            if (!weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche66_1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, FreiesGleis), Weiche66_1);
            }
        }
        private void UpdateGleisbild_Weiche67_68()
        {

            Weiche weiche67 = WeichenListe.GetWeiche("Weiche67");
            if (weiche67 == null) return;
            Weiche weiche68 = WeichenListe.GetWeiche("Weiche68");
            if (weiche68 == null) return;

            MeldeZustand meldeZustand1 = new MeldeZustand(weiche67, OST);
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche68, WEST);

            DisplayPicture(GetSchaltbildWeicheL270(weiche67), Weiche67);
            DisplayPicture(GetSchaltbildWeicheL90(weiche68), Weiche68);

            if ((weiche67.Abzweig == false) && (weiche68.Abzweig == false))
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(meldeZustand1, FreiesGleis, FreiesGleis), Weiche68_67);
            else if ((weiche67.Abzweig == true) && (weiche68.Abzweig == false))
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(FreiesGleis, FreiesGleis, meldeZustand1), Weiche68_67);
            else if ((weiche67.Abzweig == false) && (weiche68.Abzweig == true))
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(FreiesGleis, meldeZustand2, FreiesGleis), Weiche68_67);
            else
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(FreiesGleis, meldeZustand2, meldeZustand1), Weiche68_67);
            if (weiche68.Abzweig == true)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand2), Schatten0_Ausf1);
                DisplayPicture(GetSchaltbildKurve270L(meldeZustand2), Schatten0_Ausf2);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand2, Schatten0_Ausf3);
                GleisbildZeichnung.ZeichneSchaltbild(meldeZustand2, Schatten0_Ausf4);
            }
            else
            {
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), Schatten0_Ausf1);
                DisplayPicture(GetSchaltbildKurve270L(FreiesGleis), Schatten0_Ausf2);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf3);
                GleisbildZeichnung.ZeichneSchaltbild(FreiesGleis, Schatten0_Ausf4);
            }

        }
        private void UpdateGleisbild_Weiche70()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche70");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche), Weiche70);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche70_1);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche70_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche70_1);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche70_2);
            }
        }
        private void UpdateGleisbild_Weiche71()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche71");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche), Weiche71);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche71_1);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche71_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche71_1);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche71_2);
            }
        }
        private void UpdateGleisbild_Weiche72()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche72");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche), Weiche72);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche72_1);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche72_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche72_1);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche72_2);
            }
        }
        private void UpdateGleisbild_Weiche73()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche73");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche), Weiche73);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche73_1);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche73_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche73_1);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche73_2);
            }
        }
        private void UpdateGleisbild_Weiche74()
        {
            Weiche weiche74 = WeichenListe.GetWeiche("Weiche74");
            if (weiche74 == null) return;
            Weiche weiche92 = WeichenListe.GetWeiche("Weiche92");
            if (weiche92 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche74, OST);
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche92, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche74), Weiche74);
            if (weiche74.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche74_1);
                if(weiche92.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, FreiesGleis), WeicheEcke74_92);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, meldeZustand2), WeicheEcke74_92);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche74_1);
                meldeZustand.Richtung = !meldeZustand.Richtung;
                if (weiche92.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, FreiesGleis), WeicheEcke74_92);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, meldeZustand2), WeicheEcke74_92);
            }
        }
        private void UpdateGleisbild_Weiche75()
        {
            Weiche weiche75 = WeichenListe.GetWeiche("Weiche75");
            if (weiche75 == null) return;
            Weiche weiche91 = WeichenListe.GetWeiche("Weiche91");
            if (weiche91 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche75, OST);
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche91, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche75), Weiche75);
            if (weiche75.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche75_1);
                if (weiche91.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, FreiesGleis), WeicheEcke75_91);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, meldeZustand2), WeicheEcke75_91);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche75_1);
                meldeZustand.Richtung = !meldeZustand.Richtung;
                if (weiche91.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, FreiesGleis), WeicheEcke75_91);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, meldeZustand2), WeicheEcke75_91);
            }
        }
        private void UpdateGleisbild_Weiche76()
        {
            Weiche weiche76 = WeichenListe.GetWeiche("Weiche76");
            if (weiche76 == null) return;
            Weiche weiche90 = WeichenListe.GetWeiche("Weiche90");
            if (weiche90 == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche76, OST);
            MeldeZustand meldeZustand2 = new MeldeZustand(weiche90, OST);
            DisplayPicture(GetSchaltbildWeicheR225(weiche76), Weiche76);
            if (weiche76.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), Weiche76_1);
                DisplayPicture(GetSchaltbildKurve90L(FreiesGleis), Weiche76_2);
                if (!weiche90.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, FreiesGleis), WeicheEcke76_90);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(FreiesGleis, meldeZustand2), WeicheEcke76_90);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), Weiche76_1);
                DisplayPicture(GetSchaltbildKurve90L(meldeZustand), Weiche76_2);
                meldeZustand.Richtung = !meldeZustand.Richtung;
                if (!weiche90.Abzweig)
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, FreiesGleis), WeicheEcke76_90);
                else
                    DisplayPicture(GetSchaltbildEckeOL_UR(meldeZustand, meldeZustand2), WeicheEcke76_90);
            }
        }
        private void UpdateGleisbild_Weiche80()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche80");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR180(weiche), Weiche80);
        }

        private void UpdateGleisbild_Weiche81()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche81");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR180(weiche), Weiche81);
        }

        private void UpdateGleisbild_Weiche82()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche82");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            DisplayPicture(GetSchaltbildWeicheR180(weiche), Weiche82);
        }

        private void UpdateGleisbild_Weiche90()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche90");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche90_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche90_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche91()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche91");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche91_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche91_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche92()
        {
            Weiche weiche = WeichenListe.GetWeiche("Weiche92");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche92_Gleis1);
                DisplayPicture(GetSchaltbildKurve270L(FreiesGleis), Weiche92_Gleis2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche92_Gleis1);
                DisplayPicture(GetSchaltbildKurve270L(meldeZustand), Weiche92_Gleis2);
            }
        }

        #endregion
        #region DKW Gleisfeldumgebung
        private void UpdateGleisbild_WeicheDKW7_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW7_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            DisplayPicture(GetSchaltbildDKW90_135(weiche, DKW_2nd), DKW7);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(FreiesGleis, meldeZustand), Weiche7_Gleis2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(meldeZustand, FreiesGleis), Weiche7_Gleis2);
            }
        }
        private void UpdateGleisbild_WeicheDKW7_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW7_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche7_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche7_Gleis1);
            }
        }     
        private void UpdateGleisbild_WeicheDKW9_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW9_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            DisplayPicture(GetSchaltbildDKW90_135(weiche, DKW_2nd), DKW9);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve270L_EckeOR(FreiesGleis, meldeZustand), Weiche9_Gleis1);
                DisplayPicture(GetSchaltbildEckeUL(meldeZustand), Weiche9_Gleis4);
            }
            else
            {
                DisplayPicture(GetSchaltbildKurve270L_EckeOR(meldeZustand, FreiesGleis), Weiche9_Gleis1);
                DisplayPicture(GetSchaltbildEckeUL(FreiesGleis), Weiche9_Gleis4);
            }

        }
        private void UpdateGleisbild_WeicheDKW9_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW9_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, WEST);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche9_Gleis2);
                DisplayPicture(GetSchaltbildEckeOR(meldeZustand), Weiche9_Gleis3);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche9_Gleis2);
                DisplayPicture(GetSchaltbildEckeOR(FreiesGleis), Weiche9_Gleis3);
            }
        }       
        private void UpdateGleisbild_KW22_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("KW22_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            DisplayPicture(GetSchaltbildKW90_45(weiche, DKW_2nd), KW22);
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(FreiesGleis, meldeZustand, FreiesGleis), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), KW22_Gleis3);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(meldeZustand, FreiesGleis, FreiesGleis), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), KW22_Gleis3);
            }

        }
        private void UpdateGleisbild_KW22_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("KW22_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            
            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), KW22_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), KW22_Gleis1);
            }
        }       
        private void UpdateGleisbild_DKW24_1()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW24_1");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);
            Weiche DKW_2nd = GetDWK_2nd(weiche.Name);
            DisplayPicture(GetSchaltbildDKW90_45(weiche, DKW_2nd), DKW24);

            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), DKW24_Gleis2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), DKW24_Gleis2);
            }
        }
        private void UpdateGleisbild_DKW24_2()
        {
            Weiche weiche = WeichenListe.GetWeiche("DKW24_2");
            if (weiche == null) return;
            MeldeZustand meldeZustand = new MeldeZustand(weiche, OST);

            if (weiche.Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(FreiesGleis, meldeZustand), DKW24_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(meldeZustand, FreiesGleis), DKW24_Gleis1);
            }
        }
        #endregion
        #endregion

        #region Schaltbild zeichnen

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

        #region nur Ecken
        private dynamic GetSchaltbildEckeUR(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOR(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOL(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeUL(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeUR_Sp(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOR_Sp(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOL_Sp(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeUL_Sp(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                return bild;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOL_UR(MeldeZustand ZustandOL, MeldeZustand ZustandUR)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOL_UR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if (!ZustandOL.IstFrei())
            {
                if ((ZustandOL.Besetzt == true) && (ZustandOL.Fahrstrasse == false))
                {
                    return bild;
                }
                else
                {
                    if (ZustandOL.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;

                    if (ZustandOL.Fahrstrasse) farbe = Farbe_Gelb;
                    if (ZustandOL.Sicher) farbe = Farbe_Gruen;
                    if (ZustandOL.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            if (!ZustandUR.IstFrei())
            {
                if ((ZustandUR.Besetzt == true) && (ZustandUR.Fahrstrasse == false))
                {
                    return bild;
                }
                else
                {
                    if (ZustandUR.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;

                    if (ZustandUR.Fahrstrasse) farbe = Farbe_Gelb;
                    if (ZustandUR.Sicher) farbe = Farbe_Gruen;
                    if (ZustandUR.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeUL_OR(MeldeZustand ZustandUL, MeldeZustand ZustandOR)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUL_OR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if (!ZustandUL.IstFrei())
            {
                if ((ZustandUL.Besetzt == true) && (ZustandUL.Fahrstrasse == false))
                {
                    return bild;
                }
                else
                {
                    if (ZustandUL.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links;

                    if (ZustandUL.Fahrstrasse) farbe = Farbe_Gelb;
                    if (ZustandUL.Sicher) farbe = Farbe_Gruen;
                    if (ZustandUL.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            if (!ZustandOR.IstFrei())
            {
                if ((ZustandOR.Besetzt == true) && (ZustandOR.Fahrstrasse == false))
                {
                    return bild;
                }
                else
                {
                    if (ZustandOR.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;

                    if (ZustandOR.Fahrstrasse) farbe = Farbe_Gelb;
                    if (ZustandOR.Sicher) farbe = Farbe_Gruen;
                    if (ZustandOR.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        #endregion

        #region Kruezung
        private dynamic GetSchaltbildKreuzung90_135(MeldeZustand Zustandwaage, MeldeZustand Zustandschraege)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kreuzung90_135; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if (!Zustandwaage.IstFrei())
            {
                if ((Zustandwaage.Besetzt == true) && (Zustandwaage.Fahrstrasse == false))
                {
                    zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                    farbe = Farbe_Rot;
                    ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                    if(Zustandschraege.Besetzt == true)
                    {
                        zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                        farbe = Farbe_Rot;
                        ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                    }
                    return bild; //Bild ausgeben
                }
                else
                {
                    if (Zustandwaage.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                    if (Zustandwaage.Fahrstrasse) farbe = Farbe_Gelb;
                    if (Zustandwaage.Sicher) farbe = Farbe_Gruen;
                    if (Zustandwaage.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            if(!Zustandschraege.IstFrei())
            {
                if ((Zustandschraege.Besetzt == true) && (Zustandschraege.Fahrstrasse == false))
                {
                    zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                    farbe = Farbe_Rot;
                    ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                    return bild; //Bild ausgeben
                }
                else
                {
                    if (Zustandschraege.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts;

                    if (Zustandschraege.Fahrstrasse) farbe = Farbe_Gelb;
                    if (Zustandschraege.Sicher) farbe = Farbe_Gruen;
                    if (Zustandschraege.Besetzt) farbe = Farbe_Rot;
                }
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }

        #endregion
        #region Geraden mit einer Ecke
        private dynamic GetSchaltbildGerade180_EckeOR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade0_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }      
        private dynamic GetSchaltbildGerade90_EckeOL(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeOR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeUR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeUR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeUL(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeUL;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade270_EckeOR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            return bild; //Bild ausgeben
        }
        #endregion
        #region Geraden mit zwei Ecken
        private dynamic GetSchaltbildGerade90_EckeOL_UR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_OL, MeldeZustand Zustand_UR)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL_UR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Gerade.Besetzt == true) && (Zustand_Gerade.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_OL.Besetzt == true) && (Zustand_OL.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_OL.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;

                if (Zustand_OL.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_OL.Sicher) farbe = Farbe_Gruen;
                if (Zustand_OL.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_OL.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }

            if ((Zustand_UR.Besetzt == true) && (Zustand_UR.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_UR.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;

                if (Zustand_UR.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_UR.Sicher) farbe = Farbe_Gruen;
                if (Zustand_UR.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_UR.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        #endregion
        #region Kurve Links
        private dynamic GetSchaltbildKurve0L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve0L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_0;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve45L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve180R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve135L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve180L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve180L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve225L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve0R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_0;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve315L(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        #endregion
        #region Kurve Links mit Ecke
        private dynamic GetSchaltbildKurve90L_EckeUR(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90L_EckeUR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben

        }
        private dynamic GetSchaltbildKurve180L_EckeOR(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R_EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270L_EckeOR(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270L_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve225L_EckeUR(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270R_EckeUR;
            Graphics gleis = Graphics.FromImage(bild);

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        #endregion
        #region Kurve Rechts
        private dynamic GetSchaltbildKurve0R(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve0R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_0;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90R(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve180R(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve180R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270R(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve225R(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild; //Bild ausgeben
        }
        #endregion
        #region Kurve Rechts mit Ecke
        private dynamic GetSchaltbildKurve90R_EckeOR(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R_EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve180R_EckeOL(MeldeZustand Zustand_Kurve, MeldeZustand Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve180R_EckeOL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand_Kurve.Besetzt == true) && (Zustand_Kurve.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                farbe = Farbe_Rot;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);


            if ((Zustand_Ecke.Besetzt == true) && (Zustand_Ecke.Fahrstrasse == false))
            {
                //Do nothing
            }
            else
            {
                if (Zustand_Ecke.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts;

                if (Zustand_Ecke.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Ecke.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Ecke.Besetzt) farbe = Farbe_Rot;
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            }
            return bild; //Bild ausgeben
        }

        #endregion
        #region Weiche Links
        private dynamic GetSchaltbildWeicheL90(Weiche weiche)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheL90_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if(weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheL180(Weiche weiche)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheL180_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }


            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_unten;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_oben;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheL270(Weiche weiche)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheL270_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheL315(Weiche weiche)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheL315_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        #endregion
        #region Weiche Rechts
        private dynamic GetSchaltbildWeicheR45(Weiche weiche)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheR45_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);

            return bild;
        }
        private dynamic GetSchaltbildWeicheR90(Weiche weiche)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheR90_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }

            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheR180(Weiche weiche)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheR180_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheR225(Weiche weiche)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheR225_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildWeicheR270(Weiche weiche)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheR270_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (weiche.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (weiche.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
            else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if (weiche.ZeitAktiv > 0)
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Grau, Farbe_Weis);
            }
            else
            {
                ZeichneFahrstraße(ref gleis, zeichenmuster, Farbe_Weis, Farbe_Weis);
            }

            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if (weiche.FahrstrasseRichtung_vonZunge)
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                else
                {
                    if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;
                    else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                if (weiche.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (weiche.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (weiche.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        #endregion
        #region DKW
        private dynamic GetSchaltbildKW90_45(Weiche DKW1, Weiche DKW2)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheKW90_45_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Error || DKW2.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Unbekannt || DKW2.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if ((DKW1.Besetzt == true || DKW2.Besetzt == true) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                ZeichneFahrstraße(ref gleis, zunge, Farbe_Rot, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if ((DKW1.ZeitAktiv > 0) || (DKW2.ZeitAktiv > 0))
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Grau, Farbe_Weis);
                }
                else
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis, Farbe_Weis);
                }
            }

            Image zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
            Color farbe = Farbe_Grau;
            if (((DKW1.Besetzt == false) || (DKW2.Besetzt == false)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                return bild;
            }
            else if (((DKW1.Besetzt == true) || (DKW2.Besetzt == true)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                farbe = Farbe_Rot;
                zeichenmuster = zunge;
            }
            else
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (DKW1.FahrstrasseSicher || DKW2.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (DKW1.Besetzt || DKW2.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildDKW90_45(Weiche DKW1, Weiche DKW2)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheDKW90_45_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Error || DKW2.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Unbekannt || DKW2.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if ((DKW1.Besetzt == true || DKW2.Besetzt == true) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                ZeichneFahrstraße(ref gleis, zunge, Farbe_Rot, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if ((DKW1.ZeitAktiv > 0) || (DKW2.ZeitAktiv > 0))
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Grau, Farbe_Weis);
                }
                else
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis, Farbe_Weis);
                }
            }

            Image zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
            Color farbe = Farbe_Grau;
            if (((DKW1.Besetzt == false) || (DKW2.Besetzt == false)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                return bild;
            }
            else if (((DKW1.Besetzt == true) || (DKW2.Besetzt == true)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                farbe = Farbe_Rot;
                zeichenmuster = zunge;
            }
            else
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (DKW1.FahrstrasseSicher || DKW2.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (DKW1.Besetzt || DKW2.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
        }
        private dynamic GetSchaltbildDKW90_135(Weiche DKW1, Weiche DKW2)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheDKW90_135_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Error || DKW2.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Unbekannt || DKW2.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            Image zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zunge = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
            if ((DKW1.Besetzt == true || DKW2.Besetzt == true) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                ZeichneFahrstraße(ref gleis, zunge, Farbe_Rot, Farbe_Weis);
                return bild; //Bild ausgeben
            }
            else
            {
                if ((DKW1.ZeitAktiv > 0) || (DKW2.ZeitAktiv > 0))
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Grau, Farbe_Weis);
                }
                else
                {
                    ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis, Farbe_Weis);
                }
            }

            Image zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
            Color farbe = Farbe_Grau;
            if (((DKW1.Besetzt == false) || (DKW2.Besetzt == false)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                return bild;
            }
            else if (((DKW1.Besetzt == true) || (DKW2.Besetzt == true)) && ((DKW1.FahrstrasseAktive == false) || (DKW2.FahrstrasseAktive == false)))
            {
                farbe = Farbe_Rot;
                zeichenmuster = zunge;
            }
            else
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                }
                if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive) farbe = Farbe_Gelb;
                if (DKW1.FahrstrasseSicher || DKW2.FahrstrasseSicher) farbe = Farbe_Gruen;
                if (DKW1.Besetzt || DKW2.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe, Farbe_Gelb);
            return bild;
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
                else
                {
                    if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"))
                    {
                        AutoSignalUpdate("Signal_Schatten8");
                        AutoSignalUpdate("Signal_Schatten9");
                        AutoSignalUpdate("Signal_Schatten10");
                        AutoSignalUpdate("Signal_Schatten11");
                    }
                }
            }
        }
        /// <summary>
        /// Fahrstraßen im Bild zeichnen
        /// </summary>
        private void FahrstrasseBildUpdate()
        {
            UpdateWeichenBelegung();

            //Hauptbahnhof - Linker Teil der Gleise
            UpdateGleisbild_GL1_links(BelegtmelderListe.GetBelegtStatus("W6") && WeichenListe.GetWeiche("Weiche6").Abzweig,  new List<Fahrstrasse> { Gleis1_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis1 });
            UpdateGleisbild_GL2_links(BelegtmelderListe.GetBelegtStatus("W6") && !WeichenListe.GetWeiche("Weiche6").Abzweig, new List<Fahrstrasse> { Gleis2_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis2 });
            UpdateGleisbild_GL3_links(BelegtmelderListe.GetBelegtStatus("W5") && !WeichenListe.GetWeiche("Weiche5").Abzweig, new List<Fahrstrasse> { Gleis3_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis3 });
            UpdateGleisbild_GL4_links(BelegtmelderListe.GetBelegtStatus("DKW7_W8") && !WeichenListe.GetWeiche("DKW7_2").Abzweig, new List<Fahrstrasse> { Gleis4_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis4 });
            UpdateGleisbild_GL5_links(BelegtmelderListe.GetBelegtStatus("DKW9") && !WeichenListe.GetWeiche("DKW9_2").Abzweig, new List<Fahrstrasse> { Gleis5_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis5 });
            UpdateGleisbild_GL6_links(BelegtmelderListe.GetBelegtStatus("DKW9") && WeichenListe.GetWeiche("DKW9_2").Abzweig, new List<Fahrstrasse> { Gleis6_nach_Block1}, new List<Fahrstrasse> { Block2_nach_Gleis6 });
            
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
            UpdateGleisbild_GL1_rechts(BelegtmelderListe.GetBelegtStatus("W26") && WeichenListe.GetWeiche("Weiche26").Abzweig, new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts2_nach_Gleis1 } , new List<Fahrstrasse> { Gleis1_nach_rechts1 , Gleis1_nach_rechts2 });
            UpdateGleisbild_GL2_rechts(BelegtmelderListe.GetBelegtStatus("W26") && !WeichenListe.GetWeiche("Weiche26").Abzweig, new List<Fahrstrasse> { Rechts1_nach_Gleis2, Rechts2_nach_Gleis2 } , new List<Fahrstrasse> { Gleis2_nach_rechts1 , Gleis2_nach_rechts2 });
            UpdateGleisbild_GL3_rechts(BelegtmelderListe.GetBelegtStatus("W25") && !WeichenListe.GetWeiche("Weiche25").Abzweig, new List<Fahrstrasse> { Rechts1_nach_Gleis3, Rechts2_nach_Gleis3 } , new List<Fahrstrasse> { Gleis3_nach_rechts1 , Gleis3_nach_rechts2 });
            UpdateGleisbild_GL4_rechts(BelegtmelderListe.GetBelegtStatus("DKW24_W23") && !WeichenListe.GetWeiche("DKW24_2").Abzweig, new List<Fahrstrasse> { Rechts1_nach_Gleis4, Rechts2_nach_Gleis4 } , new List<Fahrstrasse> { Gleis4_nach_rechts1 , Gleis4_nach_rechts2 });
            UpdateGleisbild_GL5_rechts(BelegtmelderListe.GetBelegtStatus("KW22") && !WeichenListe.GetWeiche("KW22_2").Abzweig, new List<Fahrstrasse> { Rechts1_nach_Gleis5, Rechts2_nach_Gleis5 } , new List<Fahrstrasse> { Gleis5_nach_rechts1 , Gleis5_nach_rechts2 });
            UpdateGleisbild_GL6_rechts(BelegtmelderListe.GetBelegtStatus("HBf6_Halt_R"), new List<Fahrstrasse> { Rechts1_nach_Gleis6, Rechts2_nach_Gleis6 } , new List<Fahrstrasse> { Gleis6_nach_rechts1 , Gleis6_nach_rechts2 });

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
            UpdateGleisbild_Tunnel1_Einf(BelegtmelderListe.GetBelegtStatus("Tunnel1_Einfahrt"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis3_nach_rechts1, //Nach rechts
                                                            Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1});
            UpdateGleisbild_Tunnel2_Einf(BelegtmelderListe.GetBelegtStatus("Tunnel2_Einfahrt"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts2, Gleis2_nach_rechts2, Gleis3_nach_rechts2, //Nach rechts
                                                            Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2});

            UpdateGleisbild_Tunnel(BelegtmelderListe.GetBelegtStatus("Tunnel1"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis3_nach_rechts1, //Nach rechts
                                                            Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1},
                                   BelegtmelderListe.GetBelegtStatus("Tunnel2"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts2, Gleis2_nach_rechts2, Gleis3_nach_rechts2, //Nach rechts
                                                            Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2});

            UpdateGleisbild_Tunnel1_Halt(BelegtmelderListe.GetBelegtStatus("Tunnel1_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis3_nach_rechts1, //Nach rechts
                                                            Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1});
            UpdateGleisbild_Tunnel2_Halt(BelegtmelderListe.GetBelegtStatus("Tunnel2_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_rechts2, Gleis2_nach_rechts2, Gleis3_nach_rechts2, //Nach rechts
                                                            Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2});


            //Gleise im Block 1 aktualisieren
            UpdateGleisbild_Weiche6(BelegtmelderListe.GetBelegtStatus("W6"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block1, Gleis2_nach_Block1 },//Nach links
                                    new List<Fahrstrasse> { Block2_nach_Gleis1, Block2_nach_Gleis2 });//Nach rechts

            UpdateGleisbild_Block1a(BelegtmelderListe.GetBelegtStatus("Block1_a"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block1, Gleis2_nach_Block1, Gleis3_nach_Block1, //Nach links
                                                            Gleis4_nach_Block1, Gleis5_nach_Block1, Gleis6_nach_Block1 },
                                    new List<Fahrstrasse>()); //nach rechts immer aus

            UpdateGleisbild_Block1b(BelegtmelderListe.GetBelegtStatus("Block1_b"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block1, Gleis2_nach_Block1, Gleis3_nach_Block1, //Nach links
                                                            Gleis4_nach_Block1, Gleis5_nach_Block1, Gleis6_nach_Block1 },
                                    new List<Fahrstrasse>()); //nach rechts immer aus
            UpdateGleisbild_Block1_Halt(BelegtmelderListe.GetBelegtStatus("Block1_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block1, Gleis2_nach_Block1, Gleis3_nach_Block1, //Nach links
                                                            Gleis4_nach_Block1, Gleis5_nach_Block1, Gleis6_nach_Block1 },
                                    new List<Fahrstrasse>()); //nach rechts immer aus            
            UpdateGleisbild_Block2(BelegtmelderListe.GetBelegtStatus("Block2"), //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block2, Block9_nach_Block2 }, //Nach Links
                                    new List<Fahrstrasse>()); //nach rechts immer aus
            UpdateGleisbild_Block2_Halt(BelegtmelderListe.GetBelegtStatus("Block2_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block2, Block9_nach_Block2 }, //Nach Links
                                    new List<Fahrstrasse>()); //nach rechts immer aus
            //Gleise im Block 2 aktualisieren
            UpdateGleisbild_Weiche5(BelegtmelderListe.GetBelegtStatus("W5"), //Besetzt
                                    new List<Fahrstrasse>  {Gleis3_nach_Block1, Gleis4_nach_Block1, Gleis5_nach_Block1, Gleis6_nach_Block1},
                                    new List<Fahrstrasse> { Block2_nach_Gleis3, Block2_nach_Gleis4, Block2_nach_Gleis5, Block2_nach_Gleis6 });
            //Gleise im Block 3 aktualisieren
            UpdateGleisbild_Weiche25(BelegtmelderListe.GetBelegtStatus("W25"), //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis3, Rechts1_nach_Gleis4, Rechts1_nach_Gleis5, Rechts1_nach_Gleis6, //Nach links
                                                            Rechts2_nach_Gleis3, Rechts2_nach_Gleis4, Rechts2_nach_Gleis5, Rechts2_nach_Gleis6 },
                                    new List<Fahrstrasse> { Gleis3_nach_rechts1, Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1, //Nach rechts
                                                            Gleis3_nach_rechts2, Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2 });
            UpdateGleisbild_Block3(BelegtmelderListe.GetBelegtStatus("Block3"), //Besetzt
                                    new List<Fahrstrasse> (),//Nie nach links
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis3_nach_rechts1, //Nach rechts
                                                            Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1, 
                                                            Gleis1_nach_rechts2, Gleis2_nach_rechts2, Gleis3_nach_rechts2, 
                                                            Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2});
            //Gleise im Block 4 aktualisieren                        
            UpdateGleisbild_Block4(BelegtmelderListe.GetBelegtStatus("Block4"), //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts1_nach_Gleis2, Rechts1_nach_Gleis3, //Nach links
                                                            Rechts1_nach_Gleis4, Rechts1_nach_Gleis5, Rechts1_nach_Gleis6, 
                                                            Rechts2_nach_Gleis1, Rechts2_nach_Gleis2, Rechts2_nach_Gleis3, //Nach rechts
                                                            Rechts2_nach_Gleis4, Rechts2_nach_Gleis5, Rechts2_nach_Gleis6 },
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Weiche26(BelegtmelderListe.GetBelegtStatus("W26"), //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts1_nach_Gleis2, Rechts2_nach_Gleis1, Rechts2_nach_Gleis2 }, //Nach links
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis1_nach_rechts2, Gleis2_nach_rechts2 });//Nach rechts
            UpdateGleisbild_Block5a(BelegtmelderListe.GetBelegtStatus("Block5_a"), //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block5 } ,  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block5b(BelegtmelderListe.GetBelegtStatus("Block5_b"), //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block5 },  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block5_Halt(BelegtmelderListe.GetBelegtStatus("Block5_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block5 },  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block6(BelegtmelderListe.GetBelegtStatus("Block6"), //Besetzt
                                    new List<Fahrstrasse> { Block5_nach_Block6, Block8_nach_Block6 },  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block6_Halt(BelegtmelderListe.GetBelegtStatus("Block6_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Block5_nach_Block6, Block8_nach_Block6 },  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block7(BelegtmelderListe.GetBelegtStatus("Block7"), //Besetzt
                                    new List<Fahrstrasse> { Schatten8_nach_Block7, Schatten9_nach_Block7, Schatten10_nach_Block7, Schatten11_nach_Block7 },  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block8(BelegtmelderListe.GetBelegtStatus("Block8"), //Besetzt
                                    new List<Fahrstrasse> { Schatten0_nach_Block8, Schatten1_nach_Block8 },  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block8_Halt(BelegtmelderListe.GetBelegtStatus("Block8_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Schatten0_nach_Block8, Schatten1_nach_Block8 },  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts
            UpdateGleisbild_Block9(BelegtmelderListe.GetBelegtStatus("Block9"), //Besetzt
                                    new List<Fahrstrasse> { Schatten1_nach_Block9, Schatten2_nach_Block9, Schatten3_nach_Block9,
                                                Schatten4_nach_Block9, Schatten5_nach_Block9, Schatten6_nach_Block9,
                                                Schatten7_nach_Block9},//nach rechts
                                    new List<Fahrstrasse>());  //nie nach rechts
            UpdateGleisbild_Block9_Halt(BelegtmelderListe.GetBelegtStatus("Block9_Halt"), //Besetzt
                                    new List<Fahrstrasse> { Schatten1_nach_Block9, Schatten2_nach_Block9, Schatten3_nach_Block9,
                                                Schatten4_nach_Block9, Schatten5_nach_Block9, Schatten6_nach_Block9,
                                                Schatten7_nach_Block9},//nach rechts
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
                            new List<Fahrstrasse> { Block8_nach_Block6 },  //Block8
                            new List<Fahrstrasse> { Schatten1_nach_Block9, Schatten2_nach_Block9, Schatten3_nach_Block9,
                                                    Schatten4_nach_Block9, Schatten5_nach_Block9, Schatten6_nach_Block9,
                                                    Schatten7_nach_Block9}); //Block9

            UpdateGleisbild_Block5_Block9(BelegtmelderListe.GetBelegtStatus("Block5_b"), //Block5: Besetzt
                        new List<Fahrstrasse> { Block1_nach_Block5 },  //Block6: Nach links
                        new List<Fahrstrasse>(), //Block5: nie nach rechts
                        BelegtmelderListe.GetBelegtStatus("Block9"), //Block9: Besetzt
                        new List<Fahrstrasse>(), //Block9: nie nach rechts
                        new List<Fahrstrasse> { Schatten1_nach_Block9, Schatten2_nach_Block9, Schatten3_nach_Block9,
                                                Schatten4_nach_Block9, Schatten5_nach_Block9, Schatten6_nach_Block9,
                                                Schatten7_nach_Block9});  //Block9: Nach rechts

            UpdateGleisbild_SchattenkleinAusf(BelegtmelderListe.GetBelegtStatus("SchattenMitte1"), new List<Fahrstrasse> { Schatten8_nach_Block7, Schatten9_nach_Block7, Schatten10_nach_Block7, Schatten11_nach_Block7 });

            
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

        private void GetBelegtMelderStatus(byte GruppenIndex)
        {
            z21Start.LAN_RMBUS_GETDATA(GruppenIndex);
        }

        
    }
}
