using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using edu.stanford.nlp.ie.crf;
using System.Diagnostics;

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
        public List<string> GetTownList()
        {

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

        public void HarvestDataFromBooks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> townList = GetTownList();


            string[] bookPaths = GetFilePaths();
            foreach (var bookPath in bookPaths)
            {
                // Tager 1 sekund for 100 bøger
                IEnumerable<bool> authorResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Author:"));
                IEnumerable<bool> titleResults = File.ReadAllLines(@bookPath).Select(s => s.Contains("Title:"));
                string author = FindAuthor(authorResults, bookPath);
                string title = FindTitle(titleResults, bookPath);

                // tager 2 sekunder for 100 bøger
                string book = File.ReadAllText(@bookPath);
                // splitter bogen op i sætninger. Sætninger med et bynavn tages ud og køre igennem Stanford Named Entity Recognizer (NER) for .NET for at bedømme om det er en by eller ej.
                // Dette gøres fordi NER er ret tungt at køre på hele bogen. Så med et stort datasæt vil det tage alt for langt tid.

                // Tager 2 minutter for 100 bøger
                List<string> listOfPotentialSentences = GetPotentialTownSentences(book, townList);

                // Tager 2 sekunder for 100 bøger for begge metoder
                List<string> listOfPotentialTownSentences = FindTownsInTxt(listOfPotentialSentences);
                List<string> listOfTowns = GetListOfTowns(listOfPotentialTownSentences);


            }
            sw.Stop();
            TimeSpan elapsedTime = sw.Elapsed;
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
                result.Add(Classifier.classifyToString(sentence));
                
            }

            return result;
        }


        //Optimize method
        public List<string> GetPotentialTownSentences(string book, List<string> townList)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> sentences = Regex.Split(book, @"(?<=[\.!\?])\s+").ToList();
            List<string> sentenceListOfPossibleTowns = new List<string>();

            foreach (var town in townList)
            {
                foreach (var sentence in sentences)
                {
                    bool matchingvalues = sentence.Contains(town);
                    if (matchingvalues)
                    {
                        sentenceListOfPossibleTowns.Add(sentence);
                    }
                }

            }
            sw.Stop();
            return sentenceListOfPossibleTowns;
        }

        public List<string> GetListOfTowns(List<string> listOfPotentialTownSentences)
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
                            }
                            else
                            {
                                townsInBook.Add(word.Remove(index1));
                            }
 
                        }
                    }
                    if (word.EndsWith("/LOCATION") && isNextWordALocation == true)
                    {
                        int index1 = word.IndexOf("/LOCATION");
                        if (index1 != -1)
                        {
                            townName = townName + word.Remove(index1);
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