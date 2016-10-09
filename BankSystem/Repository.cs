using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BankSystem.Models;
using BankSystem.Models.ViewModels;

namespace BankSystem
{
    public class Repository
    {
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
    }
}