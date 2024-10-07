using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para alterar a senha de um usuário
    /// </summary>
    public class AlterarSenhaDoUsuarioViewModel
    {
        /// <summary>
        /// Senha atual do usuário
        /// </summary>
        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string SenhaAtual { get; set; }

        /// <summary>
        /// Nova Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string NovaSenha { get; set; }

        /// <summary>
        /// Cofirmação da nova senha
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As novas senhas não conferem.")]
        [Display(Name = "Confirmação da nova senha")]
        public string ConfirmacaoDeNovaSenha { get; set; }
    }
}
