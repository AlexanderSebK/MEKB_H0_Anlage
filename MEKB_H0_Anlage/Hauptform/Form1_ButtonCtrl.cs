using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
        private void UpdateSchalter()
        {
            UpdateButton_Fahrstr_GL1_links();
            UpdateButton_Fahrstr_GL2_links();
            UpdateButton_Fahrstr_GL3_links();
            UpdateButton_Fahrstr_GL4_links();
            UpdateButton_Fahrstr_GL5_links();
            UpdateButton_Fahrstr_GL6_links();

            UpdateButton_Fahrstr_Block2_Einfahrt();
            UpdateButton_Fahrstr_GL1_rechts();
            UpdateButton_Fahrstr_GL2_rechts();
            UpdateButton_Fahrstr_GL3_rechts();
            UpdateButton_Fahrstr_GL4_rechts();
            UpdateButton_Fahrstr_GL5_rechts();
            UpdateButton_Fahrstr_GL6_rechts();

            UpdateButton_Fahrstr_Rechts1_nach_Hbf();
            UpdateButton_Fahrstr_Rechts2_nach_Hbf();
            

            UpdateButton_Fahrstr_Block5_Ausf();
            UpdateButton_Fahrstr_Block8_Ausf();
            UpdateButton_Fahrstr_Block9_Ausf();

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
        private void UpdateButton_Fahrstr_GL1_links()
        {
           if(FahrstrassenListe.FahrstrasseBlockiert("Gleis1_nach_Block1"))
            {
                Gleis1_nach_Block1_Button.Enabled = false;
                Gleis1_nach_Block1_Button.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L1");
                if (signal != null)
                {
                    if(signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Gleis1_nach_Block1_Button.Enabled = true;
                Gleis1_nach_Block1_Button.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_GL2_links()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis2_nach_Block1"))
            {
                Gleis2_nach_Block1_Button.Enabled = false;
                Gleis2_nach_Block1_Button.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L2");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }
            }
            else
            {
                Gleis2_nach_Block1_Button.Enabled = true;
                Gleis2_nach_Block1_Button.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }       
        private void UpdateButton_Fahrstr_GL3_links()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis3_nach_Block1"))
            {
                Fahrstr_GL3_links.Enabled = false;
                Fahrstr_GL3_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L3");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }
            }
            else
            {
                Fahrstr_GL3_links.Enabled = true;
                Fahrstr_GL3_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }     
        private void UpdateButton_Fahrstr_GL4_links()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis4_nach_Block1"))
            {
                Fahrstr_GL4_links.Enabled = false;
                Fahrstr_GL4_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L4");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }
            }
            else
            {
                Fahrstr_GL4_links.Enabled = true;
                Fahrstr_GL4_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_GL5_links()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis5_nach_Block1"))
            {
                Fahrstr_GL5_links.Enabled = false;
                Fahrstr_GL5_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L5");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }
            }
            else
            {
                Fahrstr_GL5_links.Enabled = true;
                Fahrstr_GL5_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_GL6_links()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis6_nach_Block1"))
            {
                Fahrstr_GL6_links.Enabled = false;
                Fahrstr_GL6_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_L6");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL6_links.Enabled = true;
                Fahrstr_GL6_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }       
        private void UpdateButton_Fahrstr_Block2_Einfahrt()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Block2_nach_Gleis3") ||
                FahrstrassenListe.FahrstrasseBlockiert("Block2_nach_Gleis4") ||
                FahrstrassenListe.FahrstrasseBlockiert("Block2_nach_Gleis5") ||
                FahrstrassenListe.FahrstrasseBlockiert("Block2_nach_Gleis6"))
            {
                Block2_Einfahrt.Enabled = false;
                Block2_Einfahrt.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Einfahrt_L");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }
            }
            else
            {
                Block2_Einfahrt.Enabled = true;
                Block2_Einfahrt.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }

        private void UpdateButton_Fahrstr_GL1_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis1_nach_TunnelAussen"))
            {
                Fahrstr_GL1_rechts.Enabled = false;
                Fahrstr_GL1_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R1"); 
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL1_rechts.Enabled = true;
                Fahrstr_GL1_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_GL2_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis2_nach_TunnelAussen"))
            {
                Fahrstr_GL2_rechts.Enabled = false;
                Fahrstr_GL2_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R2");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL2_rechts.Enabled = true;
                Fahrstr_GL2_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_GL3_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis3_nach_TunnelAussen"))
            {
                Fahrstr_GL3_rechts.Enabled = false;
                Fahrstr_GL3_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R3");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL3_rechts.Enabled = true;
                Fahrstr_GL3_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_GL4_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis4_nach_TunnelAussen"))
            {
                Fahrstr_GL4_rechts.Enabled = false;
                Fahrstr_GL4_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R4");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL4_rechts.Enabled = true;
                Fahrstr_GL4_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_GL5_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis5_nach_TunnelAussen"))
            {
                Fahrstr_GL5_rechts.Enabled = false;
                Fahrstr_GL5_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R5");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL5_rechts.Enabled = true;
                Fahrstr_GL5_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_GL6_rechts()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Gleis6_nach_TunnelAussen"))
            {
                Fahrstr_GL6_rechts.Enabled = false;
                Fahrstr_GL6_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                Signal signal = SignalListe.GetSignal("Signal_Ausfahrt_R6");
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL6_rechts.Enabled = true;
                Fahrstr_GL6_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts;
            }
        }
        private void UpdateButton_Fahrstr_Rechts1_nach_Hbf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("TunnelAussen_nach_Gleis1"))
            {
                Fahrstr_Rechts1.Enabled = false;
                Fahrstr_Rechts1.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                Signal signal = SignalListe.GetSignal("Signal_RTunnel_1"); 
                if (signal != null)
                {
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
        private void UpdateButton_Fahrstr_Block9_Ausf()
        {
            if (FahrstrassenListe.FahrstrasseBlockiert("Block9_nach_Block2"))
            {
                Block9.Enabled = false;
                Block9.BackgroundImage = Properties.Resources.Fahrstrasse_oben_deakt;
            }
            else
            {
                Block9.Enabled = true;
                Block9.BackgroundImage = Properties.Resources.Fahrstrasse_oben;
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
                    if (signal.Zustand != 0) signal.Schalten(0, z21Start);
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
