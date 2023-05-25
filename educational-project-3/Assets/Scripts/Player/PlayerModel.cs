using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        public event Action OnPlayerCreated;
        public event Action OnPlayerRemoved;
        public event Action<string> OnPlayerMove;
        public event Action OnPlayerAttack;
        public event Action OnDealDamage;
        
        public Vector3 Position { get; set; } = Vector3.zero;
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

        public void CreatePlayer(Vector3 position)
        {
            Position = position;
            
            OnPlayerCreated?.Invoke();
        }
        
        public void RemovePlayer()
        {
            OnPlayerRemoved?.Invoke();
        }

        public void Move(string emoji)
        {
            OnPlayerMove?.Invoke(emoji);
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