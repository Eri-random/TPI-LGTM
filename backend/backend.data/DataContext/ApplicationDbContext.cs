using backend.data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.data.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("rol");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");
                entity.Property(e => e.RolId)
                    .HasColumnName("rol_id");
                entity.Property(e => e.RolId);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.Property(e => e.Id)
                    .HasColumnName("id");
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");
                entity.Property(e => e.RolId)
                    .HasColumnName("rol_id");
                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono");
                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion");
                entity.Property(e => e.Localidad)
                    .HasColumnName("localidad");
                entity.Property(e => e.Provincia)
                    .HasColumnName("provincia");
                entity.Property(e => e.Apellido)
                    .HasColumnName("apellido");
                entity.Property(e => e.Email)
                    .HasColumnName("email");
                entity.Property(e => e.Contrasena)
                    .HasColumnName("contrasena");
                entity.Property(e => e.Cuit)
                    .HasColumnName("cuit");
            });
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);
        }
    }
}
