using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Hauptform : Form
    {
        private List<string> SperrButtons = new List<string>();

        private void UpdateFahrstrassenSchalter()
        {
            UpdateSperrungen();
            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                var Fund = this.GleisplanAnzeige.Controls.Find(fahrstrasse.Name + "_Button", true);
                foreach (Control control in Fund)
                {
                    if (control is Button button)
                    {
                        if (FahrstrassenListe.FahrstrasseAlleGleicheBlockiert(fahrstrasse))
                        {
                            if (button.Enabled == true)
                            {
                                button.Enabled = false;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben_deakt;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten_deakt;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else {
                                    break;
                                }
                                if (fahrstrasse.EinfahrtsSignal.Zustand != SignalZustand.HP0) fahrstrasse.EinfahrtsSignal.Schalten(SignalZustand.HP0);
                            }
                        }
                        else
                        {
                            if (button.Enabled == false)
                            {
                                button.Enabled = true;
                                if (button.BackgroundImage.Tag.Equals("oben"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_oben;
                                    button.BackgroundImage.Tag = "oben";
                                }
                                else if (button.BackgroundImage.Tag.Equals("unten"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_unten;
                                    button.BackgroundImage.Tag = "unten";
                                }
                                else if (button.BackgroundImage.Tag.Equals("rechts"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
                                    button.BackgroundImage.Tag = "rechts";
                                }
                                else if (button.BackgroundImage.Tag.Equals("links"))
                                {
                                    button.BackgroundImage = Properties.Resources.Fahrstrasse_links;
                                    button.BackgroundImage.Tag = "links";
                                }
                                else { }                                
                            }
                        }
                    }
                }
            }        
        }

        private void UpdateSperrungen()
        {
            List<string> Aenderungen = new List<string>();
            foreach (string ButtonName in SperrButtons)
            {
                var Fund = this.GleisplanAnzeige.Controls.Find(ButtonName, true);
                foreach (Control control in Fund)
                {
                    if (control is CheckBox checkBox)
                    {
                        if(checkBox.Checked)
                        {
                            Aenderungen.AddRange(checkBox.Tag.ToString().Split('+'));
                        }
                    }
                }
            }

            foreach (Fahrstrasse fahrstrasse in FahrstrassenListe.Liste)
            {
                if(Aenderungen.Contains(fahrstrasse.Name))
                {
                    FahrstrassenListe.GesperrteFahrstarssen[fahrstrasse.Name] = true; // Fahrstrasse ist gesperrt
                    if (fahrstrasse.GetGesetztStatus())
                    {
                        //Fahrstraße deaktivieren wenn gesetzt
                        fahrstrasse.DeleteFahrstrasse(WeichenListe.Liste);
                    }
                }
                else
                {
                    FahrstrassenListe.GesperrteFahrstarssen[fahrstrasse.Name] = false; // Fahrstrasse ist nicht (oder nicht mehr) gesperrt
                }
            }

        }
    }
}
