# DatabaseEksamensProjekt

# Classes in project to pay attention to
- ReadAndInsert.cs (Harvests all the txt books and inserts them into databases)
- DBHandlerMYSQL.cs (Handles the connections to the MYSQL database)
- DBHandlerMongo.cs (Handles the connections to the MongoDb database)
- Book.cs (Entity)
- City.cs (Entity)

## Harvesting books from the Gutenberg Project and inserting them into MYSQL and MongoDb databases.
1) Download the stanford NLP project from https://sergey-tihon.github.io/Stanford.NLP.NET/StanfordNER.html
2) Make sure the path of the unzipped file is C:\Temp\ (as that is where the relation is pointing in the code)
3) Path should look as such. C:\Temp\stanford-ner-2016-10-31\classifiers\english.all.3class.distsim.crf.ser.gz

I had to remove the The Project Gutenberg Etext of Chromosome 12, by the Human Genome Project. It had only DNA code and the file was 146 mb, all which my methods were trying to clean up.

4) The method HarvestDataFromBooks() from the class ReadAndInsert.cs is harvesting all the books for author, title, and English cities and inserting it into the databases.

It takes about 3 hours and 15 minutes to harvest all the books and insert them into the databases. This is optimized through the use of plinq/threads as well as making sure only upper case possible city names are checked. A lot of checks are in place to make sure the data is in correct format.

![alt text](https://i.gyazo.com/67d69dbc39ef06e897dbc200a4860eae.png)

## The Databases used are MYSQL and MongoDB

## The MYSQL database structure
There is a many to many relation between the book and the cities. Therefore I've created 3 tables. One table for books, and one table for cities and then a table with book_id's and city_id's. That way I can join them to retrieve the info I want
![alt text](https://i.gyazo.com/87c9676546eed94eb5bcf0d64f42a573.png)

## The MongoDb database structure

The MongoDb saves the data directly into the nosql collection through a list of books, which each includes a list of cities as an array.

![alt text](https://i.gyazo.com/34a447e582c03b23bd4910a7ea3ecf14.png)

## How the data is modeled in my application.

The model consists two simple entities. A book that has an GUID id for MongoDb, a BookId, an Author, A Title, and a List of objects of City. A city consists of a CityId, Name, Latitude, Longitude.

![alt text](https://i.gyazo.com/6bd2b698ef2b27721f0febb3605037b6.png)

![alt text](https://i.gyazo.com/9ead3133b102562d734542dfe9b7ecbb.png)

## How the data is imported.

The MYSQL queries are performed through the DBHandlerMYSQL.cs class. I used a stringbuilder to build a query for inserting all the books and cities into the MYSQL database in one batch. This is to avoid connecting to the database multiple times and optimizing the time it takes to insert the elements into the database. 

For selecting the data, I've created a method called PerformQueryReturnBooksList(), that performs a given query and then map the result with an object of the Book entity, and put it in a list of books and returns it. This is to prevent redundant connection code to the database and mapping.

I've created a method for each of the queries, whith the main purpose of creating the query strings.

Whenever I need some data, I take all columns that match with what a Book object is in the code. This is to keep a simple design where the data can be accessed through objects.


## Performing a test of the runtime of the 20 queries.

As seen below. It takes 11 sec to run through all 20 queries and return a list of books for each one.

![alt text](https://i.gyazo.com/8025f846e0802cce5ec288a6e091f695.png)

## Which Database is better for a project like this?

The MongoDb takes up a lot more space. Especially when you have several layers with arrays. For me it simply started swallowing my harddisk space when I inserted the data. There is a chance I am not storing the data correct. While MongoDb is known for being faster as it focuses less on the effeciency of saving data, but rather the processing speed, I feel like it won't matter for the end user in this type of project. The insertion into the mysql database took about 2 sec, and each query takes about 0.5 sec. I would pick the MYSQL database for this type of project.
