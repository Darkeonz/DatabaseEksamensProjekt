﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Entities
{
    public class Book
    {
        [BsonId]
        public Guid BookMongoId { get; set; }
        public int? BookId { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

        public List<City> Cities { get; set; }
    }
}