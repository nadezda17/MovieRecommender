using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataModel;
using DataLayer.DataManager;

namespace RecommenderEngine
{
    public static class ContentFiltering
    {
        public static List<string> recommendMovies(string username)
        {

            UserManager um = new UserManager();
            MovieManager mm = new MovieManager();
            
            List<string> terms = um.topTerms(username);

            List<string> movies = new List<string>();

            foreach (string term in terms)
            {
                List<string> ids = mm.findMovies(term).Select(x => x.id.ToString()).ToList();
                movies =movies.Union(ids).ToList();
            }
            
            return movies;
        }
    }
}
