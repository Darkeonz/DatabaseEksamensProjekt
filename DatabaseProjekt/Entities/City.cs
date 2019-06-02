using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProjekt.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}