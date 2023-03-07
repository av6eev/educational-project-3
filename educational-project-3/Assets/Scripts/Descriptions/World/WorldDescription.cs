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

        [Header("Game Parameters")] 
        public int MaxPlayersCount = 1;
        
        [Header("Game UI Parameters")] 
        public string TurnCooldownText = "твой ход! Оставшееся время: ";
        public int TurnCooldown = 30;
    }
}