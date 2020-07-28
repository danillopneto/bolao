using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Competitor
    {
        public int id { get; set; }
        public int countryId { get; set; }
        public int sportId { get; set; }
        public string name { get; set; }
        public double score { get; set; }
        public double aggregatedScore { get; set; }
        public bool isQualified { get; set; }
        public bool isWinner { get; set; }
        public int redCards { get; set; }
        public string nameForURL { get; set; }
        public int type { get; set; }
        public int popularityRank { get; set; }
        public string shortName { get; set; }
        public string longName { get; set; }
        public List<int> recentMatches { get; set; }
        public Lineups lineups { get; set; }
        public List<Statistic> statistics { get; set; }
    }
}