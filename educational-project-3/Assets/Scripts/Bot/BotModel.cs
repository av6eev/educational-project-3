using System;
using System.Collections.Generic;
using System.Linq;
using Descriptions.BotCommands;
using Game;
using Player;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.Rest.DiscordAPI;

namespace Bot
{
    
    public class BotModel
    {
        private const string BotId = "1078053498404474942";
        private DiscordMessageReaction _messageReaction;
        public readonly Dictionary<string, PlayerModel> ActiveUsers = new();

        private readonly GameManager _manager;

        public BotModel(GameManager manager)
        {
            _manager = manager;
        }
        
        public void CheckRequestMessage(DiscordMessage message)
        {
            Action<DiscordMessage> action;
            
            if (message.Author.Bot is null or false)
            {
                Debug.Log("Request message send: " + message.Content + ", from: " + message.Author.Username + ", messageID: " + message.Id);

                action = BotCommandHelper.RequestCommands.TryGetValue(message.Content, out var callback) ? callback : null;
            }
            else
            {
                Debug.Log("Response message send: " + message.Content + ", from: " + message.Author.Username + ", messageID: " + message.Id);

                action = BotCommandHelper.ResponseCommands.TryGetValue(message.Content, out var callback) ? callback : null;
            }

            action?.Invoke(message);
        }
        
        public async void AddUsers(DiscordMessageReaction reaction)
        {
            _messageReaction = reaction;
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;

            var leftTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, "⚪");
            var rightTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, "⚫");
            var leftTeamToArray = leftTeam.Data.ToArray();
            var rightTeamToArray = rightTeam.Data.ToArray();
            var teams = new Dictionary<DiscordUser, string>();

            foreach (var user in leftTeamToArray)
            {
                teams.Add(user, "⚪");
            }
            
            foreach (var user in rightTeamToArray)
            {
                teams.Add(user, "⚫");
            }
            
            foreach (var user in teams)
            {
                if (!CheckUser(user.Key)) continue;
            
                var model = new PlayerModel(user.Key.Id, user.Value);
                var presenter = new PlayerPresenter(model, _manager);
                presenter.Activate();
                
                ActiveUsers.Add(model.Id, model);
            }

            BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);
        }

        private bool CheckUser(DiscordUser user)
        {
            if (user.Id.Equals(BotId) || ActiveUsers.ContainsKey(user.Id))
            {
                return false;
            }

            return true;
        }

        public async void RemoveUser(string userId, string emojiName)
        {
            if (ActiveUsers.ContainsKey(userId))
            {
                if (emojiName != ActiveUsers[userId].TeamName) return;
                
                await DiscordAPI.CreateMessage(_messageReaction.ChannelId, $"{userId} удален из {ActiveUsers[userId].TeamName} команды", null, false, null, null, null, null);
                ActiveUsers.Remove(userId);
                BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);
            }
        }
    }
}