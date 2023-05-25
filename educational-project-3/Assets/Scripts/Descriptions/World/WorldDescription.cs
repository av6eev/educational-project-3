using System;
using Descriptions.Base;
using UnityEngine;

namespace Descriptions.World
{
    [Serializable]
    public class WorldDescription : IDescription
    {
        [Header("Floor Parameters")]
        public int X = 30;
        public int Z = 30;
        public int AreaX = 40;
        public int AreaZ = 40;
        public float NoiseHeight = .9f;
        public float DetailScale = 3f;
        [Range(0f, .8f)] public float ChanceToIncreaseCellOnce = .5f;
        [Range(0f, .4f)] public float ChanceToIncreaseCellTwice = .1f;

        [Header("Props Parameters")] 
        public int TreesCount = 20;
        public int BushesCount = 10;
        public int SmallRocksCount = 10;
        public int RockStructuresCount = 2;
        public int LanternsCount = 3;

        [Header("Game Parameters")] 
        public int MaxPlayersCount = 2;
        
        [Header("Game UI Parameters")] 
        public string TurnCooldownText = "твой ход! Оставшееся время: ";
        public int TurnCooldown = 30;
    }
}