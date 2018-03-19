using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Globalization;
namespace EasySurvey.Controllers
{
    public class AccountController : Controller
    {


        [HttpGet]
        public ActionResult LogIn()
        {
            return View();

        }
        [HttpPost]
        public ActionResult LogIn(String username, string password, string ReturnUrl, bool rememberMe = true)
        {
            
           // Roles.AddUserToRole("Doctor", "Doctors");
            
            
            if (!ValidateLogOn(username, password))
            {
                ViewData["rememberMe"] = rememberMe;
                return View();

            }
            FormsAuthenticationTicket autticket = new FormsAuthenticationTicket
                (1, username, DateTime.Now, DateTime.Now.AddMinutes(15), rememberMe, username);
            string encTicket = FormsAuthentication.Encrypt(autticket);
            this.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");


        }
        [Authorize]
        public ActionResult ChangePassword()
        {

            ViewData["PasswordLength"] = Membership.MinRequiredPasswordLength;

            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = Membership.MinRequiredPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                MembershipUser user = Membership.GetUser();
                //string resetPass = user.ResetPassword();

                if (user.ChangePassword(currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        private bool ValidateLogOn(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!Membership.ValidateUser(username, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }
        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < Membership.MinRequiredPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         Membership.MinRequiredPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

    }
}
