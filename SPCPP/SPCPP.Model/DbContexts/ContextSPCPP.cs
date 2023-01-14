using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.DbContexts
{
    public class ContextSPCPP : ContextBase
    {
        public ContextSPCPP()
        {
            this.ConnectionStringDapper = Environment.GetEnvironmentVariable("DefaultConnection2");
        }

    }
}
