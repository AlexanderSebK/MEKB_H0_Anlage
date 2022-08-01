using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public class Fahrstrasse
    {
        public Fahrstrasse()
        {
            Fahrstr_Weichenliste = new List<Weiche>();
            ControlSetPointer = 0;
            SetPointer = 0;
            Fahrstr_Sig = new Signal();
        }
        public List<Weiche> Fahrstr_Weichenliste { get; set; }

        public Signal Fahrstr_Sig;
        public bool Safe { get; set; }
        private bool FahrstrasseGesetzt { get; set; }
        private bool FahrstrasseAktiv { get; set; }

        private int ControlSetPointer;
        private int SetPointer;

        private void WeichenSicherheit(List<Weiche> ListeGlobal, bool Sicherheitsstatus)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;
                ListeGlobal[ListID].FahrstrasseSicher = Sicherheitsstatus;
            }
        }

        public void StarteFahrstrasse(List<Weiche> ListeGlobal)
        {
            FahrstrasseGesetzt = true;
            Safe = false;
            WeichenSicherheit(ListeGlobal, Safe);
        }

        public void SetFahrstrasseRichtung(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;

                ListeGlobal[ListID].FahrstrasseRichtung_vonZunge = weiche.FahrstrasseRichtung_vonZunge;
            }
        }

        public void SetFahrstrasse(List<Weiche> ListeGlobal, Z21 Z21_Instanz)
        {
            if (SetPointer >= Fahrstr_Weichenliste.Count) SetPointer = 0;
            Weiche weiche = Fahrstr_Weichenliste[SetPointer];


            int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
            if (ListID == -1) return;

            if (ListeGlobal[ListID].Status_Unbekannt || ListeGlobal[ListID].Status_Error)
            {
                MessageBox.Show("Weiche reagiert nicht. Verbindung von Z21 prüfen", "Weiche unbekannt oder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FahrstrasseGesetzt = false;
                FahrstrasseAktiv = false;
                return;
            }

            int Adresse = ListeGlobal[ListID].Adresse;
            ListeGlobal[ListID].FahrstrasseRichtung_vonZunge = weiche.FahrstrasseRichtung_vonZunge;
            ListeGlobal[ListID].FahrstrasseAbzweig = weiche.FahrstrasseAbzweig;

            if (ListeGlobal[ListID].Abzweig != weiche.FahrstrasseAbzweig)   //Wenn Weiche noch nicht in Position ist
            {
                if (ListeGlobal[ListID].ZeitAktiv <= 0) //Weiche nicht aktiv
                {
                    if (ListeGlobal[ListID].Spiegeln)
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(Adresse, !weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    else
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(Adresse, weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                }
            }
            SetPointer++; // Nächste Weiche
        }
        public void ControlSetFahrstrasse(List<Weiche> ListeGlobal, Z21 Z21_Instanz)
        {
            if (Fahrstr_Weichenliste.Count == 0) //Weichenloser Block
            {
                Safe = true;
                WeichenSicherheit(ListeGlobal, Safe);
                return;
            }
            if (!Safe)
            {
                if (ControlSetPointer < Fahrstr_Weichenliste.Count)
                {
                    Weiche weiche = Fahrstr_Weichenliste[ControlSetPointer];
                    int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                    if (ListID == -1) return;

                    if (ListeGlobal[ListID].Spiegeln)
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(ListeGlobal[ListID].Adresse, !weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    else
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(ListeGlobal[ListID].Adresse, weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    ControlSetPointer++;
                }
                else
                {
                    if (GetBusyStatus(ListeGlobal) == false)
                    {
                        ControlSetPointer = 0;
                        Safe = true;
                        WeichenSicherheit(ListeGlobal, Safe);
                    }
                }
            }
        }
        public void AktiviereFahrstasse(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;

                ListeGlobal[ListID].FahrstrasseAktive = true;
            }
            FahrstrasseAktiv = true;
        }

        public bool GetBusyStatus(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return false;

                if (ListeGlobal[ListID].ZeitAktiv > 0) return true; //Eine Weiche noch beim Schalten?
            }
            return false;
        }

        public bool GetGesetztStatus()
        {
            return FahrstrasseGesetzt;
        }
        public bool GetAktivStatus()
        {
            return FahrstrasseAktiv;
        }
        public bool CheckFahrstrassePos(List<Weiche> ListeGlobal)
        {
            if (!FahrstrasseGesetzt) return false;
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return false;
                if (ListeGlobal[ListID].Abzweig != weiche.FahrstrasseAbzweig)   //Wenn Weiche noch nicht in Position ist
                {
                    return false;
                }
            }
            return true;
        }


        public List<Weiche> GetFahrstrassenListe()
        {
            return Fahrstr_Weichenliste;
        }
        public void DeleteFahrstrasse(List<Weiche> ListeGlobal)
        {
            FahrstrasseGesetzt = false;
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;

                ListeGlobal[ListID].FahrstrasseAktive = false;
            }
            FahrstrasseAktiv = false;
            Safe = false;
            WeichenSicherheit(ListeGlobal, Safe);
        }

    }

    public class FahrstrassenConfig
    {
        public FahrstrassenConfig()
        {

        }
    }
}
