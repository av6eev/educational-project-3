using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        public event Action<string, Vector3, float> OnPlayerCreated;
        public event Action<string> OnPlayerRemoved;
        
        public Vector3 Position { get; private set; } = Vector3.zero;
        public string Id { get; }
        public string TeamName { get; }
        
        public PlayerModel(string id, string teamName)
        {
            Id = id;
            TeamName = teamName;
        }

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void CreatePlayer(string id, Vector3 position, float angle)
        {
            OnPlayerCreated?.Invoke(id, position, angle);
        }
        
        public void RemovePlayer(string id)
        {
            OnPlayerRemoved?.Invoke(id);
        }
    }
}