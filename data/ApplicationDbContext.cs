using Microsoft.EntityFrameworkCore;
using TodoApp.API.Models;

namespace TodoApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                modelBuilder.HasDefaultSchema("todoapp");
            }

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Estado>().ToTable("Estados");
            modelBuilder.Entity<Tarea>().ToTable("Tareas");

            base.OnModelCreating(modelBuilder);
        }
    }
}
