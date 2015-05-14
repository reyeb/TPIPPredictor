using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    class PositionSeqCreator
    {
        /// <summary>
        /// Fills in the Position number of residues based on their aligned seq and pdbFiles
        /// </summary>
        public static void CreatePositionDic()
        {
            //if the entry is based on the ProteinID the PositionSeq is required
            if (Program.Input.EntryType == InputFileEntryType.ProteinID)
            {
               
                    var aaList = GenerateAAPositionSequence(ProteinInterfacePredictor.currentProtein.ID, ProteinInterfacePredictor.currentProtein.AlignedSequence);
                    ProteinInterfacePredictor.currentProtein.PositionSequence = referenceStringCreator(aaList);
            }
            //get PositionSeq for all homologs
            foreach (var homolog in MainPredictor.homologProteins)
            {
                    var AAPositionSequence = GenerateAAPositionSequence(homolog.ID,homolog.AlignedSequence);
                    homolog.AminoAcids = AAPositionSequence;
            }
        }

        static List<AminoAcid> GenerateAAPositionSequence(string homologID, string homologAlignedSequence)
        {
            var pDBFileManagerInstance = new PDBFileManager();
            var pdbFileContent = File.ReadAllLines(Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, homologID + ".pdb")).ToList();
            var listOfAAPosition = pDBFileManagerInstance.GetAAPositionNumber(pdbFileContent, Util.GetChain(homologID));
            var AAPositionSequence = CreateAAPositionSequence(homologAlignedSequence, listOfAAPosition);
            return AAPositionSequence;
        }
        static List<string> referenceStringCreator(List<AminoAcid> aaList)
        {
            var str=new List<string>();
            foreach (var aa in aaList)
            {
                str.Add( aa.Position);
            }
            return str;
        }

        static List<AminoAcid> CreateAAPositionSequence(string homologAlignedSeq, List<string> listOfAAPosition)
        {
            var counter = 0;
            var AAPositionSequence = new List<AminoAcid>();
            
            foreach (var resi in homologAlignedSeq)
            {
                if (resi != '-')
                {
                     var aa = new AminoAcid()
                    {
                         Position = listOfAAPosition[counter],
                    };
                    AAPositionSequence.Add(aa);
                    counter++;
                }
                else if (resi == '-')
                {
                    var aa = new AminoAcid()
                    {
                        Position = "-",
                    };
                    AAPositionSequence.Add(aa);
                }
                else
                    throw new Exception("something went wrong in GenerateAAPositionSequence!");
            }

            return AAPositionSequence;
        }
    }
}
