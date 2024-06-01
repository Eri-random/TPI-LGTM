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

        public virtual DbSet<Donacion> Donacions { get; set; }

        public virtual DbSet<Idea> Ideas { get; set; }

        public virtual DbSet<InfoOrganizacion> InfoOrganizacions { get; set; }

        public virtual DbSet<Necesidad> Necesidads { get; set; }

        public virtual DbSet<Organizacion> Organizacions { get; set; }

        public virtual DbSet<Paso> Pasos { get; set; }

        public virtual DbSet<Rol> Rols { get; set; }

        public virtual DbSet<Sede> Sedes { get; set; }

        public virtual DbSet<Subcategorium> Subcategoria { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Donacion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("donacion_pk");

                entity.ToTable("donacion");

                entity.HasIndex(e => e.OrganizacionId, "IX_donacion_organizacion_id");

                entity.HasIndex(e => e.UsuarioId, "IX_donacion_usuario_id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
                entity.Property(e => e.Producto)
                    .IsRequired()
                    .HasColumnName("producto");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

                entity.HasOne(d => d.Organizacion).WithMany(p => p.Donacions)
                    .HasForeignKey(d => d.OrganizacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("donacion_organizacion_fk");

                entity.HasOne(d => d.Usuario).WithMany(p => p.Donacions)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("donacion_usuario_fk");
            });

            modelBuilder.Entity<Idea>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("idea_pkey");

                entity.ToTable("idea");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Dificultad).HasColumnName("dificultad");
                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasColumnName("titulo");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

                entity.HasOne(d => d.Usuario).WithMany(p => p.Ideas)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idea_usuario_fk");
            });

            modelBuilder.Entity<InfoOrganizacion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("id");

                entity.ToTable("info_organizacion");

                entity.HasIndex(e => e.OrganizacionId, "organizacion_id");

                entity.HasIndex(e => e.OrganizacionId, "organizacion_info").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DescripcionBreve)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("descripcion_breve");
                entity.Property(e => e.DescripcionCompleta)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasColumnName("descripcion_completa");
                entity.Property(e => e.Img)
                    .IsRequired()
                    .HasColumnName("img");
                entity.Property(e => e.Organizacion)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("organizacion");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");

                entity.HasOne(d => d.OrganizacionNavigation).WithOne(p => p.InfoOrganizacion)
                    .HasForeignKey<InfoOrganizacion>(d => d.OrganizacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("organizacion_fkey");
            });

            modelBuilder.Entity<Necesidad>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("necesidad_pkey");

                entity.ToTable("necesidad");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Icono)
                    .IsRequired()
                    .HasColumnName("icono");
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Organizacion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("organizacion_pkey");

                entity.ToTable("organizacion");

                entity.HasIndex(e => e.UsuarioId, "usuario_id").IsUnique();

                entity.HasIndex(e => e.UsuarioId, "usuario_id_key");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Cuit)
                    .IsRequired()
                    .HasColumnName("cuit");
                entity.Property(e => e.Direccion)
                    .IsRequired()
                    .HasColumnName("direccion");
                entity.Property(e => e.Latitud).HasColumnName("latitud");
                entity.Property(e => e.Localidad)
                    .IsRequired()
                    .HasColumnName("localidad");
                entity.Property(e => e.Longitud).HasColumnName("longitud");
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");
                entity.Property(e => e.Provincia)
                    .IsRequired()
                    .HasColumnName("provincia");
                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasColumnName("telefono");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

                entity.HasOne(d => d.Usuario).WithOne(p => p.Organizacion)
                    .HasForeignKey<Organizacion>(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usuario_fkey");

                entity.HasMany(d => d.Subcategoria).WithMany(p => p.Organizacions)
                    .UsingEntity<Dictionary<string, object>>(
                        "OrganizacionNecesidad",
                        r => r.HasOne<Subcategorium>().WithMany()
                            .HasForeignKey("SubcategoriaId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("organizacion_subcategoria_fk"),
                        l => l.HasOne<Organizacion>().WithMany()
                            .HasForeignKey("OrganizacionId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("organizacion_necesidad_fk"),
                        j =>
                        {
                            j.HasKey("OrganizacionId", "SubcategoriaId").HasName("organizacion_necesidad_pk");
                            j.ToTable("organizacion_necesidad");
                            j.IndexerProperty<int>("OrganizacionId").HasColumnName("organizacion_id");
                            j.IndexerProperty<int>("SubcategoriaId").HasColumnName("subcategoria_id");
                        });
            });

            modelBuilder.Entity<Paso>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("paso_pk");

                entity.ToTable("paso");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion");
                entity.Property(e => e.IdeaId).HasColumnName("idea_id");
                entity.Property(e => e.PasoNum).HasColumnName("paso_num");

                entity.HasOne(d => d.Idea).WithMany(p => p.Pasos)
                    .HasForeignKey(d => d.IdeaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("paso_idea_fk");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.RolId).HasName("rol_pkey");

                entity.ToTable("rol");

                entity.Property(e => e.RolId).HasColumnName("rol_id");
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Sede>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("sede_pkey");

                entity.ToTable("sede");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Direccion)
                    .IsRequired()
                    .HasColumnName("direccion");
                entity.Property(e => e.Latitud).HasColumnName("latitud");
                entity.Property(e => e.Localidad)
                    .IsRequired()
                    .HasColumnName("localidad");
                entity.Property(e => e.Longitud).HasColumnName("longitud");
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
                entity.Property(e => e.Provincia)
                    .IsRequired()
                    .HasColumnName("provincia");
                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasColumnName("telefono");

                entity.HasOne(d => d.Organizacion).WithMany(p => p.Sedes)
                    .HasForeignKey(d => d.OrganizacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("organizacion_fkey");
            });

            modelBuilder.Entity<Subcategorium>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("subcategorias_pkey");

                entity.ToTable("subcategoria");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.NecesidadId).HasColumnName("necesidad_id");
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");

                entity.HasOne(d => d.Necesidad).WithMany(p => p.Subcategoria)
                    .HasForeignKey(d => d.NecesidadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subcategoria_necesidad_fk");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
<<<<<<< HEAD
                entity.HasKey(e => e.Id).HasName("usuario_pkey");

                entity.ToTable("usuario");

                entity.HasIndex(e => e.RolId, "IX_usuario_rol_id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Apellido)
                    .HasMaxLength(100)
                    .HasColumnName("apellido");
                entity.Property(e => e.Contrasena)
                    .HasMaxLength(255)
                    .HasColumnName("contrasena");
                entity.Property(e => e.Direccion).HasColumnName("direccion");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Localidad)
                    .HasMaxLength(100)
                    .HasColumnName("localidad");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
                entity.Property(e => e.Provincia)
                    .HasMaxLength(100)
                    .HasColumnName("provincia");
                entity.Property(e => e.RolId).HasColumnName("rol_id");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .HasColumnName("telefono");

                entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.RolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usuario_rol_id_fkey");
            });
=======
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
            });
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);
>>>>>>> origin/main
        }
    }
}
