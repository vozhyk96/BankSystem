using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.ViewModels
{
    public class ChangeMoney
    {
        public int CardId { get; set; }
        public double money { get; set; }
        public bool isAdd { get; set; }
    }
}