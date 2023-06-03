using Floor;
using UnityEngine;

namespace Props.Mushroom
{
    public class MushroomView : MonoBehaviour, IFloorObject
    {
        public PropType Type => PropType.Mushroom;
    }
}