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
        [NonSerialized] public readonly Dictionary<string, PlayerView> Players = new();

        public DescriptionsCollectionSo DescriptionsCollection;
        public FloorView FloorView;
        public CameraManager CameraManager;

        public GameObject UIRoot;
        public GameObject EndGameUIRoot;
        
        public TextMeshProUGUI TurnCooldownTxt;
        public TextMeshProUGUI EndGameWinnerTxt;
        
        public Button SkipButton;
        public Button PauseButton;
        public Button NewGameButton;

        private void Update()
        {
            foreach (var view in Players.Values)
            {
                var infoRootTransform = view.InfoRoot.transform;
                
                infoRootTransform.rotation = Quaternion.LookRotation(infoRootTransform.position - CameraManager.MainCamera.transform.position);
            }
        }

        public PlayerView InstantiatePlayer(PlayerView playerView, string playerId, Vector3 position, float angle)
        {
            var view = Instantiate(playerView, position, Quaternion.Euler(0f, angle, 0f));

            Players.Add(playerId, view);
            return view;
        }

        public void ManageUI(string type, bool state)
        {
            switch (type)
            {
                case "Main":
                    UIRoot.SetActive(state);
                    break;
                case "End":
                    EndGameUIRoot.SetActive(state);
                    break;
                case "All":
                    UIRoot.SetActive(state);
                    EndGameUIRoot.SetActive(state);
                    break;
            }
        }

        public void DestroyOnUnload()
        {
            Destroy(GameObject.Find("DiscordManager"));
        }
    }
}