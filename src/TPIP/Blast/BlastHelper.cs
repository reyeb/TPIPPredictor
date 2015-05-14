using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TPIP
{
    class BlastHelper
    {
        /// <summary>
        /// Blasts against PDB to generate all homologs
        /// </summary>
        public static void Blast(string argument)
        {
            Process proc = new Process();
            proc.StartInfo.WorkingDirectory = ConfigFileChecker.NCBIBlastDirectory;
            proc.StartInfo.FileName = Path.Combine(ConfigFileChecker.NCBIBlastDirectory, "blastp.exe");
            proc.StartInfo.Arguments = argument;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            StreamReader myStreamReader = proc.StandardError;
            string errorString = myStreamReader.ReadToEnd();
            if (errorString != "")
            {
                Console.WriteLine("An error occured in blasting against PDB: ", errorString);
                //  Environment.Exit(0);
            }
            else
            {
                proc.WaitForExit();
                proc.Close();
            }

        }


    }
}
