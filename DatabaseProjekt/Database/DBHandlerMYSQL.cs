using DatabaseProjekt.Entities;
using java.lang;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Database
{
    public class DBHandlerMYSQL
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
            StringBuilder queryForCities = new StringBuilder("INSERT INTO cities (id, nameofcity, latitude, longitude) VALUES ");

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

        public List<Book> GetAllBooks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var city = new City();
            List<Book> bookList = new List<Book>();
            List<City> cities = new List<City>();

            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder query = new StringBuilder(
                "SELECT books.id as bookid, cities.id as cityid, books.author, books.title, cities.nameofcity, cities.latitude, cities.longitude " +
                "FROM books " +
                "JOIN books_cities_mentions ON books.id = books_cities_mentions.book_id " +
                "JOIN cities ON cities.id = books_cities_mentions.city_id ");

            bookList = PerformQueryReturnBooksList(query, ConnectionString);

            sw.Stop();
            TimeSpan elapsedTime = sw.Elapsed;
            return bookList;
        }

        public List<Book> GetBooksByTitle(string bookTitle)
        {
            List<Book> bookList = new List<Book>();
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder query = new StringBuilder(
                "SELECT books.id as bookid, cities.id as cityid, books.author, books.title, cities.nameofcity, cities.latitude, cities.longitude " +
                "FROM books " +
                "JOIN books_cities_mentions ON books.id = books_cities_mentions.book_id " +
                "JOIN cities ON cities.id = books_cities_mentions.city_id " +
                "Where books.title = " + "'" + bookTitle + "'");

            bookList = PerformQueryReturnBooksList(query, ConnectionString);
            return bookList;
        }

        public List<Book> GetBooksByAuthor(string author)
        {
            List<Book> bookList = new List<Book>();
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder query = new StringBuilder(
                "SELECT books.id as bookid, cities.id as cityid, books.author, books.title, cities.nameofcity, cities.latitude, cities.longitude " +
                "FROM books " +
                "JOIN books_cities_mentions ON books.id = books_cities_mentions.book_id " +
                "JOIN cities ON cities.id = books_cities_mentions.city_id " +
                "Where books.author = " + "'" + author + "'");

            bookList = PerformQueryReturnBooksList(query, ConnectionString);
            return bookList;
        }

        public List<Book> GetBooksByCity(string cityName)
        {
            List<Book> bookList = new List<Book>();
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder queryForAllBooks = new StringBuilder(
                "SELECT books.id as bookid, cities.id as cityid, books.author, books.title, cities.nameofcity, cities.latitude, cities.longitude " +
                "FROM books " +
                "JOIN books_cities_mentions ON books.id = books_cities_mentions.book_id " +
                "JOIN cities ON cities.id = books_cities_mentions.city_id " +
                "Where cities.nameofcity = " + "'" + cityName + "'");

            bookList = PerformQueryReturnBooksList(queryForAllBooks, ConnectionString);
            return bookList;
        }
        // "SELECT *, ( 3959 * acos( cos( radians(" . $lat. ") ) * cos( radians( lat ) ) * cos( radians( lng ) - radians(" . $lng. ") ) + sin( radians(" . $lat. ") ) * sin( radians( lat ) ) ) ) AS distance FROM your_table HAVING distance < 5"

        public List<Book> GetBooksXMilesFromGeolocation(float lat, float lng, int distance)
        {
            List<Book> bookList = new List<Book>();
            string ConnectionString = "Server=127.0.0.1;Database=gutenberg;Uid=root;Pwd=Rallermus1;";
            StringBuilder query = new StringBuilder(
                "SELECT books.id as bookid, cities.id as cityid, books.author, books.title, cities.nameofcity, cities.latitude, cities.longitude " +
                ", ( 3959 * acos( cos( radians(" + lat + ") ) * cos( radians( cities.latitude ) ) * cos( radians( cities.longitude ) - radians(" + lng + ") ) + sin( radians(" + lat + ") ) * sin( radians( cities.latitude ) ) ) ) AS distance " +
                "FROM books " +
                "JOIN books_cities_mentions ON books.id = books_cities_mentions.book_id " +
                "JOIN cities ON cities.id = books_cities_mentions.city_id HAVING distance < " + distance
                );

            bookList = PerformQueryReturnBooksList(query, ConnectionString);
            return bookList;
        }


        public List<Book> PerformQueryReturnBooksList(StringBuilder qeury, string ConnectionString)
        {
            List<Book> bookList = new List<Book>();
            List<City> cities = new List<City>();

            using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                mConnection.Open();

                using (dataAdapter.SelectCommand = new MySqlCommand(qeury.ToString(), mConnection))
                {
                    DataTable table = new DataTable();
                    dataAdapter.Fill(table);

                    foreach (DataRow Row in table.Rows)
                    {
                        if (bookList.Count == 0)
                        {
                            var city = new City();
                            var book = new Book()
                            {
                                BookId = Row.Field<int>("bookid"),
                                Author = Row.Field<string>("author"),
                                Title = Row.Field<string>("title")
                            };
                            city.CityId = Row.Field<int>("cityid");
                            city.Name = Row.Field<string>("nameofcity");
                            city.Latitude = Row.Field<float>("latitude");
                            city.Longitude = Row.Field<float>("longitude");
                            cities.Add(city);
                            book.Cities = cities;
                            bookList.Add(book);
                        }
                        else if ((bookList[bookList.Count - 1].BookId == Row.Field<int>("bookid") && bookList.Count != 0))
                        {
                            var city = new City()
                            {
                                CityId = Row.Field<int>("cityid"),
                                Name = Row.Field<string>("nameofcity"),
                                Latitude = Row.Field<float>("latitude"),
                                Longitude = Row.Field<float>("longitude")
                            };
                            cities.Add(city);
                        }
                        else
                        {
                            var book = new Book()
                            {
                                BookId = Row.Field<int>("bookid"),
                                Author = Row.Field<string>("author"),
                                Title = Row.Field<string>("title"),
                                Cities = cities
                            };
                            bookList.Add(book);
                        }

                    }
                }
            }
            return bookList;
        }
    }
}