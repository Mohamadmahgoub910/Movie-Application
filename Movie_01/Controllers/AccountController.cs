using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.ViewModels.Account;
using System.Text;
using System.Text.Encodings.Web;

namespace MovieApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // ═══════════════════════════════════════════════════════════
        // REGISTER
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Add to User role
                await _userManager.AddToRoleAsync(user, "User");

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    Request.Scheme);

                // Send confirmation email
                try
                {
                    await _emailService.SendEmailConfirmationAsync(user.Email, user.FullName, confirmationLink);
                }
                catch (Exception ex)
                {
                    // Log error but don't stop the registration process
                    // You can add logging here
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                }

                TempData["Success"] = "تم إنشاء الحساب بنجاح! يرجى تفعيل حسابك من خلال الرابط المرسل إلى بريدك الإلكتروني.";
                return RedirectToAction("RegisterConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, GetArabicErrorMessage(error.Code, error.Description));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // EMAIL CONFIRMATION
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "رابط التفعيل غير صحيح.";
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                // Send welcome email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);
                }
                catch (Exception ex)
                {
                    // Log error but don't stop the process
                    Console.WriteLine($"Welcome email sending failed: {ex.Message}");
                }

                TempData["Success"] = "تم تفعيل حسابك بنجاح! يمكنك الآن تسجيل الدخول.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "حدث خطأ أثناء تفعيل الحساب. الرابط قد يكون منتهي الصلاحية.";
            return RedirectToAction("Index", "Home", new { area = "Public" });
        }

        // ═══════════════════════════════════════════════════════════
        // LOGIN
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "هذا الحساب غير مفعل. يرجى التواصل مع الإدارة.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "يرجى تفعيل حسابك من خلال البريد الإلكتروني المرسل إليك.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Check if user is Admin
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirect based on role
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "Public" });
                }
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "تم قفل حسابك مؤقتاً بسبب محاولات تسجيل دخول فاشلة متعددة. يرجى المحاولة بعد 5 دقائق.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            return View(model);
        }

        // ═══════════════════════════════════════════════════════════
        // LOGOUT
        // ═══════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Index", "Home", new { area = "Public" });
        }

        // ═══════════════════════════════════════════════════════════
        // FORGOT PASSWORD
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = encodedToken },
                Request.Scheme);

            // Send reset email
            try
            {
                await _emailService.SendPasswordResetAsync(user.Email, user.FullName, resetLink);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Password reset email sending failed: {ex.Message}");
            }

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // RESET PASSWORD
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult ResetPassword(string token = null)
        {
            if (token == null)
            {
                TempData["Error"] = "رمز إعادة تعيين كلمة المرور مطلوب.";
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
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
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم إعادة تعيين كلمة المرور بنجاح!";
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, GetArabicErrorMessage(error.Code, error.Description));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // ACCESS DENIED
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // HELPER METHOD - Arabic Error Messages
        // ═══════════════════════════════════════════════════════════

        private string GetArabicErrorMessage(string errorCode, string defaultMessage)
        {
            return errorCode switch
            {
                "DuplicateUserName" => "اسم المستخدم موجود بالفعل.",
                "DuplicateEmail" => "البريد الإلكتروني مستخدم بالفعل.",
                "InvalidEmail" => "البريد الإلكتروني غير صحيح.",
                "InvalidToken" => "الرمز غير صحيح أو منتهي الصلاحية.",
                "PasswordMismatch" => "كلمة المرور غير صحيحة.",
                "PasswordTooShort" => "كلمة المرور يجب أن تكون 6 أحرف على الأقل.",
                "PasswordRequiresNonAlphanumeric" => "كلمة المرور يجب أن تحتوي على رمز خاص (!@#$%^&*).",
                "PasswordRequiresDigit" => "كلمة المرور يجب أن تحتوي على رقم (0-9).",
                "PasswordRequiresLower" => "كلمة المرور يجب أن تحتوي على حرف صغير (a-z).",
                "PasswordRequiresUpper" => "كلمة المرور يجب أن تحتوي على حرف كبير (A-Z).",
                "UserNotFound" => "المستخدم غير موجود.",
                "UserAlreadyHasPassword" => "المستخدم لديه كلمة مرور بالفعل.",
                "UserLockoutNotEnabled" => "قفل الحساب غير مفعل.",
                "UserAlreadyInRole" => "المستخدم مضاف للدور بالفعل.",
                "UserNotInRole" => "المستخدم ليس في هذا الدور.",
                "RoleNotFound" => "الدور غير موجود.",
                _ => defaultMessage
            };
        }
    }
}