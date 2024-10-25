using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {
        private IDbConnection _connection;
        private readonly IOptions<ConexionConfiguracion> _configs;
        public FactoryConnection(IDbConnection connection) 
        {
            _connection = connection;
        }
        
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            if(_connection == null)
            {
                _connection = new MySqlConnection(_configs.Value.MigrationTest);
            }
            if(_connection.State != ConnectionState.Open)
            { 
                _connection.Open();
            }
            return _connection;
        }
    }
}
