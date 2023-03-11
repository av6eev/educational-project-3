using System.Collections.Generic;
using Cells;
using Descriptions.World;
using UnityEngine;

namespace Floor
{
    public class FloorModel
    {
        public readonly Dictionary<Vector3, Cell> Cells = new();
        public Vector3 FirstStartPosition;
        public Vector3 SecondStartPosition;
    }
}