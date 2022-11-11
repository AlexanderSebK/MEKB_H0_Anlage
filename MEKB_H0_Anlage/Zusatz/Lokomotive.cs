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
    public class LokomotivenListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Lokomotive> Liste;
        public LokomotivenListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Lokomotive>();
        }
        public LokomotivenListe(string Dateiname)
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Lokomotive>();
            DateiImportieren(Dateiname);
        }
        public Lokomotive GetFahrstrasse(string Name)
        {
            if (Verzeichnis.TryGetValue(Name, out int ListID))
            {
                return Liste[ListID];
            }
            return null;
        }

        public void DateiImportieren(string Dateiname)
        {
            string[] fileEntries = Directory.GetFiles(Dateiname);
            string KeineLokAdr = "Folgende Dateien besitzen keine LokAdressen und werden ignoriert:\n\n";
            bool FehlendeAdr = false;
            foreach (string fileName in fileEntries)
            {

                XElement XMLFile = XElement.Load(fileName);              //XML-Datei öffnen
                var list = XMLFile.Elements("Lok").ToList();             //Alle Elemente des Types Lok in eine Liste Umwandeln 

                foreach (XElement lok in list)                            //Alle Elemente der Liste einzeln durchlaufen
                {
                    Lokomotive Lokomotive = new Lokomotive();
                    if (lok.Element("Adresse") == null)
                    {
                        KeineLokAdr += String.Format("- {0} \n", fileName);
                        FehlendeAdr = true;
                        continue;
                    }
                    Lokomotive.Adresse = Int16.Parse(lok.Element("Adresse").Value);     //LokAdresse des Elements auslesen

                    if (lok.Element("Name") == null) Lokomotive.Name = "";
                    else Lokomotive.Name = lok.Element("Name").Value;                   //Lokname des Elements auslesen
                    if (lok.Element("Gattung") == null) Lokomotive.Gattung = "";
                    else Lokomotive.Gattung = lok.Element("Gattung").Value;             //StandardGattung Eintragen
                    Lokomotive.Funktionen.Add("Licht");
                    for (int i = 1; i <= 21; i++)
                    {
                        string Label = String.Format("Funktion{0}", i);
                        if (lok.Element(Label) == null)
                        {
                            Lokomotive.Funktionen.Add(null);
                        }
                        else
                        {
                            Lokomotive.Funktionen.Add(lok.Element(Label).Value);
                        }
                    }

                    Liste.Add(Lokomotive);                                       //Lokomotive zur Lokliste hinzufügen
                }
            }
            if (FehlendeAdr) MessageBox.Show(KeineLokAdr, "Keine Adressen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (!Liste.GroupBy(x => x.Adresse).All(g => g.Count() == 1))
            {
                List<int> DoppelAdressen = Liste.GroupBy(x => x.Adresse)
                                        .Where(g => g.Count() > 1)
                                        .Select(y => y.Key)
                                        .ToList();

                List<Lokomotive> Ausschuss = new List<Lokomotive>();

                foreach (int adr in DoppelAdressen)
                {
                    var Subliste = Liste.FindAll(Lok => Lok.Adresse == adr);
                    Ausschuss.AddRange(Subliste);
                }
                string nachricht = "Loks mit gleichen Adressen gefunden: \n\n";

                foreach (Lokomotive lok in Ausschuss)
                {
                    nachricht += String.Format("{0} - {1}\n", lok.Adresse, lok.Name);
                }

                MessageBox.Show(nachricht, "Mehrdeutige Adresse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }

    }



    public class Lokomotive : IEquatable<Lokomotive>
    {
        #region Parameter
        /// <summary>
        /// Parameter: Name der Lok als String
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Parameter: Adresse der Lok
        /// </summary>
        public int Adresse { get; set; }
        /// <summary>
        /// Parameter: Standard Gattung
        /// </summary>
        public string Gattung { get; set; }        
        /// <summary>
        /// Funktionstasten
        /// </summary>
        public List<string> Funktionen { get; set; }
        /// <summary>
        /// Anzahl Fahrstufen 0=14, 2=28, 4 = 128
        /// </summary>
        public byte FahrstufenInfo { get; set; }
        #endregion
        #region Status
        /// <summary>
        /// Aktuelle Geschwindigkeit
        /// </summary>
        public int Fahrstufe { get; set; }
        /// <summary>
        /// Aktuelle Richtung
        /// </summary>
        public int Richtung { get; set; }
        /// <summary>
        /// True: Richtung wird gespiegelt
        /// </summary>
        public bool LokUmgedreht { get; set; }
        /// <summary>
        /// Aktive Funktionen
        /// </summary>
        public bool[] AktiveFunktion { get; set; }     
        /// <summary>
        /// True: Lok wird vom Programm gesteuert
        /// False: Lok wird manuel über Handy oder Lokmaus gesteuert
        /// </summary>
        public bool Automatik { set; get; }
        public string AktuellerBlock { set; get; }
        public string VorherigerBlock { set; get; } 

        public int ErlaubteGeschwindigkeit { set; get; }
        #endregion

        #region Links und Delegates
        /// <summary>
        /// Verknüpfung zum Steuerpult-Fenster
        /// </summary>
        public ZugSteuerpult Steuerpult { get; set; }

        /// <summary>
        /// Verknüpfung zur Funktion um die Fahrstufe einzustellen. Funktion übergreift zur Steuerpult-Fenster und Z21-Klasse
        /// </summary>
        /// <param name="Adresse">Adresse der Lok</param>
        /// <param name="Fahrstufe">Neue Fahrstufe</param>
        /// <param name="Richtung">Fahrtrichtung</param>
        /// <param name="Fahstrufeninfo">Fahrstufenformat</param>
        public delegate void CMD_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo);
        /// <summary>
        /// Verknüpfung zur Funktion um die Funktionen der Lok zus schalten. Funktion übergreift zur Steuerpult-Fenster und Z21-Klasse
        /// </summary>
        /// <param name="Adresse">Adresse der Lok</param>
        /// <param name="Zustand">Neuer Zustand der Funktion</param>
        /// <param name="FunktionsNr">Funktionsnummer</param>
        public delegate void CMD_LOKFUNKTION(int Adresse, byte Zustand, byte FunktionsNr);
        /// <summary>
        /// Verknüpfung zur Funktion um den Status der Lok abzufragen. Funktion übergreift zur Steuerpult-Fenster und Z21-Klasse
        /// </summary>
        /// <param name="Adresse">Adresse der Lok</param>
        public delegate void CMD_LOKSTATUS(int Adresse);

        private CMD_LOKFAHRT setLOKFahrt;
        private CMD_LOKFUNKTION setLOKFunktion;
        private CMD_LOKSTATUS setLOKStatus;

        #endregion

        #region Funktions
        #region Konstruktors
        public Lokomotive()
        {
            Funktionen = new List<string>();
            AktiveFunktion = new bool[30];
            Steuerpult = new ZugSteuerpult(this);
            Automatik = false;
        }
        #endregion
        #region Delegates Registrierungen
        public void Register_CMD_LOKFAHRT(CMD_LOKFAHRT function)
        {
            setLOKFahrt = function;
            Steuerpult.Register_CMD_LOKFAHRT(Set_LOKFAHRT);
        }
        public void Register_CMD_LOKFUNKTION(CMD_LOKFUNKTION function)
        {
            setLOKFunktion = function;
            Steuerpult.Register_CMD_LOKFUNKTION(Set_LOKFUNKTION);
        }
        public void Register_CMD_LOKSTATUS(CMD_LOKSTATUS function)
        {
            setLOKStatus = function;
            Steuerpult.Register_CMD_LOKSTATUS(Set_LOKSTATUS);
        }
        #endregion
        #region Z21-Loksteuerung
        private void Set_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            int RichtungMitUmkehr = Richtung;
            if (LokUmgedreht)
            {
                if (Richtung == LokFahrstufen.Vorwaerts) RichtungMitUmkehr = LokFahrstufen.Rueckwaerts;
                else RichtungMitUmkehr = LokFahrstufen.Vorwaerts;
            }
            setLOKFahrt?.Invoke(Adresse, Fahrstufe, RichtungMitUmkehr, Fahstrufeninfo);
        }
        private void Set_LOKFUNKTION(int Adresse, byte Zustand, byte FunktionsNr)
        {
            setLOKFunktion?.Invoke(Adresse, Zustand, FunktionsNr);
        }
        private void Set_LOKSTATUS(int Adresse)
        {
            setLOKStatus?.Invoke(Adresse);
        }
        #endregion
        #region Listenfunktionen
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
            if (!(obj is Lokomotive objAsPart)) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Unterfunktion Weichen vergleichen
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Lokomotive other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Method that perform shallow copy  
        public Lokomotive Copy()
        {
            return (Lokomotive)this.MemberwiseClone();
        }
        #endregion
        #region BlockVerwaltung
        public void BlockVerfolgung(BelegtmelderListe belegtmelderListe, WeichenListe weichenListe)
        {
            if (AktuellerBlock.Equals("Lok verloren")) return;
            if (this.Fahrstufe != 0)
            {
                Belegtmelder Aktuel = belegtmelderListe.GetBelegtmelder(this.AktuellerBlock);
                if (Aktuel == null)
                {
                    AktuellerBlock = "Lok verloren";
                    return;
                }
                if(!Aktuel.IstBelegt()) //Keine Belegtmeldung -> Lok verloren
                {
                    AktuellerBlock = "Lok verloren";
                    return;
                }
                Belegtmelder Naechster = belegtmelderListe.GetBelegtmelder(Aktuel.NaechsterBlock(this.VorherigerBlock, weichenListe));
                if(Naechster == null)
                {
                    return;
                }
                if(Naechster.IstBelegt())
                {
                    if(Naechster.Registriert.Equals(""))
                    {
                        Naechster.Registriert = this.Name;
                        VorherigerBlock = AktuellerBlock;
                        AktuellerBlock = Naechster.Name;
                    }
                }
            }
        }
        #endregion
        #endregion
    }


    public class LokKontrolle
    {
        public LokKontrolle()
        {

        }
        public static string Abkuerzung(string Gattung)
        {
            switch (Gattung)
            {
                case "InterCityExpress": return "ICE";
                case "InterCity": return "IC";
                case "InterRegioExpress": return "IRE";
                case "InterRegio": return "IR";
                case "RegionalExpress": return "RE";
                case "RegionalBahn": return "RB";
                case "S-Bahn": return "S";
                case "Güterzug": return "G";
                default: return Gattung;
            }
        }
    }

}
