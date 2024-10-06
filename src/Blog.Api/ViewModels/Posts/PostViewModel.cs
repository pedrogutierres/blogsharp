namespace Blog.Api.ViewModels.Posts
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
        public Guid AutorId { get; set; }
        public PostAutorViewModel Autor { get; set; }
        public IEnumerable<PostComentarioViewModel> Comentarios { get; set; }
    }

    public class PostAutorViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
    }

    public class PostComentarioViewModel
    {
        public Guid Id { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataHoraCriacao { get; set; }
    }
}
