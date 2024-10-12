using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public sealed class PostMapping : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Conteudo).IsRequired();
            builder.Property(p => p.Excluido).IsRequired();
            builder.Property(p => p.DataHoraCriacao).IsRequired();

            builder.HasOne(p => p.Autor)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comentarios)
                .WithOne(a => a.Post)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Posts");
        }
    }
}
