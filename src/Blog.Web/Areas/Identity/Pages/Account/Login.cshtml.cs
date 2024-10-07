// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Blog.Data;
using Blog.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O e-mail deve ser informado.")]
            [EmailAddress(ErrorMessage = "O e-mail informado está inválido.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha deve ser informada.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [Display(Name = "Lembrar?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário logado por senha.");

                    var autor = await _context.Autores.FindAsync(Guid.Parse(HttpContext.User.GetUserId()));
                    if (autor?.Ativo == false)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário inativo.");
                        return Page();
                    }

                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    var customClaims = await _userManager.GetCustomClaimsAsync(user, autor.Nome, autor.Sobrenome);
                    await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, customClaims);
                    
                    _logger.LogInformation("Claims customizadas atualizadas.");
                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("Usuário bloqueado temporariamente.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados do usuário inválidos.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
