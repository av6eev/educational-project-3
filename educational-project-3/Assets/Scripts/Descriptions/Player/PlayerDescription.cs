using System;
using Descriptions.Base;
using Player;
using UnityEngine;

namespace Descriptions.Player
{
    [Serializable]
    public class PlayerDescription : IDescription
    {
        [Header("General")] 
        public PlayerClassType Type;
        public PlayerView Prefab;
        public AnimatorOverrideController AnimatorOverrideController;

        [Header("Movement Settings")]
        [Range(0f, 0.07f)] public float MoveTowardsSpeed = 0.06f;
        [Range(0f, 500f)] public float RotateTowardsSpeed = 300f;
        
        [Header("Class Stats")] 
        public float MaxHealth;
        public float AttackDamage;
        [Range(0, 100)] public float Resistance;
    }
}