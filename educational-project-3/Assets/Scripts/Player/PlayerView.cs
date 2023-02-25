using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        public Transform PlayerPrefab;
        public readonly Dictionary<string, Transform> Players = new();
        public TextMeshPro Text;

        public void InstantiatePlayer(string id, Vector3 position, float angle)
        {
            var player = Instantiate(PlayerPrefab, new Vector3(position.x, 0.29f, position.z), Quaternion.Euler(0f, angle, 0f));
            
            Text.text = id;
            Players.Add(id, player);
        }
        
        public void DestroyPlayer(string id)
        {
            if (!Players.ContainsKey(id)) return;
            
            Destroy(Players[id].gameObject);
            Players.Remove(id);
        }
    }
}