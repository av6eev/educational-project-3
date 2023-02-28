using System;
using Descriptions.Base;
using Player;
using UnityEngine;

namespace Descriptions.Player
{
    [Serializable]
    public class PlayerDescription : IDescription
    {
        [Header("Type And Prefab")] 
        public PlayerClassType Type;
        public PlayerView Prefab;

        [Header("Class Stats")] 
        public int AttackDamage;
        public int Resistance;
    }
}