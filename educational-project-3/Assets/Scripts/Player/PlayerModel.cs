using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        public event Action OnPlayerCreated;
        public event Action OnPlayerRemoved;
        public event Action<Vector3, Vector3> OnPlayerMove;
        public event Action OnPlayerAttack;
        public event Action OnDealDamage;
        
        public Vector3 Position { get; set; } = Vector3.zero;
        public Vector3 Angle { get; set; } = Vector3.zero;
        public Vector3 Direction { get; set; } = Vector3.zero;
        
        public string Id { get; }
        public string TeamName { get; }
        public string Name { get; }

        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float AttackDamage { get; set; }
        public float Resistance { get; set; }

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

        public void Attack()
        {
            OnPlayerAttack?.Invoke();
        }

        public void DealDamage(float damage)
        {
            CurrentHealth -= damage;
            
            OnDealDamage?.Invoke();
        }
    }
}