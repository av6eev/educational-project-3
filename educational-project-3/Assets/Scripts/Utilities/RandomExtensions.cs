using UnityEngine;
using Random = System.Random;

namespace Utilities
{
    public static class RandomExtensions
    {
        public static float NextSingleFloat(this Random random, float min = 0f, float max = 1f)
        {
            return Mathf.Lerp(min, max, (float)random.NextDouble());
        }
    }
}