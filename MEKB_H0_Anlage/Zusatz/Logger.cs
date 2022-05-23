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
    }
}
