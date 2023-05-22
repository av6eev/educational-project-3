using Game;
using UnityEngine;
using Utilities;
using Vector3 = UnityEngine.Vector3;

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

            var description = _manager.GameDescriptions.Players[activePlayer.ClassType];
            var transform = _manager.GameView.Players[activePlayer.Id].transform;
            var defaultPosition = transform.position;
            var targetCellPosition = _manager.FloorModel.Cells[new Vector3(activePlayer.Position.x + activePlayer.Direction.x, 0, activePlayer.Position.z + activePlayer.Direction.z)].Position;
            var targetRotation = Quaternion.LookRotation(targetCellPosition - defaultPosition);
            
            if (targetRotation != new Quaternion(0f,0f,0f,1f))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, description.RotateTowardsSpeed * deltaTime);
            }
            
            transform.position = Vector3.MoveTowards(defaultPosition, targetCellPosition, description.MoveTowardsSpeed);    
        }
    }
}