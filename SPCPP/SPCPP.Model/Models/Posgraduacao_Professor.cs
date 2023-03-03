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

        public ulong posgraduacao_id { get; set; }

        public double nota { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public string? status { get; set; }
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double A3 { get; set; }
        public double A4 { get; set; }
        public double DP { get; set; }
        public double PC { get; set; }
        public double PQ { get; set; }
        public double indiceH { get; set; }


        public Professor Professors { get; set; } = null!;
        public Posgraduacao Posgraduacaos { get; set; } = null!;
    }
}
