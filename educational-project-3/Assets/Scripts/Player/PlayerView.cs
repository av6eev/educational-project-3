using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        public TextMeshPro Text;
        public GameObject Root;
        public Animator Animator;
        
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        
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

        public void PlayWalkAnimation(bool state)
        {
            Animator.SetBool(IsWalking, state);
        }
    }
}