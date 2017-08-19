using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataModel
{
    public class Explanation
    {
        public Dictionary<string, float> explanationCB;
        public List<User> explanationCF;

        public Explanation()
        {
            explanationCB = new Dictionary<string, float>();
            explanationCF = new List<User>();
        }
    }
}
