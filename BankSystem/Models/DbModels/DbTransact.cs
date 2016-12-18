using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.DbModels
{
    public class DbTransact
    {
        public int id { get; set; }
        public int CardInId { get; set; }
        public int CardOutId { get; set; }
        public double money { get; set; }
        public DateTime date { get; set; }
    }
}