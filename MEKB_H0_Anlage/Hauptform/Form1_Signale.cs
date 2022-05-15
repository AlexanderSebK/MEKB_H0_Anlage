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
        public void SetupSignalListe()
        {
            XElement XMLFile = XElement.Load("Signalliste.xml");       //XML-Datei öffnen
            var list = XMLFile.Elements("Signal").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach (XElement signal in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                int SAdresse = Int16.Parse(signal.Element("Adresse").Value);                               //Signaladresse des Elements auslesen
                int SAdresse2;
                if (signal.Element("Adresse2") == null) SAdresse2 = 0;//Nicht vorhanden - 2.Adresse 0 eintragen
                else SAdresse2 = Int16.Parse(signal.Element("Adresse2").Value);//2. Signaladresse des Elements auslesen

                string SName = signal.Element("Name").Value;                                                //Signal Name des Elements auslesen
                string STyp = signal.Element("Typ").Value;                               //Typ des Signals auslesen
                int SAdr11 = Int16.Parse(signal.Element("Adr1Zustand1").Value);          //Zustand bei Signaladresse Schaltung auf 0
                int SAdr12 = Int16.Parse(signal.Element("Adr1Zustand2").Value);          //Zustand bei Signaladresse Schaltung auf 1
                int SAdr21 = Int16.Parse(signal.Element("Adr2Zustand1").Value);          //Zustand bei 2. Signaladresse Schaltung auf 0
                int SAdr22 = Int16.Parse(signal.Element("Adr2Zustand2").Value);          //Zustand bei 2. Signaladresse Schaltung auf 1

                Signalliste.Add(new Signal() 
                    { 
                        Name = SName, 
                        Adresse = SAdresse, 
                        Adresse2 = SAdresse2, 
                        Typ = STyp, 
                        Adr1_1 = SAdr11,
                        Adr1_2 = SAdr12, 
                        Adr2_1 = SAdr21, 
                        Adr2_2 = SAdr22
                }) ;  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
            }
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
            
            else if (signal.Typ.Equals("2B_180"))
            {
                switch (signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_180; break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_180; break;
                    default: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_180; break;
                }
            }
            else if (signal.Typ.Equals("3B_90"))
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
            else if (signal.Typ.Equals("2B_0"))
            {
                switch (signal.Zustand)
                {
                    case 0: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP0_0; break;
                    case 1: img = MEKB_H0_Anlage.Properties.Resources.Signal_2B_HP1_0; break;
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
        private void GetSignalStatus(string Signalname)
        {          

            int ListID = Signalliste.IndexOf(new Signal() { Name = Signalname }); //Weiche mit diesem Namen in der Liste suchen
            if (ListID == -1) return;                                               //Weiche nicht vorhanden, Funktion abbrechen
            int Adresse = Signalliste[ListID].Adresse;                             //Adresse der Weiche übernehmen
            z21Start.Z21_GET_WEICHE(Adresse);                                       //paket senden "GET Weiche"
            Adresse = Signalliste[ListID].Adresse2;                             //Adresse der Weiche übernehmen
            z21Start.Z21_GET_WEICHE(Adresse);                                       //paket senden "GET Weiche"
        }

        private void SchalteSignal(string name, int HPx)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = name });
            if (ListID != -1)
            {
                Signalliste[ListID].Schalten(HPx, z21Start);
            }
        }

        private int getSignalZustand(string Signalname)
        {
            int ListID = Signalliste.IndexOf(new Signal() { Name = Signalname });
            if (ListID != -1)
            {
                return Signalliste[ListID].Zustand;
            }
            return -1;
        }

        private void AutoSignalUpdate(string Signalname)
        {
            if (AutoSignale.Checked)
            {
                int ListID = Signalliste.IndexOf(new Signal() { Name = Signalname });
                if (ListID != -1)
                {
                    int Stellung = AllowedSignalPos(Signalliste[ListID].Name);

                    if (Signalliste[ListID].Zustand != Stellung) //Schalten bei Unterschied
                    {
                        if (Stellung != Signal.HP0)
                        {
                            //Autoschalten auf Fahrt, wenn Option es erlaubt
                            if (Config.ReadConfig("AutoSignalFahrt").Equals("true"))
                            {
                                Signalliste[ListID].Schalten(Stellung, z21Start);
                            }
                        }
                        else //Schalten auf Halt
                        {
                            Signalliste[ListID].Schalten(Stellung, z21Start);
                        }
                    }
                }             
             
            }
        }

        private int AllowedSignalPos(string SignalName)
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
                            if (!GetWeiche("Weiche1").Abzweig &&
                                !GetWeiche("Weiche4").Abzweig &&
                                GetWeiche("Weiche6").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP2;                                
                        }
                        else
                        {
                            if (Gleis1_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_L2":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche1").Abzweig &&
                                !GetWeiche("Weiche4").Abzweig &&
                                !GetWeiche("Weiche6").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Gleis2_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_L3":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche1").Abzweig && 
                                GetWeiche("Weiche2").Abzweig && 
                                !GetWeiche("Weiche3").Abzweig && 
                                !GetWeiche("Weiche5").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Gleis3_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_L4":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche1").Abzweig &&
                                GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                !GetWeiche("DKW7_2").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis4_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_L5":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche1").Abzweig &&
                                GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                GetWeiche("DKW7_2").Abzweig &&
                                !GetWeiche("Weiche8").Abzweig &&
                                GetWeiche("DKW9_1").Abzweig &&
                                !GetWeiche("DKW9_2").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis5_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_L6":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche1").Abzweig &&
                                GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                GetWeiche("DKW7_2").Abzweig &&
                                !GetWeiche("Weiche8").Abzweig &&
                                GetWeiche("DKW9_1").Abzweig &&
                                GetWeiche("DKW9_2").Abzweig)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis6_nach_Block1.Safe)
                                /*if block1 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;

                    case "Signal_Einfahrt_L":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche2").Abzweig &&
                                GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche4").Abzweig &&
                                GetWeiche("Weiche6").Abzweig)
                                /*if Gleis1 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche2").Abzweig &&
                                GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche4").Abzweig &&
                                !GetWeiche("Weiche6").Abzweig)
                                /*if Gleis2 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                !GetWeiche("Weiche5").Abzweig)
                                /*if Gleis3 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                !GetWeiche("DKW7_2").Abzweig)
                                /*if Gleis4 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                GetWeiche("DKW7_2").Abzweig &&
                                !GetWeiche("Weiche8").Abzweig &&
                                GetWeiche("DKW9_1").Abzweig &&
                                !GetWeiche("DKW9_2").Abzweig)
                                /*if Gleis5 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche2").Abzweig &&
                                !GetWeiche("Weiche3").Abzweig &&
                                GetWeiche("Weiche5").Abzweig &&
                                GetWeiche("DKW7_1").Abzweig &&
                                GetWeiche("DKW7_2").Abzweig &&
                                !GetWeiche("Weiche8").Abzweig &&
                                GetWeiche("DKW9_1").Abzweig &&
                                GetWeiche("DKW9_2").Abzweig)
                                /*if Gleis6 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Block2_nach_Gleis1.Safe)
                                /*if Gleis1 frei*/
                                return Signal.HP2;
                            if (Block2_nach_Gleis2.Safe)
                                /*if Gleis2 frei*/
                                return Signal.HP2;
                            if (Block2_nach_Gleis3.Safe)
                                /*if Gleis3 frei*/
                                return Signal.HP2;
                            if (Block2_nach_Gleis4.Safe)
                                /*if Gleis4 frei*/
                                return Signal.HP2;
                            if (Block2_nach_Gleis5.Safe)
                                /*if Gleis5 frei*/
                                return Signal.HP2;
                            if (Block2_nach_Gleis6.Safe)
                                /*if Gleis6 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;

                    case "Signal_Ausfahrt_R1":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis1_nach_rechts1.Safe)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (Gleis1_nach_rechts2.Safe)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_R2":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis2_nach_rechts1.Safe)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (Gleis2_nach_rechts2.Safe)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_R3":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis3_nach_rechts1.Safe)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (Gleis3_nach_rechts2.Safe)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_R4":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("DKW24_1").Abzweig &&
                                !GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (GetWeiche("DKW24_1").Abzweig &&
                                !GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis4_nach_rechts1.Safe)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (Gleis4_nach_rechts2.Safe)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_R5":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("KW22_1").Abzweig &&
                                !GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (GetWeiche("KW22_1").Abzweig &&
                                !GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis5_nach_rechts1.Safe)
                                /*if Block Rechts1 frei*/
                                return Signal.HP2;
                            if (Gleis5_nach_rechts2.Safe)
                                /*if Block Rechts2 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_Ausfahrt_R6":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche21").Abzweig &&
                                GetWeiche("KW22_1").Abzweig &&
                                GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                GetWeiche("Weiche50").Abzweig)
                                /*if Block2 frei*/
                                return Signal.HP2;
                            if (GetWeiche("Weiche21").Abzweig &&
                                GetWeiche("KW22_1").Abzweig &&
                                GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                !GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche30").Abzweig &&
                                !GetWeiche("Weiche50").Abzweig)
                                /*if Block5 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Gleis6_nach_rechts1.Safe)
                                /*if Block2 frei*/
                                return Signal.HP2;
                            if (Gleis6_nach_rechts2.Safe)
                                /*if Block5 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_RTunnel_1":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis1 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis2 frei*/
                                return Signal.HP2;
                            if (!GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis3 frei*/
                                return Signal.HP2;
                            if (GetWeiche("DKW24_1").Abzweig &&
                                !GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis4 frei*/
                                return Signal.HP2;
                            if (GetWeiche("KW22_1").Abzweig &&
                                !GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis5 frei*/
                                return Signal.HP2;
                            if (GetWeiche("Weiche21").Abzweig &&
                                GetWeiche("KW22_1").Abzweig &&
                                GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                GetWeiche("Weiche51").Abzweig)
                                /*if Gleis6 frei*/
                                return Signal.HP2;
                        }
                        else
                        {
                            if (Rechts1_nach_Gleis1.Safe)
                                /*if Gleis1 frei*/
                                return Signal.HP2;
                            if (Rechts1_nach_Gleis2.Safe)
                                /*if Gleis2 frei*/
                                return Signal.HP2;
                            if (Rechts1_nach_Gleis3.Safe)
                                /*if Gleis3 frei*/
                                return Signal.HP2;
                            if (Rechts1_nach_Gleis4.Safe)
                                /*if Gleis4 frei*/
                                return Signal.HP2;
                            if (Rechts1_nach_Gleis5.Safe)
                                /*if Gleis5 frei*/
                                return Signal.HP2;
                            if (Rechts1_nach_Gleis6.Safe)
                                /*if Gleis6 frei*/
                                return Signal.HP2;
                        }
                        return Signal.HP0;
                    case "Signal_RTunnel_2":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis1 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche26").Abzweig &&
                                !GetWeiche("Weiche28").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis2 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis3 frei*/
                                return Signal.HP1;
                            if (GetWeiche("DKW24_1").Abzweig &&
                                !GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis4 frei*/
                                return Signal.HP1;
                            if (GetWeiche("KW22_1").Abzweig &&
                                !GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis5 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche21").Abzweig &&
                                GetWeiche("KW22_1").Abzweig &&
                                GetWeiche("KW22_2").Abzweig &&
                                !GetWeiche("Weiche23").Abzweig &&
                                GetWeiche("DKW24_1").Abzweig &&
                                GetWeiche("DKW24_2").Abzweig &&
                                GetWeiche("Weiche25").Abzweig &&
                                GetWeiche("Weiche28").Abzweig &&
                                GetWeiche("Weiche27").Abzweig &&
                                !GetWeiche("Weiche29").Abzweig &&
                                !GetWeiche("Weiche51").Abzweig)
                                /*if Gleis6 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Rechts2_nach_Gleis1.Safe)
                                /*if Gleis1 frei*/
                                return Signal.HP1;
                            if (Rechts2_nach_Gleis2.Safe)
                                /*if Gleis2 frei*/
                                return Signal.HP1;
                            if (Rechts2_nach_Gleis3.Safe)
                                /*if Gleis3 frei*/
                                return Signal.HP1;
                            if (Rechts2_nach_Gleis4.Safe)
                                /*if Gleis4 frei*/
                                return Signal.HP1;
                            if (Rechts2_nach_Gleis5.Safe)
                                /*if Gleis5 frei*/
                                return Signal.HP1;
                            if (Rechts2_nach_Gleis6.Safe)
                                /*if Gleis6 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;

                    case "Signal_Block5":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche52").Abzweig &&
                                GetWeiche("Weiche53").Abzweig)
                                /*if Block2 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche52").Abzweig)
                                /*if Block5 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Block1_nach_Block2.Safe)
                                /*if Block2 frei*/
                                return Signal.HP1;
                            if (Block1_nach_Block5.Safe)
                                /*if Block5 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Block2":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche53").Abzweig)
                                /*if Block2 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Block9_nach_Block2.Safe)
                                /*if Block2 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Block6":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche60").Abzweig)
                                /*if Block6 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Block5_nach_Block6.Safe)
                                /*if Block6 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Block8":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Schatten1") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten2") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten3") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten4") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten5") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten6") != Signal.HP0) return Signal.HP0;
                            if (getSignalZustand("Signal_Schatten7") != Signal.HP0) return Signal.HP0;
                            if (GetWeiche("Weiche60").Abzweig)
                                /*if Block6 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Block8_nach_Block6.Safe)
                                /*if Block6 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;

                    case "Signal_Schatten0":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche68").Abzweig)
                                /*if Block8 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten0_nach_Block8.Safe)
                                /*if Block8 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten1":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (!GetWeiche("Weiche68").Abzweig &&
                                !GetWeiche("Weiche67").Abzweig)
                                /*if Block8 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche67").Abzweig &&
                                !GetWeiche("Weiche66").Abzweig &&
                                !GetWeiche("Weiche65").Abzweig &&
                                !GetWeiche("Weiche64").Abzweig &&
                                !GetWeiche("Weiche63").Abzweig &&
                                !GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten1_nach_Block8.Safe)
                                /*if Block8 frei*/
                                return Signal.HP1;
                            if (Schatten1_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten2":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (GetWeiche("Weiche66").Abzweig &&
                                !GetWeiche("Weiche65").Abzweig &&
                                !GetWeiche("Weiche64").Abzweig &&
                                !GetWeiche("Weiche63").Abzweig &&
                                !GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten2_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten3":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (GetWeiche("Weiche65").Abzweig &&
                                !GetWeiche("Weiche64").Abzweig &&
                                !GetWeiche("Weiche63").Abzweig &&
                                !GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten3_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten4":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (GetWeiche("Weiche64").Abzweig &&
                                !GetWeiche("Weiche63").Abzweig &&
                                !GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten4_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten5":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (GetWeiche("Weiche63").Abzweig &&
                                !GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten5_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten6":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (GetWeiche("Weiche62").Abzweig &&
                                GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten6_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten7":
                        if (!FahrstrassenStrg)
                        {
                            if (getSignalZustand("Signal_Block8") != Signal.HP0) return Signal.HP0;

                            if (!GetWeiche("Weiche61").Abzweig)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Schatten7_nach_Block9.Safe)
                                /*if Block9 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    case "Signal_Schatten8":
                        if (!FahrstrassenStrg)
                        {
                            if (GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return Signal.HP1;
                           if (GetWeiche("Weiche80").Abzweig &&
                               !GetWeiche("Weiche70").Abzweig &&
                               !GetWeiche("Weiche71").Abzweig &&
                               !GetWeiche("Weiche72").Abzweig &&
                               !GetWeiche("Weiche73").Abzweig &&
                               !GetWeiche("Weiche74").Abzweig &&
                               !GetWeiche("Weiche75").Abzweig &&
                               !GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return Signal.HP1;

                        }
                        else
                        {
                            if (Schatten8_nach_Block7.Safe && 
                                Block7_nach_Schatten0.Safe)
                                /*if Schatten0 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten1.Safe)
                                /*if Schatten1 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten2.Safe)
                                /*if Schatten2 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten3.Safe)
                                /*if Schatten3 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten4.Safe)
                                /*if Schatten4 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten5.Safe)
                                /*if Schatten5 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten6.Safe)
                                /*if Schatten6 frei*/
                                return Signal.HP1;
                            if (Schatten8_nach_Block7.Safe &&
                                Block7_nach_Schatten7.Safe)
                                /*if Schatten7 frei*/
                                return Signal.HP1;

                        }
                        return Signal.HP0;
                    case "Signal_Schatten9":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                !GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return Signal.HP1;

                        }
                        else
                        {
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten0.Safe)
                                /*if Schatten0 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten1.Safe)
                                /*if Schatten1 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten2.Safe)
                                /*if Schatten2 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten3.Safe)
                                /*if Schatten3 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten4.Safe)
                                /*if Schatten4 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten5.Safe)
                                /*if Schatten5 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten6.Safe)
                                /*if Schatten6 frei*/
                                return Signal.HP1;
                            if (Schatten9_nach_Block7.Safe &&
                                Block7_nach_Schatten7.Safe)
                                /*if Schatten7 frei*/
                                return Signal.HP1;

                        }
                        return Signal.HP0;
                    case "Signal_Schatten10":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                !GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return Signal.HP1;

                        }
                        else
                        {
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten0.Safe)
                                /*if Schatten0 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten1.Safe)
                                /*if Schatten1 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten2.Safe)
                                /*if Schatten2 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten3.Safe)
                                /*if Schatten3 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten4.Safe)
                                /*if Schatten4 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten5.Safe)
                                /*if Schatten5 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten6.Safe)
                                /*if Schatten6 frei*/
                                return Signal.HP1;
                            if (Schatten10_nach_Block7.Safe &&
                                Block7_nach_Schatten7.Safe)
                                /*if Schatten7 frei*/
                                return Signal.HP1;

                        }
                        return Signal.HP0;
                    case "Signal_Schatten11":
                        if (!FahrstrassenStrg)
                        {
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                GetWeiche("Weiche70").Abzweig)
                                /*if Schatten0 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                GetWeiche("Weiche71").Abzweig)
                                /*if Schatten1 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                GetWeiche("Weiche72").Abzweig)
                                /*if Schatten2 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                GetWeiche("Weiche73").Abzweig)
                                /*if Schatten3 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                GetWeiche("Weiche74").Abzweig)
                                /*if Schatten4 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                GetWeiche("Weiche75").Abzweig)
                                /*if Schatten5 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                GetWeiche("Weiche76").Abzweig)
                                /*if Schatten6 / Block 7 frei*/
                                return Signal.HP1;
                            if (!GetWeiche("Weiche80").Abzweig &&
                                !GetWeiche("Weiche81").Abzweig &&
                                !GetWeiche("Weiche82").Abzweig &&
                                !GetWeiche("Weiche70").Abzweig &&
                                !GetWeiche("Weiche71").Abzweig &&
                                !GetWeiche("Weiche72").Abzweig &&
                                !GetWeiche("Weiche73").Abzweig &&
                                !GetWeiche("Weiche74").Abzweig &&
                                !GetWeiche("Weiche75").Abzweig &&
                                !GetWeiche("Weiche76").Abzweig)
                                /*if Schatten7 / Block 7 frei*/
                                return Signal.HP1;

                        }
                        else
                        {
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten0.Safe)
                                /*if Schatten0 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten1.Safe)
                                /*if Schatten1 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten2.Safe)
                                /*if Schatten2 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten3.Safe)
                                /*if Schatten3 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten4.Safe)
                                /*if Schatten4 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten5.Safe)
                                /*if Schatten5 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten6.Safe)
                                /*if Schatten6 frei*/
                                return Signal.HP1;
                            if (Schatten11_nach_Block7.Safe &&
                                Block7_nach_Schatten7.Safe)
                                /*if Schatten7 frei*/
                                return Signal.HP1;

                        }
                        return Signal.HP0;
                    case "Signal_Schatten_Einf":
                        if (!FahrstrassenStrg)
                        {
                           if (!GetWeiche("Weiche90").Abzweig)
                                /*if Schatten11 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche90").Abzweig &&
                                GetWeiche("Weiche91").Abzweig)
                                /*if Schatten10 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche90").Abzweig &&
                                !GetWeiche("Weiche91").Abzweig &&
                                GetWeiche("Weiche92").Abzweig)
                                /*if Schatten9 frei*/
                                return Signal.HP1;
                            if (GetWeiche("Weiche90").Abzweig &&
                                !GetWeiche("Weiche91").Abzweig &&
                                !GetWeiche("Weiche92").Abzweig)
                                /*if Schatten8 frei*/
                                return Signal.HP1;
                        }
                        else
                        {
                            if (Block6_nach_Schatten11.Safe)
                                /*if Schatten11 frei*/
                                return Signal.HP1;
                            if (Block6_nach_Schatten10.Safe)
                                /*if Schatten10 frei*/
                                return Signal.HP1;
                            if (Block6_nach_Schatten9.Safe)
                                /*if Schatten9 frei*/
                                return Signal.HP1;
                            if (Block6_nach_Schatten8.Safe)
                                /*if Schatten8 frei*/
                                return Signal.HP1;
                        }
                        return Signal.HP0;
                    default: return Signal.HP0;
                }
            }//Check AutoSignale
            return 99; //Automatische Signale inaktiv
        }
    }
}
