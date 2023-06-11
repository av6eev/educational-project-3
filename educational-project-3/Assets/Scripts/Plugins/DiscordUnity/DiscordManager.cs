using System;
using Game;
using Plugins.DiscordUnity.DiscordUnity.API;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.DiscordAPI;

namespace Plugins.DiscordUnity
{
    public class DiscordManager : MonoBehaviour, IDiscordServerEvents, IDiscordMessageEvents, IDiscordStatusEvents
    {
        public GameManager Manager;
    
        #region Singleton
        public static DiscordManager Singleton { get; private set; }

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Singleton != this) return;
            
            DiscordAPI.UnregisterEventsHandler(this);
            DiscordAPI.Stop();
            Singleton = null;
        }
        #endregion

        private void Update()
        {
            DiscordAPI.Update();
            Manager?.SystemEngine.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Manager?.FixedSystemEngine.Update(Time.deltaTime);
        }

        public async void OnServerJoined(DiscordServer server) {}

        public void OnServerUpdated(DiscordServer server) {}

        public void OnServerLeft(DiscordServer server) {}

        public void OnServerBan(DiscordServer server, DiscordUser user) {}

        public void OnServerUnban(DiscordServer server, DiscordUser user) {}

        public void OnServerEmojisUpdated(DiscordServer server, DiscordEmoji[] emojis) {}

        public void OnServerMemberJoined(DiscordServer server, DiscordServerMember member) {}

        public void OnServerMemberUpdated(DiscordServer server, DiscordServerMember member) {}

        public void OnServerMemberLeft(DiscordServer server, DiscordServerMember member) {}

        public void OnServerMembersChunk(DiscordServer server, DiscordServerMember[] members, string[] notFound, DiscordPresence[] presences) {}

        public void OnServerRoleCreated(DiscordServer server, DiscordRole role)
        {
            Debug.Log("Role added: " + role.Name);
        }

        public void OnServerRoleUpdated(DiscordServer server, DiscordRole role) {}

        public void OnServerRoleRemove(DiscordServer server, DiscordRole role) {}

        //message events
        public void OnMessageCreated(DiscordMessage message)
        {
            Manager.BotModel.CheckRequestMessage(message);
        }

        public void OnMessageUpdated(DiscordMessage message) {}

        public void OnMessageDeleted(DiscordMessage message) {}

        public void OnMessageDeletedBulk(string[] messageIds) {}

        public void OnMessageReactionAdded(DiscordMessageReaction messageReaction)
        {
            switch (Manager.GameModel.GameStage)
            {
                case GameStage.Preparing:
                    Manager.BotModel.ChooseTeam(messageReaction);
                    break;
                case GameStage.Choosing:
                    Manager.BotModel.ChooseClass(messageReaction);
                    break;
                case GameStage.Started:
                    Manager.BotModel.ChooseAction(messageReaction);
                    break;
            }
        }

        public void OnMessageReactionRemoved(DiscordMessageReaction messageReaction)
        {
            Manager.BotModel.PlayerDeselectTeam(messageReaction.UserId, messageReaction.Emoji.Name);
        }

        public void OnMessageAllReactionsRemoved(DiscordMessageReaction messageReaction) {}

        public void OnMessageEmojiReactionRemoved(DiscordMessageReaction messageReaction) {}
        
        // discord status events
        public void OnPresenceUpdated(DiscordPresence presence) {}

        public void OnTypingStarted(DiscordChannel channel, DiscordUser user, DateTime timestamp) {}

        public void OnServerTypingStarted(DiscordChannel channel, DiscordServerMember member, DateTime timestamp) {}

        public void OnUserUpdated(DiscordUser user) {}
    }
}
