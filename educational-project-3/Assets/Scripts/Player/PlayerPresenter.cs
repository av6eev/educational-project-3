using System.Threading.Tasks;
using Game;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerPresenter : IPresenter
    {
        private readonly PlayerModel _model;
        private readonly GameManager _manager;
        private readonly PlayerView _view;

        public PlayerPresenter(PlayerModel model, GameManager manager, PlayerView view)
        {
            _model = model;
            _manager = manager;
            _view = view;
        }

        public void Deactivate()
        {
            _model.OnPlayerCreated -= InitPlayer;
            _model.OnPlayerRemoved -= DestroyPlayer;
            _model.OnPlayerMove -= MovePlayer;
        }

        public void Activate()
        {
            _view.Disable();
            
            _model.OnPlayerCreated += InitPlayer;
            _model.OnPlayerRemoved += DestroyPlayer;
            _model.OnPlayerMove += MovePlayer;
        }

        private async void MovePlayer(Vector3 direction)
        {
            _manager.FloorModel.Cells[new Vector3(_model.Position.x, 0, _model.Position.z)].IsActive = false;
            
            var position = _model.Position + direction;
            
            _manager.FloorModel.Cells[new Vector3(position.x, 0, position.z)].IsActive = true;

            await Task.Delay(1000);
            
            _view.transform.position += _model.Direction;
            _model.Direction = Vector3.zero;
            
            _manager.GameModel.ChangeTurn();
        }

        private void InitPlayer()
        {
            _view.Text.text = _model.Name;
            _view.Enable();
        }
        
        private void DestroyPlayer()
        {
            _view.DestroyPlayer();
        }
    }
}