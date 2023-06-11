using System.Collections;
using Bot;
using Player.System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Game
{
    public class GamePresenter : IPresenter
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly GameManager _manager;
        private Coroutine _coroutine;

        private int _pauseCounter = 0;
        
        public GamePresenter(GameModel model, GameView view, GameManager manager)
        {
            _model = model;
            _view = view;
            _manager = manager;
        }
        
        public void Deactivate()
        {
            _model.OnGameStarted -= GameStarted;
            _model.OnGameEnded -= GameEnded;
            _model.OnTurnChanged -= TurnChanged;
            
            _view.SkipButton.onClick.RemoveListener(TurnChanged);
            _view.PauseButton.onClick.RemoveListener(GamePaused);
            _view.NewGameButton.onClick.RemoveListener(StartNewGame);
        }

        public void Activate()
        {
            _model.OnGameStarted += GameStarted;
            _model.OnGameEnded += GameEnded;
            _model.OnTurnChanged += TurnChanged;
            
            _view.SkipButton.onClick.AddListener(TurnChanged);
            _view.PauseButton.onClick.AddListener(GamePaused);
            _view.NewGameButton.onClick.AddListener(StartNewGame);
        }

        private void GameEnded()
        {
            _view.ManageUI("Main", false);
            _view.ManageUI("End", true);
            _view.EndGameWinnerTxt.text += _model.ActivePlayer.Name;
        }

        private async void GameStarted()
        {
            // _manager.GameView.CameraManager.Enable();
            // await Task.Delay(8700);

            _model.TurnTime = _manager.GameDescriptions.World.TurnCooldown;

            _view.TurnCooldownTxt.text = _model.ActivePlayer.Name + " " + _manager.GameDescriptions.World.TurnCooldownText + _manager.GameDescriptions.World.TurnCooldown;
            _view.ManageUI("Main", true);
            
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

        private void StartNewGame()
        {
            _view.DestroyOnUnload();

            SceneManager.LoadSceneAsync((int)SceneIndexes.StartMenu, LoadSceneMode.Single);
        }
        
        private void GamePaused()
        {
            Time.timeScale = _pauseCounter % 2 == 0 ? 0f : 1f;

            _pauseCounter++;
        }
    }
}