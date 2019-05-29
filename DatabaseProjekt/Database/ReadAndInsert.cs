using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.OleDb;


namespace DatabaseProjekt.Database
{
    public class ReadAndInsert
    {
      
        //danner en liste af stierne til byerne.
        public string[] GetFilePaths() {
            return Directory.GetFiles(@"E:\Skoleprojekter\testdata\testbooks");
        }

        //Henter alle engelske bynavne ind og lægger dem i en liste.
        public List<string> GetTownList() {

            using (var reader = new StreamReader(@"E:\Skoleprojekter\testdata\testtowns\towns.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('"');

                    listA.Add(values[3]);                   
                }
                return listA;
            }
        }

        public void ReadAllBooks() {

            string[] bookPaths = GetFilePaths();
            foreach (var bookPath in bookPaths)
            {             
                IEnumerable<bool> authorResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Author:"));
                IEnumerable<bool> titleResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Title:"));
                string author = FindAuthor(authorResults, bookPath);
                string title = FindTitle(titleResults, bookPath);

                string book = File.ReadAllText(@bookPath);
                List<string> townList = GetTownList();
                foreach (var town in townList)
                {
                    bool matchingvalues = book.Contains(town);
                    if (matchingvalues)
                    {
                        string value = town;
                    }
                }
                
               
            }
            
        }

        //Finder author for en bog
        public string FindAuthor(IEnumerable<bool> authorResults, string bookPath)
        {
            string[] book = File.ReadAllLines(@bookPath);
            int authorLineNumber = 0;
            foreach (var authorResult in authorResults)
            {
                if (authorResult)
                {
                    string authorLineResult = book[authorLineNumber];
                    return authorLineResult.Substring(authorLineResult.IndexOf("Author: ") + "Author: ".Length);                                      
                }
                authorLineNumber = authorLineNumber + 1;
            }
            return "No author found";
        }

        //finder title for en bog
        public string FindTitle(IEnumerable<bool> titleResults, string bookPath)
        {
            string[] book = File.ReadAllLines(@bookPath);
            int titleLineNumber = 0;
            foreach (var titleResult in titleResults)
            {
                if (titleResult)
                {
                    string authorLineResult = book[titleLineNumber];
                    return authorLineResult.Substring(authorLineResult.IndexOf("Title: ") + "Title: ".Length);                   
                }
                titleLineNumber = titleLineNumber + 1;
            }
            return "No title found";
        }

    }
}