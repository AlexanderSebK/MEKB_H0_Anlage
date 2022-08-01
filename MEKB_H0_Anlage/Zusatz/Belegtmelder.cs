﻿using System;
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
        public Belegtmelder GetBelegtmelder(int Modul, int Port)
        {
            List<Belegtmelder> Portliste = Liste.FindAll(x => x.Modulnummer == Modul);
            Belegtmelder belegtmelder = Portliste.Find(x => x.Portnummer == Port);
            return belegtmelder;
        }
        public void UpdateBelegtmelder(byte GruppenIndex, byte[] RMStatus)
        {
            List<bool> PortListe = new List<bool>();
            for (int i = 0; i < RMStatus.Length; i++)
            {
                PortListe.AddRange(ConvertByteToBoolArray(RMStatus[i]));

                for(int a = 0; a < 8;a++)
                {
                    int ModulNummer = i + 1;
                    if (GruppenIndex == 1) ModulNummer = ModulNummer + 10;
                    int PortNummer = a + 1;

                    Belegtmelder belegtmelder = GetBelegtmelder(ModulNummer, PortNummer);
                    if (belegtmelder == null) continue;
                    belegtmelder.MeldeBesetzt(PortListe[a]);
                }
                PortListe.Clear();
            }
        }
        public bool BlockFrei(string Blockname)
        {
            if (Blockname == null) return false;
            List<string> Blocklist = new List<string>();
            switch (Blockname)
            {
                case "Block1": Blocklist = new List<string>() { "Block1_a", "Block1_b", "Block1_Halt" }; break;
                case "Block2": Blocklist = new List<string>() { "Block2", "Block2_Halt" }; break;
                case "Block3": Blocklist = new List<string>() { "Block3" }; break;
                case "Block4": Blocklist = new List<string>() { "Block4" }; break;
                case "Block5": Blocklist = new List<string>() { "Block5", "Block5_Halt" }; break;
                case "Block6": Blocklist = new List<string>() { "Block6", "Block6_Halt" }; break;
                case "Block7": Blocklist = new List<string>() { "Block7", "SchattenMitte1", "SchattenMitte2" }; break;
                case "Block8": Blocklist = new List<string>() { "Block8", "Block8_Halt" }; break;
                case "Block9": Blocklist = new List<string>() { "Block9", "Block9_Halt" }; break;
                case "Bahnhof1": Blocklist = new List<string>() { "HBf1", "HBf1_Halt_L", "HBf1_Halt_R" }; break;
                case "Bahnhof2": Blocklist = new List<string>() { "HBf2", "HBf2_Halt_L", "HBf2_Halt_R" }; break;
                case "Bahnhof3": Blocklist = new List<string>() { "HBf3", "HBf3_Halt_L", "HBf3_Halt_R" }; break;
                case "Bahnhof4": Blocklist = new List<string>() { "HBf4", "HBf4_Halt_L", "HBf4_Halt_R" }; break;
                case "Bahnhof5": Blocklist = new List<string>() { "HBf5", "HBf5_Halt_L", "HBf5_Halt_R" }; break;
                case "Bahnhof6": Blocklist = new List<string>() { "HBf6", "HBf6_Halt_L", "HBf6_Halt_R" }; break;
                case "Tunnel1": Blocklist = new List<string>() { "Tunnel1", "Tunnel1_Halt", "Tunnel1_Einfahrt" }; break;
                case "Tunnel2": Blocklist = new List<string>() { "Tunnel2", "Tunnel2_Halt", "Tunnel2_Einfahrt" }; break;
                case "Eingleisen": Blocklist = new List<string>() { "Eingleisen", "Eingleisen_Halt" }; break;
                case "Schatten1": Blocklist = new List<string>() { "Schatten_Gl1", "Schatten_Gl1_Halt" }; break;
                case "Schatten2": Blocklist = new List<string>() { "Schatten_Gl2", "Schatten_Gl2_Halt" }; break;
                case "Schatten3": Blocklist = new List<string>() { "Schatten_Gl3", "Schatten_Gl3_Halt" }; break;
                case "Schatten4": Blocklist = new List<string>() { "Schatten_Gl4", "Schatten_Gl4_Halt" }; break;
                case "Schatten5": Blocklist = new List<string>() { "Schatten_Gl5", "Schatten_Gl5_Halt" }; break;
                case "Schatten6": Blocklist = new List<string>() { "Schatten_Gl6", "Schatten_Gl6_Halt" }; break;
                case "Schatten7": Blocklist = new List<string>() { "Schatten_Gl7", "Schatten_Gl7_Halt" }; break;
                case "Schatten8": Blocklist = new List<string>() { "Schatten_Gl8", "Schatten_Gl8_Halt" }; break;
                case "Schatten9": Blocklist = new List<string>() { "Schatten_Gl9", "Schatten_Gl9_Halt" }; break;
                case "Schatten10": Blocklist = new List<string>() { "Schatten_Gl10", "Schatten_Gl10_Halt" }; break;
                case "Schatten11": Blocklist = new List<string>() { "Schatten_Gl11", "Schatten_Gl11_Halt" }; break;
                case "Bhf_Gleis1_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W6" }; break;
                case "Bhf_Gleis2_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W6" }; break;
                case "Bhf_Gleis3_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W2_W3", "W5"}; break;
                case "Bhf_Gleis4_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W2_W3", "W5" , "DKW7_W8" }; break;
                case "Bhf_Gleis5_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W2_W3", "W5", "DKW7_W8", "DKW9" }; break;
                case "Bhf_Gleis6_AusfahrtL": Blocklist = new List<string>() { "W1_W4", "W2_W3", "W5", "DKW7_W8", "DKW9" }; break;
                default: return false;
            }


            bool Erg = false; //Variable: Einer der Abschnitte belegt?
            foreach (string Abschnitt in Blocklist)
            {
                if (Verzeichnis.TryGetValue(Abschnitt, out int ListID))
                {
                    Erg |= Liste[ListID].IstBelegt();
                }
            }
            return !Erg;    //Invertieretes Ergebnis übergeben 
        }

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
