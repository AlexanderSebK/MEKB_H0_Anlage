namespace MEKB_H0_Anlage
{
    partial class Signal_Einstellungen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Signal_Einstellungen));
            this.AutoSigStellung = new System.Windows.Forms.RadioButton();
            this.AutoSigStrasse = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AutoFahrt = new System.Windows.Forms.CheckBox();
            this.SignalEinstellungenSchliesen = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AutoSigStellung
            // 
            this.AutoSigStellung.AutoSize = true;
            this.AutoSigStellung.Checked = true;
            this.AutoSigStellung.Location = new System.Drawing.Point(16, 48);
            this.AutoSigStellung.Name = "AutoSigStellung";
            this.AutoSigStellung.Size = new System.Drawing.Size(143, 17);
            this.AutoSigStellung.TabIndex = 0;
            this.AutoSigStellung.TabStop = true;
            this.AutoSigStellung.Text = "über Weichenstellungen ";
            this.AutoSigStellung.UseVisualStyleBackColor = true;
            this.AutoSigStellung.CheckedChanged += new System.EventHandler(this.AutoSigStellung_CheckedChanged);
            // 
            // AutoSigStrasse
            // 
            this.AutoSigStrasse.AutoSize = true;
            this.AutoSigStrasse.Location = new System.Drawing.Point(16, 72);
            this.AutoSigStrasse.Name = "AutoSigStrasse";
            this.AutoSigStrasse.Size = new System.Drawing.Size(127, 17);
            this.AutoSigStrasse.TabIndex = 1;
            this.AutoSigStrasse.Text = "über Weichenstraßen";
            this.AutoSigStrasse.UseVisualStyleBackColor = true;
            this.AutoSigStrasse.CheckedChanged += new System.EventHandler(this.AutoSigStrasse_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.AutoSigStellung);
            this.groupBox1.Controls.Add(this.AutoSigStrasse);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 104);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automatische Signalsteuerung";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Erlaubte Signalbilder definieren...";
            // 
            // AutoFahrt
            // 
            this.AutoFahrt.AutoSize = true;
            this.AutoFahrt.Location = new System.Drawing.Point(8, 120);
            this.AutoFahrt.Name = "AutoFahrt";
            this.AutoFahrt.Size = new System.Drawing.Size(313, 17);
            this.AutoFahrt.TabIndex = 3;
            this.AutoFahrt.Text = "Wenn Signalbild Fahrt erlaubt automatisch auf Fahrt schalten";
            this.AutoFahrt.UseVisualStyleBackColor = true;
            this.AutoFahrt.CheckedChanged += new System.EventHandler(this.AutoFahrt_CheckedChanged);
            // 
            // SignalEinstellungenSchliesen
            // 
            this.SignalEinstellungenSchliesen.Location = new System.Drawing.Point(224, 152);
            this.SignalEinstellungenSchliesen.Name = "SignalEinstellungenSchliesen";
            this.SignalEinstellungenSchliesen.Size = new System.Drawing.Size(99, 23);
            this.SignalEinstellungenSchliesen.TabIndex = 4;
            this.SignalEinstellungenSchliesen.Text = "Schließen";
            this.SignalEinstellungenSchliesen.UseVisualStyleBackColor = true;
            this.SignalEinstellungenSchliesen.Click += new System.EventHandler(this.SignalEinstellungenSchliesen_Click);
            // 
            // Signal_Einstellungen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 185);
            this.Controls.Add(this.SignalEinstellungenSchliesen);
            this.Controls.Add(this.AutoFahrt);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Signal_Einstellungen";
            this.Text = "Einstellungen Signale";
            this.Shown += new System.EventHandler(this.Signal_Einstellungen_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton AutoSigStellung;
        private System.Windows.Forms.RadioButton AutoSigStrasse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox AutoFahrt;
        private System.Windows.Forms.Button SignalEinstellungenSchliesen;
    }
}