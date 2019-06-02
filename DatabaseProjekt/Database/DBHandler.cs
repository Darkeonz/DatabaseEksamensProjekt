using DatabaseProjekt.Entities;
using java.lang;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Data
{
    public class DBConnection
    {

        public static void BulkToMySQL(List<Book> bookList)
        {
            string ConnectionString = "";
            StringBuilder sCommand = new StringBuilder("INSERT INTO books (author, title) VALUES ");
            using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
            {
                List<string> Rows = new List<string>();
                foreach (var book in bookList)
                {
                    Rows.Add(string.Format("('{0}','{1}')", MySqlHelper.EscapeString("test"), MySqlHelper.EscapeString("test")));

                    sCommand.append(string.Join(",", Rows));
                    sCommand.append(";");
                    mConnection.Open();
                    using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}