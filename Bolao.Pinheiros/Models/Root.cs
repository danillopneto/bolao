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

        public IEnumerable<Game> GetAwayWinnings()
        {
            return games.Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.awayCompetitor.id);
        }

        public IEnumerable<Game> GetDraws()
        {
            return games.Where(x => x.IsDraw());
        }

        public double GetGoals()
        {
            return games.Sum(x => x.homeCompetitor.score + x.awayCompetitor.score);
        }

        public double GetGoalsAway()
        {
            return games.Where(x => x.homeCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + games.Where(x => x.awayCompetitor.id == mainGame.awayCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public double GetGoalsHome()
        {
            return games.Where(x => x.homeCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.homeCompetitor.score)
                    + games.Where(x => x.awayCompetitor.id == mainGame.homeCompetitor.id).Sum(x => x.awayCompetitor.score);
        }

        public IEnumerable<Game> GetHomeWinnings()
        {
            return games.Where(x => x.GetWinner() != null && x.GetWinner().id == mainGame.homeCompetitor.id);
        }

        public double GetMaximumScores()
        {
            return games.Max(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }

        public double GetMinimumScores()
        {
            return games.Min(x => x.awayCompetitor.score + x.homeCompetitor.score);
        }
    }
}