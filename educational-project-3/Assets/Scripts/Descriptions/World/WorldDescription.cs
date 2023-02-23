using System;
using Descriptions.Base;
using UnityEngine;

namespace Descriptions.World
{
    [Serializable]
    public class WorldDescription : IDescription
    {
        public string Id;
        
        [Header("World Parameters")]
        public int X = 30;
        public int Z = 30;
    }
}