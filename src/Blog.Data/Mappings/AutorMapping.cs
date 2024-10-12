using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public sealed class AutorMapping : IEntityTypeConfiguration<Autor>
    {
        public void Configure(EntityTypeBuilder<Autor> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Sobrenome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Ativo).IsRequired();

            builder.Property(p => p.DataHoraCriacao).IsRequired();

            builder.HasMany(p => p.Posts)
              .WithOne(a => a.Autor)
              .HasForeignKey(p => p.AutorId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comentarios)
              .WithOne(a => a.Autor)
              .HasForeignKey(p => p.AutorId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Autores");
        }
    }
}
