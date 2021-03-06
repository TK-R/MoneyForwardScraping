﻿using System;
using System.IO;
using System.Xml.Linq;
using MoneyForward;

namespace MFSCraping
{
    class Program
    {
        static  void Main(string[] args)
        {
            var path = Environment.CurrentDirectory + "\\Data\\Auth.xml";
            string mail = "", pass = "";

            if (File.Exists(path))
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
            
            // Login 
            var mflib = new MFLib();
            if(!mflib.LoginAsync(mail, pass).Result)
                return;

            var res = mflib.GetAllAssetWithDiff();

            Console.WriteLine(res.Result);
        }
    }
}
