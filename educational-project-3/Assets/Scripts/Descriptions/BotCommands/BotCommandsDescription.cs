namespace Descriptions.BotCommands
{
    public static class BotCommandsDescription
    {
        public const string CommandSymbol = ".";
        public const string DefaultNoCommand = "Команда не найдена!";

        public const string PrepareToGame = "prepare";
        public const string ChooseClass = "class";
        public const string StartGame = "start";
        public const string CheckHealth = "ping";

        public const string StartGameResponse = "Игра началась!";
        public const string ChooseClassResponse = "Выберите класс для своего персонажа!";
        public const string PrepareToGameResponse = "Подготовка к началу игры! Выберите сторону!";
        public const string CheckHealthResponse = "Pong";

        public static string CompleteCommand(string command)
        {
            return $"{CommandSymbol}{command}";
        }
    }
}