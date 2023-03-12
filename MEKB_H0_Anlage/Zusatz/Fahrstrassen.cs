using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public class Fahrstrasse
    {
        public Fahrstrasse()
        {
            Fahrstr_Weichenliste = new List<Weiche>();
            ControlSetPointer = 0;
            SetPointer = 0;
            EinfahrtsSignal = new Signal();
            Name = "";
            Fahrstr_Blockierende = new List<string>();
            Fahrstr_GleicherEingang = new List<string>();
        }

        public Fahrstrasse(FahrstrassenKonfig config, WeichenListe weichenListe, SignalListe signalListe)
        {
            Name = config.Name;
            Fahrstr_Weichenliste = new List<Weiche>();
            ControlSetPointer = 0;
            SetPointer = 0;

            Fahrstr_Blockierende = config.Fahrstr_Blockierende;
            Fahrstr_GleicherEingang = config.Fahrstr_GleicherEingang;

            EinfahrtsSignal = signalListe.GetSignal(config.EinfahrtsSignal);
            EndSignal = signalListe.GetSignal(config.EndSignal);

            if (EinfahrtsSignal == null) 
            { 
                MessageBox.Show(
                    String.Format("Fehler in {0}: {1} nicht gefunden", Name, config.EinfahrtsSignal), 
                    "Schwerer Fehler", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error); 
                EinfahrtsSignal = new Signal(); 
                return; 
            }

            if (EndSignal == null)
            {
                MessageBox.Show(
                    String.Format("Fehler in {0}: {1} nicht gefunden", Name, config.EndSignal),
                    "Schwerer Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                EndSignal = new Signal();
                return;
            }

            Weiche weiche;
            foreach (WeichenKonfig weichenConfig in config.WeichenConfig)
            {
                weiche = weichenListe.GetWeiche(weichenConfig.Name).Copy();
                if (weiche == null) 
                { 
                    MessageBox.Show(
                        String.Format("Fehler in {0}: {1} nicht gefunden", Name, weichenConfig.Name),
                        "Schwerer Fehler", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    Fahrstr_Weichenliste = new List<Weiche>();
                    return; 
                }
                weiche.FahrstrasseRichtung_vonZunge = weichenConfig.RichtungVonZunge;
                weiche.FahrstrasseAbzweig = weichenConfig.Abzweig;
                Fahrstr_Weichenliste.Add(weiche);
            }
        }

        public string Name { get; set; }

        public List<Weiche> Fahrstr_Weichenliste { get; set; }
        public List<string> Fahrstr_Blockierende { get; set; }
        public List<string> Fahrstr_GleicherEingang { get; set; }
        /// <summary>
        /// Einfahrtssignal der Fahrtstrasse
        /// </summary>
        public Signal EinfahrtsSignal { get; set; }
        /// <summary>
        /// Ausfahrtssignal der Fahrstrasse
        /// </summary>
        public Signal EndSignal { get; set; }
        /// <summary>
        /// Alle Weichen zur Sicherheit nochmal geschaltet. Fahrstrasse ist sicher
        /// </summary>
        public bool Safe { get; set; }
        /// <summary>
        /// Fahrstrasse reserviert
        /// </summary>
        private bool FahrstrasseGesetzt { get; set; }
        /// <summary>
        /// Alle Weichen der Fahrstrasse in richtiger Stellung, aber noch nicht kontrolliert
        /// </summary>
        private bool FahrstrasseAktiv { get; set; }

        //Index Welche weiche gerade Geschaltet wird
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


    public class FahrstrassenListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Fahrstrasse> Liste;
        public FahrstrassenListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Fahrstrasse>();
        }
        public FahrstrassenListe(string Dateiname, WeichenListe weichenListe, SignalListe signalListe)
        {
            DateiImportieren(Dateiname, weichenListe, signalListe);
        }
        public Fahrstrasse GetFahrstrasse(string Abschnitt)
        {
            if (Verzeichnis.TryGetValue(Abschnitt, out int ListID))
            {
                return Liste[ListID];
            }
            return null;
        }
        public List<Fahrstrasse> GetFahrstrasse(string[] Abschnitte)
        {
            List<Fahrstrasse> fahrstrassen = new List<Fahrstrasse>();
            foreach(string name in Abschnitte)
            {
                Fahrstrasse fahrstrasse = GetFahrstrasse(name);
                if (fahrstrasse == null)
                {
                    //fahrstrassen.Add(fahrstrasse);
                    fahrstrasse = new Fahrstrasse();
                }
                else
                {
                    fahrstrassen.Add(fahrstrasse);
                }
            }
            return fahrstrassen;
        }
        public bool FahrstrasseBlockiert(string Abschnitt)
        {
            Fahrstrasse fahrstrasse = GetFahrstrasse(Abschnitt);
            if (fahrstrasse != null)
            {
                foreach (string Strasse in fahrstrasse.Fahrstr_Blockierende)
                {
                    Fahrstrasse blockiernede = GetFahrstrasse(Strasse);
                    if(blockiernede == null)
                    {
                        return true; //Fahrstrasse nicht gefunden
                    }
                    else
                    {
                        if(blockiernede.GetGesetztStatus())
                        {
                            return true; //Blockierende Fahrstrasse aktiv
                        }
                    }
                }
                return false; //Keine der blockierenden Fahrstrassen aktiv
            }
            return true; //Fahrstrasse nicht gefunden
        }
        public bool FahrstrasseGleicheGesetzt(string Abschnitt)
        {
            Fahrstrasse fahrstrasse = GetFahrstrasse(Abschnitt);
            if (fahrstrasse != null)
            {
                foreach (string Strasse in fahrstrasse.Fahrstr_GleicherEingang)
                {
                    Fahrstrasse Gleich = GetFahrstrasse(Strasse);
                    if (Gleich == null)
                    {
                        return false; //Fahrstrasse nicht gefunden
                    }
                    else
                    {
                        if (Gleich.GetGesetztStatus())
                        {
                            return true; //Blockierende Fahrstrasse aktiv
                        }
                    }
                }
                return false; //Keine der blockierenden Fahrstrassen aktiv
            }
            return false; //Fahrstrasse nicht gefunden
        }
        public void DateiImportieren(string Dateiname, WeichenListe weichenListe, SignalListe signalListe)
        {
            Liste = new List<Fahrstrasse>();
            Verzeichnis = new Dictionary<string, int>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen


            var list = XMLFile.Elements("Fahrstrasse").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach (XElement fahrstrasse in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                FahrstrassenKonfig Konfiguration = new FahrstrassenKonfig()
                {
                    Name = fahrstrasse.Element("Name").Value,
                    EinfahrtsSignal = fahrstrasse.Element("Einfahrtssignal").Value,
                    EndSignal = fahrstrasse.Element("Endsignal").Value
                };
                
                var FahrstrassenWeichenListe = fahrstrasse.Element("Weichen").Elements("WeichenConfig").ToList();

                foreach (XElement wKonfig in FahrstrassenWeichenListe)
                {
                    WeichenKonfig weiche = new WeichenKonfig(
                        wKonfig.Element("Name").Value,
                        wKonfig.Element("Abzweig").Value == "true",
                        wKonfig.Element("vonZunge").Value == "true");
                    Konfiguration.WeichenConfig.Add(weiche);
                }

                var BlockierendeFahrstrassen = fahrstrasse.Element("BlockierendeFahrstrassen").Elements("Fahrstrasse").ToList();
                foreach (XElement block in BlockierendeFahrstrassen)
                {
                    Konfiguration.Fahrstr_Blockierende.Add(block.Value);
                }
                var GleicheFahrstrassen = fahrstrasse.Element("GleicheEinfahrt").Elements("Fahrstrasse").ToList();
                foreach (XElement gleiche in GleicheFahrstrassen)
                {
                    Konfiguration.Fahrstr_GleicherEingang.Add(gleiche.Value);
                }

                Liste.Add(new Fahrstrasse(Konfiguration, weichenListe, signalListe));  //Neue Fahrstrasse mit diesen Parametern hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        public List<Fahrstrasse> GetWithEingangsSignal(Signal signal)
        {
            return Liste.FindAll(x => x.EinfahrtsSignal.Equals(signal));
        }
    }

    

    public class FahrstrassenKonfig
    {
        public FahrstrassenKonfig()
        {
            Fahrstr_Blockierende = new List<string>();
            Fahrstr_GleicherEingang = new List<string>();
        }
        public string Name { get; set; }
        public string EinfahrtsSignal{ get; set; }
        public string EndSignal { get; set; }

        public List<WeichenKonfig> WeichenConfig = new System.Collections.Generic.List<WeichenKonfig>();
        public List<string> Fahrstr_Blockierende { get; set; }
        public List<string> Fahrstr_GleicherEingang { get; set; }
    }

    public struct WeichenKonfig
    {
        public WeichenKonfig(string name, bool abzweig, bool vonZunge)
        {
            Name = name;
            Abzweig = abzweig;
            RichtungVonZunge = vonZunge;            
        }
        public string Name { get; }
        public bool Abzweig { get; }
        public bool RichtungVonZunge { get; }
        
    }
}
