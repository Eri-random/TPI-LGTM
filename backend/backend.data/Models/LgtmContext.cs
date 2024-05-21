﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend.data.Models;

public partial class LgtmContext : DbContext
{
    public LgtmContext()
    {
    }

    public LgtmContext(DbContextOptions<LgtmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Donacion> Donacions { get; set; }

    public virtual DbSet<Idea> Ideas { get; set; }

    public virtual DbSet<InfoOrganizacion> InfoOrganizacions { get; set; }

    public virtual DbSet<Organizacion> Organizacions { get; set; }

    public virtual DbSet<Paso> Pasos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=lgtm;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Donacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("donacion_pk");

            entity.ToTable("donacion");

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

            entity.HasIndex(e => e.UsuarioId, "IX_idea_usuario_id");

            entity.Property(e => e.Id).HasColumnName("id");
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

            entity.HasIndex(e => e.OrganizacionId, "organizacion_id").IsUnique();

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

        modelBuilder.Entity<Organizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organizacion_pkey");

            entity.ToTable("organizacion");

            entity.HasIndex(e => e.UsuarioId, "usuario_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cuit)
                .IsRequired()
                .HasColumnName("cuit");
            entity.Property(e => e.Direccion)
                .IsRequired()
                .HasColumnName("direccion");
            entity.Property(e => e.Localidad)
                .IsRequired()
                .HasColumnName("localidad");
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
        });

        modelBuilder.Entity<Paso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("paso_pk");

            entity.ToTable("paso");

            entity.HasIndex(e => e.IdeaId, "IX_paso_idea_id");

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

            entity.HasIndex(e => e.Nombre, "rol_nombre_key").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.RolId, "IX_usuario_rol_id");

            entity.HasIndex(e => e.Email, "usuario_email_key").IsUnique();

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
