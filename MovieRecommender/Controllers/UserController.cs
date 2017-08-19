using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServiceStack.Text;
using DataLayer;
using DataLayer.DataModel;
using DataLayer.DataManager;
using RecommenderEngine;

namespace MovieRecommender.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet, Route("api/user/getAll")]
        public List<User> getAll()
        {
            UserManager um = new UserManager();
            return um.getAllUsers();
        }
        
        [HttpGet, Route("api/user/getSimUsers/{username}")]
        public List<User> getSimUsers(string username)
        {
            UserManager um = new UserManager();
            List<string> simUsers = new List<string>(um.KsimilarUsers(username, 4).Select(item => item.Key));

            return um.getUsers(simUsers);
        }

        [HttpPost, Route("api/user/registerUser")]
        public bool registerUser([FromBody] dynamic jSonUserObject)
        {
            User user = JsonSerializer.DeserializeFromString<User>(jSonUserObject.ToString());
            UserManager newUser = new UserManager();
            return newUser.insertUser(user);
        }

        [HttpPost, Route("api/user/loginUser")]
        public User loginUser([FromBody] dynamic jSonUserObject)
        {
            User user = JsonSerializer.DeserializeFromString<User>(jSonUserObject.ToString());
            UserManager userc = new UserManager();
            user = userc.getUser(user.username, user.password);

            if (user != null)
                Recommender.recommendMovies(user.username);

            return user;
        }

        [HttpGet, Route("api/user/delete/{username}")]
        public void deleteUser(string username)
        {
            UserManager um = new UserManager();
            um.deleteUser(username);
        }
        
        [HttpPost, Route("api/user/update")]
        public void update([FromBody] dynamic jSonUserObject)
        {
            User user = JsonSerializer.DeserializeFromString<User>(jSonUserObject.ToString());
            UserManager userc = new UserManager();
            userc.updateUser(user);
        }

        [HttpPost, Route("api/User/movieLiked")]
        public void movieLiked([FromBody] dynamic dataObject)
        {
            UserManager um = new UserManager();
            string username = dataObject.username;
            int movieId = Convert.ToInt32(dataObject.movieId);
            bool ifLike = dataObject.like;
            um.triggerMovieLike(username, movieId, ifLike);
        }

        [HttpGet, Route("api/User/getWatchedMovies/{username}")]
        public List<Movie> getWatchedMovies(string username)
        {
            UserManager um = new UserManager();
            MovieManager mm = new MovieManager();
            User user=um.getUser(username);
            List<string> watchedMovies = user.watchedMovies.Select(item => item.Key).ToList();
            return mm.getMoviesById(watchedMovies);
        }
        
    }
}
