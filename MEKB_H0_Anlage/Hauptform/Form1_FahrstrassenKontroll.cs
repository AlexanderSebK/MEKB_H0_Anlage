using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
        // Unterklasse: Aktivieren der Fahrstraßen und überprüfen ob Fahrstraße verfügbar ist
        #region Hbf_Links
        #region Einfahrt
        private void Block2_Einfahrt_Click(object sender, EventArgs e)
        {   //Einer der Block2-Fahrstrassen aktiv
            if (Block2_nach_Gleis1.GetGesetztStatus() ||
               Block2_nach_Gleis2.GetGesetztStatus() ||
               Block2_nach_Gleis3.GetGesetztStatus() ||
               Block2_nach_Gleis4.GetGesetztStatus() ||
               Block2_nach_Gleis5.GetGesetztStatus() ||
               Block2_nach_Gleis6.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Block2_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis1);
                if (Block2_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis2);
                if (Block2_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis3);
                if (Block2_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis4);
                if (Block2_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis5);
                if (Block2_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Block2_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Block2_Auswahl.Visible) Block2_Auswahl.Visible = false;
                else Block2_Auswahl.Visible = true;
            }
        }
        private void Block2_Einfaht_GL1_Click(object sender, EventArgs e)
        {
            if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                !Gleis2_nach_Block1.GetGesetztStatus() &&
                !Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis1);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL2_Click(object sender, EventArgs e)
        {
            if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                !Gleis2_nach_Block1.GetGesetztStatus() &&
                !Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis2);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL3_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis3);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL4_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis4);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL5_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis5);
                Block2_Auswahl.Visible = false;
            }
        }
        private void Block2_Einfaht_GL6_Click(object sender, EventArgs e)
        {
            if (!Gleis3_nach_Block1.GetGesetztStatus() &&
                !Gleis4_nach_Block1.GetGesetztStatus() &&
                !Gleis5_nach_Block1.GetGesetztStatus() &&
                !Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block2_nach_Gleis6);
                Block2_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Ausfahrt
        private void Fahrstr_GL1_links_Click(object sender, EventArgs e)
        {
            if (Gleis1_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis2_nach_Block1.GetGesetztStatus() &&
                   !Gleis3_nach_Block1.GetGesetztStatus() &&
                   !Gleis4_nach_Block1.GetGesetztStatus() &&
                   !Gleis5_nach_Block1.GetGesetztStatus() &&
                   !Gleis6_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis1_nach_Block1);
                }
            }
        }
        private void Fahrstr_GL2_links_Click(object sender, EventArgs e)
        {
            if (Gleis2_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                   !Gleis3_nach_Block1.GetGesetztStatus() &&
                   !Gleis4_nach_Block1.GetGesetztStatus() &&
                   !Gleis5_nach_Block1.GetGesetztStatus() &&
                   !Gleis6_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis2_nach_Block1);
                }
            }
        }
        private void Fahrstr_GL3_links_Click(object sender, EventArgs e)
        {
            if (Gleis3_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                   !Gleis2_nach_Block1.GetGesetztStatus() &&
                   !Gleis4_nach_Block1.GetGesetztStatus() &&
                   !Gleis5_nach_Block1.GetGesetztStatus() &&
                   !Gleis6_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis3_nach_Block1);
                }
            }
        }

        #region Gleis 4
        private void Fahrstr_GL4_links_Click(object sender, EventArgs e)
        {
            if (Gleis4_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                   !Gleis2_nach_Block1.GetGesetztStatus() &&
                   !Gleis3_nach_Block1.GetGesetztStatus() &&
                   !Gleis5_nach_Block1.GetGesetztStatus() &&
                   !Gleis6_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis4_nach_Block1);
                }
            }
        }
        
        #endregion
        #region Gleis 5
        private void Fahrstr_GL5_links_Click(object sender, EventArgs e)
        {
            if (Gleis5_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                   !Gleis2_nach_Block1.GetGesetztStatus() &&
                   !Gleis3_nach_Block1.GetGesetztStatus() &&
                   !Gleis4_nach_Block1.GetGesetztStatus() &&
                   !Gleis6_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis5_nach_Block1);
                }
            }
        }
        #endregion
        #region Gleis 6
        private void Fahrstr_GL6_links_Click(object sender, EventArgs e)
        {
            if (Gleis6_nach_Block1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_Block1);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Gleis1_nach_Block1.GetGesetztStatus() &&
                   !Gleis2_nach_Block1.GetGesetztStatus() &&
                   !Gleis3_nach_Block1.GetGesetztStatus() &&
                   !Gleis4_nach_Block1.GetGesetztStatus() &&
                   !Gleis5_nach_Block1.GetGesetztStatus() &&
                   !Block2_nach_Gleis1.GetGesetztStatus() &&
                   !Block2_nach_Gleis2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Gleis6_nach_Block1);
                }
            }
            /*
            //Einer der Gl6-links Fahrstrassen aktiv
            if (Gleis6_nach_Block1.GetGesetztStatus() ||
                Gleis6_nach_Block5.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis6_nach_Block1.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_Block1);
                if (Gleis6_nach_Block5.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_Block5);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl6_links_Auswahl.Visible) Gl6_links_Auswahl.Visible = false;
                else Gl6_links_Auswahl.Visible = true;
            }
            */
        }
        private void Block1_nach_Block2_Click(object sender, EventArgs e)
        {
            if (Block1_nach_Block2.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block1_nach_Block2);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Block9_nach_Block2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Block1_nach_Block2);
                }
            }
            Block1_Auswahl.Visible = false;
        }
        private void Block1_nach_Block5_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Block1_nach_Block5);  //Aktiv? auschalten    
            Block1_Auswahl.Visible = false;
        }
        #endregion
        #endregion
        #endregion
        #region Hbf_Rechts
        #region Einfahrt
        #region von Wendeschleife 1
        private void Fahrstr_Rechts1_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Rechts1_nach_Gleis1.GetGesetztStatus() ||
                Rechts1_nach_Gleis2.GetGesetztStatus() ||
                Rechts1_nach_Gleis3.GetGesetztStatus() ||
                Rechts1_nach_Gleis4.GetGesetztStatus() ||
                Rechts1_nach_Gleis5.GetGesetztStatus() ||
                Rechts1_nach_Gleis6.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Rechts1_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis1);
                if (Rechts1_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis2);
                if (Rechts1_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis3);
                if (Rechts1_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis4);
                if (Rechts1_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis5);
                if (Rechts1_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Rechts1_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Rechts1_Auswahl.Visible) Rechts1_Auswahl.Visible = false;
                else Rechts1_Auswahl.Visible = true;
            }
        }
        private void Rechts1_Einfahrt_Gl1_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis1);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl2_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis2);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl3_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis3);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl4_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis4);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl5_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis5);
                Rechts1_Auswahl.Visible = false;
            }
        }
        private void Rechts1_Einfahrt_Gl6_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts1_nach_Gleis6);
                Rechts1_Auswahl.Visible = false;
            }
        }
        #endregion
        #region von Wendeschleife 2
        private void Fahrstr_Rechts2_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Rechts2_nach_Gleis1.GetGesetztStatus() ||
                Rechts2_nach_Gleis2.GetGesetztStatus() ||
                Rechts2_nach_Gleis3.GetGesetztStatus() ||
                Rechts2_nach_Gleis4.GetGesetztStatus() ||
                Rechts2_nach_Gleis5.GetGesetztStatus() ||
                Rechts2_nach_Gleis6.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Rechts2_nach_Gleis1.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis1);
                if (Rechts2_nach_Gleis2.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis2);
                if (Rechts2_nach_Gleis3.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis3);
                if (Rechts2_nach_Gleis4.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis4);
                if (Rechts2_nach_Gleis5.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis5);
                if (Rechts2_nach_Gleis6.GetGesetztStatus()) ToggleFahrstrasse(Rechts2_nach_Gleis6);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Rechts2_Auswahl.Visible) Rechts2_Auswahl.Visible = false;
                else Rechts2_Auswahl.Visible = true;
            }
        }
        private void Rechts2_Einfahrt_Gl1_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis1);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl2_Click(object sender, EventArgs e)
        {
            if (!Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis2);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl3_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis3);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl4_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis4);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl5_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis5);
                Rechts2_Auswahl.Visible = false;
            }
        }
        private void Rechts2_Einfahrt_Gl6_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Rechts2_nach_Gleis6);
                Rechts2_Auswahl.Visible = false;
            }
        }
        #endregion
        #endregion
        #region Ausfahrt
        #region Gleis 1
        private void Fahrstr_GL1_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl1-rechts Fahrstrassen aktiv
            if (Gleis1_nach_rechts1.GetGesetztStatus() ||
                Gleis1_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis1_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis1_nach_rechts1);
                if (Gleis1_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis1_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl1_rechts_Auswahl.Visible) Gl1_rechts_Auswahl.Visible = false;
                else Gl1_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl1_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_rechts1);
                Gl1_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl1_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis1_nach_rechts2);
                Gl1_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Gleis 2
        private void Fahrstr_GL2_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl2-rechts Fahrstrassen aktiv
            if (Gleis2_nach_rechts1.GetGesetztStatus() ||
                Gleis2_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis2_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis2_nach_rechts1);
                if (Gleis2_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis2_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl2_rechts_Auswahl.Visible) Gl2_rechts_Auswahl.Visible = false;
                else Gl2_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl2_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_rechts1);
                Gl2_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl2_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus() &&
                !Rechts1_nach_Gleis2.GetGesetztStatus() &&
                !Rechts2_nach_Gleis2.GetGesetztStatus() &&
                !Rechts1_nach_Gleis1.GetGesetztStatus() &&
                !Rechts2_nach_Gleis1.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis2_nach_rechts2);
                Gl2_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Gleis 3
        private void Fahrstr_GL3_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl3-rechts Fahrstrassen aktiv
            if (Gleis3_nach_rechts1.GetGesetztStatus() ||
                Gleis3_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis3_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis3_nach_rechts1);
                if (Gleis3_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis3_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl3_rechts_Auswahl.Visible) Gl3_rechts_Auswahl.Visible = false;
                else Gl3_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl3_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_rechts1);
                Gl3_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl3_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis3_nach_rechts2);
                Gl3_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Gleis 4
        private void Fahrstr_GL4_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl4-rechts Fahrstrassen aktiv
            if (Gleis4_nach_rechts1.GetGesetztStatus() ||
                Gleis4_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis4_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis4_nach_rechts1);
                if (Gleis4_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis4_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl4_rechts_Auswahl.Visible) Gl4_rechts_Auswahl.Visible = false;
                else Gl4_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl4_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_rechts1);
                Gl4_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl4_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis4_nach_rechts2);
                Gl4_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Gleis 5
        private void Fahrstr_GL5_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl5-rechts Fahrstrassen aktiv
            if (Gleis5_nach_rechts1.GetGesetztStatus() ||
                Gleis5_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis5_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis5_nach_rechts1);
                if (Gleis5_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis5_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl5_rechts_Auswahl.Visible) Gl5_rechts_Auswahl.Visible = false;
                else Gl5_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl5_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_rechts1);
                Gl5_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl5_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis6_nach_rechts1.GetGesetztStatus() &&
                !Gleis6_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis5_nach_rechts2);
                Gl5_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #region Gleis 6
        private void Fahrstr_GL6_rechts_Click(object sender, EventArgs e)
        {
            //Einer der Gl6-rechts-Fahrstrassen aktiv
            if (Gleis6_nach_rechts1.GetGesetztStatus() ||
                Gleis6_nach_rechts2.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Gleis6_nach_rechts1.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_rechts1);
                if (Gleis6_nach_rechts2.GetGesetztStatus()) ToggleFahrstrasse(Gleis6_nach_rechts2);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Gl6_rechts_Auswahl.Visible) Gl6_rechts_Auswahl.Visible = false;
                else Gl6_rechts_Auswahl.Visible = true;
            }
        }
        private void Gl6_Ausfahrt_rechts1_Click(object sender, EventArgs e)
        {
            if (!Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_rechts1);
                Gl6_rechts_Auswahl.Visible = false;
            }
        }
        private void Gl6_Ausfahrt_rechts2_Click(object sender, EventArgs e)
        {
            if (!Gleis5_nach_rechts1.GetGesetztStatus() &&
                !Gleis5_nach_rechts2.GetGesetztStatus() &&
                !Gleis4_nach_rechts1.GetGesetztStatus() &&
                !Gleis4_nach_rechts2.GetGesetztStatus() &&
                !Gleis3_nach_rechts1.GetGesetztStatus() &&
                !Gleis3_nach_rechts2.GetGesetztStatus() &&
                !Gleis2_nach_rechts1.GetGesetztStatus() &&
                !Gleis2_nach_rechts2.GetGesetztStatus() &&
                !Gleis1_nach_rechts1.GetGesetztStatus() &&
                !Gleis1_nach_rechts2.GetGesetztStatus() &&

                !Rechts1_nach_Gleis6.GetGesetztStatus() &&
                !Rechts2_nach_Gleis6.GetGesetztStatus() &&
                !Rechts1_nach_Gleis5.GetGesetztStatus() &&
                !Rechts2_nach_Gleis5.GetGesetztStatus() &&
                !Rechts1_nach_Gleis4.GetGesetztStatus() &&
                !Rechts2_nach_Gleis4.GetGesetztStatus() &&
                !Rechts1_nach_Gleis3.GetGesetztStatus() &&
                !Rechts2_nach_Gleis3.GetGesetztStatus())
            {
                ToggleFahrstrasse(Gleis6_nach_rechts2);
                Gl6_rechts_Auswahl.Visible = false;
            }
        }
        #endregion
        #endregion
        #endregion

        #region Schattenbahnof Einfahrt
        private void Block6_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Block6_nach_Schatten8.GetGesetztStatus() ||
               Block6_nach_Schatten9.GetGesetztStatus() ||
               Block6_nach_Schatten10.GetGesetztStatus() ||
               Block6_nach_Schatten11.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Block6_nach_Schatten8.GetGesetztStatus()) ToggleFahrstrasse(Block6_nach_Schatten8);
                if (Block6_nach_Schatten9.GetGesetztStatus()) ToggleFahrstrasse(Block6_nach_Schatten9);
                if (Block6_nach_Schatten10.GetGesetztStatus()) ToggleFahrstrasse(Block6_nach_Schatten10);
                if (Block6_nach_Schatten11.GetGesetztStatus()) ToggleFahrstrasse(Block6_nach_Schatten11);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Block6_Auswahl.Visible) Block6_Auswahl.Visible = false;
                else Block6_Auswahl.Visible = true;
            }
        }
        private void Block6_Schatten11_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Block6_nach_Schatten11);
            Block6_Auswahl.Visible = false;
        }
        private void Block6_Schatten10_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Block6_nach_Schatten10);
            Block6_Auswahl.Visible = false;
        }
        private void Block6_Schatten9_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Block6_nach_Schatten9);
            Block6_Auswahl.Visible = false;
        }
        private void Block6_Schatten8_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Block6_nach_Schatten8);
            Block6_Auswahl.Visible = false;
        }
        #endregion
        #region Schattenbahnhof Intern
        private string ausfahrtAuswahl = "";
        private void Fahrstr_Schatten8_Ausf_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Schatten8_nach_Block7.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Schatten8_nach_Block7.GetGesetztStatus()) ToggleFahrstrasse(Schatten8_nach_Block7);
                if (Block7_nach_Schatten7.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten7);
                if (Block7_nach_Schatten6.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten6);
                if (Block7_nach_Schatten5.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten5);
                if (Block7_nach_Schatten4.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten4);
                if (Block7_nach_Schatten3.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten3);
                if (Block7_nach_Schatten2.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten2);
                if (Block7_nach_Schatten1.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten1);
                if (Block7_nach_Schatten0.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten0);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (SchattenIntern_Auswahl.Visible) SchattenIntern_Auswahl.Visible = false;
                else
                {
                    SchattenIntern_Auswahl.Visible = true;
                    ausfahrtAuswahl = "Schatten8";
                }
            }
        }
        private void Fahrstr_Schatten9_Ausf_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Schatten9_nach_Block7.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Schatten9_nach_Block7.GetGesetztStatus()) ToggleFahrstrasse(Schatten9_nach_Block7);
                if (Block7_nach_Schatten7.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten7);
                if (Block7_nach_Schatten6.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten6);
                if (Block7_nach_Schatten5.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten5);
                if (Block7_nach_Schatten4.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten4);
                if (Block7_nach_Schatten3.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten3);
                if (Block7_nach_Schatten2.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten2);
                if (Block7_nach_Schatten1.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten1);
                if (Block7_nach_Schatten0.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten0);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (SchattenIntern_Auswahl.Visible) SchattenIntern_Auswahl.Visible = false;
                else
                {
                    SchattenIntern_Auswahl.Visible = true;
                    ausfahrtAuswahl = "Schatten9";
                }
            }
        }
        private void Fahrstr_Schatten10_Ausf_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Schatten10_nach_Block7.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Schatten10_nach_Block7.GetGesetztStatus()) ToggleFahrstrasse(Schatten10_nach_Block7);
                if (Block7_nach_Schatten7.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten7);
                if (Block7_nach_Schatten6.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten6);
                if (Block7_nach_Schatten5.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten5);
                if (Block7_nach_Schatten4.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten4);
                if (Block7_nach_Schatten3.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten3);
                if (Block7_nach_Schatten2.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten2);
                if (Block7_nach_Schatten1.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten1);
                if (Block7_nach_Schatten0.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten0);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (SchattenIntern_Auswahl.Visible) SchattenIntern_Auswahl.Visible = false;
                else
                {
                    SchattenIntern_Auswahl.Visible = true;
                    ausfahrtAuswahl = "Schatten10";
                }
            }
        }
        private void Fahrstr_Schatten11_Ausf_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Schatten11_nach_Block7.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Schatten11_nach_Block7.GetGesetztStatus()) ToggleFahrstrasse(Schatten11_nach_Block7);
                if (Block7_nach_Schatten7.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten7);
                if (Block7_nach_Schatten6.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten6);
                if (Block7_nach_Schatten5.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten5);
                if (Block7_nach_Schatten4.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten4);
                if (Block7_nach_Schatten3.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten3);
                if (Block7_nach_Schatten2.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten2);
                if (Block7_nach_Schatten1.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten1);
                if (Block7_nach_Schatten0.GetGesetztStatus()) ToggleFahrstrasse(Block7_nach_Schatten0);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (SchattenIntern_Auswahl.Visible) SchattenIntern_Auswahl.Visible = false;
                else
                {
                    SchattenIntern_Auswahl.Visible = true;
                    ausfahrtAuswahl = "Schatten11";
                }
            }
        }

        private void Intern_Schatten0_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten0);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten1_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten1);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten2_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten2);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten3_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten3);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten4_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten4);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten5_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten5);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten6_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten6);
            SchattenIntern_Auswahl.Visible = false;
        }
        private void Intern_Schatten7_Click(object sender, EventArgs e)
        {
            if (ausfahrtAuswahl.Equals("Schatten11")) ToggleFahrstrasse(Schatten11_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten10")) ToggleFahrstrasse(Schatten10_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten9")) ToggleFahrstrasse(Schatten9_nach_Block7);
            if (ausfahrtAuswahl.Equals("Schatten8")) ToggleFahrstrasse(Schatten8_nach_Block7);
            ToggleFahrstrasse(Block7_nach_Schatten7);
            SchattenIntern_Auswahl.Visible = false;
        }
        #endregion
        #region Schattenbahnhof Ausfahrt
        private void Fahrstr_Schatten7_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten7_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten7_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten2_nach_Block9.GetGesetztStatus() &&
                   !Schatten3_nach_Block9.GetGesetztStatus() &&
                   !Schatten4_nach_Block9.GetGesetztStatus() &&
                   !Schatten5_nach_Block9.GetGesetztStatus() &&
                   !Schatten6_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten7_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten6_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten6_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten6_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten2_nach_Block9.GetGesetztStatus() &&
                   !Schatten3_nach_Block9.GetGesetztStatus() &&
                   !Schatten4_nach_Block9.GetGesetztStatus() &&
                   !Schatten5_nach_Block9.GetGesetztStatus() &&
                   !Schatten7_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten6_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten5_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten5_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten5_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten2_nach_Block9.GetGesetztStatus() &&
                   !Schatten3_nach_Block9.GetGesetztStatus() &&
                   !Schatten4_nach_Block9.GetGesetztStatus() &&
                   !Schatten6_nach_Block9.GetGesetztStatus() &&
                   !Schatten7_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten5_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten4_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten4_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten4_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten2_nach_Block9.GetGesetztStatus() &&
                   !Schatten3_nach_Block9.GetGesetztStatus() &&
                   !Schatten5_nach_Block9.GetGesetztStatus() &&
                   !Schatten6_nach_Block9.GetGesetztStatus() &&
                   !Schatten7_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten4_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten3_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten3_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten3_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten2_nach_Block9.GetGesetztStatus() &&
                   !Schatten4_nach_Block9.GetGesetztStatus() &&
                   !Schatten5_nach_Block9.GetGesetztStatus() &&
                   !Schatten6_nach_Block9.GetGesetztStatus() &&
                   !Schatten7_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten3_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten2_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten2_nach_Block9.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten2_nach_Block9);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block9.GetGesetztStatus() &&
                   !Schatten3_nach_Block9.GetGesetztStatus() &&
                   !Schatten4_nach_Block9.GetGesetztStatus() &&
                   !Schatten5_nach_Block9.GetGesetztStatus() &&
                   !Schatten6_nach_Block9.GetGesetztStatus() &&
                   !Schatten7_nach_Block9.GetGesetztStatus() &&
                   !Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten2_nach_Block9);
                }
            }
        }
        private void Fahrstr_Schatten1_Ausf_Click(object sender, EventArgs e)
        {
            //Einer der Block2-Fahrstrassen aktiv
            if (Schatten1_nach_Block8.GetGesetztStatus() ||
               Schatten1_nach_Block9.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Schatten1_nach_Block8.GetGesetztStatus()) ToggleFahrstrasse(Schatten1_nach_Block8);
                if (Schatten1_nach_Block9.GetGesetztStatus()) ToggleFahrstrasse(Schatten1_nach_Block9);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Schatten1_Auswahl.Visible) Schatten1_Auswahl.Visible = false;
                else Schatten1_Auswahl.Visible = true;
            }
        }
        private void Schatten1_Block8_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Schatten1_nach_Block8);
            Schatten1_Auswahl.Visible = false;
        }
        private void Schatten1_Block9_Click(object sender, EventArgs e)
        {
            ToggleFahrstrasse(Schatten1_nach_Block9);
            Schatten1_Auswahl.Visible = false;
        }
        private void Fahrstr_Schatten0_Ausf_Click(object sender, EventArgs e)
        {
            if (Schatten0_nach_Block8.GetGesetztStatus())
            {
                ToggleFahrstrasse(Schatten0_nach_Block8);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Schatten1_nach_Block8.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Schatten0_nach_Block8);
                }
            }
        }
        #endregion

        #region Freie Strecke
        private void Block5_Click(object sender, EventArgs e)
        {
            //Einer der Gl6-links Fahrstrassen aktiv
            if (Block1_nach_Block2.GetGesetztStatus() ||
                Block1_nach_Block5.GetGesetztStatus())
            {   //Aktive Fahrstrasse ausschalten
                if (Block1_nach_Block2.GetGesetztStatus()) ToggleFahrstrasse(Block1_nach_Block2);
                if (Block1_nach_Block5.GetGesetztStatus()) ToggleFahrstrasse(Block1_nach_Block5);
            }
            else
            {   //Gleisauswahl erscheinen lassen
                if (Block1_Auswahl.Visible) Block1_Auswahl.Visible = false;
                else Block1_Auswahl.Visible = true;
            }
        }
        private void Block5_Ausfahrt_Click(object sender, EventArgs e)
        {
            if (Block5_nach_Block6.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block5_nach_Block6);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Block8_nach_Block6.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Block5_nach_Block6);
                }
            }
        }
        private void Block8_Click(object sender, EventArgs e)
        {
            if (Block8_nach_Block6.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block8_nach_Block6);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Block5_nach_Block6.GetGesetztStatus() )
                {
                    ToggleFahrstrasse(Block8_nach_Block6);
                }
            }
        }
        private void Block9_Click(object sender, EventArgs e)
        {
            if (Block9_nach_Block2.GetGesetztStatus())
            {
                ToggleFahrstrasse(Block9_nach_Block2);  //Aktiv? auschalten
            }
            else
            {
                //Keine Sperrende Fahstraße aktiv
                if (!Block1_nach_Block2.GetGesetztStatus())
                {
                    ToggleFahrstrasse(Block9_nach_Block2);
                }
            }
        }
        #endregion       
    }
}