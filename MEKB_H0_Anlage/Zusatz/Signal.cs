using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public class SignalListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Signal> Liste;

        public SignalListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
        }

        public SignalListe(string Dateiname)
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
            DateiImportieren(Dateiname);
        }

        public void DateiImportieren(string Dateiname)
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Signal>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen
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

                Liste.Add(new Signal()
                {
                    Name = SName,
                    Adresse = SAdresse,
                    Adresse2 = SAdresse2,
                    Typ = STyp,
                    Adr1_1 = SAdr11,
                    Adr1_2 = SAdr12,
                    Adr2_1 = SAdr21,
                    Adr2_2 = SAdr22
                });  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        public Signal GetSignal(string Signalname)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Signalname, out ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

        public Signal GetSignalErsteAdresse(int Adresse)
        {
            int ListID = Liste.FindIndex(x => x.Adresse == Adresse);
            if (ListID != -1)
            {
                return Liste[ListID];
            }
            return null;
        }
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
        public int Zustand { get; set; }
        public int Adr1_1 { get; set; }
        public int Adr1_2 { get; set; }
        public int Adr2_1 { get; set; }
        public int Adr2_2 { get; set; }

        public const int HP0 = 0;
        public const int HP1 = 1;
        public const int HP2 = 2;

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
        /// Weiche setzen
        /// </summary>
        /// <param name="new_zustand">Schaltzustand von Z21 empfangen (Adresse X Ausgang Y)</param>
        public void MaskenSetzen(int new_zustand)
        {
            if (new_zustand == 0) Zustand = 9;
            else if (new_zustand == 1) Zustand = Adr1_1; //Adresse 1 ist auf Ausgang 1 gesetzt? -> Zustand für Ausgang 2 übernehmen
            else if (new_zustand == 2) Zustand = Adr1_2;
            else if (new_zustand == 5) Zustand = Adr2_1;
            else if (new_zustand == 6) Zustand = Adr2_2;
            else Zustand = 0;
        }
        public void Schalten(int HPx, Z21 z21)
        {
            if (HPx == Adr1_1) { z21.LAN_X_SET_SIGNAL(Adresse, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (HPx == Adr1_2) { z21.LAN_X_SET_SIGNAL(Adresse, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse2); Letzte_Adresswahl = false; }
            else if (HPx == Adr2_1) { z21.LAN_X_SET_SIGNAL(Adresse2, false); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
            else if (HPx == Adr2_2) { z21.LAN_X_SET_SIGNAL(Adresse2, true); z21.LAN_X_SET_SIGNAL_OFF(Adresse); Letzte_Adresswahl = true; }
        }
    }
}
