namespace MEKB_H0_Anlage
{
    partial class MenuFenster_Signalistentool
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
            this.ToolSignalIndex = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.ToolSignalName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ToolAdr1 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.ToolAdr1S0 = new System.Windows.Forms.ComboBox();
            this.ToolAdr1S1 = new System.Windows.Forms.ComboBox();
            this.ToolAdr2S0 = new System.Windows.Forms.ComboBox();
            this.ToolAdr2S1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ToolAdr2 = new System.Windows.Forms.NumericUpDown();
            this.ToolRoutenIndex = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.ToolSignalWeichenListe = new System.Windows.Forms.DataGridView();
            this.DataGridWeichenName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridAbzweig = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ToolSignalIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolAdr1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolAdr2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolRoutenIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolSignalWeichenListe)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolSignalIndex
            // 
            this.ToolSignalIndex.Location = new System.Drawing.Point(51, 7);
            this.ToolSignalIndex.Name = "ToolSignalIndex";
            this.ToolSignalIndex.Size = new System.Drawing.Size(52, 20);
            this.ToolSignalIndex.TabIndex = 0;
            this.ToolSignalIndex.ValueChanged += new System.EventHandler(this.ToolSignalIndex_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Index";
            // 
            // ToolSignalName
            // 
            this.ToolSignalName.Location = new System.Drawing.Point(248, 6);
            this.ToolSignalName.Name = "ToolSignalName";
            this.ToolSignalName.Size = new System.Drawing.Size(121, 20);
            this.ToolSignalName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(171, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Signalname";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Adresse 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Adresse 2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(140, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Adresse 1 - Status 0";
            // 
            // ToolAdr1
            // 
            this.ToolAdr1.Location = new System.Drawing.Point(72, 48);
            this.ToolAdr1.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ToolAdr1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ToolAdr1.Name = "ToolAdr1";
            this.ToolAdr1.Size = new System.Drawing.Size(52, 20);
            this.ToolAdr1.TabIndex = 9;
            this.ToolAdr1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(140, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Adresse 1 - Status 1";
            // 
            // ToolAdr1S0
            // 
            this.ToolAdr1S0.FormattingEnabled = true;
            this.ToolAdr1S0.Location = new System.Drawing.Point(248, 47);
            this.ToolAdr1S0.Name = "ToolAdr1S0";
            this.ToolAdr1S0.Size = new System.Drawing.Size(121, 21);
            this.ToolAdr1S0.TabIndex = 11;
            // 
            // ToolAdr1S1
            // 
            this.ToolAdr1S1.FormattingEnabled = true;
            this.ToolAdr1S1.Location = new System.Drawing.Point(248, 74);
            this.ToolAdr1S1.Name = "ToolAdr1S1";
            this.ToolAdr1S1.Size = new System.Drawing.Size(121, 21);
            this.ToolAdr1S1.TabIndex = 12;
            // 
            // ToolAdr2S0
            // 
            this.ToolAdr2S0.FormattingEnabled = true;
            this.ToolAdr2S0.Location = new System.Drawing.Point(248, 101);
            this.ToolAdr2S0.Name = "ToolAdr2S0";
            this.ToolAdr2S0.Size = new System.Drawing.Size(121, 21);
            this.ToolAdr2S0.TabIndex = 13;
            // 
            // ToolAdr2S1
            // 
            this.ToolAdr2S1.FormattingEnabled = true;
            this.ToolAdr2S1.Location = new System.Drawing.Point(248, 128);
            this.ToolAdr2S1.Name = "ToolAdr2S1";
            this.ToolAdr2S1.Size = new System.Drawing.Size(121, 21);
            this.ToolAdr2S1.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(140, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Adresse 2 - Status 1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(140, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Adresse 2 - Status 0";
            // 
            // ToolAdr2
            // 
            this.ToolAdr2.Location = new System.Drawing.Point(72, 102);
            this.ToolAdr2.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ToolAdr2.Name = "ToolAdr2";
            this.ToolAdr2.Size = new System.Drawing.Size(52, 20);
            this.ToolAdr2.TabIndex = 17;
            // 
            // ToolRoutenIndex
            // 
            this.ToolRoutenIndex.Location = new System.Drawing.Point(54, 215);
            this.ToolRoutenIndex.Name = "ToolRoutenIndex";
            this.ToolRoutenIndex.Size = new System.Drawing.Size(52, 20);
            this.ToolRoutenIndex.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 217);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Route";
            // 
            // ToolSignalWeichenListe
            // 
            this.ToolSignalWeichenListe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ToolSignalWeichenListe.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataGridWeichenName,
            this.DataGridAbzweig});
            this.ToolSignalWeichenListe.Location = new System.Drawing.Point(12, 241);
            this.ToolSignalWeichenListe.Name = "ToolSignalWeichenListe";
            this.ToolSignalWeichenListe.Size = new System.Drawing.Size(284, 197);
            this.ToolSignalWeichenListe.TabIndex = 20;
            // 
            // DataGridWeichenName
            // 
            this.DataGridWeichenName.HeaderText = "Weiche";
            this.DataGridWeichenName.Name = "DataGridWeichenName";
            // 
            // DataGridAbzweig
            // 
            this.DataGridAbzweig.HeaderText = "Weiche auf Abzweig?";
            this.DataGridAbzweig.Name = "DataGridAbzweig";
            this.DataGridAbzweig.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridAbzweig.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DataGridAbzweig.Width = 120;
            // 
            // MenuFenster_Signalistentool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ToolSignalWeichenListe);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ToolRoutenIndex);
            this.Controls.Add(this.ToolAdr2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ToolAdr2S1);
            this.Controls.Add(this.ToolAdr2S0);
            this.Controls.Add(this.ToolAdr1S1);
            this.Controls.Add(this.ToolAdr1S0);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ToolAdr1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ToolSignalName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ToolSignalIndex);
            this.Name = "MenuFenster_Signalistentool";
            this.Text = "Tool: Signaliste";
            ((System.ComponentModel.ISupportInitialize)(this.ToolSignalIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolAdr1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolAdr2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolRoutenIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolSignalWeichenListe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown ToolSignalIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ToolSignalName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown ToolAdr1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ToolAdr1S0;
        private System.Windows.Forms.ComboBox ToolAdr1S1;
        private System.Windows.Forms.ComboBox ToolAdr2S0;
        private System.Windows.Forms.ComboBox ToolAdr2S1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown ToolAdr2;
        private System.Windows.Forms.NumericUpDown ToolRoutenIndex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView ToolSignalWeichenListe;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGridWeichenName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DataGridAbzweig;
    }
}