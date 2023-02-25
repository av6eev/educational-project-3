using System;
using Cells;
using Game;
using UnityEngine;
using Utilities;

namespace Floor.System
{
    public class GenerateFloorSystem : ISystem
    {
        private readonly FloorModel _model;
        private readonly GameManager _manager;
        private readonly Action _endGenerate;

        public GenerateFloorSystem(FloorModel model, GameManager manager, Action endGenerate)
        {
            _model = model;
            _manager = manager;
            _endGenerate = endGenerate;
        }
        
        public void Update(float deltaTime)
        {
            var description = _manager.GameDescriptions.World;
            
            for (var i = 0; i < description.X; i++)
            {
                for (var j = 0; j < description.Z; j++)
                {
                    var position = new Vector3(i, 0, j);
                    var cell = new Cell(position)
                    {
                        Type = (description.X - i + j) % 2 == 1 ? 1 : 0
                    };

                    if (i == 0 || i == description.X - 1 || j == 0 || j == description.Z - 1)
                    {
                        cell.IsBorder = true;
                    }
                    
                    _model.Cells.Add(position, cell);
                }
                
                _endGenerate();
            }
        }
    }
}