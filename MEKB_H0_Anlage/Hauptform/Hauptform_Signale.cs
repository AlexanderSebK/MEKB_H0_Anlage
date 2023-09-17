using System;
using System.Timers;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEKB_H0_Anlage
{
    public partial class Hauptform : Form
    {
        public void GetSignalSchaltbild(Signal signal, PictureBox picBox)
        {
            dynamic img;
            if(signal.Typ.Equals("3B_270"))
            {
                switch(signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP0_270;break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP1_270; break;
                    case SignalZustand.HP2: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP2_270; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_270; break;
                }
            }
            else if (signal.Typ.Equals("2B_270"))
            {
                switch (signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_270; break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_270; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_270; break;
                }
            }
            
            else if (signal.Typ.Equals("2B_180"))
            {
                switch (signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_180; break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_180; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_180; break;
                }
            }
            else if (signal.Typ.Equals("3B_90"))
            {
                switch (signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP0_90; break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP1_90; break;
                    case SignalZustand.HP2: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_HP2_90; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_3B_90; break;
                }
            }
            else if (signal.Typ.Equals("2B_90"))
            {
                switch (signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_90; break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_90; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_90; break;
                }
            }
            else if (signal.Typ.Equals("2B_0"))
            {
                switch (signal.Zustand)
                {
                    case SignalZustand.HP0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_0; break;
                    case SignalZustand.HP1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_0; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_0; break;
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
        private void GetSignalStatus_Z21(string Signalname)
        {

            Signal signal = SignalListe.GetSignal(Signalname); //Weiche mit diesem Namen in der Liste suchen
            if (signal == null) return;                                               //Weiche nicht vorhanden, Funktion abbrechen
            int Adresse = signal.Adresse;                             //Adresse der Weiche übernehmen
            z21Start.LAN_X_GET_TURNOUT_INFO(Adresse);                                       //paket senden "GET Weiche"
            Adresse = signal.Adresse2;                             //Adresse der Weiche übernehmen
            z21Start.LAN_X_GET_TURNOUT_INFO(Adresse);                                       //paket senden "GET Weiche"
        }

        private void AutoSignalUpdate(string Signalname)
        {
            if (AutoSignale.Checked)
            {
                Signal signal = SignalListe.GetSignal(Signalname);
                if (signal != null)
                {
                    //SignalZustand Stellung = AllowedSignalPos(signal.Name);
                    SignalZustand ErlaubteSignalstellung = signal.ErlaubteStellung(FahrstrassenListe, WeichenListe);

                    if (signal.Zustand != ErlaubteSignalstellung) //Schalten bei Unterschied
                    {
                        if (ErlaubteSignalstellung != SignalZustand.HP0)
                        {
                            //Autoschalten auf Fahrt, wenn Option es erlaubt
                            if (Config.ReadConfig("AutoSignalFahrt").Equals("true"))
                            {
                                signal.Schalten(ErlaubteSignalstellung);
                            }
                        }
                        else //Schalten auf Halt
                        {
                            signal.Schalten(SignalZustand.HP0);
                        }
                    }
                }             
             
            }
        }
    }
}
