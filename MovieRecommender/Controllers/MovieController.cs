using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ServiceStack.Text;
using MovieRecommender.Providers;
using DataLayer.DataManager;
using DataLayer.DataModel;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace MovieRecommender.Controllers
{
    public class MovieController : ApiController
    {
        [HttpGet, Route("api/Movie/getAll")]
        public List<Movie> getAll()
        {
            MovieManager mm = new MovieManager();
            return mm.getMovies();
        }

        [HttpGet, Route("api/Movie/getMovie/{id}")]
        public Movie getMovie(int id)
        {
            MovieManager mm = new MovieManager();
            Movie movie = mm.getMovie(id);
            movie.genres = mm.getGenres(id);
            movie.director = mm.getDirector(id);
            movie.actors = mm.getActors(id);
            movie.cinema = mm.getCinema(id);
            movie.timePeriod = mm.getTimePeriod(id);

            return movie;
        }

        [HttpPost, Route("api/Movie/insert")]
        public void insert([FromBody] dynamic jsonMovieObject)
        {
            Movie movie = ServiceStack.Text.JsonSerializer.DeserializeFromString<Movie>(jsonMovieObject.ToString());
            string actors = jsonMovieObject.actors.ToString();
            movie.actors = actors.Split(',').ToList();
            MovieManager mm = new MovieManager();

            mm.insertMovie(movie);
        }

        [HttpGet, Route("api/movie/delete/{id}")]
        public void delete(int id)
        {
            MovieManager mm = new MovieManager();
            mm.deleteMovie(id);
        }

        [HttpGet, Route("api/Movie/getGenres")]
        public List<string> getGenres()
        {
            MovieManager mm = new MovieManager();
            return mm.getGenres();
        }
        [HttpGet, Route("api/Movie/getDirectors")]
        public List<string> getDirectors()
        {
            MovieManager mm = new MovieManager();
            return mm.getDirectors();
        }
        [HttpGet, Route("api/Movie/getActors")]
        public List<string> getActors()
        {
            MovieManager mm = new MovieManager();
            return mm.getActors();
        }
        [HttpGet, Route("api/Movie/getCinemas")]
        public List<string> getCinemas()
        {
            MovieManager mm = new MovieManager();
            return mm.getCinemas();
        }
        
        [HttpPost, Route("api/Movie/filterMovies")]
        public List<Movie> filterMovies([FromBody] dynamic filters)
        {
            MovieManager mm = new MovieManager();
            string genre = filters.genre;
            string director = filters.director;
            string actor = filters.actor;
            string cinema = filters.cinema;

            return mm.filterMovies(genre, director, actor, cinema);

        }
        
        [HttpGet, Route("api/Movie/getExplanation/{username}")]
        public Dictionary<string,Explanation> getExplanation(string username)
        {
            RecommendationManager rm = new RecommendationManager();
            Recommendation recomm = rm.getRecommendation(username);
            Dictionary<string, Explanation> explanation = new Dictionary<string, Explanation>();

            foreach(string movieID in recomm.moviesIds)
            explanation.Add(movieID,
                RecommenderEngine.Recommender.makeExplanation(recomm.username, movieID));

            return explanation;
        }

        [HttpGet, Route("api/Movie/initRecomm")]
        public List<Movie> initRecomm()
        {
            MovieManager mm = new MovieManager();
            return mm.initRecommendation();
        }
        
        [HttpPost, Route("api/movie/uploadImage")]
        public async Task<string> uploadImage()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                //string uploadPath = "../Content/images/coffees";
                string uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/movies");
               
                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);

                return "Uploaded image successfully";
            }
            else
            {
                return "Error! Could not upload image.";
            }
        }

        [HttpGet, Route("api/coffee/getImage/{imageName}")]
        public HttpResponseMessage getImage(string imageName)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            String filePath = HttpContext.Current.Server.MapPath("~/content/images/movies/" + imageName);

            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            result.Content = new ByteArrayContent(memoryStream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            fileStream.Close();
            return result;
        }
    }
}
