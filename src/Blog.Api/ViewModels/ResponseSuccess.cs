namespace Blog.Api.ViewModels
{
    public class ResponseSuccess
    {
        /// <summary>
        /// ID do dado que foi salvo, alterado, consultado, excluido ou etc
        /// </summary>
        public Guid? Id { get; set; }

        public ResponseSuccess() { }
        public ResponseSuccess(Guid id) => Id = id;
    }
}
