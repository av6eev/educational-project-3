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
            foreach (var cell in _model.Cells)
            {
                var view = _view.InitializeCell(cell.Position, cell.Type);
                view.Text.text = $"{cell.IsEmpty}";

                switch (cell.PropType)
                {
                    case PropType.Tree:
                        _view.InitializeFloorObject(new Vector3(cell.Position.x, cell.Position.y + .3f, cell.Position.z), _view.TreePrefabs);
                        break;
                    case PropType.SmallRock:
                        _view.InitializeFloorObject(new Vector3(cell.Position.x, cell.Position.y + .3f, cell.Position.z), _view.SmallRockPrefabs);
                        break;
                    case PropType.Rock:
                        break;
                    case PropType.RockStructure:
                        _view.InitializeFloorObject(new Vector3(cell.Position.x, cell.Position.y + .35f, cell.Position.z), _view.RocksStructurePrefabs);
                        break;
                    case PropType.None:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}