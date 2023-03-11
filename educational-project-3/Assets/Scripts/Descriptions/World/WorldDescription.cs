using System;
using Descriptions.Base;
using UnityEngine;

namespace Descriptions.World
{
    [Serializable]
    public class WorldDescription : IDescription
    {
        public string Id;
        
        [Header("Floor Parameters")]
        public int X = 30;
        public int Z = 30;
        public int AreaX = 40;
        public int AreaZ = 40;
        public float NoiseHeight = .9f;
        public float DetailScale = 3f;

        [Header("Props Parameters")] 
        public int TreesCount = 20;
        public int RocksCount = 20;

        [Header("Game Parameters")] 
        public int MaxPlayersCount = 2;
        
        [Header("Game UI Parameters")] 
        public string TurnCooldownText = "твой ход! Оставшееся время: ";
        public int TurnCooldown = 30;
    }
}