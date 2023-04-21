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
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z), _view.TreePrefabs);
                        break;
                    case PropType.SmallRock:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z), _view.SmallRockPrefabs);
                        break;
                    case PropType.Rock:
                        if (cell.IsGroupCenter)
                        {
                            _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .3f, position.z), _view.RockPrefabs);
                        }
                        break;
                    case PropType.RockStructure:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .35f, position.z), _view.RocksStructurePrefabs);
                        break;
                    case PropType.Lantern:
                        _view.InitializeFloorObject(new Vector3(position.x, cell.YOffset + .4f, position.z), _view.LanternView);
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