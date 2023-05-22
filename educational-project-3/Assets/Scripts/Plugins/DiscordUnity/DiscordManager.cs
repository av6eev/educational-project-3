using System;
using Bot;
using Game;
using Plugins.DiscordUnity.DiscordUnity.API;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.DiscordAPI;

namespace Plugins.DiscordUnity
{
    public class DiscordManager : MonoBehaviour, IDiscordServerEvents, IDiscordMessageEvents, IDiscordStatusEvents
    {
        [NonSerialized] public string BotToken;
        public DiscordLogLevel LogLevel = DiscordLogLevel.None;
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
            if (Singleton == this)
            {
                DiscordAPI.UnregisterEventsHandler(this);
                DiscordAPI.Stop();
                Singleton = null;
            }
        }
        #endregion

        protected virtual async void Start()
        {
            Debug.Log("Game is starting...");

            DiscordAPI.Logger = new DiscordLogger(LogLevel);
            DiscordAPI.RegisterEventsHandler(this);
            await DiscordAPI.StartWithBot(BotToken);
            
            Debug.Log("Started!");
        }

        private void Update()
        {
            DiscordAPI.Update();
            Manager.SystemEngine.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Manager.FixedSystemEngine.Update(Time.deltaTime);
        }

        public async void OnServerJoined(DiscordServer server)
        {
            Debug.Log("hello world");
            await DiscordUnity.Rest.DiscordAPI.CreateMessage("1078053058321317963", "Hello World", null, false, null, null, null, null);
        }

        public void OnServerUpdated(DiscordServer server)
        {

        }

        public void OnServerLeft(DiscordServer server)
        {

        }

        public void OnServerBan(DiscordServer server, DiscordUser user)
        {

        }

        public void OnServerUnban(DiscordServer server, DiscordUser user)
        {

        }

        public void OnServerEmojisUpdated(DiscordServer server, DiscordEmoji[] emojis)
        {

        }

        public void OnServerMemberJoined(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerMemberUpdated(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerMemberLeft(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerMembersChunk(DiscordServer server, DiscordServerMember[] members, string[] notFound, DiscordPresence[] presences)
        {

        }

        public void OnServerRoleCreated(DiscordServer server, DiscordRole role)
        {
            Debug.Log("role added: " + role.Name);
        }

        public void OnServerRoleUpdated(DiscordServer server, DiscordRole role)
        {

        }

        public void OnServerRoleRemove(DiscordServer server, DiscordRole role)
        {

        }

        //message events
        public void OnMessageCreated(DiscordMessage message)
        {
            Manager.BotModel.CheckRequestMessage(message);
        }

        public void OnMessageUpdated(DiscordMessage message)
        {

        }

        public void OnMessageDeleted(DiscordMessage message)
        {
            Debug.Log("Message Deleted...");
            // DiscordUnity.Rest.DiscordAPI.CreateMessage(message.ChannelId, "Message Deleted: " + message.Content, null, false, null, null, null, null);
        }

        public void OnMessageDeletedBulk(string[] messageIds)
        {

        }

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

        public void OnMessageAllReactionsRemoved(DiscordMessageReaction messageReaction)
        {

        }

        public void OnMessageEmojiReactionRemoved(DiscordMessageReaction messageReaction)
        {
            Debug.Log("emoji removed");
        }
        
        // discord status events
        public void OnPresenceUpdated(DiscordPresence presence)
        {

        }

        public void OnTypingStarted(DiscordChannel channel, DiscordUser user, DateTime timestamp)
        {
            Debug.Log("started typing");
        }

        public void OnServerTypingStarted(DiscordChannel channel, DiscordServerMember member, DateTime timestamp)
        {
            Debug.Log("started typing");
        }

        public void OnUserUpdated(DiscordUser user)
        {

        }

        #region Logger
        public enum DiscordLogLevel
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Debug = 3
        }

        private class DiscordLogger : DiscordUnity.ILogger
        {
            private readonly DiscordLogLevel level;

            public DiscordLogger(DiscordLogLevel level)
            {
                this.level = level;
            }

            public void Log(string log)
            {
                if (level >= DiscordLogLevel.Debug)
                    Debug.Log(log);
            }

            public void LogWarning(string log)
            {
                if (level >= DiscordLogLevel.Warning)
                {
                    Debug.LogWarning(log);
                }
            }

            public void LogError(string log, Exception exception = null)
            {
                if (level >= DiscordLogLevel.Error)
                {
                    Debug.LogError(log);
                    Debug.LogError(exception);
                }
            }
        }
        #endregion
    }
}
