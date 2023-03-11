using System.Collections.Generic;
using Cells;
using Rock;
using Tree;
using UnityEngine;

namespace Floor
{
    public class FloorView : MonoBehaviour
    {
        public Transform ParentGo;
        public List<CellView> CellPrefabs;
        public List<TreeView> TreePrefabs;
        public List<RockView> RockPrefabs;

        public CellView InitializeCell(Vector3 position, int type)
        {
            var cell = Instantiate(type == 0 ? CellPrefabs[0] : CellPrefabs[1], position, Quaternion.identity);
            cell.transform.SetParent(ParentGo);

            return cell;
        }
        
        public void InitializeTree(Vector3 position)
        {
            var tree = Instantiate(TreePrefabs[Random.Range(0, TreePrefabs.Count)], position, Quaternion.identity);
            tree.transform.SetParent(ParentGo);
        }
        
        public void InitializeRock(Vector3 position)
        {
            var tree = Instantiate(RockPrefabs[Random.Range(0, RockPrefabs.Count)], position, Quaternion.identity);
            tree.transform.SetParent(ParentGo);
        }
    }
}