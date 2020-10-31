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
    public partial class ZugSteuerpult : Form
    {
        public ZugSteuerpult()
        {
            InitializeComponent();
        }

        private void Fahrstufe_ValueChanged(object sender, EventArgs e)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.FahrstufenAnzeige; //Gleisbild
            Graphics anzeige = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln

            SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            int drawvalue = (int)(168 - (Fahrstufe.Value * 1.3125));
            Geschwindigkeit.Text = drawvalue.ToString();
            anzeige.FillRectangle(myBrush, new Rectangle(0, 0, 64, drawvalue));

            bild.MakeTransparent(System.Drawing.Color.Black);

            FahrAnzeige.Image = bild;

            myBrush.Dispose();
            anzeige.Dispose();
            
        }


        private void ZugSteuerpult_Shown(object sender, EventArgs e)
        {
            Fahrstufe.Value = 0;
            Fahrstufe_ValueChanged(sender, e);
        }
    }
}
