using System;
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
}
