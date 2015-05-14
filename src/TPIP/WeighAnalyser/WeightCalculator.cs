using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    static class WeightCalculator
    {

        //static List<ResiduesWeightScore> wightScorelist = new List<ResiduesWeightScore>();
        public static void GenerateWeightedScoreDic()
        {
            //var dicAAWeight = new Dictionary<string, List<string>>();
            // weights are filled here first
            var WeightFillerInstance = new WeightFiller();
            WeightFillerInstance.FillWeightDics();

            foreach (var homolog in MainPredictor.homologProteins)
            {
                //if (homolog.ID.ToUpper() != ProteinInterfacePredictor.currentProtein.ID.ToUpper())
                //{

                //wightScorelist.Clear();
                foreach (var aa in homolog.AminoAcids)
                {
                    if (aa.Position != "-")
                    {
                        var finalWeight = CalculateWeightScore(homolog, aa);
                        aa.Weight = finalWeight.ToString();
                    }
                    else
                        aa.Weight = "-";
                }
                //var Wstr = CreateWeightSequence(homolog.Key, new Dictionary<string, List<string>>());
               // dicAAWeight.Add(homolog.Key, Wstr);
                //}
            }
            //return dicAAWeight;
        }//end of method

        static double CalculateWeightScore(HomologProtein homolog, AminoAcid aa)
        {
            //var pdbId = PDBID.Split('-')[0];
            var w1 = homolog.Weighti;
            var w2 = aa.Weightj;
            var W = w1 * w2;
            return W;
        }

        //List<string> CreateWeightSequence(string proteinKey, Dictionary<string, List<string>> dicAAPosition)
        //{
        //    List<string> aaWeighScore = new List<string>();
        //    var proteinSeq = dicAAPosition[proteinKey];

        //    var PDBId = proteinKey.Split('-')[0];
        //    var chainId = proteinKey.Split('-')[1];

        //    foreach (var aa in proteinSeq)
        //    {
        //        if (aa == "-")
        //        {
        //            aaWeighScore.Add("-");
        //        }
        //        else
        //        {
        //            var resiNumber = aa;
        //            var weight = wightScorelist.Where(a => a.ResiduesNumber == int.Parse(resiNumber)).Select(a => a.Weight);
        //            aaWeighScore.Add(weight.ToList()[0].ToString());
        //        }

        //    }

        //    return aaWeighScore;
        //}
    }
}
