using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankSystem.Models.ViewModels
{
    public class ViewUser
    {
        public string id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public string phone { get; set; }
        public string adress { get; set; }
        public bool HasPassword { get; set; }
        public bool isUser { get; set; }
        public bool isAdmin { get; set; }
        public Picture picture { get; set; }

    }
}