using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.DbModels
{
    public class Mail
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}