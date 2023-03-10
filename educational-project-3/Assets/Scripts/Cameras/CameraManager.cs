using UnityEngine;
using UnityEngine.Playables;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public PlayableDirector PlayableDirector;
        public GameObject MainCamera;
        public GameObject CutsceneCamera;

        public void Enable()
        {
            PlayableDirector.Play();
            
            MainCamera.SetActive(false);
            CutsceneCamera.SetActive(true);
        }

        public void Disable()
        {
            PlayableDirector.Stop();
            
            MainCamera.SetActive(true);
            CutsceneCamera.SetActive(false);
        }
    }
}
