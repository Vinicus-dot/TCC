using javax.annotation.processing;
using SPCPP.Model.Enums;
using SPCPP.Model.Helper;
using System.ComponentModel.DataAnnotations;

namespace SPCPP.Model.Models
{
    public class User
    {
        

        public ulong Id { get; set; }
        [Required(ErrorMessage = "Digite o nome do usuário")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Digite o login do usuário")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Digite o e-mail do usuário")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é valido!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Informe o perfil do usuário")]
        public PerfilEnum Perfil { get; set; }
        [Required(ErrorMessage = "Digite a senha do usuário")]
        public string Senha { get; set; } = null!;

        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }

        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }

        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            Senha = novaSenha.GerarHash();
            return novaSenha;
        }

        public ICollection<Professor>? Professors { get; set; } = null!;
       

    }
}
