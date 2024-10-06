namespace Blog.Api.ViewModels.Posts
{
    public class PublicarPostViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
    }
}
