using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace RPG_Game
{
    public class DatabaseHelper
    {
        public static DatabaseHelper instance;
        private readonly SqlConnection conn;

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
        }


        public int StoreGameData(Tables table, DateTime time, byte[] data)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
                string rawSql = $"INSERT INTO {tableStr} (TimeStamp, Data) OUTPUT INSERTED.ID VALUES (@TIMESTAMP, @DATA);";
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
                return lastId;
            }
            else
            {
                throw new ArgumentException($"Retrieval failed for Table \"{table}\".");
            }
        }

        public void UpdateGameData(Tables table, int id, DateTime time, byte[] data)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
                string rawSql = $"UPDATE {tableStr} SET TimeStamp = @TIME, Data = @DATA WHERE Id = @ID";

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
                conn.Close();
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

            return timeStamps.ToArray();
        }
        public byte[] RetrieveData(Tables table, int id)
        {
            if (tableDictionary.TryGetValue(table, out string tableStr))
            {
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
                return data;
            }
            else
            {
                throw new ArgumentException($"Retrieval failed for Table \"{table}\".");
            }
        }
    }

    public enum Tables
    {
        Inventory,
        Saves
    }
}
