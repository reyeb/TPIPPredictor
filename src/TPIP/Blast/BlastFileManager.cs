using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    class BlastFileManager
    {

        static public void CreateHomologsPDBNamesList()
        {
            var file = Path.Combine(ConfigFileChecker.NCBIBlastDirectory, "output.txt");
            var BlastFileContent = File.ReadAllLines(file).Where(a => a.StartsWith("pdb|")).ToList();
            var fastaFileManagerInstance = new FastaFileManager();

            var counter = 0;
            foreach (var line in BlastFileContent)
            {
                //The number of homologs to generate
                if (counter > Constant.max_Number_of_Homologos)
                    break;
                var parts = line.Split(new[] { "|", " " }, StringSplitOptions.RemoveEmptyEntries);
                // checks if the homolog should be used in process
                if (!DoesExistInDeleteList(ProteinInterfacePredictor.currentProtein, parts[1]))
                {
                  
                    //add homologs only if they are involved in a complex
                    var IsHomologInComplex = fastaFileManagerInstance.CheckIfHomologIsaComplex(parts[1]);
                    if (IsHomologInComplex)
                    {
                        Console.WriteLine("* Adding homolog: " + parts[1]);
                        var homolog = new HomologProtein
                        {
                            PDBCode = parts[1],
                            Chain = parts[2],
                            Evalue = double.Parse(parts[parts.Length - 1])
                        };
                        MainPredictor.homologProteins.Add(homolog);
                        //homologsList.Add(homolog);
                        counter++;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if current homolog is in the user pdbCodes_tobe_removed_List or is the same as the QP it will not be used in process.
        /// </summary>
       static bool DoesExistInDeleteList(InputProtein currentInput, string pdbcode)
        {
            var inputPDBCode = currentInput.InputIdentifier.Substring(0, 4).ToUpper();
            if (currentInput.PdbCodes_tobe_removed_List.Contains(pdbcode.ToUpper()) || pdbcode.ToUpper() == inputPDBCode)
                return true;
            else
                return false;
        }


        static public void CreateHomologPartnerSeqForBlast()
        {
            var fastaSeqBuilder = new StringBuilder();
            var pDBFileManagerInstance = new PDBFileManager();
            foreach (var InteractingPartner in InterfaceGenerator.InteractingPartners)
            {
                var parts = InteractingPartner.Split('-');
                //var fileName = Directory.GetFiles(@"D:\Reyhaneh\Works\BioInformatics-Kingston\Research\Investments\Apps\TPIPPredictor\PdbFiles\", parts[0] + "*.pdb");
                var fileName = Directory.GetFiles(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, parts[0] + "*.pdb");
                try
                {
                    var pdbFileContent = File.ReadAllLines(fileName[0]).ToList();
                    fastaSeqBuilder.AppendLine(">" + InteractingPartner);
                    var aaSeq = pDBFileManagerInstance.GetProteinSequence(pdbFileContent, parts[1]);
                    fastaSeqBuilder.AppendLine(aaSeq);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("IOException source: {0}", ex.Message);
                }
            }
            File.WriteAllText(Path.Combine(ConfigFileChecker.NCBIBlastDirectory, "PartnersSeq.txt"), fastaSeqBuilder.ToString());
        }

    }
}
