using Game;
using UnityEngine;
using Utilities;

namespace Player.System
{
    public class PlayerMovementSystem : ISystem
    {
        private readonly GameManager _manager;
        
        private const float LerpSpeed = 0.01f;
        private const float MoveTowardsSpeed = 0.06f;

        public PlayerMovementSystem(GameManager manager)
        {
            _manager = manager;
        }
        
        public void Update(float deltaTime)
        {
            var activePlayer = _manager.GameModel.ActivePlayer;
            
            if (activePlayer.Direction == Vector3.zero) return;

            var transform = _manager.GameView.Players[activePlayer.Id].transform;
            var defaultPosition = transform.position;
            var targetCellPosition = _manager.FloorModel.Cells[new Vector3(activePlayer.Position.x, 0, activePlayer.Position.z)].Position;
            var targetPosition = new Vector3(targetCellPosition.x, 0.25f, targetCellPosition.z);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(activePlayer.Angle), .25f);
            transform.position = Vector3.MoveTowards(defaultPosition, Vector3.Lerp(defaultPosition,targetPosition, LerpSpeed), MoveTowardsSpeed);
        }
    }
}