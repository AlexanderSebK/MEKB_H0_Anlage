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
        private Lok Lokdaten { set; get; }
        public ZugSteuerpult(Lok Instance)
        {
            InitializeComponent();
            Lokdaten = Instance;
            UpdateLokDaten();
        }

        public delegate void CMD_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo);
        public delegate void CMD_LOKFUNKTION(int Adresse, byte Zustand, byte FunktionsNr);

        private CMD_LOKFAHRT setLOKFahrt;
        private CMD_LOKFUNKTION setLOKFunktion;

        public void Register_CMD_LOKFAHRT(CMD_LOKFAHRT function) { setLOKFahrt = function; }
        public void Register_CMD_LOKFUNKTION(CMD_LOKFUNKTION function) { setLOKFunktion = function; }

        private bool ValueChangedByZ21 = false;

        public void UpdateLokDaten()
        {
            ValueChangedByZ21 = true;
            if (Lokdaten.Fahrstufe > Fahrstufe.Maximum) Fahrstufe.Value = Fahrstufe.Maximum;
            else Fahrstufe.Value = Lokdaten.Fahrstufe;
            
            if (Lokdaten.FahrstufenInfo == 0) StufenInfo.SelectedIndex = 0;
            else if (Lokdaten.FahrstufenInfo == 2) StufenInfo.SelectedIndex = 1;
            else if (Lokdaten.FahrstufenInfo == 4) StufenInfo.SelectedIndex = 2;
            
            if (Lokdaten.AktiveFunktion[0]) Fkt_Licht.BackColor = Color.Yellow;
            else Fkt_Licht.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[1]) Fkt1.BackColor = Color.White;
            else Fkt1.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[2]) Fkt2.BackColor = Color.White;
            else Fkt2.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[3]) Fkt3.BackColor = Color.White;
            else Fkt3.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[4]) Fkt4.BackColor = Color.White;
            else Fkt4.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[5]) Fkt5.BackColor = Color.White;
            else Fkt5.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[6]) Fkt6.BackColor = Color.White;
            else Fkt6.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[7]) Fkt7.BackColor = Color.White;
            else Fkt7.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[8]) Fkt8.BackColor = Color.White;
            else Fkt8.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[9]) Fkt9.BackColor = Color.White;
            else Fkt9.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[10]) Fkt10.BackColor = Color.White;
            else Fkt10.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[11]) Fkt11.BackColor = Color.White;
            else Fkt11.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[12]) Fkt12.BackColor = Color.White;
            else Fkt12.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[13]) Fkt13.BackColor = Color.White;
            else Fkt13.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[14]) Fkt14.BackColor = Color.White;
            else Fkt14.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[15]) Fkt15.BackColor = Color.White;
            else Fkt15.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[16]) Fkt16.BackColor = Color.White;
            else Fkt16.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[17]) Fkt17.BackColor = Color.White;
            else Fkt17.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[18]) Fkt18.BackColor = Color.White;
            else Fkt18.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[19]) Fkt19.BackColor = Color.White;
            else Fkt19.BackColor = Color.DarkGray;
            if (Lokdaten.AktiveFunktion[20]) Fkt20.BackColor = Color.White;
            else Fkt20.BackColor = Color.DarkGray;

            if (Lokdaten.LokUmgedreht) Fahrwechsel.BackColor = Color.White;
            else Fahrwechsel.BackColor = Color.DarkGray;

            if(Lokdaten.Richtung == LokFahrstufen.Vorwaerts)
            {
                vor.BackColor = Color.White;
                Ruck.BackColor = Color.DarkGray;
            }
            else
            {
                vor.BackColor = Color.DarkGray;
                Ruck.BackColor = Color.White;
            }
        }

        private void Fahrstufe_ValueChanged(object sender, EventArgs e)
        {
            //Grundgleisbild
            Bitmap bild = MEKB_H0_Anlage.Properties.Resources.FahrstufenAnzeige; //Gleisbild
            Graphics anzeige = Graphics.FromImage(bild); //In bearbeitbare Grafik umwandeln
            SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            int drawvalue;
            switch (StufenInfo.SelectedIndex)
            {
                case 0: drawvalue = 168 - (Fahrstufe.Value * 12); Fahrstufe.Maximum = 14; break; //14 Fahrstufen
                case 1: drawvalue = 168 - (Fahrstufe.Value * 6); Fahrstufe.Maximum = 28; break;  //28 Fahrstufen
                default: drawvalue = (int)(168 - (Fahrstufe.Value * 1.3125)); Fahrstufe.Maximum = 126; break; //128 Fahrstufen
            }

            Geschwindigkeit.Text = Lokdaten.Fahrstufe.ToString();
            anzeige.FillRectangle(myBrush, new Rectangle(0, 0, 64, drawvalue));

            bild.MakeTransparent(System.Drawing.Color.Black);

            FahrAnzeige.Image = bild;

            myBrush.Dispose();
            anzeige.Dispose();
            
            if(ValueChangedByZ21)
            {
                //Value Changed by Z21 - do nothing
                ValueChangedByZ21 = false;
            }
            else
            {
                //Value was changed by user - send Command
                Lokdaten.Fahrstufe = Fahrstufe.Value;
                setLOKFahrt?.Invoke(Lokdaten.Adresse, (byte)Lokdaten.Fahrstufe, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);              
            }

        }


        private void ZugSteuerpult_Shown(object sender, EventArgs e)
        {
            Fahrstufe.Value = 0;
            Fahrstufe_ValueChanged(sender, e);
        }


        private void Fkt_Set_Click(object sender, EventArgs e)
        {
            byte FunktionsNr;
            System.Windows.Forms.Button Obj = (Button)sender;
            switch (Obj.Name)
            {
                case "Fkt1": FunktionsNr = 1; break;
                case "Fkt2": FunktionsNr = 2; break;
                case "Fkt3": FunktionsNr = 3; break;
                case "Fkt4": FunktionsNr = 4; break;
                case "Fkt5": FunktionsNr = 5; break;
                case "Fkt6": FunktionsNr = 6; break;
                case "Fkt7": FunktionsNr = 7; break;
                case "Fkt8": FunktionsNr = 8; break;
                case "Fkt9": FunktionsNr = 9; break;
                case "Fkt10": FunktionsNr = 10; break;
                case "Fkt11": FunktionsNr = 11; break;
                case "Fkt12": FunktionsNr = 12; break;
                case "Fkt13": FunktionsNr = 13; break;
                case "Fkt14": FunktionsNr = 14; break;
                case "Fkt15": FunktionsNr = 15; break;
                case "Fkt16": FunktionsNr = 16; break;
                case "Fkt17": FunktionsNr = 17; break;
                case "Fkt18": FunktionsNr = 18; break;
                case "Fkt19": FunktionsNr = 19; break;
                case "Fkt20": FunktionsNr = 20; break;
                default: return;
            }
            if (Obj.BackColor != Color.White)
            {
                setLOKFunktion?.Invoke(Lokdaten.Adresse, 1, FunktionsNr);
            }
            else
            {
                setLOKFunktion?.Invoke(Lokdaten.Adresse, 0, FunktionsNr);
            }
        }

        private void Fkt_Licht_Click(object sender, EventArgs e)
        {
            if (Fkt_Licht.BackColor != Color.Yellow)
            {
                setLOKFunktion?.Invoke(Lokdaten.Adresse, 1, 0);
            }
            else
            {
                setLOKFunktion?.Invoke(Lokdaten.Adresse, 0, 0);
            }
        }

        private void ZugSteuerpult_Load(object sender, EventArgs e)
        {
            Adresse.Text = String.Format("Adresse: {0}", Lokdaten.Adresse);
            Rufnummer.Text = "Rufnummer: "+ LokKontrolle.Abkuerzung(Lokdaten.Gattung) + Lokdaten.Adresse.ToString();
            if(Lokdaten.Name == null)
            {
                Lokname.Text = String.Format("Lok: {0}", Lokdaten.Adresse);
            }
            else
            {
                if(!Lokdaten.Name.Equals(""))
                {
                    Lokname.Text = Lokdaten.Name;
                }
                else
                {
                    Lokname.Text = String.Format("Lok: {0}", Lokdaten.Adresse);
                }
            }
        }

        private void vor_Click(object sender, EventArgs e)
        {
            Lokdaten.Richtung = LokFahrstufen.Vorwaerts;
            setLOKFahrt?.Invoke(Lokdaten.Adresse, (byte)Lokdaten.Fahrstufe, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
        }

        private void Ruck_Click(object sender, EventArgs e)
        {
            Lokdaten.Richtung = LokFahrstufen.Rueckwaerts;
            setLOKFahrt?.Invoke(Lokdaten.Adresse, (byte)Lokdaten.Fahrstufe, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
        }

        private void Anhalten_Click(object sender, EventArgs e)
        {
            setLOKFahrt?.Invoke(Lokdaten.Adresse, 0, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
        }

        private void Notbremse_Click(object sender, EventArgs e)
        {
            setLOKFahrt?.Invoke(Lokdaten.Adresse, 255, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
        }

        private void Fahrwechsel_Click(object sender, EventArgs e)
        {
            Lokdaten.LokUmgedreht = !Lokdaten.LokUmgedreht;
            if (Lokdaten.LokUmgedreht) Fahrwechsel.BackColor = Color.White;
            else Fahrwechsel.BackColor = Color.DarkGray;
            setLOKFahrt?.Invoke(Lokdaten.Adresse, (byte)Lokdaten.Fahrstufe, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
        }
    }
}
