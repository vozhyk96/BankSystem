﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem
{
    public struct Valute
    {
        public string name { get; set; }
        public string charCode { get; set; }
        public decimal rate { get; set; }

        public int scale { get; set; }
    }
}