using System;
using System.Timers;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEKB_H0_Anlage
{
    public partial class Hauptform : Form
    {
        
        /// <summary>
        /// Anfrage an Z21 senden: Letzten Status des Signals abfragen
        /// </summary>
        /// <param name="Signalname">Name des Signals</param>
        private void GetSignalStatus_Z21(string Signalname)
        {

            Signal signal = SignalListe.GetSignal(Signalname); //Weiche mit diesem Namen in der Liste suchen
            if (signal == null) return;                                               //Weiche nicht vorhanden, Funktion abbrechen
            int Adresse = signal.Adresse;                             //Adresse der Weiche übernehmen
            z21Start.LAN_X_GET_TURNOUT_INFO(Adresse);                                       //paket senden "GET Weiche"
            Adresse = signal.Adresse2;                             //Adresse der Weiche übernehmen
            z21Start.LAN_X_GET_TURNOUT_INFO(Adresse);                                       //paket senden "GET Weiche"
        }

    }
}
