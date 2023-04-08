using Game;
using UnityEngine;
using Utilities;

namespace Start
{
    public class StartPresenter : MonoBehaviour
    {
        public StartView View;
        
        private IPresenter _presenter;
        private readonly GameManager _manager = new();
        private readonly SystemEngine _systemEngine = new();
        private readonly FixedSystemEngine _fixedSystemEngine = new();
        
        private void Start()
        {
            _manager.StartView = View;
            _manager.GameDescriptions = new GameDescriptions(View.DescriptionsCollection);
            _manager.SystemEngine = _systemEngine;
            _manager.FixedSystemEngine = _fixedSystemEngine;

            _presenter = new StartGamePresenter(_manager, View);
            _presenter.Activate();
        }
        
        private void Update()
        {
            _systemEngine.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _fixedSystemEngine.Update(Time.deltaTime);
        }
    }
}