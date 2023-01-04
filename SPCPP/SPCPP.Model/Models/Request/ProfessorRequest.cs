using SPCPP.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models.Request
{
    public class ProfessorRequest
    {

        [Required(ErrorMessage = "Nome é obrigatório!")]
        [StringLength(255)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Digite o login do usuário")]
        [StringLength(8)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Digite o e-mail do usuário")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é valido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o perfil do usuário")]
        public PerfilEnum Perfil { get; set; }
        [Required(ErrorMessage = "Senha é obrigatório!")]
        [StringLength(20)]
        public string Senha { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataCadastro { get; set; }

 

       

        [Required(ErrorMessage = "CNome é obrigatório!")]
        [StringLength(255)]
        public string Cnome { get; set; } = null!;     

        [StringLength(255)]
        public string Lotacao { get; set; } = null!;

        [Required(ErrorMessage = "Data Nascimento é obrigatório!")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data" + " Nascimento")]
        public DateTime Data_nasc { get; set; }


        public int Carga_atual { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        public bool Afastado { get; set; }

        public bool Administrativo { get; set; }

        public bool Avaliador { get; set; }

    }
}
