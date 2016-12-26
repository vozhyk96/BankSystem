using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace BankSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Инициализируем объекта типа XmlTextReader и
            //загружаем XML документ с сайта центрального банка
            XmlTextReader reader = new XmlTextReader("http://www.cbr.ru/scripts/XML_daily.asp");
            //В эти переменные будем сохранять куски XML
            //с определенными валютами (Euro, USD)
            
            string BelXML = "";
            string USDXMl = "";
            string EuroXML = "";
            string AustDXML = "";
            string BrFXML = "";
            string BlgLXML = "";
            string DaKXML = "";
            //Перебираем все узлы в загруженном документе
            while (reader.Read())
            {
                //Проверяем тип текущего узла
                switch (reader.NodeType)
                {
                    //Если этого элемент Valute, то начинаем анализировать атрибуты
                    case XmlNodeType.Element:

                        if (reader.Name == "Valute")
                        {
                            if (reader.HasAttributes)
                            {
                                //Метод передвигает указатель к следующему атрибуту
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01090B")
                                        {
                                            //Возвращаемся к элементу, содержащий текущий узел атрибута
                                            reader.MoveToElement();
                                            //Считываем содержимое дочерних узлом
                                            BelXML = reader.ReadOuterXml();
                                        }
                                    }

                                    if (reader.Name == "ID")
                                    {
                                        //Если значение атрибута равно R01235, то перед нами информация о курсе доллара
                                        if (reader.Value == "R01235")
                                        {
                                            //Возвращаемся к элементу, содержащий текущий узел атрибута
                                            reader.MoveToElement();
                                            //Считываем содержимое дочерних узлом
                                            USDXMl = reader.ReadOuterXml();
                                        }
                                    }

                                    //Аналогичную процедуру делаем для ЕВРО
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01239")
                                        {
                                            reader.MoveToElement();
                                            EuroXML = reader.ReadOuterXml();
                                        }
                                    }

                                    //Австралийский доллар
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01010")
                                        {
                                            reader.MoveToElement();
                                            AustDXML = reader.ReadOuterXml();
                                        }
                                    }

                                    //Британский фунт
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01035")
                                        {
                                            reader.MoveToElement();
                                            BrFXML = reader.ReadOuterXml();
                                        }
                                    }

                                    //Болгарский лев
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01100")
                                        {
                                            reader.MoveToElement();
                                            BlgLXML = reader.ReadOuterXml();
                                        }
                                    }

                                    //Датская крона
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01215")
                                        {
                                            reader.MoveToElement();
                                            DaKXML = reader.ReadOuterXml();
                                        }
                                    }

                                    
                                }
                            }
                        }

                        break;
                }
            }

            //Из выдернутых кусков XML кода создаем новые XML документы
            XmlDocument belXmlDocument = new XmlDocument();
            belXmlDocument.LoadXml(BelXML);
            XmlDocument usdXmlDocument = new XmlDocument();
            usdXmlDocument.LoadXml(USDXMl);
            XmlDocument euroXmlDocument = new XmlDocument();
            euroXmlDocument.LoadXml(EuroXML);
            XmlDocument AustDXmlDocument = new XmlDocument();
            AustDXmlDocument.LoadXml(AustDXML);
            XmlDocument BrFXmlDocument = new XmlDocument();
            BrFXmlDocument.LoadXml(BrFXML);
            XmlDocument BlgLXmlDocument = new XmlDocument();
            BlgLXmlDocument.LoadXml(BlgLXML);
            XmlDocument DaKXmlDocument = new XmlDocument();
            DaKXmlDocument.LoadXml(DaKXML);
            
            List<Valute> valutes = new List<Valute>();
            Valute val = new Valute();
            //Метод возвращает узел, соответствующий выражению XPath
            XmlNode xmlNode = belXmlDocument.SelectSingleNode("Valute/Value");
            decimal belValue = Convert.ToDecimal(xmlNode.InnerText);
            xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");
            //Считываем значение и конвертируем в decimal. Курс валют получен
            val.name = "Американский доллар";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
            val.name = "Евро";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            xmlNode = AustDXmlDocument.SelectSingleNode("Valute/Value");
            val.name = "Австралийский доллар";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            xmlNode = BrFXmlDocument.SelectSingleNode("Valute/Value");
            val.name = "Британский фунт";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            xmlNode = BlgLXmlDocument.SelectSingleNode("Valute/Value");
            val.name = "Болгарский лев";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            xmlNode = DaKXmlDocument.SelectSingleNode("Valute/Value");
            val.name = "Датская крона";
            val.rate = Convert.ToDecimal(xmlNode.InnerText) / belValue;
            valutes.Add(val);

            return View();
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