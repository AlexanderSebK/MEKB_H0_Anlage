using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
        private void LokCtrl_LoklisteAusfuellen()
        {
            List<Lokomotive> Lokliste = LokomotivenArchiv.AlleLoks();

            foreach (Lokomotive Lokomtive in Lokliste)
            {
                LokCtrl1_Name.Items.Add(Lokomtive.Name);
                LokCtrl2_Name.Items.Add(Lokomtive.Name);
                LokCtrl3_Name.Items.Add(Lokomtive.Name);
                LokCtrl4_Name.Items.Add(Lokomtive.Name);
                LokCtrl5_Name.Items.Add(Lokomtive.Name);
                LokCtrl6_Name.Items.Add(Lokomtive.Name);
                LokCtrl7_Name.Items.Add(Lokomtive.Name);
                LokCtrl8_Name.Items.Add(Lokomtive.Name);
                LokCtrl9_Name.Items.Add(Lokomtive.Name);
                LokCtrl10_Name.Items.Add(Lokomtive.Name);
            }
        }  
        private void Setze_Lok_Fahrt(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            z21Start.Z21_SET_LOCO_DRIVE(Adresse, Fahrstufe, Richtung, Fahstrufeninfo);
        }
        private void Setze_Lok_Funktion(int Adresse, byte Zustand, byte FunktionsNr)
        {
            z21Start.Z21_SET_LOCO_FUNCTION(Adresse, Zustand, FunktionsNr);
        }
        private void Setze_Lok_Status(int Adresse)
        {
            z21Start.Z21_GET_LOCO_INFO(Adresse);
        }
    }
}
