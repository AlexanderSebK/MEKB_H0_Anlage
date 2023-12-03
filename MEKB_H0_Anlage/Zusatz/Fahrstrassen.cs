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
            Fahrstr_Belegtmelder = new List<string>();
        }

        public Fahrstrasse(FahrstrassenKonfig config, WeichenListe weichenListe, SignalListe signalListe)
        {
            Name = config.Name;
            Fahrstr_Weichenliste = new List<Weiche>();
            WeichenKonfigSollstellung = new Dictionary<string, bool>();
            WeichenKonfigRichtung = new Dictionary<string, bool>();
            ControlSetPointer = 0;
            SetPointer = 0;

            Fahrstr_Blockierende = config.Fahrstr_Blockierende;
            Fahrstr_GleicherEingang = config.Fahrstr_GleicherEingang;
            Fahrstr_Belegtmelder = config.Fahrstr_Belegtmelder;

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
                weiche = weichenListe.GetWeiche(weichenConfig.Name);
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

                WeichenKonfigSollstellung.Add(weiche.Name, weichenConfig.Abzweig);
                WeichenKonfigRichtung.Add(weiche.Name, weichenConfig.RichtungVonZunge);
                Fahrstr_Weichenliste.Add(weiche);
            }
        }

        public string Name { get; set; }

        public List<Weiche> Fahrstr_Weichenliste { get; set; }
        public List<string> Fahrstr_Blockierende { get; set; }
        public List<string> Fahrstr_GleicherEingang { get; set; }
        public List<string> Fahrstr_Belegtmelder { get; set; }

        public Dictionary<string, bool> WeichenKonfigSollstellung { get; set; }
        public Dictionary<string, bool> WeichenKonfigRichtung { get; set; }
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

        private void WeichenSicherheit(bool Sicherheitsstatus)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                weiche.FahrstrasseSicher = Sicherheitsstatus;
            }
        }
        public void StarteFahrstrasse()
        {
            FahrstrasseGesetzt = true;
            Safe = false;
            WeichenSicherheit(Safe);
        }
        public void SetFahrstrasseRichtung()
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                weiche.FahrstrasseRichtung_vonZunge = this.WeichenKonfigRichtung[weiche.Name]; // weiche.FahrstrasseRichtung_vonZunge;
            }
        }
        public void SetFahrstrasse(Z21 Z21_Instanz)
        {
            if (SetPointer >= Fahrstr_Weichenliste.Count) SetPointer = 0;
            Weiche weiche = Fahrstr_Weichenliste[SetPointer];

            bool sollstellung = WeichenKonfigSollstellung[weiche.Name];
            bool richtung = WeichenKonfigRichtung[weiche.Name];

            if (weiche.Status_Unbekannt || weiche.Status_Error)
            {
                MessageBox.Show("Weiche reagiert nicht. Verbindung von Z21 prüfen", "Weiche unbekannt oder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FahrstrasseGesetzt = false;
                FahrstrasseAktiv = false;
                return;
            }

            int Adresse = weiche.Adresse;
            weiche.FahrstrasseRichtung_vonZunge = richtung; // weiche.FahrstrasseRichtung_vonZunge;
            weiche.FahrstrasseAbzweig = sollstellung; // weiche.FahrstrasseAbzweig;

            if (weiche.Abzweig != sollstellung)   //Wenn Weiche noch nicht in Position ist
            {
                weiche.SetzeWeiche(sollstellung, true);
            }
            SetPointer++; // Nächste Weiche
        }
        public void ControlSetFahrstrasse(Z21 Z21_Instanz)
        {
            if (Fahrstr_Weichenliste.Count == 0) //Weichenloser Block
            {
                Safe = true;
                WeichenSicherheit(Safe);
                return;
            }
            if (!Safe)
            {
                if (ControlSetPointer < Fahrstr_Weichenliste.Count)
                {
                    Weiche weiche = Fahrstr_Weichenliste[ControlSetPointer];
                    bool sollstellung = WeichenKonfigSollstellung[weiche.Name];
                    weiche.SetzeWeiche(sollstellung, true);
                    ControlSetPointer++;
                }
                else
                {
                    if (GetBusyStatus() == false)
                    {
                        ControlSetPointer = 0;
                        Safe = true;
                        WeichenSicherheit(Safe);
                    }
                }
            }
        }
        public void AktiviereFahrstasse()
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                weiche.FahrstrasseAktive = true;
            }
            FahrstrasseAktiv = true;
        }
        public bool GetBusyStatus()
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                if (weiche.AmBewegen) return true; //Eine Weiche noch beim Schalten?
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
        public bool CheckFahrstrassePos()
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                if (weiche.Abzweig != WeichenKonfigSollstellung[weiche.Name])   //Wenn Weiche noch nicht in Position ist
                {
                    return false;
                }
            }
            return true;
        }
        public bool IstFahrstrasseBelegt(List<Belegtmelder> ListeBelegtmelder)
        {
            if (ListeBelegtmelder == null) return true;
            foreach(string Belegtmelder in Fahrstr_Belegtmelder)
            {
                int ListID = ListeBelegtmelder.IndexOf(new Belegtmelder() { Name = Belegtmelder });  //Weiche in Globale Liste suchen
                if (ListID == -1) return true;
                if (ListeBelegtmelder[ListID].IstBelegt()) return true;
            }
            return false;
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
            WeichenSicherheit(Safe);
        }

    }


    public class FahrstrassenListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Fahrstrasse> Liste;

        public Dictionary<string, bool> GesperrteFahrstarssen;

        public FahrstrassenListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Fahrstrasse>();
            GesperrteFahrstarssen = new Dictionary<string, bool>();
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
                if (fahrstrasse != null)
                {
                    fahrstrassen.Add(fahrstrasse);
                }
            }
            return fahrstrassen;
        }
        public bool FahrstrasseBlockiert(string Abschnitt)
        {
            Fahrstrasse fahrstrasse = GetFahrstrasse(Abschnitt);
            if (GesperrteFahrstarssen[fahrstrasse.Name]) return true; // Fahrstrasse ist gesperrt
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
        public bool FahrstrasseAlleGleicheBlockiert(Fahrstrasse fahrstrasse)
        {
            if (!this.FahrstrasseBlockiert(fahrstrasse.Name)) return false; //Selber nicht blockiert?
            foreach (string Strasse in fahrstrasse.Fahrstr_GleicherEingang)
            {
                Fahrstrasse Gleich = GetFahrstrasse(Strasse);
                if (Gleich != null)              
                {
                    if (!this.FahrstrasseBlockiert(Gleich.Name))
                    {
                        return false; //Eine der Fahrstrassen ist frei
                    }
                }
            }
            return true;
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
            GesperrteFahrstarssen = new Dictionary<string, bool>();
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
                Konfiguration.Fahrstr_Belegtmelder = new List<string>();
                XElement XML_FahrstrassenBelegtmelder = fahrstrasse.Element("BelegtmelderBlock");
               
                if(XML_FahrstrassenBelegtmelder != null)
                {
                    var FahrstrassenBelegtmelder = XML_FahrstrassenBelegtmelder.Elements("Belegtmelder").ToList();
                    foreach(XElement Belegtmelder in FahrstrassenBelegtmelder)
                    {
                        Konfiguration.Fahrstr_Belegtmelder.Add(Belegtmelder.Value);
                    }
                }


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
                GesperrteFahrstarssen.Add(Konfiguration.Name, false);
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
            Fahrstr_Belegtmelder = new List<string>();  
        }
        public string Name { get; set; }
        public string EinfahrtsSignal{ get; set; }
        public string EndSignal { get; set; }

        public List<WeichenKonfig> WeichenConfig = new System.Collections.Generic.List<WeichenKonfig>();
        public List<string> Fahrstr_Blockierende { get; set; }
        public List<string> Fahrstr_GleicherEingang { get; set; }
        public List<string> Fahrstr_Belegtmelder { get; set; }
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
