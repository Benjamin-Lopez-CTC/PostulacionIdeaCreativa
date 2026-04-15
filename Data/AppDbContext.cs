using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Models;

namespace IdeasCreativasApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Idea> Ideas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Relación uno a muchos: Equipo tiene muchos Alumnos (hasta 2)
            modelBuilder.Entity<Equipo>()
                .HasMany(e => e.Integrantes)
                .WithOne(a => a.Equipo)
                .HasForeignKey(a => a.EquipoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación uno a muchos: Equipo tiene muchas Ideas
            modelBuilder.Entity<Equipo>()
                .HasMany(e => e.IdeasPostuladas)
                .WithOne(i => i.Equipo)
                .HasForeignKey(i => i.EquipoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
