using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    class ScoreCalculator
    {
        static int refBegin;
        static int refEnd;
        //static Dictionary<string, List<int>> ResiBeginEndDic = new Dictionary<string, List<int>>();
        static double fragmentTotalSumWeight = 0;
        static double fragmentInteractionSumWeight = 0;

        public string CalculateScore()
        {
           // ResiBeginEndDic.Clear();
            var inputProteinPositionSequence = new List<string>();
            if (Program.Input.EntryType == InputFileEntryType.ProteinID)
            {
                inputProteinPositionSequence = ProteinInterfacePredictor.currentProtein.PositionSequence;
                GetRefBeginEndResi(inputProteinPositionSequence);
            }
            else if (Program.Input.EntryType == InputFileEntryType.ProteinSequence)
            {
                refBegin = 0;
                refEnd = MainPredictor.homologProteins.First().AlignedSequence.Count();
            }
            else
            {
                throw new Exception ("Error in ScoreCalculator ");
            }
            CreateBeginEndDic();

            var ColomnScoreList = new List<ColomnScore>();
            var MedPointScoreList = new List<ColomnScore>();
            var size = ProteinInterfacePredictor.currentProtein.AlignedSequence.Length;
            for (int col = 0; col < size; col++)
            {

                if (refBegin <= col && col <= refEnd)
                {
                    fragmentTotalSumWeight = 0;
                    fragmentInteractionSumWeight = 0;
                    CalculateColomnSequenceScore(col);
                    double SequenceScore = 0;
                    if (fragmentInteractionSumWeight != 0)
                        SequenceScore = (fragmentInteractionSumWeight / fragmentTotalSumWeight);
                    var data = new ColomnScore()
                    {
                        ColomnNumber = col,
                        SEqScore = SequenceScore,
                    };
                    if (ProteinInterfacePredictor.currentProtein.AlignedSequence[col] != '-')
                        ColomnScoreList.Add(data);
                    MedPointScoreList.Add(data);
                }
            }
            var PredictedColomnList = GetPredictedInteractingColomnList(ColomnScoreList, MedPointScoreList);
            var resultString = GetResultsAsString(inputProteinPositionSequence, PredictedColomnList, ColomnScoreList);
            return resultString;
        }

        string GetResultsAsString(List<string> referenceProteinPositionSequence, List<int> PredictedColomnList, List<ColomnScore> ColomnScoreList)
        {
            var resultBuilder = new StringBuilder();
            var referenceProteinAlignedSequnce=ProteinInterfacePredictor.currentProtein.AlignedSequence;
            var precitedIntercationSeq = GeneratePredictedInteractionSequence(referenceProteinAlignedSequnce, PredictedColomnList);
            resultBuilder.AppendLine(precitedIntercationSeq);
            if (Program.Input.EntryType == InputFileEntryType.ProteinID)
            {
                var interfaceResiduesNumber = GenerateInterfaceResiNumber(referenceProteinPositionSequence, PredictedColomnList);
                resultBuilder.AppendLine(interfaceResiduesNumber);
            }
            var scoreSeq = GetColomnScoreForallQPResidues(referenceProteinAlignedSequnce, ColomnScoreList);
            resultBuilder.AppendLine(scoreSeq);
            return resultBuilder.ToString();
        }
        string GeneratePredictedInteractionSequence(string referenceProteinAlignedSequnce, List<int> resultList)
        {
            var proteinseq = "";
            for (int i = 0; i < referenceProteinAlignedSequnce.Length; i++)
            {
                if (resultList.Contains(i))
                {
                    proteinseq = proteinseq + "I";
                }
                else if (referenceProteinAlignedSequnce[i] != '-')
                {
                    proteinseq = proteinseq + ".";
                }
            }
            return proteinseq;
        }

        string GenerateInterfaceResiNumber(List<string> referenceProteinPositionSequence, List<int> resultList)
        {
            var interfaceResiduesNumber = "";
            foreach (var intResi in resultList.OrderBy(a => a))
                interfaceResiduesNumber = interfaceResiduesNumber + "," + referenceProteinPositionSequence[intResi];
            return interfaceResiduesNumber;
        }

        string GetColomnScoreForallQPResidues(string referenceProteinAlignedSequnce, List<ColomnScore> ColomnScoreList)
        {
            var scoreSeq = "";
            foreach (var data in ColomnScoreList)
            {
                if (referenceProteinAlignedSequnce[data.ColomnNumber] != '-')
                    scoreSeq += data.FinalScore + ",";
            }
            return scoreSeq;
        }

        List<int> GetPredictedInteractingColomnList(List<ColomnScore> ColomnScoreList, List<ColomnScore> MedPointScoreList)
        {
            var maxSeqScore = ColomnScoreList.OrderByDescending(a => a.SEqScore).ToList()[0].SEqScore;
            var normalizedList = ColomnScoreList.Select(a =>
            {
                a.FinalScore = (a.SEqScore);
                return a;
            }).ToList();

            var sortedList = normalizedList.OrderByDescending(a => a.FinalScore).ToList();

            var mean = GetMeanPoint(MedPointScoreList);

            var sotedColomnList = sortedList.Where(a => a.FinalScore != 0).Select(a => a.ColomnNumber).ToList();
            var resultList = sotedColomnList.Take(mean).ToList();
            return resultList;
        }

        int GetMeanPoint(List<ColomnScore> MedPointScoreList)
        {
            var SortedMedPointScoreList = MedPointScoreList.OrderByDescending(a => a.FinalScore).ToList();
            var sortedscore = SortedMedPointScoreList.Select(a => a.FinalScore).ToList();
            var meanSum = Math.Round(sortedscore.Sum());
            var mean = Convert.ToInt32(meanSum);
            return mean;
        }

        static public void CalculateColomnSequenceScore(int col)
        {
            foreach (var homolog in MainPredictor.homologProteins)
            {
                if (col >= homolog.BeginResi && col <= homolog.EndResi)
                {
                    if (homolog.AminoAcids[col].Interactions != "-")
                    {
                        fragmentTotalSumWeight += double.Parse(homolog.AminoAcids[col].Weight);
                        if (homolog.AminoAcids[col].Interactions == "I")
                        {
                            fragmentInteractionSumWeight += double.Parse(homolog.AminoAcids[col].Weight);
                        }
                    }
                }
            }

        }

        void CreateBeginEndDic()
        {

            foreach (var p in MainPredictor.homologProteins)
            {
                int begin = 0;
                int end = 0;
                for (int i = 0; i < p.AlignedSequence.Count(); i++)
                    if (p.AlignedSequence[i] != '-')
                    {
                        begin = i;
                        break;
                    }
                for (int i = p.AlignedSequence.Count() - 1; i > 0; i--)
                    if (p.AlignedSequence[i] != '-')
                    {
                        end = i;
                        break;
                    }
                p.EndResi = end;
                p.BeginResi = begin;
            }


        }


        void GetRefBeginEndResi(List<string> referenceProteinPositionSequence)
        {
            
            for (int i = 0; i < referenceProteinPositionSequence.Count(); i++)
            {
                if (referenceProteinPositionSequence[i] != "-")
                {
                    refBegin = i;
                    break;
                }
            }
            for (int i = referenceProteinPositionSequence.Count() - 1; i > 0; i--)
            {
                if (referenceProteinPositionSequence[i] != "-")
                {
                    refEnd = i;
                    break;
                }
            }
        }


    }
}
