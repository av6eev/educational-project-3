using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Player;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.Rest.DiscordAPI;

namespace Bot
{
    
    public class BotModel
    {
        public Action<string> OnPlayerEntered;
        public Action<string> OnPlayerLeft;
        
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

            foreach (var model in from user in teams where CheckUser(user.Key) select new PlayerModel(user.Key.Id, user.Value, user.Key.Username))
            {
                ActiveUsers.Add(model.Id, model);
            }

            await BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);

            if (ActiveUsers.Count == 2)
            {
                _manager.GameModel.GameStage = GameStage.Choosing;
            }
        }

        public async void ChooseClass(DiscordMessageReaction reaction)
        {
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;
            
            var melee = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, "🗡");
            var archer = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, "🏹");
            var mage = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, "🧙");
            var meleeArray = melee?.Data.ToArray();
            var archerArray = archer?.Data.ToArray();
            var mageArray = mage?.Data.ToArray();
            var teams = new Dictionary<DiscordUser, string>();

            foreach (var user in meleeArray!)
            {
                teams.Add(user, "🗡");
            }
            
            foreach (var user in archerArray!)
            {
                teams.Add(user, "🏹");
            }
            
            foreach (var user in mageArray!)
            {
                teams.Add(user, "🧙");
            }
            
            foreach (var user in teams)
            {
                if (!CheckUserClass(user, reaction)) continue;

                var model = ActiveUsers[user.Key.Id];
                PlayerView prefab = null;
                
                switch (user.Value)
                {
                    case "🗡":
                        model.ClassType = PlayerClassType.Melee;
                        prefab = _manager.GameDescriptions.Players[PlayerClassType.Melee].Prefab;
                        break;
                    case "🏹":
                        model.ClassType = PlayerClassType.Archer;
                        prefab = _manager.GameDescriptions.Players[PlayerClassType.Archer].Prefab;
                        break;
                    case "🧙":
                        model.ClassType = PlayerClassType.Mage;
                        prefab = _manager.GameDescriptions.Players[PlayerClassType.Mage].Prefab;
                        break;
                    default:
                        model.ClassType = PlayerClassType.None;
                        break;
                }

                _manager.GameView.PlayerView = prefab;
        
                OnPlayerEntered?.Invoke(model.Id);
            }
        }

        private bool CheckUser(DiscordUser user)
        {
            return !user.Id.Equals(BotId) && !ActiveUsers.ContainsKey(user.Id);
        }
        
        private bool CheckUserClass(KeyValuePair<DiscordUser, string> user, DiscordMessageReaction reaction)
        {
            if (!user.Key.Id.Equals(BotId) && ActiveUsers.ContainsKey(user.Key.Id))
            {
                if (reaction.Emoji.Name == user.Value && ActiveUsers[user.Key.Id].ClassType == PlayerClassType.None)
                {
                    return true;
                }
            }

            return false;
        }

        public async void RemoveUser(string userId, string emojiName)
        {
            if (ActiveUsers.ContainsKey(userId))
            {
                if (emojiName != ActiveUsers[userId].TeamName) return;
                
                await DiscordAPI.CreateMessage(_messageReaction.ChannelId, $"{userId} удален из {ActiveUsers[userId].TeamName} команды", null, false, null, null, null, null);

                OnPlayerLeft?.Invoke(userId);
                ActiveUsers.Remove(userId);
                
                await BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);
            }
        }
    }
}