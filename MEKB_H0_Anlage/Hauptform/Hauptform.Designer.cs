namespace MEKB_H0_Anlage
{
    partial class Hauptform
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Hauptform));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gleisplanLadenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.LokEditorOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.lokomotivenNeuLadenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ProgrammBeenden = new System.Windows.Forms.ToolStripMenuItem();
            this.zentraleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verbindungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Verbinden = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Trennen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Z21MenuEinstellungen = new System.Windows.Forms.ToolStripMenuItem();
            this.WeichenMenuEinstellungen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.überwachungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belegtmeldungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.weichenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.signalsteuergungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hilfeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signaleEditierenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uberSteuerprogrammToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Statusbar = new System.Windows.Forms.StatusStrip();
            this.HauptStatusbar = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarSpg = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarStrom = new System.Windows.Forms.ToolStripStatusLabel();
            this.TrackStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.Hinttext = new System.Windows.Forms.ToolTip(this.components);
            this.AutoSignale = new System.Windows.Forms.CheckBox();
            this.AutoFahrdienstleiter = new System.Windows.Forms.CheckBox();
            this.AutoFahrplan = new System.Windows.Forms.CheckBox();
            this.AutoBahnhofsansagen = new System.Windows.Forms.CheckBox();
            this.LokKontrolle = new System.Windows.Forms.CheckBox();
            this.StopAlle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GleisplanAnzeige = new System.Windows.Forms.Panel();
            this.KontrollPanel = new System.Windows.Forms.Panel();
            this.FehlerListe = new System.Windows.Forms.ListView();
            this.ErrorIconList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Btn_Fahrzeuge = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lokomotivenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.Statusbar.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.KontrollPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.zentraleToolStripMenuItem,
            this.toolStripMenuItem2,
            this.hilfeToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1904, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gleisplanLadenToolStripMenuItem,
            this.toolStripSeparator4,
            this.LokEditorOpen,
            this.lokomotivenNeuLadenToolStripMenuItem,
            this.toolStripSeparator1,
            this.ProgrammBeenden});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItem1.Text = "Datei";
            // 
            // gleisplanLadenToolStripMenuItem
            // 
            this.gleisplanLadenToolStripMenuItem.Name = "gleisplanLadenToolStripMenuItem";
            this.gleisplanLadenToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.gleisplanLadenToolStripMenuItem.Text = "Gleisplan laden";
            this.gleisplanLadenToolStripMenuItem.Click += new System.EventHandler(this.gleisplanLadenToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(196, 6);
            // 
            // LokEditorOpen
            // 
            this.LokEditorOpen.Name = "LokEditorOpen";
            this.LokEditorOpen.Size = new System.Drawing.Size(199, 22);
            this.LokEditorOpen.Text = "Lok-Editor";
            this.LokEditorOpen.Click += new System.EventHandler(this.LokEditorOpen_Click);
            // 
            // lokomotivenNeuLadenToolStripMenuItem
            // 
            this.lokomotivenNeuLadenToolStripMenuItem.Name = "lokomotivenNeuLadenToolStripMenuItem";
            this.lokomotivenNeuLadenToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.lokomotivenNeuLadenToolStripMenuItem.Text = "Lokomotiven neu laden";
            this.lokomotivenNeuLadenToolStripMenuItem.Click += new System.EventHandler(this.LokomotivenNeuLadenToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // ProgrammBeenden
            // 
            this.ProgrammBeenden.Name = "ProgrammBeenden";
            this.ProgrammBeenden.Size = new System.Drawing.Size(199, 22);
            this.ProgrammBeenden.Text = "Beenden";
            this.ProgrammBeenden.Click += new System.EventHandler(this.ProgrammBeenden_Click);
            // 
            // zentraleToolStripMenuItem
            // 
            this.zentraleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verbindungToolStripMenuItem,
            this.toolStripSeparator2,
            this.Z21MenuEinstellungen,
            this.WeichenMenuEinstellungen,
            this.toolStripSeparator3,
            this.überwachungToolStripMenuItem});
            this.zentraleToolStripMenuItem.Name = "zentraleToolStripMenuItem";
            this.zentraleToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.zentraleToolStripMenuItem.Text = "Zentrale";
            // 
            // verbindungToolStripMenuItem
            // 
            this.verbindungToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Verbinden,
            this.Menu_Trennen});
            this.verbindungToolStripMenuItem.Name = "verbindungToolStripMenuItem";
            this.verbindungToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.verbindungToolStripMenuItem.Text = "Verbindung";
            // 
            // Menu_Verbinden
            // 
            this.Menu_Verbinden.Name = "Menu_Verbinden";
            this.Menu_Verbinden.Size = new System.Drawing.Size(127, 22);
            this.Menu_Verbinden.Text = "Verbinden";
            this.Menu_Verbinden.Click += new System.EventHandler(this.Menu_Verbinden_Click);
            // 
            // Menu_Trennen
            // 
            this.Menu_Trennen.Name = "Menu_Trennen";
            this.Menu_Trennen.Size = new System.Drawing.Size(127, 22);
            this.Menu_Trennen.Text = "Trennen";
            this.Menu_Trennen.Click += new System.EventHandler(this.Menu_Trennen_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(193, 6);
            // 
            // Z21MenuEinstellungen
            // 
            this.Z21MenuEinstellungen.Name = "Z21MenuEinstellungen";
            this.Z21MenuEinstellungen.Size = new System.Drawing.Size(196, 22);
            this.Z21MenuEinstellungen.Text = "Z21-Einstellungen";
            this.Z21MenuEinstellungen.Click += new System.EventHandler(this.MenuZ21Eigenschaften_Click);
            // 
            // WeichenMenuEinstellungen
            // 
            this.WeichenMenuEinstellungen.Name = "WeichenMenuEinstellungen";
            this.WeichenMenuEinstellungen.Size = new System.Drawing.Size(196, 22);
            this.WeichenMenuEinstellungen.Text = "Weichen-Einstellungen";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // überwachungToolStripMenuItem
            // 
            this.überwachungToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.belegtmeldungToolStripMenuItem,
            this.weichenToolStripMenuItem,
            this.lokomotivenToolStripMenuItem});
            this.überwachungToolStripMenuItem.Name = "überwachungToolStripMenuItem";
            this.überwachungToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.überwachungToolStripMenuItem.Text = "Überwachung";
            // 
            // belegtmeldungToolStripMenuItem
            // 
            this.belegtmeldungToolStripMenuItem.Name = "belegtmeldungToolStripMenuItem";
            this.belegtmeldungToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.belegtmeldungToolStripMenuItem.Text = "Belegtmeldung";
            this.belegtmeldungToolStripMenuItem.Click += new System.EventHandler(this.BelegtmeldungToolStripMenuItem_Click);
            // 
            // weichenToolStripMenuItem
            // 
            this.weichenToolStripMenuItem.Name = "weichenToolStripMenuItem";
            this.weichenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.weichenToolStripMenuItem.Text = "Weichen";
            this.weichenToolStripMenuItem.Click += new System.EventHandler(this.weichenToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signalsteuergungToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(108, 20);
            this.toolStripMenuItem2.Text = "Automatisierung";
            // 
            // signalsteuergungToolStripMenuItem
            // 
            this.signalsteuergungToolStripMenuItem.Name = "signalsteuergungToolStripMenuItem";
            this.signalsteuergungToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.signalsteuergungToolStripMenuItem.Text = "Signalsteuergung";
            this.signalsteuergungToolStripMenuItem.Click += new System.EventHandler(this.SignalsteuergungToolStripMenuItem_Click);
            // 
            // hilfeToolStripMenuItem
            // 
            this.hilfeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            this.hilfeToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.hilfeToolStripMenuItem.Text = "Hilfe";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signaleEditierenToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // signaleEditierenToolStripMenuItem
            // 
            this.signaleEditierenToolStripMenuItem.Name = "signaleEditierenToolStripMenuItem";
            this.signaleEditierenToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.signaleEditierenToolStripMenuItem.Text = "Signale Editieren";
            this.signaleEditierenToolStripMenuItem.Click += new System.EventHandler(this.SignaleEditierenToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uberSteuerprogrammToolStripMenuItem});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // uberSteuerprogrammToolStripMenuItem
            // 
            this.uberSteuerprogrammToolStripMenuItem.Name = "uberSteuerprogrammToolStripMenuItem";
            this.uberSteuerprogrammToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.uberSteuerprogrammToolStripMenuItem.Text = "Über Steuerprogramm";
            this.uberSteuerprogrammToolStripMenuItem.Click += new System.EventHandler(this.UeberSteuerprogrammToolStripMenuItem_Click);
            // 
            // Statusbar
            // 
            this.Statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HauptStatusbar,
            this.StatusBarSpg,
            this.StatusBarStrom,
            this.TrackStatus});
            this.Statusbar.Location = new System.Drawing.Point(0, 937);
            this.Statusbar.Name = "Statusbar";
            this.Statusbar.Size = new System.Drawing.Size(1904, 24);
            this.Statusbar.TabIndex = 1;
            this.Statusbar.Text = "statusStrip1";
            // 
            // HauptStatusbar
            // 
            this.HauptStatusbar.AutoSize = false;
            this.HauptStatusbar.BackColor = System.Drawing.Color.Red;
            this.HauptStatusbar.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.HauptStatusbar.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.HauptStatusbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.HauptStatusbar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HauptStatusbar.Margin = new System.Windows.Forms.Padding(0);
            this.HauptStatusbar.MergeIndex = 0;
            this.HauptStatusbar.Name = "HauptStatusbar";
            this.HauptStatusbar.Size = new System.Drawing.Size(150, 24);
            this.HauptStatusbar.Text = "Z21: UDP";
            this.HauptStatusbar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusBarSpg
            // 
            this.StatusBarSpg.AutoSize = false;
            this.StatusBarSpg.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusBarSpg.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.StatusBarSpg.Margin = new System.Windows.Forms.Padding(0);
            this.StatusBarSpg.Name = "StatusBarSpg";
            this.StatusBarSpg.Size = new System.Drawing.Size(150, 24);
            this.StatusBarSpg.Text = "Gleisspannung:";
            this.StatusBarSpg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusBarStrom
            // 
            this.StatusBarStrom.AutoSize = false;
            this.StatusBarStrom.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusBarStrom.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.StatusBarStrom.Margin = new System.Windows.Forms.Padding(0);
            this.StatusBarStrom.Name = "StatusBarStrom";
            this.StatusBarStrom.Size = new System.Drawing.Size(150, 24);
            this.StatusBarStrom.Text = "Stromverbauch:";
            this.StatusBarStrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TrackStatus
            // 
            this.TrackStatus.AutoSize = false;
            this.TrackStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.TrackStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.TrackStatus.Margin = new System.Windows.Forms.Padding(0);
            this.TrackStatus.Name = "TrackStatus";
            this.TrackStatus.Size = new System.Drawing.Size(150, 24);
            this.TrackStatus.Text = "Strecke: ";
            this.TrackStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Hinttext
            // 
            this.Hinttext.AutoPopDelay = 10000;
            this.Hinttext.InitialDelay = 500;
            this.Hinttext.ReshowDelay = 100;
            // 
            // AutoSignale
            // 
            this.AutoSignale.Appearance = System.Windows.Forms.Appearance.Button;
            this.AutoSignale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AutoSignale.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AutoSignale.ForeColor = System.Drawing.Color.Silver;
            this.AutoSignale.Location = new System.Drawing.Point(6, 15);
            this.AutoSignale.Name = "AutoSignale";
            this.AutoSignale.Size = new System.Drawing.Size(136, 24);
            this.AutoSignale.TabIndex = 938;
            this.AutoSignale.Text = "Automatische Signale";
            this.AutoSignale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hinttext.SetToolTip(this.AutoSignale, resources.GetString("AutoSignale.ToolTip"));
            this.AutoSignale.UseVisualStyleBackColor = false;
            this.AutoSignale.CheckedChanged += new System.EventHandler(this.AutoSignale_CheckedChanged);
            // 
            // AutoFahrdienstleiter
            // 
            this.AutoFahrdienstleiter.Appearance = System.Windows.Forms.Appearance.Button;
            this.AutoFahrdienstleiter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AutoFahrdienstleiter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AutoFahrdienstleiter.ForeColor = System.Drawing.Color.Silver;
            this.AutoFahrdienstleiter.Location = new System.Drawing.Point(1137, 41);
            this.AutoFahrdienstleiter.Name = "AutoFahrdienstleiter";
            this.AutoFahrdienstleiter.Size = new System.Drawing.Size(161, 24);
            this.AutoFahrdienstleiter.TabIndex = 939;
            this.AutoFahrdienstleiter.Text = "Automatischer Fahrdienstleiter";
            this.AutoFahrdienstleiter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hinttext.SetToolTip(this.AutoFahrdienstleiter, resources.GetString("AutoFahrdienstleiter.ToolTip"));
            this.AutoFahrdienstleiter.UseVisualStyleBackColor = false;
            this.AutoFahrdienstleiter.CheckedChanged += new System.EventHandler(this.AutoFahrdienstleiter_CheckedChanged);
            // 
            // AutoFahrplan
            // 
            this.AutoFahrplan.Appearance = System.Windows.Forms.Appearance.Button;
            this.AutoFahrplan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AutoFahrplan.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AutoFahrplan.ForeColor = System.Drawing.Color.Silver;
            this.AutoFahrplan.Location = new System.Drawing.Point(1304, 41);
            this.AutoFahrplan.Name = "AutoFahrplan";
            this.AutoFahrplan.Size = new System.Drawing.Size(161, 24);
            this.AutoFahrplan.TabIndex = 940;
            this.AutoFahrplan.Text = "Automatischer Fahrplan";
            this.AutoFahrplan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hinttext.SetToolTip(this.AutoFahrplan, resources.GetString("AutoFahrplan.ToolTip"));
            this.AutoFahrplan.UseVisualStyleBackColor = false;
            this.AutoFahrplan.CheckedChanged += new System.EventHandler(this.AutoFahrplan_CheckedChanged);
            // 
            // AutoBahnhofsansagen
            // 
            this.AutoBahnhofsansagen.Appearance = System.Windows.Forms.Appearance.Button;
            this.AutoBahnhofsansagen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AutoBahnhofsansagen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AutoBahnhofsansagen.ForeColor = System.Drawing.Color.Silver;
            this.AutoBahnhofsansagen.Location = new System.Drawing.Point(970, 41);
            this.AutoBahnhofsansagen.Name = "AutoBahnhofsansagen";
            this.AutoBahnhofsansagen.Size = new System.Drawing.Size(161, 24);
            this.AutoBahnhofsansagen.TabIndex = 941;
            this.AutoBahnhofsansagen.Text = "Bahnhofsansagen";
            this.AutoBahnhofsansagen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hinttext.SetToolTip(this.AutoBahnhofsansagen, resources.GetString("AutoBahnhofsansagen.ToolTip"));
            this.AutoBahnhofsansagen.UseVisualStyleBackColor = false;
            this.AutoBahnhofsansagen.CheckedChanged += new System.EventHandler(this.AutoBahnhofsansagen_CheckedChanged);
            // 
            // LokKontrolle
            // 
            this.LokKontrolle.Appearance = System.Windows.Forms.Appearance.Button;
            this.LokKontrolle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LokKontrolle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LokKontrolle.ForeColor = System.Drawing.Color.Silver;
            this.LokKontrolle.Location = new System.Drawing.Point(1471, 41);
            this.LokKontrolle.Name = "LokKontrolle";
            this.LokKontrolle.Size = new System.Drawing.Size(161, 24);
            this.LokKontrolle.TabIndex = 999;
            this.LokKontrolle.Text = "Auto. Notbremse für Loks";
            this.LokKontrolle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hinttext.SetToolTip(this.LokKontrolle, "Leuchtet: Loks werden überwacht und führen Notbremse aus\r\nDunkel: Loks fahren nur" +
        " manuell. Keine Notbremsen oder Verfolgung");
            this.LokKontrolle.UseVisualStyleBackColor = false;
            this.LokKontrolle.CheckedChanged += new System.EventHandler(this.LokKontrolle_CheckedChanged);
            // 
            // StopAlle
            // 
            this.StopAlle.BackColor = System.Drawing.Color.Red;
            this.StopAlle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.StopAlle.ForeColor = System.Drawing.SystemColors.Control;
            this.StopAlle.Location = new System.Drawing.Point(6, 15);
            this.StopAlle.Name = "StopAlle";
            this.StopAlle.Size = new System.Drawing.Size(136, 23);
            this.StopAlle.TabIndex = 765;
            this.StopAlle.Text = "Alle Loks anhalten";
            this.StopAlle.UseVisualStyleBackColor = false;
            this.StopAlle.Click += new System.EventHandler(this.StopAlle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AutoSignale);
            this.groupBox1.Location = new System.Drawing.Point(0, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 45);
            this.groupBox1.TabIndex = 937;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automatisierung";
            // 
            // GleisplanAnzeige
            // 
            this.GleisplanAnzeige.AutoScroll = true;
            this.GleisplanAnzeige.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GleisplanAnzeige.Location = new System.Drawing.Point(0, 72);
            this.GleisplanAnzeige.Name = "GleisplanAnzeige";
            this.GleisplanAnzeige.Size = new System.Drawing.Size(1904, 865);
            this.GleisplanAnzeige.TabIndex = 1;
            // 
            // KontrollPanel
            // 
            this.KontrollPanel.Controls.Add(this.FehlerListe);
            this.KontrollPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.KontrollPanel.Location = new System.Drawing.Point(0, 890);
            this.KontrollPanel.Name = "KontrollPanel";
            this.KontrollPanel.Size = new System.Drawing.Size(1904, 47);
            this.KontrollPanel.TabIndex = 996;
            // 
            // FehlerListe
            // 
            this.FehlerListe.HideSelection = false;
            this.FehlerListe.LargeImageList = this.ErrorIconList;
            this.FehlerListe.Location = new System.Drawing.Point(3, 3);
            this.FehlerListe.Name = "FehlerListe";
            this.FehlerListe.Size = new System.Drawing.Size(1901, 40);
            this.FehlerListe.SmallImageList = this.ErrorIconList;
            this.FehlerListe.TabIndex = 1000;
            this.FehlerListe.UseCompatibleStateImageBehavior = false;
            this.FehlerListe.View = System.Windows.Forms.View.Tile;
            // 
            // ErrorIconList
            // 
            this.ErrorIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ErrorIconList.ImageStream")));
            this.ErrorIconList.TransparentColor = System.Drawing.Color.Transparent;
            this.ErrorIconList.Images.SetKeyName(0, "Fehler.png");
            this.ErrorIconList.Images.SetKeyName(1, "Warnung.png");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_Fahrzeuge);
            this.groupBox2.Controls.Add(this.StopAlle);
            this.groupBox2.Location = new System.Drawing.Point(658, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 45);
            this.groupBox2.TabIndex = 997;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Steuerung";
            // 
            // Btn_Fahrzeuge
            // 
            this.Btn_Fahrzeuge.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Btn_Fahrzeuge.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Fahrzeuge.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Btn_Fahrzeuge.Location = new System.Drawing.Point(148, 15);
            this.Btn_Fahrzeuge.Name = "Btn_Fahrzeuge";
            this.Btn_Fahrzeuge.Size = new System.Drawing.Size(136, 23);
            this.Btn_Fahrzeuge.TabIndex = 766;
            this.Btn_Fahrzeuge.Text = "Fahrzeuge";
            this.Btn_Fahrzeuge.UseVisualStyleBackColor = false;
            this.Btn_Fahrzeuge.Click += new System.EventHandler(this.Btn_Fahrzeuge_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(967, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 1000;
            this.label1.Text = "Noch nicht implementiert";
            // 
            // lokomotivenToolStripMenuItem
            // 
            this.lokomotivenToolStripMenuItem.Name = "lokomotivenToolStripMenuItem";
            this.lokomotivenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.lokomotivenToolStripMenuItem.Text = "Lokomotiven";
            this.lokomotivenToolStripMenuItem.Click += new System.EventHandler(this.lokomotivenToolStripMenuItem_Click);
            // 
            // Hauptform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 961);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LokKontrolle);
            this.Controls.Add(this.AutoFahrplan);
            this.Controls.Add(this.AutoBahnhofsansagen);
            this.Controls.Add(this.AutoFahrdienstleiter);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.KontrollPanel);
            this.Controls.Add(this.GleisplanAnzeige);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Statusbar);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Hauptform";
            this.Text = "Modell Eisenbahn Klub Berlin 1932 e.V. - H0 - Anlagensteuerung";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Hauptform_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.Statusbar.ResumeLayout(false);
            this.Statusbar.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.KontrollPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem LokEditorOpen;
        private System.Windows.Forms.ToolStripMenuItem zentraleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verbindungToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Menu_Verbinden;
        private System.Windows.Forms.ToolStripMenuItem Menu_Trennen;
        private System.Windows.Forms.StatusStrip Statusbar;
        private System.Windows.Forms.ToolStripStatusLabel HauptStatusbar;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarSpg;
        private System.Windows.Forms.ToolStripMenuItem hilfeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarStrom;
        private System.Windows.Forms.ToolTip Hinttext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Z21MenuEinstellungen;
        private System.Windows.Forms.ToolStripMenuItem WeichenMenuEinstellungen;
        private System.Windows.Forms.ToolStripMenuItem uberSteuerprogrammToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ProgrammBeenden;
        private System.Windows.Forms.Button StopAlle;
        private System.Windows.Forms.ToolStripStatusLabel TrackStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox AutoSignale;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem signalsteuergungToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem überwachungToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem belegtmeldungToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signaleEditierenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lokomotivenNeuLadenToolStripMenuItem;
        private System.Windows.Forms.Panel GleisplanAnzeige;
        private System.Windows.Forms.Panel KontrollPanel;
        private System.Windows.Forms.ToolStripMenuItem gleisplanLadenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.CheckBox AutoFahrplan;
        private System.Windows.Forms.CheckBox AutoFahrdienstleiter;
        private System.Windows.Forms.CheckBox AutoBahnhofsansagen;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Btn_Fahrzeuge;
        private System.Windows.Forms.ToolStripMenuItem weichenToolStripMenuItem;
        private System.Windows.Forms.CheckBox LokKontrolle;
        private System.Windows.Forms.ListView FehlerListe;
        private System.Windows.Forms.ImageList ErrorIconList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem lokomotivenToolStripMenuItem;
    }
}

