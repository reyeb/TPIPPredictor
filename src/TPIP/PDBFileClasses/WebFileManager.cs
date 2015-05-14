using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace TPIP
{
    class WebFileManager
    {
        static public  void GetPDBFile(string pdbCode, string chain)
        {
            DownloadPDBFiles(pdbCode.ToUpper());
            DecompressFiles(pdbCode.ToUpper(), chain);
        }
       static void  DownloadPDBFiles(string pdbCode)
        {
            //var url = @"http://www.rcsb.org/pdb/download/downloadFile.do?fileFormat=pdb&compression=NO&structureId=";
            var url = @"ftp://ftp.wwpdb.org/pub/pdb/data/structures/divided/pdb/{0}/pdb{1}.ent.gz";
            var currentUrl = String.Format(url, pdbCode.Substring(1, 2).ToLower(), pdbCode.ToLower());
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(currentUrl, Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, pdbCode.ToUpper() + ".ent.gz"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured in downloading PDB Files: ", ex.Message);
            }
            Thread.Sleep(100);
        }

         static void DecompressFiles(string pdbCode, string chain)
        {
            var file = Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, pdbCode + ".ent.gz");
            using (FileStream fInStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                {
                    var newname = file.Replace(".ent.gz", "-" + chain.ToUpper() + ".pdb");
                    // newname = newname.Replace("ZippedPDBSumHBFiles", "PDBSumHBFiles");
                    using (FileStream fOutStream = new FileStream(newname, FileMode.Create, FileAccess.Write))
                    {
                        byte[] tempBytes = new byte[4096];
                        int i;
                        while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                        {
                            fOutStream.Write(tempBytes, 0, i);
                        }
                    }
                }
            }
            File.Delete(file);
        }

    }
}
