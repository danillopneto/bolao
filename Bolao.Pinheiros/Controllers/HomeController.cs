﻿using Bolao.Pinheiros.Models;
using Bolao.Pinheiros.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Mvc;

namespace Bolao.Pinheiros.Controllers
{
    public class HomeController : SamlController
    {
        #region " CONSTANTS "

        private static readonly string DATE_FORMAT = "dd/MM/yyyy";
        private static readonly List<int> EXCLUDE_COMPETITIONS = new List<int> { 146, 165, 173, 397, 564, 5431, 5456, 5462, 5547, 5565, 5556, 5605, 5606, 5639, 5807, 5818, 5820, 5825, 6108, 6148, 6219, 6221, 6252, 6345, 6347, 6362, 6896, 7110, 7298, 7299, 7345, 7466, 7499, 7522, 7523, 7528, 7529, 7530, 7538, 7540, 7556, 7568, 7569, 7570, 7571, 7572, 7573, 7574 };
        private static readonly string GAMES_DATA = "GamesData";
        private static readonly int MAXIMUM_GAMES = 5;
        private static readonly string URL_BASE = "https://webws.365scores.com/web/games/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&sports=1&startDate={0}&endDate={1}&showOdds=true";
        private static readonly string URL_BASE_COMPETITIONS = "https://webws.365scores.com/web/standings/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&competitions=";
        private static readonly string URL_BASE_GAME = "https://webws.365scores.com/web/game/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&gameId={0}";
        private static readonly string URL_BASE_GAMES = "https://webws.365scores.com/web/games/?langId=31&timezoneName=America/Sao_Paulo&userCountryId=21&appTypeId=5&games={0}&startDate=01/01/2000&endDate={1}";

        #endregion " CONSTANTS "

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

        #region " VIEWS "

        public ActionResult Index()
        {
//#if !DEBUG
//            CheckOrDoLogin();
//#endif
            var date = DateTime.Now.ToBrasiliaDateTime();
            var model = GetDataFromGames(date, false);
            model.Date = date;
            model.standings = GetCompetitionsData(model.games);

            GamesData = model;
            return View(model);
        }

        #endregion " VIEWS "

        #region " ACTION RESULTS "

        [HttpPost]
        public ActionResult GetGamesData(DateTime date)
        {
            var model = GetDataFromGames(date, false);
            model.Date = date;
            model.standings = GetCompetitionsData(model.games);

            GamesData = model;
            return PartialView("_Games", model);
        }

        [HttpPost]
        public ActionResult GetGamesDataUpdate()
        {
            var model = GetDataFromGames(GamesData.Date, true);
            return Json(model);
        }

        [HttpPost]
        public ActionResult GetStatistics(int gameId)
        {
            var gameData = GetGameData(gameId);

            var recentGames = new List<int>();
            recentGames.AddRange(gameData.game.previousMeetings.Where(x => x != gameId).Take(MAXIMUM_GAMES));
            recentGames.AddRange(gameData.game.homeCompetitor.recentMatches.Where(x => x != gameId).Take(MAXIMUM_GAMES));
            recentGames.AddRange(gameData.game.awayCompetitor.recentMatches.Where(x => x != gameId).Take(MAXIMUM_GAMES));

            recentGames = recentGames.Distinct().ToList();

            var gameStatistics = GetGamesData(recentGames);
            gameStatistics.mainGame = gameData.game;

            return PartialView("_Statistics", gameStatistics);
        }

        #endregion " ACTION RESULTS "

        #region " PRIVATE METHODS "

        private List<Standing> GetCompetitionsData(List<Game> games)
        {
            var competitions = games.Select(x => x.competitionId).Distinct();
            var url = string.Concat(URL_BASE_COMPETITIONS, string.Join(",", competitions));
            var standings = GetDataFromApi<Root>(url);
            return standings.standings;
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

        private Root GetDataFromGames(DateTime date, bool onlyLiveGames)
        {
            var startDate = date.ToString(DATE_FORMAT);
            var endDate = date.ToString(DATE_FORMAT);
            var url = string.Format(URL_BASE, startDate, endDate);
            if (onlyLiveGames)
            {
                url = string.Concat(url, "&onlyLiveGames=true");
            }

            var model = GetDataFromApi<Root>(url);
            if (model.games != null && model.games.Any())
            {
                model.games = model.games.Where(x => !EXCLUDE_COMPETITIONS.Contains(x.competitionId)).ToList();
                model.games = model.games.OrderBy(x => x.startTime).ToList();
            }

            return model;
        }

        private GameData GetGameData(int gameId)
        {
            var url = string.Format(URL_BASE_GAME, gameId);
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

        #endregion " PRIVATE METHODS "
    }
}