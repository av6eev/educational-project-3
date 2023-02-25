using UnityEngine;

namespace Cells
{
    public class Cell
    {
        public Vector3 Position;
        public bool IsBorder;
        public int Type;
        public bool IsActive;

        public Cell(Vector3 position)
        {
            Position = position;
        }
    }
}