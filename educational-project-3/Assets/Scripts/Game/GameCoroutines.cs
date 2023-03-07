using System.Collections;
using UnityEngine;

namespace Game
{
    public sealed class GameCoroutines : MonoBehaviour
    {
        private static GameCoroutines Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                var go = new GameObject("CoroutineManager");
                _instance = go.AddComponent<GameCoroutines>();
                    
                DontDestroyOnLoad(go);

                return _instance;
            }
        }

        private static GameCoroutines _instance;

        public static Coroutine RunCoroutine(IEnumerator enumerator)
        { 
            return Instance.StartCoroutine(enumerator);
        }
        
        public static void DisableCoroutine(Coroutine coroutine)
        {
            if (coroutine == null) return;
            
            Instance.StopCoroutine(coroutine);
        }
    }
}