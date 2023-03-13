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
            _model.OnPlayerLeft -= RemovePlayer;
            _model.OnGameStarting -= StartGame;
        }

        public void Activate()
        {
            _model.OnPlayerEntered += PlayerEntered;
            _model.OnPlayerLeft += RemovePlayer;
            _model.OnGameStarting += StartGame;
        }

        private void StartGame()
        {
            var presenter = new GamePresenter(_manager.GameModel, _manager.GameView, _manager);
            presenter.Activate();
            
            _manager.GameModel.StartGame(_model.ActiveUsers);
        }

        private void PlayerEntered(string playerId)
        {
            var floorModel = _manager.FloorModel;

            foreach (var cell in _manager.FloorModel.Cells.Where(cell => cell.Position == floorModel.FirstStartPosition))
            {
                CreatePlayer(playerId, floorModel, !cell.IsActive ? floorModel.FirstStartPosition : floorModel.SecondStartPosition, 45);
            }
        }

        private void CreatePlayer(string playerId, FloorModel model, Vector3 basePosition, float angle)
        {
            var playerModel = _model.ActiveUsers[playerId];
            var position = new Vector3(basePosition.x, 0.29f, basePosition.z);
            var presenter = new PlayerPresenter(playerModel, _manager, _manager.GameView.InstantiatePlayer(playerId, position, angle));
            
            _playerPresenters.Add(playerModel.Id, presenter);
            presenter.Activate();
            
            playerModel.CreatePlayer(position, new Vector3(0, angle, 0));
            
            foreach (var cell in _manager.FloorModel.Cells.Where(cell => cell.Position == basePosition))
            {
                cell.IsActive = true;
            }
        }

        private void RemovePlayer(string playerId)
        {
            if (_model.ActiveUsers.Count == 0 && !_model.ActiveUsers.ContainsKey(playerId)) return;
            
            foreach (var cell in _manager.FloorModel.Cells.Where(cell => cell.Position == _model.ActiveUsers[playerId].Position))
            {
                cell.IsActive = false;
            }

            _model.ActiveUsers[playerId].RemovePlayer();
            
            _playerPresenters[playerId].Deactivate();
            _playerPresenters.Remove(playerId);
        }
    }
}