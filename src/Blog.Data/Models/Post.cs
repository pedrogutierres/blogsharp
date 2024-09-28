using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Data.Models
{
    public sealed class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O título do post deve ser informado.")]
        [StringLength(200, ErrorMessage = "O título do post deve conter no máximo {1} caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Conteudo { get; set; }

        public bool Excluido { get; set; } = false;

        public DateTime DataHoraCriacao{ get; set; } = DateTime.Now;
        public DateTime? DataHoraAlteracao { get; set; }

        [Required]
        [ForeignKey("Autor")]
        public Guid AutorId { get; set; }
        public Autor Autor { get; set; }

        public ICollection<Comentario> Comentarios { get; set; }
    }
}
