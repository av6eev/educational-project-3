using Floor;
using UnityEngine;

namespace Props.Lantern
{
    public class LanternView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Lantern;
    }
}