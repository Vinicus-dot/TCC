﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models.Request
{
    public class ProfessorCadastrado
    {
        public string user_id { get; set; }
        public string siape { get; set; }

        public string Cnome { get; set; }

        public string Email { get; set; }

        public DateTime Data_nasc { get; set; }
        public DateTime DataCadastro { get; set; }
        public double nota { get; set; }
        public int Carga_atual { get; set; }
        public string Status { get; set; }


    }
}