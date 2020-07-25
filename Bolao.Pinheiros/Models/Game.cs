using System;
using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Game
    {
        public int id { get; set; }
        public int sportId { get; set; }
        public int competitionId { get; set; }
        public string competitionDisplayName { get; set; }
        public DateTime startTime { get; set; }
        public int statusGroup { get; set; }
        public string statusText { get; set; }
        public string shortStatusText { get; set; }
        public int gameTimeAndStatusDisplayType { get; set; }
        public double gameTime { get; set; }
        public string gameTimeDisplay { get; set; }
        public bool hasTVNetworks { get; set; }
        public GameCompetitor homeCompetitor { get; set; }
        public GameCompetitor awayCompetitor { get; set; }
        public bool? hasBetsTeaser { get; set; }
        public Odds odds { get; set; }
        public int? seasonNum { get; set; }
        public int? stageNum { get; set; }
        public int? roundNum { get; set; }
        public bool? hasLineups { get; set; }
        public bool? hasFieldPositions { get; set; }
        public string winDescription { get; set; }
        public int? groupNum { get; set; }
        public bool? hasMissingPlayers { get; set; }
        public string aggregateText { get; set; }
        public string shortAggregateText { get; set; }

        public List<Stage> stages { get; set; }
        public List<Event> events { get; set; }
        public Venue venue { get; set; }
        public List<int> previousMeetings { get; set; }
        public List<Official> officials { get; set; }
        public List<TvNetwork> tvNetworks { get; set; }
        public List<Member> members { get; set; }
        public List<Widget> widgets { get; set; }
        public List<Prediction> predictions { get; set; }

        public bool IsDraw()
        {
            return homeCompetitor.score == awayCompetitor.score;
        }

        public GameCompetitor GetLoser()
        {
            if (homeCompetitor.score < awayCompetitor.score)
            {
                return homeCompetitor;
            }
            else if (awayCompetitor.score < homeCompetitor.score)
            {
                return awayCompetitor;
            }

            return null;
        }

        public GameCompetitor GetWinner()
        {
            if (homeCompetitor.score > awayCompetitor.score)
            {
                return homeCompetitor;
            }
            else if (awayCompetitor.score > homeCompetitor.score)
            {
                return awayCompetitor;
            }

            return null;
        }
    }
}