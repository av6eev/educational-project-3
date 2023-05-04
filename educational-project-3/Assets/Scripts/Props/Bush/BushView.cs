using Floor;
using UnityEngine;

namespace Props.Bush
{
    public class BushView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Bush;
    }
}