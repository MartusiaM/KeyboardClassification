using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    class Database
    {
        private static string databaseName = "SampleDatabase.sqlite";

        public Database(string FileName)
        {
            CreateDatabase();
            LoadSamplesToDatabase(FileName);
        }

        public Dictionary<int, User> ExtractSamples()
        {
            Dictionary<int, User> samples = new Dictionary<int, User>();

            //pobieranie obiektow z bazy danych
            string command = "SELECT * FROM  tx_badanie01";
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + databaseName + ";Version=3;"))
            {
                dbConnection.Open();
                using (SQLiteCommand SQLCommand = new SQLiteCommand(command, dbConnection))
                {
                    using (SQLiteDataReader reader = SQLCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(1);
                            string probe = reader.GetString(2);

                            if (samples.Keys.Contains(id))
                            {
                                samples[id].Inputs.Add(probe);
                            }
                            else
                            {
                                User user = new User();
                                user.UserID = id;
                                user.Inputs = new List<string>();
                                user.Inputs.Add(probe);
                                samples.Add(id, user);
                            }
                            
                        }
                    }
                }
                dbConnection.Close();
            }

            return samples;
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(databaseName);

            string text = @"CREATE TABLE  tx_badanie01 (
                   time timestamp NOT NULL default CURRENT_TIMESTAMP,
                   user_id int(10) NOT NULL,
                   input0 text NOT NULL,
                   IP text,
                   browser text,
                   PRIMARY KEY  (`time`,`user_id`)
                   );";

            RunCommand(text);
        }

        private void LoadSamplesToDatabase(string FileName)
        {
            try
            {
                string script = File.ReadAllText(FileName);
                RunCommand(script);
            }
            catch(Exception e)
            {
                Console.WriteLine("No file with data found.");
            }

        }

        private void RunCommand(string command)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + databaseName + ";Version=3;"))
            {
                dbConnection.Open();

                using (SQLiteCommand SQLCommand = new SQLiteCommand(dbConnection))
                {
                    SQLCommand.CommandText = command;
                    SQLCommand.CommandType = CommandType.Text;
                    SQLCommand.ExecuteNonQuery();
                    dbConnection.Close();
                }
            }
        }


    }


}
