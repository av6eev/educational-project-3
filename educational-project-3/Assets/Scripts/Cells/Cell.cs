using System;
using Floor;
using UnityEngine;

namespace Cells
{
    public class Cell
    {
        public Vector3 Position;
        
        public int Type;
        public PropType PropType;
        
        public bool IsBorder;
        public bool IsActive;
        public bool IsPlayable;
        public bool IsEmpty;

        public Cell(Vector3 position)
        {
            Position = position;
        }
    }
}