using ButterCheeseEggs.Models;
using ButterCheeseEggs.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ButterCheeseEggs.Controllers
{
    public class GameController : Controller
    {
        private const string GameStateSessionId = "GameState";

        private readonly ILogger<GameController> _logger;

        private readonly IGameService gameService;

        public GameController(ILogger<GameController> logger, IGameService gameService)
        {
            _logger = logger;
            this.gameService = gameService;
        }


        public IActionResult Play()
        {
            GameState state = GetGameState();

            return View(state);
        }


        public IActionResult Reset()
        {
            GameState state = new GameState();

            SaveGameState(state);

            return RedirectToAction(nameof(Play));
        }


        public IActionResult MakeMove(int x, int y)
        {
            GameState state = GetGameState();

            gameService.MakeNextMove(state, x, y);

            SaveGameState(state);

            return RedirectToAction(nameof(Play));
        }


        private GameState GetFakeGameState()
        {
            GameState state = new GameState();

            state.NextStepBy = Players.O;
            state.Winner = Players.None;

            state.Table[0, 0] = TileStates.X;
            state.Table[1, 0] = TileStates.O;
            state.Table[2, 2] = TileStates.X;
            state.Table[1, 1] = TileStates.O;

            return state;
        }


        public GameState GetGameState()
        {
            GameState state;
            string? stateJson = HttpContext.Session?.GetString(GameStateSessionId);

            if (stateJson == null)
            {
                // ToDo: This is for testing only. Replace this by new GameState();
                state = GetFakeGameState();
                HttpContext.Session?.SetString(GameStateSessionId, state.ToJson());
            }
            else
            {
                state = GameState.FromJson(stateJson);
            }

            return state;
        }


        public void SaveGameState(GameState state)
        {
            if (state == null) { throw new ArgumentNullException("state"); }

            string json = state.ToJson();

            HttpContext.Session?.SetString(GameStateSessionId, json);
        }

    }
}
