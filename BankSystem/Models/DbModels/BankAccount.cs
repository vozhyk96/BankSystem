using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.DbModels
{
    public class BankAccount
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public double money { get; set; }
        public bool isCredit { get; set; }
        public bool isNone { get; set; }
    }
}