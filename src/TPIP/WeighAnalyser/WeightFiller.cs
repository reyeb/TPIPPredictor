using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
  
    class WeightFiller
    {
        //public static Dictionary<string, double> weightiDic = new Dictionary<string, double>();
       // public static Dictionary<string, double> weightjDic = new Dictionary<string, double>();
        public void FillWeightDics()
        {
            FillWeightiDic();
            FillWeightjDic();
        }

        /// <summary>
        /// this function reads the file related to Blast for each Quary sequenecs and using the 
        /// formula for wi will calculate it and place it in the weightiDic.[wi=-log(E-value(Ai))]
        /// </summary>
        /// 
        List<string> proteinNameList = new List<string>();
        void FillWeightiDic()
        {
           // weightiDic.Clear();
            var BlastEvaluecontent =MainPredictor.homologProteins.Select(a => a.ID).ToList();
            double wi = 0;
            foreach (var data in BlastEvaluecontent)
            {
                if (data.ToUpper() != ProteinInterfacePredictor.currentProtein.ID.ToUpper())
                {
                    var evalue = MainPredictor.homologProteins.Where(a => a.ID== data).Select(a => a.Evalue).ToList()[0];
                    if (evalue < Math.Pow(10, -200))
                        wi = Math.Pow(10, -200);
                    else if (evalue > 1)
                        wi = 1;
                    else
                        wi = evalue;

                   // weightiDic.Add(data, (1 - wi));
                    var weighti = 1 - wi;
                    MainPredictor.homologProteins.FindByID(data).Weighti = weighti;
                }
            }
        }


        /// <summary>
        /// this function reads the file related to Blast for pairwise seq alignemnt  and using the 
        /// formula for yi will calculate it and place it in the weightjDic.[yi=1/(1-log(Mean(Evalue(Cj))))]
        /// </summary>
        /// 
        List<Blast2SeqEvalue> Blast2SeqEvalueList = new List<Blast2SeqEvalue>();
        void FillWeightjDic()
        {
           // weightjDic.Clear();
            FillBlast2SeqEvalueList();
            double yj = 0;
            //foreach (var p in InterfaceGenerator.InteractingPartners)
            foreach (var homolog in MainPredictor.homologProteins)
            {
                foreach (var aa in homolog.AminoAcids)
                {
                    if (aa.Position != "-")
                    {

                        var p = homolog.PDBCode + "-" + aa.BestInteractingPartnerChain;
                        var orderedList = Blast2SeqEvalueList.Where(a => a.protein1 == p).Select(a => a).ToList();
                        if (InterfaceGenerator.InteractingPartners.Count() != 1)
                        {
                            var evaluesList = new List<double>();
                            var currentAdedddata = new List<string>();

                            foreach (var e in orderedList)
                            {
                                //deletes the double cases
                                if (!currentAdedddata.Contains(e.protein2))
                                {
                                    evaluesList.Add(e.eValue);
                                    currentAdedddata.Add(e.protein2);
                                }

                            }


                            var remainingProteins = InterfaceGenerator.InteractingPartners.Count() - 1 - evaluesList.Count();
                            // if any comparison has not been in the list it means there is no hit less than the evalue=1 when blasting and we should automatically give a value of 1 to taht comparison
                            if (remainingProteins != 0)
                            {
                                for (int i = 0; i < remainingProteins; i++)
                                    evaluesList.Add(1);
                            }

                            //calucate the mean of evalue and yi
                            double logValue;
                            double sumEvalues = 0;
                            foreach (var value in evaluesList)
                            {
                                if (value < Math.Pow(10, -200))
                                    logValue = Math.Pow(10, -200);
                                else if (value > 1)
                                    logValue = 1;
                                else
                                    logValue = value;

                                sumEvalues += logValue;
                            }
                            yj = sumEvalues / (InterfaceGenerator.InteractingPartners.Count() - 1);
                        }
                        // if the homolog is only one then give it a one
                        else
                        {
                            yj = 1;

                        }
                        //weightjDic.Add(p, yj);
                        aa.Weightj = yj;
                    }
                }
            }// end of main foreach
        }

        void FillBlast2SeqEvalueList()
        {
            var Blast2SeqEvaluecontent = File.ReadAllLines(Path.Combine(ConfigFileChecker.NCBIBlastDirectory, "SAOutput.txt"));
            double evalue = 0;

            foreach (var data in Blast2SeqEvaluecontent)
            {
                var parts = data.Split('\t');
                if (parts[0] != parts[1])//to remove the ones which are  agaianst itself
                {
                    evalue = double.Parse(parts[2]);
                    if (evalue > 1)
                        evalue = 1;
                    var blastData = new Blast2SeqEvalue
                    {
                        protein1 = parts[0],
                        protein2 = parts[1],
                        eValue = evalue
                    };
                    Blast2SeqEvalueList.Add(blastData);
                }
            }
        }
        class Blast2SeqEvalue
        {
            /// <summary>
            /// keeps the information regarding Blast2SeqEvalue(SAOutput.txt file)
            /// </summary>
            public string protein1 { get; set; }
            public string protein2 { get; set; }
            public double eValue { get; set; }

        }
    }
}
