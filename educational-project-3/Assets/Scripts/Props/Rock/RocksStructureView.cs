using Floor;
using UnityEngine;

namespace Props.Rock
{
    public class RocksStructureView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.RockStructure;
    }
}