using Descriptions.BotCommands;

namespace Bot
{
    public class BotModel
    {
        private readonly BotCommandsDescription _description;

        public BotModel(BotCommandsDescription description)
        {
            _description = description;
        }
    }
}