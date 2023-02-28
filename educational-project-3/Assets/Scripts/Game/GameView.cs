using System;
using Descriptions.Base;
using Floor;
using Player;
using UnityEngine;

namespace Game
{
    public class GameView : MonoBehaviour
    {
        public DescriptionsCollectionSo DescriptionsCollection;
        [NonSerialized] public PlayerView PlayerView;
        public FloorView FloorView;
        
        public PlayerView InstantiatePlayer(Vector3 position, float angle)
        {
            return Instantiate(PlayerView, new Vector3(position.x, 0.29f, position.z), Quaternion.Euler(0f, angle, 0f));
        }
    }
}