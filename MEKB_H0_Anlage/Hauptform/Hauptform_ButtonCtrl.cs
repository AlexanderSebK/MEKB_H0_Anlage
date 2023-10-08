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
        private void UpdateSchalter()
        {
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
        


            UpdateButton_Fahrstr_Rechts1_nach_Hbf();
            UpdateButton_Fahrstr_Rechts2_nach_Hbf();
            

            UpdateButton_Fahrstr_Block5_Ausf();
            UpdateButton_Fahrstr_Block8_Ausf();

            UpdateButton_Fahrstr_Schatten11_Ausf();
            UpdateButton_Fahrstr_Schatten10_Ausf();
            UpdateButton_Fahrstr_Schatten9_Ausf();
            UpdateButton_Fahrstr_Schatten8_Ausf();

            UpdateButton_Fahrstr_Schatten0_Ausf();
            UpdateButton_Fahrstr_Schatten1_Ausf();
            UpdateButton_Fahrstr_Schatten2_Ausf();
            UpdateButton_Fahrstr_Schatten3_Ausf();
            UpdateButton_Fahrstr_Schatten4_Ausf();
            UpdateButton_Fahrstr_Schatten5_Ausf();
            UpdateButton_Fahrstr_Schatten6_Ausf();
            UpdateButton_Fahrstr_Schatten7_Ausf();
        }

        #region Fahrstrassen Hbf
       
 
        
        private void UpdateButton_Fahrstr_Rechts1_nach_Hbf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("TunnelAussen_nach_Gleis1"))
            {
                Fahrstr_Rechts1.Enabled = false;
                Fahrstr_Rechts1.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_RTunnel_1"); 
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Rechts1.Enabled = true;
                Fahrstr_Rechts1.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Rechts2_nach_Hbf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("TunnelInnen_nach_Gleis1"))
            {
                Fahrstr_Rechts2.Enabled = false;
                Fahrstr_Rechts2.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_RTunnel_2");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Rechts2.Enabled = true;
                Fahrstr_Rechts2.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        #endregion

        #region Freie Strecke
        private void UpdateButton_Fahrstr_Block5_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Block5_nach_Block6"))
            {
                Block5_Ausfahrt.Enabled = false;
                Block5_Ausfahrt.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }*/

            }
            else
            {
                Block5_Ausfahrt.Enabled = true;
                Block5_Ausfahrt.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_Block8_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Block8_nach_Block6"))
            {
                Block8.Enabled = false;
                Block8.BackgroundImage = Properties.Resources.Fahrstrasse_unten_deakt;
            }
            else
            {
                Block8.Enabled = true;
                Block8.BackgroundImage = Properties.Resources.Fahrstrasse_unten;
            }
        }
        
        #endregion

        #region Schattenbahnhof intern
        private void UpdateButton_Fahrstr_Schatten11_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten11_nach_Eingleisen"))
            {
                Fahrstr_Schatten11_Ausf.Enabled = false;
                Fahrstr_Schatten11_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Schatten11");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Schatten11_Ausf.Enabled = true;
                Fahrstr_Schatten11_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_Schatten10_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten10_nach_Eingleisen"))
            {
                Fahrstr_Schatten10_Ausf.Enabled = false;
                Fahrstr_Schatten10_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Schatten10");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Schatten10_Ausf.Enabled = true;
                Fahrstr_Schatten10_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_Schatten9_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten9_nach_Eingleisen"))
            {
                Fahrstr_Schatten9_Ausf.Enabled = false;
                Fahrstr_Schatten9_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Schatten9");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Schatten9_Ausf.Enabled = true;
                Fahrstr_Schatten9_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_Schatten8_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten8_nach_Eingleisen"))
            {
                Fahrstr_Schatten8_Ausf.Enabled = false;
                Fahrstr_Schatten8_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Schatten8");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(SignalZustand.HP0);
                }

            }
            else
            {
                Fahrstr_Schatten8_Ausf.Enabled = true;
                Fahrstr_Schatten8_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        #endregion

        #region Schattenbahnhof Ausfahrt
        private void UpdateButton_Fahrstr_Schatten0_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Eingleisen_nach_Block8"))
            {
                Fahrstr_Schatten0_Ausf.Enabled = false;
                Fahrstr_Schatten0_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten0_Ausf.Enabled = true;
                Fahrstr_Schatten0_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }     
        private void UpdateButton_Fahrstr_Schatten1_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten1_nach_Block8"))
            {
                Fahrstr_Schatten1_Ausf.Enabled = false;
                Fahrstr_Schatten1_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten1_Ausf.Enabled = true;
                Fahrstr_Schatten1_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten2_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten2_nach_Block9"))
            {
                Fahrstr_Schatten2_Ausf.Enabled = false;
                Fahrstr_Schatten2_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten2_Ausf.Enabled = true;
                Fahrstr_Schatten2_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten3_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten3_nach_Block9"))
            {
                Fahrstr_Schatten3_Ausf.Enabled = false;
                Fahrstr_Schatten3_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten3_Ausf.Enabled = true;
                Fahrstr_Schatten3_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten4_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten4_nach_Block9"))
            {
                Fahrstr_Schatten4_Ausf.Enabled = false;
                Fahrstr_Schatten4_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten4_Ausf.Enabled = true;
                Fahrstr_Schatten4_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten5_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten5_nach_Block9"))
            {
                Fahrstr_Schatten5_Ausf.Enabled = false;
                Fahrstr_Schatten5_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten5_Ausf.Enabled = true;
                Fahrstr_Schatten5_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten6_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten6_nach_Block9"))
            {
                Fahrstr_Schatten6_Ausf.Enabled = false;
                Fahrstr_Schatten6_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten6_Ausf.Enabled = true;
                Fahrstr_Schatten6_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_Schatten7_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Schatten7_nach_Block9"))
            {
                Fahrstr_Schatten7_Ausf.Enabled = false;
                Fahrstr_Schatten7_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                /*int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }*/

            }
            else
            {
                Fahrstr_Schatten7_Ausf.Enabled = true;
                Fahrstr_Schatten7_Ausf.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }

        #endregion

        
    }
}
