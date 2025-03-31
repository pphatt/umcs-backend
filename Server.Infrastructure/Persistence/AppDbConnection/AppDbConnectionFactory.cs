using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Server.Infrastructure.Persistence.AppDbConnection;

public class AppDbConnectionFactory : IAppDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public AppDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<SqlConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        return connection;
    }
}
