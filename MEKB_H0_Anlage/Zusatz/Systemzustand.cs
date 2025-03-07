using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEKB_H0_Anlage
{
    public class Systemzustand
    {
        private static readonly Lazy<Systemzustand> lazy =
        new Lazy<Systemzustand>(() => new Systemzustand());
        public static Systemzustand Instance { get { return lazy.Value; } }

        Systemzustand() 
        {
            Betriebsbereit = false;
        }

        // Zentrale ist verbunden und initialisieren
        public bool Betriebsbereit;
    }
}
