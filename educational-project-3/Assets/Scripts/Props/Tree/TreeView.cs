using Floor;
using UnityEngine;

namespace Props.Tree
{
    public class TreeView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Tree;
    }
}