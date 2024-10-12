using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public sealed class ComentarioMapping : IEntityTypeConfiguration<Comentario>
    {
        public void Configure(EntityTypeBuilder<Comentario> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Conteudo)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Excluido).IsRequired();
            builder.Property(p => p.DataHoraCriacao).IsRequired();

            builder.HasOne(p => p.Autor)
                .WithMany(a => a.Comentarios)
                .HasForeignKey(p => p.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Comentarios");
        }
    }
}
