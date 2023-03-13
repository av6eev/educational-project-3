using System.Collections.Generic;
using Cells;
using Props.Lantern;
using Props.Rock;
using Props.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Floor
{
    public class FloorView : MonoBehaviour
    {
        public Transform ParentGo;
        
        public List<CellView> CellPrefabs;
        public List<TreeView> TreePrefabs;
        public List<RockView> RockPrefabs;
        public List<SmallRockView> SmallRockPrefabs;
        public List<RocksStructureView> RocksStructurePrefabs;
        public List<LanternView> LanternView;

        public CellView InitializeCell(Vector3 position, int type)
        {
            var cell = Instantiate(type == 0 ? CellPrefabs[0] : CellPrefabs[1], position, Quaternion.identity);
            cell.transform.SetParent(ParentGo);

            return cell;
        }
        
        public void InitializeFloorObject<T>(Vector3 position, List<T> prefabs) where T : MonoBehaviour, IFloorObject
        {
            if (prefabs.Count == 1)
            {
                Instantiate(prefabs[0], position, Quaternion.Euler(0, Random.Range(0, 360), 0)).transform.SetParent(ParentGo);
            }
            else
            {
                Instantiate(prefabs[Random.Range(0, prefabs.Count)], position, Quaternion.identity).transform.SetParent(ParentGo);
            }
        }
    }
}