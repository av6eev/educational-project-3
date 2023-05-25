#nullable enable
using System;
using System.Collections.Generic;
using Cells;
using Descriptions.World;
using Game;
using UnityEngine;
using Utilities;
using Random = System.Random;

namespace Floor.System
{
    public class GenerateWorldSystem : ISystem
    {
        private readonly Action _endGenerate;

        private readonly FloorModel _model;
        private readonly GameManager _manager;
        private readonly WorldDescription _worldDescription;
        
        private readonly int _cornerX1;
        private readonly int _cornerX2;
        private readonly int _cornerZ1; 
        private readonly int _cornerZ2;

        public GenerateWorldSystem(FloorModel model, GameManager manager, Action endGenerate)
        {
            _model = model;
            _manager = manager;
            _endGenerate = endGenerate;
            _worldDescription = _manager.GameDescriptions.World;
            
            _cornerX1 = _worldDescription.AreaX / 2 - _worldDescription.X / 2;
            _cornerX2 = _worldDescription.AreaX / 2 + _worldDescription.X / 2;
            _cornerZ1 = _worldDescription.AreaZ / 2 - _worldDescription.Z / 2;
            _cornerZ2 = _worldDescription.AreaZ / 2 + _worldDescription.Z / 2;
        }
        
        public void Update(float deltaTime)
        {
            var id = 0;
            
            for (var x = 0; x < _worldDescription.AreaX; x++)
            {
                for (var z = 0; z < _worldDescription.AreaZ; z++)
                {
                    var position = new Vector3(x, 0, z);
                    var areaCell = new Cell(id++)
                    {
                        Position = position,
                        Type = UnityEngine.Random.Range(0, _manager.GameView.FloorView.CellPrefabs.Count),
                        IsPlayable = false,
                        IsEmpty = true,
                        PropType = PropType.None,
                        YOffset = GenerateNoise(x, z, _worldDescription.DetailScale) * _worldDescription.NoiseHeight
                    };

                    SetupPlayableZone(x, _cornerX1, _cornerX2, z, _cornerZ1, _cornerZ2, areaCell, position);
                    SetupCameras(x, z, _cornerX1, _cornerZ1, _cornerX2, _cornerZ2, _worldDescription);

                    if (x == 0 || x == _worldDescription.AreaX  || z == 0 || z == _worldDescription.AreaZ )
                    {
                        areaCell.IsBorder = true;
                    }

                    if (!_model.Cells.ContainsKey(areaCell.Position))
                    {
                        _model.Cells.Add(areaCell.Position, areaCell);           
                    }
                }
            }
            
            GroupCells(_worldDescription);
            SetupObjectLocation(PropType.Grass, _model.Cells.Count / 2);
            SetupObjectLocation(PropType.Tree, _worldDescription.TreesCount);
            SetupObjectLocation(PropType.Bush, _worldDescription.BushesCount);
            SetupObjectLocation(PropType.SmallRock, _worldDescription.SmallRocksCount);
            SetupObjectLocation(PropType.RockStructure, _worldDescription.RockStructuresCount);
            SetupObjectLocation(PropType.Lantern, _worldDescription.LanternsCount);
            SetupObjectLocation(PropType.Rock, default, true);

            _endGenerate();
        }

        private void GroupCells(WorldDescription description)
        {
            var cells = _model.Cells;
            var random = new Random();
            var count = 0;

            foreach (var cell in cells.Values)
            {
                if (cell is not { IsPlayable: false, IsBorder: false, IsEmpty: true, GroupId: 0 }) continue;
                
                count++;
            }
            
            for (var i = 0; i < 50; i++)
            {
                var chance = random.NextSingleFloat();
                var targetCell = _model.GetCellById(random.Next(count));
                var groupId = i + 1;
                
                if (targetCell == null) continue;
               
                if (chance < description.ChanceToIncreaseCellOnce)
                {
                    cells.TryGetValue(targetCell.Position + new Vector3(1, 0, 0), out var cell1);
                    cells.TryGetValue(targetCell.Position + new Vector3(0, 0, 1), out var cell2);
                    cells.TryGetValue(targetCell.Position + new Vector3(1, 0, 1), out var cell3);
                    
                    if (chance < description.ChanceToIncreaseCellTwice && cell1 != null && cell2 != null && cell3 != null)
                    {
                        cells.TryGetValue(targetCell.Position + new Vector3(2, 0, 0), out var cell4);
                        cells.TryGetValue(targetCell.Position + new Vector3(2, 0, 1), out var cell5);
                        cells.TryGetValue(targetCell.Position + new Vector3(2, 0, 2), out var cell6);
                        cells.TryGetValue(targetCell.Position + new Vector3(0, 0, 2), out var cell7);
                        cells.TryGetValue(targetCell.Position + new Vector3(1, 0, 2), out var cell8);

                        CreateGroup(new List<Cell?> { targetCell, cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8 }, groupId, 3);

                        cell3.IsGroupCenter = true;

                        continue;
                    }

                    CreateGroup(new List<Cell?> { targetCell, cell1, cell2, cell3 }, groupId, 2);
                }
            }
        }

        private void SetupObjectLocation(PropType type, int propCount = 0, bool isBigStructure = false)
        {
            if (isBigStructure)
            {
                var groupsId = _model.GetAllGroups(3);

                foreach (var id in groupsId)
                {
                    foreach (var cell in _model.GetGroupById(id, 3))
                    {
                        cell.PropType = type;
                        cell.IsEmpty = false;
                    }    
                }
            }
            else
            {
                for (var i = 0; i < propCount; i++)
                {
                    var randomCell = UnityEngine.Random.Range(0, _model.Cells.Count);

                    foreach (var cell in _model.Cells.Values)
                    {
                        if (cell.Id != randomCell || cell is not { IsPlayable: false, IsBorder: false, GroupId: 0, IsEmpty: true }) continue;
                        
                        cell.PropType = type;
                        cell.IsEmpty = false;
                    }
                }
            }
        }
        
        private void SetupPlayableZone(int x, int cornerX1, int cornerX2, int z, int cornerZ1, int cornerZ2, Cell areaCell, Vector3 position)
        {
            if (x >= cornerX1 && x <= cornerX2 && z >= cornerZ1 && z <= cornerZ2)
            {
                areaCell.Type = 0;
                areaCell.IsPlayable = true;
                areaCell.YOffset = 0f;

                if (x == cornerX1 + 1 && z == cornerZ1 + 1)
                {
                    _manager.FloorModel.FirstStartPosition = position;
                }

                if (x == cornerX2 - 1 && z == cornerZ2 - 1)
                {
                    _manager.FloorModel.SecondStartPosition = position;
                }
            }
        }

        private void SetupCameras(int x, int z, int cornerX1, int cornerZ1, int cornerX2, int cornerZ2, WorldDescription worldDescription)
        {
            var cameraDescription = _manager.GameDescriptions.Camera;
            var mainCam = _manager.GameView.CameraManager.MainCamera.transform;
            var startCam = _manager.GameView.CameraManager.CutsceneCameraStart.transform;
            var to1Cam = _manager.GameView.CameraManager.CutsceneCameraTo1.transform;
            var to2Cam = _manager.GameView.CameraManager.CutsceneCameraTo2.transform;
            var to3Cam = _manager.GameView.CameraManager.CutsceneCameraTo3.transform;
            
            if (x == cornerX1 && z == cornerZ1 + worldDescription.Z / 2) // left middle
            {
                to3Cam.position = new Vector3(x - cameraDescription.BorderHorizontalOffset, cameraDescription.BorderVerticalOffset, z);
                to3Cam.rotation = Quaternion.Euler(cameraDescription.VerticalAngle, -270, 0);
            }

            if (x == cornerX2 && z == cornerZ1 + worldDescription.Z / 2) // right middle
            {
                to1Cam.position = new Vector3(x + cameraDescription.BorderHorizontalOffset, cameraDescription.BorderVerticalOffset, z);
                to1Cam.rotation = Quaternion.Euler(cameraDescription.VerticalAngle, -90, 0);

                mainCam.position = new Vector3(x + cameraDescription.BorderHorizontalOffset, cameraDescription.BorderVerticalOffset, z);
                mainCam.rotation = Quaternion.Euler(cameraDescription.VerticalAngle, -90, 0);
            }

            if (x == cornerX1 + worldDescription.X / 2 && z == cornerZ2) // top middle
            {
                to2Cam.position = new Vector3(x, cameraDescription.BorderVerticalOffset, z + cameraDescription.BorderHorizontalOffset);
                to2Cam.rotation = Quaternion.Euler(cameraDescription.VerticalAngle, -180, 0);
            }

            if (x == cornerX1 + worldDescription.X / 2 && z == cornerZ1) // bottom middle
            {
                startCam.position = new Vector3(x, cameraDescription.BorderVerticalOffset, z - cameraDescription.BorderHorizontalOffset);
                startCam.rotation = Quaternion.Euler(cameraDescription.VerticalAngle, 0, 0);
            }
        }

        private void CreateGroup(List<Cell?> cells, int groupId, int groupSize)
        {
            if (!_model.IsCellsFits(cells)) return;

            foreach (var cell in cells)
            {
                cell.GroupId = groupId;
                cell.GroupSize = groupSize;
            }
        }

        private float GenerateNoise(int x, int z, float detailScale)
        {
            var xNoise = x / detailScale;
            var zNoise = z / detailScale;

            return Mathf.PerlinNoise(xNoise, zNoise);
        }
    }
}