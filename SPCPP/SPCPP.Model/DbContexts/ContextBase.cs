using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.DbContexts
{
    public class ContextBase
    {
        public string ConnectionStringDapper { get; set; }
        public MySqlTransaction Transaction { get; set; }
        public MySqlConnection Connection { get; set; }
        private string DbName { get; set; }

        public ContextBase() { }

        public ContextBase(string connectionString)
        {
            ConnectionStringDapper = connectionString;

        }

        public void GetConnection()
        {
            Connection = new MySqlConnection(ConnectionStringDapper);
            if (Connection != null)
                DbName = Connection.Database;
        }

        public void GetConnectionTransaction()
        {
            Connection = new MySqlConnection(ConnectionStringDapper);
            DbName = Connection.Database;
            Open();
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();

            Close();
        }

        public void Rollback()
        {
            if (Transaction != null)
                Transaction.Rollback();

            Close();
        }

        public void Open()
        {
            if (Connection != null)
                Connection.Open();
        }

        public void Close()
        {
            if (Connection != null)
                Connection.Close();
        }

        public string GetNameDB()
        {
            return DbName;
        }
    }

}
