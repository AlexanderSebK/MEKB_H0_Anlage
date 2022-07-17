﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Konfigurationsdatei lesen/schreiben
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Wert aus Konfiguration lesen
        /// </summary>
        /// <param name="key">Name des Konfigurationswertes</param>
        /// <returns>Wert des Konfigurationswertes</returns>
        public static string ReadConfig(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings; //Konfigurationsmanager aufrufen und Datei laden
                string result = appSettings[key] ?? "Not Found";    //Name des Werts suchen
                return result;                                      //Rückgabewert
            }
            catch
            {
                return "Error";                                     //"Error" zurückgeben
            }
        }
        /// <summary>
        /// Wert in Konfiguration schreiben
        /// </summary>
        /// <param name="key">Name des Konfigurationswertes</param>
        /// <param name="value">Wert des Konfigurationswertes</param>
        /// <returns>"Success" oder "Error"</returns>
        public static string WriteConfig(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);//Konfigurationsdatei laden
                var settings = configFile.AppSettings.Settings;                                         //Konfigurationsmanager aufrufen
                if (settings[key] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                {
                    settings.Add(key, value);                                                           //Neuen Wert anlegen
                }
                else
                {
                    settings[key].Value = value;                                                        //Alten Wert überschreiben
                }
                configFile.Save(ConfigurationSaveMode.Modified);                                        //Konfigurationsdatei speichern
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);    //Konfigurationsmanager aktualisieren
                return "Success";                                                                       //Erfolgreiche Rückgabe
            }
            catch (ConfigurationErrorsException)                                                        //Fehler aufgetreten
            {
                return "Error";                                                                         //Fehler Rückgabe        
            }
        }
    }
    
    public class Fahrstrasse
    {
        public Fahrstrasse()
        {
            Fahrstr_Weichenliste = new List<Weiche>();
            ControlSetPointer = 0;
            SetPointer = 0;
            Fahrstr_Sig = new Signal();
        }
        public List<Weiche> Fahrstr_Weichenliste { get; set; }

        public Signal Fahrstr_Sig;
        public bool Safe { get; set; }
        private bool FahrstrasseGesetzt { get; set; }
        private bool FahrstrasseAktiv { get; set; }

        private int ControlSetPointer;
        private int SetPointer;

        private void WeichenSicherheit(List<Weiche> ListeGlobal, bool Sicherheitsstatus)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;
                ListeGlobal[ListID].FahrstrasseSicher = Sicherheitsstatus;
            }
        }

        public void StarteFahrstrasse(List<Weiche> ListeGlobal)
        {
            FahrstrasseGesetzt = true;
            Safe = false;
            WeichenSicherheit(ListeGlobal, Safe);
        }

        public void SetFahrstrasseRichtung(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;

                ListeGlobal[ListID].FahrstrasseRichtung_vonZunge = weiche.FahrstrasseRichtung_vonZunge;
            }
        }

        public void SetFahrstrasse(List<Weiche> ListeGlobal, Z21 Z21_Instanz)
        {
            if (SetPointer >= Fahrstr_Weichenliste.Count) SetPointer = 0;
            Weiche weiche = Fahrstr_Weichenliste[SetPointer];
            

            int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
            if (ListID == -1) return;

            if(ListeGlobal[ListID].Status_Unbekannt || ListeGlobal[ListID].Status_Error)
            {
                MessageBox.Show("Weiche reagiert nicht. Verbindung von Z21 prüfen", "Weiche unbekannt oder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FahrstrasseGesetzt = false;
                FahrstrasseAktiv = false;
                return;
            }

            int Adresse = ListeGlobal[ListID].Adresse;
            ListeGlobal[ListID].FahrstrasseRichtung_vonZunge = weiche.FahrstrasseRichtung_vonZunge;
            ListeGlobal[ListID].FahrstrasseAbzweig = weiche.FahrstrasseAbzweig;
                
            if (ListeGlobal[ListID].Abzweig != weiche.FahrstrasseAbzweig)   //Wenn Weiche noch nicht in Position ist
            {
                if (ListeGlobal[ListID].ZeitAktiv <= 0) //Weiche nicht aktiv
                {
                    if (ListeGlobal[ListID].Spiegeln)
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(Adresse, !weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    else
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(Adresse, weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                }
            }
            SetPointer++; // Nächste Weiche
        }
        public void ControlSetFahrstrasse(List<Weiche> ListeGlobal, Z21 Z21_Instanz)
        {
            if (Fahrstr_Weichenliste.Count == 0) //Weichenloser Block
            {
                Safe = true;
                WeichenSicherheit(ListeGlobal, Safe);
                return;
            }
            if (!Safe)
            {
                if (ControlSetPointer < Fahrstr_Weichenliste.Count)
                {
                    Weiche weiche = Fahrstr_Weichenliste[ControlSetPointer];
                    int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                    if (ListID == -1) return;

                    if (ListeGlobal[ListID].Spiegeln)
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(ListeGlobal[ListID].Adresse, !weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    else
                    {
                        Z21_Instanz.LAN_X_SET_TURNOUT(ListeGlobal[ListID].Adresse, weiche.FahrstrasseAbzweig, true, true);
                        ListeGlobal[ListID].ZeitAktiv = ListeGlobal[ListID].Schaltzeit;
                    }
                    ControlSetPointer++;
                }
                else
                {
                    if (GetBusyStatus(ListeGlobal) == false)
                    {
                        ControlSetPointer = 0;
                        Safe = true;
                        WeichenSicherheit(ListeGlobal, Safe);
                    }
                }
            }
        }
        public void AktiviereFahrstasse(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return;

                ListeGlobal[ListID].FahrstrasseAktive = true;           
            }
            FahrstrasseAktiv = true;
        }

        public bool GetBusyStatus(List<Weiche> ListeGlobal)
        {
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return false;

                if(ListeGlobal[ListID].ZeitAktiv > 0) return true; //Eine Weiche noch beim Schalten?
            }
            return false;
        }

        public bool GetGesetztStatus()
        {
            return FahrstrasseGesetzt;
        }
        public bool GetAktivStatus()
        {
            return FahrstrasseAktiv;
        }
        public bool CheckFahrstrassePos(List<Weiche> ListeGlobal)
        {
            if (!FahrstrasseGesetzt) return false;
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return false;
                if (ListeGlobal[ListID].Abzweig != weiche.FahrstrasseAbzweig)   //Wenn Weiche noch nicht in Position ist
                {
                    return false;
                }               
            }
            return true;
        }
       

        public List<Weiche> GetFahrstrassenListe()
        {
            return Fahrstr_Weichenliste;
        }
        public void DeleteFahrstrasse(List<Weiche> ListeGlobal)
        {
            FahrstrasseGesetzt = false;
            foreach (Weiche weiche in Fahrstr_Weichenliste)
            {
                int ListID = ListeGlobal.IndexOf(new Weiche() { Name = weiche.Name });  //Weiche in Globale Liste suchen
                if (ListID == -1) return ;

                ListeGlobal[ListID].FahrstrasseAktive = false;
            }
            FahrstrasseAktiv = false;
            Safe = false;
            WeichenSicherheit(ListeGlobal, Safe);
        }
        
    }
    public class LokKontrolle
    {
        public LokKontrolle()
        {

        }
        public static string Abkuerzung(string Gattung)
        {
            switch(Gattung)
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
    public class Lok : IEquatable<Lok>
    {
        public Lok()
        {
            Funktionen = new List<string>();
            AktiveFunktion = new bool[30];
            Steuerpult = new ZugSteuerpult(this);
            Automatik = false;
        }
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
        /// Anzahl Fahrstufen 0=14, 2=28, 4 = 128
        /// </summary>
        public byte FahrstufenInfo { get; set; }
        /// <summary>
        /// True: Lok wird vom Programm gesteuert
        /// </summary>
        public bool Automatik { set; get; }
        public ZugSteuerpult Steuerpult { get; set; }

        public delegate void CMD_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo);
        public delegate void CMD_LOKFUNKTION(int Adresse, byte Zustand, byte FunktionsNr);
        public delegate void CMD_LOKSTATUS(int Adresse);

        private CMD_LOKFAHRT setLOKFahrt;
        private CMD_LOKFUNKTION setLOKFunktion;
        private CMD_LOKSTATUS setLOKStatus;

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

        private void Set_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            int RichtungMitUmkehr = Richtung;
            if(LokUmgedreht)
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
            if (!(obj is Lok objAsPart)) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Wird bei Listensuche benötigt: Unterfunktion Weichen vergleichen
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Lok other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Method that perform shallow copy  
        public Lok Copy()
        {
            return (Lok)this.MemberwiseClone();
        }
    }
    

    

    public struct MeldeZustand
    {
        public MeldeZustand(bool besetzt, bool fahrstrasse, bool sicher, bool richtung)
        {
            Besetzt = besetzt;
            Fahrstrasse = fahrstrasse;
            Sicher = sicher;
            Richtung = richtung;
        }

        public MeldeZustand(Weiche weiche, bool richtung)
        {
            Besetzt = weiche.Besetzt;
            Fahrstrasse = weiche.FahrstrasseAktive;
            Sicher = weiche.FahrstrasseSicher;
            Richtung = weiche.FahrstrasseRichtung_vonZunge ^ richtung;
        }

        public MeldeZustand(bool StatusALL)
        {
            Besetzt = StatusALL;
            Fahrstrasse = StatusALL;
            Sicher = StatusALL;
            Richtung = StatusALL;
        }

        public bool Besetzt { get; set; }
        public bool Fahrstrasse { get; set; }
        public bool Sicher { get; set; }
        public bool Richtung { get; set; }

        public bool IstFrei()
        {
            return !(Besetzt || Fahrstrasse);
        }
    }
}
