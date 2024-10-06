namespace Blog.Api.ViewModels.Comentarios
{
    /// <summary>
    /// Dados do comentário
    /// </summary>
    public class ComentarioViewModel
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Conteúdo do comentário
        /// </summary>
        public string Conteudo { get; set; }

        /// <summary>
        /// Informa se o comentário foi excluído
        /// </summary>
        public bool Excluido { get; set; }

        /// <summary>
        /// Data e hora de quando o comentário foi excluído (caso tenha sido)
        /// </summary>
        public DateTime? DataHoraExclusao { get; set; }

        /// <summary>
        /// Data e hora da publicação do comentário
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }

        /// <summary>
        /// Data e hora da última alteração do comentário
        /// </summary>
        public DateTime? DataHoraAlteracao { get; set; }

        /// <summary>
        /// Identificador do post ao qual o comentário pertence
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Autor do comentário
        /// </summary>
        public ComentarioAutorViewModel Autor { get; set; }
    }

    /// <summary>
    /// Dados do autor do comentário
    /// </summary>
    public class ComentarioAutorViewModel
    {
        /// <summary>
        /// Identificador único do Autor
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do autor
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome do autor
        /// </summary>
        public string Sobrenome { get; set; }
    }
}
