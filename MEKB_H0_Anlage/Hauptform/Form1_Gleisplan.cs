using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


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
        private Fahrstrasse Gleis1_nach_Block2 { set; get; }
        private Fahrstrasse Gleis2_nach_Block2 { set; get; }
        private Fahrstrasse Gleis3_nach_Block2 { set; get; }
        private Fahrstrasse Gleis4_nach_Block2 { set; get; }
        private Fahrstrasse Gleis5_nach_Block2 { set; get; }
        private Fahrstrasse Gleis6_nach_Block2 { set; get; }
        private Fahrstrasse Gleis1_nach_Block5 { set; get; }
        private Fahrstrasse Gleis2_nach_Block5 { set; get; }
        private Fahrstrasse Gleis3_nach_Block5 { set; get; }
        private Fahrstrasse Gleis4_nach_Block5 { set; get; }
        private Fahrstrasse Gleis5_nach_Block5 { set; get; }
        private Fahrstrasse Gleis6_nach_Block5 { set; get; }
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

        #endregion
        #region Weichen Setup
        //Graphics Gleisplan = null;    
        /// <summary>
        /// XML-Datei mit Weichendaten auslesen und damit eine Fahrstr_Weichenliste zu erstellen
        /// </summary>
        private void SetupWeichenListe()
        {
            XElement XMLFile = XElement.Load("Weichenliste.xml");       //XML-Datei öffnen
            var optionen = XMLFile.Elements("Optionen").ToList();
            bool Q_M = false;
            bool deaktiv = true;
            foreach (XElement werte in optionen)
            {
                Q_M = werte.Element("Q_Modus").Value.Equals("1");
                deaktiv = werte.Element("Deaktivieren").Value.Equals("1");
            }

            var list = XMLFile.Elements("Weiche").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach(XElement weiche in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                int WAdresse = Int16.Parse(weiche.Element("Adresse").Value);                                //Weichenadresse des Elements auslesen
                string WName = weiche.Element("name").Value;                                                //Weichenname des Elements auslesen
                bool Wspiegeln = (weiche.Element("spiegeln").Value == "1");                                 //Parameter für gespiegelte Weichen auslesen
                int time = 500;
                if(weiche.Element("Zeit") != null) time = Int16.Parse(weiche.Element("Zeit").Value);
                Weichenliste.Add(new Weiche() { Name = WName, Adresse = WAdresse, Spiegeln = Wspiegeln, Schaltzeit=time, Q_Modus = Q_M, Deaktivieren=deaktiv });  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
            }
        }
        #endregion
        #region Weichen Steuerung
        /// <summary>
        /// Aktuellen Status der Weiche anfordern (Nachricht senden für Weichenstatus anfordern)
        /// Mit Weichenname "Alle" werden alle Weichen der Liste abgefragt
        /// </summary>
        /// <param name="Weichenname">Name der Weiche in der Liste zum Abfragen des Status</param>
        private void GetWeichenStatus(string Weichenname)
        {
            if(Weichenname.Equals("Alle"))      //Alle Weichen ansprechen
            {
                foreach(Weiche weiche in Weichenliste)
                {
                    z21Start.Z21_GET_WEICHE(weiche.Adresse);    //Paket senden "GET Weiche"
                    Task.Delay(50);                             //50ms warten
                }
            }
            else
            {
                int ListID = Weichenliste.IndexOf(new Weiche() { Name = Weichenname }); //Weiche mit diesem Namen in der Liste suchen
                if (ListID == -1) return;                                               //Weiche nicht vorhanden, Funktion abbrechen
                int Adresse = Weichenliste[ListID].Adresse;                             //Adresse der Weiche übernehmen
                z21Start.Z21_GET_WEICHE(Adresse);                                       //paket senden "GET Weiche"
            }
        }
        /// <summary>
        /// Weiche Schalten auf Position
        /// </summary>
        /// <param name="WeichenName">Weichennamen der zu schaltenen Weiche</param>
        /// <param name="Abzweig">true: auf Abzweig schalten / false: auf Gerade schalten</param>
        private void SetWeiche(string WeichenName, bool Abzweig)
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = WeichenName });
            if (ListID == -1) return;
            int Adresse = Weichenliste[ListID].Adresse;
            bool Q_Modus = Weichenliste[ListID].Q_Modus;
            int Schaltzeit = Weichenliste[ListID].Schaltzeit;
            bool deaktiviren = Weichenliste[ListID].Deaktivieren;

            if (Weichenliste[ListID].Spiegeln) Abzweig = !Abzweig;
            if (Betriebsbereit) _ = z21Start.Z21_SET_WEICHEAsync(Adresse, Abzweig, Q_Modus, Schaltzeit,deaktiviren);
        }
        /// <summary>
        /// Weichenstellung wechseln
        /// </summary>
        /// <param name="WeichenName">Weichennamen der zu schaltenen Weiche</param>
        private void ToggleWeiche(string WeichenName)
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = WeichenName });
            if (ListID == -1) return;
            int Adresse = Weichenliste[ListID].Adresse;
            bool Abzweig = Weichenliste[ListID].Abzweig;

            if (Weichenliste[ListID].FahrstrasseAktive) return;

            Abzweig = !Abzweig;     //Toggeln

            bool Q_Modus = Weichenliste[ListID].Q_Modus;
            int Schaltzeit = Weichenliste[ListID].Schaltzeit;
            bool deaktiviren = Weichenliste[ListID].Deaktivieren;

            if (Weichenliste[ListID].Spiegeln) Abzweig = !Abzweig;
            if (Betriebsbereit) _ = z21Start.Z21_SET_WEICHEAsync(Adresse, Abzweig, Q_Modus, Schaltzeit, deaktiviren);
        }
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
                fahrstrasse.DeleteFahrstrasse(Weichenliste);
            }
            else
            {
                //Fahrstraße aktivieren
                if (Betriebsbereit) fahrstrasse.SetFahrstrasse(Weichenliste, z21Start);
            }
            //Weichenliste der Fahrstraßen übernehmen
            List<Weiche> FahrstrassenWeichen = fahrstrasse.GetFahrstrassenListe();
            //Weichenliste durchgehen
            foreach (Weiche weiche in FahrstrassenWeichen)
            {
                int ListID = Weichenliste.IndexOf(new Weiche() { Name = weiche.Name });//Weiche in der globalen Liste suchen
                if (ListID == -1) return;   //Weiche nicht gefunden
                UpdateWeicheImGleisplan(Weichenliste[ListID]); //Weiche im Gleisplan aktualisieren
            }
            //Alle Fahrstraßen/Buttons aktualisieren
            UpdateSchalterSignale();
            FahrstrasseBildUpdate();
        }
        #region Fahrstarßen Setup
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            SetupGleis1_nach_Block2();
            SetupGleis2_nach_Block2();
            SetupGleis3_nach_Block2();
            SetupGleis4_nach_Block2();
            SetupGleis5_nach_Block2();
            SetupGleis6_nach_Block2();

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
            SetupBlock5_nach_Block6();


            SetupBlock6_nach_Schatten8();
            SetupBlock6_nach_Schatten9();
            SetupBlock6_nach_Schatten10();
            SetupBlock6_nach_Schatten11();

            SetupBlock7_nach_Schatten0();
            SetupBlock7_nach_Schatten1();
            SetupBlock7_nach_Schatten2();
            SetupBlock7_nach_Schatten3();
            SetupBlock7_nach_Schatten4();
            SetupBlock7_nach_Schatten5();
        }
        #region Bahnhof Ausfahrt links
        private void SetupGleis1_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis1_nach_links /////////
            Gleis1_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupGleis2_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis2_nach_links /////////
            Gleis2_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis3_nach_links /////////
            Gleis3_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis4_nach_links /////////
            Gleis4_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis5_nach_links /////////
            Gleis5_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_Block2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis6_nach_links /////////
            Gleis6_nach_Block2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche52 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche53 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_Block2.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Einfahrt links
        private void SetupBlock2_nach_Gleis1()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis1 /////////
            Block2_nach_Gleis1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis2()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis2 /////////
            Block2_nach_Gleis2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis3()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis3 /////////
            Block2_nach_Gleis3 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis4()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis4 /////////
            Block2_nach_Gleis4 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis5()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis5 /////////
            Block2_nach_Gleis5 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock2_nach_Gleis6()
        {
            Weiche weiche;
            int ListID;
            ////////// Block2_nach_Gleis6 /////////
            Block2_nach_Gleis6 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            weiche.Status_Unbekannt = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Ausfahrt rechts
        private void SetupGleis1_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis1_nach_rechts1 /////////
            Gleis1_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis2_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis2_nach_rechts1 /////////
            Gleis2_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis3_nach_rechts1 /////////
            Gleis3_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis4_nach_rechts1 /////////
            Gleis4_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis5_nach_rechts1 /////////
            Gleis5_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_rechts1()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis6_nach_rechts1 /////////
            Gleis6_nach_rechts1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis1_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis1_nach_rechts2 /////////
            Gleis1_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis2_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis2_nach_rechts2 /////////
            Gleis2_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis3_nach_rechts2 /////////
            Gleis3_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis4_nach_rechts2 /////////
            Gleis4_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis5_nach_rechts2 /////////
            Gleis5_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_rechts2()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis6_nach_rechts2 /////////
            Gleis6_nach_rechts2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche30 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche50 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_rechts2.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #region Bahnhof Einfahrt rechts
        private void SetupRechts1_nach_Gleis1()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis1 /////////
            Rechts1_nach_Gleis1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis2()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis2 /////////
            Rechts1_nach_Gleis2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis3()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis3 /////////
            Rechts1_nach_Gleis3 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis4()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis4 /////////
            Rechts1_nach_Gleis4 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis5()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis5 /////////
            Rechts1_nach_Gleis5 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts1_nach_Gleis6()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts1_nach_Gleis6 /////////
            Rechts1_nach_Gleis6 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts1_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis1()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis1 /////////
            Rechts2_nach_Gleis1 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis1.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis2()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis2 /////////
            Rechts2_nach_Gleis2 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche26 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis3()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis3 /////////
            Rechts2_nach_Gleis3 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis4()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis4 /////////
            Rechts2_nach_Gleis4 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis5()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis5 /////////
            Rechts2_nach_Gleis5 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis5.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupRechts2_nach_Gleis6()
        {
            Weiche weiche;
            int ListID;
            ////////// rechts2_nach_Gleis6 /////////
            Rechts2_nach_Gleis6 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche51 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche29 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche28 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche27 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche25 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW24_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche23 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: KW22_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche21 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Rechts2_nach_Gleis6.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        
        private void SetupBlock1_nach_Block5()
        {
            ////////// Block1_nach_Block5 /////////
            Block1_nach_Block5 = new Fahrstrasse();
        }
        private void SetupBlock5_nach_Block6()
        {
            Weiche weiche;
            int ListID;
            ////////// Block5_nach_Block6 /////////
            Block5_nach_Block6 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche60" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche60 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Block5_nach_Block6.Fahrstr_Weichenliste.Add(weiche);

        }
        #region Schatten Einfahrt
        private void SetupBlock6_nach_Schatten8()
        {
            Weiche weiche;
            int ListID;
            ////////// Block6_nach Schattenbahnhof Gl. 8 /////////
            Block6_nach_Schatten8 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche90" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche91" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche92" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche92 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten8.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten9()
        {
            Weiche weiche;
            int ListID;
            ////////// Block6_nach Schattenbahnhof Gl. 9 /////////
            Block6_nach_Schatten9 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche90" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche91" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche92" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche92 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten9.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten10()
        {
            Weiche weiche;
            int ListID;
            ////////// Block6_nach Schattenbahnhof Gl. 10 /////////
            Block6_nach_Schatten10 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche90" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten10.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche91" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche91 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block6_nach_Schatten10.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock6_nach_Schatten11()
        {
            Weiche weiche;
            int ListID;
            ////////// Block6_nach Schattenbahnhof Gl. 11 /////////
            Block6_nach_Schatten11 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche90" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche90 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block6_nach_Schatten11.Fahrstr_Weichenliste.Add(weiche);
        }

        private void SetupSchatten11_nach_Block7()
        {
            Weiche weiche;
            int ListID;
            ////////// Schattenbahnhof Gl. 11 Ausf/////////
            Schatten11_nach_Block7 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche80" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche81" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche82" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche82 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten11_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten10_nach_Block7()
        {
            Weiche weiche;
            int ListID;
            ////////// Schattenbahnhof Gl. 10 Ausf/////////
            Schatten10_nach_Block7 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche80" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche81" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche82" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche82 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten10_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten9_nach_Block7()
        {
            Weiche weiche;
            int ListID;
            ////////// Schattenbahnhof Gl. 9 Ausf/////////
            Schatten9_nach_Block7 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche80" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Schatten9_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche81" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche81 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten9_nach_Block7.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupSchatten8_nach_Block7()
        {
            Weiche weiche;
            int ListID;
            ////////// Schattenbahnhof Gl. 9 Ausf/////////
            Schatten8_nach_Block7 = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche80" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche80 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Schatten8_nach_Block7.Fahrstr_Weichenliste.Add(weiche);

        }


        private void SetupBlock7_nach_Schatten0()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten0 /////////
            Block7_nach_Schatten0 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten0.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock7_nach_Schatten1()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten1 /////////
            Block7_nach_Schatten1 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten1.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche71" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten1.Fahrstr_Weichenliste.Add(weiche);

        }
        private void SetupBlock7_nach_Schatten2()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten2 /////////
            Block7_nach_Schatten2 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche71" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche72" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten2.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten3()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten3 /////////
            Block7_nach_Schatten3 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche71" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche72" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche73" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten3.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten4()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten4 /////////
            Block7_nach_Schatten4 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche71" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche72" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche73" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche74" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Block7_nach_Schatten4.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupBlock7_nach_Schatten5()
        {
            Weiche weiche;
            int ListID;
            ////////// Block7_nach_Schatten4 /////////
            Block7_nach_Schatten5 = new Fahrstrasse();
            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche70" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche70 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche71" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche71 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche72" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche72 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche73" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche73 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche74" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche74 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Block7_nach_Schatten5.Fahrstr_Weichenliste.Add(weiche);
        }
        #endregion
        #endregion
        #region Fahrstraßen bestimmen
        
        MeldeZustand ErrechneZustand(bool besetzt, List<Fahrstrasse> Fahrstrassen_west, List<Fahrstrasse> Fahrstrassen_ost)
        {

            int aktiv_west = 0;
            int aktiv_ost = 0;
            int safe_west = 0;
            int safe_ost = 0;

            bool richtung = false;
            bool fahrstrasseAktiv = false;
            bool sicher = false;
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

            if (aktiv_west > 1) return new MeldeZustand(false);
            if (aktiv_ost > 1) return new MeldeZustand(false);
            if (safe_west > 1) return new MeldeZustand(false);
            if (safe_ost > 1) return new MeldeZustand(false);

            if ((aktiv_west == 1) && (aktiv_ost == 1)) return new MeldeZustand(false);

            if (aktiv_west == 1) 
            { 
                richtung = WEST;
                fahrstrasseAktiv = true;
                if (safe_west == 1) sicher = true;
            }
            if (aktiv_ost == 1) 
            { 
                richtung = OST;
                fahrstrasseAktiv = true;
                if (safe_ost == 1) sicher = true;
            }


            return new MeldeZustand(besetzt, fahrstrasseAktiv, sicher, richtung);
        }

        private string ErrechneWeichenZustand(Weiche weiche, string vonZunge, string zuZunge)
        {
            string Zustand = "";

            if (weiche.FahrstrasseSicher) Zustand += "Sicher:";
            if (weiche.Besetzt) Zustand += " Besetzt";
            if (weiche.FahrstrasseAktive)
            {
                Zustand += " Fahrstrasse";
                if (weiche.FahrstrasseRichtung_vonZunge) Zustand += vonZunge;
                else Zustand += zuZunge;
            }
            if (!weiche.Besetzt && !weiche.FahrstrasseAktive)
            {
                Zustand = "Frei";
            }

            return Zustand;
        }

        private MeldeZustand Frei()
        {
            return new MeldeZustand(false, false, false, false);
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
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_links_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_links_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_links_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_links_3);
        }
        private void UpdateGleisbild_GL2_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_links_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_links_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_links_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_links_3);
        }
        private void UpdateGleisbild_GL3_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_links_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_links_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_links_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_links_3);
        }
        private void UpdateGleisbild_GL4_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_links_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_links_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_links_2);
        }
        private void UpdateGleisbild_GL5_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL5_links_0);

        }
        private void UpdateGleisbild_GL6_links(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL6_links_0);
            DisplayPicture(GetSchaltbildKurve270R(zustand), GL6_links_1);
        }
        #endregion
        #region Bahnhofsausfahrt rechts
        private void UpdateGleisbild_GL1_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL1_rechts_3);
            DisplayPicture(GetSchaltbildKurve90R(zustand), GL1_rechts_4);
            DisplayPicture(GetSchaltbildEckeUL(zustand), GL1_rechts_5);
        }
        private void UpdateGleisbild_GL2_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL2_rechts_3);
        }
        private void UpdateGleisbild_GL3_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL3_rechts_3);
        }
        private void UpdateGleisbild_GL4_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL4_rechts_2);
        }       
        private void UpdateGleisbild_GL5_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL5_rechts_0);
        }
        private void UpdateGleisbild_GL6_rechts(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), GL6_rechts_0);
        }
        #endregion
        #region Bahnhofseinfahrt links
        private void UpdateGleisbild_Block_BhfEinfahrtL(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block_BhfEinfahrt_Links);
        }
        #endregion
        #region Block 1
        private void UpdateGleisbild_Block1a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1a_1);
        }

        private void UpdateGleisbild_Block1c(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_3);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_4);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_5);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_6);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_7);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_8);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block1c_9);
            DisplayPicture(GetSchaltbildKurve270L(zustand),Block1d_0);
            DisplayPicture(GetSchaltbildEckeUR(zustand),   Block1d_1);
            DisplayPicture(GetSchaltbildEckeOL(zustand),   Block1d_2);
            DisplayPicture(GetSchaltbildKurve225L(zustand),Block1d_3);
        }
        #endregion
        #region Block 2
        private void UpdateGleisbild_Block2(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildEckeOL_Sp(zustand), Block2_1);
            DisplayPicture(GetSchaltbildEckeUR_Sp(zustand), Block2_2);
            DisplayPicture(GetSchaltbildGerade225(zustand), Block2_3);
            DisplayPicture(GetSchaltbildEckeOL_Sp(zustand), Block2_4);
            DisplayPicture(GetSchaltbildEckeUR_Sp(zustand), Block2_5);
            DisplayPicture(GetSchaltbildKurve225R(zustand), Block2_6);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block2_7);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block2_8);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block2_10);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block2_11);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block2_12);
        }
        #endregion
        #region Block 3
        private void UpdateGleisbild_Block3a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3a);
        }
        private void UpdateGleisbild_Block3b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3b_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3b_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3b_3);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3b_4);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block3b_5);
        }
        #endregion
        #region Block 4
        private void UpdateGleisbild_Block4a(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4a_1);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4a_2);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4a_3);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4a_4);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4a_5);
        }
        private void UpdateGleisbild_Block4b(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block4b_1);
        }
        #endregion
        #region Block 5
        private void UpdateGleisbild_Block5(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {           
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_0);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_1);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_2);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_3);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_4);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_5);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_6);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_7);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_8);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_9);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_10);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_11);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_12);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_13);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_14);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_15);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_16);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_17);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block5_18);
            DisplayPicture(GetSchaltbildKurve180L(zustand), Block5_19);
            DisplayPicture(GetSchaltbildEckeOR_Sp(zustand), Block5_20);
            DisplayPicture(GetSchaltbildKurve135L(zustand), Block5_21);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block5_22);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block5_23);
        }
        #endregion
        #region Block 6
        private void UpdateGleisbild_Block6(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_1);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_2);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_3);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_4);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_5);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_6);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_7);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_8);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_9);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_10);
            DisplayPicture(GetSchaltbildGerade270(zustand), Block6_11);
        }
        #endregion
        #region Block 7
        private void UpdateGleisbild_Block7(bool besetzt, List<Fahrstrasse> Fahrstrasse_links, List<Fahrstrasse> Fahrstrasse_rechts)
        {
            MeldeZustand zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block7_1);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block7_2);
            DisplayPicture(GetSchaltbildKurve0L(zustand),   Block7_3);
            DisplayPicture(GetSchaltbildEckeOR_Sp(zustand), Block7_4);
            DisplayPicture(GetSchaltbildEckeUL_Sp(zustand), Block7_5);
            DisplayPicture(GetSchaltbildKurve315L(zustand), Block7_6);
            //DisplayPicture(GetSchaltbildGerade90(zustand), Block7_7);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_8);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_9);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_10);

        }
        #endregion
        #region Schattenbahnhof
        private void UpdateGleisbild_SchattenkleinAusf(List<bool> besetzt, List<Fahrstrasse> Fahrstrassen)
        {
            MeldeZustand zustand8 = ErrechneZustand(besetzt[0], new List<Fahrstrasse> { Fahrstrassen[0] }, new List<Fahrstrasse>()); //Gleis8
            MeldeZustand zustand9 = ErrechneZustand(besetzt[1], new List<Fahrstrasse> { Fahrstrassen[1] }, new List<Fahrstrasse>()); //Gleis9
            MeldeZustand zustand10 = ErrechneZustand(besetzt[2], new List<Fahrstrasse> { Fahrstrassen[2] }, new List<Fahrstrasse>()); //Gleis10
            MeldeZustand zustand11 = ErrechneZustand(besetzt[3], new List<Fahrstrasse> { Fahrstrassen[3] }, new List<Fahrstrasse>()); //Gleis11

            /*
            DisplayPicture(GetSchaltbildGerade180(zustand), Block7_1);
            DisplayPicture(GetSchaltbildGerade180(zustand), Block7_2);
            DisplayPicture(GetSchaltbildKurve0L(zustand), Block7_3);
            DisplayPicture(GetSchaltbildEckeOR_Sp(zustand), Block7_4);
            DisplayPicture(GetSchaltbildEckeUL_Sp(zustand), Block7_5);
            DisplayPicture(GetSchaltbildKurve315L(zustand), Block7_6);
            //DisplayPicture(GetSchaltbildGerade90(zustand), Block7_7);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_8);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_9);
            DisplayPicture(GetSchaltbildGerade90(zustand), Block7_10);
            */

        }

        #endregion


        #region Block (sonder)
        private void UpdateGleisbild_Block5_Block8(bool besetzt_Block5, List<Fahrstrasse> Fahrstrasse_links_Block5, List<Fahrstrasse> Fahrstrasse_rechts_Block5, 
                                                   bool besetzt_Block7, List<Fahrstrasse> Fahrstrasse_links_Block7, List<Fahrstrasse> Fahrstrasse_rechts_Block7)
        {
            MeldeZustand Zustand_Block5 = ErrechneZustand(besetzt_Block5, Fahrstrasse_links_Block5, Fahrstrasse_rechts_Block5);
            MeldeZustand Zustand_Block7 = ErrechneZustand(besetzt_Block7, Fahrstrasse_links_Block7, Fahrstrasse_rechts_Block7);
        }
        #endregion

        #region Weichen Gleisfeldumgebung

        private void UpdateGleisbild_Weiche1()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);

            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);

            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);           
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeOR(meldeZustand),   Weiche50a_1);
                DisplayPicture(GetSchaltbildKurve270R(meldeZustand),Weiche50a_2);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50a_3);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50a_4);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50a_5);
                DisplayPicture(GetSchaltbildKurve90L(meldeZustand), Weiche50a_6);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand),Weiche50a_7);
                DisplayPicture(GetSchaltbildKurve180R(meldeZustand), Weiche50a_8);
                DisplayPicture(GetSchaltbildKurve0L(meldeZustand), Weiche50a_9);
                DisplayPicture(GetSchaltbildEckeUL_Sp(meldeZustand), Weiche50a_10);
                DisplayPicture(GetSchaltbildKurve315L(meldeZustand),Weiche50a_11); // TODO :
                DisplayPicture(GetSchaltbildGerade270(meldeZustand), Weiche50a_12);
                DisplayPicture(GetSchaltbildGerade270(meldeZustand), Weiche50a_13);

                DisplayPicture(GetSchaltbildGerade90_EckeUL(FreiesGleis, meldeZustand), Weiche50b_1);
                DisplayPicture(GetSchaltbildKurve90L_EckeUR(FreiesGleis, meldeZustand), Weiche50b_6);
                DisplayPicture(GetSchaltbildKurve180L_EckeOR(FreiesGleis, meldeZustand), Weiche50b_7);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50b_2);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50b_4);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche50b_5);
                DisplayPicture(GetSchaltbildGerade270(meldeZustand), Weiche50b_8);
                DisplayPicture(GetSchaltbildGerade270(meldeZustand), Weiche50b_9);
                DisplayPicture(GetSchaltbildGerade90_EckeUL(meldeZustand, FreiesGleis), Weiche50b_1);
                DisplayPicture(GetSchaltbildKurve90L_EckeUR(meldeZustand, FreiesGleis), Weiche50b_6);
                DisplayPicture(GetSchaltbildKurve180L_EckeOR(meldeZustand, FreiesGleis), Weiche50b_7);
            }
        }
        private void UpdateGleisbild_Weiche51()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), Weiche51a_1);
                DisplayPicture(GetSchaltbildKurve270L(meldeZustand), Weiche51a_2);
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche51a_3);
                
                DisplayPicture(GetSchaltbildGerade90_EckeOL(FreiesGleis, meldeZustand), Weiche51b_1);

            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90(meldeZustand), Weiche51b_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(meldeZustand, FreiesGleis), Weiche51b_1);
            }
        }
        private void UpdateGleisbild_Weiche52()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche52" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);

            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche53" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);          
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve0R(FreiesGleis), Weiche53_Gleis0);
                DisplayPicture(GetSchaltbildEckeOL(FreiesGleis), Weiche53_Gleis1);
                DisplayPicture(GetSchaltbildKurve225L_EckeUR(meldeZustand, FreiesGleis), Weiche53_Gleis2);

            }
            else
            {
                DisplayPicture(GetSchaltbildKurve0R(meldeZustand), Weiche53_Gleis0);
                DisplayPicture(GetSchaltbildEckeOL(meldeZustand), Weiche53_Gleis1);
                DisplayPicture(GetSchaltbildKurve225L_EckeUR(FreiesGleis, meldeZustand), Weiche53_Gleis2);
            }
        }
        private void UpdateGleisbild_Weiche60()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche60" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);           
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade270_EckeOR(FreiesGleis, meldeZustand), Weiche60_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade270_EckeOR(meldeZustand, FreiesGleis), Weiche60_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche90()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche90" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche91" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche92" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], WEST);
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(FreiesGleis, meldeZustand), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(meldeZustand), KW22_Gleis3);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(meldeZustand, FreiesGleis), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(FreiesGleis), KW22_Gleis3);
            }

        }
        private void UpdateGleisbild_KW22_2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);
            
            if (Weichenliste[ListID].Abzweig)
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
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) return;
            MeldeZustand meldeZustand = new MeldeZustand(Weichenliste[ListID], OST);

            if (Weichenliste[ListID].Abzweig)
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

        private void ZeichneFahrstraße(ref Graphics gleisbild, Image Type, Color Farbe)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 32;
            int height = 32;
            ColorMap colorMap = new ColorMap
            {
                OldColor = Color.FromArgb(255, 255, 255, 0),  // original gelb
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
            return bild; //Bild ausgeben
        }
        #endregion
        #region nur Geraden
        private dynamic GetSchaltbildGerade0(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade0; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }     
        private dynamic GetSchaltbildGerade45(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade45; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade180(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade0; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade225(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade45; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade135(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade135; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade270(MeldeZustand Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            if (Zustand.IstFrei()) return bild;

            Image zeichenmuster;
            Color farbe = Farbe_Grau;

            if ((Zustand.Besetzt == true) && (Zustand.Fahrstrasse == false))
            {
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
            }

            return bild; //Bild ausgeben
        }
        #endregion
        #region Geraden mit zwei Ecken
        private dynamic GetSchaltbildGerade90_EckeOL_UR(MeldeZustand Zustand_Gerade, MeldeZustand Zustand_Ecke)
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
            }
            else
            {
                if (Zustand_Gerade.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts;

                if (Zustand_Gerade.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Gerade.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Gerade.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Gerade.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
                zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
                farbe = Farbe_Rot;
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links;

                if (Zustand.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand.Sicher) farbe = Farbe_Gruen;
                if (Zustand.Besetzt) farbe = Farbe_Rot;
            }
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            }
            else
            {
                if (Zustand_Kurve.Richtung == WEST) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts;

                if (Zustand_Kurve.Fahrstrasse) farbe = Farbe_Gelb;
                if (Zustand_Kurve.Sicher) farbe = Farbe_Gruen;
                if (Zustand_Kurve.Besetzt) farbe = Farbe_Rot;
            }

            if (!Zustand_Kurve.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);


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
                if (!Zustand_Ecke.IstFrei()) ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_180;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);

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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
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

            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_180;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_0;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (weiche.Abzweig) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270, 0, 0);   //Abzweig
            else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);                    //Gerade

            Image zeichenmuster;
            Color farbe = Farbe_Grau;
            if ((weiche.Besetzt == false) && (weiche.FahrstrasseAktive == false))
            {
                return bild;
            }
            else if ((weiche.Besetzt == true) && (weiche.FahrstrasseAktive == false))
            {
                farbe = Farbe_Rot;
                if (weiche.Abzweig) zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270;
                else zeichenmuster = MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90;
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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            ZeichneFahrstraße(ref gleis, zunge, Farbe_Weis);

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
            ZeichneFahrstraße(ref gleis, zeichenmuster, farbe);
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
            if (fahrstrasse.GetGesetztStatus())    //Fahrstraße wurde gesetzt
            {
                //Prüfen ob alle Weichen der Fahrstraßen richtig geschaltet sind
                if (fahrstrasse.CheckFahrstrasse(Weichenliste) == false) //Noch nicht alle Weichen gestellt
                {
                    //Alle Weichen Schalten, falls Instance nicht am  schalten ist.
                    if (fahrstrasse.Busy == false)
                    {
                        if (Betriebsbereit) fahrstrasse.SetFahrstrasse(Weichenliste, z21Start);
                    }
                }
                else //Alle Weichen in richtiger Stellung
                {
                    //Fahrstraße als aktiviert kennzeichnen
                    fahrstrasse.AktiviereFahrstasse(Weichenliste);
                    //Jede Weiche in der Fahrstraßenliste durchlaufen
                    foreach (Weiche weiche in fahrstrasse.Fahrstr_Weichenliste)
                    {
                        int ListID = Weichenliste.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                        if (ListID == -1) return;   //Weichen nicht gefunden - Funktion abbrechen
                        UpdateWeicheImGleisplan(Weichenliste[ListID]);  //Weichenbild aktualisieren
                    }

                    //Weichen zyklisch nochmal schalten um hängenbleiben zu vermeiden
                    if (Betriebsbereit) fahrstrasse.ControlSetFahrstrasse(Weichenliste, z21Start);
                }
            }
        }
        /// <summary>
        /// Fahrstraßen im Bild zeichnen
        /// </summary>
        private void FahrstrasseBildUpdate()
        {
            //Hauptbahnhof - Linker Teil der Gleise
            UpdateGleisbild_GL1_links(false, new List<Fahrstrasse> { Gleis1_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis1 });
            UpdateGleisbild_GL2_links(false, new List<Fahrstrasse> { Gleis2_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis2 });
            UpdateGleisbild_GL3_links(false, new List<Fahrstrasse> { Gleis3_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis3 });
            UpdateGleisbild_GL4_links(false, new List<Fahrstrasse> { Gleis4_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis4 });
            UpdateGleisbild_GL5_links(false, new List<Fahrstrasse> { Gleis5_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis5 });
            UpdateGleisbild_GL6_links(false, new List<Fahrstrasse> { Gleis6_nach_Block2 }, new List<Fahrstrasse> { Block2_nach_Gleis6 });

            //Hauptbahnhof - Rechter Teil der Gleise
            UpdateGleisbild_GL1_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts2_nach_Gleis1 } , new List<Fahrstrasse> { Gleis1_nach_rechts1 , Gleis1_nach_rechts2 });
            UpdateGleisbild_GL2_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis2, Rechts2_nach_Gleis2 } , new List<Fahrstrasse> { Gleis2_nach_rechts1 , Gleis2_nach_rechts2 });
            UpdateGleisbild_GL3_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis3, Rechts2_nach_Gleis3 } , new List<Fahrstrasse> { Gleis3_nach_rechts1 , Gleis3_nach_rechts2 });
            UpdateGleisbild_GL4_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis4, Rechts2_nach_Gleis4 } , new List<Fahrstrasse> { Gleis4_nach_rechts1 , Gleis4_nach_rechts2 });
            UpdateGleisbild_GL5_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis5, Rechts2_nach_Gleis5 } , new List<Fahrstrasse> { Gleis5_nach_rechts1 , Gleis5_nach_rechts2 });
            UpdateGleisbild_GL6_rechts(false, new List<Fahrstrasse> { Rechts1_nach_Gleis6, Rechts2_nach_Gleis6 } , new List<Fahrstrasse> { Gleis6_nach_rechts1 , Gleis6_nach_rechts2 });

            //Gleise im Block 1 aktualisieren
            UpdateGleisbild_Block1a(false, //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block2, Gleis2_nach_Block2 },//Nach links
                                    new List<Fahrstrasse> { Block2_nach_Gleis1, Block2_nach_Gleis2 });//Nach rechts

            UpdateGleisbild_Block1c(false, //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block2, Gleis2_nach_Block2, Gleis3_nach_Block2, //Nach links
                                                            Gleis4_nach_Block2, Gleis5_nach_Block2, Gleis6_nach_Block2 },
                                    new List<Fahrstrasse>()); //nach rechts immer aus
            UpdateGleisbild_Block2(false, //Besetzt
                                    new List<Fahrstrasse> { Gleis1_nach_Block2, Gleis2_nach_Block2, Gleis3_nach_Block2, //Nach links
                                                            Gleis4_nach_Block2, Gleis5_nach_Block2, Gleis6_nach_Block2 },
                                    new List<Fahrstrasse>()); //nach rechts immer aus
            //Gleise im Block 2 aktualisieren
            UpdateGleisbild_Block_BhfEinfahrtL(false, //Besetzt
                                    new List<Fahrstrasse>  {Gleis3_nach_Block2, Gleis4_nach_Block2, Gleis5_nach_Block2, Gleis6_nach_Block2 },
                                    new List<Fahrstrasse> { Block2_nach_Gleis3, Block2_nach_Gleis4, Block2_nach_Gleis5, Block2_nach_Gleis6 });
            //Gleise im Block 3 aktualisieren
            UpdateGleisbild_Block3a(false, //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis3, Rechts1_nach_Gleis4, Rechts1_nach_Gleis5, Rechts1_nach_Gleis6, //Nach links
                                                            Rechts2_nach_Gleis3, Rechts2_nach_Gleis4, Rechts2_nach_Gleis5, Rechts2_nach_Gleis6 },
                                    new List<Fahrstrasse> { Gleis3_nach_rechts1, Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1, //Nach rechts
                                                            Gleis3_nach_rechts2, Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2 });
            UpdateGleisbild_Block3b(false, //Besetzt
                                    new List<Fahrstrasse> (),//Nie nach links
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis3_nach_rechts1, //Nach rechts
                                                            Gleis4_nach_rechts1, Gleis5_nach_rechts1, Gleis6_nach_rechts1, 
                                                            Gleis1_nach_rechts2, Gleis2_nach_rechts2, Gleis3_nach_rechts2, 
                                                            Gleis4_nach_rechts2, Gleis5_nach_rechts2, Gleis6_nach_rechts2});
            //Gleise im Block 4 aktualisieren                        
            UpdateGleisbild_Block4a(false, //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts1_nach_Gleis2, Rechts1_nach_Gleis3, //Nach links
                                                            Rechts1_nach_Gleis4, Rechts1_nach_Gleis5, Rechts1_nach_Gleis6, 
                                                            Rechts2_nach_Gleis1, Rechts2_nach_Gleis2, Rechts2_nach_Gleis3, //Nach rechts
                                                            Rechts2_nach_Gleis4, Rechts2_nach_Gleis5, Rechts2_nach_Gleis6 },
                                    new List<Fahrstrasse>());     //nie nach rechts
            UpdateGleisbild_Block4b(false, //Besetzt
                                    new List<Fahrstrasse> { Rechts1_nach_Gleis1, Rechts1_nach_Gleis2, Rechts2_nach_Gleis1, Rechts2_nach_Gleis2 }, //Nach links
                                    new List<Fahrstrasse> { Gleis1_nach_rechts1, Gleis2_nach_rechts1, Gleis1_nach_rechts2, Gleis2_nach_rechts2 });//Nach rechts
            UpdateGleisbild_Block5(false, //Besetzt
                                    new List<Fahrstrasse> { Block1_nach_Block5 } ,  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block6(false, //Besetzt
                                    new List<Fahrstrasse> { Block5_nach_Block6 },  //Nach links
                                    new List<Fahrstrasse>());     //nie nach rechts

            UpdateGleisbild_Block7(false, //Besetzt
                                    new List<Fahrstrasse>(),  //nie nach links
                                    new List<Fahrstrasse>()); //nach rechts

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

            UpdateGleisbild_Weiche90();     //Umfeld um Weiche 90
            UpdateGleisbild_Weiche91();     //Umfeld um Weiche 91
            UpdateGleisbild_Weiche92();     //Umfeld um Weiche 92

            UpdateGleisbild_Block5_Block8(false, //Block6: Besetzt
                        new List<Fahrstrasse> { Block1_nach_Block5 },  //Block6: Nach links
                        new List<Fahrstrasse>(), //Block6: nie nach rechts
                        false, //Block7: Besetzt
                        new List<Fahrstrasse> { Block1_nach_Block5 },  //Block7: Nach links
                        new List<Fahrstrasse>());//Block7: nie nach rechts

            UpdateGleisbild_SchattenkleinAusf(new List<bool> { false, false, false, false }, new List<Fahrstrasse> { Block6_nach_Schatten8, Block6_nach_Schatten8, Block6_nach_Schatten8, Block6_nach_Schatten8 });
        }
    }
}
