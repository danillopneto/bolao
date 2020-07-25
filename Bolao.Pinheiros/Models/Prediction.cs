using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Prediction
    {
        public int id { get; set; }
        public int type { get; set; }
        public string title { get; set; }
        public bool showVotes { get; set; }
        public int totalVotes { get; set; }
        public Odds odds { get; set; }
        public List<Option> options { get; set; }
    }
}