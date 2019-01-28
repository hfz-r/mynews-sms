using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEmailSender _emailSender;
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userService,
            IAuthenticationService authenticationService,
            IUserActivityService userActivityService,
            IEmailSender emailSender,
            IWorkContext workContext,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _authenticationService = authenticationService;
            _userActivityService = userActivityService;
            _emailSender = emailSender;
            _workContext = workContext;
            _notificationService = notificationService;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                if (model.UserName != null)
                    model.UserName = model.UserName.Trim();

                //first time login
                var user = await _userService.GetUserByUsernameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Unknown user account");
                    _notificationService.ErrorNotification("Unknown user account");

                    return View(model);
                }

                if (user.LastLoginDateUtc == null)
                    return RedirectToAction("FirstTimeLogin", new {id = user.Id});

                var loginResult = await _userService.ValidateUserAsync(model.UserName, model.Password, model.RememberMe);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                    {
                        //sign in new user
                        await _authenticationService.SignInAsync(user, model.RememberMe);

                        await _userActivityService.InsertActivityAsync(user, "Login", $"Login ('{user.UserName}')",
                            user);

                        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToAction("Dashboard", "Home");

                        return Redirect(returnUrl);
                    }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError(string.Empty, "No user account found");
                        _notificationService.ErrorNotification("No user account found");
                        break;
                    case UserLoginResults.NotRegistered:
                        ModelState.AddModelError(string.Empty, "Account is not registered");
                        _notificationService.ErrorNotification("Account is not registered");
                        break;
                    case UserLoginResults.LockedOut:
                        ModelState.AddModelError(string.Empty, "User is locked out");
                        _notificationService.ErrorNotification("User is locked out");
                        break;
                    default:
                        ModelState.AddModelError(string.Empty, "The credentials provided are incorrect");
                        _notificationService.ErrorNotification("The credentials provided are incorrect");
                        break;
                }
            }

            return View(model);
        }

        //
        // GET: /Account/FirstTimeLogin
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FirstTimeLogin(int Id)
        {
            var user = await _userService.GetUserByIdAsync(Id);
            if (user == null)
                return RedirectToAction("Login");

            var model = new FirstTimeLoginViewModel
            {
                Id = user.Id
            };
            return View(model);
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> FirstTimeLogin(FirstTimeLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user != null)
                {
                    var changePasswordResult =
                        await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (changePasswordResult.Errors.Any())
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View("FirstTimeLogin", model);
                    }

                    if (changePasswordResult.Succeeded)
                    {
                        var loginResult = await _userService.ValidateUserAsync(user.UserName, model.NewPassword, false);
                        if (loginResult != UserLoginResults.Successful)
                        {
                            ModelState.AddModelError(string.Empty, "First time login failed.");
                            return View("FirstTimeLogin", model);
                        }

                        await _authenticationService.SignInAsync(user, false);

                        await _userActivityService.InsertActivityAsync(user, "Login1stTime", $"First time login ('{user.UserName}')", user);

                        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToAction("Dashboard", "Home");

                        return Redirect(returnUrl);
                    }
                }
            }

            return View("FirstTimeLogin", model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserGuid = Guid.NewGuid(),
                    UserName = model.UserName,
                    Email = model.Email,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    AccessFailedCount = 0,
                    LockoutEnd = null,
                    LastActivityDateUtc = DateTime.UtcNow,
                    //LastLoginDateUtc = DateTime.UtcNow,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, IdentityDefaults.RegisteredRoleName);
                    await _authenticationService.SignInAsync(user, false);
                    //await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _userActivityService.InsertActivityAsync("Logout", "Logout", _workContext.CurrentUser);
            await _authenticationService.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                //var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Unknown user with this email");
                    _notificationService.ErrorNotification("Unknown user with this email");
                    // If user does not exist or is not confirmed.
                    return View("ForgotPassword");
                }

                if (user.Email == model.Email)
                {
                    //Generate password token
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    //Create URL with above token
                    var callbackUrl = Url.Action("ResetPassword", "Account", new {userId = user.Id, code = code},
                        protocol: HttpContext.Request.Scheme);

                    _notificationService.SuccessNotification("Successfully sent password to email!");

                    //Call send email methods
                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                        "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                    ModelState.AddModelError(string.Empty, "Successfully sent link reset password to email!");
                    _notificationService.SuccessNotification("Successfully sent link reset password to email!");

                    return View("ForgotPassword");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}