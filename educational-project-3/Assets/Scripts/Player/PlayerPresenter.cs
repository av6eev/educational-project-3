using Game;
using Utilities;

namespace Player
{
    public class PlayerPresenter : IPresenter
    {
        private readonly PlayerModel _model;
        private readonly GameManager _manager;

        public PlayerPresenter(PlayerModel model, GameManager manager)
        {
            _model = model;
            _manager = manager;
        }

        public void Deactivate()
        {
            
        }

        public void Activate()
        {
            
        }
    }
}