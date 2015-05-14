using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TPIP
{
    class ConfigFileChecker
    {
        //public static string FastaFilePath { get; set; }
        public static string NCBIBlastDirectory { get; set; }
        public static string ClustalWDirectory { get; set; }

        public static void SetConfiguratedValues()
        {
            //FastaFilePath = ConfigurationManager.AppSettings["FastaFilePath"];
            NCBIBlastDirectory = ConfigurationManager.AppSettings["NCBIBlastDirectory"];
            ClustalWDirectory = ConfigurationManager.AppSettings["ClustalWDirectory"];
        }
    }
}
