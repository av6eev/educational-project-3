using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        public event Action OnPlayerCreated;
        public event Action OnPlayerRemoved;
        public event Action<Vector3, Vector3> OnPlayerMove;
        
        public Vector3 Position { get; set; } = Vector3.zero;
        public Vector3 Angle { get; set; } = Vector3.zero;
        public Vector3 Direction { get; set; } = Vector3.zero;
        
        public string Id { get; }
        public string TeamName { get; }
        public string Name { get; }
        
        public PlayerClassType ClassType;
        
        public PlayerModel(string id, string teamName, string name)
        {
            Id = id;
            TeamName = teamName;
            Name = name;
            ClassType = PlayerClassType.None;
        }

        public void CreatePlayer(Vector3 position, Vector3 angle)
        {
            Position = position;
            Angle = angle;
            
            OnPlayerCreated?.Invoke();
        }
        
        public void RemovePlayer()
        {
            OnPlayerRemoved?.Invoke();
        }

        public void Move(Vector3 direction, Vector3 newAngle)
        {
            OnPlayerMove?.Invoke(direction, newAngle);
        }
    }
}