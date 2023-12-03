using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

namespace MEKB_H0_Anlage
{
    public class WeichenListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Weiche> Liste;

        private Z21 Z21 { get; set; }

        public WeichenListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Weiche>();
        }

        public WeichenListe(string Dateiname)
        {
            DateiImportieren(Dateiname);
        }

        public void DigitalzentraleZugriff(Z21 zentrale)
        {
            Z21 = zentrale;
            foreach(Weiche weiche in Liste)
            {
                weiche.DigitalzentraleZugriff(zentrale);
            }
        }

        public void DateiImportieren(string Dateiname)
        {
            Liste = new List<Weiche>();
            Verzeichnis = new Dictionary<string, int>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen           
            var list = XMLFile.Elements("Weiche").ToList();             //Alle Elemente des Types Weiche in eine Liste Umwandeln 

            foreach (XElement weiche in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                int WAdresse = Int16.Parse(weiche.Element("Adresse").Value);                                //Weichenadresse des Elements auslesen
                string WName = weiche.Element("name").Value;                                                //Weichenname des Elements auslesen
                bool Wspiegeln = (weiche.Element("spiegeln").Value == "1");                                 //Parameter für gespiegelte Weichen auslesen
                int time = 500;
                if (weiche.Element("Zeit") != null) time = Int16.Parse(weiche.Element("Zeit").Value);
                Liste.Add(new Weiche() { Name = WName, Adresse = WAdresse, Spiegeln = Wspiegeln, Schaltzeit = time });  //Mit den Werten eine neue Weiche zur Fahrstr_Weichenliste hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        /// <summary>
        /// Weiche in der Liste suchen und zurückgeben
        /// </summary>
        /// <param name="Name">Name der Weiche</param>
        /// <returns>Weicheninstanz oder NULL wenn nicht gefunden</returns>
        public Weiche GetWeiche(string Name)
        {
            if (Verzeichnis.TryGetValue(Name, out int ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

        public Weiche GetWeiche(int Adresse)
        {
            int ListID = Liste.FindIndex(x => x.Adresse == Adresse); //Finde Weiche mit dieser Adresse 
            if (ListID != -1)//Weiche gefunden in der Liste
            {
                return Liste[ListID];
            }
            return null;
        }
        public void WeichenStatusAlle()
        {
            List<int> WeichenAdressen = new List<int>();
            foreach (Weiche weiche in Liste)
            {
                WeichenAdressen.Add(weiche.Adresse);
            }
            Z21.LAN_X_GET_TURNOUT_INFO(WeichenAdressen);
        }


        /// <summary>
        /// Aktuellen Status der Weiche anfordern (Nachricht senden für Weichenstatus anfordern)
        /// Mit Weichenname "Alle" werden alle Weichen der Liste abgefragt
        /// </summary>
        /// <param name="Weichenname">Name der Weiche in der Liste zum Abfragen des Status</param>
        public void WeichenStatus(string Weichenname)
        {
            if (Weichenname.Equals("Alle"))      //Alle Weichen ansprechen
            {
                foreach (Weiche weiche in Liste)
                {
                    weiche.WeichenStatusAnfragen();                 //Anfrage an Zentrale senden
                    Task.Delay(50);                                //50ms warten
                }
            }
            else
            {
                if (Verzeichnis.TryGetValue(Weichenname, out int ListID))
                {
                    Liste[ListID].WeichenStatusAnfragen();          //Anfrage an Zentrale senden
                }
            }
        }

        /// <summary>
        /// Weiche Schalten auf Position
        /// </summary>
        /// <param name="IgnoriereFahrstrasseBlock">true: Trotzdem Schalten, auch wenn Weiche durch Fahrstrasse blockiert ist</param>
        /// <param name="WeichenName">Weichennamen der zu schaltenen Weiche</param>
        /// <param name="Abzweig">true: auf Abzweig schalten / false: auf Gerade schalten</param>
        public void SetzeWeiche(string WeichenName, bool Abzweig, bool IgnoriereFahrstrasseBlock = false)
        {
            if (Verzeichnis.TryGetValue(WeichenName, out int ListID))
            {
                Liste[ListID].SetzeWeiche(Abzweig, IgnoriereFahrstrasseBlock);               
            }
        }
        /// <summary>
        /// Weichenstellung wechseln
        /// </summary>
        /// <param name="IgnoriereFahrstrasseBlock">true: Trotzdem Schalten, auch wenn Weiche durch Fahrstrasse blockiert ist</param>
        /// <param name="WeichenName">Weichennamen der zu schaltenen Weiche</param>
        public void ToggleWeiche(string WeichenName, bool IgnoriereFahrstrasseBlock = false)
        {
            if (Verzeichnis.TryGetValue(WeichenName, out int ListID))
            {
                Liste[ListID].SetzeWeiche(!Liste[ListID].Abzweig, IgnoriereFahrstrasseBlock);             
            }
        }
    }


    public class Weiche : IEquatable<Weiche>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        public Weiche()
        {
            Abzweig = false;
            Status_Unbekannt = true;
            Status_Error = false;
            Besetzt = false;
            FahrstrasseAktive = false;
            Schaltzeit = 3000;
            AmBewegen = false;
            Z21 = new Z21();
        }
        #region Parameter
        /// <summary>
        /// Name der Weiche als String
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Adresse der Weiche
        /// </summary>
        public int Adresse { get; set; }
        /// <summary>
        /// Parameter Zustand, den die Weiche bei aktiver Fahrstraße annimmt
        /// </summary>
        public bool FahrstrasseAbzweig { get; set; }
        /// <summary>
        /// Pfeilrichtung: true = vonZunge; false = zurZunge;
        /// </summary>
        public bool FahrstrasseRichtung_vonZunge { get; set; }
        /// <summary>
        /// Zeit in ms wie lange der Schaltausgang aktiv sein muss
        /// </summary>
        public int Schaltzeit { get; set; }
        /// <summary>
        /// Weichen Befehl zur Z21 wird gespiegelt. 
        /// False: Zustand 0 = Befehl 0; Zustand 1 = Befehl 1
        /// True: Zustand 0 = Befehl 1; Zustand 1 = Befehl 0
        /// </summary>
        public bool Spiegeln { get; set; }
        #endregion
        #region Status
        /// <summary>
        /// true, Weiche steht auf Abzweig 
        /// </summary>
        public bool Abzweig { get; set; }
        /// <summary>
        /// true, Weiche für eine Fahrstrasse reserviert
        /// </summary>
        public bool FahrstrasseAktive { get; set; }
        /// <summary>
        /// true, Weiche teil einer sicheren Fahrstrasse
        /// </summary>
        public bool FahrstrasseSicher { get; set; }
        /// <summary>
        /// Wenn gesetzt, Weichestellung ist unbekannt
        /// </summary>
        public bool Status_Unbekannt { get; set; }
        /// <summary>
        /// Wenn gesetzt hat Z21 einen Fehler am Decoder erkannt
        /// </summary>
        public bool Status_Error { get; set; }
        /// <summary>
        /// Weiche ist noch am bewegen (Stoppbefehl noch nicht gesendet)
        /// </summary>
        public bool AmBewegen { get; set; }
        public bool Besetzt { get; set; }

        private Z21 Z21 { get; set; }

        private Timer CooldownTimer;
        #endregion
        #region Listenfunktionen
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
            if (!(obj is Weiche objAsPart)) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Unterfunktion Weichen vergleichen
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Weiche other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Method that perform shallow copy  
        public Weiche Copy()
        {
            return (Weiche)this.MemberwiseClone();
        }
        #endregion
        #region Funktionen
        public void DigitalzentraleZugriff(Z21 zentrale)
        {
            Z21 = zentrale;
        }
        /// <summary>
        /// Weiche Schalten
        /// </summary>
        /// <param name="IgnoriereFahrstrasseBlock">true: Trotzdem Schalten, auch wenn Weiche durch Fahrstrasse blockiert ist</param>
        /// <param name="Endzustand">false: Gerade | true: Abzweig</param>
        public void SetzeWeiche(bool Endzustand, bool IgnoriereFahrstrasseBlock = false)
        {

            if (FahrstrasseAktive && !IgnoriereFahrstrasseBlock) return; //Durch Fahrstrasse kontrolliert
            if (AmBewegen) return; //Weiche noch beim schalten

            if (Spiegeln) Endzustand = !Endzustand;

            Z21.LAN_X_SET_TURNOUT(Adresse, Endzustand, true, true); //Q-Modus aktiviert, Schaltausgang aktiv
            CooldownTimer = new Timer(AusgangAuschalten, Endzustand, Schaltzeit, System.Threading.Timeout.Infinite);
            AmBewegen = true;
        }

        private void AusgangAuschalten(Object o)
        {
            if(o == null) return;
            if (o is bool Endzustand)
            {
                Z21.LAN_X_SET_TURNOUT(Adresse, Endzustand, true, false); //Q-Modus aktiviert, Schaltausgang aus
                AmBewegen = false;
            }
        }

        /// <summary>
        /// Anfrage an Z21 senden und aktuellen Weichenzustand erfragen. Antwort wird mit Involke-Funktionen ausgewertet
        /// </summary>
        public void WeichenStatusAnfragen()
        {
            Z21.LAN_X_GET_TURNOUT_INFO(Adresse);    //Paket senden "GET Weiche"
        }


        /// <summary>
        /// Rückantwort der Z21 analisieren
        /// </summary>
        /// <param name="SchaltCode">Paketinhalt der Z21</param>
        public bool StatusUpdate(int SchaltCode)
        {
            bool AlterAbzweig = Abzweig;
            switch (SchaltCode)
            {
                case 0:
                    Status_Unbekannt = true;
                    break;
                case 1:
                    Abzweig = true;
                    Status_Unbekannt = false;
                    Status_Error = false;
                    break;
                case 2:
                    Abzweig = false;
                    Status_Unbekannt = false;
                    Status_Error = false;
                    break;
                default:
                    Status_Error = true;
                    break;
            }
            if (Spiegeln) Abzweig = !Abzweig;   //Weiche spiegeln, wenn Paramter gesetzt ist

            if (AlterAbzweig == Abzweig) return false; //Keine Änderungen/Update
            else return true; //Änderungen
        }

        #endregion
    }

    
}
