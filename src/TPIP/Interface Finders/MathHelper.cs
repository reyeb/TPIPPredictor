using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPIP
{
    class MathHelper
    {
        public double EuclideanDistance(AACoordinate point1, AACoordinate point2)
        {
            var xp = Math.Pow(point1.X - point2.X, 2);
            var yp = Math.Pow(point1.Y - point2.Y, 2);
            var zp = Math.Pow(point1.Z - point2.Z, 2);
            var dis = Math.Sqrt(xp + yp + zp);
            return dis;
        }

    }
}
