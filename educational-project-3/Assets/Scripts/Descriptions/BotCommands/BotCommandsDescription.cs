namespace Descriptions.BotCommands
{
    public static class BotCommandsDescription
    {
        private const string CommandSymbol = ".";
        public const string DefaultNoCommand = "Команда не найдена!";

        public const string StartGame = "start";
        public const string CheckHealth = "ping";

        public const string StartGameResponse = "Игра началась! Выберите сторону!";
        public const string CheckHealthResponse = "Pong";

        public static string CompleteCommand(string command)
        {
            return $"{CommandSymbol}{command}";
        }
    }
}