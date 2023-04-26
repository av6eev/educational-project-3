using Floor;
using UnityEngine;

namespace Props.Rock
{
    public class RockView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Rock;
    }
}