namespace Bolao.Pinheiros.Models
{
    public class Competition
    {
        public int id { get; set; }
        public int countryId { get; set; }
        public int sportId { get; set; }
        public string name { get; set; }
        public bool hasStandings { get; set; }
        public bool hasLiveStandings { get; set; }
        public bool hasStandingsGroups { get; set; }
        public string nameForURL { get; set; }
        public int totalGames { get; set; }
        public int liveGames { get; set; }
        public int popularityRank { get; set; }
        public bool hasActiveGames { get; set; }
        public string longName { get; set; }
        public string shortName { get; set; }
    }
}