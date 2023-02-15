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
        private List<Lokomotive> LokaleListe { set; get; }
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

        private void GeneriereUnterListen()
        {

        }
        #endregion



        #region ArchivVerwalten
        public void DateiImportieren(string OrdnerName)
        {
            string[] fileEntries = Directory.GetFiles(OrdnerName, "*.xml", SearchOption.AllDirectories);
            foreach (string fileName in fileEntries)
            {
                    LokaleListe.Add(new Lokomotive(fileName));                                       //Lokomotive zur Lokliste hinzufügen
            }

            //Prüfen und melden ob Loks mit fehlenden Adressen gefunden wurden
            //if (FehlendeAdr) MessageBox.Show(KeineLokAdr, "Keine Adressen", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
