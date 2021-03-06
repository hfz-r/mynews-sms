﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc.Filters;
using StockManagementSystem.Web.Security;

namespace StockManagementSystem.Controllers
{
    [HttpsRequirement(SslRequirement.NoMatter)]
    public class AccountController : Controller
    {
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserAccountModelFactory _userAccountModelFactory;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;
        private readonly IEmailSender _emailSender;
        private readonly IPictureService _pictureService;

        public AccountController(
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            MediaSettings mediaSettings,
            IAuthenticationService authenticationService,
            IUserAccountModelFactory userAccountModelFactory,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IUserActivityService userActivityService,
            IGenericAttributeService genericAttributeService,
            INotificationService notificationService,
            IWorkContext workContext,
            ITenantContext tenantContext,
            IEmailSender emailSender,
            IPictureService pictureService)
        {
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _mediaSettings = mediaSettings;
            _authenticationService = authenticationService;
            _userAccountModelFactory = userAccountModelFactory;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _userActivityService = userActivityService;
            _genericAttributeService = genericAttributeService;
            _notificationService = notificationService;
            _workContext = workContext;
            _tenantContext = tenantContext;
            _emailSender = emailSender;
            _pictureService = pictureService;
        }

        #region Login / logout

        //[HttpsRequirement(SslRequirement.Yes)]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> Login()
        {
            var model = await _userAccountModelFactory.PrepareLoginModel();
            return View(model);
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                    model.Username = model.Username.Trim();

                #region First time login

                var userFirstTime = _userSettings.UsernamesEnabled
                    ? await _userService.GetUserByUsernameAsync(model.Username)
                    : await _userService.GetUserByEmailAsync(model.Email);

                if (userFirstTime == null)
                {
                    ModelState.AddModelError(string.Empty, "Unknown user account");
                    _notificationService.ErrorNotification("Unknown user account");

                    model = await _userAccountModelFactory.PrepareLoginModel();
                    return View(model);
                }

                if (userFirstTime.LastLoginDateUtc == null)
                    return RedirectToAction("FirstTimeLogin", new { id = userFirstTime.Id });

                #endregion

                var loginResult = await _userRegistrationService
                    .ValidateUserAsync(_userSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                        {
                            var user = _userSettings.UsernamesEnabled
                                ? await _userService.GetUserByUsernameAsync(model.Username)
                                : await _userService.GetUserByEmailAsync(model.Email);

                            //sign in new user
                            await _authenticationService.SignInAsync(user, model.RememberMe);

                            //activity log
                            await _userActivityService.InsertActivityAsync(user, "Login", $"Login ('{user.Username}')", user);

                            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError(string.Empty, "No user account found");
                        _notificationService.ErrorNotification("No user account found");
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError(string.Empty, "User is deleted");
                        _notificationService.ErrorNotification("User is deleted");
                        break;
                    case UserLoginResults.NotActive:
                        ModelState.AddModelError(string.Empty, "Account is not active");
                        _notificationService.ErrorNotification("Account is not active");
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

            //If we got this far, something failed, redisplay form
            model = await _userAccountModelFactory.PrepareLoginModel();
            return View(model);
        }

        //[HttpsRequirement(SslRequirement.Yes)]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> FirstTimeLogin(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
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
        public async Task<IActionResult> FirstTimeLogin(FirstTimeLoginViewModel model, string returnUrl)
        {
            if (!model.Id.HasValue)
                throw new DefaultException("First time login failed");

            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.Id.Value);
                if (user != null)
                {
                    var changePasswordRequest = new ChangePasswordRequest(user.Email, true,
                        _userSettings.DefaultPasswordFormat, model.NewPassword, model.CurrentPassword);
                    var changePasswordResult = await _userRegistrationService.ChangePasswordAsync(changePasswordRequest);
                    if (changePasswordResult.Success)
                    {
                        //update login details
                        user.FailedLoginAttempts = 0;
                        user.CannotLoginUntilDateUtc = null;
                        user.LastLoginDateUtc = DateTime.UtcNow;
                        user.RegisteredInTenantId = _tenantContext.CurrentTenant.Id;

                        //add to 'Registered' role
                        var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);
                        if (registeredRole == null)
                            throw new DefaultException("'Registered' role could not be loaded");

                        user.AddUserRole(new UserRole { Role = registeredRole });

                        //remove from 'Guests' role
                        var guestRole = user.Roles.FirstOrDefault(r => r.SystemName == UserDefaults.GuestsRoleName);
                        if (guestRole != null)
                        {
                            user.RemoveUserRole(user.UserRoles.FirstOrDefault(mapping => mapping.RoleId == guestRole.Id));
                        }

                        await _userService.UpdateUserAsync(user);

                        //activity log
                        await _userActivityService.InsertActivityAsync(user, "FirstTimeLogin", $"First time login ('{user.Username}')", user);

                        //succeed notification
                        _notificationService.SuccessNotification("First time login succeed");

                        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToRoute("Login");

                        return Redirect(returnUrl);
                    }

                    //errors
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userActivityService.InsertActivityAsync("Logout", "Logout", _workContext.CurrentUser);
            await _authenticationService.SignOutAsync();

            return RedirectToRoute("Login");
        }

        #endregion

        #region Register

        //[HttpsRequirement(SslRequirement.Yes)]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel();
            model = await _userAccountModelFactory.PrepareRegisterModel(model, false);

            return View(model);
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (_workContext.CurrentUser.IsRegistered())
            {
                //Already registered user. 
                await _authenticationService.SignOutAsync();

                //Save a new record
                _workContext.CurrentUser = await _userService.InsertGuestUserAsync();
            }
            var user = _workContext.CurrentUser;
            user.RegisteredInTenantId = _tenantContext.CurrentTenant.Id;

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                    model.Username = model.Username.Trim();

                var registrationRequest = new UserRegistrationRequest(
                    user,
                    model.Email,
                    _userSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _userSettings.DefaultPasswordFormat,
                    _tenantContext.CurrentTenant.Id);
                var registrationResult = await _userRegistrationService.RegisterUserAsync(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.GenderAttribute, model.Gender);
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute, model.FirstName);
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.LastNameAttribute, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_userSettings.PhoneEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.PhoneAttribute, model.Phone);

                    //login
                    await _authenticationService.SignInAsync(user, true);

                    if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                        return RedirectToRoute("HomePage");

                    return Redirect(returnUrl);
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = await _userAccountModelFactory.PrepareRegisterModel(model, true);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = "Username not available";

            if (_userSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentUser?.Username != null &&
                    _workContext.CurrentUser.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = "Current username";
                }
                else
                {
                    var user = await _userService.GetUserByUsernameAsync(username);
                    if (user == null)
                    {
                        statusText = "Username available";
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        #endregion

        #region ForgotPassword

        //[HttpsRequirement(SslRequirement.Yes)]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> ForgotPassword()
        {
            var model = await _userAccountModelFactory.PrepareForgotPasswordModel();
            return View(model);
        }

        [HttpPost, ActionName("ForgotPassword")]
        [AntiForgery]
        [FormValueRequired("send-email")]
        public async Task<IActionResult> ForgotPasswordSend(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user != null && user.Active && !user.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());

                    DateTime? generatedDateTime = DateTime.UtcNow;
                    await _genericAttributeService.SaveAttributeAsync(user,
                        UserDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    var callbackUrl = Url.RouteUrl("ForgotPasswordConfirm", new
                    {
                        token = passwordRecoveryToken.ToString(),
                        email = model.Email,
                    },
                        protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                        "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                    model.Result = "Email with instructions has been sent to you.";
                }
                else
                {
                    model.Result = "Email not found.";
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //[HttpsRequirement(SslRequirement.Yes)]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> ForgotPasswordConfirm(string token, string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return RedirectToRoute("Login");

            if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PasswordRecoveryTokenAttribute)))
            {
                return View(new ForgotPasswordConfirmViewModel
                {
                    DisablePasswordChanging = true,
                    Result = "Your password already has been changed. For changing it once more, you need to again recover the password."
                });
            }

            var model = await _userAccountModelFactory.PrepareForgotPasswordConfirmModel();

            //validate token
            if (!await _userService.IsPasswordRecoveryTokenValidAsync(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Wrong password recovery token";
            }

            if (await _userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Your password recovery link is expired";
            }

            return View(model);
        }

        [HttpPost, ActionName("ForgotPasswordConfirm")]
        [AntiForgery]
        [FormValueRequired("set-password")]
        public async Task<IActionResult> ForgotPasswordConfirmPost(string token, string email, ForgotPasswordConfirmViewModel model)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return RedirectToRoute("Login");

            //validate token
            if (!await _userService.IsPasswordRecoveryTokenValidAsync(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Wrong password recovery token";
                return View(model);
            }

            if (await _userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Your password recovery link is expired";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = await _userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(email, false,
                    _userSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.PasswordRecoveryTokenAttribute, "");

                    model.DisablePasswordChanging = true;
                    model.Result = "Your password has been changed";
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Change password

        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> ChangePassword()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            var model = await _userAccountModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_userService.PasswordIsExpired(_workContext.CurrentUser))
                ModelState.AddModelError(string.Empty, "Your password has expired, please create a new one");

            return View(model);
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email, true,
                    _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = await _userRegistrationService.ChangePasswordAsync(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = "Password was changed";
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account

        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> Info()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            var model = new UserInfoModel();
            model = await _userAccountModelFactory.PrepareUserInfoModel(model, _workContext.CurrentUser, false);

            return View(model);
        }

        [HttpPost]
        [AntiForgery]
        public async Task<IActionResult> Info(UserInfoModel model)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            var user = _workContext.CurrentUser;

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UsernamesEnabled && _userSettings.AllowUsersToChangeUsernames)
                    {
                        if (!user.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            await _userRegistrationService.SetUsernameAsync(user, model.Username.Trim());

                            //re-authenticate
                            await _authenticationService.SignInAsync(user, true);
                        }
                    }

                    //email
                    if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        await _userRegistrationService.SetEmailAsync(user, model.Email.Trim());

                        //re-authenticate (if usernames are disabled)
                        if (!_userSettings.UsernamesEnabled)
                            await _authenticationService.SignInAsync(user, true);
                    }

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.TimeZoneIdAttribute, model.TimeZoneId);

                    //form fields
                    if (_userSettings.GenderEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.GenderAttribute, model.Gender);

                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute, model.FirstName);
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.LastNameAttribute, model.LastName);

                    if (_userSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.DateOfBirthAttribute, dateOfBirth);
                    }

                    if (_userSettings.PhoneEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.PhoneAttribute, model.Phone);

                    return RedirectToRoute("UserInfo");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model = await _userAccountModelFactory.PrepareUserInfoModel(model, user, true);

            return View(model);
        }

        #endregion

        #region Avatar

        [HttpsRequirement(SslRequirement.NoMatter)]
        public async Task<IActionResult> Avatar()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var model = new UserAvatarModel();
            model = await _userAccountModelFactory.PrepareUserAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [AntiForgery]
        [FormValueRequired("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(UserAvatarModel model, IFormFile uploadedFile)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = await _pictureService.GetPictureById(await _genericAttributeService.GetAttributeAsync<int>(user, 
                        UserDefaults.AvatarPictureIdAttribute));
                    if (!string.IsNullOrEmpty(uploadedFile?.FileName))
                    {
                        var avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new DefaultException($"Maximum avatar size is {avatarMaxSize} bytes.");

                        var userPictureBinary = await _pictureService.GetDownloadBits(uploadedFile);
                        userAvatar = userAvatar != null
                            ? await _pictureService.UpdatePicture(userAvatar.Id, userPictureBinary, uploadedFile.ContentType, null)
                            : await _pictureService.InsertPicture(userPictureBinary, uploadedFile.ContentType, null);
                    }

                    var userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.AvatarPictureIdAttribute, userAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        await _genericAttributeService.GetAttributeAsync<int>(user,
                            UserDefaults.AvatarPictureIdAttribute), _mediaSettings.AvatarPictureSize, false);

                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = await _userAccountModelFactory.PrepareUserAvatarModel(model);

            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [AntiForgery]
        [FormValueRequired("remove-avatar")]
        public async Task<IActionResult> RemoveAvatar(UserAvatarModel model)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return Challenge();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            var userAvatar = await _pictureService.GetPictureById(await _genericAttributeService.GetAttributeAsync<int>(user,
                UserDefaults.AvatarPictureIdAttribute));
            if (userAvatar != null)
                await _pictureService.DeletePicture(userAvatar);

            await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.AvatarPictureIdAttribute, 0);

            return RedirectToRoute("UserAvatar");
        }

        #endregion
    }
}