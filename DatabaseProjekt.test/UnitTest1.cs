using DatabaseProjekt.Database;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;


namespace DatabaseProjekt.test
{
    public class UnitTest1
    {
        [Fact]
        public void TestSingleBook()
        {         
            ReadAndInsert readAndInsert = new ReadAndInsert();
            var townlist = readAndInsert.GetTownList();
            var book = File.ReadAllText("book\\17.txt");

            var listSentences = readAndInsert.GetPotentialTownSentences(book, townlist);
            Assert.Equal(2, listSentences.Count);
        }

        [Fact]
        public void TestAllBooks()
        {
            ReadAndInsert readAndInsert = new ReadAndInsert();
            var townlist = readAndInsert.GetTownList();

            var files = Directory.GetFiles("book");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var file in files)
            {
                var book = File.ReadAllText(file);

                var listSentences = readAndInsert.GetPotentialTownSentences(book, townlist);
            }
            
            sw.Stop();
            Console.WriteLine($"Found sentences in {sw.Elapsed}");


        }   
    }
}
