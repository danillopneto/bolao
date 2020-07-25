using Bolao.Pinheiros.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

                return TempData.Peek(GAMES_DATA) as Root;
            }

            set
            {
                TempData[GAMES_DATA] = value;
            }
        }

        public ActionResult GetGamesData(DateTime date)
        {
            var model = GetDataFromGames(date);
            GamesData = model;
            return PartialView("_Games", model);

        }

        [HttpPost]
        public ActionResult GetStatistics(int gameId)
        {
            var game = GamesData.games.First(x => x.id == gameId);
            var gameData = GetGameData(game);
            var previousMeetings = GetPreviousMeetings(gameData.game.previousMeetings);
            previousMeetings.mainGame = game;

            return PartialView("_Statistics", previousMeetings);
        }

        public ActionResult Index()
        {
            var date = DateTime.Now;
            var model = GetDataFromGames(date);
            GamesData = model;
            return View(model);
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

        private Root GetPreviousMeetings(List<int> previousMeetings)
        {
            previousMeetings = previousMeetings.OrderByDescending(x => x).ToList().Take(MAXIMUM_GAMES).ToList();
            var url = string.Format(URL_BASE_GAMES, string.Join(",", previousMeetings), DateTime.Now.ToString(DATE_FORMAT));
            var previousMeetingsData = GetDataFromApi<Root>(url);
            return previousMeetingsData;
        }

        private void InsertStatistics(Root model)
        {
            foreach (var game in model.games)
            {
                GetGameData(game);
            }
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
    }
}