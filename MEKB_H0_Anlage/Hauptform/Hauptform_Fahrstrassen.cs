using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace MEKB_H0_Anlage
{
    public partial class Hauptform : Form
    {
        /// <summary>
        /// Fahrstraßen initialisieren
        /// </summary>
        private void SetupFahrstrassen()
        {
            //Fahrstrassen Importieren
            FahrstrassenListe = new FahrstrassenListe("Fahrstrassenliste.xml", WeichenListe, SignalListe);
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
                if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) AutoSignalUpdate(fahrstrasse.EinfahrtsSignal.Name);
            }
            else
            {
                //Fahrstraße aktivieren
                if (Betriebsbereit) fahrstrasse.StarteFahrstrasse();
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

        /// <summary>
        /// Button Click: Fahrstrasse an und ausschalten
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void FahrstrassenButton_Click(object sender, EventArgs e)
        {
            //Neue Fahrstrasse anlegen
            Fahrstrasse fahrstrasse;
            //Flag ob neue Buttons unter den Fahrstrassen-Button erstellt werden (False -> Werden erstellt)
            bool Abbau = false;

            //Prüfen ob Funktion von einem Button ausgelöst wurde
            if (sender is Button button)
            {
                //Button muss mit dem Namen der Fahrstraße als Tag ausgestattet sein
                if (button.Tag == null) return;
                string ZielFahrstrasse = button.Tag.ToString();

                //Tag endet mit '-' -> Es wurde ein generierter Button gedrückt
                if (ZielFahrstrasse.EndsWith("-"))
                {
                    Abbau = true; //Abbau der Generierten Buttons
                    ZielFahrstrasse = ZielFahrstrasse.Substring(0, ZielFahrstrasse.Length - 1); //'-' entfernen
                }

                //Fahrstrasse aus der Liste finden mit Namen
                fahrstrasse = FahrstrassenListe.GetFahrstrasse(ZielFahrstrasse);

                //Keine Fahrstraße gefunden -> Funktion abbrechen
                if (fahrstrasse == null) return;

                //Wenn Fahrstrassen gleichen Ausgangspunkt haben
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
                        // Untergruppe mit Buttons soll gelöscht werden
                        if (Abbau)
                        {
                            //Fahrstrasse ist nicht blockiert mit anderen Fahrstrassen
                            if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                            {
                                //Fahrstrasse einschalten
                                ToggleFahrstrasse(fahrstrasse);
                                //Unterbuttons löschen
                                LoescheButtons(fahrstrasse.Fahrstr_GleicherEingang);

                                //Button freigeben 
                                Button clickedButton = button;
                                clickedButton.Dispose();
                            }
                        }
                        else //Buttons sollen generiert werden (Erster Klick auf Buttons mit mehreren Ausgängen
                        {
                            GeneriereButtons(fahrstrasse.Fahrstr_GleicherEingang, button.Location.X, button.Location.Y);
                        }
                    }
                }
                //Einzelne Fahrstrasse
                else
                {
                    //Fahrstrasse aktiv?
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        ToggleFahrstrasse(fahrstrasse);  //Aktiv? auschalten
                    }
                    else
                    {
                        //Keine Sperrende Fahstraße aktiv
                        if (!FahrstrassenListe.FahrstrasseBlockiert(fahrstrasse.Name))
                        {
                            ToggleFahrstrasse(fahrstrasse);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Untergruppe an Buttons generieren
        /// </summary>
        /// <param name="Fahrstrassen">Liste an Fahrstrassennamen</param>
        /// <param name="X">Position des Ursprungsbuttons (X-Koordinate)</param>
        /// <param name="Y">Position des Ursprungsbuttons (Y-Koordinate)</param>
        private void GeneriereButtons(List<string> Fahrstrassen, int X, int Y)
        {
            //Offset von 20 Punkten nach links
            X += 20;

            //Wenn Buttons schon existieren -> löschen
            Control Modul = this.Controls[Fahrstrassen[0] + "_Auswahl"];
            if (Modul is Button button)
            {
                LoescheButtons(Fahrstrassen);
                return;
            }
            //Für jede Fahrstrasse einen Button anlegen
            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                //Neuen Button erstellen
                Button newButton = new Button
                {
                    Name = Fahrstrassenname + "_Auswahl",
                    Tag = Fahrstrassenname + "-",
                    Size = new Size(100, 20),
                    Location = new Point(X, Y),
                    Enabled = !FahrstrassenListe.FahrstrasseBlockiert(Fahrstrassenname)
                };
                //Offset um 20 Punkte nach unten
                Y += 20;
                //Funktion an neuen Button zurodnen
                newButton.Click += new System.EventHandler(this.FahrstrassenButton_Click);
                newButton.BringToFront();

                //Fahrstrassenname beinhaltet einen Unterstrich
                if (Fahrstrassenname.Contains('_'))
                {
                    //Nur letzten Teil übernehmen
                    string[] text = Fahrstrassenname.Split('_');
                    newButton.Text = text[2];
                }
                //Fahrstrassenname als Text übernehmen
                else
                {
                    newButton.Text = Fahrstrassenname;
                }
                //Button hinzufügen
                this.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }
        
        /// <summary>
        /// Buttons löschen
        /// </summary>
        /// <param name="Fahrstrassen">Liste mit dem Namen der Fahrstrassen</param>
        private void LoescheButtons(List<string> Fahrstrassen)
        {
            foreach (string Fahrstrassenname in Fahrstrassen)
            {
                //Name mit Auswahl erweitern
                Control Modul = this.Controls[Fahrstrassenname + "_Auswahl"];
                if (Modul is Button)
                {
                    this.Controls.Remove(Modul);
                }
            }

        }
    }
}
