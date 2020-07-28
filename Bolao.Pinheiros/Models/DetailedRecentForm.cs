using System;

namespace Bolao.Pinheiros.Models
{
    public class DetailedRecentForm
    {
        public int id { get; set; }
        public int sportId { get; set; }
        public int competitionId { get; set; }
        public int seasonNum { get; set; }
        public int stageNum { get; set; }
        public int roundNum { get; set; }
        public string competitionDisplayName { get; set; }
        public DateTime startTime { get; set; }
        public int statusGroup { get; set; }
        public string statusText { get; set; }
        public string shortStatusText { get; set; }
        public int gameTimeAndStatusDisplayType { get; set; }
        public double gameTime { get; set; }
        public string gameTimeDisplay { get; set; }
        public bool hasTVNetworks { get; set; }
        public Competitor homeCompetitor { get; set; }
        public Competitor awayCompetitor { get; set; }
        public int? groupNum { get; set; }
        public string competitionGroupByName { get; set; }
    }
}