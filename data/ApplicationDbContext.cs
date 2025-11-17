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
            // Configurar nombres exactos de las tablas en la base de datos
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Estado>().ToTable("Estados");
            modelBuilder.Entity<Tarea>().ToTable("Tareas");

            base.OnModelCreating(modelBuilder);
        }
    }
}
