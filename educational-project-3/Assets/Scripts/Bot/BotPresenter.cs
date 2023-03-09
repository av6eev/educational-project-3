using System.Collections.Generic;
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
            
            if (!floorModel.Cells[floorModel.FirstStartPosition].IsActive)
            {
                CreatePlayer(playerId, floorModel, floorModel.FirstStartPosition, 45);
            }
            else
            {
                CreatePlayer(playerId, floorModel, floorModel.SecondStartPosition, -135);
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
            
            model.Cells[basePosition].IsActive = true;
        }

        private void RemovePlayer(string playerId)
        {
            if (_model.ActiveUsers.Count == 0 && !_model.ActiveUsers.ContainsKey(playerId)) return;
            
            _manager.FloorModel.Cells[_model.ActiveUsers[playerId].Position].IsActive = false;
            _model.ActiveUsers[playerId].RemovePlayer();
            
            _playerPresenters[playerId].Deactivate();
            _playerPresenters.Remove(playerId);
        }
    }
}