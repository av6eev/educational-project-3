using Floor;
using UnityEngine;

namespace Cells
{
    public class Cell
    {
        public int Id { get; }
        public int GroupId;

        public int GroupSize;

        public Vector3 Position;
        public float YOffset;
        
        public int Type;
        public PropType PropType;

        public bool IsGroupCenter;
        public bool IsBorder;
        public bool IsActive;
        public bool IsPlayable;
        public bool IsEmpty;

        public Cell(int id)
        {
            Id = id;
        }
    }
}