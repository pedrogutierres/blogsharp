using Blog.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries.Where(p => p.Entity.GetType().GetProperty("DataHoraCriacao") != null
                                                  || p.Entity.GetType().GetProperty("DataHoraAlteracao") != null))
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "DataHoraCriacao"))
                {
                    var dataHoraCriacao = entry.Property("DataHoraCriacao");

                    if (entry.State == EntityState.Added && (dataHoraCriacao.CurrentValue == null || DateTime.MinValue.Equals(dataHoraCriacao.CurrentValue)))
                        dataHoraCriacao.CurrentValue = DateTime.Now;
                    else if (entry.State == EntityState.Modified)
                        dataHoraCriacao.IsModified = false;
                }

                if (entry.Properties.Any(p => p.Metadata.Name == "DataHoraAlteracao"))
                {
                    var dataHoraAlteracao = entry.Property("DataHoraAlteracao");

                    if (entry.State == EntityState.Modified)
                    {
                        dataHoraAlteracao.CurrentValue = DateTime.Now;
                        dataHoraAlteracao.IsModified = true;
                    }
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
