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
                        _view.InitializeTree(new Vector3(cell.Position.x, cell.Position.y + .3f, cell.Position.z));
                        break;
                    case PropType.Rock:
                        _view.InitializeRock(new Vector3(cell.Position.x, cell.Position.y + .3f, cell.Position.z));
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