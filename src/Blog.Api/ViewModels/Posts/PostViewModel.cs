namespace Blog.Api.ViewModels.Posts
{
    /// <summary>
    /// Dados do post
    /// </summary>
    public class PostViewModel
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título do post
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Conteúdo do post
        /// </summary>
        public string Conteudo { get; set; }

        /// <summary>
        /// Informa se o posto foi excluído
        /// </summary>
        public bool Excluido { get; set; }

        /// <summary>
        /// Data e hora da publicação do post
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }

        /// <summary>
        /// Data e hora da última alteração do post
        /// </summary>
        public DateTime? DataHoraAlteracao { get; set; }

        /// <summary>
        /// Autor do post
        /// </summary>
        public PostAutorViewModel Autor { get; set; }

        /// <summary>
        /// Comentários do post
        /// </summary>
        public IEnumerable<PostComentarioViewModel> Comentarios { get; set; }
    }

    /// <summary>
    /// Dados do Autor do post
    /// </summary>
    public class PostAutorViewModel
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

    /// <summary>
    /// Dados do Comentário do post
    /// </summary>
    public class PostComentarioViewModel
    {
        /// <summary>
        /// Identificador único do Comentário
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Conteúdo do comentário
        /// </summary>
        public string Conteudo { get; set; }

        /// <summary>
        /// Data e hora da publicção do comentário
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }
    }
}
