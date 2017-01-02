using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BankSystem.Models;
using BankSystem.Models.ViewModels;
using System.Drawing;
using System.IO;
using BankSystem.Models.DbModels;
using System.Collections.Generic;

namespace BankSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неудачная попытка входа.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
            // Если пользователь введет неправильные коды за указанное время, его учетная запись 
            // будет заблокирована на заданный период. 
            // Параметры блокирования учетных записей можно настроить в IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неправильный код.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // Дополнительные сведения о том, как включить подтверждение учетной записи и сброс пароля, см. по адресу: http://go.microsoft.com/fwlink/?LinkID=320771
                    // Отправка сообщения электронной почты с этой ссылкой
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Подтверждение учетной записи", "Подтвердите вашу учетную запись, щелкнув <a href=\"" + callbackUrl + "\">здесь</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Не показывать, что пользователь не существует или не подтвержден
                    return View("ForgotPasswordConfirmation");
                }

                // Дополнительные сведения о том, как включить подтверждение учетной записи и сброс пароля, см. по адресу: http://go.microsoft.com/fwlink/?LinkID=320771
                // Отправка сообщения электронной почты с этой ссылкой
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Сброс пароля", "Сбросьте ваш пароль, щелкнув <a href=\"" + callbackUrl + "\">здесь</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Не показывать, что пользователь не существует
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Создание и отправка маркера
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Получение сведений о пользователе от внешнего поставщика входа
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult ChangeData(string modelId)
        {
            ApplicationUser user = new ApplicationUser();
            user = Repository.GetUser(modelId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewUser model = GetViewModel(user);
            return View(model);
        }

        public ActionResult ChangeData(ViewUser user)
        {
            if (user.id != null)
            {
                Repository.ChangeUser(user);
                return RedirectToAction("UserPage", "Account", new { id = user.id });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(string modelId, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                // установка массива байтов

                Repository.AddPicture(modelId, imageData);

                return RedirectToAction("UserPage", "Account", new { id = User.Identity.GetUserId() });
            }
            return RedirectToAction("UserPage", "Account", new { id = User.Identity.GetUserId() });
        }

        public ActionResult Delete(string id)
        {
            Repository.DeleteImage(id);
            return RedirectToAction("UserPage", "Account", new { id = id });
        }

        private byte[] ImageToByteArray(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private ViewUser GetViewModel(ApplicationUser user)
        {
            ViewUser model = new ViewUser();
            
            if (user.Image == null)
            {
                string path = HttpContext.Server.MapPath("~/Images/default.png");
                Image image = Image.FromFile(path);
                user.Image = ImageToByteArray(image);
                Repository.AddPicture(user.Id, user.Image);

            }
            model.surname = "";
            model.name = "";
            model.patronymic = "";
            model.phone = "";
            model.adress = "";
            model.picture = new Picture(user.Image);
            if (model.picture.HtmlRaw == "")
                Repository.DeleteImage(user.Id);
            model.id = user.Id;
            model.isAdmin = user.isAdmin;
            if (user.surname != null)
            {
                model.surname = user.surname;
            }
            if (user.name != null)
            {
                model.name = user.name;
            }
            if (user.patronymic != null)
            {
                model.patronymic = user.patronymic;
            }
            if (user.phone != null)
            {
                model.phone = user.phone;
            }
            if (user.adress != null)
            {
                model.adress = user.adress;
            }
            model.HasPassword = true;
            model.isUser = false;
            if (user.PasswordHash == "")
            {
                model.HasPassword = false;
            }
            if (model.id == User.Identity.GetUserId())
            {
                model.isUser = true;
            }
            

            return model;
        }

        public ActionResult UserPage(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = new ApplicationUser();
                user = Repository.GetUser(id);
                if (user != null)
                {
                    if ((User.Identity.GetUserId() == id) || (Repository.GetUser(User.Identity.GetUserId()).isAdmin))
                    {
                        ViewUser model = GetViewModel(user);
                        return View(model);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangeAdmin(string userId)
        {
            
            Repository.ChangeUserAdmin(userId);
            return RedirectToAction("UserPage", "Account", new { id = userId });
        }

        [HttpGet]
        public ActionResult CreateNewCard(string userId)
        {
            Card model = new Card();
            model.UserId = userId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateNewCard(Card card)
        {
            List<int> ids = Repository.GetAccIdsOfUser(card.UserId);
            if ((card.AccountId != 0)&&(!ids.Contains(card.AccountId)))
            {
                ModelState.AddModelError("AccountId", "Такого счёта не существует!");
            }
            if (ModelState.IsValid)
            {
                Repository.CreateCard(card);
                return RedirectToAction("UserPage", "Account", new { id = card.UserId });
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View(card);

        }

        [HttpGet]
        public ActionResult ChangeMoney(int id, bool isAdd)
        {
            ChangeMoney model = new ChangeMoney();
            model.CardId = id;
            model.isAdd = isAdd;
            
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeMoney(ChangeMoney res)
        {
            if (res.money < 0)
            {
                ModelState.AddModelError("money", "Нельзя брать отрицательную сумму!");
            }
            if((!res.isAdd)&&(res.money > Repository.GetAccountById(Repository.GetCardById(res.CardId).AccountId).money))
            {
                ModelState.AddModelError("money", "У него нет столько денег!");
            }
            if (ModelState.IsValid)
            {
                if (!res.isAdd)
                    res.money *= -1;
                Repository.AddMoney(res.CardId, res.money);
                Repository.AddTransact(0, res.CardId, res.money, User.Identity.GetUserId());
                return RedirectToAction("UserPage", "Account", new { id = Repository.GetCardById(res.CardId).UserId });
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View(res);
        }

        public ActionResult RemoveCard(string userId, int id)
        {
            Repository.DeleteCard(id);
            return RedirectToAction("UserPage", "Account", new { id = userId });
        }

        [HttpGet]
        public ActionResult RemoveBAcc(string userId, int id)
        {
            Repository.DeleteBankAccount(id);
            return RedirectToAction("UserPage", "Account", new { id = userId });
        }

        [HttpGet]
        public ActionResult ChangePercent(string userId, int id)
        {
            BankAccount acc = Repository.GetAccountById(id);
            return View(acc);
        }
        [HttpPost]
        public ActionResult ChangePercent(BankAccount acc)
        {
            if (acc.percent < 0)
            {
                ModelState.AddModelError("percent", "Процент не может быть отрицательным!");
            }
            if (ModelState.IsValid)
            {
                Repository.ChangePercent(acc);
                return RedirectToAction("UserPage", "Account", new { id = acc.UserId });
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View(acc);
        }

        [HttpGet]
        public ActionResult OpenCredit(string userId)
        {
            BankAccount acc = new BankAccount();
            acc.UserId = userId;
            return (View(acc));
        }

        [HttpPost]
        public ActionResult OpenCredit(BankAccount acc, int accid = 0)
        {
            if(acc.money < 0)
            {
                ModelState.AddModelError("money", "Нельзя брать отрицательную сумму!");
            }
            if(acc.percent < 0)
            {
                ModelState.AddModelError("percent", "Процент не может быть отрицательным!");
            }
            if(ModelState.IsValid)
            {
                List<int> ids = Repository.GetAccIdsOfUser(acc.UserId);
                if((accid != 0)&&(!ids.Contains(accid)))
                {
                    ModelState.AddModelError("UserPage", "Такого счёта не существует!");
                }
            }
            if (ModelState.IsValid)
            {
                Repository.OpenCredit(acc, accid, User.Identity.GetUserId());
                return RedirectToAction("UserPage", "Account", new { id = acc.UserId });
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View(acc);
        }

        [HttpGet]
        public ActionResult transact()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult transact(ViewTransact transact)
        {
            
            List<int> ids = Repository.GetIdsOfCards();
            if (!ids.Contains(transact.CardInId))
            {
                ModelState.AddModelError("CardInId", "Такой карточки не существует!");
            }
            if (!ids.Contains(transact.CardOutId))
            {
                ModelState.AddModelError("CardOutId", "Такой карточки не существует!");
            }
            ids = Repository.GetCardIdsOfUser(User.Identity.GetUserId());
            if(!ids.Contains(transact.CardOutId))
            {
                ModelState.AddModelError("CardOutId", "Вы должны отправлять деньги со своего счёта!");
            }
            if (ModelState.IsValid)
            {
                BankAccount acc = Repository.GetAccountById(Repository.GetCardById(transact.CardOutId).AccountId);
                if ((acc.isCredit) && (acc.money < 0))
                {
                    ModelState.AddModelError("money", "Вы не можете переводить деньги с кредитного счёта!");
                }
                if (transact.money > acc.money)
                {
                    ModelState.AddModelError("money", "Вы не можете отправлять больше денег, чем у вас есть!");
                }
                if (transact.money < 0)
                {
                    ModelState.AddModelError("money", "Значение должно быть положительным!");
                }
            }
            if (ModelState.IsValid)
            {

                Repository.transact(transact.CardOutId, transact.money, transact.CardInId);
                Repository.AddTransact(transact.CardInId, transact.CardOutId, transact.money, User.Identity.GetUserId());
                return RedirectToAction("UserPage", "Account", new { id = User.Identity.GetUserId() });
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View(transact);
        }

        public ActionResult FindUsers(string s = "")
        {
            if (!Repository.FindUserById(User.Identity.GetUserId()).isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            List<ApplicationUser> model = Repository.FindUsers(s);
            return View(model);
        }

        [HttpGet]
        public ActionResult Transacts(byte byCard, string UserId = "", int CardId = 0)
        {
            
            List<DbTransact> model = new List<DbTransact>();
            if (byCard == 1)
                model = Repository.GetTransactsOfCard(CardId);
            else model = Repository.GetTransactsOfUser(UserId);
            if (CardId != 0)
                ViewData["UserId"] = Repository.GetCardById(CardId).UserId;
            else ViewData["UserId"] = UserId;
            return View(model);
        }

        public ActionResult GetMails()
        {
            if(!Repository.FindUserById(User.Identity.GetUserId()).isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Mail> model = Repository.GetMails();
            return View(model);
        }

        
        public ActionResult LookMail(string id)
        {
            id = id.Replace('☻','.');
            if (!Repository.FindUserById(User.Identity.GetUserId()).isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            Mail model = Repository.GetMailById(id);
            return View(model);
        }

        public ActionResult DeleteMail(string id)
        {
            if (!Repository.FindUserById(User.Identity.GetUserId()).isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            Repository.DeleteMail(id);
            return RedirectToAction("GetMails", "Account");
        }
        /*public ActionResult Transacts(List<DbTransact> model, DateTime date)
        {

        }*/
        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}