using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class Posgraduacao
    {
        [Key]
        public ulong id { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório!")]
        [StringLength(255)]
        public string nome { get; set; }

        [Required(ErrorMessage = "Nome do curso é obrigatório!")]
        [StringLength(255)]
        public string nome_curso { get; set; }

        [Required(ErrorMessage = "Campus do curso é obrigatório!")]
        [StringLength(100)]
        public string campus_curso { get; set; }

        public string? descricao { get; set; }

        public string? edital { get; set; }

        public DateTime DataCadastro { get; set; }

        
        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataDesativacao { get; set; }

        public ICollection<Posgraduacao_Professor>? Posgraduacao_Professors { get; set; } = null!;
    }
}
