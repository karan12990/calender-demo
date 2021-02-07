using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using GoogleCalenderDemo.Dtos;
using GoogleCalenderDemo.Models;
using GoogleCalenderDemo.Utils;
using WebMatrix.WebData;

namespace GoogleCalenderDemo.Controllers
{
    /// <summary>
    /// AccountController
    /// </summary>
    [Authorize]
    [SimpleMembership]
    public class AccountController : Controller
    {

        /// <summary>
        /// Login Get
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        /// <summary>
        /// Login Post
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
                return RedirectToLocal(returnUrl);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// POST: /Account/LogOff
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET: /Account/Register
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// POST: /Account/Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {model.Email});
                    WebSecurity.Login(model.UserName, model.Password);

                    // Get Google authorization to manage calendars
                    var url = GoogleAuthorizationHelper.GetAuthorizationUrl(model.Email);
                    Response.Redirect(url);

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", Helper.ErrorCodeToString(e.StatusCode));
                }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Authorization(string code)
        {
            // Retrieve the authenticator and save it in session for future use
            var authenticator = GoogleAuthorizationHelper.GetAuthenticator(code);
            Session["authenticator"] = authenticator;

            // Save the refresh token locally
            using (var dbContext = new CalendarContext())
            {
                var userName = User.Identity.Name;
                var userRegistry = dbContext.GoogleRefreshTokens.FirstOrDefault(c => c.UserName == userName);

                if (userRegistry == null)
                    dbContext.GoogleRefreshTokens.Add(
                        new GoogleRefreshToken
                        {
                            UserName = userName,
                            RefreshToken = authenticator.RefreshToken
                        });
                else
                    userRegistry.RefreshToken = authenticator.RefreshToken;

                dbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}