using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataModel;
using MongoDB.Driver;
using MongoDB.Bson;
using ServiceStack.Text;

namespace DataLayer.DataManager
{
    public class RecommendationManager
    {

        public static IMongoClient _client;
        public static IMongoDatabase _database;

        public RecommendationManager()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("MovieRecommender");
        }

        public Recommendation getRecommendation(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("recommendations");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            BsonDocument b = collection.Find(filter).FirstOrDefault();

            if (b != null)
            {
                Recommendation recomm = JsonSerializer.DeserializeFromString<Recommendation>(b.ToJson());


                return recomm;
            }
            else
                return null;
        }

        public void upsertRecommendation(Recommendation r)
        {
            var collection = _database.GetCollection<BsonDocument>("recommendations");
            var filter = Builders<BsonDocument>.Filter.Eq("username", r.username);

            BsonArray moviesIds = new BsonArray();
            foreach (string movie in r.moviesIds)
            {
                moviesIds.Add(movie);
            }

            var update = Builders<BsonDocument>.Update
                .Set("username", r.username)
                .Set("lastUpdated", r.lastUpdated)
                .Set("moviesIds", r.moviesIds);

            collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public void deleteRecommendation(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("recommendations");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var result = collection.DeleteMany(filter);
        }

 
    }
}
