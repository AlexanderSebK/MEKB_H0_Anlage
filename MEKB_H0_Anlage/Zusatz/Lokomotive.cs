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
        #region Zusatzangaben (Nur als Suche benötigt)
        /// <summary>
        /// Epoche der Lok (0 = keine Gegeben)
        /// </summary>
        public int Epoche { set; get; }
        /// <summary>
        /// Typ der Lok (Dampflok)
        /// </summary>
        public string Typ { set; get; }
        /// <summary>
        /// Eisenbahn Verwaltung
        /// </summary>
        public string Verwaltung { set; get; }
        /// <summary>
        /// Hersteller des Models
        /// </summary>
        public string Hersteller { set; get; }
        #endregion
        #region Fahreigenschaften
        /// <summary>
        /// Nur für Automatik: Maximale Fahrstufe in Prozent. 
        /// FahrstufeInfo   128, 28, 14
        ///     100         128, 28, 14
        ///     80          102, 22, 11,
        /// </summary>
        public int V_max { set; get; }
        /// <summary>
        /// Nur für Automatik: Mittlere Fahrstufe (ca. 40km/h)
        /// In Prozent
        /// </summary>
        public int V_mid { set; get; }
        /// <summary>
        /// Nur für Automatik: Niedrige Geschwindigkeit (ca. Schrittgeschwindigkeit)
        /// Zum Ranrollen an das Rote Signal
        /// </summary>
        public int V_min { set; get; }
        /// <summary>
        /// Zeit bis von Stand auf Vmax Bescheunigt werden soll
        /// Sekunden
        /// </summary>
        public int ZeitBisMax { set; get; }
        #endregion
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
        /// <summary>
        /// Constructor
        /// </summary>
        public Lokomotive()
        {
            Adresse = 0;
            Name = "";
            Gattung = "";
            Fahrstufe = 0;
            Richtung = 0;

            Funktionen = new List<string>();
            for (int i = 0; i < 21; i++) { Funktionen.Add(null); }
            AktiveFunktion = new bool[30];
            Steuerpult = new ZugSteuerpult(this);
            Automatik = false;

            Epoche = 0;
            Typ = "";
            Verwaltung = "";
            Hersteller = "";

            V_max = 100;
            V_mid = 40;
            V_min = 5;
            ZeitBisMax = 60;
        }
        /// <summary>
        /// Constructor mit XML-Dateipfad. Werte aus der XML-Datei werden übernommen
        /// </summary>
        /// <param name="fileName">Dateipfad zur XML-Datei</param>
        public Lokomotive(string fileName)
        {
            Adresse = 0;
            Name = "";
            Gattung = "";
            Fahrstufe = 0;
            Richtung = 0;

            Funktionen = new List<string>();
            for (int i = 0; i < 21; i++) { Funktionen.Add(null); }
            AktiveFunktion = new bool[30];
            Steuerpult = new ZugSteuerpult(this);
            Automatik = false;

            Epoche = 0;
            Typ = "";
            Verwaltung = "";
            Hersteller = "";

            if (!Path.GetExtension(fileName).Equals(".xml")) return;
            XElement XMLFile = XElement.Load(fileName);              //XML-Datei öffnen
            if (XMLFile == null) return;
            XElement lok = XMLFile.Elements("Lok").ToList().First(); //Alle Elemente des Types Lok in eine Liste Umwandeln 

            //LokAdresse lesen
            if (lok.Element("Adresse") != null)     Adresse = Int16.Parse(lok.Element("Adresse").Value);     
            if (lok.Element("Name") != null)        Name = lok.Element("Name").Value;     
            if (lok.Element("Gattung") != null)     Gattung = lok.Element("Gattung").Value;
            if (lok.Element("Epoche") != null)      Epoche = Int16.Parse(lok.Element("Epoche").Value);
            if (lok.Element("Typ") != null)         Typ = lok.Element("Typ").Value;
            if (lok.Element("Verwaltung") != null)  Verwaltung = lok.Element("Verwaltung").Value;
            if (lok.Element("Hersteller") != null)  Hersteller = lok.Element("Hersteller").Value;
            Funktionen.Clear();    
            Funktionen.Add("Licht");
            for (int i = 1; i <= 21; i++)
            {
                string Label = String.Format("Funktion{0}", i);
                if (lok.Element(Label) == null)
                {
                    Funktionen.Add(null);
                }
                else
                {
                    Funktionen.Add(lok.Element(Label).Value);
                }
            }
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
        /// <summary>
        /// Aktuellen Block berechnen
        /// </summary>
        /// <param name="belegtmelderListe">Liste der Blöcke</param>
        /// <param name="weichenListe">Weichenliste</param>
        public void BlockVerfolgung(BelegtmelderListe belegtmelderListe, WeichenListe weichenListe)
        {
            // Wenn Position unbekannt: Funktion nicht ausführen
            if (AktuellerBlock.Equals("Lok verloren")) return;
             
            // Lok fährt
            if (this.Fahrstufe != 0)
            {
                // Letzten gespeicherten Block auslesen
                Belegtmelder Aktuel = belegtmelderListe.GetBelegtmelder(this.AktuellerBlock);
                if (Aktuel == null) // Block unbekannt -> Lok verloren
                {
                    AktuellerBlock = "Lok verloren";
                    return;
                }
                if(!Aktuel.IstBelegt()) //Keine Belegtmeldung -> Lok verloren
                {
                    AktuellerBlock = "Lok verloren";
                    return;
                }
                //Potentieller nächsten Nachbarblock finden
                Belegtmelder Naechster = belegtmelderListe.GetBelegtmelder(Aktuel.NaechsterBlock(this.VorherigerBlock, weichenListe));
                if(Naechster == null) //Nicht gefunden
                {
                    return;
                }
                if(Naechster.IstBelegt()) // Gefunden und nächster Block ist belegt
                {
                    if(Naechster.Registriert.Equals("")) // Nächster Block nicht von einer anderen Lok reserviert
                    {
                        Naechster.Registriert = this.Name; //Lok für diesen Block registrieren
                        VorherigerBlock = AktuellerBlock; //Aktuellen Block in Vorherigen Block speichern
                        AktuellerBlock = Naechster.Name; //Nächsten Block als aktuellen Block definieren
                    }
                }
            }
        }
        #endregion

        #region Export
        public XElement ExportLokData()
        {
            XElement LokXMLData = new XElement("Lokliste",
                new XElement("Lok",
                    new XElement("Name", Name),
                    new XElement("Adresse", Adresse.ToString()),
                    new XElement("Gattung", Gattung),
                    new XElement("Epoche", Epoche.ToString()),
                    new XElement("Typ", Typ),
                    new XElement("Verwaltung", Verwaltung),
                    new XElement("Hersteller", Hersteller),
                    new XElement("Funktion1", Funktionen[1]),
                    new XElement("Funktion2", Funktionen[2]),
                    new XElement("Funktion3", Funktionen[3]),
                    new XElement("Funktion4", Funktionen[4]),
                    new XElement("Funktion5", Funktionen[5]),
                    new XElement("Funktion6", Funktionen[6]),
                    new XElement("Funktion7", Funktionen[7]),
                    new XElement("Funktion8", Funktionen[8]),
                    new XElement("Funktion9", Funktionen[9]),
                    new XElement("Funktion10", Funktionen[10]),
                    new XElement("Funktion11", Funktionen[11]),
                    new XElement("Funktion12", Funktionen[12]),
                    new XElement("Funktion13", Funktionen[13]),
                    new XElement("Funktion14", Funktionen[14]),
                    new XElement("Funktion15", Funktionen[15]),
                    new XElement("Funktion16", Funktionen[16]),
                    new XElement("Funktion17", Funktionen[17]),
                    new XElement("Funktion18", Funktionen[18]),
                    new XElement("Funktion19", Funktionen[19]),
                    new XElement("Funktion20", Funktionen[20])
                )
            );



            return LokXMLData;
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
            if (string.IsNullOrEmpty(Gattung)) return "";
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
                default: return "";
            }
        }
    }
}
