using System;
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
