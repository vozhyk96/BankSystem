using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.DbModels
{
    public class Card
    {
        public int id { get; set; }
        public int AccountId { get; set; }
        public string UserId { get; set; }
    }
}