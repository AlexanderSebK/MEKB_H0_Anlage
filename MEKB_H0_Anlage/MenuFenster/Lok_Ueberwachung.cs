using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Lok_Ueberwachung : Form
    {
        #region Instanzen
        private AktiveLokomotiven AktiveLokomotiven = AktiveLokomotiven.Instance;
        private Lokomotive lokomotive = new Lokomotive();
        #endregion

        public Lok_Ueberwachung()
        {
            InitializeComponent();
            LokIndex.Items.Clear();
            for (int i = 0; i < AktiveLokomotiven.Liste.Count; i++)
            {
                LokIndex.Items.Add(AktiveLokomotiven.Liste[i].Name);
            }            
        }

        private void LokIndex_SelectedItemChanged(object sender, EventArgs e)
        {
            lokomotive.PropertyChanged -= UpdateText;
            lokomotive = AktiveLokomotiven.GetLokomotive(LokIndex.SelectedItem.ToString());
            lokomotive.PropertyChanged += UpdateText;
            LOK_Addr.Text = lokomotive.Adresse.ToString();
        }

        public void UpdateText(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.ToString().Equals("Richtung") || e.PropertyName.ToString().Equals("LokUmgedreht"))
            {
                if (lokomotive.Richtung == 0 && !lokomotive.LokUmgedreht) LOK_Richtung.Text = "Vorwärts";
                else if (lokomotive.Richtung == 128 && lokomotive.LokUmgedreht) LOK_Richtung.Text = "Vorwärts";
                else LOK_Richtung.Text = "Rückwärts";
            }
            if (e.PropertyName.ToString().Equals("Fahrstufe"))
            {
                AppendTextBox(LOK_Speed,lokomotive.Fahrstufe.ToString());
            }
            if (e.PropertyName.ToString().Equals("AktuellerBlock"))
            {
                AppendTextBox(Block_AKT,lokomotive.AktuellerBlock);
            }
            if (e.PropertyName.ToString().Equals("VorherigerBlock"))
            {
                AppendTextBox(Block_VOR, lokomotive.VorherigerBlock);
            }
            if (e.PropertyName.ToString().Equals("NexterBlock"))
            {
                AppendTextBox(Block_NEXT, lokomotive.NexterBlock);
            }
            if (e.PropertyName.ToString().Equals("LetzterBekannterBlock"))
            {
                AppendTextBox(Block_LASTKNOWN, lokomotive.LetzterBekannterBlock);
            }
        }


        public void AppendTextBox(TextBox textbox, string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<TextBox, string>(AppendTextBox), new object[] { textbox, value });
                return;
            }
            textbox.Text = value;
        }
    }

}
