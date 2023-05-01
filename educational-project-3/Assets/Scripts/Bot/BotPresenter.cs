using System.Collections.Generic;
using System.Linq;
using Game;
using Player;
using UnityEngine;
using Utilities;

namespace Bot
{
    public class BotPresenter : IPresenter
    {
        private readonly BotModel _model;
        private readonly GameManager _manager;
        private readonly Dictionary<string, PlayerPresenter> _playerPresenters = new();

        public BotPresenter(BotModel model, GameManager manager)
        {
            _model = model;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            _model.OnPlayersEntered -= PlayersEntered;
            _model.OnPlayerDeselectTeam -= RemoveActivePlayer;
            _model.OnGameStarting -= StartGame;
        }

        public void Activate()
        {
            _model.OnPlayersEntered += PlayersEntered;
            _model.OnPlayerDeselectTeam += RemoveActivePlayer;
            _model.OnGameStarting += StartGame;
        }

        private void StartGame(string channelId)
        {
            var presenter = new GamePresenter(_manager.GameModel, _manager.GameView, _manager);
            presenter.Activate();
            
            _manager.GameModel.StartGame(_model.ActiveUsers, channelId);
        }

        private void PlayersEntered()
        {
            var players = _model.ActiveUsers.Values.ToList();
            var floorModel = _manager.FloorModel;
            const float yOffset = 0.25f;

            for (var i = 0; i < _model.ActiveUsers.Count; i++)
            {
                var position = Vector3.zero;
                var angle = 0f;
                
                switch (i)
                {
                    case 0:
                        position = new Vector3(floorModel.FirstStartPosition.x, yOffset, floorModel.FirstStartPosition.z);
                        angle = 0;
                        break;
                    case 1:
                        position = new Vector3(floorModel.SecondStartPosition.x, yOffset, floorModel.SecondStartPosition.z);
                        angle = -180;
                        break;
                }
                
                var presenter = new PlayerPresenter(players[i], _manager, _manager.GameView.InstantiatePlayer(players[i].Id, position, angle));
                presenter.Activate();
                _playerPresenters.Add(players[i].Id, presenter);
            
                players[i].CreatePlayer(position);

                floorModel.Cells[new Vector3(position.x, 0, position.z)].IsActive = true;
            }
        }

        private void RemoveActivePlayer(string playerId)
        {
            if (_model.ActiveUsers.Count == 0 && !_model.ActiveUsers.ContainsKey(playerId)) return;
            
            foreach (var cell in _manager.FloorModel.Cells.Values.Where(cell => cell.Position == _model.ActiveUsers[playerId].Position))
            {
                cell.IsActive = false;
            }

            _model.ActiveUsers[playerId].RemovePlayer();
        }
    }
}