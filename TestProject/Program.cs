using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.DataModel;
using DataLayer.DataManager;
using RecommenderEngine;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            User u = new User();
            Movie m = new Movie();

            UserManager um = new UserManager();
            MovieManager mm = new MovieManager();

            #region Test Data (Movies)
            //m.title = "Le fabuleux destin d'Amélie Poulain";
            //m.releaseDate = new DateTime(2002, 02, 08);
            //m.director = " Jean-Pierre Jeunet";
            //m.genres.Add("Comedy");
            //m.genres.Add("Romance");
            //m.actors.Add("Audrey Tautou");
            //m.actors.Add("Mathieu Kassovitz");
            //m.cinema = "France";
            //m.imageFileName = "Amelie-poulain.jpg";
            //m.timePeriod = "2000's";
            //m.trailer = "https://www.youtube.com/watch?v=HUECWi5pX7o";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Eternal Sunshine of the Spotless Mind";
            //m.releaseDate = new DateTime(2004, 03, 19);
            //m.director = " Michel Gondry";
            //m.genres.Add("Drama");
            //m.genres.Add("Romance");
            //m.genres.Add("Sci-Fi");
            //m.actors.Add("Jim Carrey");
            //m.actors.Add("Kate Winslet");
            //m.cinema = "USA";
            //m.timePeriod = "2000's";
            //m.imageFileName = "eternal-sunshine-of-a-spotless-mind.jpg";

            //m.trailer = "https://www.youtube.com/watch?v=yE-f1alkq9I";

            //mm.insertMovie(m);
            //m = new Movie();

            //m.title = " Léon: The Professional";
            //m.releaseDate = new DateTime(1994, 11, 18);
            //m.director = "Luc Besson";
            //m.genres.Add("Crime");
            //m.genres.Add("Thriller");
            //m.genres.Add("Drama");
            //m.actors.Add("Jean Reno");
            //m.actors.Add("Gary Oldman");
            //m.actors.Add("Natalie Portman");
            //m.cinema = "France";
            //m.timePeriod = "90's";
            //m.imageFileName = "leon.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=aNQqoExfQsg";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Pulp Fiction";
            //m.releaseDate = new DateTime(1994, 10, 14);
            //m.director = "Quentin Tarantino";
            //m.genres.Add("Crime");
            //m.genres.Add("Drama");
            //m.actors.Add("John Travolta");
            //m.actors.Add("Uma Thurman");
            //m.actors.Add("Tim Roth");
            //m.actors.Add("Samuel L. Jackson");
            //m.actors.Add("Bruce Willis");
            //m.cinema = "USA";
            //m.imageFileName = "Pulp-Fiction.jpg";
            //m.timePeriod = "90's";
            //m.trailer = "https://www.youtube.com/watch?v=s7EdQ4FqbhY";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Intouchables";
            //m.releaseDate = new DateTime(2011, 11, 02);
            //m.director = "Olivier Nakache";
            //m.director = "Eric Toledano";
            //m.genres.Add("Biography");
            //m.genres.Add("Comedy");
            //m.genres.Add("Drama");
            //m.actors.Add("François Cluzet");
            //m.actors.Add("Omar Sy");
            //m.cinema = "France";
            //m.timePeriod = "2010's";
            //m.imageFileName = "Intouchables.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=34WIbmXkewU";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "American Beauty";
            //m.releaseDate = new DateTime(1999, 10, 01);
            //m.director = "Sam Mendes";
            //m.genres.Add("Drama");
            //m.genres.Add("Romance");
            //m.actors.Add("Kevin Spacey");
            //m.actors.Add("Mena Suvari");
            //m.actors.Add("Annette Bening");
            //m.actors.Add("Thora Birch");
            //m.actors.Add("Wes Bentley");
            //m.cinema = "USA";
            //m.timePeriod = "90's";
            //m.imageFileName = "american-beauty.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=3ycmmJ6rxA8";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Fight Club";
            //m.releaseDate = new DateTime(1999, 10, 15);
            //m.director = "David Fincher";
            //m.genres.Add("Drama");
            //m.actors.Add("Edward Norton");
            //m.actors.Add("Brad Pitt");
            //m.actors.Add("Helena Bonham Carter");
            //m.cinema = "USA";
            //m.timePeriod = "90's";
            //m.imageFileName = "Fight-Club.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=SUXWAEX2jlg";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Requiem for a Dream ";
            //m.releaseDate = new DateTime(2000, 12, 15);
            //m.director = "Darren Aronofsky";
            //m.genres.Add("Drama");
            //m.actors.Add("Ellen Burstyn");
            //m.actors.Add("Jared Leto");
            //m.actors.Add("Jennifer Connelly");
            //m.actors.Add("Marlon Wayans");
            //m.cinema = "USA";
            //m.timePeriod = "2000's";
            //m.imageFileName = "requiem-for-a-dream.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=jzk-lmU4KZ4";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "A Clockwork Orange";
            //m.releaseDate = new DateTime(1971, 12, 19);
            //m.director = "Stanley Kubrick";
            //m.genres.Add("Crime");
            //m.genres.Add("Drama");
            //m.genres.Add("Sci-Fi");
            //m.actors.Add("Malcolm McDowell");
            //m.actors.Add("Patrick Magee");
            //m.actors.Add("Adrienne Corri");
            //m.actors.Add("Miriam Karlin");
            //m.cinema = "UK";
            //m.timePeriod = "70's";
            //m.imageFileName = "clockwork-orange.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=SPRzm8ibDQ8";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "2001: A Space Odyssey";
            //m.releaseDate = new DateTime(1968, 05, 12);
            //m.director = "Stanley Kubrick";
            //m.genres.Add("Adventure");
            //m.genres.Add("Sci-Fi");
            //m.actors.Add("Keir Dullea");
            //m.actors.Add("Gary Lockwood");
            //m.cinema = "UK";
            //m.timePeriod = "60's";
            //m.imageFileName = "space-oddisey.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=Z2UWOeBcsJI";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "Mulholland Dr.";
            //m.releaseDate = new DateTime(2001, 10, 12);
            //m.director = "David Lynch";
            //m.genres.Add("Drama"); m.genres.Add("Mystery");
            //m.genres.Add("Thriller");
            //m.actors.Add("Naomi Watts");
            //m.actors.Add("Justin Theroux");
            //m.actors.Add("Ann Miller");
            //m.actors.Add("Laura Elena Harring ");
            //m.actors.Add("Robert Forster");
            //m.cinema = "USA";
            //m.timePeriod = "2000's";
            //m.imageFileName = "Mulholland-Dr.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=jbZJ487oJlY";

            //mm.insertMovie(m);

            //m = new Movie();
            //m.title = "The Life Aquatic with Steve Zissou";
            //m.releaseDate = new DateTime(2004, 12, 25);
            //m.director = "Wes Anderson";
            //m.genres.Add("Adventure");
            //m.genres.Add("Comedy");
            //m.genres.Add("Drama");
            //m.actors.Add("Bill Murray");
            //m.actors.Add("Owen Wilson");
            //m.actors.Add("Anjelica Huston");
            //m.cinema = "USA";
            //m.timePeriod = "2000's";
            //m.imageFileName = "The-Life-Aquatic-with-Steve-Zissou.jpg";
            //m.trailer = "https://www.youtube.com/watch?v=yh401Rmkq0o";

            //mm.insertMovie(m);
            #endregion

            #region Test Data (User)

            //string[] usernames = { "nadja123", "anica123", "stela123", "ivana123", "ema123", "dina123" };
            //string[] names = { "Nadja", "Anica", "Stela", "Ivana", "Ema", "Dina" };
            //string[] lastnames = { "Milojkovic", "Mitrovic", "Mitic", "Rakic", "Ivanovic", "Petkovic" };

            //for (int i = 0; i < usernames.Length; i++)
            //{
            //    u.username = usernames[i];
            //    u.password = usernames[i];
            //    u.firstName = names[i];
            //    u.lastName = lastnames[i];

            //    um.insertUser(u);
            //}

            #endregion

            #region TestData (interestVector,watchedMovies) 
            //int[] moviesId = new int[] { 1, 4, 6, 7, 10, 8, 11, 2, 6, 3, 1, 2, 3, 4, 5, 11, 9, 5, 2, 6, 4, 6, 8, 10, 11, 1, 2, 5, 6, 7, 9 };
            //for (int i = 0; i < usernames.Length; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //        um.triggerMovieLike(usernames[i], moviesId[i + j], true);
            //}
            #endregion
            
        }
    }
}
