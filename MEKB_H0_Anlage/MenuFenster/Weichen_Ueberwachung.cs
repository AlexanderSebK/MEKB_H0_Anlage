using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using System.Xml.Serialization;

namespace MEKB_H0_Anlage
{
    public partial class Weichen_Ueberwachung : Form
    {
        #region Instanzen
        private static System.Timers.Timer HeartbeatTimer;
        
        private List<Weiche> Weichenliste;

        private int x_FelderStart = 10;
        private int y_FelderStart = 30;

        #endregion



        public Weichen_Ueberwachung()
        {
            InitializeComponent();
        }
        public Weichen_Ueberwachung(List<Weiche> weichen)
        {
            InitializeComponent();
            Weichenliste = weichen;
            GeneriereFelder();
            
        }

        public void GeneriereFelder()
        {
            for (int i = 0; i < Weichenliste.Count; i++)
            {
                Controls.Add(NamenfeldGenerator(Weichenliste[i], i));
                Controls.Add(AdressfeldGenerator(Weichenliste[i], i));
                Controls.Add(AbzweigfeldGenerator(Weichenliste[i], i));
            }
        }


        private TextBox NamenfeldGenerator(Weiche weiche, int index)
        {
            TextBox WeicheName = new TextBox
            {
                Location = new System.Drawing.Point(x_FelderStart + 57, y_FelderStart + (24 * index)),
                Name = String.Format("{0}_Name", weiche.Name),
                Text = weiche.Name,
                ReadOnly = true,
                Size = new System.Drawing.Size(75, 22)
            };
            return WeicheName;
        }
        private TextBox AdressfeldGenerator(Weiche weiche, int index)
        {
            TextBox WeicheAdr = new TextBox
            {
                Location = new System.Drawing.Point(x_FelderStart + 2, y_FelderStart + (24 * index)),
                Name = String.Format("{0}_Adr", weiche.Name),
                Text = String.Format("{0}", weiche.Adresse),
                ReadOnly = true,
                Size = new System.Drawing.Size(50, 22)
            };
            return WeicheAdr;
        }

        private Label AbzweigfeldGenerator(Weiche weiche, int index)
        {
            Label WeichePos = new Label
            {
                Location = new System.Drawing.Point(x_FelderStart + 142, y_FelderStart + 5+  (24 * index)),
                Name = String.Format("{0}_Abzweig", weiche.Name),
                Size = new System.Drawing.Size(50, 22)
            };
            if (weiche.Abzweig) WeichePos.Text = "Abzweig";
            else WeichePos.Text = "Gerade";
            return WeichePos;
        }


        private void Refresh_Heartbeat(Object source, ElapsedEventArgs e)
        {
            //RefrechDataGrid(WeichenFenster);
        }

        private void RefrechDataGrid(System.Windows.Forms.DataGridView Grid)
        {
            Grid.Invoke((MethodInvoker)(() =>
            {
                Grid.Update();
            }));
        }

        private void Weichen_Ueberwachung_Shown(object sender, EventArgs e)
        {
            // 5 Sekunden Timer einrichten (Lebenspuls für die Verbindung)
            HeartbeatTimer = new System.Timers.Timer(100);
            // Timer mit Funktion "Refresh_Heartbeat" Verbinden
            HeartbeatTimer.Elapsed += Refresh_Heartbeat;
            HeartbeatTimer.AutoReset = true;
            HeartbeatTimer.Enabled = true;
        }

        private void Weichen_Ueberwachung_FormClosing(object sender, FormClosingEventArgs e)
        {
            HeartbeatTimer.Enabled = false;
        }
    }
}
