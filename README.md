# DatabaseEksamensProjekt


## Harvesting books from the Gutenberg Project and inserting them into MYSQL and MongoDb databases.
1) Download the stanford NLP project from https://sergey-tihon.github.io/Stanford.NLP.NET/StanfordNER.html
2) Make sure the path of the unzipped file is C:\Temp\ (as that is where the relation is pointing in the code)
3) Path should look as such. C:\Temp\stanford-ner-2016-10-31\classifiers\english.all.3class.distsim.crf.ser.gz

I had to remove the The Project Gutenberg Etext of Chromosome 12, by the Human Genome Project. It had only DNA code and the file was 146 mb, all which my methods were trying to clean up.

4) The method HarvestDataFromBooks() from the class ReadAndInsert.cs is harvesting all the books for author, title, and English cities and inserting it into the databases.

It takes about 3 hours and 15 minutes to harvest all the books and insert them into the databases. This is optimized through the use of plinq/threads as well as making sure only upper case possible city names are checked.

![alt text](https://i.gyazo.com/67d69dbc39ef06e897dbc200a4860eae.png)

## The Databases used are MYSQL and MongoDB

## The MYSQL database structure
There is a many to many relation between the book and the cities. Therefore I've created 3 tables. One table for books, and one table for cities and then a table with book_id's and city_id's. That way I can join them to retrieve the info I want
![alt text](https://i.gyazo.com/87c9676546eed94eb5bcf0d64f42a573.png)

How data is modeled in your application.
How the data is imported.
Behavior of query test set. Including a discussion on how much of the query runtime is influenced by the DB and what is influenced by the application frontend.
Your recommendation, for which database to use in such a project for production.


It took 3 hours and 14 minutes to harvest all the books and insert them into the databases.

![alt text](https://i.gyazo.com/67d69dbc39ef06e897dbc200a4860eae.png)

Haversine formula for calculating distance

![alt text](https://i.gyazo.com/fe5cc61ee470af2c55fdcae9f9d60db9.png)
