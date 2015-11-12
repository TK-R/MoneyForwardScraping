using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MFSCraping
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory + "\\Data\\Auth.xml";
            string mail, pass;

            // mail pass 
            if (args.Length >= 2)
            {
                mail = args[0];
                pass = args[1];
            }
            else if (File.Exists(path))
            {

                var doc = XDocument.Load(path);
                try
                {
                    mail = doc.Element("root").Element("mail").Value;
                    pass = doc.Element("root").Element("pass").Value;

                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Invalid xml file.");
                }
            }
            else
            {
                Console.WriteLine("MFScraping.exe <mail> <pass> or Create Auth.xml");
                return;
            }



        }
    }
}
