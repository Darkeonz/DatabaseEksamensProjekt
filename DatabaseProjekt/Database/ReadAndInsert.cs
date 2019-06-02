using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using edu.stanford.nlp.ie.crf;
using System.Diagnostics;
using DatabaseProjekt.Entities;

namespace DatabaseProjekt.Database
{
    public class ReadAndInsert
    {

        public static CRFClassifier Classifier =
          CRFClassifier.getClassifierNoExceptions(
            @"C:\Temp\stanford-ner-2016-10-31\classifiers\english.all.3class.distsim.crf.ser.gz");



        //danner en liste af stierne til byerne.
        public string[] GetFilePaths()
        {
            return Directory.GetFiles(@"E:\Skoleprojekter\testdata\testbooks");
        }

        //Henter alle engelske bynavne ind og lægger dem i en liste.
        public List<City> GetTownList()
        {

            using (var reader = new StreamReader(@"E:\Skoleprojekter\testdata\testtowns\towns.csv"))
            {
                List<City> listOfCities = new List<City>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    var city = new City() { Name = values[0], Latitude = Convert.ToDouble(values[1]), Longitude = Convert.ToDouble(values[2]), CityId = Convert.ToInt32(values[3]) };
                    listOfCities.Add(city);
                }
                return listOfCities;
            }
        }

        public void HarvestDataFromBooks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int bookId = 0;
            var townList = GetTownList();
            var listOfBooks = new List<Book>();
            string[] bookPaths = GetFilePaths();
            foreach (var bookPath in bookPaths)
            {
                // Tager 1 sekund for 100 bøger
                IEnumerable<bool> authorResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Author:"));
                IEnumerable<bool> titleResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Title:"));
                string author = FindAuthor(authorResults, bookPath);
                string title = FindTitle(titleResults, bookPath);

                // tager 2 sekunder for 100 bøger
                string bookText = File.ReadAllText(@bookPath);

                // splitter bogen op i sætninger. Sætninger med et bynavn tages ud og køre igennem Stanford Named Entity Recognizer (NER) for .NET for at bedømme om det er en by eller ej.
                // Dette gøres fordi NER er ret tungt at køre på hele bogen. Så med et stort datasæt vil det tage alt for langt tid.

                // Tager 0.30 minutter for 100 bøger
                List<string> listOfPotentialSentences = GetPotentialTownSentences(bookText, townList);

                // Tager 2 sekunder for 100 bøger for begge metoder
                List<string> listOfPotentialTownSentences = FindTownsInTxt(listOfPotentialSentences);
                List<string> listOfNERCities = GetListOfNERCities(listOfPotentialTownSentences);
                var citiesInBook = FindCitiesInBook(listOfNERCities, townList);
                var book = new Book() { BookId = bookId, Author = author, Title = title, Cities = citiesInBook };
                listOfBooks.Add(book);
                bookId++;
            }

            // tager 33 sekunder for 100 bøger at udføre alle handlinger
            sw.Stop();
            TimeSpan elapsedTime = sw.Elapsed;
        }

        private List<City> FindCitiesInBook(List<string> listOfNERCities, List<City> townList)
        {
            var listOfCitiesInBook = new List<City>();
            foreach (var town in townList)
            {
                if (listOfNERCities.Contains(town.Name))
                {
                    listOfCitiesInBook.Add(town);
                }
            }
            return listOfCitiesInBook;
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

        public List<string> FindTownsInTxt(List<string> sentences)
        {
            List<string> result = new List<string>();
            foreach (var sentence in sentences)
            {
                string sentenceClean = sentence.Replace(Environment.NewLine, " ");
                sentenceClean = sentenceClean.Replace(",", " ").Replace(";", "").Replace(":", "");
                result.Add(Classifier.classifyToString(Regex.Replace(sentenceClean, @"[^0-9A-Za-z ,]", "")));

            }

            return result;
        }


        //Optimize method
        public List<string> GetPotentialTownSentences(string book, List<City> townList)
        {

            string[] sentences = book.Split(new char[] { '.', '?', '!' });

            var sentenceQuery = from sentence in sentences.AsParallel()
                                let words = sentence.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' },
                                                        StringSplitOptions.RemoveEmptyEntries).Where(word => Char.IsUpper(word[0]))
                                where words.Distinct().Intersect(townList.Select(t => t.Name).ToArray()).Any()
                                select sentence;

            var sentenceListOfPossibleTowns = sentenceQuery.ToList();


            return sentenceListOfPossibleTowns;
        }

        public List<string> GetListOfNERCities(List<string> listOfPotentialTownSentences)
        {
            bool isNextWordALocation = false;
            string townName = string.Empty;

            List<string> townsInBook = new List<string>();
            foreach (var PotentialTownSentences in listOfPotentialTownSentences)
            {
                int i = 1;
                string[] words = PotentialTownSentences.Split(' ');

                foreach (var word in words)
                {
                    if (i < words.Length)
                    {
                        isNextWordALocation = CheckIfNextWordIsLocation(words[i]);
                    }

                    i++;
                    if (word.EndsWith("/LOCATION") && isNextWordALocation == false)
                    {
                        int index1 = word.IndexOf("/LOCATION");
                        if (index1 != -1)
                        {

                            if (townName != "")
                            {
                                townName = townName + " " + word.Remove(index1);
                                townsInBook.Add(townName);
                                townName = string.Empty;
                            }
                            else
                            {
                                townsInBook.Add(word.Remove(index1));
                                townName = string.Empty;
                            }

                        }
                    }
                    if (word.EndsWith("/LOCATION") && isNextWordALocation == true)
                    {
                        int index1 = word.IndexOf("/LOCATION");
                        if (index1 != -1)
                        {
                            townName = townName + " " + word.Remove(index1);
                        }
                    }
                }
            }
            return townsInBook;
        }

        public bool CheckIfNextWordIsLocation(string word)
        {
            if (word.EndsWith("/LOCATION"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}