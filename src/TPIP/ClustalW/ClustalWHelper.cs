using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TPIP
{
    static class ClustalWHelper
    {
        static public void CreateMSAofHomologs()
        {
            Process proc = new Process();
            proc.StartInfo.WorkingDirectory = ConfigFileChecker.ClustalWDirectory;
            proc.StartInfo.FileName = Path.Combine(ConfigFileChecker.ClustalWDirectory, "clustalw2.exe");//@"D:\Programs\ClustalW2\clustalw2.exe";
            proc.StartInfo.Arguments = @"clustalw2.exe -ALIGN -INFILE=seq.txt -OUTORDER=INPUT -OUTPUT=FASTA";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            StreamReader myStreamReader = proc.StandardError;
            string errorString = myStreamReader.ReadToEnd();
            if (errorString != "")
            {
                Console.WriteLine("An error occured in MSA using ClustalW: ", errorString);
                // Environment.Exit(0);
            }
            else
            {
                proc.WaitForExit();
                proc.Close();
            }
        }
    }
}
