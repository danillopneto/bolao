using Bolao.Pinheiros.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Mvc;

namespace Bolao.Pinheiros.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string DATE_FORMAT = "dd/MM/yyyy";
        private static readonly string GAMES_DATA = "GamesData";
        private static readonly int MAXIMUM_GAMES = 5;
        private static readonly string URL_BASE = "https://webws.365scores.com/web/games/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&sports=1&startDate={0}&endDate={1}&showOdds=true";
        private static readonly string URL_BASE_GAME = "https://webws.365scores.com/web/game/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&gameId={0}";
        private static readonly string URL_BASE_GAMES = "https://webws.365scores.com/web/games/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&games={0}&startDate=01/01/2000&endDate={1}";

        private Root GamesData
        {
            get
            {
                if (!TempData.ContainsKey(GAMES_DATA))
                {
                    TempData[GAMES_DATA] = new Root();
                }

                TempData.Keep(GAMES_DATA);
                return TempData.Peek(GAMES_DATA) as Root;
            }

            set
            {
                TempData[GAMES_DATA] = value;
            }
        }

        [HttpPost]
        public ActionResult GetGamesData(DateTime date)
        {
            var model = GetDataFromGames(date);
            model.Date = date;

            GamesData = model;
            return PartialView("_Games", model);
        }

        [HttpPost]
        public ActionResult GetGamesDataUpdate()
        {
            var model = GetDataFromGames(GamesData.Date);
            model.Date = GamesData.Date;

            GamesData = model;
            return Json(model);
        }

        [HttpPost]
        public ActionResult GetStatistics(int gameId)
        {
            var game = GamesData.games.First(x => x.id == gameId);
            var gameData = GetGameData(game);

            var recentGames = new List<int>();
            recentGames.AddRange(gameData.game.previousMeetings.Take(MAXIMUM_GAMES));
            recentGames.AddRange(gameData.game.homeCompetitor.recentMatches.Take(MAXIMUM_GAMES));
            recentGames.AddRange(gameData.game.awayCompetitor.recentMatches.Take(MAXIMUM_GAMES));

            recentGames = recentGames.Distinct().ToList();
            recentGames = recentGames.Where(x => x != gameId).ToList();

            var gameStatistics = GetGamesData(recentGames);
            gameStatistics.mainGame = game;

            return PartialView("_Statistics", gameStatistics);
        }

        public ActionResult Index()
        {
            var date = DateTime.Now;
            var model = GetDataFromGames(date);
            model.Date = date;

            GamesData = model;
            return View(model);
        }

        private T GetDataFromApi<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<T>(json);
                    if (data != null)
                    {
                        return data;
                    }
                }
            }

            return default(T);
        }

        private Root GetDataFromGames(DateTime date)
        {
            var startDate = date.ToString(DATE_FORMAT);
            var endDate = date.ToString(DATE_FORMAT);
            var url = string.Format(URL_BASE, startDate, endDate);

            var model = GetDataFromApi<Root>(url);
            return model;
        }

        private GameData GetGameData(Game game)
        {
            if (game == null)
            {
                return null;
            }

            var url = string.Format(URL_BASE_GAME, game.id);
            return GetDataFromApi<GameData>(url);
        }

        private Root GetGamesData(List<int> gamesIds)
        {
            gamesIds = gamesIds.OrderByDescending(x => x).ToList();
            var gamesData = new Root { games = new List<Game>() };
            var tarefas = new List<Thread>();

            foreach (var gameId in gamesIds)
            {
                var thread = new Thread(() => InsertGameData(gamesData, gameId));
                thread.Start();
                tarefas.Add(thread);
            }

            while (tarefas.Any(x => x.IsAlive))
            {
                Thread.Sleep(500);
            }

            return gamesData;
        }

        private void InsertGameData(Root data, double gameId)
        {
            var url = string.Format(URL_BASE_GAME, gameId);
            var gameData = GetDataFromApi<GameData>(url);
            data.games.Add(gameData.game);
        }
    }
}