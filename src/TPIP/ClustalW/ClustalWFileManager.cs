using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{

    class ClustalWFileManager
    {
        //public Dictionary<string, string> dicAlignedSequences = new Dictionary<string, string>();
        /// <summary>
        /// Creates an input file for clustalw to perform the alignment.
        /// </summary>
        /// <param name="homologProteinsList"></param>
        static public void CreateClustalWInputFile()
        {
            var allSequncesBuilder = new StringBuilder();
            var pDBFileManagerInstance = new PDBFileManager();
            //this line  adds the QP itself to the sequences to be aligned
            allSequncesBuilder.AppendLine(ProteinInterfacePredictor.currentProtein.FastaInputData);
            
            foreach (var homolog in MainPredictor.homologProteins)
            {
                //var id = homolog.PDBCode.ToLower() + "_" + homolog.Chain.ToUpper();
                var id = homolog.PDBCode.ToLower() + "-" + homolog.Chain.ToUpper();
                //to remove when we know what to do with pdbFiles
                var pdbFileContent = File.ReadAllLines(Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, id + ".pdb")).ToList();
                var fastaSeq = pDBFileManagerInstance.GetProteinSequence(pdbFileContent, homolog.Chain.ToUpper());
                // var fastaSeq = FastaFileManager.fasta_Sequence_List.Where(a => a.StartsWith(id)).Select(a => a).ToList()[0];
                //allSequncesBuilder.AppendLine(">"+fastaSeq);
                allSequncesBuilder.AppendLine(">" + id + Environment.NewLine + fastaSeq);
            }
            var outputPath = Path.Combine(ConfigFileChecker.ClustalWDirectory, "seq.txt");
            Console.WriteLine("*  ClustalW input file for MSA is created in " + outputPath);
            File.WriteAllText(outputPath, allSequncesBuilder.ToString());
        }
        /// <summary>
        /// reads the MSA from ClustalW and insert them in the MainPredictor.homologProteins based on matching to the id is the pdbidChain and the value is the homolog protein Sequence.
        /// </summary>
       static public void CreateAlignedDictionary()
        {
            var MSAContent = File.ReadAllText(Path.Combine(ConfigFileChecker.ClustalWDirectory, "seq.fasta"));
            var sections = MSAContent.Split(new[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var element in sections)
            {
                var endIndexOfIdentifier = element.IndexOf(Environment.NewLine);
                var elementId = "";
                //Since ClustalW ignores spaces for id in mode=1, w set this if else.
                if (element.StartsWith("MyData")) elementId = ProteinInterfacePredictor.currentProtein.ID.ToUpper();
                else elementId = element.Substring(0, endIndexOfIdentifier).Replace("_", "-").ToUpper();
                var elementSeq = element.Substring(endIndexOfIdentifier, element.Length - endIndexOfIdentifier).Replace(Environment.NewLine, "");
                //The current aligned seq for current Input is saved in the ProteinInterfacePredictor.currentInput
                if (elementId == ProteinInterfacePredictor.currentProtein.ID)
                {
                    ProteinInterfacePredictor.currentProtein.AlignedSequence = elementSeq;
                }
                // the information for homologs are saved in the MainPredictor.homologProteins
                else if (elementId != ProteinInterfacePredictor.currentProtein.ID)
                {
                    MainPredictor.homologProteins.FindByID(elementId).AlignedSequence = elementSeq;
                }
                else
                {
                    throw new Exception("Error occured in CreateAlignedDictionary");
                }
            }
        }
    }
}
