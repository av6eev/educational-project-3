using System.Collections.Generic;
using System.Linq;
using Cells;
using JetBrains.Annotations;
using UnityEngine;

namespace Floor
{
    public class FloorModel
    {
        public readonly Dictionary<Vector3, Cell> Cells = new();
        public Vector3 FirstStartPosition = Vector3.zero;
        public Vector3 SecondStartPosition = Vector3.zero;
        
        public bool IsGenerated = false;
        public float GenerationProgress;

        public Cell GetCellById(int id)
        {
            Cell neededCell = null;
            
            foreach (var cell in Cells.Values.Where(cell => !cell.IsPlayable && !cell.IsBorder))
            {
                if (cell.Id != id) continue;

                neededCell = cell;
            }

            return neededCell;
        }

        public bool IsCellsFits([ItemCanBeNull] List<Cell> cells)
        {
            var counter = 0;
            
            foreach (var cell in cells)
            {
                if (cell != null && Cells.ContainsKey(cell.Position) && !cell.IsPlayable && cell.IsEmpty && !cell.IsBorder && cell.GroupId == 0)
                {
                    counter++;
                }
            }

            return counter == cells.Count;
        }

        public List<Cell> GetGroupById(int groupId, int size)
        {
            return Cells.Values.Where(cell => cell.GroupId == groupId && cell.GroupSize == size).ToList();
        }

        public List<int> GetAllGroups(int size = 0)
        {
            var ids = new List<int>();

            if (size != 0)
            {
                foreach (var cell in Cells.Values.Where(cell => cell.GroupId != 0 && !ids.Contains(cell.GroupId) && cell.GroupSize == size))
                {
                    ids.Add(cell.GroupId);
                }    
            }
            else
            {
                foreach (var cell in Cells.Values.Where(cell => cell.GroupId != 0 && !ids.Contains(cell.GroupId)))
                {
                    ids.Add(cell.GroupId);
                }
            }

            return ids;
        }
    }
}