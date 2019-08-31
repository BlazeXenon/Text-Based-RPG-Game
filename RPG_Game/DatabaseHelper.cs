using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Dapper;

namespace RPG_Game
{
    public class DatabaseHelper
    {
        public static DatabaseHelper instance;
        private readonly SqlConnection conn;
        private readonly IDbConnection newCon;

        private readonly Dictionary<Tables, string> tableDictionary;

        public DatabaseHelper(string connectionName)
        {
            instance = this;

            tableDictionary = new Dictionary<Tables, string>
            {
                {Tables.Inventory, "Inventory"},
                {Tables.Saves, "Saves"}
            };

            conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
            newCon = new SQLiteConnection(ConfigurationManager.ConnectionStrings["GameDataSQLite"].ConnectionString);
        }

        public bool StoreMultipleGameData(Tables[] tables, DateTime[] times, List<byte[]> dataEntries, out int lastId)
        {
            // Ensures that we have the same length for tables, times, and dataEntries
            if (tables.Length != times.Length || tables.Length != dataEntries.Count ||
                times.Length != dataEntries.Count)
            {
                lastId = -1;
                return false;
            }

            bool transactionComplete = true;
            lastId = -1;

            newCon.Open();
            using (var trans = newCon.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < tables.Length; i++)
                    {
                        // Retrieve variables for this entry
                        Tables table = tables[i];
                        DateTime time = times[i];
                        byte[] data = dataEntries[i];

                        if (tableDictionary.TryGetValue(table, out string tableStr))
                        {
                            string rawSql = $"INSERT INTO {tableStr} (TimeStamp, Data) VALUES (@TIMESTAMP, @DATA);";
                            string getLastIdSql = "SELECT last_insert_rowid();";
                        
                            DynamicParameters dp = new DynamicParameters();
                            dp.Add("@TIMESTAMP", time.ToString(), DbType.AnsiString);
                            dp.Add("@DATA", data, DbType.Binary);

                            newCon.Execute(new CommandDefinition(rawSql, dp));
                            long id = (long)newCon.ExecuteScalar(getLastIdSql);
                            lastId = (int)id;
                        }
                        else
                        {
                            Console.WriteLine($"Retrieval failed for Table \"{table}\".");
                            transactionComplete = false;
                            break;
                        }
                    }

                    if (transactionComplete)
                        trans.Commit();
                    else
                        trans.Rollback();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    transactionComplete = false;
                }
                
            }
            newCon.Close();
            return transactionComplete;
        }

        public int StoreGameData(Tables table, DateTime time, byte[] data)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
                string rawSql = $"INSERT INTO {tableStr} (TimeStamp, Data) VALUES (@TIMESTAMP, @DATA);";
                string getLastIdSql = "SELECT last_insert_rowid();";
                long lastId = -1;

                newCon.Open();

                DynamicParameters dp = new DynamicParameters();
                dp.Add("@TIMESTAMP", time.ToString(), DbType.AnsiString);
                dp.Add("@DATA", data, DbType.Binary);

                newCon.Execute(new CommandDefinition(rawSql, dp));
                lastId = (long)newCon.ExecuteScalar(getLastIdSql);

                newCon.Close();

                return (int)lastId;

                /*string rawSql = $"INSERT INTO {tableStr} (TimeStamp, Data) OUTPUT INSERTED.ID VALUES (@TIMESTAMP, @DATA);";
                int lastId = -1;

                conn.Open();
                using (SqlCommand sql = new SqlCommand(rawSql, conn))
                {
                    sql.Prepare();

                    sql.Parameters.Add("@TIMESTAMP", SqlDbType.DateTime);
                    sql.Parameters.Add("@DATA", SqlDbType.VarBinary);

                    sql.Parameters["@TIMESTAMP"].Value = time;
                    sql.Parameters["@DATA"].Value = data;

                    lastId = (int)sql.ExecuteScalar();
                }
                conn.Close();
                return lastId;*/
            }
            else
            {
                throw new ArgumentException($"Retrieval failed for Table \"{table}\".");
            }
        }

        public bool UpdateMultipleGameData(Tables[] tables, int[] ids, DateTime[] times, List<byte[]> dataEntries)
        {
            // Ensures that we have the same length for tables, times, and dataEntries
            if (tables.Length != times.Length || tables.Length != dataEntries.Count ||
                times.Length != dataEntries.Count)
                return false;

            bool transactionComplete = true;

            newCon.Open();
            using (var trans = newCon.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < tables.Length; i++)
                    {
                        Tables table = tables[i];
                        int id = ids[i];
                        DateTime time = times[i];
                        byte[] data = dataEntries[i];

                        if (tableDictionary.TryGetValue(table, out string tableStr))
                        {
                            string rawSql = $"UPDATE {tableStr} SET TimeStamp = @TIME, Data = @DATA WHERE Id = @ID";

                            DynamicParameters dp = new DynamicParameters();
                            dp.Add("@ID", id, DbType.Int32);
                            dp.Add("@TIME", time.ToString(), DbType.AnsiString);
                            dp.Add("@DATA", data, DbType.Binary);

                            newCon.Execute(new CommandDefinition(rawSql, dp));
                        }
                        else
                        {
                            Console.WriteLine($"Retrieval failed for Table \"{table}\".");
                            transactionComplete = false;
                            break;
                        }
                    }

                    if (transactionComplete)
                        trans.Commit();
                    else
                        trans.Rollback();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    transactionComplete = false;
                }
            }
            newCon.Close();

            return transactionComplete;
        }

        public void UpdateGameData(Tables table, int id, DateTime time, byte[] data)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
                string rawSql = $"UPDATE {tableStr} SET TimeStamp = @TIME, Data = @DATA WHERE Id = @ID";

                newCon.Open();

                DynamicParameters dp = new DynamicParameters();
                dp.Add("@ID", id, DbType.Int32);
                dp.Add("@TIME", time.ToString(), DbType.AnsiString);
                dp.Add("@DATA", data, DbType.Binary);

                newCon.Execute(new CommandDefinition(rawSql, dp));

                newCon.Close();

                /*string rawSql = $"UPDATE {tableStr} SET TimeStamp = @TIME, Data = @DATA WHERE Id = @ID";

                conn.Open();
                using (SqlCommand sql = new SqlCommand(rawSql, conn))
                {
                    sql.Prepare();

                    sql.Parameters.Add("@TIME", SqlDbType.DateTime);
                    sql.Parameters.Add("@DATA", SqlDbType.VarBinary);
                    sql.Parameters.Add("@ID", SqlDbType.Int);

                    sql.Parameters["@TIME"].Value = time;
                    sql.Parameters["@DATA"].Value = data;
                    sql.Parameters["@ID"].Value = id;

                    sql.ExecuteNonQuery();
                }
                conn.Close();*/
            }
            else
            {
                throw new ArgumentException("Retrieval failed for Table \"{table}\".");
            }
        }

        public string[] GetLoadScreenData()
        {
            string rawSql = "SELECT * FROM Saves;";
            List<string> timeStamps = new List<string>();

            newCon.Open();

            using (IDataReader reader = newCon.ExecuteReader(rawSql))
            {
                while (reader.Read())
                {
                    PlayerStats currentStats = Program.Deserialize((byte[])reader["Data"]) as PlayerStats;
                    string preparedString = "";
                    preparedString += reader["TimeStamp"].ToString();
                    preparedString += $" | {currentStats.Name}, {currentStats.PlayerClass.ToString()} Level {currentStats.Level}";
                    timeStamps.Add(preparedString);
                }
            }

            newCon.Close();
            return timeStamps.ToArray();

            /*
            string rawSql = "SELECT * FROM Saves;";
            List<string> timeStamps = new List<string>();

            conn.Open();
            using (SqlCommand com = new SqlCommand(rawSql, conn))
            {
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlayerStats currentStats = Program.Deserialize((byte[])reader["Data"]) as PlayerStats;
                        string preparedString = "";
                        preparedString += reader["TimeStamp"].ToString();
                        preparedString += $" | {currentStats.Name}, {currentStats.PlayerClass.ToString()} Level {currentStats.Level}";
                        timeStamps.Add(preparedString);
                    }
                }
            }
            conn.Close();

            return timeStamps.ToArray();*/
        }
        public byte[] RetrieveData(Tables table, int id)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
                string rawSql = $"SELECT Data FROM {tableStr} WHERE Id = @ID";
                byte[] data;

                newCon.Open();
                
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@ID", id, DbType.Int32);

                data = newCon.ExecuteScalar(rawSql, dp) as byte[];

                newCon.Close();
                return data;

                /*
                byte[] data;
                conn.Open();
                using (SqlCommand com = new SqlCommand($"SELECT Data FROM {tableStr} WHERE Id = @ID", conn))
                {
                    com.Prepare();
                    com.Parameters.Add("@ID", SqlDbType.Int);
                    com.Parameters["@ID"].Value = id;
                    data = com.ExecuteScalar() as byte[];
                }
                conn.Close();
                return data;*/
            }
            else
            {
                throw new ArgumentException($"Retrieval failed for Table \"{table}\".");
            }
        }

        public void DeleteSaveData()
        {
            string saveDel = "DELETE FROM Saves WHERE Id = @Id";
            string invDel = "DELETE FROM Inventory WHERE Id = @Id";

            newCon.Open();

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@Id", Menu.GameSaveId, DbType.Int32);

            using (var transcation = newCon.BeginTransaction())
            {
                newCon.Execute(saveDel, dp);
                newCon.Execute(invDel, dp);
                transcation.Commit();
            }

            newCon.Close();
        }
    }

    public enum Tables
    {
        Inventory,
        Saves
    }
}
