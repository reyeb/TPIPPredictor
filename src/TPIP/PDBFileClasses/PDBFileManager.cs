using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TPIP
{
    class PDBFileManager
    {
        public List<AACoordinate> CreateMainAtomList(List<string> pdbFileContent, string mainChainID)
        {
            var mainChainAtoms = new List<AACoordinate>();
            var ChainContent = pdbFileContent.Where(a => a.StartsWith("ATOM") && a.Substring(21, 1) == mainChainID).ToList();
            mainChainAtoms = FillAtomList(ChainContent);
            return mainChainAtoms;
        }// end of function

        public List<AACoordinate> CreateOtherAtomList(List<string> pdbFileContent, string mainChainID)
        {
            var otherChainsAtoms = new List<AACoordinate>();
            var otherChainContent = pdbFileContent.Where(a => a.StartsWith("ATOM") && a.Substring(21, 1) != mainChainID).ToList();
            otherChainsAtoms = FillAtomList(otherChainContent);
            return otherChainsAtoms;
        }// end of function


        List<AACoordinate> FillAtomList(List<string> ChainContent)
        {
            var lstCoordinates = new List<AACoordinate>();
            AACoordinate data;
            foreach (var atom in ChainContent)
            {
                data = new AACoordinate
                {
                    ResiNumber = int.Parse(atom.Substring(22, 4).Trim()),
                    atomName = atom.Substring(12, 4).Trim(),
                    chainName = atom.Substring(21, 1).Trim(),
                    X = double.Parse(atom.Substring(30, 8).Trim()),
                    Y = double.Parse(atom.Substring(38, 8).Trim()),
                    Z = double.Parse(atom.Substring(46, 8).Trim())
                };
                lstCoordinates.Add(data);
            }

            return lstCoordinates;

        }

        public List<string> GetAAPositionNumber(List<string> pdbFileContent, string chainID)
        {
            // var content = File.ReadAllLines(".pdb");
            var residuesPositionNumberList = new List<string>();
            residuesPositionNumberList = pdbFileContent.Where(a => a.StartsWith("ATOM") && a.Substring(12, 4).Trim() == "CA" && a.Substring(21, 1).Trim() == chainID)
                    .Select(a => a.Substring(22, 4).Trim()).ToList();
            return residuesPositionNumberList;
        }

        public string GetProteinSequence(List<string> pdbFileContent, string chainID)
        {
            var content = pdbFileContent.Where(a => a.StartsWith("ATOM") && a.Substring(12, 4).Trim() == "CA" && a.Substring(21, 1).Trim() == chainID).Select(a => a.Substring(17, 3)).ToList();
            var aaSeq = "";
            var aalist = CreateOneLetterAASeq(content);

            foreach (var aa in aalist)
            {
                aaSeq = aaSeq + aa;
            }
            return aaSeq;
        }

        IEnumerable<string> CreateOneLetterAASeq(IEnumerable<string> seq)
        {
            seq = seq.Select(a => a.Replace("ARG", "R"));
            seq = seq.Select(a => a.Replace("GLU", "E"));
            seq = seq.Select(a => a.Replace("ALA", "A"));
            seq = seq.Select(a => a.Replace("ASN", "N"));
            seq = seq.Select(a => a.Replace("ASP", "D"));
            seq = seq.Select(a => a.Replace("CYS", "C"));
            seq = seq.Select(a => a.Replace("GLN", "Q"));
            seq = seq.Select(a => a.Replace("GLY", "G"));
            seq = seq.Select(a => a.Replace("HIS", "H"));
            seq = seq.Select(a => a.Replace("ILE", "I"));
            seq = seq.Select(a => a.Replace("LEU", "L"));
            seq = seq.Select(a => a.Replace("LYS", "K"));
            seq = seq.Select(a => a.Replace("MET", "M"));
            seq = seq.Select(a => a.Replace("PHE", "F"));
            seq = seq.Select(a => a.Replace("PRO", "P"));
            seq = seq.Select(a => a.Replace("SER", "S"));
            seq = seq.Select(a => a.Replace("THR", "T"));
            seq = seq.Select(a => a.Replace("TRP", "W"));
            seq = seq.Select(a => a.Replace("TYR", "Y"));
            seq = seq.Select(a => a.Replace("VAL", "V"));

            return seq;
        }
    }
}
