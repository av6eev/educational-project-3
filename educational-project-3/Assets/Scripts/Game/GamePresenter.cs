using System.Threading.Tasks;
using Utilities;

namespace Game
{
    public class GamePresenter : IPresenter
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly GameManager _manager;

        public GamePresenter(GameModel model, GameView view, GameManager manager)
        {
            _model = model;
            _view = view;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            _model.OnGameStarted -= GameStarted;
        }

        public void Activate()
        {
            _model.OnGameStarted += GameStarted;
        }

        private async void GameStarted()
        {
            _manager.GameView.CameraManager.Enable();
            await Task.Delay(9000);
            
            _view.TurnCooldownTxt.text = _model.Players[0].Name + " " + _manager.GameDescriptions.World.TurnCooldownText + _manager.GameDescriptions.World.TurnCooldown;
            _view.Enable();
        }
    }
}