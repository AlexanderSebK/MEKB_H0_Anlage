namespace MEKB_H0_Anlage
{
    partial class Lok_Ueberwachung
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
            this.LokIndex = new System.Windows.Forms.DomainUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.LOK_Addr = new System.Windows.Forms.TextBox();
            this.LOK_Speed = new System.Windows.Forms.TextBox();
            this.LOK_Richtung = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Block_AKT = new System.Windows.Forms.TextBox();
            this.Block_NEXT = new System.Windows.Forms.TextBox();
            this.Block_VOR = new System.Windows.Forms.TextBox();
            this.Block_LASTKNOWN = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LokIndex
            // 
            this.LokIndex.Location = new System.Drawing.Point(79, 16);
            this.LokIndex.Name = "LokIndex";
            this.LokIndex.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LokIndex.Size = new System.Drawing.Size(120, 20);
            this.LokIndex.TabIndex = 0;
            this.LokIndex.Text = "domainUpDown1";
            this.LokIndex.SelectedItemChanged += new System.EventHandler(this.LokIndex_SelectedItemChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Listenindex";
            // 
            // LOK_Addr
            // 
            this.LOK_Addr.Location = new System.Drawing.Point(48, 44);
            this.LOK_Addr.Name = "LOK_Addr";
            this.LOK_Addr.Size = new System.Drawing.Size(100, 20);
            this.LOK_Addr.TabIndex = 2;
            // 
            // LOK_Speed
            // 
            this.LOK_Speed.Location = new System.Drawing.Point(48, 86);
            this.LOK_Speed.Name = "LOK_Speed";
            this.LOK_Speed.Size = new System.Drawing.Size(100, 20);
            this.LOK_Speed.TabIndex = 3;
            // 
            // LOK_Richtung
            // 
            this.LOK_Richtung.Location = new System.Drawing.Point(232, 86);
            this.LOK_Richtung.Name = "LOK_Richtung";
            this.LOK_Richtung.Size = new System.Drawing.Size(100, 20);
            this.LOK_Richtung.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Adr.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Speed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(176, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Richtung";
            // 
            // Block_AKT
            // 
            this.Block_AKT.Location = new System.Drawing.Point(175, 159);
            this.Block_AKT.Name = "Block_AKT";
            this.Block_AKT.Size = new System.Drawing.Size(100, 20);
            this.Block_AKT.TabIndex = 8;
            // 
            // Block_NEXT
            // 
            this.Block_NEXT.Location = new System.Drawing.Point(281, 159);
            this.Block_NEXT.Name = "Block_NEXT";
            this.Block_NEXT.Size = new System.Drawing.Size(100, 20);
            this.Block_NEXT.TabIndex = 9;
            // 
            // Block_VOR
            // 
            this.Block_VOR.Location = new System.Drawing.Point(69, 159);
            this.Block_VOR.Name = "Block_VOR";
            this.Block_VOR.Size = new System.Drawing.Size(100, 20);
            this.Block_VOR.TabIndex = 10;
            // 
            // Block_LASTKNOWN
            // 
            this.Block_LASTKNOWN.Location = new System.Drawing.Point(175, 185);
            this.Block_LASTKNOWN.Name = "Block_LASTKNOWN";
            this.Block_LASTKNOWN.Size = new System.Drawing.Size(100, 20);
            this.Block_LASTKNOWN.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(176, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Letzter Bekannter";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Aktuell";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(93, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Vorheriger";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(306, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Nächster";
            // 
            // Lok_Ueberwachung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Block_LASTKNOWN);
            this.Controls.Add(this.Block_VOR);
            this.Controls.Add(this.Block_NEXT);
            this.Controls.Add(this.Block_AKT);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LOK_Richtung);
            this.Controls.Add(this.LOK_Speed);
            this.Controls.Add(this.LOK_Addr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LokIndex);
            this.Name = "Lok_Ueberwachung";
            this.Text = "Lok_Ueberwachung";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DomainUpDown LokIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LOK_Addr;
        private System.Windows.Forms.TextBox LOK_Speed;
        private System.Windows.Forms.TextBox LOK_Richtung;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Block_AKT;
        private System.Windows.Forms.TextBox Block_NEXT;
        private System.Windows.Forms.TextBox Block_VOR;
        private System.Windows.Forms.TextBox Block_LASTKNOWN;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}