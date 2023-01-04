using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "Digite o login")]

        public string Login { get; set; }

        [Required(ErrorMessage = "Digite o e-mail")]

        public string Email { get; set; }
    }
}
