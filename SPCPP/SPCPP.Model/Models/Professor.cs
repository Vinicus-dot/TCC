using javax.annotation.processing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class Professor
    {
        

        [Key]
        [StringLength(8)]
        [Required]
        public string siape { get; set; } = null!;

        [ForeignKey("user_id")]
        public ulong user_id { get; set; }

        [Required(ErrorMessage = "CNome é obrigatório!")]
        [StringLength(255)]
        public string Cnome { get; set; } = null!;

        [Required(ErrorMessage = "Digite o e-mail do usuário")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é valido!")]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string Lotacao { get; set; } = null!;

        [Required(ErrorMessage = "Data Nascimento é obrigatório!")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data" + " Nascimento")]
        public DateTime Data_nasc { get; set; }


        
        public DateTime? Data_exoneracao { get; set; }

        
        public DateTime? Data_saida { get; set; }

        
        public DateTime? Data_aposentadoria { get; set; }


        public int Carga_atual { get; set; }

        [StringLength(255)]
        public string Status { get; set; } 


        public User Users { get; set; } = null!;
        
        public ICollection<Posgraduacao_Professor>? Posgraduacao_Professors { get; set; } = null!;


    }
}
