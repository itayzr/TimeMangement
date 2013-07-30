using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using Raven.Abstractions.Exceptions;
using TimeMangement.Helpers;
using TimeMangement.Models;
using TimeMangement.Tasks;
using TimeMangement.ViewModels;

namespace TimeMangement.Controllers
{
    public class AccountController : RavenController
    {
        //[ChildActionOnly]
        //public ActionResult AdminBanner()
        //{
        //    if (User.Identity.IsAuthenticated)
        //        return PartialView(RavenSession.GetUser(User.Identity.Name));
        //    return PartialView(null);
        //}

        [GET("account/login")]
        public ActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [POST("account/login")]
        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var user = RavenSession.GetUser(model.Login, false);
            if (user != null && user.Enabled && user.ValidatePassword(model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
                user.LastSeen = DateTimeOffset.Now;
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("login", "The user name or password provided is incorrect.");
            return View(model);
        }

        [GET("account/logout")]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [GET("account/register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [POST("account/register")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid == false)
                return View();

            // Attempt to register the user
            RavenSession.Advanced.UseOptimisticConcurrency = true; // make sure we are not overwriting an existing user
            model.SendOutKey = Guid.NewGuid().ToString();
            RavenSession.Store(new User
            {
                Id = "users/" + model.Email.ToLower(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateJoined = DateTimeOffset.Now,
                Enabled = false,
            }.SetPassword(model.Password));

            try
            {
                RavenSession.SaveChanges();
                RavenSession.Advanced.UseOptimisticConcurrency = false;

                //TaskExecutor.ExcuteLater(new SendEmailTask
                //{
                //    ReplyTo = "support@hibernatingrhinos.com",
                //    Subject = "You have a user in HibernatingRhinos.com!",
                //    SendTo = new[] { model.Email, "support@hibernatingrhinos.com" },
                //    ViewName = "RegistrationSuccessful",
                //    Model = model,
                //});

                return RedirectToAction("RegistrationSuccessful");
            }
            catch (ConcurrencyException)
            {
                ModelState.AddModelError("", "A user name for that e-mail address already exists. Please enter a different e-mail address.");
                RavenSession.Dispose();
                RavenSession = null;
            }

            //ModelState.AddModelError("", "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        [GET("account/changePassword")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [POST("account/changePassword")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                var changePasswordSucceeded = false;
                try
                {
                    var currentUser = RavenSession.GetUser(User.Identity.Name, false);
                    if (currentUser.ValidatePassword(model.OldPassword))
                    {
                        currentUser.SetPassword(model.NewPassword);
                        RavenSession.SaveChanges();
                        changePasswordSucceeded = true;
                    }
                }
                catch (Exception)
                {
                    // TODO: Log e
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[Authorize]
        [GET("account/changePassword/success")]
        public ActionResult ChangePasswordSuccess()
        {
            return View("Message", new MessageViewModel
            {
                Title = "Change Password",
                Message = "Your password has been changed successfully.",
            });
        }

        
        [HttpGet]
        [GET("account/register/success")]
        public ActionResult RegistrationSuccessful()
        {
            return View("Message", new MessageViewModel
            {
                Title = "Registration Successful",
                Message = "An e-mail with confirmation link was sent to your e-mail address, please click on it to complete registration and to enable your profile.",
            });
        }

        [HttpGet]
        [GET("account/ForgotPassword")]
        public ActionResult ForgotPassword(string email, string key)
        {
            // If both key and email are provided, user got here by clicking on the link in the first email
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(key))
            {
                var user = RavenSession.GetUser(email, false);
                if (user != null)
                {
                    if (user.SendOutKey.Equals(key, StringComparison.InvariantCultureIgnoreCase) == false) // incorrect key, ignore request
                        return RedirectToAction("ForgotPassword");

                    //var newPassword = StringRandomizer.GetRandomString(8, false, true);
                    //user.SetPassword(newPassword);
                    //user.SendOutKey = Guid.NewGuid().ToString();
                    //TaskExecutor.ExcuteLater(new SendEmailTask
                    //{
                    //    ReplyTo = "support@hibernatingrhinos.com",
                    //    Subject = "Password was reset successfully",
                    //    SendTo = new[] { user.Email },
                    //    ViewName = "PasswordReset",
                    //    Model = new ForgotPasswordModel { Email = email, Key = newPassword },
                    //});
                    return RedirectToAction("PasswordResetSuccessfully");
                }
            }

            return View(new ForgotPasswordModel { Email = email });
        }

        [HttpPost]
        [POST("account/ForgotPassword")]
        public ActionResult ForgotPassword(ForgotPasswordModel input)
        {
            if (ModelState.IsValid)
            {
                var user = RavenSession.GetUser(input.Email, false);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "No user found for this email address");
                }
                else
                {
                    input.Key = user.SendOutKey;
                    //TaskExecutor.ExcuteLater(new SendEmailTask
                    //{
                    //    ReplyTo = "noreply@ravendb.net",
                    //    Subject = "Confirm password reset",
                    //    SendTo = new[] { user.Email },
                    //    ViewName = "ForgotPassword",
                    //    Model = input,
                    //});
                    return RedirectToAction("PasswordResetPending");
                }
            }

            return View(input);
        }

        [GET("account/ForgotPassword/success")]
        public ActionResult PasswordResetSuccessfully()
        {
            return View("Message", new MessageViewModel
            {
                Title = "Password was reset successfully",
                Message = "An e-mail with the new password was sent to your e-mail address.",
            });
        }

        [GET("account/ForgotPassword/Pending")]
        public ActionResult PasswordResetPending()
        {
            var message = new MessageViewModel
            {
                Title = "Password reset pending",
                Message =
                    "An e-mail with a confirmation link was sent to the email address you specified. Please click on the link provided in it to complete this process."
            };
            return View("Message", message);
        }

        [GET("account/PermissionDenied")]
        public ActionResult PermissionDenied()
        {
            return View();
        }

        [GET("account/changeuser")]
        public ActionResult ChangeUser()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn", new { ReturnUrl = HttpContext.Request.QueryString["ReturnUrl"] });
        }
    }
}
