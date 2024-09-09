namespace MEKB_H0_Anlage
{
    partial class Weichen_Ueberwachung
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
            this.WeichenFenster = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.adresseDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abzweigDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.besetztDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.amBewegenDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.schaltzeitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aktiveZeitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spiegelnDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statusErrorDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statusUnbekanntDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.fahrstrasseAbzweigDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.fahrstrasseAktiveDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.fahrstrasseSicherDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.zielStellungDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.weicheBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.WeichenFenster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weicheBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // WeichenFenster
            // 
            this.WeichenFenster.AutoGenerateColumns = false;
            this.WeichenFenster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.WeichenFenster.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.adresseDataGridViewTextBoxColumn,
            this.abzweigDataGridViewCheckBoxColumn,
            this.besetztDataGridViewCheckBoxColumn,
            this.amBewegenDataGridViewCheckBoxColumn,
            this.schaltzeitDataGridViewTextBoxColumn,
            this.aktiveZeitDataGridViewTextBoxColumn,
            this.spiegelnDataGridViewCheckBoxColumn,
            this.statusErrorDataGridViewCheckBoxColumn,
            this.statusUnbekanntDataGridViewCheckBoxColumn,
            this.fahrstrasseAbzweigDataGridViewCheckBoxColumn,
            this.fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn,
            this.fahrstrasseAktiveDataGridViewCheckBoxColumn,
            this.fahrstrasseSicherDataGridViewCheckBoxColumn,
            this.zielStellungDataGridViewCheckBoxColumn});
            this.WeichenFenster.DataSource = this.weicheBindingSource;
            this.WeichenFenster.Location = new System.Drawing.Point(12, 12);
            this.WeichenFenster.Name = "WeichenFenster";
            this.WeichenFenster.Size = new System.Drawing.Size(1211, 426);
            this.WeichenFenster.TabIndex = 0;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.FillWeight = 120F;
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // adresseDataGridViewTextBoxColumn
            // 
            this.adresseDataGridViewTextBoxColumn.DataPropertyName = "Adresse";
            this.adresseDataGridViewTextBoxColumn.FillWeight = 60F;
            this.adresseDataGridViewTextBoxColumn.HeaderText = "Adresse";
            this.adresseDataGridViewTextBoxColumn.Name = "adresseDataGridViewTextBoxColumn";
            // 
            // abzweigDataGridViewCheckBoxColumn
            // 
            this.abzweigDataGridViewCheckBoxColumn.DataPropertyName = "Abzweig";
            this.abzweigDataGridViewCheckBoxColumn.HeaderText = "Abzweig";
            this.abzweigDataGridViewCheckBoxColumn.Name = "abzweigDataGridViewCheckBoxColumn";
            this.abzweigDataGridViewCheckBoxColumn.Width = 50;
            // 
            // besetztDataGridViewCheckBoxColumn
            // 
            this.besetztDataGridViewCheckBoxColumn.DataPropertyName = "Besetzt";
            this.besetztDataGridViewCheckBoxColumn.HeaderText = "Besetzt";
            this.besetztDataGridViewCheckBoxColumn.Name = "besetztDataGridViewCheckBoxColumn";
            this.besetztDataGridViewCheckBoxColumn.Width = 50;
            // 
            // amBewegenDataGridViewCheckBoxColumn
            // 
            this.amBewegenDataGridViewCheckBoxColumn.DataPropertyName = "AmBewegen";
            this.amBewegenDataGridViewCheckBoxColumn.HeaderText = "AmBewegen";
            this.amBewegenDataGridViewCheckBoxColumn.Name = "amBewegenDataGridViewCheckBoxColumn";
            this.amBewegenDataGridViewCheckBoxColumn.Width = 75;
            // 
            // schaltzeitDataGridViewTextBoxColumn
            // 
            this.schaltzeitDataGridViewTextBoxColumn.DataPropertyName = "Schaltzeit";
            this.schaltzeitDataGridViewTextBoxColumn.HeaderText = "Schaltzeit";
            this.schaltzeitDataGridViewTextBoxColumn.Name = "schaltzeitDataGridViewTextBoxColumn";
            this.schaltzeitDataGridViewTextBoxColumn.Width = 75;
            // 
            // aktiveZeitDataGridViewTextBoxColumn
            // 
            this.aktiveZeitDataGridViewTextBoxColumn.DataPropertyName = "AktiveZeit";
            this.aktiveZeitDataGridViewTextBoxColumn.HeaderText = "AktiveZeit";
            this.aktiveZeitDataGridViewTextBoxColumn.Name = "aktiveZeitDataGridViewTextBoxColumn";
            this.aktiveZeitDataGridViewTextBoxColumn.Width = 75;
            // 
            // spiegelnDataGridViewCheckBoxColumn
            // 
            this.spiegelnDataGridViewCheckBoxColumn.DataPropertyName = "Spiegeln";
            this.spiegelnDataGridViewCheckBoxColumn.HeaderText = "Spiegeln";
            this.spiegelnDataGridViewCheckBoxColumn.Name = "spiegelnDataGridViewCheckBoxColumn";
            this.spiegelnDataGridViewCheckBoxColumn.Width = 50;
            // 
            // statusErrorDataGridViewCheckBoxColumn
            // 
            this.statusErrorDataGridViewCheckBoxColumn.DataPropertyName = "Status_Error";
            this.statusErrorDataGridViewCheckBoxColumn.HeaderText = "Error";
            this.statusErrorDataGridViewCheckBoxColumn.Name = "statusErrorDataGridViewCheckBoxColumn";
            this.statusErrorDataGridViewCheckBoxColumn.Width = 50;
            // 
            // statusUnbekanntDataGridViewCheckBoxColumn
            // 
            this.statusUnbekanntDataGridViewCheckBoxColumn.DataPropertyName = "Status_Unbekannt";
            this.statusUnbekanntDataGridViewCheckBoxColumn.HeaderText = "Unbekannt";
            this.statusUnbekanntDataGridViewCheckBoxColumn.Name = "statusUnbekanntDataGridViewCheckBoxColumn";
            this.statusUnbekanntDataGridViewCheckBoxColumn.Width = 75;
            // 
            // fahrstrasseAbzweigDataGridViewCheckBoxColumn
            // 
            this.fahrstrasseAbzweigDataGridViewCheckBoxColumn.DataPropertyName = "FahrstrasseAbzweig";
            this.fahrstrasseAbzweigDataGridViewCheckBoxColumn.HeaderText = "FahrstrasseAbzweig";
            this.fahrstrasseAbzweigDataGridViewCheckBoxColumn.Name = "fahrstrasseAbzweigDataGridViewCheckBoxColumn";
            // 
            // fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn
            // 
            this.fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn.DataPropertyName = "FahrstrasseRichtung_vonZunge";
            this.fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn.HeaderText = "FahrstrasseRichtung_vonZunge";
            this.fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn.Name = "fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn";
            // 
            // fahrstrasseAktiveDataGridViewCheckBoxColumn
            // 
            this.fahrstrasseAktiveDataGridViewCheckBoxColumn.DataPropertyName = "FahrstrasseAktive";
            this.fahrstrasseAktiveDataGridViewCheckBoxColumn.HeaderText = "FahrstrasseAktive";
            this.fahrstrasseAktiveDataGridViewCheckBoxColumn.Name = "fahrstrasseAktiveDataGridViewCheckBoxColumn";
            // 
            // fahrstrasseSicherDataGridViewCheckBoxColumn
            // 
            this.fahrstrasseSicherDataGridViewCheckBoxColumn.DataPropertyName = "FahrstrasseSicher";
            this.fahrstrasseSicherDataGridViewCheckBoxColumn.HeaderText = "FahrstrasseSicher";
            this.fahrstrasseSicherDataGridViewCheckBoxColumn.Name = "fahrstrasseSicherDataGridViewCheckBoxColumn";
            // 
            // zielStellungDataGridViewCheckBoxColumn
            // 
            this.zielStellungDataGridViewCheckBoxColumn.DataPropertyName = "ZielStellung";
            this.zielStellungDataGridViewCheckBoxColumn.HeaderText = "ZielStellung";
            this.zielStellungDataGridViewCheckBoxColumn.Name = "zielStellungDataGridViewCheckBoxColumn";
            // 
            // weicheBindingSource
            // 
            this.weicheBindingSource.DataSource = typeof(MEKB_H0_Anlage.Weiche);
            // 
            // Weichen_Ueberwachung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 450);
            this.Controls.Add(this.WeichenFenster);
            this.Name = "Weichen_Ueberwachung";
            this.Text = "Weichen_Ueberwachung";
            ((System.ComponentModel.ISupportInitialize)(this.WeichenFenster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weicheBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView WeichenFenster;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn adresseDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn abzweigDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn besetztDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn amBewegenDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn schaltzeitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn aktiveZeitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn spiegelnDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn statusErrorDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn statusUnbekanntDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn fahrstrasseAbzweigDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn fahrstrasseRichtungvonZungeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn fahrstrasseAktiveDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn fahrstrasseSicherDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn zielStellungDataGridViewCheckBoxColumn;
        private System.Windows.Forms.BindingSource weicheBindingSource;
    }
}