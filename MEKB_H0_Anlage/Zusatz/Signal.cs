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
            var list = XMLFile.Elements("Signal").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach (XElement XMLSignal in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                int SAdresse = Int16.Parse(XMLSignal.Element("Adresse").Value);                               //Signaladresse des Elements auslesen
                int SAdresse2;
                if (XMLSignal.Element("Adresse2") == null) SAdresse2 = 0;//Nicht vorhanden - 2.Adresse 0 eintragen
                else SAdresse2 = Int16.Parse(XMLSignal.Element("Adresse2").Value);//2. Signaladresse des Elements auslesen

                string SName = XMLSignal.Element("Name").Value;                                                //Signal Name des Elements auslesen
                string STyp = XMLSignal.Element("Typ").Value;                               //Typ des Signals auslesen
                int SAdr11 = Int16.Parse(XMLSignal.Element("Adr1Zustand1").Value);          //Zustand bei Signaladresse Schaltung auf 0
                int SAdr12 = Int16.Parse(XMLSignal.Element("Adr1Zustand2").Value);          //Zustand bei Signaladresse Schaltung auf 1
                int SAdr21 = Int16.Parse(XMLSignal.Element("Adr2Zustand1").Value);          //Zustand bei 2. Signaladresse Schaltung auf 0
                int SAdr22 = Int16.Parse(XMLSignal.Element("Adr2Zustand2").Value);          //Zustand bei 2. Signaladresse Schaltung auf 1

                Signal signal = new Signal()
                {
                    Name = SName,
                    Adresse = SAdresse,
                    Adresse2 = SAdresse2,
                    Typ = STyp
                };  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
                if (Enum.IsDefined(typeof(SignalZustand), SAdr11)) { signal.Adr1_1 = (SignalZustand)SAdr11; }
                else { signal.Adr1_1 = SignalZustand.Unbestimmt; }
                if (Enum.IsDefined(typeof(SignalZustand), SAdr12)) { signal.Adr1_2 = (SignalZustand)SAdr12; }
                else { signal.Adr1_2 = SignalZustand.Unbestimmt; }
                if (Enum.IsDefined(typeof(SignalZustand), SAdr21)) { signal.Adr2_1 = (SignalZustand)SAdr21; }
                else { signal.Adr2_1 = SignalZustand.Unbestimmt; }
                if (Enum.IsDefined(typeof(SignalZustand), SAdr22)) { signal.Adr2_2 = (SignalZustand)SAdr22; }
                else { signal.Adr2_2 = SignalZustand.Unbestimmt; }

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
        /// Wird bei Listensuche benötigt: Name der Weiche zurückgeben
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
            if (NeuerZustand == Adr1_1) { z21.LAN_X_SET_SIGNAL(Adresse, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (NeuerZustand == Adr1_2) { z21.LAN_X_SET_SIGNAL(Adresse, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (NeuerZustand == Adr2_1) { z21.LAN_X_SET_SIGNAL(Adresse2, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
            else if (NeuerZustand == Adr2_2) { z21.LAN_X_SET_SIGNAL(Adresse2, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
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
