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
    public partial class Form1 : Form
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

        

        private SignalZustand GetSignalZustand(string Signalname)
        {
            Signal signal = SignalListe.GetSignal(Signalname);
            if (signal != null)
            {
                return signal.Zustand;
            }
            return SignalZustand.Unbestimmt;
        }

        private void AutoSignalUpdate(string Signalname)
        {
            if (AutoSignale.Checked)
            {
                Signal signal = SignalListe.GetSignal(Signalname);
                if (signal != null)
                {
                    SignalZustand Stellung = AllowedSignalPos(signal.Name);

                    if (signal.Zustand != Stellung) //Schalten bei Unterschied
                    {
                        if (Stellung != SignalZustand.HP0)
                        {
                            //Autoschalten auf Fahrt, wenn Option es erlaubt
                            if (Config.ReadConfig("AutoSignalFahrt").Equals("true"))
                            {
                                signal.Schalten(Stellung, z21Start);
                            }
                        }
                        else //Schalten auf Halt
                        {
                            signal.Schalten(Stellung, z21Start);
                        }
                    }
                }             
             
            }
        }

        private SignalZustand AllowedSignalPos(string SignalName)
        {
            if (AutoSignale.Checked)
            {
                bool FahrstrassenStrg = false;
                if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true")) FahrstrassenStrg = true;


                switch (SignalName)
                {
                    case "Signal_Ausfahrt_L1":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche1").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche4").Abzweig &&
                                WeichenListe.GetWeiche("Weiche6").Abzweig &&
                                BelegtmelderListe.BlockFrei("Block1") &&
                                BelegtmelderListe.BlockFrei("Bhf_Gleis1_AusfahrtL"))
                                return SignalZustand.HP2;                                
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis1_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_L2":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche1").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche4").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche6").Abzweig)
                                /*if block1 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis2_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_L3":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche1").Abzweig && 
                                WeichenListe.GetWeiche("Weiche2").Abzweig && 
                                !WeichenListe.GetWeiche("Weiche3").Abzweig && 
                                !WeichenListe.GetWeiche("Weiche5").Abzweig)
                                /*if block1 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis3_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_L4":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche1").Abzweig &&
                                WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW7_2").Abzweig)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis4_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_L5":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche1").Abzweig &&
                                WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche8").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW9_2").Abzweig)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis5_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_L6":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche1").Abzweig &&
                                WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche8").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_2").Abzweig)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis6_nach_Block1").Safe)
                                /*if block1 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;

                    case "Signal_Einfahrt_L":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche4").Abzweig &&
                                WeichenListe.GetWeiche("Weiche6").Abzweig)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche4").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche6").Abzweig)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche5").Abzweig)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW7_2").Abzweig)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche8").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW9_2").Abzweig)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche3").Abzweig &&
                                WeichenListe.GetWeiche("Weiche5").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW7_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche8").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW9_2").Abzweig)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis1").Safe)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis2").Safe)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis3").Safe)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis4").Safe)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis5").Safe)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Block2_nach_Gleis6").Safe)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;

                    case "Signal_Ausfahrt_R1":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis1_nach_TunnelAussen").Safe)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis1_nach_TunnelInnen").Safe)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_R2":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis2_nach_TunnelAussen").Safe)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis2_nach_TunnelInnen").Safe)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_R3":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis3_nach_TunnelAussen").Safe)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis3_nach_TunnelInnen").Safe)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_R4":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis4_nach_TunnelAussen").Safe)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis4_nach_TunnelInnen").Safe)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_R5":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                !WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                !WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis5_nach_TunnelAussen").Safe)
                                /*if Block Rechts1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis5_nach_TunnelInnen").Safe)
                                /*if Block Rechts2 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Ausfahrt_R6":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche21").Abzweig &&
                                WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block2 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("Weiche21").Abzweig &&
                                WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche30").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche50").Abzweig)
                                /*if Block5 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Gleis6_nach_TunnelAussen").Safe)
                                /*if Block2 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("Gleis6_nach_TunnelInnen").Safe)
                                /*if Block5 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_RTunnel_1":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP2;
                            if (!WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                !WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP2;
                            if (WeichenListe.GetWeiche("Weiche21").Abzweig &&
                                WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP2;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis1").Safe)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis2").Safe)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis3").Safe)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis4").Safe)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis5").Safe)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP2;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelAussen_nach_Gleis6").Safe)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP2;
                        }
                        return SignalZustand.HP0;
                    case "Signal_RTunnel_2":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche26").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                !WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                !WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche21").Abzweig &&
                                WeichenListe.GetWeiche("KW22_1").Abzweig &&
                                WeichenListe.GetWeiche("KW22_2").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche23").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_1").Abzweig &&
                                WeichenListe.GetWeiche("DKW24_2").Abzweig &&
                                WeichenListe.GetWeiche("Weiche25").Abzweig &&
                                WeichenListe.GetWeiche("Weiche28").Abzweig &&
                                WeichenListe.GetWeiche("Weiche27").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche29").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche51").Abzweig)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis1").Safe)
                                /*if Gleis1 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis2").Safe)
                                /*if Gleis2 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis3").Safe)
                                /*if Gleis3 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis4").Safe)
                                /*if Gleis4 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis5").Safe)
                                /*if Gleis5 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("TunnelInnen_nach_Gleis6").Safe)
                                /*if Gleis6 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;

                    case "Signal_Block5":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche52").Abzweig &&
                                WeichenListe.GetWeiche("Weiche53").Abzweig)
                                /*if Block2 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche52").Abzweig)
                                /*if Block5 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block1_nach_Block2").Safe)
                                /*if Block2 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Block1_nach_Block5").Safe)
                                /*if Block5 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Block2":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche53").Abzweig)
                                /*if Block2 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block9_nach_Block2").Safe)
                                /*if Block2 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Block6":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche60").Abzweig)
                                /*if Block6 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block5_nach_Block6").Safe)
                                /*if Block6 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Block8":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Schatten1") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten2") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten3") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten4") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten5") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten6") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (GetSignalZustand("Signal_Schatten7") != SignalZustand.HP0) return SignalZustand.HP0;
                            if (WeichenListe.GetWeiche("Weiche60").Abzweig)
                                /*if Block6 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block8_nach_Block6").Safe)
                                /*if Block6 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;

                    case "Signal_Schatten0":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche68").Abzweig)
                                /*if Block8 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten0_nach_Block8").Safe)
                                /*if Block8 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten1":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (!WeichenListe.GetWeiche("Weiche68").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche67").Abzweig)
                                /*if Block8 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche67").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche66").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche65").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche64").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche63").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten1_nach_Block8").Safe)
                                /*if Block8 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten1_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten2":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (WeichenListe.GetWeiche("Weiche66").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche65").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche64").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche63").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten2_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten3":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (WeichenListe.GetWeiche("Weiche65").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche64").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche63").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten3_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten4":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (WeichenListe.GetWeiche("Weiche64").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche63").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten4_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten5":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (WeichenListe.GetWeiche("Weiche63").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten5_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten6":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (WeichenListe.GetWeiche("Weiche62").Abzweig &&
                                WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten6_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten7":
                        if (!FahrstrassenStrg)
                        {
                            if (GetSignalZustand("Signal_Block8") != SignalZustand.HP0) return SignalZustand.HP0;

                            if (!WeichenListe.GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten7_nach_Block9").Safe)
                                /*if Block9 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten8":
                        if (!FahrstrassenStrg)
                        {
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                WeichenListe.GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                WeichenListe.GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                WeichenListe.GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                WeichenListe.GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                WeichenListe.GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return SignalZustand.HP1;
                           if (WeichenListe.GetWeiche("Weiche80").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                               !WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return SignalZustand.HP1;

                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Eingleisen").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten1").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten2").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten3").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten4").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten5").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten6").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten8_nach_Schatten7").Safe) return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten9":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                WeichenListe.GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                WeichenListe.GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                WeichenListe.GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                WeichenListe.GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                WeichenListe.GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return SignalZustand.HP1;

                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Eingleisen").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten1").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten2").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten3").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten4").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten5").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten6").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten9_nach_Schatten7").Safe) return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten10":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                WeichenListe.GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                WeichenListe.GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                WeichenListe.GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                WeichenListe.GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                WeichenListe.GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                WeichenListe.GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return SignalZustand.HP1;

                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Eingleisen").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten1").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten2").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten3").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten4").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten5").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten6").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten10_nach_Schatten7").Safe) return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten11":
                        if (!FahrstrassenStrg)
                        {
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                WeichenListe.GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                WeichenListe.GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                WeichenListe.GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                WeichenListe.GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                WeichenListe.GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                WeichenListe.GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return SignalZustand.HP1;
                            if (!WeichenListe.GetWeiche("Weiche80").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche81").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche82").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche70").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche71").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche72").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche73").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche74").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche75").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return SignalZustand.HP1;

                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Eingleisen").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten1").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten2").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten3").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten4").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten5").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten6").Safe) return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Schatten11_nach_Schatten7").Safe) return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    case "Signal_Schatten_Einf":
                        if (!FahrstrassenStrg)
                        {
                           if (!WeichenListe.GetWeiche("Weiche90").Abzweig)
                                /*if Schatten11 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche90").Abzweig &&
                                WeichenListe.GetWeiche("Weiche91").Abzweig)
                                /*if Schatten10 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche90").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche91").Abzweig &&
                                WeichenListe.GetWeiche("Weiche92").Abzweig)
                                /*if Schatten9 frei*/
                                return SignalZustand.HP1;
                            if (WeichenListe.GetWeiche("Weiche90").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche91").Abzweig &&
                                !WeichenListe.GetWeiche("Weiche92").Abzweig)
                                /*if Schatten8 frei*/
                                return SignalZustand.HP1;
                        }
                        else
                        {
                            if (FahrstrassenListe.GetFahrstrasse("Block6_nach_Schatten11").Safe)
                                /*if Schatten11 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Block6_nach_Schatten10").Safe)
                                /*if Schatten10 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Block6_nach_Schatten9").Safe)
                                /*if Schatten9 frei*/
                                return SignalZustand.HP1;
                            if (FahrstrassenListe.GetFahrstrasse("Block6_nach_Schatten8").Safe)
                                /*if Schatten8 frei*/
                                return SignalZustand.HP1;
                        }
                        return SignalZustand.HP0;
                    default: return SignalZustand.HP0;
                }
            }//Check AutoSignale
            return SignalZustand.Unbestimmt; //Automatische Signale inaktiv
        }
    }
}
