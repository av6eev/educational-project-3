using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        public TextMeshPro Text;
        public GameObject Root;
        
        public void DestroyPlayer()
        {
            Destroy(Root);
        }

        public void Disable()
        {
            Root.SetActive(false);
        }
        
        public void Enable()
        {
            Root.SetActive(true);
        }
    }
}