﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwack.Random.Sobol
{
    public class SobolDirectionInfo
    {
        public int Dimension { get; set; }
        public uint S { get; set; }
        public uint A { get; set; }
        public uint[] DirectionNumbers { get; set; }
    }
}
