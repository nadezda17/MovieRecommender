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
    [DataContract]
    public class Recommendation
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public List<string> moviesIds { get; set; } //id-evi preporučenih filmova
        [DataMember]
        public DateTime lastUpdated { get; set; }
        

        public Recommendation()
        {
            moviesIds = new List<string>();
        }
    }
}
