using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Models
{
    public sealed class Autor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Sobrenome { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataHoraCriacao { get; set; } = DateTime.Now;
        public DateTime? DataHoraAlteracao { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
