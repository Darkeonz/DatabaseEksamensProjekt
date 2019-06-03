using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Entities
{
    public class City
    {
        [BsonId] 
        public int? CityId { get; set; }
        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }
    }
}