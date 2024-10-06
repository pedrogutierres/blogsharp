﻿namespace Blog.Api.ViewModels.Comentarios
{
    public class ComentarioViewModel
    {
        public Guid Id { get; set; }
        public string Conteudo { get; set; }
        public bool Excluido { get; set; }
        public DateTime? DataHoraExclusao { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
        public Guid PostId { get; set; }
        public Guid AutorId { get; set; }
    }
}