using System.Collections.Generic;
using System.Linq;

namespace Bolao.Pinheiros.Models
{
    public class Root
    {
        private List<Game> _gamesBetweenTeams;

        public List<Bookmaker> bookmakers { get; set; }
        public List<Competition> competitions { get; set; }
        public List<GameCompetitor> competitors { get; set; }
        public List<Country> countries { get; set; }
        public List<Game> games { get; set; }
        public long lastUpdateId { get; set; }
        public int liveGamesCount { get; set; }
        public Game mainGame { get; set; }
        public int requestedUpdateId { get; set; }
        public List<Sport> sports { get; set; }
        public int ttl { get; set; }

        #region " AWAY TEAM "

        public IEnumerable<Game> GetAwayDefeats()
        {
            return GetAwayGames().Where(x => x.GetWinner() != null && x.GetWinner().id != mainGame.awayCompetitor.id);
        }

        public IEnumerable<GameCompetitor> GetAwayDefeatsTeams()
        {
            return GetAwayDefeats().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        public IEnumerable<Game> GetAwayDraws()
        {
            return GetAwayGames().Where(x => x.IsDraw());
        }

        public IEnumerable<GameCompetitor> GetAwayDrawsTeams()
        {
            return GetAwayDraws().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        public List<Game> GetAwayGames()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            var awayGames = games.Where(x => gamesBetweenTeams.All(g => g.id != x.id)
                                            && x.IsTeamInGame(mainGame.awayCompetitor.id)).ToList();
            return awayGames;
        }

        public IEnumerable<Game> GetAwayWinnings()
        {
            return GetAwayGames().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<Game> GetAwayWinningsBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<GameCompetitor> GetAwayWinningsTeams()
        {
            return GetAwayWinnings().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        #endregion " AWAY TEAM "

        #region " HOME TEAM "

        public IEnumerable<Game> GetHomeDefeats()
        {
            return GetHomeGames().Where(x => x.GetWinner() != null && x.GetWinner().id != mainGame.homeCompetitor.id);
        }

        public IEnumerable<GameCompetitor> GetHomeDefeatsTeams()
        {
            return GetHomeDefeats().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.awayCompetitor.id)).ToList();
        }

        public IEnumerable<Game> GetHomeDraws()
        {
            return GetHomeGames().Where(x => x.IsDraw());
        }

        public IEnumerable<GameCompetitor> GetHomeDrawsTeams()
        {
            return GetHomeDraws().OrderByDescending(x => x.id).Select(x => x.GetOtherTeam(mainGame.homeCompetitor.id)).ToList();
        }

        public List<Game> GetHomeGames()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            var homeGames = games.Where(x => gamesBetweenTeams.All(g => g.id != x.id)
                                            && x.IsTeamInGame(mainGame.homeCompetitor.id)).ToList();
            return homeGames;
        }

        public IEnumerable<Game> GetHomeWinnings()
        {
            return GetHomeGames().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public IEnumerable<Game> GetHomeWinningsBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public IEnumerable<GameCompetitor> GetHomeWinningsTeams()
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
                _gamesBetweenTeams = games.Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id
                                                       && x.awayCompetitor.id == mainGame.awayCompetitor.id).ToList();
                _gamesBetweenTeams.AddRange(games.Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id
                                                        && x.awayCompetitor.id == mainGame.homeCompetitor.id).ToList());
            }

            return _gamesBetweenTeams;
        }

        #region " SCORES "

        public double GetAverageGoalsAway()
        {
            return GetAwayGamesGoals() / GetHomeGames().Count();
        }

        public double GetAverageGoalsBetween()
        {
            return GetGoalsBetween() / GetGamesBetweenTeams().Count();
        }

        public double GetAverageGoalsHome()
        {
            return GetHomeGamesGoals() / GetHomeGames().Count();
        }

        public double GetAwayGamesGoals()
        {
            return GetAwayGames().Sum(x => x.homeCompetitor.score + x.awayCompetitor.score);
        }

        public double GetGoalsAwayBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + GetGamesBetweenTeams().Where(x => x.awayCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetGoalsBetween()
        {
            return GetGamesBetweenTeams().Sum(x => x.homeCompetitor.score + x.awayCompetitor.score);
        }

        public double GetGoalsHomeBetween()
        {
            return GetGamesBetweenTeams().Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + GetGamesBetweenTeams().Where(x => x.awayCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetHomeGamesGoals()
        {
            return GetHomeGames().Sum(x => x.homeCompetitor.score + x.awayCompetitor.score);
        }

        public double GetMaximumGoalsBetween()
        {
            return GetGamesBetweenTeams().Max(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMaximumScoresAway()
        {
            return GetAwayGames().Max(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMaximumScoresHome()
        {
            return GetHomeGames().Max(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMinimumGoalsBetween()
        {
            return GetGamesBetweenTeams().Min(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMinimumScoresAway()
        {
            return GetAwayGames().Min(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMinimumScoresHome()
        {
            return GetHomeGames().Min(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        #endregion " SCORES "
    }
}