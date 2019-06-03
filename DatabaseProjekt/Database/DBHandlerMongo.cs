using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Database
{
    public class DBHandlerMongo
    {
        
        private IMongoDatabase db;

        public DBHandlerMongo(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);

        }

        public void InsertRecord<t>(string table, t record)
        {
            var collection = db.GetCollection<t>(table);
            collection.InsertOne(record);
        }
    }
}