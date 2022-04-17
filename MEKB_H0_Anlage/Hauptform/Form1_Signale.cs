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

        private void SignalUpdate(Object source, ElapsedEventArgs e)
        {
            if(AutoSignale.Checked)
            {
                //Signale auf grün Schalten
                foreach(Signal signal in Signalliste)
                {
                    switch(signal.Name)
                    {
                        case "Signal_Ausfahrt_L1":
                            if (!GetWeiche("Weiche1").Abzweig && !GetWeiche("Weiche4").Abzweig && GetWeiche("Weiche6").Abzweig)
                                /*if block1 frei*/SchalteSignal("Signal_Ausfahrt_L1", Signal.HP2);
                            else
                                SchalteSignal("Signal_Ausfahrt_L1", Signal.HP0);
                            break;
                        case "Signal_Ausfahrt_L2":
                            if (!GetWeiche("Weiche1").Abzweig && !GetWeiche("Weiche4").Abzweig && !GetWeiche("Weiche6").Abzweig)
                                /*if block1 frei*/SchalteSignal("Signal_Ausfahrt_L2", Signal.HP1);
                            else
                                SchalteSignal("Signal_Ausfahrt_L2", Signal.HP0);
                            break;
                        case "Signal_Ausfahrt_L3":
                            if (GetWeiche("Weiche1").Abzweig && GetWeiche("Weiche2").Abzweig && !GetWeiche("Weiche3").Abzweig && !GetWeiche("Weiche5").Abzweig)
                                /*if block1 frei*/
                                SchalteSignal("Signal_Ausfahrt_L3", Signal.HP1);
                            else
                                SchalteSignal("Signal_Ausfahrt_L3", Signal.HP0);
                            break;
                        default: break;
                    }
                }            
            }
        }

        private void AutoSignalUpdate()
        {
            if (AutoSignale.Checked)
            {
                //Signale auf grün Schalten
                foreach (Signal signal in Signalliste)
                {
                    int Stellung = AllowedSignalPos(signal.Name);

                    if(signal.Zustand != Stellung) //Schalten bei Unterschied
                    {
                        signal.Schalten(Stellung, z21Start);
                    }
                }
            }
        }

        private int AllowedSignalPos(string SignalName)
        {
            if (AutoSignale.Checked)
            {
                switch (SignalName)
                {
                    case "Signal_Ausfahrt_L1":
                        if (!GetWeiche("Weiche1").Abzweig && !GetWeiche("Weiche4").Abzweig && GetWeiche("Weiche6").Abzweig)
                            /*if block1 frei*/
                            return Signal.HP2;
                        else
                            return Signal.HP0;
                    case "Signal_Ausfahrt_L2":
                        if (!GetWeiche("Weiche1").Abzweig && !GetWeiche("Weiche4").Abzweig && !GetWeiche("Weiche6").Abzweig)
                            /*if block1 frei*/
                            return Signal.HP1;
                        else
                            return Signal.HP0;
                    case "Signal_Ausfahrt_L3":
                        if (GetWeiche("Weiche1").Abzweig && GetWeiche("Weiche2").Abzweig && !GetWeiche("Weiche3").Abzweig && !GetWeiche("Weiche5").Abzweig)
                            /*if block1 frei*/
                            return Signal.HP1;
                        else
                            return Signal.HP0;
                    default: return Signal.HP0;
                }
            }//Check AutoSignale
            return 99; //Automatische Signale inaktiv
        }
    }
}
