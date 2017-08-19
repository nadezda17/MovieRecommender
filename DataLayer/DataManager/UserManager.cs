using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using DataLayer.DataModel;
using System.Dynamic;
using ServiceStack.Text;
using Newtonsoft.Json;

namespace DataLayer.DataManager
{
    public class UserManager
    {
        public static IMongoClient _client;
        public static IMongoDatabase _database;
        private Object objLock = new Object();

        public UserManager()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("MovieRecommender");
        }

        #region CRUD

        public List<User> getAllUsers()
        {
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Ne("username", "admin");
            var result = collection.Find(filter).ToList();
            List<User> allUsers = new List<User>();

            foreach (var u in result)
            {
                User user = new User();
                BsonDocument userDoc = new BsonDocument();
                userDoc = u.ToBsonDocument();
                user = deserializeUser(userDoc);

                allUsers.Add(user);
            }

            return allUsers;
        }

        public List<User> getUsers(List<string> usernames)
        {
            List<User> users = new List<User>();
            var collection = _database.GetCollection<User>("users");
            var filterDef = new FilterDefinitionBuilder<User>();
            var filter = filterDef.In(x => x.username, usernames);


            var result = collection.Find(filter).ToList();
            User user = new User();

            foreach (User u in result.ToList())
                users.Add(u);

            return users;
        }

        public User getUser(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var result = collection.Find(filter).ToList();

            User user = new User();

            if (result.Count != 0)
            {
                BsonDocument userDoc = result.First().ToBsonDocument();
                user = deserializeUser(userDoc);
                //BsonSerializer.Deserialize<User>(userDoc);
            }

            return user;
        }
        
        public User getUser(string username, string password)
        {
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username) & Builders<BsonDocument>.Filter.Eq("password", password);
            BsonDocument b = collection.Find(filter).FirstOrDefault();
            if (b == null)
            {
                return null;
            }
            else
            {
                User u = ServiceStack.Text.JsonSerializer.DeserializeFromString<User>(b.ToJson());
                return u;
            }
        }

        public bool insertUser(User newUser)
        {
            if (!isUniqueUser(newUser.username))
                return false;

            var userDoc = new BsonDocument {
                {"firstName", newUser.firstName },
                {"lastName", newUser.lastName },
                {"username", newUser.username},
                { "password", newUser.password}
            };

            var collection = _database.GetCollection<BsonDocument>("users");
            collection.InsertOneAsync(userDoc);

            return true;
        }

        public void updateUser(User u)
        {
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", u.username);
            var update = Builders<BsonDocument>.Update
                .Set("password", u.password)
                .Set("firstName", u.firstName)
                .Set("lastName", u.lastName);

            var result = collection.UpdateOneAsync(filter, update);
        }

        public void deleteUser(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var result = collection.DeleteMany(filter);
            
            RecommendationManager rm = new RecommendationManager();
            rm.deleteRecommendation(username);   
        }
        
        #endregion

        public void insertWatchedMovie(string username, int movieId, bool like)
        {
            User user = new User();
            user = getUser(username);
            if (user.username == "")
                return;

            List<string> movies = new List<string>(user.watchedMovies.Keys);
            if (movies.Contains(movieId.ToString()))
                return;
            else
                user.watchedMovies.Add(movieId.ToString(), (like) ? (byte)1 : (byte)0);

            var update = Builders<BsonDocument>.Update
                .Set("watchedMovies", user.watchedMovies);

            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            collection.UpdateOneAsync(filter, update);
        }

        public void updateInterestVector(string username, string term, float weight, bool like)
        {
            User u = new User();
            u = getUser(username);
            if (u.username == "")
                return;

            Dictionary<string, float> interestVector = new Dictionary<string, float>();

            if (!like)
                weight = -weight;

            if (u.interestVector.ContainsKey(term))
                u.interestVector[term] += weight;
            else
                u.interestVector.Add(term, weight);

            interestVector = u.interestVector;
            var update = Builders<BsonDocument>.Update
                         .Set("interestVector", interestVector);

            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            collection.UpdateOneAsync(filter, update);
        }

        public void updateSimilarity(User u)
        {
            var update = Builders<BsonDocument>.Update
                         .Set("userSimilarityVector", u.userSimilarityVector);

            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", u.username);
            collection.UpdateOneAsync(filter, update);
        }
        
        public void triggerMovieLike(string username, int movieID, bool like)
        {
            MovieManager mm = new MovieManager();
            Movie movie = new Movie();
            movie = mm.getMovie(movieID);

            insertWatchedMovie(username, movie.id, like);

            List<KeyValuePair<string, float>> targetTerms = new List<KeyValuePair<string, float>>();
            targetTerms = mm.getMovieTerms(movieID);

            string term;
            float weight;
            foreach (KeyValuePair<string, float> kvp in targetTerms)
            {
                lock (objLock)
                {
                    term = kvp.Key;
                    weight = kvp.Value;

                    updateInterestVector(username, term, weight, like);
                }
            }
        }
        
        public List<KeyValuePair<string, float>> KsimilarUsers(string username, int k) 
        {
            User targetUser = new User();
            UserManager um = new UserManager();
            targetUser = um.getUser(username);
            List<KeyValuePair<string, float>> toRet = new List<KeyValuePair<string, float>>();
            int count = targetUser.userSimilarityVector.Count;

            if (k > count)
                k = count;
            toRet = targetUser.userSimilarityVector.OrderByDescending(x => x.Value).Take(k).ToList();

            toRet.RemoveAll(x => x.Value < 0);

            return toRet;
        }

        public List<string> topTerms(string username)
        {
            User u = new User();
            u = getUser(username);
            var topTerms = u.interestVector.OrderByDescending(entry => entry.Value)
                     .Take(3)
                     .ToDictionary(pair => pair.Key, pair => pair.Value);

            List<string> topTermsList = new List<string>(topTerms.Keys);

            return topTermsList;
        }
        
        public bool isUniqueUser(string username)
        {
            bool unique = true;
            var collection = _database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var result = collection.Find(filter).ToList();

            if (result.Count > 0)
                unique = false;

            return unique;
        }

        public User deserializeUser(BsonDocument userDoc)
        {
            User user = new User();

            user.id = userDoc.GetValue("_id", new BsonString(string.Empty)).AsObjectId.ToString();
            user.firstName = userDoc.GetValue("firstName", new BsonString(string.Empty)).AsString;
            user.lastName = userDoc.GetValue("lastName", new BsonString(string.Empty)).AsString;
            user.username = userDoc.GetValue("username", new BsonString(string.Empty)).AsString;
            user.password = userDoc.GetValue("password", new BsonString(string.Empty)).AsString;

            var resMovies = userDoc.GetValue("watchedMovies", null);

            if (resMovies != null)
            {
                string moviesDoc = resMovies.ToString();
                user.watchedMovies = JsonConvert.DeserializeObject<Dictionary<string, byte>>(moviesDoc);
            }

            BsonValue resInterestVec = userDoc.GetValue("interestVector", null);
            if (resInterestVec != null)
            {
                string interestVecDoc = resInterestVec.ToString();
                user.interestVector = JsonConvert.DeserializeObject<Dictionary<string, float>>(interestVecDoc);
            }

            BsonValue resSimilarityVec = userDoc.GetValue("userSimilarityVector", null);
            if (resSimilarityVec != null)
            {
                string similarityVec = resSimilarityVec.ToString();
                user.userSimilarityVector = JsonConvert.DeserializeObject<Dictionary<string, float>>(similarityVec);
            }

            return user;
        }
    }
}
