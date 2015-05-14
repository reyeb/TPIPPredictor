using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    
        static class _Protein_Ext
        {
            public static HomologProtein FindByID(this List<HomologProtein> list, string id)
            {
                return list.FirstOrDefault(a => a.ID==id);
            }
        }

        class HomologProtein
        {
            public string PDBCode { get; set; }
            public string Chain { get; set; }
            public string ID { get { return PDBCode + "-" + Chain; } }
            public double Evalue { get; set; }
            public List<AminoAcid> AminoAcids { get; set; }
            public string AlignedSequence { get; set; }
            public double Weighti { get; set; }
            public int BeginResi { get; set; }
            public int EndResi { get; set; }
            
        }
        class AminoAcid
        {
            public string Position { get; set; }
            public string Weight { get; set; }
            public string Interactions { get; set; }
            public string BestInteractingPartnerChain { get; set; }
            public double Weightj { get; set; }
        }
    
}
