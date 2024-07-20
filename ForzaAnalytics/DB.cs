using Dapper;
using Microsoft.Data.SqlClient;

namespace ForzaAnalytics
{
    internal static class DB
    {
        //public const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true;Trusted_Connection=True;MultipleActiveResultSets=false;Encrypt=false";
        public const string ConnectionString = "Server=.;Integrated Security=true;Trusted_Connection=True;MultipleActiveResultSets=false;Encrypt=false";
        public const string DBName = "ForzaAnalytics";
        public const string TableName = "Packet";


        public static async Task<SqlConnection> Setup()
        {
            using var sql = new SqlConnection(ConnectionString);

            var db = await sql.ExecuteScalarAsync<int?>($"SELECT db_id('{DBName}')");

            if (db is null)
            {
                await sql.ExecuteAsync($@"CREATE DATABASE [{DBName}];");
            }

            var dbSql = new SqlConnection($"{ConnectionString};Initial Catalog={DBName}");

            var table = await dbSql.ExecuteScalarAsync<int?>($"SELECT object_id('{TableName}')");

            if (table is null)
            {
                await CreateTable<Packet>(dbSql);
            }

            return dbSql;
        }

        static async Task CreateTable<T>(SqlConnection dbSql)
        {
            //var cmd = $"CREATE TABLE {TableName} (Id int IDENTITY, Time datetimeoffset(7) DEFAULT(SYSDATETIMEOFFSET()), {string.Join(", ", GetFields().Select(x => $"{x.name} {typemap[x.type]} NOT NULL"))})";

            //await dbSql.ExecuteAsync(cmd);

            //IEnumerable<(string name, Type type)> GetFields()
            //{
            //	foreach (var f in typeof(T).GetProperties())
            //	{
            //		yield return (f.Name, f.PropertyType);
            //	}
            //}
        }

        internal static string CreateInsertCmd()
        {
            return $"INSERT INTO {DB.TableName}({string.Join(", ", GetFields().Select(x => x.name))}) values ({string.Join(", ", GetFields().Select(x => $"@{x.name}"))})";

            IEnumerable<(string name, Type type)> GetFields()
            {
                foreach (var f in typeof(Packet).GetProperties())
                {
                    yield return (f.Name, f.PropertyType);
                }
            }
        }

        internal static async Task<int> PurgeAfter(SqlConnection sql, float currentRaceTime, float distance, DateTimeOffset timestampMS)
        {
            return await sql.ExecuteAsync("delete from Packet where [Time] > @timestampMS and (CurrentRaceTime >= @currentRaceTime OR Distance > @distance)", new { timestampMS, currentRaceTime, distance });
        }

        static readonly Dictionary<Type, string> typemap = new()
        {
            { typeof(float) , "real" },
            { typeof(long) , "bigint" },
            { typeof(int) , "int" },
            { typeof(short) , "smallint" },
            { typeof(byte) , "tinyint" },
            { typeof(bool) , "bit" },
        };
    }
}
