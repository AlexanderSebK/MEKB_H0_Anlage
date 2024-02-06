using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Belegtmelderliste
    /// </summary>
    public class BelegtmelderListe
    {
        private Dictionary<string, int> Verzeichnis;
        public List<Belegtmelder> Liste;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public BelegtmelderListe()
        {
            Verzeichnis = new Dictionary<string, int>();
            Liste = new List<Belegtmelder>();
        } 
        /// <summary>
        /// Constructor mit Importdatei
        /// </summary>
        /// <param name="Dateiname">Pfad zur Datei</param>
        public BelegtmelderListe(string Dateiname)
        {
            DateiImportieren(Dateiname);
        }

        /// <summary>
        /// Datei importieren 
        /// </summary>
        /// <param name="Dateiname">Pfad zur Datei</param>
        public void DateiImportieren(string Dateiname)
        {
            Liste = new List<Belegtmelder>();
            Verzeichnis = new Dictionary<string, int>();
            XElement XMLFile = XElement.Load(Dateiname);       //XML-Datei öffnen

            var list = XMLFile.Elements("Belegtmelder").ToList();             //Alle Elemente des Types Belegtmelders in eine Liste Umwandeln 

            foreach (XElement melder in list)                            //Alle Elemente der Liste einzeln durchlaufen
            {
                string Name;
                if (melder.Attribute("Name") != null)
                {
                    Name = melder.Attribute("Name").Value;                                //Belegtmeldername des Elements auslesen
                }
                else
                {
                    Name = melder.Element("Name").Value;
                }
                int Modulnummer = Int16.Parse(melder.Element("Modulnummer").Value);        //Modulnummer
                int Portnummer = Int16.Parse(melder.Element("Portnummer").Value);               //Portnummer
                int CoolDowntime = 6000;
                Belegtmelder belegtmelder =  new Belegtmelder() { Name = Name, Modulnummer = Modulnummer, Portnummer = Portnummer, CoolDownTime = CoolDowntime };  //Mit den Werten einen neuen Belegtmelder zur Liste hinzufügen

                belegtmelder.NachbarBlocks = new List<NachbarBlock>();

                XElement Nachbarbloecke = melder.Element("Nachbarbloecke");
                if (Nachbarbloecke != null)
                {
                    var NachbarbloeckeListe = Nachbarbloecke.Elements("Nachbarblock").ToList();
                    foreach (XElement block in NachbarbloeckeListe)
                    {
                        string BlockName;
                        if (block.Attribute("Name") != null)  BlockName = block.Attribute("Name").Value;
                        else BlockName = block.Element("Blockname").Value;

                        //bool Fahrrichtung = block.Element("Fahrtrichtung").Value == "1";
                        string KommeVon = "";
                        if(block.Attribute("KommeVon") != null) KommeVon = block.Attribute("KommeVon").Value;


                        NachbarBlock nachbar = new NachbarBlock() { BlockName = BlockName, KommeVon = KommeVon};
                        XElement WeichenGerade = block.Element("WeichenGerade");
                        XElement WeichenAbzweig = block.Element("WeichenAbzweig");
                        if (WeichenGerade != null)
                        {
                            var Weichen = WeichenGerade.Elements("Weiche").ToList();
                            foreach (XElement weichenElement in Weichen)
                            {
                                nachbar.WeichenGerade.Add(weichenElement.Value);
                            }
                        }
                        if (WeichenAbzweig != null)
                        {
                            var Weichen = WeichenAbzweig.Elements("Weiche").ToList();
                            foreach (XElement weichenElement in Weichen)
                            {
                                nachbar.WeichenAbzweig.Add(weichenElement.Value);
                            }
                        }
                        belegtmelder.NachbarBlocks.Add(nachbar);
                    } // foreach block
                } // Nachbarblöcke != null
                Liste.Add(belegtmelder);

            }
            for (int i = 0; i < Liste.Count; i++)
            {
                Verzeichnis.Add(Liste[i].Name, i);
            }
        }
        /// <summary>
        /// Cooldowntimer herunterzählen (alle)
        /// </summary>
        /// <param name="time">Zeit in ms zum herunterzählen</param>
        public void CoolDownUpdate(int time)
        {
            foreach (Belegtmelder belegtmelder in Liste)
            {
                belegtmelder.CoolDown(time);
            }
        }
        /// <summary>
        /// Aktuellen Zustand des Belegtmelder abfragen
        /// </summary>
        /// <param name="Abschnitt">Name des Abschnitts</param>
        /// <returns>true = Abschnitt ist belegt</returns>
        public bool GetBelegtStatus(string Abschnitt)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Abschnitt, out ListID))
            {
                return Liste[ListID].IstBelegt();
            }
            return false;
        }
        /// <summary>
        /// Belegtmelder-Element aus der Liste, nach Abschnittsname bekommen
        /// </summary>
        /// <param name="Abschnitt">Name des Abschnitts</param>
        /// <returns>Belegtmelder-Instanz</returns>
        public Belegtmelder GetBelegtmelder(string Abschnitt)
        {
            int ListID;
            if (Verzeichnis.TryGetValue(Abschnitt, out ListID))
            {
                return Liste[ListID];
            }
            return null;
        }
        /// <summary>
        /// Belegtmelder-Elemente aus der Liste, nach Port und Moduladresse bekommen
        /// </summary>
        /// <param name="Modul">Modulnummer</param>
        /// <param name="Port">Portnummer</param>
        /// <returns>Liste der Belegtmelder-Instanzen</returns>
        public List<Belegtmelder> GetBelegtmelder(int Modul, int Port)
        {
            List<Belegtmelder> Portliste = Liste.FindAll(x => x.Modulnummer == Modul);
            List<Belegtmelder> belegtmelder = Portliste.FindAll(x => x.Portnummer == Port);
            return belegtmelder;
        }
        /// <summary>
        /// Belegtmelderstatus schreibe (Bei empfangenen Z21-Packet)
        /// </summary>
        /// <param name="GruppenIndex">Index des Bytearrays (1 oder 2)</param>
        /// <param name="RMStatus">Belegtmelderstatus als Bytearray</param>
        public void UpdateBelegtmelder(byte GruppenIndex, byte[] RMStatus)
        {
            List<bool> PortListe = new List<bool>();
            for (int i = 0; i < RMStatus.Length; i++)
            {
                PortListe.AddRange(ConvertByteToBoolArray(RMStatus[i]));

                for(int a = 0; a < 8;a++)
                {
                    int ModulNummer = i + 1;
                    if (GruppenIndex == 1) ModulNummer += 10;
                    int PortNummer = a + 1;

                    List<Belegtmelder> belegtmelder = GetBelegtmelder(ModulNummer, PortNummer);
                    foreach (Belegtmelder melder in belegtmelder)
                    {
                        if (melder == null) continue;
                        melder.MeldeBesetzt(PortListe[a]);
                    }
                }
                PortListe.Clear();
            }
        }
        /// <summary>
        /// Anfrage an Zentrale für neuen Belegtmelderstatus
        /// </summary>
        /// <param name="z21">Instanz der Zentrale</param>
        /// <param name="GruppenIndex">Index des Bytearrays</param>
        public void StatusAnfordernBelegtmelder(Z21 z21, byte GruppenIndex)
        {
            z21.LAN_RMBUS_GETDATA(GruppenIndex);
        }
        private static bool[] ConvertByteToBoolArray(byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) != 0;

            return result;
        }

    }
    /// <summary>
    /// Belegtmelder
    /// </summary>
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
        /// Ist der Gleisabschnitt sicher belegt
        /// </summary>
        private bool Stabil {  set; get; }
        private int CoolUpTimer { set; get; }
        /// <summary>
        /// Zeit wie lange noch der Status belegt aktiv bleibt
        /// </summary>
        private int CoolDownTimer { set; get; }
        /// <summary>
        /// Block von Lok registriert
        /// </summary>
        public string Registriert { set; get; }


        public List<NachbarBlock> NachbarBlocks { set; get; }

        #endregion

        /// <summary>
        /// Gibt den Namen des nächsten Blocks an
        /// </summary>
        /// <param name="InFahrt">True = In normaler Fahrtrichtung </param>
        /// <param name="weichenListe">Liste der benutzten Weichne (globale Liste)</param>
        /// <returns>Name des Nächsten Blocks oder "gesperrt"</returns>
        public string NaechsterBlock(string Einfahrtsblock, WeichenListe weichenListe)
        {
            foreach(NachbarBlock block in NachbarBlocks)
            {
                if(block.IstErreichbar(weichenListe, Einfahrtsblock))
                {
                    return block.BlockName;
                }
            }
            return "gesperrt";
        }

        /// <summary>
        /// Abfrage nach dem aktuellen Belegtstatus
        /// </summary>
        /// <returns>true - Abschnitt belegt, flase - Abschnittfrei</returns>
        public bool IstBelegt()
        {
            if (Belegt && Stabil) return true;
            else
            {
                if (CoolDownTimer > 0) return true;
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
                    if (Stabil == true) CoolDownTimer = CoolDownTime;
                    else CoolDownTimer = 0;
                }
            }
            Belegt = Status;
            if (Status == false)
            {
                Stabil = false;
                CoolUpTimer = 0;
            }
        }

        /// <summary>
        /// Cooldownzeit berechnen
        /// </summary>
        /// <param name="ZeitVergangen">Zeit nach dem letzten Aufruf (in ms)</param>
        public void CoolDown(int ZeitVergangen)
        {
            if ((!Belegt) && (CoolDownTimer > 0))
            {
                CoolDownTimer -= ZeitVergangen;
                if (CoolDownTimer <= 0) CoolDownTimer = 0;
            }

            if (Stabil == false)
            {
                if (Belegt && (CoolUpTimer <= 500))
                {
                    CoolUpTimer += ZeitVergangen;
                }
                if (CoolUpTimer >= 500) Stabil = true;
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


    public class NachbarBlock
    {
        public List<String> WeichenAbzweig; //Liste von Weichennamen, die auf Abzweig stehen müssen, damit dieser Block erreicht werden kann
        public List<String> WeichenGerade;  //Liste von Weichennamen, die auf Gerade stehen müssen, damit dieser Block erreicht werden kann
        public string KommeVon; //Letzte Block, aus dem der Zug eingefahren ist 

        public string BlockName; //Name des Nächsten Blocks
        public NachbarBlock()
        {
            WeichenAbzweig = new List<String>();
            WeichenGerade = new List<String>();
            KommeVon = "";
        }

        public bool IstErreichbar(WeichenListe weichenListe, string komme="")
        {
            if(KommeVon != null)
            {
                if (KommeVon != "")
                {
                    if (!KommeVon.Equals(komme)) return false;
                }
            }
            foreach(string weichename in WeichenAbzweig)
            {
                Weiche weiche = weichenListe.GetWeiche(weichename);
                if(weiche == null) return false;
                if(!weiche.Abzweig) return false;
            }
            foreach (string weichename in WeichenGerade)
            {
                Weiche weiche = weichenListe.GetWeiche(weichename);
                if (weiche == null) return false;
                if (weiche.Abzweig) return false;
            }
            return true;
        }
    }
}
