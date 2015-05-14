using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    class InputProtein
    {
        /// <summary>
        /// this should contain the user id so we can output the same as he/she wants. 
        /// if input is pdbcode this will be used. if FASTA string is used it will be anything after >. 
        /// </summary>
        public string InputIdentifier { get; set; }
        /// <summary>
        /// Contains the the following: >+InputIdentifier+Seq
        /// </summary>
        public string FastaInputData { get; set; }
        /// <summary>
        /// if the user inputs a set of pdbcodes to be deleted is saved here and checked once blasting.
        /// </summary>
        public List<string> PdbCodes_tobe_removed_List { get; set; }
        
        public string ID { get; set; }
        
        public string PdbFilesLocation { get; set; }

        public string AlignedSequence { get; set; }

        public List<string> PositionSequence { get; set; }
    }
}
