using Game;
using UnityEngine;
using Utilities;

namespace Bot
{
    public class BotPresenter : IPresenter
    {
        private readonly BotModel _model;
        private readonly GameManager _manager;

        public BotPresenter(BotModel model, GameManager manager)
        {
            _model = model;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            _model.OnPlayerStarted -= CreatePlayer;
            _model.OnPlayerLeft -= RemovePlayer;
        }

        public void Activate()
        {
            _model.OnPlayerStarted += CreatePlayer;
            _model.OnPlayerLeft += RemovePlayer;
        }

        private void CreatePlayer(string playerId)
        {
            if (!_manager.FloorModel.Cells[_manager.FloorModel.FirstStartPosition].IsActive)
            {
                _model.ActiveUsers[playerId].CreatePlayer(playerId, _manager.FloorModel.FirstStartPosition, 45);
                _manager.FloorModel.Cells[_manager.FloorModel.FirstStartPosition].IsActive = true;
            }
            else
            {
                _model.ActiveUsers[playerId].CreatePlayer(playerId, _manager.FloorModel.SecondStartPosition, -135);
                _manager.FloorModel.Cells[_manager.FloorModel.SecondStartPosition].IsActive = true;
            }    
        }
        
        private void RemovePlayer(string playerId)
        {
            if (_model.ActiveUsers.Count == 0 && !_model.ActiveUsers.ContainsKey(playerId)) return;

            _manager.FloorModel.Cells[_model.ActiveUsers[playerId].Position].IsActive = false;
            _model.ActiveUsers[playerId].RemovePlayer(playerId);
        }
    }
}