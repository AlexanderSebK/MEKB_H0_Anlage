using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace MEKB_H0_Anlage
{
    public partial class Form1 : Form
    {
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
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
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
                // Prüfen ob Initialisierung der Z21 fertig ist
                if (!(Weichen_Init & Signal_Init))
                {
                    MessageBox.Show("Initialisierung der Z21 nicht abgeschlossen. Bitte warten und erneut versuchen", "Fenster kann nicht geöffnet werden", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

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
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
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
                z21Start.Z21_GET_LOCO_INFO(AktiveLoks[index].Adresse);

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
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
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
                    index--;
                    if (index < 0) return;
                    if (index > AktiveLoks.Length) return;
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
                index--; //Start bei 0
                if (index < 0) return;
                if (index > AktiveLoks.Length) return;
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
                index--; //Start bei 0
                if (index < 0) return;
                if (index > AktiveLoks.Length) return;
            }
            else //Kein Index erkannt
            {
                return;
            }
            //Adresse übernehmen
            Adressfeld.Value = AktiveLoks[index].Adresse;
            //Name übernehmen
            SuchFeld.Text = AktiveLoks[index].Name;
            //Gattung übernehmen
            Gattungsfeld.Text = AktiveLoks[index].Gattung;
            //Rufnummer generieren
            if (AktiveLoks[index].Adresse != 0) Ruffeld.Text = LokomotivenArchiv.Abkuerung[AktiveLoks[index].Gattung] + AktiveLoks[index].Adresse.ToString();
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
            z21Start.Z21_SET_LOCO_DRIVE(Adresse, Fahrstufe, Richtung, Fahstrufeninfo);
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
            z21Start.Z21_SET_LOCO_FUNCTION(Adresse, Zustand, FunktionsNr);
        }
        /// <summary>
        /// Delegate-Funktion. Wird benutzt um externe Instance (Hier neues Fahrpultfenster) auf Z21 Funktionen zuzugreifen
        /// Aktuelle Lokinformationen von der Z21 abfragen
        /// </summary>
        /// <param name="Adresse">Lokadresse</param>
        private void Setze_Lok_Status(int Adresse)
        {
            z21Start.Z21_GET_LOCO_INFO(Adresse);
        }
    }
}
