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

        [Header("Class Stats")] 
        public float MaxHealth;
        public float AttackDamage;
        [Range(0, 100)] public float Resistance;
    }
}