using System;
using System.Collections.Generic;
using Player;

namespace Game
{
    public class GameModel
    {
        public event Action OnGameStarted;
        public event Action OnTurnChanged;
        
        private readonly List<PlayerModel> _players = new();
        public string ChannelId { get; private set; } 

        public GameStage GameStage;
        
        public int Turn = 0;
        public int TurnTime;

        public PlayerModel ActivePlayer => Turn % 2 == 0 ? _players[0] : _players[1];

        public GameModel()
        {
            GameStage = GameStage.Preparing;
        }

        public void StartGame(Dictionary<string, PlayerModel> players, string channelId)
        {
            ChannelId = channelId;
            
            foreach (var player in players.Values)
            {
                _players.Add(player);
            }
            
            OnGameStarted?.Invoke();
        }

        public void ChangeTurn()
        {
            OnTurnChanged?.Invoke();
        }
    }
}