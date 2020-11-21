﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Warriors : BaseUnit
    {
        public Warriors() : base(40, 5, 4, 2, 1, 1)
        {
            Type = UnitType.Warriors;
            GAS = UnitGAS.Air;
            Name = "Warriors";
        }
    }
}