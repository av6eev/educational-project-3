using Game;
using UnityEngine;
using Utilities;

namespace Player.System
{
    public class PlayerMovementSystem : ISystem
    {
        private readonly GameManager _manager;

        public PlayerMovementSystem(GameManager manager)
        {
            _manager = manager;
        }
        
        public void Update(float deltaTime)
        {
            var activePlayer = _manager.GameModel.ActivePlayer;
            
            if (activePlayer.Direction == Vector3.zero) return;

            var view = _manager.GameView.Players[activePlayer.Id].transform;
            
            view.rotation = Quaternion.Slerp(view.rotation, Quaternion.Euler(activePlayer.Angle), .25f);
            view.position += activePlayer.Direction * deltaTime;
        }
    }
}