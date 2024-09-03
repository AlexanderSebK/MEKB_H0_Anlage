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
            this.belegtmelderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registriertDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OK = new System.Windows.Forms.Button();
            this.Abbruch = new System.Windows.Forms.Button();
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
            this.Tabelle.Size = new System.Drawing.Size(465, 253);
            this.Tabelle.TabIndex = 0;
            // 
            // belegtmelderBindingSource
            // 
            this.belegtmelderBindingSource.DataSource = typeof(MEKB_H0_Anlage.Belegtmelder);
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
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(401, 271);
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
            this.Abbruch.Location = new System.Drawing.Point(320, 271);
            this.Abbruch.Name = "Abbruch";
            this.Abbruch.Size = new System.Drawing.Size(75, 23);
            this.Abbruch.TabIndex = 2;
            this.Abbruch.Text = "Abbrechen";
            this.Abbruch.UseVisualStyleBackColor = true;
            this.Abbruch.Click += new System.EventHandler(this.Abbruch_Click);
            // 
            // BelegtmelderAuswahl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 303);
            this.Controls.Add(this.Abbruch);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Tabelle);
            this.Name = "BelegtmelderAuswahl";
            this.Text = "BelegtmelderAuswahl";
            ((System.ComponentModel.ISupportInitialize)(this.Tabelle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.belegtmelderBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView Tabelle;
        private System.Windows.Forms.BindingSource belegtmelderBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn registriertDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Abbruch;
    }
}