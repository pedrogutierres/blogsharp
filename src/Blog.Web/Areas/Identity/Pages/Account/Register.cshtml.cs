// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Blog.Application.Exceptions;
using Blog.Application.Identity;
using Blog.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AutenticacaoService _autenticacaoService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            AutenticacaoService autenticacaoService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _autenticacaoService = autenticacaoService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome deve ser informado.")]
            [Display(Name = "Nome")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "O sobrenome deve ser informado.")]
            [Display(Name = "Sobrenome")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "O e-mail deve ser informado.")]
            [EmailAddress(ErrorMessage = "O e-mail informado está inválido.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha deve ser informada.")]
            [StringLength(100, ErrorMessage = "A senha deve conter no entre {2} e {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "As senhas não conferem.")]
            [Display(Name = "Confirme a senha")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _autenticacaoService.RegistrarUsuarioAsync(Input.Email, Input.Password, Input.FirstName, Input.LastName);
                    if (user != null)
                    {
                        var customClaims = await _userManager.GetCustomClaimsAsync(user, Input.FirstName, Input.LastName);

                        await _signInManager.SignInWithClaimsAsync(user, false, customClaims);

                        _logger.LogInformation("Claims customizadas atualizadas.");

                        return LocalRedirect(returnUrl);
                    }
                }
                catch (BusinessException ex)
                {
                    foreach (var error in ex.Data.Values)
                        ModelState.AddModelError(string.Empty, error.ToString());
                }
                catch
                {
                    throw;
                }
            }

            return Page();
        }
    }
}
