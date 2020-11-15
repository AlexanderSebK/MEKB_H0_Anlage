namespace MEKB_H0_Anlage
{
    partial class Z21_Einstellung
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Firmware = new System.Windows.Forms.TextBox();
            this.Z21_Typ = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Z21_Eigenschaften_OK = new System.Windows.Forms.Button();
            this.Z21_Update = new System.Windows.Forms.Button();
            this.Z21_Get = new System.Windows.Forms.Button();
            this.Config_laden = new System.Windows.Forms.Button();
            this.Config_save = new System.Windows.Forms.Button();
            this.Abo_Fahren = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Abo_RMBus = new System.Windows.Forms.CheckBox();
            this.Abo_Railcom = new System.Windows.Forms.CheckBox();
            this.Abo_SystemStatus = new System.Windows.Forms.CheckBox();
            this.Abo_AllFahren = new System.Windows.Forms.CheckBox();
            this.Abo_LOCONET_Basis = new System.Windows.Forms.CheckBox();
            this.Abo_LOCONET_Loks = new System.Windows.Forms.CheckBox();
            this.Abo_LOCONET_Weichen = new System.Windows.Forms.CheckBox();
            this.Abo_LOCONET_detect = new System.Windows.Forms.CheckBox();
            this.Abo_AllRailCom = new System.Windows.Forms.CheckBox();
            this.Abo_CAN_detect = new System.Windows.Forms.CheckBox();
            this.Abos = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.IP_1 = new System.Windows.Forms.TextBox();
            this.IP_2 = new System.Windows.Forms.TextBox();
            this.IP_3 = new System.Windows.Forms.TextBox();
            this.IP_4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.IP_Port = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Z21_Connect = new System.Windows.Forms.Button();
            this.Z21_DisConnect = new System.Windows.Forms.Button();
            this.AutoConnect = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Seriennummer = new System.Windows.Forms.TextBox();
            this.Abos.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Firmware
            // 
            this.Firmware.Location = new System.Drawing.Point(314, 17);
            this.Firmware.Name = "Firmware";
            this.Firmware.ReadOnly = true;
            this.Firmware.Size = new System.Drawing.Size(47, 20);
            this.Firmware.TabIndex = 0;
            // 
            // Z21_Typ
            // 
            this.Z21_Typ.Location = new System.Drawing.Point(407, 17);
            this.Z21_Typ.Name = "Z21_Typ";
            this.Z21_Typ.ReadOnly = true;
            this.Z21_Typ.Size = new System.Drawing.Size(61, 20);
            this.Z21_Typ.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Typ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(234, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Firmware";
            // 
            // Z21_Eigenschaften_OK
            // 
            this.Z21_Eigenschaften_OK.Location = new System.Drawing.Point(370, 297);
            this.Z21_Eigenschaften_OK.Name = "Z21_Eigenschaften_OK";
            this.Z21_Eigenschaften_OK.Size = new System.Drawing.Size(121, 23);
            this.Z21_Eigenschaften_OK.TabIndex = 2;
            this.Z21_Eigenschaften_OK.Text = "OK";
            this.Z21_Eigenschaften_OK.UseVisualStyleBackColor = true;
            this.Z21_Eigenschaften_OK.Click += new System.EventHandler(this.Z21_Eigenschaften_OK_Click);
            // 
            // Z21_Update
            // 
            this.Z21_Update.Location = new System.Drawing.Point(155, 239);
            this.Z21_Update.Name = "Z21_Update";
            this.Z21_Update.Size = new System.Drawing.Size(140, 23);
            this.Z21_Update.TabIndex = 3;
            this.Z21_Update.Text = "Werte schreiben";
            this.Z21_Update.UseVisualStyleBackColor = true;
            this.Z21_Update.Click += new System.EventHandler(this.Z21_Update_Click);
            // 
            // Z21_Get
            // 
            this.Z21_Get.Location = new System.Drawing.Point(155, 268);
            this.Z21_Get.Name = "Z21_Get";
            this.Z21_Get.Size = new System.Drawing.Size(140, 23);
            this.Z21_Get.TabIndex = 4;
            this.Z21_Get.Text = "Werte lesen";
            this.Z21_Get.UseVisualStyleBackColor = true;
            this.Z21_Get.Click += new System.EventHandler(this.Z21_Get_Click);
            // 
            // Config_laden
            // 
            this.Config_laden.Location = new System.Drawing.Point(9, 268);
            this.Config_laden.Name = "Config_laden";
            this.Config_laden.Size = new System.Drawing.Size(140, 23);
            this.Config_laden.TabIndex = 5;
            this.Config_laden.Text = "Konfiguration laden";
            this.toolTip1.SetToolTip(this.Config_laden, "Konfiguration aus der Datei laden ");
            this.Config_laden.UseVisualStyleBackColor = true;
            this.Config_laden.Click += new System.EventHandler(this.Config_laden_Click);
            // 
            // Config_save
            // 
            this.Config_save.Location = new System.Drawing.Point(9, 239);
            this.Config_save.Name = "Config_save";
            this.Config_save.Size = new System.Drawing.Size(140, 23);
            this.Config_save.TabIndex = 6;
            this.Config_save.Text = "Konfiguration speichern";
            this.toolTip1.SetToolTip(this.Config_save, "Aktuelle Konfiguration in eine Datei speichern");
            this.Config_save.UseVisualStyleBackColor = true;
            this.Config_save.Click += new System.EventHandler(this.Config_save_Click);
            // 
            // Abo_Fahren
            // 
            this.Abo_Fahren.AutoSize = true;
            this.Abo_Fahren.Location = new System.Drawing.Point(6, 19);
            this.Abo_Fahren.Name = "Abo_Fahren";
            this.Abo_Fahren.Size = new System.Drawing.Size(151, 17);
            this.Abo_Fahren.TabIndex = 7;
            this.Abo_Fahren.Text = "eigene Loks und Weichen";
            this.toolTip1.SetToolTip(this.Abo_Fahren, "Automatisches Empfangen von Lokdaten und Weichen, wenn diese sich verändert haben" +
        " \r\n(nur für eigene angesteuerte Lokomotiven/Weichen)");
            this.Abo_Fahren.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.OwnerDraw = true;
            // 
            // Abo_RMBus
            // 
            this.Abo_RMBus.AutoSize = true;
            this.Abo_RMBus.Location = new System.Drawing.Point(188, 19);
            this.Abo_RMBus.Name = "Abo_RMBus";
            this.Abo_RMBus.Size = new System.Drawing.Size(101, 17);
            this.Abo_RMBus.TabIndex = 8;
            this.Abo_RMBus.Text = "Rückmelde Bus";
            this.toolTip1.SetToolTip(this.Abo_RMBus, "Daten von Rückmeldebus automatisch Empfangen ohne Anforderung");
            this.Abo_RMBus.UseVisualStyleBackColor = true;
            // 
            // Abo_Railcom
            // 
            this.Abo_Railcom.AutoSize = true;
            this.Abo_Railcom.Location = new System.Drawing.Point(6, 65);
            this.Abo_Railcom.Name = "Abo_Railcom";
            this.Abo_Railcom.Size = new System.Drawing.Size(65, 17);
            this.Abo_Railcom.TabIndex = 9;
            this.Abo_Railcom.Text = "RailCom";
            this.toolTip1.SetToolTip(this.Abo_Railcom, "Daten von RailCom automatisch Empfangen ohne Anforderung\r\n(Nur eigene Loks)");
            this.Abo_Railcom.UseVisualStyleBackColor = true;
            // 
            // Abo_SystemStatus
            // 
            this.Abo_SystemStatus.AutoSize = true;
            this.Abo_SystemStatus.Location = new System.Drawing.Point(188, 42);
            this.Abo_SystemStatus.Name = "Abo_SystemStatus";
            this.Abo_SystemStatus.Size = new System.Drawing.Size(116, 17);
            this.Abo_SystemStatus.TabIndex = 10;
            this.Abo_SystemStatus.Text = "Status der Zentrale";
            this.toolTip1.SetToolTip(this.Abo_SystemStatus, "Eigenschaften der Zentrale bei Änderungen automatisch Empfangen ohne Anforderung");
            this.Abo_SystemStatus.UseVisualStyleBackColor = true;
            // 
            // Abo_AllFahren
            // 
            this.Abo_AllFahren.AutoSize = true;
            this.Abo_AllFahren.Location = new System.Drawing.Point(6, 42);
            this.Abo_AllFahren.Name = "Abo_AllFahren";
            this.Abo_AllFahren.Size = new System.Drawing.Size(136, 17);
            this.Abo_AllFahren.TabIndex = 11;
            this.Abo_AllFahren.Text = "Alle Loks und Weichen";
            this.toolTip1.SetToolTip(this.Abo_AllFahren, "Automatisches Empfangen von Nachrichten von allen Loks und Weichen, wenn sich Wer" +
        "te geändert haben\r\n(Alle Loks und Weichen)");
            this.Abo_AllFahren.UseVisualStyleBackColor = true;
            // 
            // Abo_LOCONET_Basis
            // 
            this.Abo_LOCONET_Basis.AutoSize = true;
            this.Abo_LOCONET_Basis.Location = new System.Drawing.Point(337, 19);
            this.Abo_LOCONET_Basis.Name = "Abo_LOCONET_Basis";
            this.Abo_LOCONET_Basis.Size = new System.Drawing.Size(111, 17);
            this.Abo_LOCONET_Basis.TabIndex = 12;
            this.Abo_LOCONET_Basis.Text = "LOCONET Basics";
            this.toolTip1.SetToolTip(this.Abo_LOCONET_Basis, "Automatisches Empfangen von LOCONET Nachrichten.\r\n(Nur Systemeigenschaften)");
            this.Abo_LOCONET_Basis.UseVisualStyleBackColor = true;
            // 
            // Abo_LOCONET_Loks
            // 
            this.Abo_LOCONET_Loks.AutoSize = true;
            this.Abo_LOCONET_Loks.Location = new System.Drawing.Point(337, 42);
            this.Abo_LOCONET_Loks.Name = "Abo_LOCONET_Loks";
            this.Abo_LOCONET_Loks.Size = new System.Drawing.Size(103, 17);
            this.Abo_LOCONET_Loks.TabIndex = 13;
            this.Abo_LOCONET_Loks.Text = "LOCONET Loks";
            this.toolTip1.SetToolTip(this.Abo_LOCONET_Loks, "Automatisches Empfangen von LOCONET Nachrichten.\r\n(Nur Loks)");
            this.Abo_LOCONET_Loks.UseVisualStyleBackColor = true;
            // 
            // Abo_LOCONET_Weichen
            // 
            this.Abo_LOCONET_Weichen.AutoSize = true;
            this.Abo_LOCONET_Weichen.Location = new System.Drawing.Point(337, 65);
            this.Abo_LOCONET_Weichen.Name = "Abo_LOCONET_Weichen";
            this.Abo_LOCONET_Weichen.Size = new System.Drawing.Size(123, 17);
            this.Abo_LOCONET_Weichen.TabIndex = 14;
            this.Abo_LOCONET_Weichen.Text = "LOCONET Weichen";
            this.toolTip1.SetToolTip(this.Abo_LOCONET_Weichen, "Automatisches Empfangen von LOCONET Nachrichten.\r\n(Nur Weichen)");
            this.Abo_LOCONET_Weichen.UseVisualStyleBackColor = true;
            // 
            // Abo_LOCONET_detect
            // 
            this.Abo_LOCONET_detect.AutoSize = true;
            this.Abo_LOCONET_detect.Location = new System.Drawing.Point(337, 88);
            this.Abo_LOCONET_detect.Name = "Abo_LOCONET_detect";
            this.Abo_LOCONET_detect.Size = new System.Drawing.Size(137, 17);
            this.Abo_LOCONET_detect.TabIndex = 15;
            this.Abo_LOCONET_detect.Text = "LOCONET Rückmelder";
            this.toolTip1.SetToolTip(this.Abo_LOCONET_detect, "Automatisches Empfangen von LOCONET Nachrichten.\r\n(Nur Rückmelder)");
            this.Abo_LOCONET_detect.UseVisualStyleBackColor = true;
            // 
            // Abo_AllRailCom
            // 
            this.Abo_AllRailCom.AutoSize = true;
            this.Abo_AllRailCom.Location = new System.Drawing.Point(6, 88);
            this.Abo_AllRailCom.Name = "Abo_AllRailCom";
            this.Abo_AllRailCom.Size = new System.Drawing.Size(85, 17);
            this.Abo_AllRailCom.TabIndex = 16;
            this.Abo_AllRailCom.Text = "Alle RailCom";
            this.toolTip1.SetToolTip(this.Abo_AllRailCom, "Daten von RailCom automatisch Empfangen ohne Anforderung\r\n(AlleLoks)\r\n");
            this.Abo_AllRailCom.UseVisualStyleBackColor = true;
            // 
            // Abo_CAN_detect
            // 
            this.Abo_CAN_detect.AutoSize = true;
            this.Abo_CAN_detect.Location = new System.Drawing.Point(188, 65);
            this.Abo_CAN_detect.Name = "Abo_CAN_detect";
            this.Abo_CAN_detect.Size = new System.Drawing.Size(108, 17);
            this.Abo_CAN_detect.TabIndex = 17;
            this.Abo_CAN_detect.Text = "CAN Rückmelder";
            this.toolTip1.SetToolTip(this.Abo_CAN_detect, "Automatisches Empfangen von Nachrichten durch einen Rückmelder am CAN-Bus \r\n(nur " +
        "schwarze Z21)");
            this.Abo_CAN_detect.UseVisualStyleBackColor = true;
            // 
            // Abos
            // 
            this.Abos.Controls.Add(this.Abo_Fahren);
            this.Abos.Controls.Add(this.Abo_LOCONET_detect);
            this.Abos.Controls.Add(this.Abo_CAN_detect);
            this.Abos.Controls.Add(this.Abo_LOCONET_Weichen);
            this.Abos.Controls.Add(this.Abo_AllFahren);
            this.Abos.Controls.Add(this.Abo_LOCONET_Loks);
            this.Abos.Controls.Add(this.Abo_AllRailCom);
            this.Abos.Controls.Add(this.Abo_LOCONET_Basis);
            this.Abos.Controls.Add(this.Abo_Railcom);
            this.Abos.Controls.Add(this.Abo_RMBus);
            this.Abos.Controls.Add(this.Abo_SystemStatus);
            this.Abos.Location = new System.Drawing.Point(12, 118);
            this.Abos.Name = "Abos";
            this.Abos.Size = new System.Drawing.Size(479, 115);
            this.Abos.TabIndex = 18;
            this.Abos.TabStop = false;
            this.Abos.Text = "Empfangen von Nachrichten ohne Anforderung";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Seriennummer);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.AutoConnect);
            this.groupBox2.Controls.Add(this.Z21_Typ);
            this.groupBox2.Controls.Add(this.Z21_DisConnect);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.Z21_Connect);
            this.groupBox2.Controls.Add(this.Firmware);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.IP_Port);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.IP_4);
            this.groupBox2.Controls.Add(this.IP_3);
            this.groupBox2.Controls.Add(this.IP_2);
            this.groupBox2.Controls.Add(this.IP_1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(474, 100);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IP-Verbindung";
            // 
            // IP_1
            // 
            this.IP_1.Location = new System.Drawing.Point(75, 17);
            this.IP_1.Name = "IP_1";
            this.IP_1.Size = new System.Drawing.Size(28, 20);
            this.IP_1.TabIndex = 0;
            // 
            // IP_2
            // 
            this.IP_2.Location = new System.Drawing.Point(109, 17);
            this.IP_2.Name = "IP_2";
            this.IP_2.Size = new System.Drawing.Size(28, 20);
            this.IP_2.TabIndex = 1;
            // 
            // IP_3
            // 
            this.IP_3.Location = new System.Drawing.Point(143, 17);
            this.IP_3.Name = "IP_3";
            this.IP_3.Size = new System.Drawing.Size(28, 20);
            this.IP_3.TabIndex = 2;
            // 
            // IP_4
            // 
            this.IP_4.Location = new System.Drawing.Point(177, 17);
            this.IP_4.Name = "IP_4";
            this.IP_4.Size = new System.Drawing.Size(28, 20);
            this.IP_4.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "IP-Adresse";
            // 
            // IP_Port
            // 
            this.IP_Port.Location = new System.Drawing.Point(143, 41);
            this.IP_Port.Name = "IP_Port";
            this.IP_Port.Size = new System.Drawing.Size(62, 20);
            this.IP_Port.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "IP-Port";
            // 
            // Z21_Connect
            // 
            this.Z21_Connect.Location = new System.Drawing.Point(6, 67);
            this.Z21_Connect.Name = "Z21_Connect";
            this.Z21_Connect.Size = new System.Drawing.Size(97, 23);
            this.Z21_Connect.TabIndex = 7;
            this.Z21_Connect.Text = "Verbinden";
            this.Z21_Connect.UseVisualStyleBackColor = true;
            this.Z21_Connect.Click += new System.EventHandler(this.Z21_Connect_Click);
            // 
            // Z21_DisConnect
            // 
            this.Z21_DisConnect.Location = new System.Drawing.Point(109, 67);
            this.Z21_DisConnect.Name = "Z21_DisConnect";
            this.Z21_DisConnect.Size = new System.Drawing.Size(96, 23);
            this.Z21_DisConnect.TabIndex = 20;
            this.Z21_DisConnect.Text = "Trennen";
            this.Z21_DisConnect.UseVisualStyleBackColor = true;
            this.Z21_DisConnect.Click += new System.EventHandler(this.Z21_DisConnect_Click);
            // 
            // AutoConnect
            // 
            this.AutoConnect.AutoSize = true;
            this.AutoConnect.Location = new System.Drawing.Point(237, 71);
            this.AutoConnect.Name = "AutoConnect";
            this.AutoConnect.Size = new System.Drawing.Size(221, 17);
            this.AutoConnect.TabIndex = 21;
            this.AutoConnect.Text = "Automatisch bei Programmstart verbinden";
            this.AutoConnect.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(234, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Seriennummer";
            // 
            // Seriennummer
            // 
            this.Seriennummer.Location = new System.Drawing.Point(314, 41);
            this.Seriennummer.Name = "Seriennummer";
            this.Seriennummer.ReadOnly = true;
            this.Seriennummer.Size = new System.Drawing.Size(87, 20);
            this.Seriennummer.TabIndex = 23;
            // 
            // Z21_Einstellung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 328);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Abos);
            this.Controls.Add(this.Config_save);
            this.Controls.Add(this.Config_laden);
            this.Controls.Add(this.Z21_Get);
            this.Controls.Add(this.Z21_Update);
            this.Controls.Add(this.Z21_Eigenschaften_OK);
            this.Name = "Z21_Einstellung";
            this.Text = "Z21_Einstellung";
            this.Abos.ResumeLayout(false);
            this.Abos.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Firmware;
        private System.Windows.Forms.TextBox Z21_Typ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Z21_Eigenschaften_OK;
        private System.Windows.Forms.Button Z21_Update;
        private System.Windows.Forms.Button Z21_Get;
        private System.Windows.Forms.Button Config_laden;
        private System.Windows.Forms.Button Config_save;
        private System.Windows.Forms.CheckBox Abo_Fahren;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox Abo_RMBus;
        private System.Windows.Forms.CheckBox Abo_Railcom;
        private System.Windows.Forms.CheckBox Abo_SystemStatus;
        private System.Windows.Forms.CheckBox Abo_AllFahren;
        private System.Windows.Forms.CheckBox Abo_LOCONET_Basis;
        private System.Windows.Forms.CheckBox Abo_LOCONET_Loks;
        private System.Windows.Forms.CheckBox Abo_LOCONET_Weichen;
        private System.Windows.Forms.CheckBox Abo_LOCONET_detect;
        private System.Windows.Forms.CheckBox Abo_AllRailCom;
        private System.Windows.Forms.CheckBox Abo_CAN_detect;
        private System.Windows.Forms.GroupBox Abos;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Z21_DisConnect;
        private System.Windows.Forms.Button Z21_Connect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox IP_Port;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox IP_4;
        private System.Windows.Forms.TextBox IP_3;
        private System.Windows.Forms.TextBox IP_2;
        private System.Windows.Forms.TextBox IP_1;
        private System.Windows.Forms.CheckBox AutoConnect;
        private System.Windows.Forms.TextBox Seriennummer;
        private System.Windows.Forms.Label label2;
    }
}