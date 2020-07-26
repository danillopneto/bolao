using System;
using System.Collections.Generic;
using System.Linq;

namespace Bolao.Pinheiros.Models
{
    public class Game
    {
        private const int FIRST_TEN_MINUTES = 10;
        private const string GOAL_NAME = "Gol";
        private const int HALF_TIME = 45;
        private const int STAGE_ID_FIRST_HALF = 7;
        private const int STAGE_ID_SECOND_HALF = 9;

        public string aggregateText { get; set; }
        public GameCompetitor awayCompetitor { get; set; }
        public string competitionDisplayName { get; set; }
        public int competitionId { get; set; }
        public List<Event> events { get; set; }
        public double gameTime { get; set; }
        public int gameTimeAndStatusDisplayType { get; set; }
        public string gameTimeDisplay { get; set; }
        public int? groupNum { get; set; }
        public bool? hasBetsTeaser { get; set; }
        public bool? hasFieldPositions { get; set; }
        public bool? hasLineups { get; set; }
        public bool? hasMissingPlayers { get; set; }
        public bool hasTVNetworks { get; set; }
        public GameCompetitor homeCompetitor { get; set; }
        public int id { get; set; }
        public List<Member> members { get; set; }
        public Odds odds { get; set; }
        public List<Official> officials { get; set; }
        public List<Prediction> predictions { get; set; }
        public List<int> previousMeetings { get; set; }
        public int? roundNum { get; set; }
        public int? seasonNum { get; set; }
        public string shortAggregateText { get; set; }
        public string shortStatusText { get; set; }
        public int sportId { get; set; }
        public int? stageNum { get; set; }
        public List<Stage> stages { get; set; }
        public DateTime startTime { get; set; }
        public int statusGroup { get; set; }
        public string statusText { get; set; }
        public List<TvNetwork> tvNetworks { get; set; }
        public Venue venue { get; set; }
        public List<Widget> widgets { get; set; }
        public string winDescription { get; set; }

        public bool DidBothTeamScore()
        {
            return homeCompetitor.score > 0
                    && awayCompetitor.score > 0;
        }

        public string GetCurrentScoreText()
        {
            return startTime > DateTime.Now
                        ? startTime.ToShortTimeString()
                        : homeCompetitor.score >= 0 ? string.Format("{0} - {1}", homeCompetitor.score, awayCompetitor.score) : string.Empty;
        }

        public int GetGoalsFirstExtraTime()
        {
            return events != null
                        ? events.Where(x => x.eventType.name == GOAL_NAME && x.stageId == STAGE_ID_FIRST_HALF && x.addedTime > 0).Count()
                        : 0;
        }

        public int GetGoalsFirstHalf()
        {
            return events != null
                        ? events.Where(x => x.eventType.name == GOAL_NAME && x.stageId == STAGE_ID_FIRST_HALF).Count()
                        : 0;
        }

        public int GetGoalsFirstTenMinutes()
        {
            return events != null
                        ? events.Where(x => x.eventType.name == GOAL_NAME && x.gameTime <= FIRST_TEN_MINUTES).Count()
                        : 0;
        }

        public int GetGoalsSecondExtraTime()
        {
            return events != null
                        ? events.Where(x => x.eventType.name == GOAL_NAME && x.stageId == STAGE_ID_SECOND_HALF && x.addedTime > 0).Count()
                        : 0;
        }

        public int GetGoalsSecondHalf()
        {
            return events != null
                        ? events.Where(x => x.eventType.name == GOAL_NAME && x.stageId == STAGE_ID_SECOND_HALF).Count()
                        : 0;
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

        public GameCompetitor GetOtherTeam(int teamId)
        {
            if (homeCompetitor.id != teamId)
            {
                return homeCompetitor;
            }
            else if (awayCompetitor.id != teamId)
            {
                return awayCompetitor;
            }

            return null;
        }

        public double GetSumScore()
        {
            return homeCompetitor.score + awayCompetitor.score;
        }

        public GameCompetitor GetTeam(int teamId)
        {
            if (homeCompetitor.id == teamId)
            {
                return homeCompetitor;
            }
            else if (awayCompetitor.id == teamId)
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

        public bool IsDraw()
        {
            return homeCompetitor.score == awayCompetitor.score;
        }

        public bool IsTeamInGame(int teamId)
        {
            return homeCompetitor.id == teamId || awayCompetitor.id == teamId;
        }

        public override string ToString()
        {
            return string.Format(
                                 "{0} {1} x {2} {3} - {4}",
                                 homeCompetitor.name,
                                 homeCompetitor.score >= 0 ? homeCompetitor.score.ToString() : string.Empty,
                                 awayCompetitor.score >= 0 ? awayCompetitor.score.ToString() : string.Empty,
                                 awayCompetitor.name,
                                 startTime.ToShortDateString());
        }
    }
}