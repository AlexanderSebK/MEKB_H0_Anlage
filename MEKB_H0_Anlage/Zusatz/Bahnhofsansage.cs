using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Threading;
using System.IO;


namespace MEKB_H0_Anlage
{
    public class Bahnhofsansage
    {

        private SpeechSynthesizer sprecher { get; set; }

        private Thread Ansprache { get; set; }

        public Bahnhofsansage()
        {
            sprecher = new SpeechSynthesizer();
            sprecher.SetOutputToDefaultAudioDevice();
            //Geschwindigkeit (-10 - 10)
            sprecher.Rate = 0;
            //Lautstärke (0-100)
            sprecher.Volume = 100;
            //Such passende Stimme zu angegebenen Argumenten
            try
            {
                sprecher.SelectVoice("Microsoft Hedda Desktop ");
            }
            catch
            {

            }
            //sprecher.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("de-DE"));
            
        }


        public void Ansage(Lokomotive lokomotive, string rufname, string text_davor, string text_danach)
        {
            string AutoRufname = LokKontrolle.Abkuerzung(lokomotive.Gattung) + lokomotive.Adresse.ToString();

            if(AutoRufname.Equals(rufname))
            {
                rufname = LokKontrolle.Sprachausgabe(lokomotive.Gattung);

                if(lokomotive.Adresse > 999)
                {
                    int topAdress = lokomotive.Adresse / 100;
                    int lowAdress = lokomotive.Adresse % 100;

                    rufname += "," + topAdress.ToString() + " " + lowAdress.ToString("00");
                }
                else
                {
                    rufname += " " + lokomotive.Adresse.ToString();
                }
            }

            Ansprache = new Thread(() => Sprachausgabe(rufname, lokomotive, text_davor, text_danach));
            Ansprache.Start();

        }

        public void Sprachausgabe(string ruf, Lokomotive lokomotive, string text_davor, string text_danach)
        {
            string text = ruf;
            if (!lokomotive.Zielbahnhof.Equals("")) text += " , nach , " + lokomotive.Zielbahnhof;
            if (!lokomotive.Zwischenbahnhoefe.Equals("")) text += " , über , " + lokomotive.Zwischenbahnhoefe + ". ";

            if (!text_davor.Equals("")) text = text_davor + ". " + text;
            if (!text_danach.Equals("")) text += text_danach;

            sprecher.Speak(text);
           
        }



    }
}
