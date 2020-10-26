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
        private void UpdateSchalterSignale()
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
            UpdateButton_Fahrstr_RechtsSubButtons_nach_Hbf();

        }

        private void UpdateButton_Fahrstr_GL1_links()
        {
            if (Gleis2_nach_links.GetGesetztStatus() ||
                Gleis3_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus())
            {
                Fahrstr_GL1_links.Enabled = false;
                Fahrstr_GL1_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L1" });
                if (ListID != -1)
                {
                    if(Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL1_links.Enabled = true;
                Fahrstr_GL1_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_GL2_links()
        {
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis3_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus())
            {
                Fahrstr_GL2_links.Enabled = false;
                Fahrstr_GL2_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L2" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_GL2_links.Enabled = true;
                Fahrstr_GL2_links.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_GL3_links()
        {
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis2_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus() ||
                Block2_nach_Gleis3.GetGesetztStatus() ||
                Block2_nach_Gleis4.GetGesetztStatus() ||
                Block2_nach_Gleis5.GetGesetztStatus() ||
                Block2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL3_links.Enabled = false;
                Fahrstr_GL3_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L3" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis2_nach_links.GetGesetztStatus() ||
                Gleis3_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus() ||
                Block2_nach_Gleis3.GetGesetztStatus() ||
                Block2_nach_Gleis4.GetGesetztStatus() ||
                Block2_nach_Gleis5.GetGesetztStatus() ||
                Block2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL4_links.Enabled = false;
                Fahrstr_GL4_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L4" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis2_nach_links.GetGesetztStatus() ||
                Gleis3_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus() ||
                Block2_nach_Gleis3.GetGesetztStatus() ||
                Block2_nach_Gleis4.GetGesetztStatus() ||
                Block2_nach_Gleis5.GetGesetztStatus() ||
                Block2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL5_links.Enabled = false;
                Fahrstr_GL5_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L5" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis2_nach_links.GetGesetztStatus() ||
                Gleis3_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Block2_nach_Gleis1.GetGesetztStatus() ||
                Block2_nach_Gleis2.GetGesetztStatus() ||
                Block2_nach_Gleis3.GetGesetztStatus() ||
                Block2_nach_Gleis4.GetGesetztStatus() ||
                Block2_nach_Gleis5.GetGesetztStatus() ||
                Block2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL6_links.Enabled = false;
                Fahrstr_GL6_links.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_L6" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_links.GetGesetztStatus() ||
                Gleis2_nach_links.GetGesetztStatus() )               
            {
                Block2_Einfaht_GL1.Enabled = false;
                Block2_Einfaht_GL2.Enabled = false;                
            }
            else
            {
                Block2_Einfaht_GL1.Enabled = true;
                Block2_Einfaht_GL2.Enabled = true;
            }
            if (Gleis3_nach_links.GetGesetztStatus() ||
                Gleis4_nach_links.GetGesetztStatus() ||
                Gleis5_nach_links.GetGesetztStatus() ||
                Gleis6_nach_links.GetGesetztStatus() )
            {
                Block2_Einfahrt.Enabled = false;
                Block2_Einfahrt.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Einfahrt_L" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis1.GetGesetztStatus() ||
                Rechts1_nach_Gleis2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis1.GetGesetztStatus() ||
                Rechts2_nach_Gleis2.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus() )
            {
                Fahrstr_GL1_rechts.Enabled = false;
                Fahrstr_GL1_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R1" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis1.GetGesetztStatus() ||
                Rechts1_nach_Gleis2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis1.GetGesetztStatus() ||
                Rechts2_nach_Gleis2.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL2_rechts.Enabled = false;
                Fahrstr_GL2_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R2" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL3_rechts.Enabled = false;
                Fahrstr_GL3_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R3" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL4_rechts.Enabled = false;
                Fahrstr_GL4_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R4" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL5_rechts.Enabled = false;
                Fahrstr_GL5_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R5" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_GL6_rechts.Enabled = false;
                Fahrstr_GL6_rechts.BackgroundImage = Properties.Resources.Fahrstrasse_rechts_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_Ausfahrt_R6" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Rechts2_nach_Gleis1.GetGesetztStatus() ||
                Rechts2_nach_Gleis2.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_Rechts1.Enabled = false;
                Fahrstr_Rechts1.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_RTunnel_1" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
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
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Rechts1_nach_Gleis1.GetGesetztStatus() ||
                Rechts1_nach_Gleis2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus())
            {
                Fahrstr_Rechts2.Enabled = false;
                Fahrstr_Rechts2.BackgroundImage = Properties.Resources.Fahrstrasse_links_deakt;
                int ListID = Signalliste.IndexOf(new Signal() { Name = "Signal_RTunnel_2" });
                if (ListID != -1)
                {
                    if (Signalliste[ListID].Zustand != 0) Signalliste[ListID].Schalten(0, z21Start);
                }

            }
            else
            {
                Fahrstr_Rechts2.Enabled = true;
                Fahrstr_Rechts2.BackgroundImage = Properties.Resources.Fahrstrasse_links;
            }
        }
        private void UpdateButton_Fahrstr_RechtsSubButtons_nach_Hbf()
        {
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() )               
            {
                Rechts1_Einfahrt_Gl1.Enabled = false;
                Rechts1_Einfahrt_Gl2.Enabled = false;
                Rechts2_Einfahrt_Gl1.Enabled = false;
                Rechts2_Einfahrt_Gl2.Enabled = false;
            }
            else
            {
                Rechts1_Einfahrt_Gl1.Enabled = true;
                Rechts1_Einfahrt_Gl2.Enabled = true;
                Rechts2_Einfahrt_Gl1.Enabled = true;
                Rechts2_Einfahrt_Gl2.Enabled = true;
            }
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus() ||
                Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus() ||
                Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus() ||
                Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus() ||
                Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus() ||
                Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus())
            {
                
                Rechts1_Einfahrt_Gl3.Enabled = false;
                Rechts1_Einfahrt_Gl4.Enabled = false;
                Rechts1_Einfahrt_Gl5.Enabled = false;
                Rechts1_Einfahrt_Gl6.Enabled = false;
               
                Rechts2_Einfahrt_Gl3.Enabled = false;
                Rechts2_Einfahrt_Gl4.Enabled = false;
                Rechts2_Einfahrt_Gl5.Enabled = false;
                Rechts2_Einfahrt_Gl6.Enabled = false;
            }
            else
            {
                
                Rechts1_Einfahrt_Gl3.Enabled = true;
                Rechts1_Einfahrt_Gl4.Enabled = true;
                Rechts1_Einfahrt_Gl5.Enabled = true;
                Rechts1_Einfahrt_Gl6.Enabled = true;
               
                Rechts2_Einfahrt_Gl3.Enabled = true;
                Rechts2_Einfahrt_Gl4.Enabled = true;
                Rechts2_Einfahrt_Gl5.Enabled = true;
                Rechts2_Einfahrt_Gl6.Enabled = true;
            }
        }
    }
}
