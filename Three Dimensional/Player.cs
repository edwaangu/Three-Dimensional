using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Three_Dimensional
{
    internal class Player
    {
        public double x, y, z;
        public PointF direction;
        public Player(double _x, double _y, double _z, PointF _direction)
        {
            x = _x;
            y = _y;
            z = _z;
            direction = _direction;
        }
    }
}
