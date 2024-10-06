using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    public class RegistrarUsuarioViewModel
    {
        [Required(ErrorMessage = "O nome deve ser informado.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome deve ser informado.")]
        public string Sobrenome { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O {0} deve ser informado")]
        [EmailAddress(ErrorMessage = "O {0} está inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmação de senha")]
        public string ConfirmacaoDeSenha { get; set; }
    }
}
