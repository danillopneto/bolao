using System.Collections.Generic;
using System.Linq;

namespace Bolao.Pinheiros.Models
{
    public class Root
    {
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

        public double GetAverageGoals()
        {
            return GetGoals() / GetGamesBetweenTeams().Count();
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
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<Game> GetDraws()
        {
            return GetGamesBetweenTeams().Where(x => x.IsDraw());
        }

        public List<Game> GetGamesBetweenTeams()
        {
            var gamesBetweenTeams = games.Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id
                                                    && x.awayCompetitor.id == mainGame.awayCompetitor.id).ToList();
            gamesBetweenTeams.AddRange(games.Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id
                                                    && x.awayCompetitor.id == mainGame.homeCompetitor.id).ToList());
            return gamesBetweenTeams;
        }

        public List<Game> GetHomeGames()
        {
            var gamesBetweenTeams = GetGamesBetweenTeams();
            var homeGames = games.Where(x => gamesBetweenTeams.All(g => g.id != x.id)
                                            && x.IsTeamInGame(mainGame.homeCompetitor.id)).ToList();
            return homeGames;
        }

        public double GetGoals()
        {
            return GetGamesBetweenTeams().Sum(x => x.homeCompetitor.score + x.awayCompetitor.score);
        }

        public double GetGoalsAway()
        {
            return GetGamesBetweenTeams().Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + GetGamesBetweenTeams().Where(x => x.awayCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetGoalsHome()
        {
            return GetGamesBetweenTeams().Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + GetGamesBetweenTeams().Where(x => x.awayCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public IEnumerable<Game> GetHomeWinnings()
        {
            return GetGamesBetweenTeams().Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public double GetMaximumScores()
        {
            return GetGamesBetweenTeams().Max(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMinimumScores()
        {
            return GetGamesBetweenTeams().Min(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }
    }
}