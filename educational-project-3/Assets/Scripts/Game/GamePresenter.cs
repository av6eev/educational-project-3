using System.Collections;
using System.Threading.Tasks;
using Bot;
using Player.System;
using UnityEngine;
using Utilities;

namespace Game
{
    public class GamePresenter : IPresenter
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly GameManager _manager;
        private Coroutine _coroutine;

        public GamePresenter(GameModel model, GameView view, GameManager manager)
        {
            _model = model;
            _view = view;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            _model.OnGameStarted -= GameStarted;
            _model.OnTurnChanged -= TurnChanged;
            
            _view.SkipButton.onClick.RemoveListener(TurnChanged);
        }

        public void Activate()
        {
            _model.OnGameStarted += GameStarted;
            _model.OnTurnChanged += TurnChanged;
            
            _view.SkipButton.onClick.AddListener(TurnChanged);
        }

        private async void GameStarted()
        {
            // _manager.GameView.CameraManager.Enable();
            // await Task.Delay(8700);

            _model.TurnTime = _manager.GameDescriptions.World.TurnCooldown;

            _view.TurnCooldownTxt.text = _model.ActivePlayer.Name + " " + _manager.GameDescriptions.World.TurnCooldownText + _manager.GameDescriptions.World.TurnCooldown;
            _view.Enable();
            
            await BotCommandHelper.OnChangeTurn(_model.ActivePlayer.Name, _model.ChannelId);

            _manager.FixedSystemEngine.Add(SystemTypes.PlayerMovementSystem, new PlayerMovementSystem(_manager));
            
            if (_coroutine != null) return;
            _coroutine = GameCoroutines.RunCoroutine(TurnFunction());
        }
        
        private async void TurnChanged()
        {
            GameCoroutines.DisableCoroutine(_coroutine);
            _coroutine = null;
            
            _model.Turn++;
            _view.TurnCooldownTxt.text = _model.ActivePlayer.Name + " " + _manager.GameDescriptions.World.TurnCooldownText + _manager.GameDescriptions.World.TurnCooldown;
            
            await BotCommandHelper.OnChangeTurn(_model.ActivePlayer.Name, _model.ChannelId);
            
            _coroutine = GameCoroutines.RunCoroutine(TurnFunction());
        }

        private IEnumerator TurnFunction()
        {
            _model.TurnTime = _manager.GameDescriptions.World.TurnCooldown;

            while (_model.TurnTime-- > 0)
            {
                _view.TurnCooldownTxt.text = _model.ActivePlayer.Name + " " + _manager.GameDescriptions.World.TurnCooldownText + _model.TurnTime;
                yield return new WaitForSeconds(1);
            }
            
            TurnChanged();
        }
    }
}