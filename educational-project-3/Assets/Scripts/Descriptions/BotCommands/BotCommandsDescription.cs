using System;
using Descriptions.Base;
using UnityEngine;

namespace Descriptions.BotCommands
{
    [Serializable]
    public class BotCommandsDescription : IDescription
    {
        [Header("Base")]
        public string Id;
        public string CommandSymbol;
        
        [Header("Commands")]
        public string StartGame;
        public string CheckHealth;
        
        [Header("Commands Answers")]
        public string StartGameAnswer;
        public string CheckHealthAnswer;
        public string CompleteCommand(string command)
        {
            return $"{CommandSymbol}{command}";
        }
    }
}