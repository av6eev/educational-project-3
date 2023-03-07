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

            var leftTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.FirstTeamEmoji);
            var rightTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.SecondTeamEmoji);
            var leftTeamToArray = leftTeam.Data.ToArray();
            var rightTeamToArray = rightTeam.Data.ToArray();
            var teams = new Dictionary<DiscordUser, string>();

            foreach (var user in leftTeamToArray)
            {
                teams.Add(user, BotCommandHelper.FirstTeamEmoji);
            }
            
            foreach (var user in rightTeamToArray)
            {
                teams.Add(user, BotCommandHelper.SecondTeamEmoji);
            }

            foreach (var model in from user in teams where CheckUser(user.Key) select new PlayerModel(user.Key.Id, user.Value, user.Key.Username))
            {
                ActiveUsers.Add(model.Id, model);
            }

            await BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);

            if (ActiveUsers.Count == 1)
            {
                _manager.GameModel.GameStage = GameStage.Choosing;
            }
        }

        public async void ChooseClass(DiscordMessageReaction reaction)
        {
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;
            
            var melee = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.MeleeEmoji);
            var archer = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.ArcherEmoji);
            var mage = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.MageEmoji);
            var meleeArray = melee?.Data.ToArray();
            var archerArray = archer?.Data.ToArray();
            var mageArray = mage?.Data.ToArray();
            var teams = new Dictionary<DiscordUser, string>();

            foreach (var user in meleeArray!)
            {
                teams.Add(user, BotCommandHelper.MeleeEmoji);
            }
            
            foreach (var user in archerArray!)
            {
                teams.Add(user, BotCommandHelper.ArcherEmoji);
            }
            
            foreach (var user in mageArray!)
            {
                teams.Add(user, BotCommandHelper.MageEmoji);
            }
            
            foreach (var user in teams)
            {
                if (!CheckUserClass(user, reaction)) continue;

                var model = ActiveUsers[user.Key.Id];
                PlayerView prefab = null;
                
                switch (user.Value)
                {
                    case BotCommandHelper.MeleeEmoji:
                        model.ClassType = PlayerClassType.Melee;
                        prefab = _manager.GameDescriptions.Players[PlayerClassType.Melee].Prefab;
                        break;
                    case BotCommandHelper.ArcherEmoji:
                        model.ClassType = PlayerClassType.Archer;
                        prefab = _manager.GameDescriptions.Players[PlayerClassType.Archer].Prefab;
                        break;
                    case BotCommandHelper.MageEmoji:
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
            if (user.Key.Id.Equals(BotId) || !ActiveUsers.ContainsKey(user.Key.Id)) return false;
            
            return reaction.Emoji.Name == user.Value && ActiveUsers[user.Key.Id].ClassType == PlayerClassType.None;
        }

        public async void RemoveUser(string userId, string emojiName)
        {
            if (!ActiveUsers.ContainsKey(userId)) return;
            
            switch (_manager.GameModel.GameStage)
            {
                case GameStage.Preparing:
                    if (emojiName != ActiveUsers[userId].TeamName) return;
                
                    await DiscordAPI.CreateMessage(_messageReaction.ChannelId, $"{userId} удален из {ActiveUsers[userId].TeamName} команды", null, false, null, null, null, null);

                    OnPlayerLeft?.Invoke(userId);
                    ActiveUsers.Remove(userId);
                
                    await BotCommandHelper.ShowActivePlayers(_messageReaction, ActiveUsers);
                        
                    break;
                case GameStage.Choosing:
                    if (ActiveUsers[userId].ClassType != PlayerClassType.None)
                    {
                        ActiveUsers[userId].ClassType = PlayerClassType.None;
                        OnPlayerLeft?.Invoke(userId);
                    }
                        
                    break;
                case GameStage.Started:
                    break;
            }
        }
    }
}