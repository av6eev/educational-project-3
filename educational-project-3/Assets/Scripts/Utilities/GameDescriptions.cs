using Descriptions.Base;
using Descriptions.BotCommands;
using Descriptions.World;

namespace Utilities
{
    public class GameDescriptions
    {
        public WorldDescription World;
        public BotCommandsDescription BotCommands;

        public GameDescriptions(DescriptionsCollectionSo collection)
        {
            World = collection.Collection.World.Description;
            BotCommands = collection.Collection.BotCommands.Description;
        }
    }
}