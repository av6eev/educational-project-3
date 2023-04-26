using System;
using System.Collections.Generic;
using Cameras;
using Descriptions.Base;
using Floor;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameView : MonoBehaviour
    {
        [NonSerialized] public PlayerView PlayerView;
        [NonSerialized] public readonly Dictionary<string, PlayerView> Players = new();
        
        public DescriptionsCollectionSo DescriptionsCollection;
        public FloorView FloorView;
        public CameraManager CameraManager;
        
        public GameObject UIRoot;
        public TextMeshProUGUI TurnCooldownTxt;
        public Button SkipButton;

        private void Update()
        {
            foreach (var view in Players.Values)
            {
                view.InfoRoot.transform.rotation = Quaternion.LookRotation(view.InfoRoot.transform.position - CameraManager.MainCamera.transform.position);
            }
        }

        public PlayerView InstantiatePlayer(string playerId, Vector3 position, float angle)
        {
            var view = Instantiate(PlayerView, position, Quaternion.Euler(0f, angle, 0f));
            
            Players.Add(playerId, view);
            
            return view;
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