using System;
using Descriptions.BotCommands;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.Rest.DiscordAPI;

namespace Bot
{
    public class BotModel
    {
        public void CheckRequestMessage(DiscordMessage message)
        {
            Action<DiscordMessage> action;
            
            if (message.Author.Bot is null or false)
            {
                Debug.Log("Request message send: " + message.Content + ", from: " + message.Author.Username + ", messageID: " + message.Id);

                action = BotCommandHelper.RequestCommands.TryGetValue(message.Content, out var callback) ? callback : DefaultMessage;
            }
            else
            {
                Debug.Log("Response message send: " + message.Content + ", from: " + message.Author.Username + ", messageID: " + message.Id);
        
                action = BotCommandHelper.ResponseCommands.TryGetValue(message.Content, out var callback) ? callback : DefaultMessage;
            }
            
            action(message);
        }
        
        private async void DefaultMessage(DiscordMessage message)
        {
            await DiscordAPI.CreateMessage(message.ChannelId, BotCommandsDescription.DefaultNoCommand, null, false, null, null, null, null);
        }
    }
}