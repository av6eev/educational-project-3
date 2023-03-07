using System;
using Cameras;
using Descriptions.Base;
using Floor;
using Player;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameView : MonoBehaviour
    {
        public DescriptionsCollectionSo DescriptionsCollection;
        [NonSerialized] public PlayerView PlayerView;
        public FloorView FloorView;
        public CameraManager CameraManager;
        
        public GameObject UIRoot;
        public TextMeshProUGUI TurnCooldownTxt;
        
        public PlayerView InstantiatePlayer(Vector3 position, float angle)
        {
            return Instantiate(PlayerView, new Vector3(position.x, 0.29f, position.z), Quaternion.Euler(0f, angle, 0f));
        }

        public void Enable()
        {
            UIRoot.SetActive(true);
        }
        
        public void Disable()
        {
            UIRoot.SetActive(false);
        }
    }
}