using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    class MainPredictor
    {
        public static List<HomologProtein> homologProteins = new List<HomologProtein>();
        /// <summary>
        /// generates all the homologs related to the QP.
        /// </summary>
        public void GetHomologs()
        {
            File.WriteAllText(Path.Combine(ConfigFileChecker.NCBIBlastDirectory, "input.txt"), ProteinInterfacePredictor.currentProtein.FastaInputData);
            Console.WriteLine("* Blast to get homologs:");
            BlastHelper.Blast("-query input.txt -db pdbaa -evalue 0.01 -out output.txt");
           // var blastFileManagerInstance = new BlastFileManager();
            BlastFileManager.CreateHomologsPDBNamesList();
            if (homologProteins.Count() == 0)
            {
                throw new ValidationException(" T-PIP is not able to predict interfaces for this protein since no homologous complex exists for this protein.");
            }
            //to comment for test and also when pdbFiles are available
            
            GetHomologsPDBFiles();
        }

        void GetHomologsPDBFiles()
        {
            
            foreach (var homolog in homologProteins)
            {

                if (!File.Exists(Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, homolog.PDBCode.ToUpper() + "-" + homolog.Chain.ToUpper() + ".pdb")))
                {
                    WebFileManager.GetPDBFile(homolog.PDBCode, homolog.Chain);
                    Console.WriteLine("* Downloading PDB file of homolog: " + homolog.PDBCode);
                }
                else
                    Console.WriteLine("*  PDB file of homolog " + homolog.PDBCode+" already exists.");
            }
        }

        /// <summary>
        /// Aligns the QP and homolgs and generated an alignement seq for all of them
        /// </summary>
        public void GenerateMSAofHomologs()
        {
           
            var clustalWFileManagerInstance = new ClustalWFileManager();
            ClustalWFileManager.CreateClustalWInputFile();
            Console.WriteLine("*  ClustalW started sequence alignment ");
            ClustalWHelper.CreateMSAofHomologs();
            ClustalWFileManager.CreateAlignedDictionary();
            Console.WriteLine("*  ClustalW created the sequence alignment ");
        }

        /// <summary>
        /// generated the position and interface  resi in the aligment sequences based on the PDB files
        /// </summary>
        public void AnalysAminoAcidofHomologs()
        {
            PositionSeqCreator.CreatePositionDic();
            InteractionSeqCreator.CreateInteractionDic();
        }

        public void CreateWeightDics()
        {
            Console.WriteLine("*  Calculating Weights... ");
            AnalyseHomologPartners();
             WeightCalculator.GenerateWeightedScoreDic();
        }

        public string ClaculateScore()
        {
            Console.WriteLine("*  Calculating scores... ");
            var scoreCalculatorInstance = new ScoreCalculator();
            var resultString = scoreCalculatorInstance.CalculateScore();
            return resultString;
        }

        void AnalyseHomologPartners()
        {
            //var blastFileManagerInstance = new BlastFileManager();
            BlastFileManager.CreateHomologPartnerSeqForBlast();
            BlastHelper.Blast("-query PartnersSeq.txt -subject PartnersSeq.txt -outfmt \"6 qseqid sseqid evalue\" -out SAOutput.txt");
        }



      
    }
}
