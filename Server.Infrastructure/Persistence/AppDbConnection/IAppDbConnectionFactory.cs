using Microsoft.Data.SqlClient;

namespace Server.Infrastructure.Persistence.AppDbConnection;

public interface IAppDbConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync();
}
