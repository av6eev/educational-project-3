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
        }

        public void Activate()
        {
            _model.OnPlayerEntered += PlayerEntered;
            _model.OnPlayerLeft += RemovePlayer;
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

        private void CreatePlayer(string playerId, FloorModel model, Vector3 position, float angle)
        {
            var playerModel = _model.ActiveUsers[playerId];
            var presenter = new PlayerPresenter(playerModel, _manager, _manager.GameView.InstantiatePlayer(position, angle));
            
            _playerPresenters.Add(playerModel.Id, presenter);
            presenter.Activate();
            
            playerModel.CreatePlayer(position);
            
            model.Cells[position].IsActive = true;
        }

        private void RemovePlayer(string playerId)
        {
            if (_model.ActiveUsers.Count == 0 && !_model.ActiveUsers.ContainsKey(playerId)) return;
            
            _playerPresenters[playerId].Deactivate();
            _playerPresenters.Remove(playerId);
            
            _manager.FloorModel.Cells[_model.ActiveUsers[playerId].Position].IsActive = false;
            _model.ActiveUsers[playerId].RemovePlayer();
        }
    }
}