using System;
using System.Linq;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Fenster für Z21 - Einstellugen
    /// </summary>
    public partial class Z21_Einstellung : Form
    {
        #region Instancen
        public Einstellungen Einstellungen = Einstellungen.Instance;
        /// <summary>
        /// Instance der Z21 (übernommen von dem Hauptfenster)
        /// </summary>
        private Z21 z21Instance;
        #endregion
        #region Konstruktor
        public Z21_Einstellung()
        {
            InitializeComponent();
        }
        #endregion
        #region externe Zugriff
        /// <summary>
        /// Instance der Z21 vom Hauptfenster übernehmen und Konfigurationsdatei lesen
        /// </summary>
        /// <param name="instance">Zentraleninstance</param>
        public void Get_Z21_Instance(Z21 instance)
        {
            z21Instance = instance;
            LoadConfig();
        }
        /// <summary>
        /// Schreiben eines Texts in die Textbox für Firmware
        /// </summary>
        /// <param name="Text">Textinhalt</param>
        public void SetFirmware(string Text)
        {
            Firmware.Text = Text;
        }
        /// <summary>
        /// Setzen der Checkboxen zu den aktiven Broadcast-Flags
        /// </summary>
        /// <param name="flags"></param>
        public void SetFlags(Flags flags)
        {
            Abo_AllRailCom.Checked = flags.Alle_Railcom;
            Abo_AllFahren.Checked = flags.Alle_Lok_Info;
            Abo_Fahren.Checked = flags.Fahren_Schalten;
            Abo_LOCONET_Basis.Checked = flags.LOCONET_Basic;
            Abo_LOCONET_detect.Checked = flags.LOCONET_Detect;
            Abo_LOCONET_Loks.Checked = flags.LOCONET_Lok;
            Abo_LOCONET_Weichen.Checked = flags.LOCONET_Weichen;
            Abo_Railcom.Checked = flags.Railcom;
            Abo_RMBus.Checked = flags.RM_Bus;
            Abo_SystemStatus.Checked = flags.System_Status;
        }
        /// <summary>
        /// Buttons für Verbinden und Trennen aktivieren/deaktivieren
        /// </summary>
        /// <param name="status"></param>
        public void ConnectStatus(bool status)
        {
            Z21_Connect.Enabled = !status;
            Z21_DisConnect.Enabled = status;
        }

        /// <summary>
        /// Schreiben eines Texts in die Textbox für Seriennummer
        /// </summary>
        /// <param name="data"></param>
        public void Set_SerienNummer(string data)
        {
            Seriennummer.Text = data;
        }
        #endregion
        #region Hilfsfunktionen
        /// <summary>
        /// Aktuelle Checkboxen für die Z21 Broadcast-Flags einlesen und variable erstellen
        /// </summary>
        /// <returns>Z21 Broadcast-Flags</returns>
        public Flags Get_Flag_Config()
        {
            Flags flags = new Flags(0)
            {
                Alle_Railcom = Abo_AllRailCom.Checked,
                Alle_Lok_Info = Abo_AllFahren.Checked,
                Fahren_Schalten = Abo_Fahren.Checked,
                LOCONET_Basic = Abo_LOCONET_Basis.Checked,
                LOCONET_Detect = Abo_LOCONET_detect.Checked,
                LOCONET_Lok = Abo_LOCONET_Loks.Checked,
                LOCONET_Weichen = Abo_LOCONET_Weichen.Checked,
                Railcom = Abo_Railcom.Checked,
                RM_Bus = Abo_RMBus.Checked,
                System_Status = Abo_SystemStatus.Checked,
                CAN_Detect = Abo_CAN_detect.Checked
            };
            return flags;
        }

        /// <summary>
        /// Unterfunktion Config-Datei auslesen und in die TextBoxen schreiben
        /// </summary>
        private void LoadConfig()
        {
            Einstellungen.LadeEinstellungen();
            String IP_Adresse = Einstellungen.Z21_IP;
            string[] IP = IP_Adresse.Split(new char[] { '.' });
            if (IP.Count() == 4)
            {
                IP_1.Text = IP[0];
                IP_2.Text = IP[1];
                IP_3.Text = IP[2];
                IP_4.Text = IP[3];
            }
            else
            {
                IP_1.Text = "Err:";
                IP_2.Text = "No";
                IP_3.Text = "IP";
                IP_4.Text = "saved";
            }

            IP_Port.Text = Einstellungen.Z21_Port.ToString();

            Abo_AllRailCom.Checked = Einstellungen.Z21Flags.Alle_Railcom;
            Abo_Railcom.Checked = Einstellungen.Z21Flags.Railcom;
            Abo_AllFahren.Checked = Einstellungen.Z21Flags.Alle_Lok_Info;
            Abo_Fahren.Checked = Einstellungen.Z21Flags.Fahren_Schalten;

            Abo_RMBus.Checked = Einstellungen.Z21Flags.RM_Bus;
            Abo_CAN_detect.Checked = Einstellungen.Z21Flags.CAN_Detect;
            Abo_SystemStatus.Checked = Einstellungen.Z21Flags.System_Status;

            Abo_LOCONET_Basis.Checked = Einstellungen.Z21Flags.LOCONET_Basic;
            Abo_LOCONET_Loks.Checked = Einstellungen.Z21Flags.LOCONET_Lok;
            Abo_LOCONET_Weichen.Checked = Einstellungen.Z21Flags.LOCONET_Weichen;
            Abo_LOCONET_detect.Checked = Einstellungen.Z21Flags.LOCONET_Detect;

            AutoConnect.Checked = Einstellungen.AutoZ21Connect;

        }
        #endregion
        #region Buttons
        /// <summary>
        /// Button "OK" - Fenster schließen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Eigenschaften_OK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        /// <summary>
        /// Button "Werte lesen" - Konfiguration von der Z21 abfragen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Get_Click(object sender, EventArgs e)
        {
            z21Instance.GET_FIRMWARE_VERSION();
            z21Instance.GET_BROADCASTFLAGS();
        }
        /// <summary>
        /// Button "Werte schreiben" - Konfiguration an die Z21 senden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Update_Click(object sender, EventArgs e)
        {
            z21Instance.Z21_SET_BROADCASTFLAGS(Get_Flag_Config());
        }
        /// <summary>
        /// Button "Konfiguration speichern" - Konfiguration in die Config-Datei speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Config_save_Click(object sender, EventArgs e)
        {
            Einstellungen.SpeicherEinstellungen();
        }
        /// <summary>
        /// Button "Konfiguration laden " - Konfiguration aus der Config-Datei laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Config_laden_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }
        /// <summary>
        /// Button "Verbinden" - Verbindung aufbauen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Connect_Click(object sender, EventArgs e)
        {
            z21Instance.Connect_Z21();
        }
        /// <summary>
        /// Button "Trennen" - Verbindung trennen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_DisConnect_Click(object sender, EventArgs e)
        {
            z21Instance.DisConnect_Z21();
        }
        #endregion
    }
}
