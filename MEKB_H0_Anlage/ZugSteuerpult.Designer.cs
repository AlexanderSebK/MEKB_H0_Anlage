namespace MEKB_H0_Anlage
{
    partial class ZugSteuerpult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZugSteuerpult));
            this.Fahrstufe = new System.Windows.Forms.TrackBar();
            this.Geschwindigkeit = new System.Windows.Forms.TextBox();
            this.Fkt_Licht = new System.Windows.Forms.Button();
            this.Fkt1 = new System.Windows.Forms.Button();
            this.Fkt2 = new System.Windows.Forms.Button();
            this.Fkt3 = new System.Windows.Forms.Button();
            this.Fkt4 = new System.Windows.Forms.Button();
            this.Fkt5 = new System.Windows.Forms.Button();
            this.Fkt6 = new System.Windows.Forms.Button();
            this.Fkt7 = new System.Windows.Forms.Button();
            this.Fkt8 = new System.Windows.Forms.Button();
            this.Fkt9 = new System.Windows.Forms.Button();
            this.Fkt10 = new System.Windows.Forms.Button();
            this.Fkt20 = new System.Windows.Forms.Button();
            this.Fkt19 = new System.Windows.Forms.Button();
            this.Fkt18 = new System.Windows.Forms.Button();
            this.Fkt17 = new System.Windows.Forms.Button();
            this.Fkt16 = new System.Windows.Forms.Button();
            this.Fkt15 = new System.Windows.Forms.Button();
            this.Fkt14 = new System.Windows.Forms.Button();
            this.Fkt13 = new System.Windows.Forms.Button();
            this.Fkt12 = new System.Windows.Forms.Button();
            this.Fkt11 = new System.Windows.Forms.Button();
            this.FktGroup = new System.Windows.Forms.GroupBox();
            this.Adresse = new System.Windows.Forms.Label();
            this.Lokname = new System.Windows.Forms.Label();
            this.Rufnummer = new System.Windows.Forms.Label();
            this.vor = new System.Windows.Forms.Button();
            this.Ruck = new System.Windows.Forms.Button();
            this.Anhalten = new System.Windows.Forms.Button();
            this.Notbremse = new System.Windows.Forms.Button();
            this.Fahrwechsel = new System.Windows.Forms.Button();
            this.Zusatzsteuerung = new System.Windows.Forms.GroupBox();
            this.StufenInfo = new System.Windows.Forms.ComboBox();
            this.Steuerung = new System.Windows.Forms.GroupBox();
            this.FahrAnzeige = new System.Windows.Forms.PictureBox();
            this.Fahrplan = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Position = new System.Windows.Forms.TextBox();
            this.Positon_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Fahrstufe)).BeginInit();
            this.FktGroup.SuspendLayout();
            this.Zusatzsteuerung.SuspendLayout();
            this.Steuerung.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FahrAnzeige)).BeginInit();
            this.Fahrplan.SuspendLayout();
            this.SuspendLayout();
            // 
            // Fahrstufe
            // 
            this.Fahrstufe.Location = new System.Drawing.Point(8, 16);
            this.Fahrstufe.Maximum = 128;
            this.Fahrstufe.Name = "Fahrstufe";
            this.Fahrstufe.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.Fahrstufe.Size = new System.Drawing.Size(45, 184);
            this.Fahrstufe.TabIndex = 0;
            this.Fahrstufe.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Fahrstufe.ValueChanged += new System.EventHandler(this.Fahrstufe_ValueChanged);
            // 
            // Geschwindigkeit
            // 
            this.Geschwindigkeit.Location = new System.Drawing.Point(8, 208);
            this.Geschwindigkeit.Name = "Geschwindigkeit";
            this.Geschwindigkeit.ReadOnly = true;
            this.Geschwindigkeit.Size = new System.Drawing.Size(96, 20);
            this.Geschwindigkeit.TabIndex = 1;
            // 
            // Fkt_Licht
            // 
            this.Fkt_Licht.Location = new System.Drawing.Point(8, 16);
            this.Fkt_Licht.Name = "Fkt_Licht";
            this.Fkt_Licht.Size = new System.Drawing.Size(75, 23);
            this.Fkt_Licht.TabIndex = 2;
            this.Fkt_Licht.Text = "Licht";
            this.Fkt_Licht.UseVisualStyleBackColor = true;
            this.Fkt_Licht.Click += new System.EventHandler(this.Fkt_Licht_Click);
            // 
            // Fkt1
            // 
            this.Fkt1.Location = new System.Drawing.Point(8, 40);
            this.Fkt1.Name = "Fkt1";
            this.Fkt1.Size = new System.Drawing.Size(75, 23);
            this.Fkt1.TabIndex = 3;
            this.Fkt1.Text = "Fkt. 1";
            this.Fkt1.UseVisualStyleBackColor = true;
            this.Fkt1.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt2
            // 
            this.Fkt2.Location = new System.Drawing.Point(8, 64);
            this.Fkt2.Name = "Fkt2";
            this.Fkt2.Size = new System.Drawing.Size(75, 23);
            this.Fkt2.TabIndex = 4;
            this.Fkt2.Text = "Fkt. 2";
            this.Fkt2.UseVisualStyleBackColor = true;
            this.Fkt2.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt3
            // 
            this.Fkt3.Location = new System.Drawing.Point(8, 88);
            this.Fkt3.Name = "Fkt3";
            this.Fkt3.Size = new System.Drawing.Size(75, 23);
            this.Fkt3.TabIndex = 5;
            this.Fkt3.Text = "Fkt. 3";
            this.Fkt3.UseVisualStyleBackColor = true;
            this.Fkt3.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt4
            // 
            this.Fkt4.Location = new System.Drawing.Point(8, 112);
            this.Fkt4.Name = "Fkt4";
            this.Fkt4.Size = new System.Drawing.Size(75, 23);
            this.Fkt4.TabIndex = 6;
            this.Fkt4.Text = "Fkt. 4";
            this.Fkt4.UseVisualStyleBackColor = true;
            this.Fkt4.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt5
            // 
            this.Fkt5.Location = new System.Drawing.Point(8, 136);
            this.Fkt5.Name = "Fkt5";
            this.Fkt5.Size = new System.Drawing.Size(75, 23);
            this.Fkt5.TabIndex = 7;
            this.Fkt5.Text = "Fkt. 5";
            this.Fkt5.UseVisualStyleBackColor = true;
            this.Fkt5.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt6
            // 
            this.Fkt6.Location = new System.Drawing.Point(88, 40);
            this.Fkt6.Name = "Fkt6";
            this.Fkt6.Size = new System.Drawing.Size(75, 23);
            this.Fkt6.TabIndex = 8;
            this.Fkt6.Text = "Fkt. 6";
            this.Fkt6.UseVisualStyleBackColor = true;
            this.Fkt6.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt7
            // 
            this.Fkt7.Location = new System.Drawing.Point(88, 64);
            this.Fkt7.Name = "Fkt7";
            this.Fkt7.Size = new System.Drawing.Size(75, 23);
            this.Fkt7.TabIndex = 9;
            this.Fkt7.Text = "Fkt. 7";
            this.Fkt7.UseVisualStyleBackColor = true;
            this.Fkt7.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt8
            // 
            this.Fkt8.Location = new System.Drawing.Point(88, 88);
            this.Fkt8.Name = "Fkt8";
            this.Fkt8.Size = new System.Drawing.Size(75, 23);
            this.Fkt8.TabIndex = 10;
            this.Fkt8.Text = "Fkt. 8";
            this.Fkt8.UseVisualStyleBackColor = true;
            this.Fkt8.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt9
            // 
            this.Fkt9.Location = new System.Drawing.Point(88, 112);
            this.Fkt9.Name = "Fkt9";
            this.Fkt9.Size = new System.Drawing.Size(75, 23);
            this.Fkt9.TabIndex = 11;
            this.Fkt9.Text = "Fkt. 9";
            this.Fkt9.UseVisualStyleBackColor = true;
            this.Fkt9.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt10
            // 
            this.Fkt10.Location = new System.Drawing.Point(88, 136);
            this.Fkt10.Name = "Fkt10";
            this.Fkt10.Size = new System.Drawing.Size(75, 23);
            this.Fkt10.TabIndex = 12;
            this.Fkt10.Text = "Fkt. 10";
            this.Fkt10.UseVisualStyleBackColor = true;
            this.Fkt10.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt20
            // 
            this.Fkt20.Location = new System.Drawing.Point(248, 136);
            this.Fkt20.Name = "Fkt20";
            this.Fkt20.Size = new System.Drawing.Size(75, 23);
            this.Fkt20.TabIndex = 22;
            this.Fkt20.Text = "Fkt. 20";
            this.Fkt20.UseVisualStyleBackColor = true;
            this.Fkt20.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt19
            // 
            this.Fkt19.Location = new System.Drawing.Point(248, 112);
            this.Fkt19.Name = "Fkt19";
            this.Fkt19.Size = new System.Drawing.Size(75, 23);
            this.Fkt19.TabIndex = 21;
            this.Fkt19.Text = "Fkt. 19";
            this.Fkt19.UseVisualStyleBackColor = true;
            this.Fkt19.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt18
            // 
            this.Fkt18.Location = new System.Drawing.Point(248, 88);
            this.Fkt18.Name = "Fkt18";
            this.Fkt18.Size = new System.Drawing.Size(75, 23);
            this.Fkt18.TabIndex = 20;
            this.Fkt18.Text = "Fkt. 18";
            this.Fkt18.UseVisualStyleBackColor = true;
            this.Fkt18.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt17
            // 
            this.Fkt17.Location = new System.Drawing.Point(248, 64);
            this.Fkt17.Name = "Fkt17";
            this.Fkt17.Size = new System.Drawing.Size(75, 23);
            this.Fkt17.TabIndex = 19;
            this.Fkt17.Text = "Fkt. 17";
            this.Fkt17.UseVisualStyleBackColor = true;
            this.Fkt17.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt16
            // 
            this.Fkt16.Location = new System.Drawing.Point(248, 40);
            this.Fkt16.Name = "Fkt16";
            this.Fkt16.Size = new System.Drawing.Size(75, 23);
            this.Fkt16.TabIndex = 18;
            this.Fkt16.Text = "Fkt. 16";
            this.Fkt16.UseVisualStyleBackColor = true;
            this.Fkt16.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt15
            // 
            this.Fkt15.Location = new System.Drawing.Point(168, 136);
            this.Fkt15.Name = "Fkt15";
            this.Fkt15.Size = new System.Drawing.Size(75, 23);
            this.Fkt15.TabIndex = 17;
            this.Fkt15.Text = "Fkt. 15";
            this.Fkt15.UseVisualStyleBackColor = true;
            this.Fkt15.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt14
            // 
            this.Fkt14.Location = new System.Drawing.Point(168, 112);
            this.Fkt14.Name = "Fkt14";
            this.Fkt14.Size = new System.Drawing.Size(75, 23);
            this.Fkt14.TabIndex = 16;
            this.Fkt14.Text = "Fkt. 14";
            this.Fkt14.UseVisualStyleBackColor = true;
            this.Fkt14.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt13
            // 
            this.Fkt13.Location = new System.Drawing.Point(168, 88);
            this.Fkt13.Name = "Fkt13";
            this.Fkt13.Size = new System.Drawing.Size(75, 23);
            this.Fkt13.TabIndex = 15;
            this.Fkt13.Text = "Fkt. 13";
            this.Fkt13.UseVisualStyleBackColor = true;
            this.Fkt13.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt12
            // 
            this.Fkt12.Location = new System.Drawing.Point(168, 64);
            this.Fkt12.Name = "Fkt12";
            this.Fkt12.Size = new System.Drawing.Size(75, 23);
            this.Fkt12.TabIndex = 14;
            this.Fkt12.Text = "Fkt. 12";
            this.Fkt12.UseVisualStyleBackColor = true;
            this.Fkt12.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // Fkt11
            // 
            this.Fkt11.Location = new System.Drawing.Point(168, 40);
            this.Fkt11.Name = "Fkt11";
            this.Fkt11.Size = new System.Drawing.Size(75, 23);
            this.Fkt11.TabIndex = 13;
            this.Fkt11.Text = "Fkt. 11";
            this.Fkt11.UseVisualStyleBackColor = true;
            this.Fkt11.Click += new System.EventHandler(this.Fkt_Set_Click);
            // 
            // FktGroup
            // 
            this.FktGroup.Controls.Add(this.Fkt1);
            this.FktGroup.Controls.Add(this.Fkt20);
            this.FktGroup.Controls.Add(this.Fkt2);
            this.FktGroup.Controls.Add(this.Fkt19);
            this.FktGroup.Controls.Add(this.Fkt_Licht);
            this.FktGroup.Controls.Add(this.Fkt3);
            this.FktGroup.Controls.Add(this.Fkt18);
            this.FktGroup.Controls.Add(this.Fkt4);
            this.FktGroup.Controls.Add(this.Fkt17);
            this.FktGroup.Controls.Add(this.Fkt5);
            this.FktGroup.Controls.Add(this.Fkt16);
            this.FktGroup.Controls.Add(this.Fkt6);
            this.FktGroup.Controls.Add(this.Fkt15);
            this.FktGroup.Controls.Add(this.Fkt7);
            this.FktGroup.Controls.Add(this.Fkt14);
            this.FktGroup.Controls.Add(this.Fkt8);
            this.FktGroup.Controls.Add(this.Fkt13);
            this.FktGroup.Controls.Add(this.Fkt9);
            this.FktGroup.Controls.Add(this.Fkt12);
            this.FktGroup.Controls.Add(this.Fkt10);
            this.FktGroup.Controls.Add(this.Fkt11);
            this.FktGroup.Location = new System.Drawing.Point(128, 56);
            this.FktGroup.Name = "FktGroup";
            this.FktGroup.Size = new System.Drawing.Size(328, 168);
            this.FktGroup.TabIndex = 23;
            this.FktGroup.TabStop = false;
            this.FktGroup.Text = "Funktionen";
            // 
            // Adresse
            // 
            this.Adresse.AutoSize = true;
            this.Adresse.Location = new System.Drawing.Point(144, 32);
            this.Adresse.Name = "Adresse";
            this.Adresse.Size = new System.Drawing.Size(71, 13);
            this.Adresse.TabIndex = 25;
            this.Adresse.Text = "Adresse: xxxx";
            // 
            // Lokname
            // 
            this.Lokname.AutoSize = true;
            this.Lokname.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lokname.Location = new System.Drawing.Point(144, 8);
            this.Lokname.Name = "Lokname";
            this.Lokname.Size = new System.Drawing.Size(76, 19);
            this.Lokname.TabIndex = 26;
            this.Lokname.Text = "Lokname";
            // 
            // Rufnummer
            // 
            this.Rufnummer.AutoSize = true;
            this.Rufnummer.Location = new System.Drawing.Point(328, 32);
            this.Rufnummer.Name = "Rufnummer";
            this.Rufnummer.Size = new System.Drawing.Size(105, 13);
            this.Rufnummer.TabIndex = 27;
            this.Rufnummer.Text = "Rufnummer: gggxxxx";
            // 
            // vor
            // 
            this.vor.Location = new System.Drawing.Point(8, 232);
            this.vor.Name = "vor";
            this.vor.Size = new System.Drawing.Size(48, 23);
            this.vor.TabIndex = 28;
            this.vor.Text = "Vor";
            this.vor.UseVisualStyleBackColor = true;
            this.vor.Click += new System.EventHandler(this.vor_Click);
            // 
            // Ruck
            // 
            this.Ruck.Location = new System.Drawing.Point(56, 232);
            this.Ruck.Name = "Ruck";
            this.Ruck.Size = new System.Drawing.Size(48, 23);
            this.Ruck.TabIndex = 29;
            this.Ruck.Text = "Rück";
            this.Ruck.UseVisualStyleBackColor = true;
            this.Ruck.Click += new System.EventHandler(this.Ruck_Click);
            // 
            // Anhalten
            // 
            this.Anhalten.Location = new System.Drawing.Point(8, 16);
            this.Anhalten.Name = "Anhalten";
            this.Anhalten.Size = new System.Drawing.Size(75, 23);
            this.Anhalten.TabIndex = 30;
            this.Anhalten.Text = "Anhalten";
            this.Anhalten.UseVisualStyleBackColor = true;
            this.Anhalten.Click += new System.EventHandler(this.Anhalten_Click);
            // 
            // Notbremse
            // 
            this.Notbremse.Location = new System.Drawing.Point(88, 16);
            this.Notbremse.Name = "Notbremse";
            this.Notbremse.Size = new System.Drawing.Size(75, 23);
            this.Notbremse.TabIndex = 31;
            this.Notbremse.Text = "Notbremse";
            this.Notbremse.UseVisualStyleBackColor = true;
            this.Notbremse.Click += new System.EventHandler(this.Notbremse_Click);
            // 
            // Fahrwechsel
            // 
            this.Fahrwechsel.Location = new System.Drawing.Point(168, 16);
            this.Fahrwechsel.Name = "Fahrwechsel";
            this.Fahrwechsel.Size = new System.Drawing.Size(75, 23);
            this.Fahrwechsel.TabIndex = 32;
            this.Fahrwechsel.Text = "Fahrwechsel";
            this.Fahrwechsel.UseVisualStyleBackColor = true;
            this.Fahrwechsel.Click += new System.EventHandler(this.Fahrwechsel_Click);
            // 
            // Zusatzsteuerung
            // 
            this.Zusatzsteuerung.Controls.Add(this.StufenInfo);
            this.Zusatzsteuerung.Controls.Add(this.Anhalten);
            this.Zusatzsteuerung.Controls.Add(this.Notbremse);
            this.Zusatzsteuerung.Controls.Add(this.Fahrwechsel);
            this.Zusatzsteuerung.Location = new System.Drawing.Point(128, 224);
            this.Zusatzsteuerung.Name = "Zusatzsteuerung";
            this.Zusatzsteuerung.Size = new System.Drawing.Size(328, 48);
            this.Zusatzsteuerung.TabIndex = 34;
            this.Zusatzsteuerung.TabStop = false;
            this.Zusatzsteuerung.Text = "Zusatzsteuerung";
            // 
            // StufenInfo
            // 
            this.StufenInfo.FormattingEnabled = true;
            this.StufenInfo.Items.AddRange(new object[] {
            "F14",
            "F28",
            "F128"});
            this.StufenInfo.Location = new System.Drawing.Point(248, 16);
            this.StufenInfo.Name = "StufenInfo";
            this.StufenInfo.Size = new System.Drawing.Size(72, 21);
            this.StufenInfo.TabIndex = 33;
            this.StufenInfo.SelectedIndexChanged += new System.EventHandler(this.StufenInfo_SelectedIndexChanged);
            // 
            // Steuerung
            // 
            this.Steuerung.Controls.Add(this.FahrAnzeige);
            this.Steuerung.Controls.Add(this.Fahrstufe);
            this.Steuerung.Controls.Add(this.Geschwindigkeit);
            this.Steuerung.Controls.Add(this.Ruck);
            this.Steuerung.Controls.Add(this.vor);
            this.Steuerung.Location = new System.Drawing.Point(8, 8);
            this.Steuerung.Name = "Steuerung";
            this.Steuerung.Size = new System.Drawing.Size(112, 264);
            this.Steuerung.TabIndex = 35;
            this.Steuerung.TabStop = false;
            this.Steuerung.Text = "Steuerung";
            // 
            // FahrAnzeige
            // 
            this.FahrAnzeige.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.FahrAnzeige.Image = ((System.Drawing.Image)(resources.GetObject("FahrAnzeige.Image")));
            this.FahrAnzeige.InitialImage = ((System.Drawing.Image)(resources.GetObject("FahrAnzeige.InitialImage")));
            this.FahrAnzeige.Location = new System.Drawing.Point(40, 21);
            this.FahrAnzeige.Name = "FahrAnzeige";
            this.FahrAnzeige.Size = new System.Drawing.Size(64, 168);
            this.FahrAnzeige.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.FahrAnzeige.TabIndex = 30;
            this.FahrAnzeige.TabStop = false;
            // 
            // Fahrplan
            // 
            this.Fahrplan.Controls.Add(this.textBox2);
            this.Fahrplan.Controls.Add(this.label2);
            this.Fahrplan.Controls.Add(this.textBox3);
            this.Fahrplan.Controls.Add(this.label1);
            this.Fahrplan.Controls.Add(this.Position);
            this.Fahrplan.Controls.Add(this.Positon_Label);
            this.Fahrplan.Location = new System.Drawing.Point(8, 272);
            this.Fahrplan.Name = "Fahrplan";
            this.Fahrplan.Size = new System.Drawing.Size(448, 64);
            this.Fahrplan.TabIndex = 36;
            this.Fahrplan.TabStop = false;
            this.Fahrplan.Text = "Fahrplan";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(368, 16);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(72, 20);
            this.textBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Nächstes Signal";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(80, 40);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(128, 20);
            this.textBox3.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ziel";
            // 
            // Position
            // 
            this.Position.Enabled = false;
            this.Position.Location = new System.Drawing.Point(80, 16);
            this.Position.Name = "Position";
            this.Position.Size = new System.Drawing.Size(128, 20);
            this.Position.TabIndex = 1;
            // 
            // Positon_Label
            // 
            this.Positon_Label.AutoSize = true;
            this.Positon_Label.Location = new System.Drawing.Point(8, 20);
            this.Positon_Label.Name = "Positon_Label";
            this.Positon_Label.Size = new System.Drawing.Size(66, 13);
            this.Positon_Label.TabIndex = 0;
            this.Positon_Label.Text = "Akt. Position";
            // 
            // ZugSteuerpult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 342);
            this.Controls.Add(this.Fahrplan);
            this.Controls.Add(this.Steuerung);
            this.Controls.Add(this.Zusatzsteuerung);
            this.Controls.Add(this.Rufnummer);
            this.Controls.Add(this.Lokname);
            this.Controls.Add(this.Adresse);
            this.Controls.Add(this.FktGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ZugSteuerpult";
            this.Text = "ZugSteuerpult";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ZugSteuerpult_Load);
            this.Shown += new System.EventHandler(this.ZugSteuerpult_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Fahrstufe)).EndInit();
            this.FktGroup.ResumeLayout(false);
            this.Zusatzsteuerung.ResumeLayout(false);
            this.Steuerung.ResumeLayout(false);
            this.Steuerung.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FahrAnzeige)).EndInit();
            this.Fahrplan.ResumeLayout(false);
            this.Fahrplan.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar Fahrstufe;
        private System.Windows.Forms.TextBox Geschwindigkeit;
        private System.Windows.Forms.Button Fkt_Licht;
        private System.Windows.Forms.Button Fkt1;
        private System.Windows.Forms.Button Fkt2;
        private System.Windows.Forms.Button Fkt3;
        private System.Windows.Forms.Button Fkt4;
        private System.Windows.Forms.Button Fkt5;
        private System.Windows.Forms.Button Fkt6;
        private System.Windows.Forms.Button Fkt7;
        private System.Windows.Forms.Button Fkt8;
        private System.Windows.Forms.Button Fkt9;
        private System.Windows.Forms.Button Fkt10;
        private System.Windows.Forms.Button Fkt20;
        private System.Windows.Forms.Button Fkt19;
        private System.Windows.Forms.Button Fkt18;
        private System.Windows.Forms.Button Fkt17;
        private System.Windows.Forms.Button Fkt16;
        private System.Windows.Forms.Button Fkt15;
        private System.Windows.Forms.Button Fkt14;
        private System.Windows.Forms.Button Fkt13;
        private System.Windows.Forms.Button Fkt12;
        private System.Windows.Forms.Button Fkt11;
        private System.Windows.Forms.GroupBox FktGroup;
        private System.Windows.Forms.Label Adresse;
        private System.Windows.Forms.Label Lokname;
        private System.Windows.Forms.Label Rufnummer;
        private System.Windows.Forms.Button vor;
        private System.Windows.Forms.Button Ruck;
        private System.Windows.Forms.Button Anhalten;
        private System.Windows.Forms.Button Notbremse;
        private System.Windows.Forms.Button Fahrwechsel;
        private System.Windows.Forms.GroupBox Zusatzsteuerung;
        private System.Windows.Forms.GroupBox Steuerung;
        private System.Windows.Forms.PictureBox FahrAnzeige;
        private System.Windows.Forms.GroupBox Fahrplan;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Position;
        private System.Windows.Forms.Label Positon_Label;
        private System.Windows.Forms.ComboBox StufenInfo;
    }
}