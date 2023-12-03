using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Signalliste mit Verwaltung
    /// </summary>
    public class SignalListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Signal> Liste;

        #region Konstruktoren
        /// <summary>
        /// Konstruktor
        /// </summary>
        public SignalListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
        }
        /// <summary>
        /// Konstruktor mit XML-Importer
        /// </summary>
        /// <param name="Dateiname">Dateiname für Signalliste</param>
        public SignalListe(string Dateiname)
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
            DateiImportieren(Dateiname);
        }
        #endregion
        /// <summary>
        /// Signalliste via XML-Datei importieren
        /// </summary>
        /// <param name="Dateiname"></param>
        public void DateiImportieren(string Dateiname)
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen
            var list = XMLFile.Elements("Signal").ToList();    //Alle Elemente des Types Signal in eine Liste Umwandeln 

            foreach (XElement XMLSignal in list)               //Alle Elemente der Liste einzeln durchlaufen
            {
                int SAdresse = Int16.Parse(XMLSignal.Element("Adresse").Value);     //Signaladresse des Elements auslesen
                int SAdresse2;
                if (XMLSignal.Element("Adresse2") == null) SAdresse2 = 0;           //Nicht vorhanden - 2.Adresse 0 eintragen
                else SAdresse2 = Int16.Parse(XMLSignal.Element("Adresse2").Value);  //2. Signaladresse des Elements auslesen

                string SName = XMLSignal.Element("Name").Value;                     //Signal Name des Elements auslesen
                
                if (Enum.TryParse<SignalZustand>(XMLSignal.Element("Adr1Zustand1").Value, out SignalZustand signalZustand1_1)) { }
                else signalZustand1_1 = SignalZustand.Unbestimmt;
                if (Enum.TryParse<SignalZustand>(XMLSignal.Element("Adr1Zustand2").Value, out SignalZustand signalZustand1_2)) { }
                else signalZustand1_2 = SignalZustand.Unbestimmt;
                if (Enum.TryParse<SignalZustand>(XMLSignal.Element("Adr2Zustand1").Value, out SignalZustand signalZustand2_1)) { if (SAdresse2 == 0) signalZustand2_1 = SignalZustand.Unbestimmt; }
                else signalZustand2_1 = SignalZustand.Unbestimmt;
                if (Enum.TryParse<SignalZustand>(XMLSignal.Element("Adr2Zustand2").Value, out SignalZustand signalZustand2_2)) { if (SAdresse2 == 0) signalZustand2_2 = SignalZustand.Unbestimmt; }
                else signalZustand2_2 = SignalZustand.Unbestimmt;

                Signal signal = new Signal()
                {
                    Name = SName,
                    Adresse = SAdresse,
                    Adresse2 = SAdresse2,
                    Adr1_1 = signalZustand1_1,
                    Adr1_2 = signalZustand1_2,
                    Adr2_1 = signalZustand2_1,
                    Adr2_2 = signalZustand2_2
                };  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen

               
                Liste.Add(signal);
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        /// <summary>
        /// Signal aus der Liste suchen
        /// </summary>
        /// <param name="Signalname">Name des Signals</param>
        /// <returns>Signal, oder null wenn nicht gefunden</returns>
        public Signal GetSignal(string Signalname)
        {
            if (Verzeichnis.TryGetValue(Signalname, out int ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

        /// <summary>
        /// Signal aus der Liste suchen
        /// </summary>
        /// <param name="Signalname">Name des Signals</param>
        /// <returns>Signal, oder null wenn nicht gefunden</returns>
        public Signal GetSignal(int Adresse)
        {
            int ListID = Liste.FindIndex(x => x.Adresse == Adresse);
            if (ListID != -1)
            {
                return Liste[ListID];
            }
            ListID = Liste.FindIndex(x => x.Adresse2 == Adresse);
            if (ListID != -1)
            {
                return Liste[ListID];
            }
            return null;
        }

        /// <summary>
        /// Zustandupdate bei Empfang von Z21 Weichendaten
        /// </summary>
        /// <param name="Adresse">Adresse des Signals</param>
        /// <param name="Schaltzustand">Schaltzustand (Z21-Protokoll)</param>
        /// <returns></returns>
        public bool UpdateSignalZustand(int Adresse, int Schaltzustand)
        {
            // Suche als 1. Adresse
            int ListID = Liste.FindIndex(x => x.Adresse == Adresse);
            if(ListID != -1) //Gefunden
            {
                // Signal gefunden, aber letzter SIgnalzugriff war über Adresse2 -> Keine Änderung
                if (Liste[ListID].Letzte_Adresswahl == true) return true;

                if (Schaltzustand == 1)
                {
                    Liste[ListID].Zustand = Liste[ListID].Adr1_1;
                    return true;
                }
                else if (Schaltzustand == 2)
                {
                    Liste[ListID].Zustand = Liste[ListID].Adr1_2;
                    return true;
                }
                else
                {
                    Liste[ListID].Zustand = SignalZustand.Unbestimmt;
                    return true;
                }
            }
            else
            {
                ListID = Liste.FindIndex(x => x.Adresse2 == Adresse);
                if (ListID != -1) //Gefunden
                {
                    // Signal gefunden, aber letzter Signalzugriff war über Adresse1 -> Keine Änderung
                    if (Liste[ListID].Letzte_Adresswahl == false) return true;

                    if (Schaltzustand == 1)
                    {
                        Liste[ListID].Zustand = Liste[ListID].Adr2_1;
                        return true;
                    }
                    else if (Schaltzustand == 2)
                    {
                        Liste[ListID].Zustand = Liste[ListID].Adr2_2;
                        return true;
                    }
                    else
                    {
                        Liste[ListID].Zustand = SignalZustand.Unbestimmt;
                        return true;
                    }
                }
            }
            return false;
        }

       

        /// <summary>
        /// Verknüpfe Z21 Instanzen
        /// </summary>
        /// <param name="zentrale">Instanz der Z21 mit der die Signale in der Liste geschalten werden</param>
        public void DigitalzentraleZugriff(Z21 zentrale)
        {
            foreach(Signal signal in Liste)
            {
                signal.DigitalzentraleZugriff(zentrale);
            }
        }

        /// <summary>
        /// Verknüpfe globale Fahrstrassen/Routen-Liste
        /// </summary>
        /// <param name="fahrstrassenListe">Gloable Fahrstrassenliste</param>
        public void ListenZugriff(FahrstrassenListe fahrstrassenListe, BelegtmelderListe belegtmelderListe)
        {
            foreach(Signal signal in Liste)
            {
                // Übernehme nur Fahrstrassen, die dieses Signal als Anfang haben
                signal.Fahrstrassen = fahrstrassenListe.GetWithEingangsSignal(signal);
                signal.BelegtmelderVerzeichnis = new Dictionary<string, List<Belegtmelder>>();
                foreach(Fahrstrasse Strasse in signal.Fahrstrassen)
                {
                    List<Belegtmelder> melderListe = new List<Belegtmelder>();
                    foreach(string Melder in Strasse.Fahrstr_Belegtmelder)
                    {
                        Belegtmelder belegtmelder = belegtmelderListe.GetBelegtmelder(Melder);
                        if(belegtmelder != null)
                        {
                            melderListe.Add(belegtmelder);
                        }
                    }
                    signal.BelegtmelderVerzeichnis.Add(Strasse.Name,melderListe);
                }
            }
        }

        public void AutoSignal(bool AutoHP1, bool AchteFahrstrassen)
        {
            foreach(Signal signal in Liste)
            {
                signal.AutoSignal(AutoHP1,AchteFahrstrassen);
            }
        }
    }
   
    /// <summary>
    /// Signal Klasse. 
    /// Beinhaltet Schaltfunktionen, Zustandskontrolle, Signalbildzuweisung, etc.
    /// </summary>
    public class Signal : IEquatable<Signal>
    {
        /// <summary>
        /// Parameter: Name des Signals als String
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Parameter: Adresse des Signals für HP0/1
        /// </summary>
        public int Adresse { get; set; }
        /// <summary>
        /// Parameter: 2.Adresse des Signals für HP2/3
        /// </summary>
        public int Adresse2 { get; set; }
        /// <summary>
        /// Lezte angewählte Adresse (true = 2. Adresse)
        /// </summary>
        public bool Letzte_Adresswahl { get; set; }
        /// <summary>
        /// Letzter geschalteter Ausgang
        /// </summary>
        public bool Letzter_Ausgang { get; set; }
        /// <summary>
        /// Zustand: HPx des Signals 
        /// </summary>
        public SignalZustand Zustand { get; set; }
        /// <summary>
        /// Signalzustand wenn Pin1 von Adresse 1 aktiviert (0 = HP0, 1 = HP1, 2 = HP2, 3 = SH1)
        /// </summary>
        public SignalZustand Adr1_1 { get; set; }
        /// <summary>
        /// Signalzustand wenn Pin2 von Adresse 1 aktiviert (0 = HP0, 1 = HP1, 2 = HP2, 3 = SH1)
        /// </summary>
        public SignalZustand Adr1_2 { get; set; }
        /// <summary>
        /// Signalzustand wenn Pin1 von Adresse 2 aktiviert (0 = HP0, 1 = HP1, 2 = HP2, 3 = SH1)
        /// </summary>
        public SignalZustand Adr2_1 { get; set; }
        /// <summary>
        /// Signalzustand wenn Pin2 von Adresse 2 aktiviert (0 = HP0, 1 = HP1, 2 = HP2, 3 = SH1)
        /// </summary>
        public SignalZustand Adr2_2 { get; set; } 
        
        /// <summary>
        /// True: Signal soll nicht automatisch auf grün schalten
        /// </summary>
        public bool AutoSperre { get; set; }
        
        private Timer CooldownTimer { get; set; }

        private Z21 Z21_zentrale { get; set; }

        public List<Fahrstrasse> Fahrstrassen { get; set; }

        public Dictionary<string, List<Belegtmelder>> BelegtmelderVerzeichnis { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Signal()
        {
            Fahrstrassen = new List<Fahrstrasse>();
            Letzte_Adresswahl = false;
            Z21_zentrale = new Z21();
            AutoSperre = false;
            Zustand = SignalZustand.Unbestimmt;
        }


        public XElement SignalZuXML()
        {
            return new XElement("Signal",
                new XElement("Name", Name),
                new XElement("Adresse", Adresse),
                new XElement("Adresse2", Adresse2),
                new XElement("Adr1Zustand1", Adr1_1),
                new XElement("Adr1Zustand2", Adr1_2),
                new XElement("Adr2Zustand1", Adr2_1),
                new XElement("Adr2Zustand2", Adr2_2));        
        }

        
        /// <summary>
        /// Wird bei Listensuche benötigt: Name des Signals zurückgeben
        /// </summary>
        /// <returns>Name der Weiche</returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Adresse der Weiche
        /// </summary>
        /// <returns>Adresse der Weiche</returns>
        public override int GetHashCode()
        {
            return Adresse;
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Weichen vergleichen
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Signal objAsPart)) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Unterfunktion Weichen vergleichen
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Signal other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }

        public void DigitalzentraleZugriff(Z21 zentrale)
        {
            Z21_zentrale = zentrale;
        }

        /// <summary>
        /// Signal schalten
        /// </summary>
        /// <param name="NeuerZustand">Neues Signalbild</param>
        /// <param name="z21">Instance der Z21</param>
        public void Schalten(SignalZustand NeuerZustand)
        {
            if (Zustand == NeuerZustand) return;    //Keine Änderung nötig

            if (Adr1_1 == NeuerZustand) {      ZustandSetzen(Adresse, false); Letzter_Ausgang = false; Letzte_Adresswahl = false; }
            else if (Adr1_2 == NeuerZustand) { ZustandSetzen(Adresse, true); Letzter_Ausgang = true; Letzte_Adresswahl = false; }
            else if (Adr2_1 == NeuerZustand) { ZustandSetzen(Adresse2, false); Letzter_Ausgang = false; Letzte_Adresswahl = true; }
            else if (Adr2_2 == NeuerZustand) { ZustandSetzen(Adresse2, true); Letzter_Ausgang = true; Letzte_Adresswahl = true; }
            else
            {
                //Signal kennt diesen Zustand nicht -> Prüfen ob HP2 zu HP1 umgewandelt werden kann, sonst Befehl ignorieren
                if (NeuerZustand == SignalZustand.HP2)
                {
                    if (Adr1_1 == SignalZustand.HP1) { ZustandSetzen(Adresse, false); Letzter_Ausgang = false; Letzte_Adresswahl = false; }
                    else if (Adr1_2 == SignalZustand.HP1) { ZustandSetzen(Adresse, true); Letzter_Ausgang = true; Letzte_Adresswahl = false; }
                    else if (Adr2_1 == SignalZustand.HP1) { ZustandSetzen(Adresse2, false); Letzter_Ausgang = false; Letzte_Adresswahl = true; }
                    else if (Adr2_2 == SignalZustand.HP1) { ZustandSetzen(Adresse2, true); Letzter_Ausgang = true; Letzte_Adresswahl = true; }
                    else
                    {
                        // Signal kennt kann kein HP1 oder HP2 (Rangiersignale)
                    }
                }
                else
                {
                    // Signal kennt diesen Zustand nicht
                }
            }
        }

        private void ZustandSetzen(int Adresse, bool Ausgang)
        {
            Z21_zentrale.LAN_X_SET_SIGNAL(Adresse, Ausgang);
            CooldownTimer = new Timer(AusgangAuschalten, null, 100, System.Threading.Timeout.Infinite);
        }


        private void AusgangAuschalten(Object o)
        {
            int addr = Adresse;
            if (Letzte_Adresswahl) addr = Adresse2;

            Z21_zentrale.LAN_X_SET_SIGNAL_OFF(addr, Letzter_Ausgang);
        }
        /// <summary>
        /// Überprüft ob diese Stellung erlaubt ist
        /// </summary>
        /// <param name="wunschZustand">Gewünschter Signalzusatnd</param>
        /// <param name="FahrstrassenModus">False: Signal achtet nur auf Weichenstellung | True: Signal achtet ob Fahrstrasse gesetzt ist</param>
        /// <returns>true: Signalstellung erlaubt</returns>
        public bool StellungErlaubt(SignalZustand wunschZustand, bool FahrstrassenModus = false)
        {
            if (wunschZustand == SignalZustand.HP0) return true;  // HP0 (Halt) immer erlaubt

            if (wunschZustand == SignalZustand.HP2)
            {
                if (!HatSignalbild(SignalZustand.HP2) && HatSignalbild(SignalZustand.HP1))
                {
                    // Sonderfall HP2 verlangt, Signal kann aber nur HP1 -> Funktion nicht abbrechen
                }
                else
                {
                    if (!HatSignalbild(wunschZustand)) return false; //Kein HP1 oder HP2 -> Rangiersignal
                }
            }
            else
            {
                if (!HatSignalbild(wunschZustand)) return false;
            }

            bool WeichenGesetzt = false; // Alle Weichen sind gestellt, dass Fahrt erteilt werden kann
            bool WeicheAbzweig = false; // Eine der Weichen ist auf Abzweig, so dass nur langsame Zeit erlaubt ist
            bool BlockBelegt = false; // Block ist belegt;

            //Finde Fahrstrassen mit diesem Signal als Eingangssignal hat
            foreach (Fahrstrasse Strasse in Fahrstrassen)
            {
                if(FahrstrassenModus) // Automatic Modus prüfen
                {
                    if (Strasse.Safe) WeichenGesetzt = true; // Gesicherte Fahrstrasse vorhanden -> Fahrt erlauben
                }
                else
                {
                    if (Strasse.CheckFahrstrassePos()) WeichenGesetzt = true; // Kontrolliere ob alle Weichen dieser Route gesetzt sind
                }

                if (WeichenGesetzt) //Route passt
                {
                    // Pürfen ob weiche auf Abzweig
                    foreach (Weiche weiche in Strasse.Fahrstr_Weichenliste)
                    {
                        if (weiche.Abzweig) WeicheAbzweig = true;
                    }

                    foreach(Belegtmelder melder in BelegtmelderVerzeichnis[Strasse.Name])
                    {
                        if (melder.IstBelegt()) BlockBelegt = true;
                    }

                    break; //Abbruch. Keine weitere Pürfungen erforderlich 
                }
            }
            if (WeicheAbzweig && IstBlockSignal()) WeicheAbzweig = false;   //Erlaube auch bei HP1 Abgezweigte Weichen, wenn kein HP1 vorhanden ist

            switch(wunschZustand)
            {
                case SignalZustand.HP1: if (WeichenGesetzt && !WeicheAbzweig && !BlockBelegt) return true; break;
                case SignalZustand.HP2: if (WeichenGesetzt                   && !BlockBelegt) return true; break;
                case SignalZustand.SH1: if (WeichenGesetzt                   && !BlockBelegt) return true; break;
                default: return false;
            }


            return false;
        }

        public bool IstSperrSignal()
        {
            if (HatSignalbild(SignalZustand.SH1) && !HatSignalbild(SignalZustand.HP1) && !HatSignalbild(SignalZustand.HP2)) return true;
            return false;
        }

        public bool IstHP2Verbund()
        {
            if (HatSignalbild(SignalZustand.HP2) && !HatSignalbild(SignalZustand.HP1)) return true;
            return false;
        }

        public bool IstBlockSignal()
        {
            if (HatSignalbild(SignalZustand.HP1) && !HatSignalbild(SignalZustand.HP2)) return true;
            return false;
        }

        /// <summary>
        /// Überprüfen ob Signalbild überhaupt mit diesem Signal möglich ist
        /// </summary>
        /// <param name="signalbild">Gefragte Stellung</param>
        /// <returns>True: Kann annehmen</returns>
        public bool HatSignalbild(SignalZustand signalbild)
        {
            return (signalbild == Adr1_1 || signalbild == Adr1_2 || signalbild == Adr2_1 || signalbild == Adr2_2);
        }

        public void AutoSignal(bool AutoHP1, bool AchteFahrstrassen)
        {
            //Signal durch User gesperrt, nicht auf Grün schalten
            if (AutoSperre) AutoHP1 = false;

            if (IstBlockSignal())
            {
                // Schalte auf HP1 wenn erlaubt und AutoHP1 erlaubt
                if (StellungErlaubt(SignalZustand.HP1, AchteFahrstrassen))
                {
                    if (AutoHP1 && Zustand != SignalZustand.HP1) Schalten(SignalZustand.HP1);
                }
                else // HP1 ist nicht erlaubt: Auf HP0 schalten
                {
                    if (Zustand != SignalZustand.HP0) Schalten(SignalZustand.HP0);
                }
            }
            else if (IstHP2Verbund())
            {
                // Schalte auf HP2 wenn erlaubt und AutoHP1 erlaubt
                if (StellungErlaubt(SignalZustand.HP2, AchteFahrstrassen))
                {
                    if (AutoHP1 && Zustand == SignalZustand.HP0) Schalten(SignalZustand.HP2);
                }
                else // HP2 ist nicht erlaubt: Auf HP0 schalten
                {
                    if (Zustand != SignalZustand.HP0) Schalten(SignalZustand.HP0);
                }
            }
            else if (IstSperrSignal())
            {
                // Sperrsignale nicht schalten
            }
            else
            {
                bool HP2_Erlaubt = StellungErlaubt(SignalZustand.HP2, AchteFahrstrassen);
                bool HP1_Erlaubt =  StellungErlaubt(SignalZustand.HP1, AchteFahrstrassen);
                if (HP1_Erlaubt && HP2_Erlaubt)
                {
                    foreach (Fahrstrasse fahrstrasse in Fahrstrassen)
                    {
                        if (fahrstrasse.CheckFahrstrassePos())
                        {
                            if (fahrstrasse.EndSignal.Zustand == SignalZustand.HP0)
                            {
                                // Wenn nächstes Signal auf rot steht -> langsame Fahrt
                                if (AutoHP1 && Zustand != SignalZustand.HP2) Schalten(SignalZustand.HP2);
                            }
                            else
                            {
                                if (AutoHP1 && Zustand != SignalZustand.HP1) Schalten(SignalZustand.HP1);
                            }
                        }
                    }
                }
                else if (HP2_Erlaubt)
                {
                    if (AutoHP1 && Zustand != SignalZustand.HP2) Schalten(SignalZustand.HP2);
                }
                else
                {
                    if (Zustand != SignalZustand.HP0) Schalten(SignalZustand.HP0);
                }
            }
        }
    }



    /// <summary>
    /// Signalbilder als ENUM
    /// </summary>
    public enum SignalZustand
    {
        HP0 = 0,
        HP1 = 1,
        HP2 = 2,
        SH1 = 3,
        Unbestimmt = 9
    };
}
