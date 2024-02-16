using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEKB_H0_Anlage
{
    public partial class Zugmenue : Form
    {
        #region Instanzen
        public Z21 z21;
        #endregion

        #region Listen
        public List<Lokomotive> AktiveLoks = new List<Lokomotive>();
        public LokomotivenVerwaltung LokomotivenArchiv;
        #endregion

        #region Parameter
        private int x_FelderStart = 10;
        private int y_FelderStart = 30;
        #endregion



        #region Threads
        public Thread ThreadLoksuche;
        #endregion



        public Zugmenue()
        {
            InitializeComponent();
        }

        public Zugmenue(Z21 zentrale, LokomotivenVerwaltung archiv)
        {
            z21 = zentrale;
            InitializeComponent();
            LokomotivenArchiv = archiv;

            ThreadLoksuche = new Thread(() => DialogHandhabungLokSuche(""));
        }

        private void LokDazu_Click(object sender, EventArgs e)
        {
            AktiveLoks.Add(new Lokomotive());
            AktiveLoks.Last().Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
            AktiveLoks.Last().Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
            GeneriereFelder(AktiveLoks.Count - 1);
        }

        private void LokEntfernen_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //Name der Instanz finden
                string name = button.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Loesch") return; //Muss auf _Name enden

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    if (index < 0) return;
                    if (index > AktiveLoks.Count) return;
                }
                else
                {
                    return;
                }

                AktiveLoks.RemoveAt(index);
                for(int i = 0; i < AktiveLoks.Count; i++)
                {
                    LokKontroll_UpdateAll(String.Format("LokCtrl{0}", i));
                }
                EntferneFelder(AktiveLoks.Count);
            }
        }

        private void GeneriereFelder(int Nummer)
        {
            Controls.Add(AdressfeldGenerator(Nummer));
            Controls.Add(SuchfeldGenerator(Nummer));
            Controls.Add(SteuertypfeldGenerator(Nummer));
            Controls.Add(GattungfeldGenerator(Nummer));
            Controls.Add(RuffeldGenerator(Nummer));
            Controls.Add(OrtwechselfeldGenerator(Nummer));
            Controls.Add(OrtfeldGenerator(Nummer));
            Controls.Add(OpenFahrpultGenerator(Nummer));
            Controls.Add(StopfeldGenerator(Nummer));
            Controls.Add(LoeschfeldGenerator(Nummer));
        }

        private void EntferneFelder(int Nummer)
        {
            NumericUpDown Adressfeld = (NumericUpDown)this.Controls.Find(String.Format("LokCtrl{0}_Adr", Nummer), true).First();
            if (Adressfeld != null) this.Controls.Remove(Adressfeld);

            Button Suchfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_Suche", Nummer), true).First();
            if (Suchfeld != null) this.Controls.Remove(Suchfeld);

            Button Steuertypfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_Strg_Typ", Nummer), true).First();
            if (Steuertypfeld != null) this.Controls.Remove(Steuertypfeld);

            ComboBox Gattungfeld = (ComboBox)this.Controls.Find(String.Format("LokCtrl{0}_Gattung", Nummer), true).First();
            if (Gattungfeld != null) this.Controls.Remove(Gattungfeld);

            TextBox Ruffeld = (TextBox)this.Controls.Find(String.Format("LokCtrl{0}_Ruf", Nummer), true).First();
            if (Ruffeld != null) this.Controls.Remove(Ruffeld);

            Button Ortwechselfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_Ort_Wechsel", Nummer), true).First();
            if (Ortwechselfeld != null) this.Controls.Remove(Ortwechselfeld);

            TextBox Ortfeld = (TextBox)this.Controls.Find(String.Format("LokCtrl{0}_Ort", Nummer), true).First();
            if (Ortfeld != null) this.Controls.Remove(Ortfeld);

            Button Fahrpultfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_OpenFahrpult", Nummer), true).First();
            if (Fahrpultfeld != null) this.Controls.Remove(Fahrpultfeld);

            Button Stopfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_Stop", Nummer), true).First();
            if (Stopfeld != null) this.Controls.Remove(Stopfeld);

            Button Loschfeld = (Button)this.Controls.Find(String.Format("LokCtrl{0}_Loesch", Nummer), true).First();
            if (Loschfeld != null) this.Controls.Remove(Loschfeld);

        }

        #region Felder
        private NumericUpDown AdressfeldGenerator(int index)
        {
            NumericUpDown LokCtrl_Adr = new NumericUpDown
            {
                Location = new Point(x_FelderStart + 22, y_FelderStart + (24 * index)),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Name = String.Format("LokCtrl{0}_Adr", index),
                Size = new Size(64, 20),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Right,
                UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left
            };
            LokCtrl_Adr.ValueChanged += new EventHandler(this.LokKontroll_Adr_ValueChanged);
            return LokCtrl_Adr;
        }

        private Button SuchfeldGenerator(int index)
        {
            Button LokCtrl_Suche = new Button
            {
                BackColor = System.Drawing.SystemColors.ControlLightLight,
                BackgroundImageLayout = System.Windows.Forms.ImageLayout.None,
                Location = new Point(x_FelderStart + 94, y_FelderStart + (24 * index) - 1),
                Name = String.Format("LokCtrl{0}_Suche", index),
                Size = new Size(168, 22),
                Text = "Lok suchen",
                UseVisualStyleBackColor = false
            };
            LokCtrl_Suche.Click += new EventHandler(this.SearchLok_Click);
            return LokCtrl_Suche;
        }
        private Button SteuertypfeldGenerator(int index)
        {
            Button LokCtrl_Strg_Typ = new Button
            {
                BackColor = System.Drawing.Color.Green,
                Enabled = true,
                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(x_FelderStart + 270, y_FelderStart + (24 * index) - 1),
                Name = String.Format("LokCtrl{0}_Strg_Typ", index),
                Size = new System.Drawing.Size(64, 22),
                Text = "Manuell",
                UseVisualStyleBackColor = false,
            };
            LokCtrl_Strg_Typ.Click += new EventHandler(this.LokKontroll_Strg_Typ_Click);
            return LokCtrl_Strg_Typ;
        }
        private ComboBox GattungfeldGenerator(int index)
        {
            ComboBox GattungFeldGenerator = new ComboBox
            {
                FormattingEnabled = true,
                Location = new System.Drawing.Point(x_FelderStart + 342, y_FelderStart + (24 * index)),
                Name = String.Format("LokCtrl{0}_Gattung", index),
                Size = new System.Drawing.Size(112, 19),
            };
            string[] Gattungen = LokKontrolle.ListeGattungsnamen();
            foreach (string Gattung in Gattungen)
            {
                GattungFeldGenerator.Items.Add(Gattung);
            }
            GattungFeldGenerator.SelectedIndexChanged += new System.EventHandler(this.LokKontroll_Update_Gattung);
            return GattungFeldGenerator;
        }
        private TextBox RuffeldGenerator(int index)
        {
            TextBox LokCtrl_Ruf = new TextBox
            {
                Enabled = false,
                Location = new Point(x_FelderStart + 462, y_FelderStart + (24 * index)),
                Name = String.Format("LokCtrl{0}_Ruf", index),
                ReadOnly = true,
                Size = new Size(72, 22)
            };
            return LokCtrl_Ruf;
        }
        private Button OrtwechselfeldGenerator(int index)
        {
            Button LokCtrl_Ort_Wechsel = new Button
            {
                Enabled = true,
                Location = new System.Drawing.Point(x_FelderStart + 542, y_FelderStart + (24 * index) - 1),
                Name = String.Format("LokCtrl{0}_Ort_Wechsel", index),
                Size = new System.Drawing.Size(72, 22),
                Text = "Ort ändern",
                UseVisualStyleBackColor = false,
            };
            //LokCtrl_Ort_Wechsel.Click += new EventHandler(this.LokKontroll_Strg_Typ_Click);
            return LokCtrl_Ort_Wechsel;
        }
        private TextBox OrtfeldGenerator(int index)
        {
            TextBox LokCtrl_Ort = new TextBox
            {
                Location = new System.Drawing.Point(x_FelderStart + 622, y_FelderStart + (24 * index)),
                Name = String.Format("LokCtrl{0}_Ort", index),
                ReadOnly = true,
                Size = new System.Drawing.Size(112, 22)
            };
            return LokCtrl_Ort;
        }
        private Button OpenFahrpultGenerator(int index)
        {
            Button OpenFahrpult = new Button
            {
                Location = new Point(x_FelderStart + 742, y_FelderStart + (24 * index)-1),
                Name = String.Format("LokCtrl{0}_OpenFahrpult", index),
                Size = new Size(56, 22),
                Text = "Fahrpult",
                UseVisualStyleBackColor = false
            };
            OpenFahrpult.Click += new EventHandler(this.LokKontroll_OpenFahrpult_Click);
            return OpenFahrpult;
        }
        private Button StopfeldGenerator(int index)
        {
            Button LokCtrl_Stop = new Button
            {
                BackColor = System.Drawing.Color.Red,
                FlatStyle = System.Windows.Forms.FlatStyle.Popup,
                ForeColor = System.Drawing.SystemColors.Control,
                Location = new Point(x_FelderStart + 806, y_FelderStart + (24 * index)),
                Name = String.Format("LokCtrl{0}_Stop", index),
                Size = new Size(48, 20),
                Text = "Stop",
                UseVisualStyleBackColor = false
            };
            LokCtrl_Stop.Click += new EventHandler(this.LokKontroll_Stop_Click);
            return LokCtrl_Stop;
        }
        private Button LoeschfeldGenerator(int index)
        {
            Button LokCtrl_Stop = new Button
            {
                Location = new Point(x_FelderStart + 862, y_FelderStart + (24 * index) - 1),
                Name = String.Format("LokCtrl{0}_Loesch", index),
                Size = new Size(22, 22),
                Text = "X",
                UseVisualStyleBackColor = false
            };
            LokCtrl_Stop.Click += new EventHandler(this.LokEntfernen_Click);
            return LokCtrl_Stop;
        }






        #endregion


        #region Loksteuerung
        /// <summary>
        /// Benutzer hat die Adresse in der aktiven Lokliste geändert
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void LokKontroll_Adr_ValueChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown Adressfeld)
            {
                //Name der Instanz finden
                string name = Adressfeld.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Adr") return; //Muss auf _Adr enden

                if (LokomotivenArchiv.SucheDurchAdresse(Adressfeld.Value, out Lokomotive lokomotive))//Finde Lok mit dieser Adresse 
                {
                    LokKontroll_UpdateAktiveLok(subs[0], lokomotive);
                }
                else // Lok nicht gefunden
                {
                    lokomotive = new Lokomotive() //Blanke Lok anlegen mit dieser Adresse
                    {
                        Adresse = ((int)Adressfeld.Value),
                        Name = String.Format("Lok: {0}", Adressfeld.Value)
                    };
                    LokKontroll_UpdateAktiveLok(subs[0], lokomotive);
                }
            }
        }
        /// <summary>
        /// Gattung wurde geändert
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void LokKontroll_Update_Gattung(object sender, EventArgs e)
        {
            if (sender is ComboBox Gattungsfeld)
            {
                //Name der Instanz finden
                string name = Gattungsfeld.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Gattung") return; //Muss auf _Gattung enden

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    if (index < 0) return;
                    if (index > AktiveLoks.Count) return;
                }
                else
                {
                    return;
                }

                AktiveLoks[index].Gattung = Gattungsfeld.Text;

                LokKontroll_UpdateAll(subs[0]);

            }
        }
        /// <summary>
        /// Button für Fahrpult wurde gedrückt -> Neues Fenster mit Fahrpult für diese Lok wird geöffnet
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void LokKontroll_OpenFahrpult_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //Name der Instanz finden
                string name = button.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "OpenFahrpult") return; //Muss auf _Name enden

                //Passendes Adressfeld heraussuchen
                NumericUpDown Adressfeld = (NumericUpDown)this.Controls.Find(subs[0] + "_Adr", true)[0];
                if (Adressfeld == null) return; //Nicht gefunden: Abbrechen

                //Adresse ist 0 oder noch nicht definiert. Fenster nicht öfnnen
                if (Adressfeld.Value == 0)
                {
                    MessageBox.Show("Keine Lok gewählt", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    if (index < 0) return;
                    if (index > AktiveLoks.Count) return;
                }
                else
                {
                    return;
                }

                //Wenn bereits geöffnet. Alten deaktivieren
                if (!AktiveLoks[index].Steuerpult.IsDisposed)
                {
                    AktiveLoks[index].Steuerpult.Dispose();
                }

                //Neuen Fahrpult anlegen
                AktiveLoks[index].Steuerpult = new ZugSteuerpult(AktiveLoks[index]);
                //Funktionen zur Loksteuerung zuweisen
                AktiveLoks[index].Register_CMD_LOKFUNKTION(Setze_Lok_Funktion);
                AktiveLoks[index].Register_CMD_LOKFAHRT(Setze_Lok_Fahrt);
                AktiveLoks[index].Register_CMD_LOKSTATUS(Setze_Lok_Status);
                //Fahrpult öffnen
                AktiveLoks[index].Steuerpult.Show();
                //Anfrage an Z21: Aktuelle Lokomotiv-Daten anfragen
                z21.Z21_GET_LOCO_INFO(AktiveLoks[index].Adresse);

            }
        }
        /// <summary>
        /// Button gedrückt Steuerungs Typ wechseln
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void LokKontroll_Strg_Typ_Click(object sender, EventArgs e)
        {
            if (sender is Button steuertyp)
            {
                //Name der Instanz finden
                string name = steuertyp.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Strg") return; //Muss auf _Strg an zweiter Position haben

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    if (index < 0) return;
                    if (index > AktiveLoks.Count) return;
                }
                else
                {
                    return;
                }

                // Wert ändern: War zuvor manuell?
                if (!AktiveLoks[index].Automatik)
                {
                    //Auf Automatik umschalten
                    steuertyp.Text = "Automatik";
                    steuertyp.BackColor = Color.FromArgb(0, 0, 255);
                    AktiveLoks[index].Automatik = true;
                }
                else //War zuvor Automatisch
                {
                    //Auf Menuell umschalten
                    steuertyp.Text = "Manuell";
                    steuertyp.BackColor = Color.FromArgb(0, 128, 0);
                    AktiveLoks[index].Automatik = false;
                }
            }
        }
        /// <summary>
        /// Button Lok anhalten wurde gedrückt
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void LokKontroll_Stop_Click(object sender, EventArgs e)
        {
            if (sender is Button stop)
            {
                //Name der Instanz finden
                string name = stop.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Stop") return; //Muss auf _Stop an zweiter Position haben

                //Aktiver Lokindex
                string indexStr = subs[0].Substring(7);
                if (Int32.TryParse(indexStr, out int index))
                {
                    if (index < 0) return;
                    if (index > AktiveLoks.Count) return;
                }
                else
                {
                    return;
                }
                // Gebe Fahrbefehl Nothalt für angegebene Adresse
                if (AktiveLoks[index].Adresse != 0)
                {
                    Setze_Lok_Fahrt(AktiveLoks[index].Adresse, 255, AktiveLoks[index].Richtung, AktiveLoks[index].FahrstufenInfo);
                }
            }
        }
        /// <summary>
        /// Unterfunktion: Wechsle aktive Lok
        /// </summary>
        /// <param name="CtrlID">Kontroll-ID "LokCtrl1" -> index = 1, "LokCtrl5" -> index = 5</param>
        /// <param name="lokomotive">Neue Lokomotive die diesen Index einnehmen soll</param>
        public void LokKontroll_UpdateAktiveLok(string CtrlID, Lokomotive lokomotive)
        {
            //Index der Lok lesen
            string indexStr = CtrlID.Substring(7);
            if (Int32.TryParse(indexStr, out int index))
            {
                if (index < 0) return;
                if (index > AktiveLoks.Count) return;
            }
            else //Kein Index erkannt
            {
                return;
            }

            //Alten Steuerpult schließen
            if (AktiveLoks[index].Steuerpult != null)
            {
                AktiveLoks[index].Steuerpult.Close();
            }
            //Lok übertragen
            AktiveLoks[index] = lokomotive;
            AktiveLoks[index].Automatik = false; //Lok standardmäßig auf manuell
            //Alle Fenster updaten
            LokKontroll_UpdateAll(CtrlID);
        }
        /// <summary>
        /// Neue Texte in alle ELemente der aktiven Lok aktualisieren
        /// </summary>
        /// <param name="CtrlID">Kontroll-ID "LokCtrl1" -> index = 1, "LokCtrl5" -> index = 5</param>
        public void LokKontroll_UpdateAll(string CtrlID)
        {
            NumericUpDown Adressfeld;
            Button SuchFeld;
            ComboBox Gattungsfeld;
            Button Steuertypfeld;
            TextBox Ruffeld;

            try
            {
                //Passendes Adressfeld heraussuchen
                Adressfeld = (NumericUpDown)this.Controls.Find(CtrlID + "_Adr", true).First();
                if (Adressfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Steuertypfeld finden
                SuchFeld = (Button)this.Controls.Find(CtrlID + "_Suche", true).First();
                if (SuchFeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Gattungsfeld finden
                Gattungsfeld = (ComboBox)this.Controls.Find(CtrlID + "_Gattung", true)[0];
                if (Gattungsfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Steuertypfeld finden
                Steuertypfeld = (Button)this.Controls.Find(CtrlID + "_Strg_Typ", true)[0];
                if (Steuertypfeld == null) return; //Nicht gefunden: Abbrechen

                //Passendes Ruffeld finden
                Ruffeld = (TextBox)this.Controls.Find(CtrlID + "_Ruf", true)[0];
                if (Ruffeld == null) return; //Nicht gefunden: Abbrechen
            }
            catch
            {
                return;
            }

            string indexStr = CtrlID.Substring(7);
            if (Int32.TryParse(indexStr, out int index))
            {
                if (index < 0) return;
                if (index > AktiveLoks.Count) return;
            }
            else //Kein Index erkannt
            {
                return;
            }
            if (AktiveLoks[index] == null) return;
            //Adresse übernehmen
            Adressfeld.Value = AktiveLoks[index].Adresse;
            //Name übernehmen
            SuchFeld.Text = AktiveLoks[index].Name;
            //Gattung übernehmen
            Gattungsfeld.Text = AktiveLoks[index].Gattung;
            //Rufnummer generieren
            if (AktiveLoks[index].Adresse != 0) Ruffeld.Text = LokKontrolle.Abkuerzung(AktiveLoks[index].Gattung) + AktiveLoks[index].Adresse.ToString();
            else Ruffeld.Text = "";

            //Automatik-Button anpassen
            if (AktiveLoks[index].Automatik)
            {
                Steuertypfeld.Text = "Automatik";
                Steuertypfeld.BackColor = Color.FromArgb(0, 0, 255);
            }
            else
            {
                Steuertypfeld.Text = "Manuell";
                Steuertypfeld.BackColor = Color.FromArgb(0, 128, 0);
            }
        }
        /// <summary>
        /// Button für Loksuche gedrückt. Öffne Fenster für die Loksuche. Fenster wird in einem separaten Thread gehandhabt. Verhindert blockieren und beschränkt es auf einen Suchfenster
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void SearchLok_Click(object sender, EventArgs e)
        {
            if (sender is Button stop)
            {
                //Name der Instanz finden
                string name = stop.Name;
                string[] subs = name.Split('_');
                if (subs[1] != "Suche") return; //Muss auf _Strg an zweiter Position haben

                //Fenster Loksuche ist noch nicht geöffnet
                if (!ThreadLoksuche.IsAlive)
                {
                    ThreadLoksuche = new Thread(() => DialogHandhabungLokSuche(subs[0]));
                    ThreadLoksuche.Start();
                }
            }
        }
        /// <summary>
        /// Theadfunktion: Fenster öffnen und auf Dialogantwort warten
        /// </summary>
        /// <param name="IndexName">Index-Name auf den die neue gesuchte Lok angelegt werden soll</param>
        private void DialogHandhabungLokSuche(string IndexName)
        {
            //Fenster anlegen
            LokSuche lokSuche = new LokSuche(LokomotivenArchiv);
            //Fenster anzeigen und auf Antwort warten
            DialogResult dialogResult = lokSuche.ShowDialog();
            //Suche erfolgreich bzw. nicht abgebrochen
            if (dialogResult == DialogResult.OK)
            {
                //Ergebnis über synchronisierte Aktion in Fenster schreiben
                this.BeginInvoke((Action<string, Lokomotive>)SchreibeSuchergebnis, IndexName, lokSuche.GewaehlteLok);
            }
        }
        /// <summary>
        /// Aktion für Thread: Kapselung von LokKontroll_UpdateAktiveLok
        /// </summary>
        /// <param name="IndexName">Index-Name auf den die neue gesuchte Lok angelegt werden soll</param>
        /// <param name="gefundeneLok">Neu gefundene Lok, die in diesen Index eingetragen werden soll</param>
        private void SchreibeSuchergebnis(string IndexName, Lokomotive gefundeneLok)
        {
            LokKontroll_UpdateAktiveLok(IndexName, gefundeneLok);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Lok-Geschwindigkeit setzen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        /// <param name="Fahrstufe">Aktuelle Fahrstufe</param>
        /// <param name="Richtung">Fahrtrichtung</param>
        /// <param name="Fahstrufeninfo">14,28 oder 128 Fahrstufen</param>
        private void Setze_Lok_Fahrt(int Adresse, byte Fahrstufe, int Richtung, byte Fahstrufeninfo)
        {
            z21.Z21_SET_LOCO_DRIVE(Adresse, Fahrstufe, Richtung, Fahstrufeninfo);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Lok-Funktion setzen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        /// <param name="Zustand">Funktion an oder ausschalten</param>
        /// <param name="FunktionsNr">Funktionsnummer</param>
        private void Setze_Lok_Funktion(int Adresse, byte Zustand, byte FunktionsNr)
        {
            z21.Z21_SET_LOCO_FUNCTION(Adresse, Zustand, FunktionsNr);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Aktuelle Lokinformationen von der Z21 abfragen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        private void Setze_Lok_Status(int Adresse)
        {
            z21.Z21_GET_LOCO_INFO(Adresse);
        }
        /// <summary>
        /// Alle Lokomotiven anhalten
        /// </summary>
        /// <param name="sender">Forms-Element, was diese Funktion ausgelöst hatte</param>
        /// <param name="e">Eventparameter</param>
        private void StopAlle_Click(object sender, EventArgs e)
        {
            foreach (Lokomotive lok in AktiveLoks)
            {
                if (lok.Adresse != 0)
                {
                    Setze_Lok_Fahrt(lok.Adresse, 255, lok.Richtung, lok.FahrstufenInfo);
                }
            }
        }

        #endregion

        private void Zugmenue_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form verstecken und Close-Funktion stoppen (Werte werden nach schließen beibehalten
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

       
    }
}
