using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CertificateManager;
using EggBasket.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using EncryptDecryptLib;

namespace EggBasket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<EggBasketUser> _signInManager;
        private readonly UserManager<EggBasketUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly EggBasket.Data.ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CreateCertificates _createCertificates;
        private readonly ImportExportCertificate _importExportCertificate;


        public RegisterModel(
            UserManager<EggBasketUser> userManager,
            SignInManager<EggBasketUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            EggBasket.Data.ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            CreateCertificates createCertificates,
            ImportExportCertificate importExportCertificate
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _createCertificates = createCertificates;
            _importExportCertificate = importExportCertificate;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [RegularExpression("^[^<>@#$%&*=]*$", ErrorMessage = "The password contains an invalid character")]  //password validation
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Choose role")]
            public string UserRole { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Choose Company")]
            public string CompanyName { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Role (if not found above)")]
            public string OtherRole { get; set; }

        }

        public List<SelectListItem> Options { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {
           

            Options = _context.Roles.Select(a =>
                                 new SelectListItem
                                 {
                                     Value = a.Name,
                                     Text = a.Name
                                 }).OrderBy(a => a.Value).ToList();
           

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var identityRsaCert3072 = CreateRsaCertificates.CreateRsaCertificate(_createCertificates, 3072);
                var publicKeyPem = _importExportCertificate.PemExportPublicKeyCertificate(identityRsaCert3072);
                var privateKeyPem = _importExportCertificate.PemExportRsaPrivateKey(identityRsaCert3072);
                var user = new EggBasketUser { UserName = Input.Email, Email = Input.Email, CompanyName = Input.CompanyName, PemPublicKey = publicKeyPem, PemPrivateKey = privateKeyPem, FirstName = Input.FirstName, LastName = Input.LastName };
                var result = await _userManager.CreateAsync(user, Input.Password);
                

                if (result.Succeeded)
                {
                    if (Input.UserRole.ToString().Contains("Other"))
                    {
                        if (!await _roleManager.RoleExistsAsync(Input.OtherRole))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(Input.OtherRole));
                            await _userManager.AddToRoleAsync(user, Input.OtherRole);
                        }
                    } else
                    {
                        await _userManager.AddToRoleAsync(user, Input.UserRole);
                    }
                
                    

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
