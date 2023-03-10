using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Player;
using Plugins.DiscordUnity.DiscordUnity.State;
using Unity.VisualScripting;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.Rest.DiscordAPI;

namespace Bot
{
    
    public class BotModel
    {
        public Action<string> OnPlayerEntered;
        public Action<string> OnPlayerLeft;
        public Action OnGameStarting;
        
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
        
        public void CheckInGameMessages(DiscordMessage message)
        {
            if (message.Author.Bot is null or false)
            {
                
            }
        }
        
        public async void AddUsers(DiscordMessageReaction reaction)
        {
            _messageReaction = reaction;

            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;

            var firstTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.FirstTeamEmoji);
            var secondTeam = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.SecondTeamEmoji);
            var firstTeamDictionary = firstTeam?.Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.FirstTeamEmoji);
            var secondTeamDictionary = secondTeam?.Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.SecondTeamEmoji);
            var teams = new Dictionary<DiscordUser, string>();
            
            teams.AddRange(firstTeamDictionary);
            teams.AddRange(secondTeamDictionary);

            foreach (var model in from user in teams where CheckNewUser(user.Key) select new PlayerModel(user.Key.Id, user.Value, user.Key.Username))
            {
                ActiveUsers.Add(model.Id, model);
            }

            await BotCommandHelper.ShowActivePlayers(ActiveUsers);

            if (ActiveUsers.Count == _manager.GameDescriptions.World.MaxPlayersCount)
            {
                _manager.GameModel.GameStage = GameStage.Choosing;
            }
        }

        public async void ChooseClass(DiscordMessageReaction reaction)
        {
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;
            
            var melee = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.MeleeEmoji);
            var archer = await DiscordAPI.GetReactions(reaction.ChannelId, reaction.MessageId, BotCommandHelper.ArcherEmoji);
            var meleeDictionary = melee?.Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.MeleeEmoji);
            var archerDictionary = archer?.Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.ArcherEmoji);
            var teams = new Dictionary<DiscordUser, string>();

            teams.AddRange(meleeDictionary);            
            teams.AddRange(archerDictionary);            
            
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
            
            if (ActiveUsers.Values.Count(user => user.ClassType != PlayerClassType.None) == _manager.GameDescriptions.World.MaxPlayersCount)
            {
                _manager.GameModel.GameStage = GameStage.Started;
                
                await BotCommandHelper.OnGameStarted(_messageReaction.ChannelId);
                
                OnGameStarting?.Invoke();
            }
        }
        
        public async void ChooseAction(DiscordMessageReaction reaction)
        {
            if (reaction.Member.User.Bot != null && reaction.Member.User.Bot != false) return;
            
            var moveTopAction = (await DiscordAPI.GetReactions(BotCommandHelper.ChannelId, reaction.MessageId, BotCommandHelper.MoveTopEmoji))?
                .Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.MoveTopEmoji);
            var moveBottomAction = (await DiscordAPI.GetReactions(BotCommandHelper.ChannelId, reaction.MessageId, BotCommandHelper.MoveBottomEmoji))?
                .Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.MoveBottomEmoji);
            var moveLeftAction = (await DiscordAPI.GetReactions(BotCommandHelper.ChannelId, reaction.MessageId, BotCommandHelper.MoveLeftEmoji))?
                .Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.MoveLeftEmoji);
            var moveRightAction = (await DiscordAPI.GetReactions(BotCommandHelper.ChannelId, reaction.MessageId, BotCommandHelper.MoveRightEmoji))?
                .Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.MoveRightEmoji);
            var hitAction = (await DiscordAPI.GetReactions(BotCommandHelper.ChannelId, reaction.MessageId, BotCommandHelper.HitActionEmoji))?
                .Data.ToArray().ToDictionary(user => user, user => BotCommandHelper.HitActionEmoji);
            
            var actions = new Dictionary<DiscordUser, string>();
            var activePlayer = _manager.GameModel.ActivePlayer;
            
            var direction = Vector3.zero;
            var rotationAngle = Vector3.zero;
            
            actions.AddRange(moveTopAction);
            actions.AddRange(moveBottomAction);
            actions.AddRange(moveLeftAction);
            actions.AddRange(moveRightAction);
            actions.AddRange(hitAction);

            foreach (var action in actions.Where(action => CheckExistUser(action.Key)))
            {
                if (action.Key.Id == activePlayer.Id)
                {
                    switch (action.Value)
                    {
                        case BotCommandHelper.MoveTopEmoji:
                            direction = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
                            rotationAngle = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
                            break;
                        case BotCommandHelper.MoveBottomEmoji:
                            direction = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
                            rotationAngle = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
                            break;
                        case BotCommandHelper.MoveLeftEmoji:
                            direction = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3( -1, 0, 0) : new Vector3(1, 0, 0);
                            rotationAngle = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, -90, 0) : new Vector3(0, 90, 0);
                            break;
                        case BotCommandHelper.MoveRightEmoji:
                            direction = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
                            rotationAngle = activePlayer.TeamName == BotCommandHelper.FirstTeamEmoji ? new Vector3(0, 90, 0) : new Vector3(0, -90, 0);
                            break;
                        case BotCommandHelper.HitActionEmoji:
                            break;
                    }    
                }
            }

            var oldPosition = new Vector3(activePlayer.Position.x, 0, activePlayer.Position.z);
            
            if (_manager.FloorModel.Cells.ContainsKey(oldPosition))
            {
                _manager.FloorModel.Cells[oldPosition].IsActive = false;
            }

            activePlayer.SetPosition(direction, rotationAngle);
            activePlayer.Move(direction);
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
                
                    await BotCommandHelper.ShowActivePlayers(ActiveUsers);
                        
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
        
        private bool CheckNewUser(DiscordUser user)
        {
            return !user.Id.Equals(BotId) && !ActiveUsers.ContainsKey(user.Id);
        }

        private bool CheckExistUser(DiscordUser user)
        {
            return !user.Id.Equals(BotId) && ActiveUsers.ContainsKey(user.Id);
        }
    }
}