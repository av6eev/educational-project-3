using System;
using Bot;
using Game;
using Plugins.DiscordUnity.DiscordUnity.API;
using Plugins.DiscordUnity.DiscordUnity.State;
using UnityEngine;
using Utilities;
using DiscordAPI = Plugins.DiscordUnity.DiscordUnity.DiscordAPI;

namespace Plugins.DiscordUnity
{
    public class DiscordManager : MonoBehaviour, IDiscordServerEvents, IDiscordMessageEvents, IDiscordStatusEvents
    {
        [SerializeField] private string BotToken;
        public DiscordLogLevel LogLevel = DiscordLogLevel.None;
        
        private readonly GameManager _manager = new();
        public GameView View;
        private readonly ControllerEngine _controllerEngine = new();

        private BotModel _botModel;
    
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
            Debug.Log("Starting Discord Unity ...");

            _manager.GameView = View;
            _manager.GameDescriptions = new GameDescriptions(View.DescriptionsCollection);
            
            _botModel = new BotModel();
            _manager.BotModel = _botModel;
            
            _controllerEngine.Add(new BotPresenter(_manager.BotModel, _manager));
            
            DiscordAPI.Logger = new DiscordLogger(LogLevel);
            DiscordAPI.RegisterEventsHandler(this);
            await DiscordAPI.StartWithBot(BotToken);

            _controllerEngine.Activate();
            
            Debug.Log("DiscordUnity started.");
        }

        private void Update()
        {
            DiscordAPI.Update();
        }

        public async void OnServerJoined(DiscordServer server)
        {
            // server.Channels.Values.FirstOrDefault(x => x.Type == DiscordUnity.Models.ChannelType.GUILD_TEXT)?.CreateMessage("Hello World!", null, null, null, null, null, null);
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
            _botModel.CheckRequestMessage(message);
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

        public async void OnMessageReactionAdded(DiscordMessageReaction messageReaction)
        {
            // if(messageReaction.Member.User.Bot == null || messageReaction.Member.User.Bot == false)
            // {
            //     Debug.Log("reaction added to: " + messageReaction.MessageId + ", from: " + messageReaction.UserId + ", reaction: " + messageReaction.Emoji.User + ", " + messageReaction.Emoji.User);
            //
            //     await DiscordUnity.Rest.DiscordAPI.CreateMessage(messageReaction.ChannelId, "User: " + messageReaction.Member.User.Username + " sent Emoji: " + messageReaction.Emoji.Name, null, false, null, null, null, null);
            //
            //     RestResult<DiscordUser[]> reactionResult;
            //     reactionResult = await DiscordUnity.Rest.DiscordAPI.GetReactions(messageReaction.ChannelId, messageReaction.MessageId, "👍");
            //
            //     DiscordUser[] array;
            //     array = reactionResult.Data.ToArray();
            //
            //     for (int i = 0; i < array.Length; i++)
            //     {
            //         Debug.Log(array[i].Username + ", has entered thumbs up");
            //     }
            //
            //     Debug.Log(messageReaction.MessageId);
            // }
        }

        public void OnMessageReactionRemoved(DiscordMessageReaction messageReaction)
        {
            Debug.Log("reaction removed");
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
