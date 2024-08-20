using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using iEPG;
using iEPG.Entity;
using Microsoft.VisualBasic;

namespace iEPGService
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Application.Run(new iEPGContext());
            using (var serviceHost = new ServiceHost(typeof(iEPGService)))
            {
                serviceHost.Open();
                Console.ReadLine();
                serviceHost.Close();
            }

        }
    }
}
