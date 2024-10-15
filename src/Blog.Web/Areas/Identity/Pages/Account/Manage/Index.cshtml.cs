// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Blog.Application.Identity;
using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Display(Name = "E-mail")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome deve ser informado.")]
            [Display(Name = "Nome")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "O sobrenome deve ser informado.")]
            [Display(Name = "Sobrenome")]
            public string LastName { get; set; }

            [Phone(ErrorMessage = "O telefone está inválido.")]
            [Display(Name = "Telefone")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            var autor = await _context.Autores.FindAsync(Guid.Parse(user.Id));
            
            Input = new InputModel
            {
                FirstName = autor?.Nome,
                LastName = autor?.Sobrenome,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário pelo ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário pelo ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Não foi possível alterar o telefone.";
                    return RedirectToPage();
                }
            }

            var autor = await _context.Autores.FindAsync(Guid.Parse(user.Id));
            if (autor == null)
            {
                StatusMessage = "Autor vinculado ao usuário não foi encontrado.";
                return RedirectToPage();
            }

            autor.Nome = Input.FirstName?.Trim();
            autor.Sobrenome = Input.LastName?.Trim();

            await _context.SaveChangesAsync();

            //await _signInManager.RefreshSignInAsync(user);
            var customClaims = await _userManager.GetCustomClaimsAsync(user, autor.Nome, autor.Sobrenome);
            await _signInManager.SignInWithClaimsAsync(user, false, customClaims);

            StatusMessage = "Seus dados foram atualizados";

            return RedirectToPage();
        }
    }
}
