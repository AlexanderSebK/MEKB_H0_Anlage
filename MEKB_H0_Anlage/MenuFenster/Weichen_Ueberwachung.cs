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
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MEKB_H0_Anlage
{
    public partial class Weichen_Ueberwachung : Form
    {
        #region Instanzen
        private static System.Timers.Timer HeartbeatTimer;

        private WeichenListe WeichenListe = WeichenListe.Instance;
        
        private Dictionary<string, bool> AbzweigStatus = new Dictionary<string, bool>();
        private Dictionary<string, string> FehlerStatus = new Dictionary<string, string>();

        private int x_FelderStart = 10;
        private int y_FelderStart = 30;

        #endregion



        public Weichen_Ueberwachung()
        {
            InitializeComponent();
            AbzweigStatus = new Dictionary<string, bool>();
            GeneriereFelder();
        }
        

        public void GeneriereFelder()
        {
            for (int i = 0; i < WeichenListe.Liste.Count; i++)
            {
                Controls.Add(NamenfeldGenerator(WeichenListe.Liste[i], i));
                Controls.Add(AdressfeldGenerator(WeichenListe.Liste[i], i));
                Controls.Add(AbzweigfeldGenerator(WeichenListe.Liste[i], i));
                Controls.Add(FehlerfeldGenerator(WeichenListe.Liste[i], i));
                Controls.Add(SchaltzeitfeldGenerator(WeichenListe.Liste[i], i));
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
            AbzweigStatus.Add(weiche.Name,weiche.Abzweig);
            return WeichePos;
        }
        private Label FehlerfeldGenerator(Weiche weiche, int index)
        {
            Label WeicheFehler = new Label
            {
                Location = new System.Drawing.Point(x_FelderStart + 242, y_FelderStart + 5 + (24 * index)),
                Name = String.Format("{0}_Fehler", weiche.Name),
                Size = new System.Drawing.Size(50, 22)
            };
            WeicheFehler.Text = FehlerAuswertung(weiche);
            FehlerStatus.Add(weiche.Name, FehlerAuswertung(weiche));
            return WeicheFehler;
        }

        private TextBox SchaltzeitfeldGenerator(Weiche weiche, int index)
        {
            TextBox WeicheAdr = new TextBox
            {
                Location = new System.Drawing.Point(x_FelderStart + 312, y_FelderStart + (24 * index)),
                Name = String.Format("{0}_SchaltZeit", weiche.Name),
                Text = String.Format("{0}", weiche.Schaltzeit),
                ReadOnly = true,
                Size = new System.Drawing.Size(50, 22)
            };
            return WeicheAdr;
        }


        private void Refresh_Heartbeat(Object source, ElapsedEventArgs e)
        {
            foreach (Weiche weiche in WeichenListe.Liste)
            {
                RefreshAbzweig(weiche.Name, weiche.Abzweig);
                RefershFehler(weiche.Name, FehlerAuswertung(weiche));
            }
        }

        #region Refresh
        #region Abzweig
        private void RefreshAbzweig(string WeichenName, bool Abzweig)
        {
            if (AbzweigStatus.ContainsKey(WeichenName)) 
            {
                if (AbzweigStatus[WeichenName] != Abzweig)
                {
                    Label Abzweigfeld = (Label)this.Controls.Find(WeichenName + "_Abzweig", true).First();
                    if (Abzweigfeld == null) return; //Nicht gefunden: Abbrechen
                    this.BeginInvoke((Action<Label, bool>)UpdateAbzweigText, Abzweigfeld, Abzweig);
                    AbzweigStatus[WeichenName] = Abzweig;
                }
            }
        }
        private void UpdateAbzweigText(Label label, bool Abzweig)
        {
            if (Abzweig) label.Text = "Abzweig";    
            else label.Text = "Gerade";
        }
        #endregion
        #region Abzweig
        private void RefershFehler(string WeichenName, string Zustand)
        {
            if (FehlerStatus.ContainsKey(WeichenName))
            {
                if (!FehlerStatus[WeichenName].Equals(Zustand))
                {
                    Label Abzweigfeld = (Label)this.Controls.Find(WeichenName + "_Fehler", true).First();
                    if (Abzweigfeld == null) return; //Nicht gefunden: Abbrechen
                    this.BeginInvoke((Action<Label, string>)UpdateFehlerText, Abzweigfeld, Zustand);
                    FehlerStatus[WeichenName] = Zustand;
                }
            }
        }
        private void UpdateFehlerText(Label label, string Zustand)
        {
            label.Text = Zustand;
        }
        #endregion

        #endregion

        private string FehlerAuswertung(Weiche weiche)
        {
            if (weiche == null) return "<fatal>";
            if (weiche.Status_Error) return "Fehler";
            if (weiche.Status_Unbekannt) return "Unbekannt";
            return "OK";
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
