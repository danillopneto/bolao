using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Row
    {
        public Competitor competitor { get; set; }
        public int gamePlayed { get; set; }
        public int gamesWon { get; set; }
        public int gamesLost { get; set; }
        public int gamesEven { get; set; }
        public int For { get; set; }
        public int against { get; set; }
        public double ratio { get; set; }
        public double points { get; set; }
        public int strike { get; set; }
        public int gamesOT { get; set; }
        public int gamesWonOnOT { get; set; }
        public int gamesWonOnPen { get; set; }
        public int gamesLossOnOT { get; set; }
        public int gamesLossOnPen { get; set; }
        public string pct { get; set; }
        public int position { get; set; }
        public int trend { get; set; }
        public List<DetailedRecentForm> detailedRecentForm { get; set; }
        public List<int> recentForm { get; set; }
        public int destinationNum { get; set; }
        public int? groupNum { get; set; }
        public int? liveGameId { get; set; }
        public int? liveGameStatus { get; set; }
    }
}