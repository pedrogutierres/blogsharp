using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    public class LoginViewModel
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O {0} deve ser informado")]
        [EmailAddress(ErrorMessage = "O {0} está inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string Senha { get; set; }
    }
}
