using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataModel;
using DataLayer.DataManager;

namespace RecommenderEngine
{
    public class Recommender
    {
        public static void recommendMovies(string username)
        {
            Recommendation recomm = new Recommendation();
            RecommendationManager rm = new RecommendationManager();

            recomm.username = username;
            recomm.lastUpdated = DateTime.Now;

            List<string> CFmovies=CollaborativeFiltering.recommendMovies(username);
            List<string> CBmovies = ContentFiltering.recommendMovies(username);

            recomm.moviesIds = CFmovies.Intersect(CBmovies).ToList();
   
            rm.upsertRecommendation(recomm);
        }

        public static Explanation makeExplanation(string username,string movieId)
        {
            Explanation explanation = new Explanation();
            UserManager um = new UserManager();
            MovieManager mm = new MovieManager();
            User user = um.getUser(username);

            //Content Based Explanation
            Dictionary<string,float> interestVector = user.interestVector;
            List<KeyValuePair<string, float>> movieTerms = mm.getMovieTerms(Convert.ToInt32(movieId));
            Dictionary<string,float> termsDict= movieTerms.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            explanation.explanationCB= interestVector.Where(x => termsDict.ContainsKey(x.Key))
                                    .ToDictionary(x => x.Key, x => x.Value);

            //Collaborative Filtering Explanation
            List<KeyValuePair<string,float>> kSimilarUsers = um.KsimilarUsers(username, 4);
            List<string> usernames = new List<string>(kSimilarUsers.Select(x => x.Key));
            List<User> users = um.getUsers(usernames);

            foreach (User u in users)
            {
                byte watched = 0;

                u.watchedMovies.TryGetValue(movieId,out watched);
                if(watched==1)
                    explanation.explanationCF.Add(u);
            }

            return explanation;
        }
    }
}
