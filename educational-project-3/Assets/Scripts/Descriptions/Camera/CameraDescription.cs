using System;
using Descriptions.Base;

namespace Descriptions.Camera
{
    [Serializable]
    public class CameraDescription : IDescription
    {
        public float BorderVerticalOffset = 10.0f;
        public float BorderHorizontalOffset = 10.0f;
        public float VerticalAngle = 30.0f;
    }
}