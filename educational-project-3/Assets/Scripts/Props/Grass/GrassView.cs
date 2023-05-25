using Floor;
using UnityEngine;

namespace Props.Grass
{
    public class GrassView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Grass;
    }
}