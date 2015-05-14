using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace TPIP
{
    class FastaFileManager
    {
        public static List<string> fasta_Sequence_List = new List<string>();

    
        public static void Fillfasta_Sequence_List()
        {
            ///checks if this file exists. if not it first downloads it
            var fastaFilePath = @"./pdb_seqres.txt";
            if (!File.Exists(fastaFilePath))
            {
                DownloadFatsFile(fastaFilePath); 
            }
            var content = File.ReadAllText(fastaFilePath);
            fasta_Sequence_List = content.Split('>').ToList();

        }
        public bool CheckIfHomologIsaComplex(string pdbcode)
        {
            var ChainCount = fasta_Sequence_List.Where(a => a.StartsWith(pdbcode.ToLower())).Select(a => a).Count();
            if (ChainCount > 1) return true;
            else
                return false;
        }

        static void DownloadFatsFile(string path)
        {
            Console.WriteLine("Begin downloading pdb_seqres.txt file from: ftp://ftp.wwpdb.org/pub/pdb/derived_data");
            var url = @"ftp://ftp.wwpdb.org/pub/pdb/derived_data/pdb_seqres.txt";
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(url, path);
            }
            catch (Exception ex)
            {
                throw new ValidationException("An Error occured while downloading a file from ftp://ftp.wwpdb.org/pub/pdb/derived_data/pdb_seqres.txt, are you sure you are coneccted to the internet?"+ex.Message);
            }
            Console.WriteLine("pdb_seqres.txt is saved in "+path);
        }

    }
}
