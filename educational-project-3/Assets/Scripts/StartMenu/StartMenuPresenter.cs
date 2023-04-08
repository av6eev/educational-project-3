using UnityEngine;

namespace StartMenu
{
    public class StartMenuPresenter : MonoBehaviour
    {
        public StartMenuView View;
        
        private void Start()
        {
            View.NewGameBtn.onClick.AddListener(OnGameStarted);
        }

        private void OnGameStarted()
        {
            View.Instance.LoadScene();
        }
    }
}