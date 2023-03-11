using Game;
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
                var view = _view.InitializeCell(cell.Key, cell.Value.Type);
                // view.Text.text = $"X: {cell.Value.Position.x}, Z: {cell.Value.Position.z}";
                view.Text.text = $"{cell.Value.IsPlayable}";
                
                if (cell.Value.IsBorder)
                {
                    // cellView.MeshRenderer.material.color = Color.red;
                }
            }
        }
    }
}