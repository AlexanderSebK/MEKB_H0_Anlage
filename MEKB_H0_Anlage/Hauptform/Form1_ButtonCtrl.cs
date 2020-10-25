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
    }
}
