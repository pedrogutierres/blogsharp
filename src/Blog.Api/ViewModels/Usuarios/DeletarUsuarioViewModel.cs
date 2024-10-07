using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para deletar um usuário
    /// </summary>
    public class DeletarUsuarioViewModel
    {
        /// <summary>
        /// Senha atual do usuário
        /// </summary>
        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string Senha { get; set; }
    }
}
