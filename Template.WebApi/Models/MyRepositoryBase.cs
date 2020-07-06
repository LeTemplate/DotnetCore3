using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Template.WebApi.Models
{
    /// <summary>
    /// Mysql Client Base
    /// </summary>
    public class MyRepositoryBase
    {
        /// <summary>
        /// Base Connection String
        /// </summary>
        private string _connectionString;
        private readonly ILogger<MyRepositoryBase> _logger;

        public MyRepositoryBase(string connectionString, ILogger<MyRepositoryBase> logger = null)
        {
            if (connectionString == null) throw new ArgumentNullException($"{nameof(connectionString)} can't be null");
            _connectionString = connectionString;
            _logger = logger;
        }

        public MyRepositoryBase(IConfiguration configuration, ILogger<MyRepositoryBase> logger = null)
        {
            if (_connectionString == null) throw new ArgumentNullException($"{nameof(configuration)} can't be null!");
            _connectionString = configuration["Mysql:ConnectionString"];
            _logger = logger;
        }

        public MySqlConnection NewConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}