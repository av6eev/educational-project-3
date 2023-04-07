using System.Collections.Generic;
using System.Linq;
using Floor;
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
            _model.OnPlayerEntered -= PlayerEntered;
            _model.OnPlayerDeselectTeam -= RemoveActivePlayer;
            _model.OnGameStarting -= StartGame;
        }

        public void Activate()
        {
            _model.OnPlayerEntered += PlayerEntered;
            _model.OnPlayerDeselectTeam += RemoveActivePlayer;
            _model.OnGameStarting += StartGame;
        }

        private void StartGame(string channelId)
        {
            var presenter = new GamePresenter(_manager.GameModel, _manager.GameView, _manager);
            presenter.Activate();
            
            _manager.GameModel.StartGame(_model.ActiveUsers, channelId);
        }

        private void PlayerEntered(string playerId)
        {
            var floorModel = _manager.FloorModel;

            foreach (var cell in _manager.FloorModel.Cells.Values.Where(cell => cell.Position == floorModel.FirstStartPosition))
            {
                CreatePlayer(playerId, !cell.IsActive ? floorModel.FirstStartPosition : floorModel.SecondStartPosition, !cell.IsActive ? 45 : -135);
            }
        }

        private void CreatePlayer(string playerId, Vector3 basePosition, float angle)
        {
            var playerModel = _model.ActiveUsers[playerId];
            var position = new Vector3(basePosition.x, 0.29f, basePosition.z);
            var presenter = new PlayerPresenter(playerModel, _manager, _manager.GameView.InstantiatePlayer(playerId, position, angle));
            
            presenter.Activate();
            _playerPresenters.Add(playerId, presenter);
            
            playerModel.CreatePlayer(position, new Vector3(0, angle, 0));
            
            foreach (var cell in _manager.FloorModel.Cells.Values.Where(cell => cell.Position == basePosition))
            {
                cell.IsActive = true;
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