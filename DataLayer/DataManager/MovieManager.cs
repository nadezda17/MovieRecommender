using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json.Linq;
using DataLayer.DataModel;

namespace DataLayer.DataManager
{
    public class MovieManager
    {
        GraphClient client;
        private Object objLock = new Object();

        public MovieManager()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "cntdb");
            try
            {
                client.Connect();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        #region CRUD
        public List<Movie> getMovies()
        {
            var query = client.Cypher
                           .Match("(m:Movie)")
                           .Return(m => new { Movie = m.As<Movie>() });

            List<Movie> movies = new List<Movie>();

            foreach (var res in query.Results)
            {
                movies.Add(res.Movie);
            }
            return movies;
        }
        public List<Movie> getMoviesById(List<string> ids)
        {
            List<Movie> movies = new List<Movie>();

            string moviesIds = JsonConvert.SerializeObject(ids.Select(int.Parse).ToList());

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("ids", ids);

            var query = client.Cypher
                           .Match("(m:Movie)")
                           .WithParams(queryDict)
                           .Where("m.id in " + moviesIds)
                           .Return(m => new { Movie = m.As<Movie>() });

            foreach (var res in query.Results)
            {
                movies.Add(res.Movie);
            }
            return movies;
        }
        public Movie getMovie(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return(m => new { Movie = m.As<Movie>() });

            Movie toRet = new Movie();

            if (query.Results.Count() == 0)
                toRet = null;
            else
                toRet = query.Results.FirstOrDefault().Movie;

            return toRet;
        }

        public void insertMovie(Movie movie)
        {

            movie.id = getUniqueID();

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("title", movie.title);
            queryDict.Add("id", movie.id);
            queryDict.Add("releaseDate", movie.releaseDate);
            queryDict.Add("trailer", movie.trailer);
            queryDict.Add("imageFileName", movie.imageFileName);

            client.Cypher
                .Create("(m:Movie {id:{id}, title:{title}," +
                "releaseDate:{releaseDate}, imageFileName:{imageFileName}, trailer:{trailer}})")
                .WithParams(queryDict).ExecuteWithoutResults();

            foreach (string genre in movie.genres)
            {
                insertGenre(movie.id, genre, 10);
            }


            insertDirector(movie.id, movie.director, 8);

            foreach (string actor in movie.actors)
            {
                insertActor(movie.id, actor, 6);
            }

            insertTimePeriod(movie.id, movie.timePeriod, 2);
            insertCinema(movie.id, movie.cinema, 4);

        }

        public void deleteMovie(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);
            string queryString = "Match (m:Movie) Where m.id={id} Detach Delete m";
            var query = new CypherQuery(queryString, queryDict, CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public List<string> getGenres(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)-[r:is]-(g:Genre)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return<string>("g.type");

            List<string> genres = query.Results.ToList();

            return genres;
        }
        public string getDirector(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)-[r:directed]-(d:Director)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return<string>("d.name");

            string director = query.Results.First();

            return director;
        }
        public List<string> getActors(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)-[r:acted_in]-(a:Actor)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return<string>("a.name");

            List<string> actors = query.Results.ToList();

            return actors;
        }
        public string getCinema(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)-[r:belongs_to]-(c:Cinema)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return<string>("c.type");

            string cinema = query.Results.First();

            return cinema;
        }
        public string getTimePeriod(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = client.Cypher
                           .Match("(m:Movie)-[r:is_from]-(tp:TimePeriod)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                           .Return<string>("tp.tperiod");

            string tperiod = query.Results.First();

            return tperiod;
        }

        public List<string> getGenres()
        {
            string queryString = "Match (g:Genre) Return Distinct g.type";
            var query = new CypherQuery(queryString, new Dictionary<string, object>(), CypherResultMode.Set);
            var result = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList();

            return result;
        }
        public List<string> getDirectors()
        {
            string queryString = "Match (d:Director) Return Distinct d.name";
            var query = new CypherQuery(queryString, new Dictionary<string, object>(), CypherResultMode.Set);
            var result = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList();

            return result;
        }
        public List<string> getActors()
        {
            string queryString = "Match (a:Actor) Return Distinct a.name";
            var query = new CypherQuery(queryString, new Dictionary<string, object>(), CypherResultMode.Set);
            var result = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList();

            return result;
        }
        public List<string> getCinemas()
        {
            string queryString = "Match (c:Cinema) Return Distinct c.type";
            var query = new CypherQuery(queryString, new Dictionary<string, object>(), CypherResultMode.Set);
            var result = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList();

            return result;
        }

        public void insertGenre(int movieId, string genre, int weight)
        {
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("id", movieId);
                queryDict.Add("genre", genre);
                queryDict.Add("weight", weight.ToString());

                client.Cypher
                           .Match("(m:Movie)")
                           .WithParams(queryDict)
                           .Where("m.id={id}")
                    .Merge("(g:Genre {type:{genre}})")
                    .Create("(m)-[:is {weight:{weight}}]->(g)")
                    .ExecuteWithoutResults();
         }
        public void insertDirector(int movieId, string director, int weight)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", movieId);
            queryDict.Add("directorName", director);
            queryDict.Add("weight", weight);

            client.Cypher
                       .Match("(m:Movie)")
                       .WithParams(queryDict)
                       .Where("m.id={id}")
                .Merge("(d:Director {name:{directorName}})")
                .Create("(d)-[:directed {weight:{weight}}]->(m)")
                .ExecuteWithoutResults();
        }
        public void insertTimePeriod(int movieId, string year, int weight)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", movieId);
            queryDict.Add("timePeriod", year);
            queryDict.Add("weight", weight);

            client.Cypher
                       .Match("(m:Movie)")
                       .WithParams(queryDict)
                       .Where("m.id={id}")
                .Merge("(tp:TimePeriod {tperiod:{timePeriod}})")
                .Create("(m)-[:is_from {weight:{weight}}]->(tp)")
                .ExecuteWithoutResults();
        }
        public void insertCinema(int movieId, string cinema, int weight)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", movieId);
            queryDict.Add("cinema", cinema);
            queryDict.Add("weight", weight);

            client.Cypher
                       .Match("(m:Movie)")
                       .WithParams(queryDict)
                       .Where("m.id={id}")
                .Merge("(c:Cinema {type:{cinema}})")
                .Create("(m)-[:belongs_to {weight:{weight}}]->(c)")
                .ExecuteWithoutResults();
        }
        public void insertActor(int movieId, string actor, int weight)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", movieId);
            queryDict.Add("actorName", actor);
            queryDict.Add("weight", weight);

            client.Cypher
                       .Match("(m:Movie)")
                       .WithParams(queryDict)
                       .Where("m.id={id}")
                .Merge("(a:Actor {name:{actorName}})")
                .Create("(a)-[:acted_in {weight:{weight}}]->(m)")
                .ExecuteWithoutResults();
        }

        #endregion

        public List<KeyValuePair<string, float>> getTerms(string queryString,int movieId)
        {
            List<KeyValuePair<string, float>> toRet = new List<KeyValuePair<string, float>>();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("movieId", movieId);

            var query = new CypherQuery(queryString,
                                    queryDict, CypherResultMode.Set);

            var results = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query);

            foreach (string res in results)
            {
                lock (objLock)
                {
                    dynamic data= JObject.Parse(res);

                    string term = data.term;
                    float weight = data.weight;
                    KeyValuePair<string, float> kvp = new KeyValuePair<string, float>(term, weight);

                    toRet.Add(kvp);
                }
            }

            return toRet;
        }

        public List<KeyValuePair<string, float>> getMovieTerms(int movieId)
        {
            List<KeyValuePair<string, float>> termsList = new List<KeyValuePair<string, float>>();

            string genresQuery = "Match (m:Movie)-[r:is]-(g:Genre)" +
                                    "Where m.id = {movieId}" +
                                    "Return { term: g.type,weight: r.weight} as KeyValue";

            string directorsQuery = "Match (d:Director)-[r:directed]-(m:Movie)" +
                                    "Where m.id = {movieId}" +
                                    "Return { term: d.name,weight: r.weight} as KeyValue";


            string actorsQuery = "Match (a:Actor)-[r:acted_in]-(m:Movie)" +
                                    "Where m.id = {movieId}" +
                                    "Return { term: a.name,weight: r.weight} as KeyValue";

            string timePeriodQuery = "Match (m:Movie)-[r:is_from]-(tp:TimePeriod)" +
                                    "Where m.id = {movieId}" +
                        "Return { term: tp.tperiod,weight: r.weight} as KeyValue";

            string cinemaQuery = "Match (m:Movie)-[r:belongs_to]-(c:Cinema)" +
                                    "Where m.id = {movieId}" +
                        "Return { term: c.type,weight: r.weight} as KeyValue";


            termsList = termsList.Union(getTerms(genresQuery, movieId)).ToList();
            termsList = termsList.Union(getTerms(directorsQuery, movieId)).ToList();
            termsList = termsList.Union(getTerms(actorsQuery, movieId)).ToList();
            termsList = termsList.Union(getTerms(timePeriodQuery, movieId)).ToList();
            termsList = termsList.Union(getTerms(cinemaQuery, movieId)).ToList();

            return termsList;
        }

        public List<Movie> findMovies(string term)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("term", term);

            string queryString = "Match (n)"+ "Where n.type = {term}" +
                "OR n.name = {term} OR n.tperiod = {term}" + 
                "Return {termType:labels(n)} as termObj";

            var queryTerm = new CypherQuery(queryString, queryDict, CypherResultMode.Set);

            var result = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(queryTerm).FirstOrDefault();

            dynamic data = JObject.Parse(result);
            string termType = data.termType[0];

            string relationshipType;
            string property;
            switch (termType)
            {
                case "Genre":
                    relationshipType = "is";
                    property = "type";
                    break;
                case "Director":
                    relationshipType = "directed";
                    property = "name";
                    break;
                case "Actor":
                    relationshipType = "acted_in";
                    property = "name";
                    break;
                case "TimePeriod":
                    relationshipType = "is_from";
                    property = "tperiod";
                    break;
                case "Cinema":
                    relationshipType = "belongs_to";
                    property = "type";
                    break;
                default:
                    relationshipType = "";
                    property = "";
                    break;
            }
            
            queryString = "MATCH (m:Movie)-[r:"+relationshipType+"]-(n:" + termType + ") "+ 
                "Where n."+property+"={term}"+
                " Return m Limit 25";
            var query = new CypherQuery(queryString, queryDict, CypherResultMode.Set);

            List<Movie> movies=((IRawGraphClient)client).ExecuteGetCypherResults<Movie>(query).ToList();
            
            return movies;
        }

        public List<Movie> initRecommendation()
        {
            List<Movie> movies = new List<Movie>();
            List<string> genres = new List<string>();
           
            genres=getGenres();
            
            foreach (string genre in genres)
            {
                var query = client.Cypher
                               .Match("(m:Movie)-[r:is]-(g:Genre)")
                               .WithParam("genre", genre)
                               .Where("g.type={genre}")
                               .Return(m => new { Movie = m.As<Movie>() })
                               .Limit(3);
                foreach (var res in query.Results)
                {
                    bool exist = movies.Any(item => item.id == res.Movie.id);
                    if (!exist)
                        movies.Add(res.Movie);
                }
            }

            return movies;
        }

        public List<Movie> filterMovies(string genre, string director,string actor, string cinema)
        {
            if (genre=="" && actor== "" && director == "" && cinema == "")
                return getMovies();

            List<Movie> movies = new List<Movie>();
            
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("genre", genre);
            queryDict.Add("director", director);
            queryDict.Add("actor", actor);
            queryDict.Add("cinema", cinema);

            string matchString = "(m:Movie)";
            string whereString = "";
            bool ifAnd = false;

            if (genre != "")
            {
                matchString += ", (m)-[:is]-(g:Genre)";
                whereString += "g.type={genre}";
                ifAnd = true;
            }
            if (director != "")
            {
                matchString += ", (m)-[:directed]-(d:Director)";
                if (ifAnd)
                    whereString += " and ";
                ifAnd = true;
                whereString += " d.name={director}";

            }
            if (actor != "")
            {
                matchString += ", (m)-[:acted_in]-(a:Actor)";
                if (ifAnd)
                    whereString += " and ";
                ifAnd = true;
                whereString += " a.name={actor}";
            }
            if (cinema != "")
            {
                matchString += ", (m)-[:belongs_to]-(c:Cinema)";
                if (ifAnd)
                    whereString += " and ";
                ifAnd = true;
                whereString += " c.type={cinema}";
            }

            var query = client.Cypher
                           .Match(matchString)
                           .WithParams(queryDict)
                           .Where(whereString)
                           .Return(m => new { Movie = m.As<Movie>() });
            
            foreach (var res in query.Results)
            {
                movies.Add(res.Movie);
            }
            return movies;
        }

        public int getUniqueID()
        {
            int id = 0;
            string queryString = "MATCH (m:Movie) RETURN m.id AS id ORDER BY m.id DESC LIMIT 1";
            var query = new CypherQuery(queryString,
                new Dictionary<string, object>(),
                CypherResultMode.Set);

            var results = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query);

            string res = results.FirstOrDefault();
            int data;
            if (res != null)
            {
                data = Convert.ToInt16(res);
                id = ++data;
            }

            return id;
        }

    }
}
