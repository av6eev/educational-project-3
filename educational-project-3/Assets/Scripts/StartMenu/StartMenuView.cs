using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StartMenu
{
    public class StartMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject _loaderCanvas;
        [SerializeField] private Image _progressBar;

        public StartMenuView Instance;
        public Button NewGameBtn;
        
        private readonly List<AsyncOperation> _scenesLoading = new();
        private float _totalSceneProgress;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene()
        {
            _loaderCanvas.SetActive(true);
            
            _scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.Game, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadingProgress());
        }

        private IEnumerator GetSceneLoadingProgress()
        {
            foreach (var scene in _scenesLoading)
            {
                while (!scene.isDone)
                {
                    _totalSceneProgress = 0f;

                    foreach (var operation in _scenesLoading)
                    {
                        _totalSceneProgress += operation.progress;
                    }

                    _totalSceneProgress = _totalSceneProgress / _scenesLoading.Count * 100f;
                    _progressBar.fillAmount = Mathf.RoundToInt(_totalSceneProgress);
                    
                    yield return null;
                }
            }
            
            _loaderCanvas.SetActive(false);
            // SceneManager.UnloadSceneAsync((int)SceneIndexes.StartMenu);
        }
    }
}