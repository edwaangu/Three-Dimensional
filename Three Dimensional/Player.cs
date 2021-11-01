using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Three_Dimensional
{
    internal class Player
    {
        public double x, y, z, direction;
        public Player(double _x, double _y, double _z, double _direction)
        {
            x = _x;
            y = _y;
            z = _z;
            direction = _direction;
        }
    }
}
