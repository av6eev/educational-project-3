using System;
using System.Collections.Generic;
using Descriptions.Camera;
using Descriptions.Player;
using Descriptions.World;

namespace Descriptions.Base
{
    [Serializable]
    public class DescriptionsCollection
    {
        public WorldDescriptionSo World;
        public CameraDescriptionSo Camera;
        public List<PlayerDescriptionSo> Players;
    }
}