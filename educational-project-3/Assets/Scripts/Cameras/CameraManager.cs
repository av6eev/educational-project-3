using UnityEngine;
using UnityEngine.Playables;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public PlayableDirector PlayableDirector;

        public void Enable()
        {
            PlayableDirector.Play();
        }
    }
}
