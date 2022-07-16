using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    public class BelegtmelderListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Belegtmelder> Liste;

        public BelegtmelderListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Belegtmelder>();
        } 

        public BelegtmelderListe(string Dateiname)
        {
            DateiImportieren(Dateiname);
        }

        public void DateiImportieren(string Dateiname)
        {
            Liste = new List<Belegtmelder>();
            Verzeichnis = new Dictionary<string, int>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen

            var list = XMLFile.Elements("Belegtmelder").ToList();             //Alle Elemente des Types Belegtmelders in eine Liste Umwandeln 

            foreach (XElement melder in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                string Name = melder.Element("Name").Value;                                //Belegtmeldername des Elements auslesen
                int Modulnummer = Int16.Parse(melder.Element("Modulnummer").Value);        //Modulnummer
                int Portnummer = Int16.Parse(melder.Element("Portnummer").Value);               //Portnummer
                int CoolDowntime = 6000;
                Liste.Add(new Belegtmelder() { Name = Name, Modulnummer = Modulnummer, Portnummer = Portnummer, CoolDownTime = CoolDowntime });  //Mit den Werten einen neuen Belegtmelder zur Liste hinzufügen
            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

        public void CoolDownUpdate(int time)
        {
            foreach (Belegtmelder belegtmelder in Liste)
            {
                belegtmelder.CoolDown(time);
            }
        }

        public bool GetBelegtStatus(string Abschnitt)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Abschnitt, out ListID))
            {
                return Liste[ListID].IstBelegt();
            }
            return false;
        }

        public Belegtmelder GetBelegtmelder(string Abschnitt)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Abschnitt, out ListID))
            {
                return Liste[ListID];
            }
            return null;
        }



    }

    public class Belegtmelder : IEquatable<Belegtmelder>
    {
        #region Parameter
        /// <summary>
        /// Name des Belegtmelders als String
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Adresse des Moduls 1...20
        /// </summary>
        public int Modulnummer { get; set; }
        /// <summary>
        /// Portnummer in dem Modul
        /// </summary>
        public int Portnummer { get; set; }
        /// <summary>
        /// Zeit in ms, we lange der Belegtmelder noch als belegt gewertet wird, nachdem keine Belegung mehr festgestellt wurde
        /// </summary>
        public int CoolDownTime { set; get; }
        #endregion
        #region Variablen
        /// <summary>
        /// Belegtstatus 
        /// </summary>
        private bool Belegt { set; get; }
        /// <summary>
        /// Zeit wie lange noch der Status belegt aktiv bleibt
        /// </summary>
        private int Time { set; get; }
        #endregion

        /// <summary>
        /// Abfrage nach dem aktuellen Belegtstatus
        /// </summary>
        /// <returns>true - Abschnitt belegt, flase - Abschnittfrei</returns>
        public bool IstBelegt()
        {
            if (Belegt) return true;
            else
            {
                if (Time > 0) return true;
                else return false;
            }
        }

        /// <summary>
        /// Setze Belegtstatus
        /// </summary>
        /// <param name="Status">Neuer Status</param>
        public void MeldeBesetzt(bool Status)
        {
            if (Belegt == true)
            {
                if (Status == false)
                {
                    Time = CoolDownTime;
                }
            }
            Belegt = Status;
        }

        /// <summary>
        /// Cooldownzeit berechnen
        /// </summary>
        /// <param name="ZeitVergangen">Zeit nach dem letzten Aufruf (in ms)</param>
        public void CoolDown(int ZeitVergangen)
        {
            if ((!Belegt) && (Time > 0))
            {
                Time -= ZeitVergangen;
                if (Time <= 0) Time = 0;
            }
        }

        #region Listen-Funktionen
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
            return (Modulnummer * 16) + Portnummer;
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Weichen vergleichen
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Belegtmelder objAsPart)) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Unterfunktion Weichen vergleichen
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Belegtmelder other)
        {
            if (other == null) return false;

            if (this.Name.Equals("") || other.Name.Equals(""))
            {
                if (this.Modulnummer == other.Modulnummer && this.Portnummer == other.Portnummer) return true;
                else return false;
            }
            else return (this.Name.Equals(other.Name));
        }
        #endregion
    }
}
