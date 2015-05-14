using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TPIP
{
    class InterfaceResult
    {
        //public List<string> InteractingPartners { get; set; }
        public List<BestInteractingPartners> WightScoreList { get; set; }
        public List<string> InteractingAtomsList { get; set; }

    }

    static class InterfaceGenerator
    {
        public static List<string> InteractingPartners = new List<string>();
        //public static Dictionary<string, List<BestInteractingPartners>> wightScoreDic = new Dictionary<string, List<BestInteractingPartners>>();
        public static InterfaceResult GenerateInterfaceResidues(string PDBID, List<AACoordinate> mainChainAtoms, List<AACoordinate> otherChainsAtoms)
        {
           var result = new InterfaceResult();
            var interactingAtomsList = new List<string>();
            var mathHelperInstance = new MathHelper();
            var wightScorelist = new List<BestInteractingPartners>();
            AACoordinate lastAtom2 = new AACoordinate();

            var ResiGroupList = mainChainAtoms.GroupBy(a => a.ResiNumber).ToList();
            foreach (var resi in ResiGroupList)
            {
                var listData = new List<PartnerData>();
                foreach (var atom1 in resi)
                {
                    double minDistance = 100000;
                    var currentchain = "";
                    if (atom1.atomName != "H")
                    {
                        foreach (var atom2 in otherChainsAtoms)
                        {
                            var currentDis = mathHelperInstance.EuclideanDistance(atom1, atom2);
                            if (currentDis < minDistance)
                            {
                                minDistance = currentDis;
                                lastAtom2 = atom2;
                                currentchain = atom2.chainName;
                            }
                        }

                        var a = new PartnerData
                        {
                            ChainName = currentchain,
                            Atom1d = atom1,
                            Atom2d = lastAtom2,
                            Distance = minDistance
                        };

                        listData.Add(a);
                    }
                }//forech atom 1

                var bestresult = listData.OrderBy(a => a.Distance).ToList()[0];
                var bestdistance = bestresult.Distance;

                var pdbId = PDBID.Split('-')[0];
                if (bestdistance < Constant.betweenChainDistanceThershold)
                    interactingAtomsList.Add(bestresult.Atom1d.ResiNumber.ToString());

                //if (!result.InteractingPartners.Contains(pdbId + "-" + bestresult.Atom2d.chainName) && pdbId != Util.GetPDBCode(ProteinInterfacePredictor.currentProtein.ID).ToUpper())
                //    result.InteractingPartners.Add(pdbId + "-" + bestresult.Atom2d.chainName);
                if (!InteractingPartners.Contains(pdbId + "-" + bestresult.Atom2d.chainName) && pdbId != Util.GetPDBCode(ProteinInterfacePredictor.currentProtein.ID).ToUpper())
                    InteractingPartners.Add(pdbId + "-" + bestresult.Atom2d.chainName);
                var aa = new BestInteractingPartners
                {
                    chainName = bestresult.ChainName,
                    resiNumber = resi.Key.ToString()
                };
                wightScorelist.Add(aa);
            }//foreach resi

            //wightScoreDic.Add(PDBID, wightScorelist);
            result.InteractingAtomsList = interactingAtomsList.Distinct().ToList();
            result.WightScoreList = wightScorelist;
            return result;
            //return InteractingAtomsList;
        }//end of method

        class PartnerData
        {
            public AACoordinate Atom1d { get; set; }
            public AACoordinate Atom2d { get; set; }
            public string ChainName { get; set; }
            public double Distance { get; set; }
        }

    }
}
