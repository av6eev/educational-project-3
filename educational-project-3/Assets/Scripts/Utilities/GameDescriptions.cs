using System.Collections.Generic;
using System.Linq;
using Descriptions.Base;
using Descriptions.Player;
using Descriptions.World;
using Player;

namespace Utilities
{
    public class GameDescriptions
    {
        public readonly WorldDescription World;
        public readonly Dictionary<PlayerClassType, PlayerDescription> Players = new();

        public GameDescriptions(DescriptionsCollectionSo collection)
        {
            World = collection.Collection.World.Description;

            foreach (var description in collection.Collection.Players.Select(element => element.Description))
            {
                Players.Add(description.Type, description);
            }
        }
    }
}