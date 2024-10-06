using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para registrar um usuário
    /// </summary>
    public class RegistrarUsuarioViewModel
    {
        /// <summary>
        /// Nome do usuário (autor)
        /// </summary>
        [Required(ErrorMessage = "O nome deve ser informado.")]
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome do usuário (autor)
        /// </summary>
        [Required(ErrorMessage = "O sobrenome deve ser informado.")]
        public string Sobrenome { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O {0} deve ser informado")]
        [EmailAddress(ErrorMessage = "O {0} está inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string Senha { get; set; }

        /// <summary>
        /// Cofirmação de senha
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmação de senha")]
        public string ConfirmacaoDeSenha { get; set; }
    }
}
