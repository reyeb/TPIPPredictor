using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    class ProteinInterfacePredictor
    {
        public static InputProtein currentProtein = new InputProtein();
       
        //public static Dictionary<string, List<BestInteractingPartners>> wightScoreDic = new Dictionary<string, List<BestInteractingPartners>>();
        public void PredictInterface()
        {
            var MainOutPutDirectory=InitializeTPIP();
            //analysis input
            Console.WriteLine("* Reading input proteins:");
            var inputDataList=InputFileManager.AnalysisInputData();

            foreach (var inputdata in inputDataList)
            {
                Console.WriteLine("* Start Prediction for: " + inputdata.InputIdentifier);
                currentProtein = inputdata;
                var current_resultLocation = Path.Combine(MainOutPutDirectory, inputdata.InputIdentifier);

                //        wightScoreDic.Clear();
                var mainPredictorInstance = new MainPredictor();
                // currentProtein.ID = inputdata.InputIdentifier.ToUpper();
                mainPredictorInstance.GetHomologs();
                mainPredictorInstance.GenerateMSAofHomologs();
                mainPredictorInstance.AnalysAminoAcidofHomologs();
                mainPredictorInstance.CreateWeightDics();
                var resultString = mainPredictorInstance.ClaculateScore();
                FinilizeTPIP(inputdata, resultString, current_resultLocation);
            }

            Console.WriteLine("*  All Proteins processed. Job Finishes!");
        }


        static string InitializeTPIP()
        {
            //check/set configuration data first
            Console.WriteLine("* Validating configuration file");
            ConfigFileChecker.SetConfiguratedValues();

            ////********These should happen once at beginging since when we have foreach it should not happen again
            FastaFileManager.Fillfasta_Sequence_List();
            ////***********

            ////CreateDirectories

            var MainOutPutDirectory = Path.Combine(Program.Input.OutPutDirectory, "PredictionResults");
            Console.WriteLine("* Creating output Directory:" + MainOutPutDirectory);
            if (!Directory.Exists(MainOutPutDirectory))
                Directory.CreateDirectory(MainOutPutDirectory);
            return MainOutPutDirectory;
        }

        static void FinilizeTPIP(InputProtein inputdata, string resultString, string current_resultLocation)
        {
            var path=Path.Combine(current_resultLocation, inputdata.InputIdentifier + "-TPIPPrediction.txt");
            File.WriteAllText(path, inputdata.FastaInputData + Environment.NewLine + resultString);
            Console.WriteLine("*  Results are ready in "+path);
            // Directory.Delete(current_pdbFilesLocation);
        }

    }
}
