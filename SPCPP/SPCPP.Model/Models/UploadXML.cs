using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class UploadXML
    {
        public string nome { get; set; }
        public bool pq { get; set; }
        public ulong posgraduacao_id { get; set; }
        public List<Artigo> artigolist { get; set; }
        public List<Patente> artigopatentepc { get; set; }
        public List<Patente> artigopatentedp { get; set; }

    }

   

    public class Artigo
    {
        public string issn { get; set; }

        public string titulo { get; set; }     

        public string ano { get; set; }

        public List<Autor> coautoresArtigo { get; set; }
    }

    public class Patente
    { 
        public string codigo_registro_patente { get; set; }      
        public string titulo { get; set; }
        public string ano { get; set; }
        public string pais { get; set; }

        public List<Autor> coautoresPatente { get; set; }

    }

    public class Autor
    {
        public string nome { get; set; }

        public string ordem_altoria { get; set; }
    }
}
