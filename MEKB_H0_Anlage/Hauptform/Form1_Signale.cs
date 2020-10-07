using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
        public void SetupSignalListe()
        {
            Signalliste.Add(new Signal() { Name = "Signal_Ausfahrt_L1" });

            Signalliste[0].Adresse = 1000;
            Signalliste[0].Typ = "3B_270";
            Signalliste[0].Setzen(0);
        }

        public void GetSignalSchaltbild(Signal signal, PictureBox picBox)
        {
            dynamic img;
            if(signal.Typ.Equals("3B_270"))
            {
                switch(signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP0_270;break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP1_270; break;
                    case 2: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP2_270; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_270; break;
                }
            }
            else if (signal.Typ.Equals("2B_270"))
            {
                switch (signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_270; break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_270; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_270; break;
                }
            }
            else if (signal.Typ.Equals("3B_00"))
            {
                switch (signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP0_90; break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP1_90; break;
                    case 2: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP2_90; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_90; break;
                }
            }
            else if (signal.Typ.Equals("2B_90"))
            {
                switch (signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_90; break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_90; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_90; break;
                }
            }
            else
            {
                img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_270;
            }

            picBox.Invoke(new EventHandler(delegate
            {
                picBox.Image = img;
            }));
        }
    }
}
