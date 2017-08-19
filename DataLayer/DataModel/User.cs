using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLayer.DataModel
{
    [DataContract]
    public class User
    {
        [DataMember]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string password { get; set; }

        [DataMember]
        public Dictionary<string,byte> watchedMovies { get; set; }

        [DataMember]
        public Dictionary<string,float> interestVector { get; set; }  
                                                         
        [DataMember]
        public Dictionary<string, float> userSimilarityVector { get; set; }


        public User()
        {
            watchedMovies = new Dictionary<string, byte>();
            interestVector = new Dictionary<string, float>();
            userSimilarityVector = new Dictionary<string, float>();
        }
    }
}
