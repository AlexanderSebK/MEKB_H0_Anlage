namespace MEKB_H0_Anlage
{
    partial class LokSuche
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
            this.OK = new System.Windows.Forms.Button();
            this.LokAuswahl = new System.Windows.Forms.ComboBox();
            this.SucheLoktyp = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SucheEpoche = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SucheGattung = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Abbruch = new System.Windows.Forms.Button();
            this.SucheVerwaltung = new System.Windows.Forms.ComboBox();
            this.SucheHersteller = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(244, 175);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(93, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "Lok auswählen";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // LokAuswahl
            // 
            this.LokAuswahl.FormattingEnabled = true;
            this.LokAuswahl.Location = new System.Drawing.Point(12, 177);
            this.LokAuswahl.Name = "LokAuswahl";
            this.LokAuswahl.Size = new System.Drawing.Size(226, 21);
            this.LokAuswahl.TabIndex = 1;
            // 
            // SucheLoktyp
            // 
            this.SucheLoktyp.FormattingEnabled = true;
            this.SucheLoktyp.Location = new System.Drawing.Point(12, 25);
            this.SucheLoktyp.Name = "SucheLoktyp";
            this.SucheLoktyp.Size = new System.Drawing.Size(175, 21);
            this.SucheLoktyp.TabIndex = 2;
            this.SucheLoktyp.SelectedIndexChanged += new System.EventHandler(this.NeuGewaehlt);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Loktyp";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Epoche";
            // 
            // SucheEpoche
            // 
            this.SucheEpoche.FormattingEnabled = true;
            this.SucheEpoche.Location = new System.Drawing.Point(12, 70);
            this.SucheEpoche.Name = "SucheEpoche";
            this.SucheEpoche.Size = new System.Drawing.Size(175, 21);
            this.SucheEpoche.TabIndex = 5;
            this.SucheEpoche.SelectedIndexChanged += new System.EventHandler(this.NeuGewaehlt);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Gattung";
            // 
            // SucheGattung
            // 
            this.SucheGattung.FormattingEnabled = true;
            this.SucheGattung.Location = new System.Drawing.Point(12, 115);
            this.SucheGattung.Name = "SucheGattung";
            this.SucheGattung.Size = new System.Drawing.Size(175, 21);
            this.SucheGattung.TabIndex = 7;
            this.SucheGattung.SelectedIndexChanged += new System.EventHandler(this.NeuGewaehlt);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Gewälte Lok";
            // 
            // Abbruch
            // 
            this.Abbruch.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Abbruch.Location = new System.Drawing.Point(343, 175);
            this.Abbruch.Name = "Abbruch";
            this.Abbruch.Size = new System.Drawing.Size(93, 23);
            this.Abbruch.TabIndex = 9;
            this.Abbruch.Text = "Abbruch";
            this.Abbruch.UseVisualStyleBackColor = true;
            // 
            // SucheVerwaltung
            // 
            this.SucheVerwaltung.FormattingEnabled = true;
            this.SucheVerwaltung.Location = new System.Drawing.Point(261, 25);
            this.SucheVerwaltung.Name = "SucheVerwaltung";
            this.SucheVerwaltung.Size = new System.Drawing.Size(175, 21);
            this.SucheVerwaltung.TabIndex = 10;
            this.SucheVerwaltung.SelectedIndexChanged += new System.EventHandler(this.NeuGewaehlt);
            // 
            // SucheHersteller
            // 
            this.SucheHersteller.FormattingEnabled = true;
            this.SucheHersteller.Location = new System.Drawing.Point(261, 70);
            this.SucheHersteller.Name = "SucheHersteller";
            this.SucheHersteller.Size = new System.Drawing.Size(175, 21);
            this.SucheHersteller.TabIndex = 11;
            this.SucheHersteller.SelectedIndexChanged += new System.EventHandler(this.NeuGewaehlt);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(352, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Bahnverwaltung";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(356, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Modellhersteller";
            // 
            // LokSuche
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 210);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SucheHersteller);
            this.Controls.Add(this.SucheVerwaltung);
            this.Controls.Add(this.Abbruch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SucheGattung);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SucheEpoche);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SucheLoktyp);
            this.Controls.Add(this.LokAuswahl);
            this.Controls.Add(this.OK);
            this.Name = "LokSuche";
            this.Text = "LokSuche";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.ComboBox LokAuswahl;
        private System.Windows.Forms.ComboBox SucheLoktyp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox SucheEpoche;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox SucheGattung;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Abbruch;
        private System.Windows.Forms.ComboBox SucheVerwaltung;
        private System.Windows.Forms.ComboBox SucheHersteller;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}