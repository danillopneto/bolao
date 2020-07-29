using Bolao.Pinheiros.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bolao.Pinheiros.Models
{
    public class Root
    {
        private int CORNER_ID = 8;
        
        private List<Game> _gamesBetweenTeams;

        public List<Bookmaker> bookmakers { get; set; }
        public List<Competition> competitions { get; set; }
        public List<Competitor> competitors { get; set; }
        public List<Country> countries { get; set; }
        public DateTime Date { get; set; }
        public List<Game> games { get; set; }
        public long lastUpdateId { get; set; }
        public int liveGamesCount { get; set; }
        public Game mainGame { get; set; }
        public int requestedUpdateId { get; set; }
        public List<Sport> sports { get; set; }
        public List<Standing> standings { get; set; }
        public int ttl { get; set; }

        #region " AWAY TEAM "

        public IEnumerable<Game> GetAwayDefeats()
        {
            return GetAwayGames().Where(x => x.GetWinner() != null && x.GetWinner().id != mainGame.awayCompetitor.id);
        }

        public IEnumerable<Competitor> GetAwayDefeatsTeams()
        {
            return GetAwayDefeats().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        public IEnumerable<Game> GetAwayDraws()
        {
            return GetAwayGames().Where(x => x.IsDraw());
        }

        public IEnumerable<Competitor> GetAwayDrawsTeams()
        {
            return GetAwayDraws().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        public List<Game> GetAwayGames()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            var awayGames = games.Where(x => gamesBetweenTeams.All(g => g.id != x.id)
                                            && x.IsTeamInGame(mainGame.awayCompetitor.id)).ToList();
            return FixGamesWithoutScore(awayGames);
        }

        public IEnumerable<Game> GetAwayWinnings()
        {
            return GetAwayGames().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<Game> GetAwayWinningsBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<Competitor> GetAwayWinningsTeams()
        {
            return GetAwayWinnings().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        #endregion " AWAY TEAM "

        #region " HOME TEAM "

        public IEnumerable<Game> GetHomeDefeats()
        {
            return GetHomeGames().Where(x => x.GetWinner() != null && x.GetWinner().id != mainGame.homeCompetitor.id);
        }

        public IEnumerable<Competitor> GetHomeDefeatsTeams()
        {
            return GetHomeDefeats().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.homeCompetitor.id)).ToList();
        }

        public IEnumerable<Game> GetHomeDraws()
        {
            return GetHomeGames().Where(x => x.IsDraw());
        }

        public IEnumerable<Competitor> GetHomeDrawsTeams()
        {
            return GetHomeDraws().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.homeCompetitor.id)).ToList();
        }

        public List<Game> GetHomeGames()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            var homeGames = games.Where(x => gamesBetweenTeams.All(g => g.id != x.id)
                                            && x.IsTeamInGame(mainGame.homeCompetitor.id)).ToList();
            return FixGamesWithoutScore(homeGames);
        }

        public IEnumerable<Game> GetHomeWinnings()
        {
            return GetHomeGames().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public IEnumerable<Game> GetHomeWinningsBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public IEnumerable<Competitor> GetHomeWinningsTeams()
        {
            return GetHomeWinnings().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.homeCompetitor.id)).ToList();
        }

        #endregion " HOME TEAM "

        public IEnumerable<Game> GetDraws()
        {
            return GetGamesBetweenTeams().Where(x => x.IsDraw());
        }

        public List<Game> GetGamesBetweenTeams()
        {
            if (_gamesBetweenTeams == null)
            {
                _gamesBetweenTeams = FixGamesWithoutScore(games).Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id
                                                                         && x.awayCompetitor.id == mainGame.awayCompetitor.id).ToList();
                _gamesBetweenTeams.AddRange(FixGamesWithoutScore(games).Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id
                                                                                && x.awayCompetitor.id == mainGame.homeCompetitor.id).ToList());
            }

            return _gamesBetweenTeams ?? new List<Game>();
        }

        public Row GetTeamStanding(int competitionId, int teamId)
        {
            if (standings != null && standings.Any())
            {
                var competition = standings.FirstOrDefault(x => x.competitionId == competitionId);
                if (competition != null)
                {
                    var teamData = competition.rows.FirstOrDefault(t => t.competitor.id == teamId);
                    if (teamData != null)
                    {
                        return teamData;
                    }
                }
            }

            return null;
        }

        #region " GOALS BY TIME "

        public int GetGoalsFirstExtraTimeAway()
        {
            return GetAwayGames().Sum(x => x.GetGoalsFirstExtraTime());
        }

        public int GetGoalsFirstExtraTimeBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetGoalsFirstExtraTime());
        }

        public int GetGoalsFirstExtraTimeHome()
        {
            return GetHomeGames().Sum(x => x.GetGoalsFirstExtraTime());
        }

        public int GetGoalsFirstHalfAway()
        {
            return GetAwayGames().Sum(x => x.GetGoalsFirstHalf());
        }

        public int GetGoalsFirstHalfBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetGoalsFirstHalf());
        }

        public int GetGoalsFirstHalfHome()
        {
            return GetHomeGames().Sum(x => x.GetGoalsFirstHalf());
        }

        public int GetGoalsFirstTenMinutesAway()
        {
            return GetAwayGames().Sum(x => x.GetGoalsFirstTenMinutes());
        }

        public int GetGoalsFirstTenMinutesBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetGoalsFirstTenMinutes());
        }

        public int GetGoalsFirstTenMinutesHome()
        {
            return GetHomeGames().Sum(x => x.GetGoalsFirstTenMinutes());
        }

        public int GetGoalsSecondExtraTimeAway()
        {
            return GetAwayGames().Sum(x => x.GetGoalsSecondExtraTime());
        }

        public int GetGoalsSecondExtraTimeBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetGoalsSecondExtraTime());
        }

        public int GetGoalsSecondExtraTimeHome()
        {
            return GetHomeGames().Sum(x => x.GetGoalsSecondExtraTime());
        }

        public int GetGoalsSecondHalfAway()
        {
            return GetAwayGames().Sum(x => x.GetGoalsSecondHalf());
        }

        public int GetGoalsSecondHalfBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetGoalsSecondHalf());
        }

        public int GetGoalsSecondHalfHome()
        {
            return GetHomeGames().Sum(x => x.GetGoalsSecondHalf());
        }

        #endregion " GOALS BY TIME "

        #region " GREATER AND LOWER SCORES GAMES "

        public Game GetGreaterAwayScore()
        {
            return GetAwayGames().First(x => x.GetSumScore() == GetMaximumGoalsAway());
        }

        public Game GetGreaterBetweenScore()
        {
            var game = GetGamesBetweenTeams().FirstOrDefault(x => x.GetSumScore() == GetMaximumGoalsBetween());
            if (games == null || !games.Any())
            {
                return game;
            }

            return new Game();
        }

        public Game GetGreaterHomeScore()
        {
            return GetHomeGames().First(x => x.GetSumScore() == GetMaximumGoalsHome());
        }

        public Game GetLowerAwayScore()
        {
            return GetAwayGames().First(x => x.GetSumScore() == GetMinimumGoalsAway());
        }

        public Game GetLowerBetweenScore()
        {
            var games = GetGamesBetweenTeams();
            if (games == null || !games.Any())
            {
                return new Game();
            }

            return games.First(x => x.GetSumScore() == GetMinimumGoalsBetween());
        }

        public Game GetLowerHomeScore()
        {
            return GetHomeGames().First(x => x.GetSumScore() == GetMinimumGoalsHome());
        }

        #endregion " GREATER AND LOWER SCORES GAMES "

        #region " SCORES "

        public double GetAverageGoals()
        {
            return games.Average(x => x.GetSumScore()).ToDecimalFormat();
        }

        public double GetAverageGoalsAway()
        {
            return (GetAwayGamesGoals() / GetHomeGames().Count()).ToDecimalFormat();
        }

        public double GetAverageGoalsBetween()
        {
            return (GetGoalsBetween() / GetGamesBetweenTeams().Count()).ToDecimalFormat();
        }

        public double GetAverageGoalsHome()
        {
            return (GetHomeGamesGoals() / GetHomeGames().Count()).ToDecimalFormat();
        }

        public double GetAwayGamesGC()
        {
            return GetAwayGames().Sum(x => x.GetOtherTeam(mainGame.awayCompetitor.id).score);
        }

        public double GetAwayGamesGoals()
        {
            return GetAwayGames().Sum(x => x.GetSumScore());
        }

        public double GetAwayGamesGP()
        {
            return GetAwayGames().Sum(x => x.GetTeam(mainGame.awayCompetitor.id).score);
        }

        public double GetGoalsAwayBetween()
        {
            var games = GetGamesBetweenTeams();
            if (games == null || !games.Any())
            {
                return 0;
            }

            return games.Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + games.Where(x => x.awayCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetGoalsBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.GetSumScore());
        }

        public double GetGoalsHomeBetween()
        {
            var games = GetGamesBetweenTeams();
            if (games == null || !games.Any())
            {
                return 0;
            }

            return games.Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + games.Where(x => x.awayCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetHomeGamesGC()
        {
            return GetHomeGames().Sum(x => x.GetOtherTeam(mainGame.homeCompetitor.id).score);
        }

        public double GetHomeGamesGoals()
        {
            return GetHomeGames().Sum(x => x.GetSumScore());
        }

        public double GetHomeGamesGP()
        {
            return GetHomeGames().Sum(x => x.GetTeam(mainGame.homeCompetitor.id).score);
        }

        public double GetMaximumGoals()
        {
            return FixGamesWithoutScore(games).Max(x => x.GetSumScore());
        }

        public double GetMaximumGoalsAway()
        {
            return GetAwayGames().Max(x => x.GetSumScore());
        }

        public double GetMaximumGoalsBetween()
        {
            var games = GetGamesBetweenTeams();
            return games.Any() ? games.Max(x => x.GetSumScore()) : 0;
        }

        public double GetMaximumGoalsHome()
        {
            return GetHomeGames().Max(x => x.GetSumScore());
        }

        public double GetMinimumGoals()
        {
            return FixGamesWithoutScore(games).Min(x => x.GetSumScore());
        }

        public double GetMinimumGoalsAway()
        {
            return GetAwayGames().Min(x => x.GetSumScore());
        }

        public double GetMinimumGoalsBetween()
        {
            var games = GetGamesBetweenTeams();
            return games.Any() ? games.Min(x => x.GetSumScore()) : 0;
        }

        public double GetMinimumGoalsHome()
        {
            return GetHomeGames().Min(x => x.GetSumScore());
        }

        #endregion " SCORES "

        #region " BETS "

        public double GetAverageGoalsFirstHalf()
        {
            return games.Average(x => x.GetGoalsFirstHalf()).ToDecimalFormat();
        }

        public double GetAverageGoalsSecondHalf()
        {
            return games.Average(x => x.GetGoalsSecondHalf()).ToDecimalFormat();
        }

        public int GetCornersAverage()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            if (gamesBetweenTeams != null
                    && gamesBetweenTeams.Count > 0)
            {
                var gamesWithStatistics = gamesBetweenTeams.Where(x => x.awayCompetitor.statistics != null);
                if (gamesWithStatistics != null)
                {
                    var statistics = gamesWithStatistics.SelectMany(x => x.awayCompetitor.statistics).ToList();
                    statistics.AddRange(gamesWithStatistics.SelectMany(x => x.homeCompetitor.statistics).ToList());
                    var cornersTotal = statistics.Where(x => x.id == CORNER_ID).Sum(x => Convert.ToInt32(x.value));
                    return cornersTotal / gamesBetweenTeams.Count;
                }
            }

            return 0;
        }

        public double GetPercentBothDidntScore()
        {
            return MathUtils.CalcPercent(games.Where(x => !x.DidBothTeamScore()).Count(), games.Count);
        }

        public double GetPercentBothScored()
        {
            return MathUtils.CalcPercent(games.Where(x => x.DidBothTeamScore()).Count(), games.Count);
        }

        #endregion " BETS "

        private List<Game> FixGamesWithoutScore(List<Game> games)
        {
            if (games == null)
            {
                return new List<Game>();
            }

            return games.Where(x => x.GetSumScore() != -2).ToList();
        }
    }
}