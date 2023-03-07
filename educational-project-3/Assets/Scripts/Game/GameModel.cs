using System;
using System.Collections.Generic;
using Player;

namespace Game
{
    public class GameModel
    {
        public event Action OnGameStarted;
        public readonly List<PlayerModel> Players = new();

        public GameStage GameStage;

        public GameModel()
        {
            GameStage = GameStage.Preparing;
        }

        public void StartGame(Dictionary<string, PlayerModel> players)
        {
            foreach (var player in players.Values)
            {
                Players.Add(player);
            }
            
            OnGameStarted?.Invoke();
        }
    }
}