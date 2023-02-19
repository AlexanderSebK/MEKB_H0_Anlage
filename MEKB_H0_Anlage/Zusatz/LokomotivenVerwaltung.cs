using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;


namespace MEKB_H0_Anlage
{
    
    public class LokomotivenVerwaltung
    {
        /// <summary>
        /// Liste der geladenen Lokomotiven
        /// </summary>
        private List<Lokomotive> LokaleListe { set; get; }

        /// <summary>
        /// Liste von vorhandenen Typen (Wird für Suchfunktion verwendet)
        /// </summary>
        public List<string> ListeTyp { get; set; }
        /// <summary>
        /// Liste von vorhandenen Epochen (Wird für Suchfunktion verwendet)
        /// </summary>
        public List<string> ListeEpoche { get; set; }
        /// <summary>
        /// Liste von vorhandener Hersteller (Wird für Suchfunktion verwendet)
        /// </summary>
        public List<string> ListeHersteller { get; set; }
        /// <summary>
        /// Liste von vorhandener Bahnverwaltungen (Wird für Suchfunktion verwendet)
        /// </summary>
        public List<string> ListeVerwaltung { get; set; }
        /// <summary>
        /// Liste von vorhandener Gattungen (Wird für Suchfunktion verwendet)
        /// </summary>
        public List<string> ListeGattung { get; set; }
        #region Funktionen
        #region Constructors
        public LokomotivenVerwaltung()
        {
            LokaleListe = new List<Lokomotive>();
        }

        public LokomotivenVerwaltung(string Verzeichnis)
        {
            LokaleListe = new List<Lokomotive>();
            DateiImportieren(Verzeichnis);
        }
        #endregion
        #region Liste durchsuchen
        public List<Lokomotive> AlleLoks()
        {
            return LokaleListe;
        }
        /// <summary>
        /// Suche Lok durch Name
        /// </summary>
        /// <param name="Lokname">Name der zu suchenden Lok</param>
        /// <param name="Ergebnis">Lokomotive bei Fund</param>
        /// <returns>False - Lok wurde nicht gefunden</returns>
        public bool SucheDurchName(string Lokname, out Lokomotive Ergebnis)
        {
            Ergebnis = new Lokomotive();
            int ListID = LokaleListe.IndexOf(new Lokomotive() { Name = Lokname });
            if (ListID == -1)
            {              
                return false;
            }
            Ergebnis = LokaleListe[ListID];
            return true;
        }
        /// <summary>
        /// Suche Lok durch Adresse
        /// </summary>
        /// <param name="Adresse">Adresse der zu suchenen Lok</param>
        /// <param name="Ergebnis">Lokomotive bei Fund</param>
        /// <returns>False - Lok wurde nicht gefunden</returns>
        public bool SucheDurchAdresse(decimal Adresse, out Lokomotive Ergebnis)
        {
            Ergebnis = new Lokomotive();
            int ListID = LokaleListe.FindIndex(x => x.Adresse == Adresse);
            if (ListID == -1)
            {
                return false;
            }
            Ergebnis = LokaleListe[ListID];
            return true;
        }

        /// <summary>
        /// Finde alle Lokomotiven mit dieser Epoche.
        /// </summary>
        /// <param name="Epoche">Epoche 1 bis 6 | 0 = Alle Epochen</param>
        /// <param name="Ursprungsliste">Liste der Loks die durchsucht werden soll</param>
        /// <returns>Gefilterte Ergebnisliste</returns>
        public List<Lokomotive> FindeAlleDurchEpoche(int Epoche, List<Lokomotive> Ursprungsliste)
        {
            if (Epoche == 0) return Ursprungsliste;
            return Ursprungsliste.FindAll(x => x.Epoche == Epoche);
        }

        /// <summary>
        /// Finde alle Lokomotiven vom gegebenen Hersteller
        /// </summary>
        /// <param name="Hersteller">Hersteller des Modells</param>
        /// <param name="Ursprungsliste">Liste der Loks die durchsucht werden soll</param>
        /// <returns>Gefilterte Ergebnisliste</returns>
        public List<Lokomotive> FindeAlleDurchHersteller(string Hersteller, List<Lokomotive> Ursprungsliste)
        {
            return Ursprungsliste.FindAll(x =>x.Hersteller.Equals(Hersteller));
        }

        /// <summary>
        /// Finde alle Lokomotiven vom gegebenen Typ
        /// </summary>
        /// <param name="Typ">Typ der Lokomotive</param>
        /// <param name="Ursprungsliste">Liste der Loks die durchsucht werden soll</param>
        /// <returns>Gefilterte Ergebnisliste</returns>
        public List<Lokomotive> FindeAlleDurchTyp(string Typ, List<Lokomotive> Ursprungsliste)
        {
            return Ursprungsliste.FindAll(x => x.Typ.Equals(Typ));
        }

        /// <summary>
        /// Finde alle Lokomotiven vom gegebenen Verwaltung
        /// </summary>
        /// <param name="Verwaltung">Verwaltung der Lokomotiven</param>
        /// <param name="Ursprungsliste">Liste der Loks die durchsucht werden soll</param>
        /// <returns>Gefilterte Ergebnisliste</returns>
        public List<Lokomotive> FindeAlleDurchVerwaltung(string Verwaltung, List<Lokomotive> Ursprungsliste)
        {
            return Ursprungsliste.FindAll(x => x.Verwaltung.Equals(Verwaltung));
        }

        /// <summary>
        /// Finde alle Lokomotiven vom gegebenen Gattung
        /// </summary>
        /// <param name="Gattung">Gattung der Lokomotive</param>
        /// <param name="Ursprungsliste">Liste der Loks die durchsucht werden soll</param>
        /// <returns>Gefilterte Ergebnisliste</returns>
        public List<Lokomotive> FindeAlleDurchGattung(string Gattung, List<Lokomotive> Ursprungsliste)
        {
            return Ursprungsliste.FindAll(x => x.Gattung.Equals(Gattung));
        }

        /// <summary>
        /// Unterlisten aus Gesamtliste generieren. (Sortiert nach Typ, Gattung, Epoche, Verwaltung, Hersteller)
        /// </summary>
        private void GeneriereUnterListen()
        {
            ListeTyp = new List<string>();
            ListeEpoche = new List<string>();
            ListeHersteller = new List<string>();
            ListeVerwaltung = new List<string>();
            ListeGattung = new List<string>();

            foreach (Lokomotive lok in LokaleListe)
            {
                if (lok.Hersteller != "")
                {
                    if(!ListeHersteller.Contains(lok.Hersteller)) ListeHersteller.Add(lok.Hersteller);
                }
                if (lok.Typ != "")
                {
                    if (!ListeTyp.Contains(lok.Typ)) ListeTyp.Add(lok.Typ);
                }
                if (lok.Gattung != "")
                {
                    if (!ListeGattung.Contains(lok.Gattung)) ListeGattung.Add(lok.Gattung);
                }
                if (lok.Verwaltung != "")
                {
                    if (!ListeVerwaltung.Contains(lok.Verwaltung)) ListeVerwaltung.Add(lok.Verwaltung);
                }

                switch(lok.Epoche)
                {
                    case 1: 
                        if (!ListeEpoche.Contains("I")){ ListeEpoche.Add("I"); } break;
                    case 2:
                        if (!ListeEpoche.Contains("II")){ ListeEpoche.Add("II"); } break;
                    case 3:
                        if (!ListeEpoche.Contains("III")){ ListeEpoche.Add("III"); } break;
                    case 4:
                        if (!ListeEpoche.Contains("IV")){ ListeEpoche.Add("IV"); } break;
                    case 5:
                        if (!ListeEpoche.Contains("V")){ ListeEpoche.Add("V"); } break;
                    case 6:
                        if (!ListeEpoche.Contains("VI")){ ListeEpoche.Add("VI"); } break;
                    default: break;
                }

                ListeTyp.Sort();
                ListeEpoche.Sort();
                ListeHersteller.Sort();
                ListeVerwaltung.Sort();
                ListeGattung.Sort();
            }
        }
        #endregion

        #region ArchivVerwalten
        /// <summary>
        /// Archiv importieren
        /// </summary>
        /// <param name="OrdnerName">Root-Ordner von allen Lokomotiven</param>
        public void DateiImportieren(string OrdnerName)
        {
            string[] fileEntries = Directory.GetFiles(OrdnerName, "*.xml", SearchOption.AllDirectories);
            foreach (string fileName in fileEntries)
            {
                    LokaleListe.Add(new Lokomotive(fileName));                                       //Lokomotive zur Lokliste hinzufügen
            }
            GeneriereUnterListen(); //Listen zum Suchen zusammenfassen
           
            //Prüfen ob es Loks mit der gleichen Adresse gibt
            if (!LokaleListe.GroupBy(x => x.Adresse).All(g => g.Count() == 1))
            {
                //Alle Doppelten Adressen herausfiltern
                List<int> DoppelAdressen = LokaleListe.GroupBy(x => x.Adresse)
                                        .Where(g => g.Count() > 1)
                                        .Select(y => y.Key)
                                        .ToList();

                //Alle gefundenen Adressen aus der Lokliste herauskopieren und in eine Subliste übergeben
                List<Lokomotive> Ausschuss = new List<Lokomotive>();
                foreach (int adr in DoppelAdressen)
                {
                    var Subliste = LokaleListe.FindAll(Lok => Lok.Adresse == adr);
                    Ausschuss.AddRange(Subliste);
                }

                //Nachricht mit den gefundenen Loks ausgeben
                string nachricht = "Loks mit gleichen Adressen gefunden: \n\n";
                foreach (Lokomotive lok in Ausschuss)
                {
                    nachricht += String.Format("{0} - {1}\n", lok.Adresse, lok.Name);
                }
                MessageBox.Show(nachricht, "Mehrdeutige Adresse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #endregion

        #region Verzeichnisse
        /// <summary>
        /// Abkürzung für Gattungen
        /// </summary>
        public Dictionary<string, string> Abkuerung = new Dictionary<string, string>()
        {
            {"InterCityExpress", "ICE"},
            {"InterCity", "IC"},
            {"InterRegioExpress", "IRE"},
            {"InterRegio", "IR"},
            {"RegionalExpress", "RE"},
            {"RegionalBahn", "RB"},
            {"S-Bahn", "S"},
            {"Güterzug", "G"},
            {"", "" }
        };
        #endregion
    }
}
