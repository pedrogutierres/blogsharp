using AutoMapper;
using Blog.Api.ViewModels.Autores;
using Blog.Api.ViewModels.Comentarios;
using Blog.Api.ViewModels.Posts;
using Blog.Data.Models;

namespace Blog.Api.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static WebApplicationBuilder AddAutoMapperSetup(this WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));

            return builder;
        }
    }

    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Post, PostViewModel>();
            CreateMap<Autor, PostAutorViewModel>();
            CreateMap<Comentario, PostComentarioViewModel>();
            CreateMap<Autor, AutorViewModel>();
            CreateMap<Comentario, ComentarioViewModel>();
        }
    }

    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<PublicarPostViewModel, Post>();
            CreateMap<EditarPostViewModel, Post>();
        }
    }
}
