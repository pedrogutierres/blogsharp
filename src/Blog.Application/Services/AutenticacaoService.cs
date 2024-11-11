using Blog.Application.Exceptions;
using Blog.Data;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Blog.Application.Services
{
    public sealed class AutenticacaoService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<AutenticacaoService> _logger;

        public AutenticacaoService(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<AutenticacaoService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
        }


        public async Task<bool> ValidarLoginUsuarioAsync(string email, string senha)
        {
            var result = await _signInManager.PasswordSignInAsync(email, senha, false, lockoutOnFailure: false);
            return result.Succeeded;
        }
        public async Task<IdentityUser> RegistrarUsuarioAsync(string email, string senha, string nome, string sobrenome)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

            try
            {
                var existeUsuarioCadastrado = await _userManager.Users.AnyAsync();

                var result = await _userManager.CreateAsync(user, senha);

                if (result.Succeeded)
                {
                    if (!existeUsuarioCadastrado)
                    {
                        if (!await _roleManager.RoleExistsAsync("Administrador"))
                        {
                            var role = new IdentityRole();
                            role.Name = "Administrador";
                            await _roleManager.CreateAsync(role);
                        }

                        await _userManager.AddToRoleAsync(user, "Administrador");
                    }

                    if (!await _roleManager.RoleExistsAsync("Autor"))
                    {
                        var role = new IdentityRole();
                        role.Name = "Autor";
                        await _roleManager.CreateAsync(role);
                    }

                    await _userManager.AddToRoleAsync(user, "Autor");

                    var autor = new Autor
                    {
                        Id = Guid.Parse(user.Id),
                        Nome = nome?.Trim(),
                        Sobrenome = sobrenome?.Trim()
                    };

                    _context.Autores.Add(autor);

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Usuário criado com senha.");

                    return user;
                }
                else
                    throw new BusinessException("Erro ao criar usuário", null, result.Errors.ToDictionary(key => key.Code, value => value.Description));
            }
            catch (DbException)
            {
                // caso houver erro na criacao de usuario e autor, excluir o usuario criado caso tenha sido
                try
                {
                    await _userManager.DeleteAsync(user);

                    var autor = await _context.Autores.FindAsync(Guid.Parse(user.Id));
                    if (autor != null)
                    {
                        _context.Autores.Remove(autor);
                        await _context.SaveChangesAsync();
                    }
                }
                catch { }

                throw;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> InativarUsuarioAsync(Guid id)
        {
            var autor = await _context.Autores.FindAsync(id);
            autor.Ativo = false;

            var posts = await _context.Posts.Where(p => p.AutorId == id).ToListAsync();
            foreach (var post in posts)
                post.Excluido = true;

            return await _context.SaveChangesAsync() > 0;
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
