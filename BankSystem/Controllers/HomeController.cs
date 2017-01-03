using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using BankSystem.Models.DbModels;

namespace BankSystem.Controllers
{
    public class HomeController : Controller
    {
        private string GetName(string s)
        {
            int position = s.IndexOf("<Name>");
            position += 6;
            string result = "";
            while (s[position] != '<')
            {
                result += s[position];
                position++;
            }
            return result;
        }
        private decimal GetValue(string s)
        {
            int position = s.IndexOf("<Rate>");
            position += 6;
            string result = "";
            while (s[position] != '<')
            {
                result += s[position];
                position++;
            }
            result = result.Replace('.', ',');
            return Convert.ToDecimal(result);
        }

        private string GetCharCode(string s)
        {
            int position = s.IndexOf("<CharCode>");
            position += 10;
            string result = "";
            while (s[position] != '<')
            {
                result += s[position];
                position++;
            }
            return result;
        }

        private int GetScale(string s)
        {
            int position = s.IndexOf("<Scale>");
            position += 7;
            string result = "";
            while (s[position] != '<')
            {
                result += s[position];
                position++;
            }
            return Convert.ToInt32(result);
        }
        public ActionResult Index()
        {
            //Инициализируем объекта типа XmlTextReader и
            //загружаем XML документ с сайта центрального банка
            XmlTextReader reader = new XmlTextReader("http://www.nbrb.by/Services/XmlExRates.aspx");
            //В эти переменные будем сохранять куски XML
            //с определенными валютами (Euro, USD)
            List<Valute> valutes = new List<Valute>();
            string XML = "";
            
            //Перебираем все узлы в загруженном документе
            while (reader.Read())
            {
                //Проверяем тип текущего узла
                switch (reader.NodeType)
                {
                    //Если этого элемент Valute, то начинаем анализировать атрибуты
                    case XmlNodeType.Element:

                        if (reader.Name == "Currency")
                        {
                            if (reader.HasAttributes)
                            {
                                //Метод передвигает указатель к следующему атрибуту
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "Id")
                                    {
                                        reader.MoveToElement();
                                        XML = reader.ReadOuterXml();
                                        Valute v = new Valute();
                                        v.name = GetName(XML);
                                        v.charCode = GetCharCode(XML);
                                        v.rate = GetValue(XML);
                                        v.scale = GetScale(XML);
                                        valutes.Add(v);
                                        
                                    }
                                   
                                }
                            }
                        }

                        break;
                }
            }

            List<string> belvalutes = new List<string>();
            foreach (var valute in valutes)
            {
                string s = String.Format("За {3} {0}({1}): {2} BYN\n", valute.name,valute.charCode,Math.Round(valute.rate,4),valute.scale);
                belvalutes.Add(s);
            }
            Valutes model = new Valutes();
            model.valutes = belvalutes;
            if(Session["sucsess"] == null)
                Session["sucsess"] = "";
            string a = Session["sucsess"].ToString();
            return View(model);
        }

        public ActionResult SendMail(string name, string email, string phone, string message)
        {
            if((name != null)&&(email != null)&&(phone != null)&&(message != null)&&(email != ""))
            {
                Mail mail = new Mail();
                mail.email = email;
                mail.name = name;
                mail.phone = phone;
                mail.message = message;
                mail.time = DateTime.Now;
                Repository.AddMail(mail);
                Session["sucsess"] = "true";
            }
            else Session["sucsess"] = "false";
            string a = Session["sucsess"].ToString();
            return RedirectToAction("Index","Home");
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}