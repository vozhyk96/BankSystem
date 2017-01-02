using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BankSystem.Models;
using BankSystem.Models.ViewModels;
using BankSystem.Models.DbModels;
using Microsoft.AspNet.Identity;

namespace BankSystem
{
    public class Repository
    {

        static public ApplicationUser FindUserById(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Find(id);
                return user;
          
            }
        }

        static public ApplicationUser GetUser(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser appuser = new ApplicationUser();
                appuser = db.Users.Find(id);
                return (appuser);
            }
        }

        static public void AddPicture(string id, byte[] imageData)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser appuser = db.Users.Find(id);
                appuser.Image = imageData;
                db.SaveChanges();
            }
        }

        static public void DeleteImage(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Find(id);
                user.Image = null;
                db.SaveChanges();
            }
        }

        static public void ChangeUser(ViewUser user)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser appuser = db.Users.Find(user.id);
                if (user.surname != null)
                {
                    appuser.surname = user.surname;
                }
                else appuser.surname = "";
                if (user.name != null)
                {
                    appuser.name = user.name;
                }
                else appuser.name = "";
                if (user.patronymic != null)
                {
                    appuser.patronymic = user.patronymic;
                }
                else appuser.patronymic = "";
                if (user.phone != null)
                {
                    appuser.phone = user.phone;
                }
                else appuser.phone = "";
                if (user.adress != null)
                {
                    appuser.adress = user.adress;
                }
                else appuser.adress = "";
                if (appuser.Email == "vozhyk96@fksis.by")
                    appuser.isAdmin = true;
                db.SaveChanges();
            }

        }

        static public void ChangeUserAdmin(string userId)
        {
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Find(userId);
                if (user.isAdmin)
                    user.isAdmin = false;
                else user.isAdmin = true;
                db.SaveChanges();
            }
        }

        static public void CreateCard(Card card)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (card.AccountId == 0)
                {
                    BankAccount acc = new BankAccount();
                    acc.money = 0;
                    acc.UserId = card.UserId;
                    acc.isNone = false;
                    card.AccountId = CreateBankAccount(acc);
                }
                db.Card.Add(card);
                db.SaveChanges();
                ChangeNone(card.AccountId);
            }
        }

        static private void ChangeNone(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                acc.isNone = true;
                foreach (var c in db.Card)
                {
                    if (c.AccountId == acc.id)
                        acc.isNone = false;
                }
                db.SaveChanges();
            }
        }

        static private int CreateBankAccount(BankAccount acc)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (acc.percent == 0)
                {
                    if (!acc.isCredit)
                        acc.percent = 2;
                    else acc.percent = 20;
                }
                acc.LastChanging = DateTime.Now;
                db.BankAccount.Add(acc);
                db.SaveChanges();
                int id = acc.id;
                return id;
            }
        }

        static public List<Card> GetCardsOfUser(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Card> result = new List<Card>();
                foreach (var card in db.Card)
                {
                    if (card.UserId == UserId)
                    {
                        AddPercent(card.AccountId, card.id);
                        result.Add(card);
                    }
                }
                return result;
            }
        }

        static public List<int> GetAccIdsOfUser(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<int> result = new List<int>();
                foreach (var acc in db.BankAccount)
                {
                    if (acc.UserId == UserId)
                    {
                        result.Add(acc.id);
                    }
                }
                return result;
            }
        }

        static public List<BankAccount> GetAccsOfUser(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<BankAccount> result = new List<BankAccount>();
                foreach (var acc in db.BankAccount)
                {
                    if (acc.UserId == UserId)
                    {
                        result.Add(acc);
                    }
                }
                return result;
            }
        }

        static public List<int> GetIdsOfCards()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<int> result = new List<int>();
                foreach (var card in db.Card)
                {
                    result.Add(card.id);
                }
                return result;
            }
        }

        static private void AddPercent(int id, int cardid)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                while (acc.LastChanging.AddDays(1) < DateTime.Now)
                {
                    double add = acc.money * acc.percent / 365 / 100;
                    AddMoney(cardid, add);
                    acc.LastChanging = acc.LastChanging.AddDays(1);
                }
                ChangeAcc(acc.id, acc.LastChanging);
            }
        }

        static private void ChangeAcc(int id, DateTime date)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                acc.LastChanging = date;
                db.SaveChanges();
            }
        }

        static public Card GetCardById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Card card = db.Card.Find(id);
                return card;
            }
        }

        static public List<int> GetCardIdsOfUser(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<int> result = new List<int>();
                foreach (var card in db.Card)
                {
                    if(card.UserId == UserId)
                    {
                        result.Add(card.id);
                    }
                }
                return result;
            }
        }

        static public BankAccount GetAccountById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                return acc;
            }
        }

        static public void AddMoney(int id, double add)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Card Card = db.Card.Find(id);
                BankAccount Account = db.BankAccount.Find(Card.AccountId);
                Account.money += add;
                db.SaveChanges();
            }
        }

        static public void DeleteCard(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Card card = db.Card.Find(id);
                int accid = card.AccountId;
                db.Card.Remove(card);
                db.SaveChanges();
                ChangeNone(accid);
            }
        }

        static public void DeleteBankAccount(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                db.BankAccount.Remove(acc);
                db.SaveChanges();
            }
        }

        static public void ChangePercent(BankAccount acc)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount bacc = db.BankAccount.Find(acc.id);
                bacc.percent = acc.percent;
                db.SaveChanges();
            }
        }

        static public void OpenCredit(BankAccount acc, int accid, string AdminId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                acc.isCredit = true;
                if (acc.money > 0)
                    acc.money *= -1;
                CreateBankAccount(acc);
                AddTransact(0, 0, acc.money * (-1), AdminId, acc.UserId);
                AddTransact(0, 0, acc.money, AdminId, acc.UserId);
                if (accid != 0)
                {
                    BankAccount account = db.BankAccount.Find(accid);
                    account.money = account.money + (acc.money*(-1));
                    db.SaveChanges();
                    ChangeNone(accid);
                }
                else
                {
                    BankAccount account = new BankAccount();
                    account.money = acc.money*(-1);
                    account.UserId = acc.UserId;
                    ChangeNone(CreateBankAccount(account));
                }
                ChangeNone(acc.id);
                
            }
        }

        static public void transact(int n1, double money, int n2)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Card c1 = db.Card.Find(n1);
                Card c2 = db.Card.Find(n2);
                AddMoney(n1, money * -1);
                if (c1.UserId == c2.UserId)
                    AddMoney(n2, money);
                else AddMoney(n2, money - money * 0.1);
            }
        }

        static public List<ApplicationUser> FindUsers(string s)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<ApplicationUser> result = new List<ApplicationUser>();
                foreach (var user in db.Users)
                {
                    if(user.name == null)
                    {
                        user.name = "";
                        user.surname = "";
                        user.patronymic = "";
                    }
                    if (s == "")
                        result.Add(user);
                    else if((user.name.Contains(s))||(user.surname.Contains(s))||(user.patronymic.Contains(s))||(user.Id.Contains(s))||(user.Email.Contains(s)))
                    {
                        result.Add(user);
                    }
                }
                return (result);
            }
        }

        static public List<DbTransact> GetTransactsOfUser(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<DbTransact> result = new List<DbTransact>();
                foreach (var transact in db.Transact)
                {
                    if (transact.AdminId == UserId)
                        result.Add(transact);
                    else
                    {
                        if ((transact.UserId == "")||(transact.UserId == null))
                        {
                            if (transact.CardInId == 0)
                            {
                                if (Repository.GetCardById(transact.CardOutId).UserId == UserId)
                                {
                                    result.Add(transact);
                                }
                            }
                            else if (transact.CardOutId == 0)
                            {
                                if (Repository.GetCardById(transact.CardInId).UserId == UserId)
                                {
                                    result.Add(transact);
                                }
                            }
                            else
                            {
                                try
                                {
                                    if ((Repository.GetCardById(transact.CardInId).UserId == UserId) || (Repository.GetCardById(transact.CardOutId).UserId == UserId))
                                    {
                                        result.Add(transact);
                                    }
                                }
                                catch { }
                            }
                        }
                        else if (transact.UserId == UserId)
                            result.Add(transact);
                    }
                }
                result = SortByDate(result);
                return result;
            }
        }

        static public List<DbTransact> GetTransactsOfCard(int CardId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<DbTransact> result = new List<DbTransact>();
                foreach (var transact in db.Transact)
                {
                    if ((transact.CardInId == CardId)||(transact.CardOutId == CardId))
                    {
                        result.Add(transact);
                    }
                }
                result = SortByDate(result);
                return result;
            }
        }

        static private List<DbTransact> SortByDate(List<DbTransact> tr)
        {
            IEnumerable<DbTransact> qresult = tr.OrderBy(t => t.date);
            qresult = qresult.Reverse();
            tr = qresult.ToList();
            return tr;
        }

        static public void AddTransact(int CardInId, int CardOutId, double money, string LogeduserId, string userId = "")
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DbTransact transact = new DbTransact();
                transact.AdminId = LogeduserId;
                transact.CardInId = CardInId;
                transact.CardOutId = CardOutId;
                transact.money = money;
                transact.UserId = userId;
                transact.date = DateTime.Now;
                db.Transact.Add(transact);
                db.SaveChanges();
            }
        }

        static public void AddMail(Mail mail)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Mails.Add(mail);
                db.SaveChanges();
            }
        }

        static public List<Mail> GetMails()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Mail> result = new List<Mail>();
                IEnumerable<Mail> nresult = db.Mails.OrderBy(t => t.time);
                nresult = nresult.Reverse();
                result = nresult.ToList();
                return result;
            }
        }

        static public Mail GetMailById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Mail result = db.Mails.Find(id);
                return result;
            }
        }

        static public void DeleteMail(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Mail m = db.Mails.Find(id);
                db.Mails.Remove(m);
                db.SaveChanges();
            }
        }
    }
}