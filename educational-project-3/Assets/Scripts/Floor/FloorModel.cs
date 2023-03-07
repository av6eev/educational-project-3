using System.Collections.Generic;
using Cells;
using Descriptions.World;
using UnityEngine;

namespace Floor
{
    public class FloorModel
    {
        public readonly Dictionary<Vector3, Cell> Cells = new();
        public readonly Vector3 FirstStartPosition;
        public readonly Vector3 SecondStartPosition;

        public FloorModel(WorldDescription description)
        {
            FirstStartPosition = new Vector3(description.X - description.X + 1, 0, description.Z - description.Z + 1);
            SecondStartPosition = new Vector3(description.X - 2, 0, description.Z - 2);
        }
    }
}