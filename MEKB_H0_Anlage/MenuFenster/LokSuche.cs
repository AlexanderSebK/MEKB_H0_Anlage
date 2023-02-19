using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class LokSuche : Form
    {
        public LokomotivenVerwaltung LokomotivenArchiv;
        public Lokomotive GewaehlteLok;

        public LokSuche()
        {
            InitializeComponent();
            LokomotivenArchiv = new LokomotivenVerwaltung();
        }

        public LokSuche(LokomotivenVerwaltung verwaltung)
        {
            InitializeComponent();
            LokomotivenArchiv = verwaltung;
            UpdateLokAuswahlListe(verwaltung.AlleLoks());

            SucheEpoche.Items.Add("Alle Epochen");
            SucheEpoche.Items.AddRange(LokomotivenArchiv.ListeEpoche.ToArray());
            SucheEpoche.SelectedIndex = SucheEpoche.FindStringExact("Alle Epochen");
            SucheGattung.Items.Add("Alle");
            SucheGattung.Items.AddRange(LokomotivenArchiv.ListeGattung.ToArray());
            SucheGattung.SelectedIndex = SucheGattung.FindStringExact("Alle");
            SucheLoktyp.Items.Add("Alle");
            SucheLoktyp.Items.AddRange(LokomotivenArchiv.ListeTyp.ToArray());
            SucheLoktyp.SelectedIndex = SucheLoktyp.FindStringExact("Alle");
            SucheHersteller.Items.Add("Alle");
            SucheHersteller.Items.AddRange(LokomotivenArchiv.ListeHersteller.ToArray());
            SucheHersteller.SelectedIndex = SucheHersteller.FindStringExact("Alle");
            SucheVerwaltung.Items.Add("Alle");
            SucheVerwaltung.Items.AddRange(LokomotivenArchiv.ListeVerwaltung.ToArray());
            SucheVerwaltung.SelectedIndex = SucheVerwaltung.FindStringExact("Alle");
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (LokAuswahl.SelectedItem == null)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                if(LokAuswahl.SelectedItem.ToString() == "Keine Lok gefunden")
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                if (LokomotivenArchiv.SucheDurchName(LokAuswahl.SelectedItem.ToString(), out GewaehlteLok))
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void NeuGewaehlt(object sender, EventArgs e)
        {
            List<Lokomotive> Ergebnisliste = LokomotivenArchiv.AlleLoks();
            if (SucheLoktyp.SelectedIndex != -1)
            {
                if (!SucheLoktyp.Text.Equals("Alle") && !SucheLoktyp.Text.Equals(""))
                {
                    Ergebnisliste = LokomotivenArchiv.FindeAlleDurchTyp(SucheLoktyp.Text, Ergebnisliste);
                }
            }
            if (SucheEpoche.SelectedIndex != -1)
            {
                switch(SucheEpoche.Text)
                {
                    case "I": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(1, Ergebnisliste); break;
                    case "II": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(2, Ergebnisliste); break;
                    case "III": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(3, Ergebnisliste); break;
                    case "IV": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(4, Ergebnisliste); break;
                    case "V": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(5, Ergebnisliste); break;
                    case "VI": Ergebnisliste = LokomotivenArchiv.FindeAlleDurchEpoche(5, Ergebnisliste); break;
                } 
            }
            if (SucheGattung.SelectedIndex != -1)
            {
                if (!SucheGattung.Text.Equals("Alle") && !SucheGattung.Text.Equals(""))
                {
                    Ergebnisliste = LokomotivenArchiv.FindeAlleDurchGattung(SucheGattung.Text, Ergebnisliste);
                }
            }
            if (SucheVerwaltung.SelectedIndex != -1)
            {
                if (!SucheVerwaltung.Text.Equals("Alle") && !SucheVerwaltung.Text.Equals(""))
                {
                    Ergebnisliste = LokomotivenArchiv.FindeAlleDurchVerwaltung(SucheVerwaltung.Text, Ergebnisliste);
                }
            }
            if (SucheHersteller.SelectedIndex != -1)
            {
                if (!SucheHersteller.Text.Equals("Alle") && !SucheHersteller.Text.Equals(""))
                {
                    Ergebnisliste = LokomotivenArchiv.FindeAlleDurchHersteller(SucheHersteller.Text, Ergebnisliste);
                }
            }

            UpdateLokAuswahlListe(Ergebnisliste);
        }

        private void UpdateLokAuswahlListe(List<Lokomotive> loks)
        {
            LokAuswahl.Items.Clear();
            foreach(Lokomotive lok in loks)
            {
                LokAuswahl.Items.Add(lok.Name);
            }
            if(LokAuswahl.Items.Count == 0)
            {
                LokAuswahl.Items.Add("Keine Lok gefunden");
            }
            LokAuswahl.SelectedIndex = 0;
        }

    }
}
