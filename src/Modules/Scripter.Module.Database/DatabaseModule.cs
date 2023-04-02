using System.Data.SqlClient;
using doob.Scripter.Shared;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace doob.Scripter.Module.Database
{
    public class DatabaseModule : IScripterModule
    {

        public QueryFactory SqlServerConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            var compiler = new SqlServerCompiler();

            return new QueryFactory(connection, compiler);
        }

        public QueryFactory PostgresConnection(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            var compiler = new PostgresCompiler();

            return new QueryFactory(connection, compiler);
        }

    }
}