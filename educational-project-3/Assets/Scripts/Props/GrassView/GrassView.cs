using Floor;
using UnityEngine;

namespace Props.GrassView
{
    public class GrassView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Grass;
    }
}