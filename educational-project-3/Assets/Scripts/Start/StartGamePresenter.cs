using System.Collections;
using System.Collections.Generic;
using Bot;
using Floor;
using Floor.System;
using Game;
using Plugins.DiscordUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Start
{
    public class StartGamePresenter : IPresenter
    {
        private readonly GameManager _manager;
        private readonly StartView _view;
        
        private readonly List<AsyncOperation> _sceneLoader = new();
        private readonly PresenterEngine _presenterEngine = new();

        private Coroutine _sceneCoroutine;
        private Coroutine _spawnCoroutine;
        
        private string _botToken;
        private float _totalSpawnProgress;
        private float _totalSceneProgress;

        public StartGamePresenter(GameManager manager, StartView view)
        {
            _manager = manager;
            _view = view;
        }
        
        public void Deactivate()
        {
        }

        public void Activate()
        {
            _view.NewGameBtn.onClick.AddListener(OnGameStarted);
            _view.KeyInputField.onValueChanged.AddListener(OnKeyInput);
        }
        
        private void OnGameStarted()
        {
            _manager.BotModel = new BotModel(_manager);
            _manager.GameModel = new GameModel();
            _manager.FloorModel = new FloorModel();
            
            _view.LoaderCanvas.SetActive(true);
            
            _sceneLoader.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.Game, LoadSceneMode.Additive));
            
            _sceneCoroutine = GameCoroutines.RunCoroutine(GetSceneLoadingProgress());
        }

        private void OnSceneLoadComplete()
        {
            GameCoroutines.DisableCoroutine(_sceneCoroutine);
            _spawnCoroutine = GameCoroutines.RunCoroutine(GetSpawnLoadingProgress());

            _manager.GameView = GameObject.Find("GameView").GetComponent<GameView>();
            
            _manager.SystemEngine.Add(SystemTypes.GenerateWorldSystem, new GenerateWorldSystem(_manager.FloorModel, _manager, EndGenerate));
   
            _presenterEngine.Add(new BotPresenter(_manager.BotModel, _manager));
            _presenterEngine.Add(new FloorPresenter(_manager.FloorModel, _manager.GameView.FloorView, _manager));
            
            var discordComponent = new GameObject("DiscordManager").AddComponent<DiscordManager>();
            discordComponent.Manager = _manager;
            discordComponent.BotToken = _botToken;
        }

        private void EndGenerate()
        {
            _manager.SystemEngine.Remove(SystemTypes.GenerateWorldSystem);
            _presenterEngine.Activate();
        }

        private void OnKeyInput(string value)
        {
            _botToken = value;
        }
        
        private IEnumerator GetSceneLoadingProgress()
        {
            foreach (var scene in _sceneLoader)
            {
                while (!scene.isDone)
                {
                    _totalSceneProgress = 0;

                    foreach (var operation in _sceneLoader)
                    {
                        _totalSceneProgress += operation.progress;
                    }

                    _totalSceneProgress = _totalSceneProgress / _sceneLoader.Count * 100f;
                    _view.LoadingTxt.text = $"Загрузка сцены: {_totalSceneProgress}%";

                    yield return null;
                }       
            }
            
            OnSceneLoadComplete();
        }
        
        private IEnumerator GetSpawnLoadingProgress()
        {
            while (!_manager.FloorModel.IsGenerated)
            {
                _totalSpawnProgress = 0f;
                _totalSpawnProgress = Mathf.Round(_manager.FloorModel.GenerationProgress * 100f);

                _view.LoadingTxt.text = $"Загрузка объектов: {_totalSceneProgress}%";

                var totalProgress = Mathf.Round((_totalSceneProgress + _totalSpawnProgress) / 2f) * 100f;
                
                _view.ProgressBar.fillAmount = Mathf.RoundToInt(totalProgress);
                
                yield return null;
            }
            
            GameCoroutines.DisableCoroutine(_spawnCoroutine);
            _view.LoaderCanvas.SetActive(false);
            SceneManager.UnloadSceneAsync((int)SceneIndexes.StartMenu);
        }
    }
}