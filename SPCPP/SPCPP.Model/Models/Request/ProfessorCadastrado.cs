﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models.Request
{
    public class ProfessorCadastrado
    {
        
        public ulong user_id { get; set; }
        public string siape { get; set; }

        public string Cnome { get; set; }

        public string Email { get; set; }
        public ulong posgraduacao_id { get; set; }
        public DateTime Data_nasc { get; set; }
        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }
        public double nota { get; set; }
        public string status { get; set; }

        public double A1 { get; set; }
        public double A2 { get; set; }
        public double A3 { get; set; }
        public double A4 { get; set; }
        public double DP { get; set; }
        public double PC { get; set; }
        public double PQ { get; set; }
        public double indiceH { get; set; }


    }
}
