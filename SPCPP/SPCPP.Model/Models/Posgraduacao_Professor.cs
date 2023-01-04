using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class Posgraduacao_Professor
    {
        [Key]
        public ulong id { get; set; }

        public ulong professor_id { get; set; }

        public ulong posgraducao_id { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }
     

        public Professor Professors { get; set; } = null!;
        public Posgraduacao Posgraduacaos { get; set; } = null!;
    }
}
