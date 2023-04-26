using Floor;
using UnityEngine;

namespace Props.Rock
{
    public class SmallRockView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.SmallRock;
    }
}