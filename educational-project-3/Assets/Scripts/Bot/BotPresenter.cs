using Game;
using Utilities;

namespace Bot
{
    public class BotPresenter : IController
    {
        private readonly BotModel _model;
        private readonly GameManager _manager;

        public BotPresenter(BotModel model, GameManager manager)
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