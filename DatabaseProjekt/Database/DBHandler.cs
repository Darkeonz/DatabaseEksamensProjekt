using DatabaseProjekt.Entities;
using java.lang;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Database
{
    public class DBHandler
    {

        public void BulkBooksToMySQL(List<Book> bookList)
        {
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder queryForBooks = new StringBuilder("INSERT INTO books (id, author, title) VALUES ");
            StringBuilder queryForCityBookMatch = new StringBuilder("INSERT INTO books_cities_mentions (book_id, city_id) VALUES ");

            using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
            {
                List<string> Rows = new List<string>();
                List<string> RowsCity = new List<string>();
                foreach (var book in bookList)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}')", book.BookId, MySqlHelper.EscapeString(book.Author), MySqlHelper.EscapeString(book.Title)));
                    foreach (var city in book.Cities)
                    {
                        RowsCity.Add(string.Format("('{0}','{1}')", book.BookId, city.CityId));
                    }
                    
                }
                queryForBooks.append(string.Join(",", Rows));
                queryForBooks.append(";");
                queryForCityBookMatch.append(string.Join(",", RowsCity));
                queryForCityBookMatch.append(";");
                mConnection.Open();
                using (MySqlCommand myCmd = new MySqlCommand(queryForBooks.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
                using (MySqlCommand myCmd = new MySqlCommand(queryForCityBookMatch.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
        }

        public void BulkCitiesToMySQL(List<City> cityList)
        {
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder queryForCities = new StringBuilder("INSERT INTO cities (id, name, latitude, longitude) VALUES ");
          
            using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
            {
                List<string> Rows = new List<string>();            
                foreach (var city in cityList)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}')", city.CityId, MySqlHelper.EscapeString(city.Name), city.Latitude, city.Longitude));
                }
                queryForCities.append(string.Join(",", Rows));
                queryForCities.append(";");

                mConnection.Open();
                using (MySqlCommand myCmd = new MySqlCommand(queryForCities.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
        }
        
    }
}