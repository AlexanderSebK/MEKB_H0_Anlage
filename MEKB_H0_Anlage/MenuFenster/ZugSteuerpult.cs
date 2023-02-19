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
        private Lokomotive Lokdaten { set; get; }
        public ZugSteuerpult(Lokomotive Instance)
        {
            InitializeComponent();
            Lokdaten = Instance;
            UpdateLokDaten();
        }

        public delegate void CMD_LOKFAHRT(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo);
        public delegate void CMD_LOKFUNKTION(int Adresse, byte Zustand, byte FunktionsNr);
        public delegate void CMD_LOKSTATUS(int Adresse);

        private CMD_LOKFAHRT setLOKFahrt;
        private CMD_LOKFUNKTION setLOKFunktion;
        private CMD_LOKSTATUS setLOKStatus;

        public void Register_CMD_LOKFAHRT(CMD_LOKFAHRT function) { setLOKFahrt = function; }
        public void Register_CMD_LOKFUNKTION(CMD_LOKFUNKTION function) { setLOKFunktion = function; }
        public void Register_CMD_LOKSTATUS(CMD_LOKSTATUS function) { setLOKStatus = function; }

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

            
            anzeige.FillRectangle(myBrush, new Rectangle(0, 0, 64, drawvalue));

            bild.MakeTransparent(System.Drawing.Color.Black);

            FahrAnzeige.Image = bild;

            myBrush.Dispose();
            anzeige.Dispose();
            
            if(ValueChangedByZ21)
            {
                //Value Changed by Z21 - do nothing
                ValueChangedByZ21 = false;
                Geschwindigkeit.Text = Lokdaten.Fahrstufe.ToString();
            }
            else
            {
                //Value was changed by user - send Command
                Lokdaten.Fahrstufe = Fahrstufe.Value;
                setLOKFahrt?.Invoke(Lokdaten.Adresse, (byte)Lokdaten.Fahrstufe, Lokdaten.Richtung, Lokdaten.FahrstufenInfo);
                Geschwindigkeit.Text = Fahrstufe.Value.ToString();
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
            if (Lokdaten.Funktionen.Count >= 2)
            {
                if (Lokdaten.Funktionen[1] != null) { Fkt1.Text = Lokdaten.Funktionen[1]; Fkt1.Enabled = true; }
                else { Fkt1.Text = ""; Fkt1.Enabled = false; }
            }
            else { Fkt1.Text = ""; Fkt1.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 3)
            {
                if (Lokdaten.Funktionen[2] != null) { Fkt2.Text = Lokdaten.Funktionen[2]; Fkt2.Enabled = true; }
                else { Fkt2.Text = ""; Fkt2.Enabled = false; }
            }
            else { Fkt2.Text = ""; Fkt2.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 4)
            {
                if (Lokdaten.Funktionen[3] != null) { Fkt3.Text = Lokdaten.Funktionen[3]; Fkt3.Enabled = true; }
                else { Fkt3.Text = ""; Fkt3.Enabled = false; }
            }
            else { Fkt3.Text = ""; Fkt3.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 5)
            {
                if (Lokdaten.Funktionen[4] != null) { Fkt4.Text = Lokdaten.Funktionen[4]; Fkt4.Enabled = true; }
                else { Fkt4.Text = ""; Fkt4.Enabled = false; }
            }
            else { Fkt4.Text = ""; Fkt4.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 6)
            {
                if (Lokdaten.Funktionen[5] != null) { Fkt5.Text = Lokdaten.Funktionen[5]; Fkt5.Enabled = true; }
                else { Fkt5.Text = ""; Fkt5.Enabled = false; }
            }
            else { Fkt5.Text = ""; Fkt5.Enabled = false; }

            if (Lokdaten.Funktionen.Count >= 7)
            {
                if (Lokdaten.Funktionen[6] != null) { Fkt6.Text = Lokdaten.Funktionen[6]; Fkt6.Enabled = true; }
                else { Fkt6.Text = ""; Fkt6.Enabled = false; }
            }
            else { Fkt6.Text = ""; Fkt6.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 8)
            {
                if (Lokdaten.Funktionen[7] != null) { Fkt7.Text = Lokdaten.Funktionen[7]; Fkt7.Enabled = true; }
                else { Fkt7.Text = ""; Fkt7.Enabled = false; }
            }
            else { Fkt7.Text = ""; Fkt7.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 9)
            {
                if (Lokdaten.Funktionen[8] != null) { Fkt8.Text = Lokdaten.Funktionen[8]; Fkt8.Enabled = true; }
                else { Fkt8.Text = ""; Fkt8.Enabled = false; }
            }
            else { Fkt8.Text = ""; Fkt8.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 10)
            {
                if (Lokdaten.Funktionen[9] != null) { Fkt9.Text = Lokdaten.Funktionen[9]; Fkt9.Enabled = true; }
                else { Fkt9.Text = ""; Fkt9.Enabled = false; }
            }
            else { Fkt9.Text = ""; Fkt9.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 11)
            {
                if (Lokdaten.Funktionen[10] != null) { Fkt10.Text = Lokdaten.Funktionen[10]; Fkt10.Enabled = true; }
                else { Fkt10.Text = ""; Fkt10.Enabled = false; }
            }
            else { Fkt10.Text = ""; Fkt10.Enabled = false; }


            if (Lokdaten.Funktionen.Count >= 12)
            {
                if (Lokdaten.Funktionen[11] != null) { Fkt11.Text = Lokdaten.Funktionen[11]; Fkt11.Enabled = true; }
                else { Fkt11.Text = ""; Fkt11.Enabled = false; }
            }
            else { Fkt11.Text = ""; Fkt11.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 13)
            {
                if (Lokdaten.Funktionen[12] != null) { Fkt12.Text = Lokdaten.Funktionen[12]; Fkt12.Enabled = true; }
                else { Fkt12.Text = ""; Fkt12.Enabled = false; }
            }
            else { Fkt12.Text = ""; Fkt12.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 14)
            {
                if (Lokdaten.Funktionen[13] != null) { Fkt13.Text = Lokdaten.Funktionen[13]; Fkt13.Enabled = true; }
                else { Fkt13.Text = ""; Fkt13.Enabled = false; }
            }
            else { Fkt13.Text = ""; Fkt13.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 15)
            {
                if (Lokdaten.Funktionen[14] != null) { Fkt14.Text = Lokdaten.Funktionen[14]; Fkt14.Enabled = true; }
                else { Fkt14.Text = ""; Fkt14.Enabled = false; }
            }
            else { Fkt14.Text = ""; Fkt14.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 16)
            {
                if (Lokdaten.Funktionen[15] != null) { Fkt15.Text = Lokdaten.Funktionen[15]; Fkt15.Enabled = true; }
                else { Fkt15.Text = ""; Fkt15.Enabled = false; }
            }
            else { Fkt15.Text = ""; Fkt15.Enabled = false; }

            if (Lokdaten.Funktionen.Count >= 17)
            {
                if (Lokdaten.Funktionen[16] != null) { Fkt16.Text = Lokdaten.Funktionen[16]; Fkt16.Enabled = true; }
                else { Fkt16.Text = ""; Fkt16.Enabled = false; }
            }
            else { Fkt16.Text = ""; Fkt16.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 18)
            {
                if (Lokdaten.Funktionen[17] != null) { Fkt17.Text = Lokdaten.Funktionen[17]; Fkt17.Enabled = true; }
                else { Fkt17.Text = ""; Fkt17.Enabled = false; }
            }
            else { Fkt17.Text = ""; Fkt17.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 19)
            {
                if (Lokdaten.Funktionen[18] != null) { Fkt18.Text = Lokdaten.Funktionen[18]; Fkt18.Enabled = true; }
                else { Fkt18.Text = ""; Fkt18.Enabled = false; }
            }
            else { Fkt18.Text = ""; Fkt18.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 20)
            {
                if (Lokdaten.Funktionen[19] != null) { Fkt19.Text = Lokdaten.Funktionen[19]; Fkt19.Enabled = true; }
                else { Fkt19.Text = ""; Fkt19.Enabled = false; }
            }
            else { Fkt19.Text = ""; Fkt19.Enabled = false; }
            if (Lokdaten.Funktionen.Count >= 21)
            {
                if (Lokdaten.Funktionen[20] != null) { Fkt20.Text = Lokdaten.Funktionen[10]; Fkt20.Enabled = true; }
                else { Fkt20.Text = ""; Fkt20.Enabled = false; }
            }
            else { Fkt20.Text = ""; Fkt20.Enabled = false; }

            if (Lokdaten.Name == null)
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

        private void StufenInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(this.StufenInfo.SelectedIndex)
            {
                case 0: Lokdaten.FahrstufenInfo = LokFahrstufen.Fahstufe14;
                    this.Fahrstufe.Maximum = 14; 
                    this.Fahrstufe.Value = 0; 
                    break;
                case 1: Lokdaten.FahrstufenInfo = LokFahrstufen.Fahstufe28;
                    this.Fahrstufe.Maximum = 28;
                    this.Fahrstufe.Value = 0; 
                    break;
                default: Lokdaten.FahrstufenInfo = LokFahrstufen.Fahstufe128;
                    this.Fahrstufe.Maximum = 128;
                    this.Fahrstufe.Value = 0; 
                    break;
            }
            
        }
    }
}
