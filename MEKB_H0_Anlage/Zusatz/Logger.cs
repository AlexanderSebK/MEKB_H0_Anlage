using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MEKB_H0_Anlage
{
    public class Logger
    {
        public Logger(string file)
        {
            if(!File.Exists(file))
            {
                File.CreateText(file);
            }
            FileToWrite = file;
        }

        private string FileToWrite;

        public void Info(string text)
        {
            try
            {
                using (StreamWriter w = File.AppendText(FileToWrite))
                {
                    w.Write("\r\n");
                    w.Write($"{DateTime.Now.ToString("hh:mm:ss.fff")}");
                    w.Write($"  :{text}");
                }
            }
            catch
            {

            }
        }
        public void SendData(string protocolName, byte[] data)
        {
            string text = "\r\n";
            text += DateTime.Now.ToString("hh:mm:ss.fff");
            text += String.Format(" PC => Z21: {0,30} - ", protocolName);
            foreach(byte b in data)
            {
                text += String.Format(" {0,2}", b.ToString("X2"));
            }
            try
            {
                using (StreamWriter w = File.AppendText(FileToWrite))
                {
                    w.Write(text);
                }
            }
            catch
            {

            }
        }

        public void ReceivedData(string protocolName, byte[] data)
        {
            string text = "\r\n";
            text += DateTime.Now.ToString("hh:mm:ss.fff");
            text += String.Format(" PC <= Z21: {0,30} - ", protocolName);
            foreach (byte b in data)
            {
                text += String.Format(" {0,2}", b.ToString("X2"));
            }
            try
            {
                using (StreamWriter w = File.AppendText(FileToWrite))
                {
                    w.Write(text);
                }
            }
            catch
            {

            }
        }

    }
}
