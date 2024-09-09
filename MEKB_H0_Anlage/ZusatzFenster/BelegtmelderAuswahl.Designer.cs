namespace MEKB_H0_Anlage
{
    partial class BelegtmelderAuswahl
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
            this.Tabelle = new System.Windows.Forms.DataGridView();
            this.OK = new System.Windows.Forms.Button();
            this.Abbruch = new System.Windows.Forms.Button();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registriertDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.belegtmelderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Tabelle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.belegtmelderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // Tabelle
            // 
            this.Tabelle.AutoGenerateColumns = false;
            this.Tabelle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabelle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.registriertDataGridViewTextBoxColumn});
            this.Tabelle.DataSource = this.belegtmelderBindingSource;
            this.Tabelle.Location = new System.Drawing.Point(12, 12);
            this.Tabelle.Name = "Tabelle";
            this.Tabelle.Size = new System.Drawing.Size(465, 230);
            this.Tabelle.TabIndex = 0;
            this.Tabelle.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabelle_CellClick);
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(402, 275);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Abbruch
            // 
            this.Abbruch.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Abbruch.Location = new System.Drawing.Point(321, 275);
            this.Abbruch.Name = "Abbruch";
            this.Abbruch.Size = new System.Drawing.Size(75, 23);
            this.Abbruch.TabIndex = 2;
            this.Abbruch.Text = "Abbrechen";
            this.Abbruch.UseVisualStyleBackColor = true;
            this.Abbruch.Click += new System.EventHandler(this.Abbruch_Click);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Ortsname";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // registriertDataGridViewTextBoxColumn
            // 
            this.registriertDataGridViewTextBoxColumn.DataPropertyName = "Registriert";
            this.registriertDataGridViewTextBoxColumn.HeaderText = "Registrierte Lok";
            this.registriertDataGridViewTextBoxColumn.Name = "registriertDataGridViewTextBoxColumn";
            this.registriertDataGridViewTextBoxColumn.Width = 200;
            // 
            // belegtmelderBindingSource
            // 
            this.belegtmelderBindingSource.DataSource = typeof(MEKB_H0_Anlage.Belegtmelder);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(301, 248);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(175, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 251);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Fahrzeug kam von Block";
            // 
            // BelegtmelderAuswahl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 313);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.Abbruch);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Tabelle);
            this.Name = "BelegtmelderAuswahl";
            this.Text = "BelegtmelderAuswahl";
            ((System.ComponentModel.ISupportInitialize)(this.Tabelle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.belegtmelderBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Tabelle;
        private System.Windows.Forms.BindingSource belegtmelderBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn registriertDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Abbruch;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
    }
}