using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApi.Models
{
    /// <summary>
    /// Micorsoft SQL server base
    /// </summary>
    public class MsRepositoryBase
    {
        /// <summary>
        /// Connecttion String
        /// </summary>
        private string _connectionString;
        private readonly ILogger<MsRepositoryBase> _logger;

        public MsRepositoryBase(string connectionString, ILogger<MsRepositoryBase> logger = null)
        {
            if (connectionString == null) throw new ArgumentNullException($"{nameof(connectionString)} can't be null!");
            _connectionString = connectionString;
        }

        public MsRepositoryBase(IConfiguration configuration, ILogger<MsRepositoryBase> logger = null)
        {
            if (_connectionString == null) throw new ArgumentNullException($"{nameof(configuration)} can't be null!");
            _connectionString = configuration["Mssql:ConnectionString"];
            _logger = logger;
        }

        /// <summary>
        /// Return a new connecttion
        /// </summary>
        /// <returns></returns>
        public SqlConnection NewConnecttion()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Get Effect Rows
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<int> UniversalSafeNonQueryAsync(string sqlcmd, params SqlParameter[] values)
        {
            using (var conn = NewConnecttion())
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sqlcmd, conn);
                cmd.Parameters.AddRange(values);
                var effactLine = 0;
                try
                {
                    effactLine = await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception Type: {}", ex.GetType());
                    _logger.LogError(" Message: {0}", ex.Message);
                }
                await conn.CloseAsync();
                return effactLine;
            }
        }

        /// <summary>
        /// 通用更新或者查找函数
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<DataTable> UniversalSafeQueryAsync(string sqlcmd, params SqlParameter[] values)
        {
            return await Task<DataTable>.Run(() =>
            {
                using (var conn = NewConnecttion())
                {
                    SqlCommand cmd = new SqlCommand(sqlcmd, conn);
                    cmd.Parameters.AddRange(values);
                    // 一次性全部加载到内存
                    try
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                        var table = new DataTable();
                        sqlDataAdapter.Fill(table);
                        return table;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Exception Type: {}", ex.GetType());
                        _logger.LogError(" Message: {0}", ex.Message);
                        return null;
                    }
                }
            });
        }
    }
}