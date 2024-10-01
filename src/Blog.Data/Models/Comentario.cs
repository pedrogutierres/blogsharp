using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Data.Models
{
    [Table("Comentarios")]
    public sealed class Comentario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O conteúdo do comentário deve ser informado.")]
        [StringLength(500, ErrorMessage = "O comentário deve conter no máximo {1} caracteres.")]
        public string Conteudo { get; set; }

        public bool Excluido { get; set; } = false;
        public DateTime? DataHoraExclusao { get; set; }

        public DateTime DataHoraCriacao { get; set; } = DateTime.Now;
        public DateTime? DataHoraAlteracao { get; set; }

        [Required]
        [ForeignKey("Post")]
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        [Required]
        [ForeignKey("Autor")]
        public Guid AutorId { get; set; }
        public Autor Autor { get; set; }
    }
}
