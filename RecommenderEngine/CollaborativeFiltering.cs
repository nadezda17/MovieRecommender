using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataModel;
using DataLayer.DataManager;

namespace RecommenderEngine
{
    public static class CollaborativeFiltering
    {
        public static void computeUserSimilarity(string username)
        {
            User targetUser = new User();
            UserManager um = new UserManager();
            targetUser = um.getUser(username);
            List<User> allUsers = new List<User>();
            allUsers = um.getAllUsers();

            Dictionary<string, float> interest1stUser = new Dictionary<string, float>();
            interest1stUser = targetUser.interestVector;

            foreach (User u in allUsers)
            {
                if (u.username == targetUser.username)
                    continue;
                Dictionary<string, float> interest2ndUser = new Dictionary<string, float>();
                interest2ndUser = u.interestVector;

                var terms = interest1stUser.Where(d => interest2ndUser.ContainsKey(d.Key));
                List<string> commonTerms = new List<string>();
                commonTerms = (from kvp in terms select kvp.Key).ToList(); //lista zajednickih termina od interesa

                int arrayLenght = commonTerms.Count;
                if (arrayLenght != 0)
                {

                    float[] vector_i, vector_j;
                    vector_i = new float[arrayLenght];
                    vector_j = new float[arrayLenght];

                    int i = 0;
                    foreach (string term in commonTerms)
                    {
                        interest1stUser.TryGetValue(term, out vector_i[i]);
                        interest2ndUser.TryGetValue(term, out vector_j[i]);
                        i++;
                    }

                    float weight = PearsonCoeff(vector_i, vector_j);
                    targetUser.userSimilarityVector[u.username] = weight;
                }
            }

            um.updateSimilarity(targetUser);
        }
        
        public static List<string> recommendMovies(string username)
        {
            computeUserSimilarity(username);


            List<User> usersList = new List<User>();
            User user = new User();
            UserManager um = new UserManager();

            List<KeyValuePair<string, float>> topUsers = new List<KeyValuePair<string, float>>();
            topUsers = um.KsimilarUsers(username, 10);

            string[] simUsers = topUsers.Select(x => x.Key).ToArray();

            foreach (string u in simUsers)
            {
                user = um.getUser(u);
                usersList.Add(user);
            }

            List<string> movies = new List<string>();
            List<string> userMovies = new List<string>();

            foreach (User u in usersList)
            {
                userMovies = u.watchedMovies.Where(k => k.Value == 1).Select(k => k.Key).ToList<string>();
                movies = movies.Union(userMovies).ToList();
            }

            User targetUser = new User();
            targetUser = um.getUser(username);
            movies = movies.Except(targetUser.watchedMovies.Keys).ToList();

            return movies;
        }
        
        public static float PearsonCoeff(float[] iVector, float[] jVector)
        {
            if (iVector.Length != jVector.Length)
                return -1;

            float coeff;

            float avgi = iVector.Average();
            float avgj = jVector.Average();

            var sum = iVector.Zip(jVector, (first, second) => (first - avgi) * (second - avgj)).Sum();

            var sumSqri = iVector.Sum(x => Math.Pow((x - avgi), 2.0));
            var sumSqrj = jVector.Sum(y => Math.Pow((y - avgi), 2.0));

            coeff = sum /(float) Math.Sqrt(sumSqri * sumSqrj);

            if (float.IsNaN(coeff))
                coeff = 0;

            return !(float.IsNaN(coeff))? coeff : 0;
        }
    }
}
