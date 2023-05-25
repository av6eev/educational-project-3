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
        public Action<string> OnPlayerDeselectTeam;
        public Action<string> OnGameStarting;
        
        private string _botId = string.Empty;
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
                if (_botId.Equals(string.Empty))
                {
                    _botId = message.Author.Id;
                }
                
                Debug.Log("Response message send: " + message.Content + ", from: " + message.Author.Username + ", messageID: " + message.Id);

                action = BotCommandHelper.ResponseCommands.TryGetValue(message.Content, out var callback) ? callback : null;
            }

            action?.Invoke(message);
        }
        
        public async void ChooseTeam(DiscordMessageReaction reaction)
        {
            _messageReaction = reaction;

            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;

            var user = reaction.Member.User;
            
            if (!CheckUser(user)) return;
            
            var model = new PlayerModel(user.Id, reaction.Emoji.Name, user.Username);
            ActiveUsers.Add(model.Id, model);

            await BotCommandHelper.ShowActivePlayers(ActiveUsers, reaction.ChannelId);

            if (ActiveUsers.Count == _manager.GameDescriptions.World.MaxPlayersCount)
            {
                _manager.GameModel.GameStage = GameStage.Choosing;
            }
        }

        public async void ChooseClass(DiscordMessageReaction reaction)
        {
            _messageReaction = reaction;
            
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;

            var newUser = reaction.Member.User;

            if (!CheckUser(newUser)) return;

            var model = ActiveUsers[newUser.Id];

            model.ClassType = reaction.Emoji.Name switch
            {
                BotCommandHelper.MeleeEmoji => PlayerClassType.Melee,
                BotCommandHelper.ArcherEmoji => PlayerClassType.Archer,
                BotCommandHelper.MageEmoji => PlayerClassType.Mage,
                _ => PlayerClassType.None
            };

            if (ActiveUsers.Values.Count(user => user.ClassType != PlayerClassType.None) == _manager.GameDescriptions.World.MaxPlayersCount)
            {
                _manager.GameModel.GameStage = GameStage.Started;
                
                await BotCommandHelper.OnGameStarted(_messageReaction.ChannelId);
                
                OnGameStarting?.Invoke(_messageReaction.ChannelId);
            }
        }
        
        public void ChooseAction(DiscordMessageReaction reaction)
        {
            var activePlayer = _manager.GameModel.ActivePlayer;
            
            _messageReaction = reaction;
            
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;
            if (reaction.Member.User.Id != activePlayer.Id) return;

            switch (reaction.Emoji.Name)
            {
                case BotCommandHelper.HitActionEmoji:
                    activePlayer.Attack();
                    return;
                default:
                    activePlayer.Move(reaction.Emoji.Name);
                    break;
            }    
        }

        public async void PlayerDeselectTeam(string userId, string emojiName)
        {
            if (!ActiveUsers.ContainsKey(userId)) return;
            
            switch (_manager.GameModel.GameStage)
            {
                case GameStage.Preparing:
                    if (emojiName != ActiveUsers[userId].TeamName) return;
                
                    await DiscordAPI.CreateMessage(_messageReaction.ChannelId, $"{userId} удален из {ActiveUsers[userId].TeamName} команды", null, false, null, null, null, null);

                    OnPlayerDeselectTeam?.Invoke(userId);
                    ActiveUsers.Remove(userId);
                
                    await BotCommandHelper.ShowActivePlayers(ActiveUsers, _messageReaction.ChannelId);
                    break;
                case GameStage.Choosing:
                    if (ActiveUsers[userId].ClassType != PlayerClassType.None)
                    {
                        ActiveUsers[userId].ClassType = PlayerClassType.None;
                        OnPlayerDeselectTeam?.Invoke(userId);
                    }
                    break;
                case GameStage.Started:
                    break;
            }
        }

        private bool CheckUser(DiscordUser user)
        {
            if (user.Id.Equals(_botId)) return false;
            
            switch (_manager.GameModel.GameStage)
            {
                case GameStage.Preparing:
                    return !user.Id.Equals(_botId) && !ActiveUsers.ContainsKey(user.Id);
                case GameStage.Choosing:
                    return !user.Id.Equals(_botId) && ActiveUsers.ContainsKey(user.Id) && ActiveUsers[user.Id].ClassType == PlayerClassType.None;
                case GameStage.Started:
                    break;
            }

            return false;
        }
    }
}