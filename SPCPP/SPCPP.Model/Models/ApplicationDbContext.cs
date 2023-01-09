﻿using com.sun.xml.@internal.bind.v2.model.core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Professor> Professores { get; set; } = null!;

        public virtual DbSet<Posgraduacao> Posgraduacao { get; set; } = null!;
        public virtual DbSet<Posgraduacao_Professor> Posgraducao_professor { get; set; } = null!;

        public virtual DbSet<User> User { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                

                optionsBuilder.UseMySql("server=localhost; database=spcpp; Uid=root; Pwd=flamengo97", ServerVersion.Parse("8.0.25-mysql"));


            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("usuario");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Nome)
                    .HasMaxLength(150)
                    .HasColumnName("Nome");

                entity.Property(e => e.Login)
                    .HasMaxLength(25)
                    .HasColumnName("Login");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnName("Email");

                entity.Property(e => e.Perfil)
                    .HasColumnType("int")
                    .HasColumnName("Perfil");

                entity.Property(e => e.Senha)
                    .HasMaxLength(200)
                    .HasColumnName("Senha");

                entity.Property(e => e.DataCadastro).HasColumnType("datetime")                   
                    .HasColumnName("DataCadastro");

                entity.Property(e => e.DataAtualizacao).HasColumnType("datetime")                  
                    .HasColumnName("DataAtualizacao");


            });


            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => new {e.user_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("professor");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.user_id, "professor_FK_1");
                entity.HasIndex(e => e.siape, "siape_FK_2");

                entity.Property(e => e.siape)
                   .HasMaxLength(8)
                   .HasColumnName("siape").IsRequired();

                entity.Property(e => e.user_id)
                    .HasColumnType("bigint(11)")
                   .HasColumnName("user_id");

                entity.Property(e => e.Cnome)
                    .HasMaxLength(150)
                    .HasColumnName("Cnome");


                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnName("Email");

                entity.Property(e => e.Lotacao)
                    .HasMaxLength(150)
                    .HasColumnName("Lotacao");

                entity.Property(e => e.Data_nasc).HasColumnType("datetime")                   
                    .HasColumnName("Data_nasc");

                entity.Property(e => e.Data_exoneracao).HasColumnType("datetime")
                    .HasColumnName("Data_exoneracao");

                entity.Property(e => e.Data_saida)
                    .HasColumnType("datetime")
                    .HasColumnName("Data_saida");

                entity.Property(e => e.Data_aposentadoria)
                    .HasColumnType("datetime")
                   .HasColumnName("Data_aposentadoria");

                entity.Property(e => e.Carga_atual)
                   .HasColumnType("int")
                  .HasColumnName("Carga_atual");

                entity.Property(e => e.Status)
                  .HasMaxLength(150)
                  .HasColumnName("Status");

                entity.Property(e => e.Afastado)
                  .HasColumnType("tinyint(1)")
                  .HasColumnName("Afastado");

                entity.Property(e => e.Administrativo)
                  .HasColumnType("tinyint(1)")
                  .HasColumnName("Administrativo");

                entity.Property(e => e.Avaliador)
                  .HasColumnType("tinyint(1)")
                  .HasColumnName("Avaliador");


                entity.HasOne(d => d.Users)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.user_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_professor_FK_1");
               
            });


            modelBuilder.Entity<Posgraduacao>(entity =>
            {                     
                entity.ToTable("posgraduacao");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("id");

                entity.Property(e => e.nome)
                    .HasMaxLength(150)
                    .HasColumnName("nome");

                entity.Property(e => e.nome_curso)
                    .HasMaxLength(150)
                    .HasColumnName("nome_curso");

                entity.Property(e => e.campus_curso)
                     .HasMaxLength(150)
                    .HasColumnName("campus_curso");

                entity.Property(e => e.descricao)
                     .HasMaxLength(255)
                    .HasColumnName("descricao");

                entity.Property(e => e.edital)
                     .HasMaxLength(150)
                    .HasColumnName("edital");

                entity.Property(e => e.DataCadastro).HasColumnType("datetime")
                    .HasColumnName("DataCadastro");

                entity.Property(e => e.DataAtualizacao).HasColumnType("datetime")
                    .HasColumnName("DataAtualizacao");

                entity.Property(e => e.DataDesativacao).HasColumnType("datetime")
                .HasColumnName("DataDesativacao");


            });

            modelBuilder.Entity<Posgraduacao_Professor>(entity =>
            {

                entity.HasIndex(e => e.professor_id, "posgraduacao_professor_FK_1");
                entity.HasIndex(e => e.posgraducao_id, "posgraduacao_professor_FK_2");


                entity.ToTable("posgraduacao_professor");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("id");

                entity.Property(e => e.professor_id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("professor_id");

                entity.Property(e => e.posgraducao_id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("posgraducao_id");

                entity.Property(e => e.DataCadastro).HasColumnType("datetime")
                    .HasColumnName("DataCadastro");

                entity.Property(e => e.DataAtualizacao).HasColumnType("datetime")
                   .HasColumnName("DataAtualizacao");



                entity.HasOne(d => d.Professors)
                    .WithMany(p => p.Posgraduacao_Professors)
                    .HasForeignKey(d => d.professor_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("posgraduacao_professor_FK_1");

                entity.HasOne(d => d.Posgraduacaos)
                    .WithMany(p => p.Posgraduacao_Professors)
                    .HasForeignKey(d => d.posgraducao_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("posgraduacao_professor_FK_2");

            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}