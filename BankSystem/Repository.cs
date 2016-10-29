using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BankSystem.Models;
using BankSystem.Models.ViewModels;
using BankSystem.Models.DbModels;

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
                if (user.name != null)
                {
                    appuser.name = user.name;
                }
                if (user.patronymic != null)
                {
                    appuser.patronymic = user.patronymic;
                }
                if (user.phone != null)
                {
                    appuser.phone = user.phone;
                }
                if (user.adress != null)
                {
                    appuser.adress = user.adress;
                }
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
                        result.Add(card);
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
                        result.Add(acc);
                }
                return result;
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

        static public BankAccount GetAccountById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                BankAccount acc = db.BankAccount.Find(id);
                return acc;
            }
        }

        static public void AddMoney(int id, int add)
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
    }
}