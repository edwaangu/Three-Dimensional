using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Three_Dimensional
{
    internal class Plane3
    {
        public Point3[] plist = new Point3[4];
        public Plane3(Point3[] _plist)
        {
            plist = _plist;
        }
    }
}
