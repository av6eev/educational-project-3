using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Descriptions.BotCommands;
using Player;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.Rest.DiscordAPI;

namespace Bot
{
    public static class BotCommandHelper
    {
        public static readonly Dictionary<string, Action<DiscordMessage>> RequestCommands = new()
        {
            { BotCommandsDescription.CompleteCommand(BotCommandsDescription.CheckHealth), CheckHealth },
            { BotCommandsDescription.CompleteCommand(BotCommandsDescription.StartGame), StartGame }
        };
        
        public static readonly Dictionary<string, Action<DiscordMessage>> ResponseCommands = new()
        {
            { BotCommandsDescription.CheckHealthResponse, CheckHealthResponse },
            { BotCommandsDescription.StartGameResponse, StartGameResponse }
        };

        public static async void ShowActivePlayers(DiscordMessageReaction reaction, Dictionary<string, PlayerModel> activeUsers)
        {
            await DiscordAPI.CreateMessage(reaction.ChannelId, "Составы команд: \n", null, false, null, null, null, null);

            foreach (var user in activeUsers)
            {
                await DiscordAPI.CreateMessage(reaction.ChannelId, $"{user.Key} - {user.Value.TeamName} команда", null, false, null, null, null, null);
            }
            
            await DiscordAPI.CreateMessage(reaction.ChannelId, "---------------\n", null, false, null, null, null, null);
        }
        
        private static async void StartGame(DiscordMessage message)
        {
            await DiscordAPI.CreateMessage(message.ChannelId, BotCommandsDescription.StartGameResponse, null, false, null, null, null, null);
        }
        
        private static async void StartGameResponse(DiscordMessage message)
        {
            Debug.Log(message.Id);
            await AddEmoji(message.ChannelId, message.Id, "⚪");
            await Task.Delay(1000);
            await AddEmoji(message.ChannelId, message.Id, "⚫");
        }

        private static async void CheckHealth(DiscordMessage message)
        {
            await DiscordAPI.CreateMessage(message.ChannelId, BotCommandsDescription.CheckHealthResponse, null, false, null, null, null, null);
        }
        
        private static async void CheckHealthResponse(DiscordMessage message)
        {
            await AddEmoji(message.ChannelId, message.Id, "😄");
        }
        
        private static async Task AddEmoji(string channelId, string messageId, string emoji)
        {
            await DiscordAPI.CreateReaction(channelId, messageId, emoji);
        }
    }
}