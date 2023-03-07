using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        public event Action<Vector3> OnPlayerCreated;
        public event Action OnPlayerRemoved;
        
        public Vector3 Position { get; private set; } = Vector3.zero;
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

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void CreatePlayer(Vector3 position)
        {
            OnPlayerCreated?.Invoke(position);
        }
        
        public void RemovePlayer()
        {
            OnPlayerRemoved?.Invoke();
        }
    }
}