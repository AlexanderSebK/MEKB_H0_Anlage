using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using System.Web;

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

        public static string WriteLokList(List<Lokomotive> lokomotiven, int Max_Loks)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);//Konfigurationsdatei laden
                var settings = configFile.AppSettings.Settings;                                         //Konfigurationsmanager aufrufen
                for (int i = 0; i < Max_Loks; i++)
                {
                    if (i < lokomotiven.Count)
                    {
                        if (settings[String.Format("LokListe{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokListe{0}", i), lokomotiven[i].Adresse.ToString());                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokListe{0}", i)].Value = lokomotiven[i].Adresse.ToString();                                                        //Alten Wert überschreiben
                        }
                        if (settings[String.Format("LokPos{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokPos{0}", i), lokomotiven[i].AktuellerBlock.ToString());                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokPos{0}", i)].Value = lokomotiven[i].AktuellerBlock.ToString();                                                        //Alten Wert überschreiben
                        }
                        if (settings[String.Format("LokPosVor{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokPosVor{0}", i), lokomotiven[i].VorherigerBlock.ToString());                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokPosVor{0}", i)].Value = lokomotiven[i].VorherigerBlock.ToString();                                                        //Alten Wert überschreiben
                        }
                    }
                    else
                    {
                        if (settings[String.Format("LokListe{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokListe{0}", i), "0");                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokListe{0}", i)].Value = "0";                                                        //Alten Wert überschreiben
                        }
                        if (settings[String.Format("LokPos{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokPos{0}", i), "");                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokPos{0}", i)].Value = "";                                                        //Alten Wert überschreiben
                        }
                        if (settings[String.Format("LokPosVor{0}", i)] == null)                                                              //Wert mit diesem Name noch nicht vorhanden
                        {
                            settings.Add(String.Format("LokPosVor{0}", i), "");                                                           //Neuen Wert anlegen
                        }
                        else
                        {
                            settings[String.Format("LokPosVor{0}", i)].Value = "";                                                        //Alten Wert überschreiben
                        }
                    }
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

    public sealed class Einstellungen
    {
        private static readonly Lazy<Einstellungen> lazy =
        new Lazy<Einstellungen>(() => new Einstellungen());

        public static Einstellungen Instance { get { return lazy.Value; } }

        public Einstellungen() 
        {
            Z21Flags = new Flags(0);
        }

        public bool AutoSignal { get; set; }
        public bool AutoFahrdienstleister { get; set; }
        public bool AutoFahrplan {  get; set; }
        public bool AutoNotbremse { get; set; }
        public bool AutoZ21Connect { get; set; }
        public bool Bahnhofsansagen { get; set; }

        public Flags Z21Flags { get; set; }

        public UInt16 Z21_Port { get; set; }
        public string Z21_IP { get; set; }

        #region Funktionen
        public void SpeicherEinstellungen()
        {
            WriteBoolToConfig("Z21_Abos_Alle_Railcom", Z21Flags.Alle_Railcom);
            WriteBoolToConfig("Z21_Abos_Railcom", Z21Flags.Railcom);
            WriteBoolToConfig("Z21_Abos_Alle_Loks", Z21Flags.Alle_Lok_Info);
            WriteBoolToConfig("Z21_Abos_Loks", Z21Flags.Fahren_Schalten);

            WriteBoolToConfig("Z21_Abos_RMBus", Z21Flags.RM_Bus);
            WriteBoolToConfig("Z21_Abos_CANBus", Z21Flags.CAN_Detect);
            WriteBoolToConfig("Z21_Abos_System_Status", Z21Flags.System_Status);

            WriteBoolToConfig("Z21_Abos_LOCONET_Basic", Z21Flags.LOCONET_Basic);
            WriteBoolToConfig("Z21_Abos_LOCONET_Loks", Z21Flags.LOCONET_Lok);
            WriteBoolToConfig("Z21_Abos_LOCONET_Weichen", Z21Flags.LOCONET_Weichen);
            WriteBoolToConfig("Z21_Abos_LOCONET_Detector", Z21Flags.LOCONET_Detect);

            WriteBoolToConfig("Auto_Connect", AutoZ21Connect);
            Config.WriteConfig("Z21_IP", Z21_IP);
            Config.WriteConfig("Z21_Port", Z21_Port.ToString());

            WriteBoolToConfig("AutoSignal", AutoSignal);
            WriteBoolToConfig("AutoNotbremse", AutoNotbremse);
            WriteBoolToConfig("AutoFahrdienstleister", AutoFahrdienstleister);
            WriteBoolToConfig("AutoFahrplan", AutoFahrplan);
            WriteBoolToConfig("Bahnhofsansagen", Bahnhofsansagen);
        }

        public void WriteBoolToConfig(string name, bool value)
        {
            if (value) Config.WriteConfig(name, "true");
            else Config.WriteConfig(name, "false");
        }

        public void LadeEinstellungen()
        {
            Z21_Port = UInt16.Parse(Config.ReadConfig("Z21_Port"));
            Z21_IP = Config.ReadConfig("Z21_IP");

            Z21Flags.Alle_Railcom = Config.ReadConfig("Z21_Abos_Alle_Railcom").Equals("true");
            Z21Flags.Railcom = Config.ReadConfig("Z21_Abos_Railcom").Equals("true");
            Z21Flags.Alle_Lok_Info = Config.ReadConfig("Z21_Abos_Alle_Loks").Equals("true");
            Z21Flags.Fahren_Schalten = Config.ReadConfig("Z21_Abos_Loks").Equals("true");

            Z21Flags.RM_Bus = Config.ReadConfig("Z21_Abos_RMBus").Equals("true");
            Z21Flags.CAN_Detect = Config.ReadConfig("Z21_Abos_CANBus").Equals("true");
            Z21Flags.System_Status = Config.ReadConfig("Z21_Abos_System_Status").Equals("true");

            Z21Flags.LOCONET_Basic = Config.ReadConfig("Z21_Abos_LOCONET_Basic").Equals("true");
            Z21Flags.LOCONET_Lok = Config.ReadConfig("Z21_Abos_LOCONET_Loks").Equals("true");
            Z21Flags.LOCONET_Weichen = Config.ReadConfig("Z21_Abos_LOCONET_Weichen").Equals("true");
            Z21Flags.LOCONET_Detect = Config.ReadConfig("Z21_Abos_LOCONET_Detector").Equals("true");

            AutoSignal = Config.ReadConfig("AutoSignal").Equals("true");
            AutoZ21Connect = Config.ReadConfig("Auto_Connect").Equals("true");
            AutoNotbremse = Config.ReadConfig("AutoNotbremse").Equals("true");
            AutoFahrdienstleister = Config.ReadConfig("AutoFahrdienstleister").Equals("true");
            AutoFahrplan = Config.ReadConfig("AutoFahrplan").Equals("true");
            Bahnhofsansagen = Config.ReadConfig("Bahnhofsansagen").Equals("true");
        }
        #endregion
    }


    

    /// <summary>
    /// Fehlermeldungen verwalten (Austausch zwischen den Instanzen)
    /// </summary>
    public sealed class Fehlermeldung
    {
        private static readonly Lazy<Fehlermeldung> lazy =
        new Lazy<Fehlermeldung>(() => new Fehlermeldung());

        public static Fehlermeldung Instance { get { return lazy.Value; } }

        private Fehlermeldung()
        {
        }


        public delegate void FEHLER_MELDEN(string text, string typ);
        public delegate void FEHLER_ENTFERNEN(string text);
        public delegate void FEHLERTEXT_ENTFERNEN(string text);

        private FEHLER_MELDEN call_Fehlermelden;
        private FEHLER_ENTFERNEN call_Fehlerentfernen;
        private FEHLERTEXT_ENTFERNEN call_Textentfernen;

        public void Register_Fehlermelden(FEHLER_MELDEN function) { call_Fehlermelden = function; }
        public void Register_Fehlerentfernen(FEHLER_ENTFERNEN function) { call_Fehlerentfernen = function; }
        public void Register_FehlertextEntfernen(FEHLERTEXT_ENTFERNEN function) { call_Textentfernen = function; }

        public void FehlerMelden(string text, string typ)
        {
            call_Fehlermelden?.Invoke(text, typ);
        }

        public void FehlerEntfernen(string text)
        {
            call_Fehlerentfernen?.Invoke(text);
        }

        public void FehlertextEntfernen(string lokname)
        {
            call_Textentfernen?.Invoke(lokname);
        }

    }
}
