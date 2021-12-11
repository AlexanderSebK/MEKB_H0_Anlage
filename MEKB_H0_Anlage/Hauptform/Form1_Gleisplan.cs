using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        //Fahrstraßen Instanzen
        private Fahrstrasse Gleis1_nach_links { set; get; }
        private Fahrstrasse Gleis2_nach_links { set; get; }
        private Fahrstrasse Gleis3_nach_links { set; get; }
        private Fahrstrasse Gleis4_nach_links { set; get; }
        private Fahrstrasse Gleis5_nach_links { set; get; }
        private Fahrstrasse Gleis6_nach_links { set; get; }
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
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            SetupGleis1_nach_links();
            SetupGleis2_nach_links();
            SetupGleis3_nach_links();
            SetupGleis4_nach_links();
            SetupGleis5_nach_links();
            SetupGleis6_nach_links();

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

        }
        private void SetupGleis1_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis1_nach_links /////////
            Gleis1_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis1_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis1_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis2_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis2_nach_links /////////
            Gleis2_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche4 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche6 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis2_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis3_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis3_nach_links /////////
            Gleis3_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis3_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis3_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis4_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis4_nach_links /////////
            Gleis4_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis4_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis5_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis5_nach_links /////////
            Gleis5_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis5_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
        private void SetupGleis6_nach_links()
        {
            Weiche weiche;
            int ListID;
            ////////// Gleis6_nach_links /////////
            Gleis6_nach_links = new Fahrstrasse();

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            weiche.Status_Unbekannt = false;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche3 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche5 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW7_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: Weiche8 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = false;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_1 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = true;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) { MessageBox.Show("Schwerer Error: DKW9_2 nicht gefunden", "Schwerer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            weiche = Weichenliste[ListID].Copy();
            weiche.FahrstrasseRichtung_vonZunge = false;
            weiche.FahrstrasseAbzweig = true;
            Gleis6_nach_links.Fahrstr_Weichenliste.Add(weiche);
        }
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
        /// <summary>
        /// Zustand in einen Stringnamen umwandeln
        /// </summary>
        /// <param name="besetzt">true - Gleisbesetzmelder aktiv</param>
        /// <param name="Fahrstrasse_links">Fahrstraße in die linke Richtung aktivieren</param>
        /// <param name="Fahrstrasse_rechts">Fahrstraße in die rechte Richtung aktivieren</param>
        /// <returns>Zustand als Stringname zurückgeben</returns>
        private string ErrechneZustand(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand;
            if (besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Fahrstrasse_links)
            {
                Zustand = "Fahrstrasse <=";
            }
            else if (Fahrstrasse_rechts)
            {
                Zustand = "Fahrstrasse =>";
            }
            else
            {
                Zustand = "Frei";
            }
            return Zustand;
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
        private void UpdateGleisbild_GL1_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt,Fahrstrasse_links,Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_links_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_links_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_links_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_links_3);
        }
        private void UpdateGleisbild_GL2_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_links_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_links_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_links_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_links_3);
        }
        private void UpdateGleisbild_GL3_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_links_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_links_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_links_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_links_3);
        }
        private void UpdateGleisbild_GL4_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_links_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_links_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_links_2);
        }
        private void UpdateGleisbild_GL5_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL5_links_0);

        }
        private void UpdateGleisbild_GL6_links(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL6_links_0);
            DisplayPicture(GetSchaltbildKurve270R(Zustand), GL6_links_1);
        }
        private void UpdateGleisbild_GL1_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL1_rechts_3);
            DisplayPicture(GetSchaltbildKurve90R(Zustand), GL1_rechts_4);
            DisplayPicture(GetSchaltbildEckeUL(Zustand), GL1_rechts_5);
        }
        private void UpdateGleisbild_GL2_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL2_rechts_3);
        }
        private void UpdateGleisbild_GL3_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_rechts_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL3_rechts_3);
        }
        private void UpdateGleisbild_GL4_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_rechts_0);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_rechts_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL4_rechts_2);
        }       
        private void UpdateGleisbild_GL5_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL5_rechts_0);
        }
        private void UpdateGleisbild_GL6_rechts(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), GL6_rechts_0);
        }
        private void UpdateGleisbild_Block1a(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1a_1);
        }
        private void UpdateGleisbild_Block1b(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);

            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche4" });
            if (ListID == -1) return;
            Weiche W4 = Weichenliste[ListID];

            ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche1" });
            if (ListID == -1) return;
            Weiche W1 = Weichenliste[ListID];

            if (W1.FahrstrasseAktive)
            {
                if (W1.FahrstrasseAbzweig) DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Block1b_2);
                else DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Block1b_2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", "Frei"),Block1b_2);
            }


            if (W4.FahrstrasseAktive)
            {
                if (W4.FahrstrasseAbzweig) DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", Zustand), Block1b_1);
                else DisplayPicture(GetSchaltbildGerade90_EckeUR(Zustand, "Frei"), Block1b_1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", "Frei"), Block1b_1);
            }
        }
        private void UpdateGleisbild_Block1c(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_3);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_4);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_5);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_6);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_7);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_8);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block1c_9);
        }
        private void UpdateGleisbild_Block2(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block2);
        }
        private void UpdateGleisbild_Block3a(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3a);
        }
        private void UpdateGleisbild_Block3b(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3b_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3b_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3b_3);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3b_4);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block3b_5);
        }
        private void UpdateGleisbild_Block4a(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4a_1);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4a_2);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4a_3);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4a_4);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4a_5);
        }
        private void UpdateGleisbild_Block4b(bool besetzt, bool Fahrstrasse_links, bool Fahrstrasse_rechts)
        {
            string Zustand = ErrechneZustand(besetzt, Fahrstrasse_links, Fahrstrasse_rechts);
            DisplayPicture(GetSchaltbildGerade90(Zustand), Block4b_1);
        }
        private void UpdateGleisbild_Weiche2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche2" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR("Frei", Zustand), Weiche2_Gleis);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche2_Gleis);               
            }

        }
        private void UpdateGleisbild_Weiche3()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche3" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche3_Gleis);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche3_Gleis);
            }
        }
        private void UpdateGleisbild_Weiche5()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche5" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Weiche5_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Weiche5_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche6()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche6" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }
                    
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve270L(Zustand), Weiche6_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(Zustand), Weiche6_Gleis3);
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche6_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildKurve270L("Frei"), Weiche6_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR("Frei"), Weiche6_Gleis3);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche6_Gleis1);
            }

        }
        private void UpdateGleisbild_WeicheDKW7_1()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_1" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR("Frei", Zustand), Weiche7_Gleis2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche7_Gleis2);
            }
        }
        private void UpdateGleisbild_WeicheDKW7_2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW7_2" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Weiche7_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Weiche7_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche8()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche8" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche8_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR("Frei", Zustand), Weiche8_Gleis1);
            }
        }
        private void UpdateGleisbild_WeicheDKW9_1()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_1" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildKurve270L_EckeOR("Frei", Zustand), Weiche9_Gleis1);
                DisplayPicture(GetSchaltbildEckeUL(Zustand), Weiche9_Gleis4);
            }
            else
            {
                DisplayPicture(GetSchaltbildKurve270L_EckeOR(Zustand, "Frei"), Weiche9_Gleis1);
                DisplayPicture(GetSchaltbildEckeUL("Frei"), Weiche9_Gleis4);
            }

        }
        private void UpdateGleisbild_WeicheDKW9_2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW9_2" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Weiche9_Gleis2);
                DisplayPicture(GetSchaltbildEckeOR(Zustand), Weiche9_Gleis3);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Weiche9_Gleis2);
                DisplayPicture(GetSchaltbildEckeOR("Frei"), Weiche9_Gleis3);
            }
        }
        private void UpdateGleisbild_Weiche21()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche21" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche21_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche21_Gleis1);
            }
        }
        private void UpdateGleisbild_KW22_1()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_1" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR("Frei", Zustand), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR(Zustand), KW22_Gleis3);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL_UR(Zustand, "Frei"), KW22_Gleis2);
                DisplayPicture(GetSchaltbildEckeUR("Frei"), KW22_Gleis3);
            }

        }
        private void UpdateGleisbild_KW22_2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "KW22_2" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", Zustand), KW22_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(Zustand, "Frei"), KW22_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche23()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche23" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche23_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche23_Gleis1);
            }
        }
        private void UpdateGleisbild_DKW24_1()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_1" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), DKW24_Gleis2);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), DKW24_Gleis2);
            }
        }
        private void UpdateGleisbild_DKW24_2()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "DKW24_2" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }
            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", Zustand), DKW24_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(Zustand, "Frei"), DKW24_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche25()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche25" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", Zustand), Weiche25_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(Zustand, "Frei"), Weiche25_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche26()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche26" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR("Frei", Zustand), Weiche26_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche26_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche27()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche27" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche27_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche27_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche28()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche28" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR("Frei", Zustand), Weiche28_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUR(Zustand, "Frei"), Weiche28_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche29()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche29" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Weiche29_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Weiche29_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche30()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche30" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR("Frei", Zustand), Weiche30_Gleis1);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90_EckeOR(Zustand, "Frei"), Weiche30_Gleis1);
            }
        }
        private void UpdateGleisbild_Weiche50()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche50" });
            if (ListID == -1) return;
            string Zustand;
            string ZustandSpiegel;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
                ZustandSpiegel = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                    ZustandSpiegel = "Fahrstrasse <=";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                    ZustandSpiegel = "Fahrstrasse =>";
                }

            }
            else
            {
                Zustand = "Frei";
                ZustandSpiegel = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeOR(Zustand),   Weiche50a_1);
                DisplayPicture(GetSchaltbildKurve270R(Zustand),Weiche50a_2);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50a_3);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50a_4);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50a_5);
                DisplayPicture(GetSchaltbildKurve90L(Zustand), Weiche50a_6);
                DisplayPicture(GetSchaltbildEckeOL(Zustand),Weiche50a_7);
                DisplayPicture(GetSchaltbildKurve180R(Zustand), Weiche50a_8);
                DisplayPicture(GetSchaltbildKurve0L(ZustandSpiegel), Weiche50a_9);
                DisplayPicture(GetSchaltbildEckeUL(ZustandSpiegel), Weiche50a_10);
                DisplayPicture(GetSchaltbildKurve90R(ZustandSpiegel),Weiche50a_11);
                DisplayPicture(GetSchaltbildGerade90(ZustandSpiegel), Weiche50a_12);
                DisplayPicture(GetSchaltbildGerade90(ZustandSpiegel), Weiche50a_13);

                DisplayPicture(GetSchaltbildGerade90_EckeUL("Frei", Zustand), Weiche50b_1);
                DisplayPicture(GetSchaltbildKurve90L_EckeUR("Frei", Zustand), Weiche50b_6);
                DisplayPicture(GetSchaltbildKurve90R_EckeOR("Frei", ZustandSpiegel), Weiche50b_7);
            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50b_2);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50b_4);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche50b_5);
                DisplayPicture(GetSchaltbildGerade90(ZustandSpiegel), Weiche50b_8);
                DisplayPicture(GetSchaltbildGerade90(ZustandSpiegel), Weiche50b_9);
                DisplayPicture(GetSchaltbildGerade90_EckeUL(Zustand, "Frei"), Weiche50b_1);
                DisplayPicture(GetSchaltbildKurve90L_EckeUR(Zustand, "Frei"), Weiche50b_6);
                DisplayPicture(GetSchaltbildKurve90R_EckeOR(ZustandSpiegel, "Frei"), Weiche50b_7);
            }
        }
        private void UpdateGleisbild_Weiche51()
        {
            int ListID = Weichenliste.IndexOf(new Weiche() { Name = "Weiche51" });
            if (ListID == -1) return;
            string Zustand;
            if (Weichenliste[ListID].Besetzt)
            {
                Zustand = "Besetzt";
            }
            else if (Weichenliste[ListID].FahrstrasseAktive)
            {
                if (Weichenliste[ListID].FahrstrasseRichtung_vonZunge)
                {
                    Zustand = "Fahrstrasse =>";
                }
                else
                {
                    Zustand = "Fahrstrasse <=";
                }

            }
            else
            {
                Zustand = "Frei";
            }
            if (Weichenliste[ListID].Abzweig)
            {
                DisplayPicture(GetSchaltbildEckeUR(Zustand), Weiche51a_1);
                DisplayPicture(GetSchaltbildKurve270L(Zustand), Weiche51a_2);
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche51a_3);
                
                DisplayPicture(GetSchaltbildGerade90_EckeOL("Frei", Zustand), Weiche51b_1);

            }
            else
            {
                DisplayPicture(GetSchaltbildGerade90(Zustand), Weiche51b_2);
                DisplayPicture(GetSchaltbildGerade90_EckeOL(Zustand, "Frei"), Weiche51b_1);
            }
        }
        private dynamic GetSchaltbildEckeUR(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.EckeUR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts, 0, 0); break; //Fahrstraße malen
                default: return MEKB_H0_Anlage.Properties.Resources.EckeUR;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOR(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.EckeOR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts, 0, 0); break; //Fahrstraße malen
                default: return MEKB_H0_Anlage.Properties.Resources.EckeOR;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeOL(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeOL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts, 0, 0); break; //Fahrstraße malen
                default: return MEKB_H0_Anlage.Properties.Resources.EckeOL;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildEckeUL(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.EckeUL; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.EckeUL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts, 0, 0); break; //Fahrstraße malen
                default: return MEKB_H0_Anlage.Properties.Resources.EckeUL;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
                       
            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links,0,0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeOL(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeOL_UR(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL_UR;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL_UR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL_UR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOL_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeOR(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links,0,0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links,0,0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeUR(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeUR;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildGerade90_EckeUL(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeUL;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Gerade90_EckeOL;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUL_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve0L(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve0L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve0L;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_oben, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve0L_unten, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90L(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90L;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90L_EckeUR(String Zustand, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90L_EckeUR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90L_EckeUR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90L_EckeUR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeUR_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270L(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270L; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve270L;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90R(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90R;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve90R_EckeOR(String Zustand,String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve90R_EckeOR; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90R_EckeOR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve90R_EckeOR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts, 0, 0); break; //Fahrstraße malen
                default: break ;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve180R(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve180R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve180R;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_unten, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180R_oben, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270L_EckeOR(String Zustand_Gerade, String Zustand_Ecke)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270L_EckeOR;
            Graphics gleis = Graphics.FromImage(bild);

            switch (Zustand_Gerade)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve270L_EckeOR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            switch (Zustand_Ecke)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve270L_EckeOR;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_EckeOR_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
        private dynamic GetSchaltbildKurve270R(String Zustand)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.Kurve270R; //Gleisbild
            Graphics gleis = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            switch (Zustand)
            {
                case "Besetzt": return MEKB_H0_Anlage.Properties.Resources.Kurve270L;//_besetzt
                case "Fahrstrasse <=": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links, 0, 0); break; //Fahrstraße malen
                case "Fahrstrasse =>": gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts, 0, 0); break; //Fahrstraße malen
                default: break;
            }
            return bild; //Bild ausgeben
        }
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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_unten, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve180L_oben, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_unten, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_0_oben, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts, 0, 0);
                }
            }

            return bild;
        }
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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts, 0, 0);
                }
            }

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

            if (weiche.FahrstrasseAktive)
            {
                if (weiche.Abzweig)
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts, 0, 0);
                }
                else
                {
                    if (weiche.FahrstrasseRichtung_vonZunge) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                    else gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                }
            }

            return bild;
        }
        private dynamic GetSchaltbildKW90_45(Weiche DKW1, Weiche DKW2)
        {
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.WeicheKW90_45_Analog;
            Graphics gleis = Graphics.FromImage(bild);

            //Bei Fehler: Error-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Error || DKW2.Status_Error) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }

            //Bei Unebkannt: Unebkannt-Symbol laden und Funktion abbrechen
            if (DKW1.Status_Unbekannt || DKW2.Status_Unbekannt) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Unbekannt, 0, 0); return bild; }

            //Weichenzunge zeichnen
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Error, 0, 0); return bild; }
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);

            if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive)
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links, 0, 0);
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts, 0, 0);
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                }

            }
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
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_45, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_270, 0, 0); return bild; }
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigL_90, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);

            if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive)
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_links, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_links, 0, 0);
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_links, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_225_rechts, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270L_rechts, 0, 0);
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90L_rechts, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                }

            }
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
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_135, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_90, 0, 0);}
            if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_AbzweigR_270, 0, 0);
            if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Zunge_Gerade_90, 0, 0);

            if (DKW1.FahrstrasseAktive || DKW2.FahrstrasseAktive)
            {
                if (DKW1.FahrstrasseRichtung_vonZunge)
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_links, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_links, 0, 0); }
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_links, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_links, 0, 0);
                }
                else
                {
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == true)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_135_rechts, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == true)) { gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve90R_rechts, 0, 0); }
                    if ((DKW1.Abzweig == true) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_Kurve270R_rechts, 0, 0);
                    if ((DKW1.Abzweig == false) && (DKW2.Abzweig == false)) gleis.DrawImage(MEKB_H0_Anlage.Properties.Resources.Fahrstr_90_rechts, 0, 0);
                }
            }
            return bild;
        }
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
            UpdateGleisbild_GL1_links(false, Gleis1_nach_links.GetAktivStatus(), Block2_nach_Gleis1.GetAktivStatus());
            UpdateGleisbild_GL2_links(false, Gleis2_nach_links.GetAktivStatus(), Block2_nach_Gleis2.GetAktivStatus());
            UpdateGleisbild_GL3_links(false, Gleis3_nach_links.GetAktivStatus(), Block2_nach_Gleis3.GetAktivStatus());
            UpdateGleisbild_GL4_links(false, Gleis4_nach_links.GetAktivStatus(), Block2_nach_Gleis4.GetAktivStatus());
            UpdateGleisbild_GL5_links(false, Gleis5_nach_links.GetAktivStatus(), Block2_nach_Gleis5.GetAktivStatus());
            UpdateGleisbild_GL6_links(false, Gleis6_nach_links.GetAktivStatus(), Block2_nach_Gleis6.GetAktivStatus());

            //Hauptbahnhof - Rechter Teil der Gleise
            UpdateGleisbild_GL1_rechts(false, Rechts1_nach_Gleis1.GetAktivStatus() || Rechts2_nach_Gleis1.GetAktivStatus(), Gleis1_nach_rechts1.GetAktivStatus() || Gleis1_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_GL2_rechts(false, Rechts1_nach_Gleis2.GetAktivStatus() || Rechts2_nach_Gleis2.GetAktivStatus(), Gleis2_nach_rechts1.GetAktivStatus() || Gleis2_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_GL3_rechts(false, Rechts1_nach_Gleis3.GetAktivStatus() || Rechts2_nach_Gleis3.GetAktivStatus(), Gleis3_nach_rechts1.GetAktivStatus() || Gleis3_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_GL4_rechts(false, Rechts1_nach_Gleis4.GetAktivStatus() || Rechts2_nach_Gleis4.GetAktivStatus(), Gleis4_nach_rechts1.GetAktivStatus() || Gleis4_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_GL5_rechts(false, Rechts1_nach_Gleis5.GetAktivStatus() || Rechts2_nach_Gleis5.GetAktivStatus(), Gleis5_nach_rechts1.GetAktivStatus() || Gleis5_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_GL6_rechts(false, Rechts1_nach_Gleis6.GetAktivStatus() || Rechts2_nach_Gleis6.GetAktivStatus(), Gleis6_nach_rechts1.GetAktivStatus() || Gleis6_nach_rechts2.GetAktivStatus());

            //Gleise im Block 1 aktualisieren
            UpdateGleisbild_Block1a(false, //Besetzt
                                    Gleis1_nach_links.GetAktivStatus() || //Nach links
                                    Gleis2_nach_links.GetAktivStatus(),
                                    Block2_nach_Gleis1.GetAktivStatus() || //Nach rechts
                                    Block2_nach_Gleis2.GetAktivStatus());
            UpdateGleisbild_Block1b(false, //Besetzt
                                    Gleis1_nach_links.GetAktivStatus() || //Nach links
                                    Gleis2_nach_links.GetAktivStatus() ||
                                    Gleis3_nach_links.GetAktivStatus() ||
                                    Gleis4_nach_links.GetAktivStatus() ||
                                    Gleis5_nach_links.GetAktivStatus() ||
                                    Gleis6_nach_links.GetAktivStatus(),
                                    Block2_nach_Gleis1.GetAktivStatus() || //Nach rechts
                                    Block2_nach_Gleis2.GetAktivStatus());
            UpdateGleisbild_Block1c(false, //Besetzt
                                    Gleis1_nach_links.GetAktivStatus() || //Nach links
                                    Gleis2_nach_links.GetAktivStatus() ||
                                    Gleis3_nach_links.GetAktivStatus() ||
                                    Gleis4_nach_links.GetAktivStatus() ||
                                    Gleis5_nach_links.GetAktivStatus() ||
                                    Gleis6_nach_links.GetAktivStatus(),
                                    false); //nach rechts immer aus
            //Gleise im Block 2 aktualisieren
            UpdateGleisbild_Block2(false, //Besetzt
                                    Gleis3_nach_links.GetAktivStatus() || //Nach links
                                    Gleis4_nach_links.GetAktivStatus() ||
                                    Gleis5_nach_links.GetAktivStatus() ||
                                    Gleis6_nach_links.GetAktivStatus(),
                                    Block2_nach_Gleis3.GetAktivStatus() ||//Nach rechts
                                    Block2_nach_Gleis4.GetAktivStatus() ||
                                    Block2_nach_Gleis5.GetAktivStatus() ||
                                    Block2_nach_Gleis6.GetAktivStatus());
            //Gleise im Block 3 aktualisieren
            UpdateGleisbild_Block3a(false, //Besetzt
                                    Rechts1_nach_Gleis3.GetAktivStatus() || //Nach links
                                    Rechts1_nach_Gleis4.GetAktivStatus() ||
                                    Rechts1_nach_Gleis5.GetAktivStatus() ||
                                    Rechts1_nach_Gleis6.GetAktivStatus() ||
                                    Rechts2_nach_Gleis3.GetAktivStatus() ||
                                    Rechts2_nach_Gleis4.GetAktivStatus() ||
                                    Rechts2_nach_Gleis5.GetAktivStatus() ||
                                    Rechts2_nach_Gleis6.GetAktivStatus(),
                                    Gleis3_nach_rechts1.GetAktivStatus() || //Nach rechts
                                    Gleis4_nach_rechts1.GetAktivStatus() ||
                                    Gleis5_nach_rechts1.GetAktivStatus() ||
                                    Gleis6_nach_rechts1.GetAktivStatus() ||
                                    Gleis3_nach_rechts2.GetAktivStatus() ||
                                    Gleis4_nach_rechts2.GetAktivStatus() ||
                                    Gleis5_nach_rechts2.GetAktivStatus() ||
                                    Gleis6_nach_rechts2.GetAktivStatus());
            UpdateGleisbild_Block3b(false, //Besetzt
                                    false,//Nie nach links
                                    Gleis1_nach_rechts1.GetAktivStatus() || //Nach rechts
                                    Gleis2_nach_rechts1.GetAktivStatus() ||
                                    Gleis3_nach_rechts1.GetAktivStatus() ||
                                    Gleis4_nach_rechts1.GetAktivStatus() ||
                                    Gleis5_nach_rechts1.GetAktivStatus() ||
                                    Gleis6_nach_rechts1.GetAktivStatus() ||
                                    Gleis1_nach_rechts2.GetAktivStatus() ||
                                    Gleis2_nach_rechts2.GetAktivStatus() ||
                                    Gleis3_nach_rechts2.GetAktivStatus() ||
                                    Gleis4_nach_rechts2.GetAktivStatus() ||
                                    Gleis5_nach_rechts2.GetAktivStatus() ||
                                    Gleis6_nach_rechts2.GetAktivStatus());
            //Gleise im Block 4 aktualisieren                        
            UpdateGleisbild_Block4a(false, //Besetzt
                                    Rechts1_nach_Gleis1.GetAktivStatus() || //Nach links
                                    Rechts1_nach_Gleis2.GetAktivStatus() ||
                                    Rechts1_nach_Gleis3.GetAktivStatus() ||
                                    Rechts1_nach_Gleis4.GetAktivStatus() ||
                                    Rechts1_nach_Gleis5.GetAktivStatus() ||
                                    Rechts1_nach_Gleis6.GetAktivStatus() ||
                                    Rechts2_nach_Gleis1.GetAktivStatus() ||
                                    Rechts2_nach_Gleis2.GetAktivStatus() ||
                                    Rechts2_nach_Gleis3.GetAktivStatus() ||
                                    Rechts2_nach_Gleis4.GetAktivStatus() ||
                                    Rechts2_nach_Gleis5.GetAktivStatus() ||
                                    Rechts2_nach_Gleis6.GetAktivStatus(),
                                    false);     //nie nach rechts
            UpdateGleisbild_Block4b(false, //Besetzt
                                    Rechts1_nach_Gleis1.GetAktivStatus() || //Nach links
                                    Rechts1_nach_Gleis2.GetAktivStatus() ||
                                    Rechts2_nach_Gleis1.GetAktivStatus() ||
                                    Rechts2_nach_Gleis2.GetAktivStatus() ,
                                    Gleis1_nach_rechts1.GetAktivStatus() || //Nach rechts
                                    Gleis2_nach_rechts1.GetAktivStatus() ||
                                    Gleis1_nach_rechts2.GetAktivStatus() ||
                                    Gleis2_nach_rechts2.GetAktivStatus());
            
            UpdateGleisbild_Weiche2();  //Umfeld um Weiche 2
            UpdateGleisbild_Weiche3();  //Umfeld um Weiche 3
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
        }
    }
}
