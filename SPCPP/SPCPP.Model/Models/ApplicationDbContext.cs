using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();


                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                

                //optionsBuilder.UseMySql("server=localhost; database=spcpp; Uid=root; Pwd=flamengo97", ServerVersion.Parse("8.0.25-mysql"));


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
                entity.HasIndex(e => e.posgraduacao_id, "posgraduacao_professor_FK_2");


                entity.ToTable("posgraduacao_professor");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("id");

                entity.Property(e => e.professor_id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("professor_id");

                entity.Property(e => e.posgraduacao_id)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("posgraduacao_id");

                entity.Property(e => e.nota)
                    .HasColumnType("double")
                    .HasColumnName("nota");

                entity.Property(e => e.DataCadastro).HasColumnType("datetime")
                    .HasColumnName("DataCadastro");

                entity.Property(e => e.DataAtualizacao).HasColumnType("datetime")
                   .HasColumnName("DataAtualizacao");

                entity.Property(e => e.status)
                  .HasMaxLength(45)
                  .HasColumnName("status");

                entity.Property(e => e.A1)
                    .HasColumnType("double")
                    .HasColumnName("a1");
                
                entity.Property(e => e.A2)
                    .HasColumnType("double")
                    .HasColumnName("a2");

                entity.Property(e => e.A3)
                    .HasColumnType("double")
                    .HasColumnName("a3");

                entity.Property(e => e.A4)
                    .HasColumnType("double")
                    .HasColumnName("a4");

                entity.Property(e => e.DP)
                    .HasColumnType("double")
                    .HasColumnName("dp");

                entity.Property(e => e.PC)
                    .HasColumnType("double")
                    .HasColumnName("pc");

                entity.Property(e => e.PQ)
                    .HasColumnType("double")
                    .HasColumnName("pq");

                entity.Property(e => e.indiceH)
                    .HasColumnType("double")
                    .HasColumnName("indiceh");

                entity.HasOne(d => d.Professors)
                    .WithMany(p => p.Posgraduacao_Professors)
                    .HasForeignKey(d => d.professor_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("posgraduacao_professor_FK_1");

                entity.HasOne(d => d.Posgraduacaos)
                    .WithMany(p => p.Posgraduacao_Professors)
                    .HasForeignKey(d => d.posgraduacao_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("posgraduacao_professor_FK_2");

            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
