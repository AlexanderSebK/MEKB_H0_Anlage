using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    /// <summary>
    /// Fenster für Z21 - Einstellugen
    /// </summary>
    public partial class Z21_Einstellung : Form
    {
        /// <summary>
        /// Instance der Z21 (übernommen von dem Hauptfenster)
        /// </summary>
        private Z21 z21Start;
        public Z21_Einstellung()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Instance der Z21 vom Hauptfenster übernehmen und Konfigurationsdatei lesen
        /// </summary>
        /// <param name="form1">Hauptfrom</param>
        public void Get_Z21_Instance(Form1 form1)
        {
            z21Start = form1.z21Start;
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
            z21Start.Z21_GET_FIRMWARE_VERSION();
            z21Start.Z21_GET_BROADCASTFLAGS();
        }
        /// <summary>
        /// Button "Werte schreiben" - Konfiguration an die Z21 senden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Update_Click(object sender, EventArgs e)
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
            z21Start.Z21_SET_BROADCASTFLAGS(flags);
        }
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
        /// Button "Konfiguration speichern" - Konfiguration in die Config-Datei speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Config_save_Click(object sender, EventArgs e)
        {
            Config.WriteConfig("Z21_IP", string.Format("{0}.{1}.{2}.{3}", IP_1.Text, IP_2.Text, IP_3.Text, IP_4.Text));
            Config.WriteConfig("Z21_Port", IP_Port.Text.ToString());

            if (Abo_AllRailCom.Checked) Config.WriteConfig("Z21_Abos_Alle_Railcom", "true");
            else Config.WriteConfig("Z21_Abos_Alle_Railcom", "false");
            if (Abo_Railcom.Checked) Config.WriteConfig("Z21_Abos_Railcom", "true");
            else Config.WriteConfig("Z21_Abos_Railcom", "false");
            if (Abo_AllFahren.Checked) Config.WriteConfig("Z21_Abos_Alle_Loks", "true");
            else Config.WriteConfig("Z21_Abos_Alle_Loks", "false");
            if (Abo_Fahren.Checked) Config.WriteConfig("Z21_Abos_Loks", "true");
            else Config.WriteConfig("Z21_Abos_Loks", "false");

            if (Abo_RMBus.Checked) Config.WriteConfig("Z21_Abos_RMBus", "true");
            else Config.WriteConfig("Z21_Abos_RMBus", "false");
            if (Abo_CAN_detect.Checked) Config.WriteConfig("Z21_Abos_CANBus", "true");
            else Config.WriteConfig("Z21_Abos_CANBus", "false");
            if (Abo_SystemStatus.Checked) Config.WriteConfig("Z21_Abos_System_Status", "true");
            else Config.WriteConfig("Z21_Abos_System_Status", "false");

            if (Abo_LOCONET_Basis.Checked) Config.WriteConfig("Z21_Abos_LOCONET_Basic", "true");
            else Config.WriteConfig("Z21_Abos_LOCONET_Basic", "false");
            if (Abo_LOCONET_Loks.Checked) Config.WriteConfig("Z21_Abos_LOCONET_Loks", "true");
            else Config.WriteConfig("Z21_Abos_LOCONET_Loks", "false");
            if (Abo_LOCONET_Weichen.Checked) Config.WriteConfig("Z21_Abos_LOCONET_Weichen", "true");
            else Config.WriteConfig("Z21_Abos_LOCONET_Weichen", "false");
            if (Abo_LOCONET_detect.Checked) Config.WriteConfig("Z21_Abos_LOCONET_Detector", "true");
            else Config.WriteConfig("Z21_Abos_LOCONET_Detector", "false");

            if (AutoConnect.Checked) Config.WriteConfig("Auto_Connect", "true");
            else Config.WriteConfig("Auto_Connect", "false");

        }
        /// <summary>
        /// Button "Kofniguration laden " - Konfiguration aus der Config-Datei laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Config_laden_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }
        /// <summary>
        /// Unterfunktion Config-Datei auslesen und in die TextBoxen schreiben
        /// </summary>
        private void LoadConfig()
        {
            String IP_Adresse = Config.ReadConfig("Z21_IP");
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

            IP_Port.Text = Config.ReadConfig("Z21_Port");

            if (Config.ReadConfig("Z21_Abos_Alle_Railcom").Equals("true")) Abo_AllRailCom.Checked = true;
            if (Config.ReadConfig("Z21_Abos_Railcom").Equals("true")) Abo_Railcom.Checked = true;
            if (Config.ReadConfig("Z21_Abos_Alle_Loks").Equals("true")) Abo_AllFahren.Checked = true;
            if (Config.ReadConfig("Z21_Abos_Loks").Equals("true")) Abo_Fahren.Checked = true;

            if (Config.ReadConfig("Z21_Abos_RMBus").Equals("true")) Abo_RMBus.Checked = true;
            if (Config.ReadConfig("Z21_Abos_CANBus").Equals("true")) Abo_CAN_detect.Checked = true;
            if (Config.ReadConfig("Z21_Abos_System_Status").Equals("true")) Abo_SystemStatus.Checked = true;

            if (Config.ReadConfig("Z21_Abos_LOCONET_Basic").Equals("true")) Abo_LOCONET_Basis.Checked = true;
            if (Config.ReadConfig("Z21_Abos_LOCONET_Loks").Equals("true")) Abo_LOCONET_Loks.Checked = true;
            if (Config.ReadConfig("Z21_Abos_LOCONET_Weichen").Equals("true")) Abo_LOCONET_Weichen.Checked = true;
            if (Config.ReadConfig("Z21_Abos_LOCONET_Detector").Equals("true")) Abo_LOCONET_detect.Checked = true;

            if (Config.ReadConfig("Auto_Connect").Equals("true")) AutoConnect.Checked = true;
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
        /// Button "Verbinden" - Verbindung aufbauen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_Connect_Click(object sender, EventArgs e)
        {
            z21Start.Connect_Z21();
        }
        /// <summary>
        /// Button "Trennen" - Verbindung trennen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Z21_DisConnect_Click(object sender, EventArgs e)
        {
            z21Start.DisConnect_Z21();
        }
        /// <summary>
        /// Schreiben eines Texts in die Textbox für Seriennummer
        /// </summary>
        /// <param name="data"></param>
        public void Set_SerienNummer(string data)
        {
            Seriennummer.Text = data;
        }
    }
}
