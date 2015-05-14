using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{

    class InputFileManager
    {
        //public static List<InputDataInfo> InputDataList = new List<InputDataInfo>();
       
        /// <summary>
        /// Checks if inputs are in pdb Fomrmat or Fasta Format
        /// </summary>
        public static List<InputProtein> AnalysisInputData()
        {
            
            var listToDelete = new List<string>();
            if (File.Exists(Program.Input.PdbCodes_tobe_removedAddress))
            {
                listToDelete = Get_pdbCodes_tobe_removed_List(Program.Input.PdbCodes_tobe_removedAddress);
            }

            if (Program.Input.EntryType == InputFileEntryType.ProteinID)
            {
                var inputContents = File.ReadAllLines(Program.Input.InputFileAddress).ToList();
                var InputDataList=AnalysisPDBFormat(inputContents, listToDelete);
                return InputDataList;
            }
            else if (Program.Input.EntryType == InputFileEntryType.ProteinSequence)
            {
                var textContent = File.ReadAllText(Program.Input.InputFileAddress);
                var inputContents = textContent.Split(new[] { ">" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var InputDataList = AnalysisFastaFormat(inputContents, listToDelete);
                return InputDataList;
            }
            else
            {
                throw new ValidationException("The format selected for input data does not exist");
            }

           
        }

        static List<InputProtein> AnalysisPDBFormat(List<string> inputContents, List<string> pdbCodestobeRemovedList)
        {
            var InputDataList = new List<InputProtein>();
            var pDBFileManagerInstance = new PDBFileManager();
            foreach (var input_PdbCode_Chain in inputContents)
            {
                Console.WriteLine("* Reading " + input_PdbCode_Chain);
                var pdbId = input_PdbCode_Chain.Substring(0, 4);
                var chain = input_PdbCode_Chain.Substring(4, 1);
                var id = pdbId.ToLower() + "-" + chain.ToUpper();
                var PdbFilesLocation = CreateDirectories(id);
                //to comment for test and also when pdbFiles are available
                if (!File.Exists(Path.Combine(PdbFilesLocation, id.ToUpper() + ".pdb")))
                     WebFileManager.GetPDBFile(pdbId, chain);
                var pdbFileContent = File.ReadAllLines(Path.Combine(PdbFilesLocation, id.ToUpper() + ".pdb")).ToList();
                var fastaSeq = pDBFileManagerInstance.GetProteinSequence(pdbFileContent, chain.ToUpper());
                var inputdata = new InputProtein
                {
                    FastaInputData = ">" + id + Environment.NewLine + fastaSeq,
                    InputIdentifier = id,
                    PdbCodes_tobe_removed_List = pdbCodestobeRemovedList,
                    PdbFilesLocation = PdbFilesLocation,
                     ID=id.ToUpper()
                };
                InputDataList.Add(inputdata);
            }
            return InputDataList;
            
        }

        static List<InputProtein> AnalysisFastaFormat(List<string> inputContents, List<string> pdbCodestobeRemovedList)
        {
            var InputDataList = new List<InputProtein>();
           
            foreach (var fastaString in inputContents)
            {
               
                var endIndexOfIdentifier = fastaString.IndexOf(Environment.NewLine);
                var id="MyData_" + fastaString.Substring(0, endIndexOfIdentifier);
                var inputdata = new InputProtein
                {
                    FastaInputData = ">MyData_" + fastaString,
                    InputIdentifier = id,
                    PdbCodes_tobe_removed_List = pdbCodestobeRemovedList,
                    ID=id.ToUpper()
                };
                Console.WriteLine("* Reading " + inputdata.InputIdentifier);
                var pdbpath = CreateDirectories(inputdata.InputIdentifier);
                inputdata.PdbFilesLocation = pdbpath;
                InputDataList.Add(inputdata);
                
            }
            return InputDataList;
            
        }

        static List<string> Get_pdbCodes_tobe_removed_List(string address)
        {
            //********* toupper() all PDBCods before putting them in.
            var allCodes = File.ReadAllLines(address).ToList();
            return allCodes;
        }

        static string CreateDirectories(string id)
        {
            var MainOutPutDirectory = Path.Combine(Program.Input.OutPutDirectory, "PredictionResults");
            var dir = Directory.CreateDirectory(Path.Combine(MainOutPutDirectory, id)).FullName;
            var pdbpath = Directory.CreateDirectory(Path.Combine(dir, "pdbFiles")).FullName;
            return pdbpath;
        }



    }
}
