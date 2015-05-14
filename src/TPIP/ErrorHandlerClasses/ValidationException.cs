using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    class ValidationException : Exception
    {
        public ValidationException(string m)
            : base(m)
        {

        }
    }
}
