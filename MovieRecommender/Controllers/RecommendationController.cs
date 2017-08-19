using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer.DataModel;
using DataLayer.DataManager;

namespace MovieRecommender.Controllers
{
    public class RecommendationController : ApiController
    {
        [HttpGet, Route("api/Recommendation/get/{username}")]
        public List<Movie> get(string username)
        {
            RecommendationManager rm = new RecommendationManager();
            Recommendation recomm= rm.getRecommendation(username);

            MovieManager mm=new MovieManager();
            return mm.getMoviesById(recomm.moviesIds);
        }
    }
}
