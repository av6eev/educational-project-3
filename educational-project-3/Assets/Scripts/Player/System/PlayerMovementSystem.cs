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
            var view = _manager.GameView.Players[activePlayer.Id].transform;

            if (activePlayer.Direction == Vector3.zero) return;
            // view.transform.Rotate(angle);
            view.rotation = Quaternion.Slerp(view.rotation, Quaternion.Euler(activePlayer.Angle), .25f);

            // view.rotation = Quaternion.Slerp(view.transform.rotation, Quaternion.Euler(activePlayer.Angle), .25f);
            // view.Rigidbody.AddForce(_manager.GameModel.ActivePlayer.Position.normalized * 5f, ForceMode.Acceleration);
        }
    }
}