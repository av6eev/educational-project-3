using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace Floor
{
    public class FloorView : MonoBehaviour
    {
        public Transform ParentGo;
        public List<CellView> CellPrefab;

        public CellView InitializeCell(Vector3 position, int type)
        {
            var cell = Instantiate(type == 0 ? CellPrefab[0] : CellPrefab[1], position, Quaternion.identity);
            cell.transform.SetParent(ParentGo);

            return cell;
        }
    }
}