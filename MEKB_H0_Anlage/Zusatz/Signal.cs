using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                string STyp = XMLSignal.Element("Typ").Value;                       //Typ des Signals auslesen

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
                    Typ = STyp,
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
        /// Suche Signal mit der gegebenen ersten Adresse des Signals
        /// </summary>
        /// <param name="Adresse">Erste Adresse des Signals</param>
        /// <returns>Signal, oder null wenn nicht gefunden</returns>
        public Signal GetSignalErsteAdresse(int Adresse)
        {
            int ListID = Liste.FindIndex(x => x.Adresse == Adresse);
            if (ListID != -1)
            {
                return Liste[ListID];
            }
            return null;
        }
        /// <summary>
        /// Suche Signal mit der gegebenen zweiten Adresse des Signals
        /// </summary>
        /// <param name="Adresse">Zweite Adresse des Signals</param>
        /// <returns>Signal, oder null wenn nicht gefunden</returns>
        public Signal GetSignalZweiteAdresse(int Adresse)
        {
            int ListID = Liste.FindIndex(x => x.Adresse2 == Adresse);
            if (ListID != -1)
            {
                return Liste[ListID];
            }
            return null;
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
        public String Name { get; set; }
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
        /// Typ des Signals
        /// 3HP_270 -> Drei Schaltbilder Signal um 270° auf dem Gleisplan gedreht 
        /// </summary>
        public string Typ { get; set; }
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Signal()
        {

        }


        public XElement SignalZuXML()
        {
            return new XElement("Signal",
                new XElement("Name", Name),
                new XElement("Adresse", Adresse),
                new XElement("Adresse2", Adresse2),
                new XElement("Typ", Typ),
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
        /// <summary>
        /// Empfangenen Zustand der Pins der Dekoder auf das Schaltbild des Signals abilden
        /// </summary>
        /// <param name="new_zustand">Schaltzustand von Z21 empfangen (Adresse X Ausgang Y)</param>
        public void MaskenSetzen(int new_zustand)
        {
            if (new_zustand == 0) Zustand = SignalZustand.Unbestimmt;
            else if (new_zustand == 1) Zustand = Adr1_1; //Adresse 1 ist auf Ausgang 1 gesetzt? -> Signalbild für diese Kombination übernehmen
            else if (new_zustand == 2) Zustand = Adr1_2; //Adresse 1 ist auf Ausgang 2 gesetzt? -> Signalbild für diese Kombination übernehmen
            else if (new_zustand == 5) Zustand = Adr2_1; //Adresse 2 ist auf Ausgang 1 gesetzt? -> Signalbild für diese Kombination übernehmen
            else if (new_zustand == 6) Zustand = Adr2_2; //Adresse 2 ist auf Ausgang 2 gesetzt? -> Signalbild für diese Kombination übernehmen
            else Zustand = SignalZustand.HP0;
        }
        /// <summary>
        /// Signal schalten
        /// </summary>
        /// <param name="NeuerZustand">Neues Signalbild</param>
        /// <param name="z21">Instance der Z21</param>
        public void Schalten(SignalZustand NeuerZustand, Z21 z21)
        {
            if(Adr1_1 == NeuerZustand || Adr1_2 == NeuerZustand || Adr2_1 == NeuerZustand || Adr2_2 == NeuerZustand)
            {

            }
            else
            {
                //Nur bei HP2: HP1 erlauben
                if (NeuerZustand == SignalZustand.HP2) NeuerZustand = SignalZustand.HP1;
            }

            if (NeuerZustand == Adr1_1) { z21.LAN_X_SET_SIGNAL(Adresse, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (NeuerZustand == Adr1_2) { z21.LAN_X_SET_SIGNAL(Adresse, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (NeuerZustand == Adr2_1) { z21.LAN_X_SET_SIGNAL(Adresse2, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
            else if (NeuerZustand == Adr2_2) { z21.LAN_X_SET_SIGNAL(Adresse2, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
        }

        //Ermittle Anhand der Weichenstellung und Meldungen ob der Block Frei ist
        public SignalZustand ErlaubteStellung(FahrstrassenListe fahrstrassenListe, WeichenListe weichenListe)
        {
            //Finde Fahrstrassen mit diesem Signal als Eingangssignal hat
            List<Fahrstrasse> Strassen = fahrstrassenListe.GetWithEingangsSignal(this);
            foreach(Fahrstrasse s in Strassen)
            {
                //Wenn Option aktiviert ist, dass nur auf Fahrstrassen geschaltet werden soll
                if (Config.ReadConfig("AutoSignalFahrstrasse").Equals("true"))
                {
                    //Fahrtstrasse nicht gesichert -> nächste Fahrstrasse
                    if (!s.Safe) continue; 
                }
                // Weg ist gesetzt
                if(s.CheckFahrstrassePos(weichenListe.Liste))
                {
                    foreach(string blockStr in s.Fahrstr_Blockierende)
                    {
                        //Eine der Blockierenden Fahrstrassen ist nicht Halt
                        Fahrstrasse BlockFahrstr = fahrstrassenListe.GetFahrstrasse(blockStr);
                        if(BlockFahrstr != null)
                        {
                            // Ist Fahrstrasse der Blockierenden fahrbar (Kreuzung)
                            if(BlockFahrstr.CheckFahrstrassePos(weichenListe.Liste))
                            {
                                //Ist nicht auf Halt
                                if(!BlockFahrstr.EinfahrtsSignal.Zustand.Equals(SignalZustand.HP0))
                                {
                                    return SignalZustand.HP0; //Blockiert
                                }
                            }
                        }
                    }
                    /*Prüfe ob Fahrzeug auf Block ist*/


                    foreach(Weiche w in s.Fahrstr_Weichenliste)
                    {
                        // Über eine Weiche muss über Abzweig gefahren werden -> Langsame Fahrt
                        if(w.Abzweig) return SignalZustand.HP2;
                    }
                    //Nächstes Signal auf Halt
                    if (s.EndSignal.Zustand == SignalZustand.HP0) return SignalZustand.HP2;
                    return SignalZustand.HP1; // Freie Fahrt
                }
            }
            return SignalZustand.HP0;
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
