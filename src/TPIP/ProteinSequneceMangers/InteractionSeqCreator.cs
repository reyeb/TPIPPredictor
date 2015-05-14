using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    static class InteractionSeqCreator
    {
        //should be changed
        //public static List<string> homologsWithNoInteractionLessThanThreshold = new List<string>();
        public static void CreateInteractionDic()
        {
            var dicAAInteraction = new Dictionary<string, List<string>>();
            var pDBFileManagerInstance = new PDBFileManager();
           // var interfaceGeneratorInstance = new InterfaceGenerator();
            //homologsWithNoInteractionLessThanThreshold.Clear();
            var homologsWithNoInteractionLessThanThreshold = new List<HomologProtein>();
            InterfaceGenerator.InteractingPartners.Clear();

            foreach (var homolog in MainPredictor.homologProteins)
            {
                Console.WriteLine("*  Analysing interfaces between the chains of " + homolog.ID);
                var pdbFileContent = File.ReadAllLines(Path.Combine(ProteinInterfacePredictor.currentProtein.PdbFilesLocation, homolog.ID + ".pdb")).ToList();
                var mainChainAtoms = pDBFileManagerInstance.CreateMainAtomList(pdbFileContent, homolog.Chain);
                var otherChainsAtoms = pDBFileManagerInstance.CreateOtherAtomList(pdbFileContent, homolog.Chain);
                var interfaceResult = InterfaceGenerator.GenerateInterfaceResidues(homolog.ID, mainChainAtoms, otherChainsAtoms);
                if (interfaceResult.InteractingAtomsList.Count() != 0)
                {
                    ExtractInteractionSeq(interfaceResult.InteractingAtomsList, homolog);
                    ExtractBestPartnerChain(interfaceResult.WightScoreList,homolog);
                }
                else
                {
                    homologsWithNoInteractionLessThanThreshold.Add(homolog);
                }
            }
            //if a homolog is a complex by there are no interfaces in the thereshold they are removed from process
           if (homologsWithNoInteractionLessThanThreshold.Count() != 0)
               EditDics(homologsWithNoInteractionLessThanThreshold);
        }

        public static void ExtractInteractionSeq(List<string> InteractingAtomsList, HomologProtein homolog)
        {
            foreach (var aa in homolog.AminoAcids)
            {
                if (InteractingAtomsList.Contains(aa.Position))
                    aa.Interactions="I";
                else if (aa.Position == "-")
                     aa.Interactions="-";
                else
                    aa.Interactions = "$";
            }
        }

        public static void ExtractBestPartnerChain(List<BestInteractingPartners> WightScoreList, HomologProtein homolog)
        {
            foreach (var aa in homolog.AminoAcids)
            {
                if (aa.Position != "-")
                {
                    var bestChain = WightScoreList.Where(a => a.resiNumber == aa.Position).Select(a => a.chainName).ToList()[0];
                    aa.BestInteractingPartnerChain = bestChain;
                }
                else
                {
                    aa.BestInteractingPartnerChain = "-";
                }
              
            }
        }


        public static void EditDics(List<HomologProtein> homologsWithNoInteractionLessThanThreshold)
        {
            foreach (var homolog in homologsWithNoInteractionLessThanThreshold)
            {
                Console.WriteLine("*  " + homolog.ID + " is removed from the process since there are no interfaces in " + Constant.betweenChainDistanceThershold.ToString() + " Angstrom");
                MainPredictor.homologProteins.Remove(homolog);
                var toRemovedata = InterfaceGenerator.InteractingPartners.Where(a => a.StartsWith(homolog.PDBCode)).Select(a => a).ToList();
                foreach (var d in toRemovedata)
                {
                    InterfaceGenerator.InteractingPartners.Remove(d);
                }
            }
        }
    }
}
