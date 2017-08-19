using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLayer.DataModel
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime releaseDate { get; set; }
        public string imageFileName { get; set; }
        public string trailer { get; set; }

        public List<string> genres { get; set; }
        public string director { get; set; }
        public List<string> actors { get; set; }
        public string timePeriod { get; set; }
        public string cinema { get; set; }

        public Movie()
        {
            genres = new List<string>();
            actors = new List<string>();
        }
    }
}
