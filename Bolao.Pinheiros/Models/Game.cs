using Bolao.Pinheiros.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bolao.Pinheiros.Models
{
    public class Game
    {
        public const int FIRST_TEN_MINUTES = 10;
        public const string GOAL_NAME = "Gol";
        public const int HALF_TIME = 45;
        public const int STAGE_ID_FIRST_HALF = 7;
        public const int STAGE_ID_SECOND_HALF = 9;

        public string aggregateText { get; set; }
        public Competitor awayCompetitor { get; set; }
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
        public Competitor homeCompetitor { get; set; }
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
        public string competitionGroupByName { get; set; }

        public bool DidBothTeamScore()
        {
            return homeCompetitor.score > 0
                    && awayCompetitor.score > 0;
        }

        public string GetCurrentScoreText()
        {
            return startTime > DateTime.Now
                        ? startTime.ToBrasiliaDateTime().ToShortTimeString()
                        : homeCompetitor.score >= 0 ? string.Format("{0} - {1}", homeCompetitor.score, awayCompetitor.score) : string.Empty;
        }

        public int GetGoalsFirstExtraTime()
        {
            return events != null
                        ? events.Where(x => x.IsGoal() && x.stageId == STAGE_ID_FIRST_HALF && x.addedTime > 0).Count()
                        : 0;
        }

        public int GetGoalsFirstHalf()
        {
            return events != null
                        ? events.Where(x => x.IsGoal() && x.stageId == STAGE_ID_FIRST_HALF).Count()
                        : 0;
        }

        public int GetGoalsFirstTenMinutes()
        {
            return events != null
                        ? events.Where(x => x.IsGoal() && x.gameTime <= FIRST_TEN_MINUTES).Count()
                        : 0;
        }

        public int GetGoalsSecondExtraTime()
        {
            return events != null
                        ? events.Where(x => x.IsGoal() && x.stageId == STAGE_ID_SECOND_HALF && x.addedTime > 0).Count()
                        : 0;
        }

        public int GetGoalsSecondHalf()
        {
            return events != null
                        ? events.Where(x => x.IsGoal() && x.stageId == STAGE_ID_SECOND_HALF).Count()
                        : 0;
        }

        public Competitor GetLoser()
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

        public Member GetPlayer(int playerId)
        {
            if (members.Any(x => x.id == playerId))
            {
                return members.First(x => x.id == playerId);
            }
            
            return null;
        }

        public Competitor GetOtherTeam(int teamId)
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
            if (homeCompetitor.score < 0)
            {
                return 0;
            }

            return homeCompetitor.score + awayCompetitor.score;
        }

        public Competitor GetTeam(int teamId)
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

        public Competitor GetWinner()
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

        public bool IsEnded()
        {
            return statusGroup == 4;
        }

        public bool IsGameLive()
        {
            return gameTimeAndStatusDisplayType == 2;
        }

        public bool IsGamePlaying()
        {
            return statusGroup == 3;
        }

        public bool IsTeamInGame(int teamId)
        {
            return homeCompetitor.id == teamId || awayCompetitor.id == teamId;
        }

        public override string ToString()
        {
            if (homeCompetitor == null
                    || awayCompetitor == null)
            {
                return string.Empty;
            }

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