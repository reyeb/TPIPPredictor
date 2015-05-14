using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    class Util
    {
        public static string GetChain(string pdbid_Chain)
        {
            var parts = pdbid_Chain.Split('-');
            return parts[1];
        }
        public static string GetPDBCode(string pdbid_Chain)
        {
            var parts = pdbid_Chain.Split('-');
            return parts[0];
        }
    }
}
