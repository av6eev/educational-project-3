using Game;
using UnityEngine;
using Utilities;

namespace Floor
{
    public class FloorPresenter : IPresenter
    {
        private readonly FloorModel _model;
        private readonly FloorView _view;
        private readonly GameManager _manager;

        public FloorPresenter(FloorModel model, FloorView view, GameManager manager)
        {
            _model = model;
            _view = view;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            
        }

        public void Activate()
        {
            var count = 0;

            foreach (var cell in _model.Cells.Values)
            {
                var position = cell.Position;
                _view.InitializeCell(new Vector3(position.x, cell.YOffset, position.z), cell.Type);
                
                switch (cell.PropType)
                {
                    case PropType.Tree:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .25f, position.z), Quaternion.Euler(-90, Random.Range(0, 360), 0), _view.TreePrefabs);
                        break;
                    case PropType.SmallRock:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z),  Quaternion.Euler(0, Random.Range(0, 360), 0), _view.SmallRockPrefabs);
                        break;
                    case PropType.Rock:
                        if (cell.IsGroupCenter)
                        {
                            _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z),  Quaternion.Euler(0, Random.Range(0, 360), 0), _view.RockPrefabs);
                        }
                        break;
                    case PropType.RockStructure:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .35f, position.z),  Quaternion.Euler(0, Random.Range(0, 360), 0), _view.RocksStructurePrefabs);
                        break;
                    case PropType.Lantern:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .4f, position.z),  Quaternion.Euler(0, Random.Range(0, 360), 0), _view.LanternPrefabs);
                        break;
                    case PropType.Grass:
                        _view.InitializeFloorObject(new Vector3(position.x - .05f, cell.YOffset + .05f, position.z - .05f), _view.GrassPrefabs[0].transform.rotation, _view.GrassPrefabs);
                        break;
                    case PropType.Bush:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset, position.z), Quaternion.Euler(-90, Random.Range(0, 360), 0), _view.BushPrefabs);
                        break;
                    case PropType.Mushroom:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .25f, position.z), Quaternion.Euler(0, Random.Range(0, 360), 0), _view.MushroomPrefabs);
                        break;
                    case PropType.GremlinSkull:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z), Quaternion.Euler(0, Random.Range(0, 360), 0), _view.GremlinSkullPrefabs);
                        break;
                    case PropType.None:
                        break;
                    default:
                        break;
                }
                
                _model.GenerationProgress = (float)count / (float)_model.Cells.Count;
                count++;
            }
            
            _model.IsGenerated = true;
        }
    }
}