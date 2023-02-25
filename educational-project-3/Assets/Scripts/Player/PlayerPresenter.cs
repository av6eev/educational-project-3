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
        }

        public void Activate()
        {
            _model.OnPlayerCreated += InitPlayer;
            _model.OnPlayerRemoved += DestroyPlayer;
        }

        private void InitPlayer(string id, Vector3 position, float angle)
        {
            _model.SetPosition(position);
            _view.InstantiatePlayer(id, position, angle);
        }
        
        private void DestroyPlayer(string id)
        {
            _view.DestroyPlayer(id);
        }
    }
}