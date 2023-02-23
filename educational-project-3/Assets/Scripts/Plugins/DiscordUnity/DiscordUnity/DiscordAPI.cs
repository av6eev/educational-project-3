using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugins.DiscordUnity.DiscordUnity.API;
using Plugins.DiscordUnity.DiscordUnity.Models;
using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity
{
    // Finish Http Calls
    // - Extra guild calls and some others
    // Finish Internal Models
    // - AuditLogChangeKey?
    // Finish State
    // - More and better states
    // - Fill in easy access methods to Http Calls

    public static partial class DiscordAPI
    {
        private static string url;
        private static string token;
        private static string session;
        private static bool acked = false;
        private static int sequence;

        private static Task listener;
        private static ClientWebSocket socket;

        public static bool IsActive { get; private set; }
        public static ILogger Logger { get; set; }

        internal static readonly JsonSerializer JsonSerializer;
        private static readonly JsonSerializerSettings JsonSettings;
        private static readonly SemaphoreSlim sendLock;
        private static TaskCompletionSource<bool> startTask;
        private static Queue<Action> callbacks;
        internal static CancellationTokenSource CancelSource;
        internal static DiscordInterfaces interfaces;

        static DiscordAPI()
        {
            Logger = new Logger();
            interfaces = new DiscordInterfaces();
            sendLock = new SemaphoreSlim(1, 1);
            callbacks = new Queue<Action>();

            JsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            JsonSerializer = JsonSerializer.CreateDefault(JsonSettings);
        }

        public static void RegisterEventsHandler(IDiscordInterface e)
            => interfaces.AddEventHandler(e);
        public static void UnregisterEventsHandler(IDiscordInterface e) 
            => interfaces.RemoveEventHandler(e);
        internal static void Sync(Action callback)
            => callbacks.Enqueue(callback);

        /// <summary> Starts DiscordUnity with a bot token. </summary>
        /// <param name="botToken">A token received by creating a bot on Discord's developer portal.</param>
        public static async Task<bool> StartWithBot(string botToken)
        {
            if (IsActive) return false;

            if (string.IsNullOrWhiteSpace(botToken))
            {
                Logger.LogError("Token is invalid!");
                return false;
            }

            token = botToken;
            IsActive = true;
            State.DiscordAPI.InitializeState();
            CancelSource = new CancellationTokenSource();
            Rest.DiscordAPI.Client = new HttpClient();
            Rest.DiscordAPI.Client.DefaultRequestHeaders.Add("User-Agent", $"DiscordBot ({"https://github.com/DiscordUnity/DiscordUnity"}, {"1.0"})");
            Rest.DiscordAPI.Client.DefaultRequestHeaders.Add("Authorization", $"Bot {token}");

            var gatewayResult = await Rest.DiscordAPI.GetBotGateway();

            if (!gatewayResult)
            {
                IsActive = false;
                Logger.LogError("Retrieving gateway failed: " + gatewayResult.Exception);
                Stop();
                return false;
            }

            url = gatewayResult.Data.Url + "?v=6&encoding=json";
            Logger.Log("Gateway received: " + url);

            socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(url), CancelSource.Token);

            if (socket.State != WebSocketState.Open)
            {
                IsActive = false;
                Logger.LogError("Could not connect with Discord: " + socket.CloseStatusDescription);
                Stop();
                return false;
            }

            Logger.Log("Connected.");
            listener = Listen();

            PayloadModel<IdentityModel> identity = new PayloadModel<IdentityModel>
            {
                Op = 2,
                Data = new IdentityModel
                {
                    Token = token,
                    Properties = new Dictionary<string,string>()
                    {
                        { "$os", Environment.OSVersion.ToString() },
                        { "$browser", "DiscordUnity" },
                        { "$device", "DiscordUnity" }
                    }
                }
            };

            startTask = new TaskCompletionSource<bool>();
            await Send(JsonConvert.SerializeObject(identity, JsonSettings));
            return await startTask.Task;
        }

        /// <summary> Stops DiscordUnity. </summary>
        public static void Stop()
        {
            if (startTask != null && !startTask.Task.IsCompleted)
                Sync(() => startTask.SetResult(false));

            startTask = null;
            IsActive = false;
            url = null;
            token = null;
            session = null;
            socket?.Dispose();
            socket = null;
            Rest.DiscordAPI.Client?.Dispose();
            Rest.DiscordAPI.Client = null;
            CancelSource?.Cancel();
            CancelSource = null;
            Logger.Log("DiscordUnity stopped.");
            interfaces.OnDiscordAPIClosed();
        }

        /// <summary> Updates DiscordUnity and hooks async calls back to calling thread. Without this, DiscordUnity will not function. </summary>
        public static void Update()
        {
            while (callbacks.Count > 0)
            {
                try
                {
                    callbacks.Dequeue()();
                }

                catch (Exception e)
                {
                    Logger.LogError("Error occured in a callback.", e);
                }
            }
        }

        private static async Task Resume()
        {
            listener.Dispose();
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Failed heartbeat ack", CancelSource.Token);
            await socket.ConnectAsync(new Uri(url), CancelSource.Token);
            listener = Listen();

            PayloadModel<ResumeModel> resume = new PayloadModel<ResumeModel>
            {
                Op = 6,
                Data = new ResumeModel
                {
                    Token = token,
                    SessionId = session,
                    Sequence = sequence
                }
            };

            await Send(JsonConvert.SerializeObject(resume, JsonSettings));
        }

        private static async Task Listen()
        {
            byte[] buffer = new byte[8192];

            while (IsActive && socket?.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancelSource.Token);

                if (!result.EndOfMessage)
                {
                    Logger.LogWarning("Received unexpected partial message.");
                    continue;
                }

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        await OnSocketMessage(Encoding.UTF8.GetString(buffer));
                        break;

                    case WebSocketMessageType.Close:
                        Logger.LogError($"Socket closed: ({socket.CloseStatus}) {socket.CloseStatusDescription}");
                        Stop();
                        return;

                    default:
                    case WebSocketMessageType.Binary:
                        Logger.LogWarning("Received unexpected type of message: " + result.MessageType);
                        break;
                }

                buffer = new byte[8192];
            }
        }

        private static async void Heartbeat(int interval)
        {
            while (IsActive && socket?.State == WebSocketState.Open)
            {
                if (acked)
                {
                    acked = false;

                    await Send(JsonConvert.SerializeObject(new PayloadModel<int>
                    {
                        Op = 1,
                        Data = sequence
                    }, JsonSettings));

                    Logger.Log("Heartbeat");
                    await Task.Delay(interval);
                }

                else
                {
                    await Resume();
                }
            }
        }

        private static async Task Send(string message)
        {
            Logger.Log("Send Message: " + message);
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

            await sendLock.WaitAsync();

            try
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancelSource.Token);
            }

            finally
            {
                sendLock.Release();
            }
        }

        private static async Task OnSocketMessage(string json)
        {
            try
            {
                Logger.Log("Received Message: " + json);
                PayloadModel payload = JsonConvert.DeserializeObject<PayloadModel>(json, JsonSettings);

                if (payload.Sequence.HasValue)
                    sequence = payload.Sequence.Value;

                switch (payload.Op)
                {
                    case 0: // Event Dispatch
                        break;
                    case 1: // Heartbeat
                        await Send(JsonConvert.SerializeObject(new PayloadModel<int>
                        {
                            Op = 1,
                            Data = sequence
                        }, JsonSettings));

                        Logger.Log("Heartbeat");
                        break;
                    case 7: // Reconnect
                        Logger.Log("Reconnect Requested");
                        await Resume();
                        break;
                    case 9: // Invalid Session
                        Logger.LogError("Received invalid session from discord.");
                        var resume = payload.As<bool>().Data;
                        if (resume) await Resume();
                        else Stop();
                        break;
                    case 10: // Hello
                        acked = true;
                        var heartbeat = payload.As<HeartbeatModel>().Data;
                        Heartbeat(heartbeat.HeartbeatInterval);
                        break;
                    case 11: // Heartbeat Ack
                        Logger.Log("Heatbeat Ack");
                        acked = true;
                        break;
                }

                if (!string.IsNullOrEmpty(payload.Event))
                {
                    switch (payload.Event.ToLower())
                    {
                        // ------ API ------ 

                        case "ready":
                            {
                                Logger.Log("Ready.");

                                var ready = payload.As<ReadyModel>().Data;

                                Sync(() =>
                                {
                                    State.DiscordAPI.Version = ready.Version;
                                    session = ready.SessionId;
                                    State.DiscordAPI.User = new DiscordUser(ready.User);

                                    foreach (var channel in ready.PrivateChannels)
                                        State.DiscordAPI.PrivateChannels[channel.Id] = new DiscordChannel(channel);

                                    foreach (var guild in ready.Guilds)
                                        State.DiscordAPI.Servers[guild.Id] = new DiscordServer(guild);

                                    startTask.SetResult(true);
                                    interfaces.OnDiscordAPIOpen();
                                });
                            }
                            break;
                        case "resumed":
                            {
                                Logger.Log("Resumed.");
                                Sync(() => interfaces.OnDiscordAPIResumed());
                            }
                            break;
                        case "reconnect":
                            {
                                Logger.Log("Reconnect.");
                                await Resume();
                            }
                            break;

                        //  ------ Channels ------ 

                        case "channel_create":
                            {
                                var channel = payload.As<ChannelModel>().Data;

                                Sync(() =>
                                {
                                    if (!string.IsNullOrEmpty(channel.GuildId))
                                    {
                                        State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id] = new DiscordChannel(channel);
                                        interfaces.OnChannelCreated(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                    }

                                    else
                                    {
                                        State.DiscordAPI.PrivateChannels[channel.Id] = new DiscordChannel(channel);
                                        interfaces.OnChannelCreated(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                    }
                                });
                            }
                            break;
                        case "channel_update":
                            {
                                var channel = payload.As<ChannelModel>().Data;

                                Sync(() =>
                                {
                                    if (!string.IsNullOrEmpty(channel.GuildId))
                                    {
                                        State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id] = new DiscordChannel(channel);
                                        interfaces.OnChannelUpdated(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                    }

                                    else
                                    {
                                        State.DiscordAPI.PrivateChannels[channel.Id] = new DiscordChannel(channel);
                                        interfaces.OnChannelUpdated(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                    }
                                });
                            }
                            break;
                        case "channel_delete":
                            {
                                var channel = payload.As<ChannelModel>().Data;

                                Sync(() =>
                                {
                                    if (!string.IsNullOrEmpty(channel.GuildId))
                                    {
                                        interfaces.OnChannelDeleted(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                        State.DiscordAPI.Servers[channel.GuildId].Channels.Remove(channel.Id);
                                    }

                                    else
                                    {
                                        interfaces.OnChannelDeleted(State.DiscordAPI.Servers[channel.GuildId].Channels[channel.Id]);
                                        State.DiscordAPI.PrivateChannels.Remove(channel.Id);
                                    }
                                });
                            }
                            break;
                        case "channel_pins_update":
                            {
                                var channelPin = payload.As<ChannelPinsModel>().Data;

                                Sync(() =>
                                {
                                    if (!string.IsNullOrEmpty(channelPin.GuildId))
                                    {
                                        State.DiscordAPI.Servers[channelPin.GuildId].Channels[channelPin.ChannelId].LastPinTimestamp = channelPin.LastPinTimestamp;
                                        interfaces.OnChannelPinsUpdated(State.DiscordAPI.Servers[channelPin.GuildId].Channels[channelPin.ChannelId], channelPin.LastPinTimestamp);
                                    }

                                    else
                                    {
                                        State.DiscordAPI.PrivateChannels[channelPin.ChannelId].LastPinTimestamp = channelPin.LastPinTimestamp;
                                        interfaces.OnChannelPinsUpdated(State.DiscordAPI.Servers[channelPin.GuildId].Channels[channelPin.ChannelId], channelPin.LastPinTimestamp);
                                    }
                                });
                            }
                            break;

                        //  ------ Servers ------ 

                        case "guild_create":
                            {
                                var guild = payload.As<GuildModel>().Data;

                                Sync(() =>
                                {
                                    State.DiscordAPI.Servers[guild.Id] = new DiscordServer(guild);
                                    interfaces.OnServerJoined(State.DiscordAPI.Servers[guild.Id]);
                                });
                            }
                            break;
                        case "guild_update":
                            {
                                var guild = payload.As<GuildModel>().Data;

                                Sync(() =>
                                {
                                    State.DiscordAPI.Servers[guild.Id] = new DiscordServer(guild);
                                    interfaces.OnServerUpdated(State.DiscordAPI.Servers[guild.Id]);
                                });
                            }
                            break;
                        case "guild_delete":
                            {
                                var guild = payload.As<GuildModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnServerLeft(State.DiscordAPI.Servers[guild.Id]);
                                    State.DiscordAPI.Servers.Remove(guild.Id);
                                });
                            }
                            break;
                        case "guild_ban_add":
                            {
                                var guildBan = payload.As<GuildBanModel>().Data;

                                Sync(() =>
                                {
                                    State.DiscordAPI.Servers[guildBan.GuildId].Bans[guildBan.User.Id] = new DiscordUser(guildBan.User);
                                    interfaces.OnServerBan(State.DiscordAPI.Servers[guildBan.GuildId], State.DiscordAPI.Servers[guildBan.GuildId].Bans[guildBan.User.Id]);
                                });
                            }
                            break;
                        case "guild_ban_remove":
                            {
                                var guildBan = payload.As<GuildBanModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnServerUnban(State.DiscordAPI.Servers[guildBan.GuildId], State.DiscordAPI.Servers[guildBan.GuildId].Bans[guildBan.User.Id]);
                                    State.DiscordAPI.Servers[guildBan.GuildId].Bans.Remove(guildBan.User.Id);
                                });
                            }
                            break;
                        case "guild_emojis_update":
                            {
                                var guildEmojis = payload.As<GuildEmojisModel>().Data;
                                State.DiscordAPI.Servers[guildEmojis.GuildId].Emojis = guildEmojis.Emojis.ToDictionary(x => x.Id, x => new DiscordEmoji(x));

                                Sync(() =>
                                {
                                    interfaces.OnServerEmojisUpdated(State.DiscordAPI.Servers[guildEmojis.GuildId], new DiscordEmoji[State.DiscordAPI.Servers[guildEmojis.GuildId].Emojis.Count]);
                                });

                            }
                            break;
                        case "guild_member_add":
                            {
                                var guildMember = payload.As<GuildMemberModel>().Data;
                                State.DiscordAPI.Servers[guildMember.GuildId].Members[guildMember.User.Id] = new DiscordServerMember(guildMember);

                                Sync(() =>
                                {
                                    interfaces.OnServerMemberJoined(State.DiscordAPI.Servers[guildMember.GuildId], State.DiscordAPI.Servers[guildMember.GuildId].Members[guildMember.User.Id]);
                                });
                            }
                            break;
                        case "guild_member_update":
                            {
                                var guildMember = payload.As<GuildMemberModel>().Data;
                                State.DiscordAPI.Servers[guildMember.GuildId].Members[guildMember.User.Id] = new DiscordServerMember(guildMember);

                                Sync(() =>
                                {
                                    interfaces.OnServerMemberUpdated(State.DiscordAPI.Servers[guildMember.GuildId], State.DiscordAPI.Servers[guildMember.GuildId].Members[guildMember.User.Id]);
                                });

                            }
                            break;
                        case "guild_member_remove":
                            {
                                var guildMember = payload.As<GuildMemberModel>().Data;
                                State.DiscordAPI.Servers[guildMember.GuildId].Members.Remove(guildMember.User.Id);

                                Sync(() =>
                                {
                                    interfaces.OnServerMemberLeft(State.DiscordAPI.Servers[guildMember.GuildId], State.DiscordAPI.Servers[guildMember.GuildId].Members[guildMember.User.Id]);
                                });
                            }
                            break;
                        
                        case "guild_members_chunk":
                            {
                                var guildMembersChunk = payload.As<GuildMembersChunkModel>().Data;
                                /*
                                Servers[guildMembersChunk.GuildId].Members = new DiscordServerMember[guildMembersChunk];

                                Sync(() =>
                                {
                                    interfaces.OnServerMembersChunk(Servers[guildMembersChunk.GuildId], Servers[guildMembersChunk.GuildId].Members, guildMembersChunk.NotFound, guildMembersChunk.Presences );
                                });
                                */

                            }
                            break;
                        case "guild_role_create":
                            {
                                var guildRole = payload.As<GuildRoleModel>().Data;
                                State.DiscordAPI.Servers[guildRole.GuildId].Roles[guildRole.Role.Id] = new DiscordRole(guildRole.Role);

                                Sync(() =>
                                {
                                    interfaces.OnServerRoleCreated(State.DiscordAPI.Servers[guildRole.GuildId], State.DiscordAPI.Servers[guildRole.GuildId].Roles[guildRole.Role.Id]);
                                });
                            }
                            break;
                        case "guild_role_update":
                            {
                                var guildRole = payload.As<GuildRoleModel>().Data;
                                State.DiscordAPI.Servers[guildRole.GuildId].Roles[guildRole.Role.Id] = new DiscordRole(guildRole.Role);

                                Sync(() =>
                                {
                                    interfaces.OnServerRoleUpdated(State.DiscordAPI.Servers[guildRole.GuildId], State.DiscordAPI.Servers[guildRole.GuildId].Roles[guildRole.Role.Id]);
                                });
                            }
                            break;
                        case "guild_role_delete":
                            {
                                var guildRoleId = payload.As<GuildRoleIdModel>().Data;
                                State.DiscordAPI.Servers[guildRoleId.GuildId].Roles.Remove(guildRoleId.RoleId);

                                Sync(() =>
                                {
                                    interfaces.OnServerRoleRemove(State.DiscordAPI.Servers[guildRoleId.GuildId], State.DiscordAPI.Servers[guildRoleId.GuildId].Roles[guildRoleId.RoleId]);
                                });
                            }
                            break;

                        //  ------ Invites ------ 

                        case "invite_create":
                            {
                                var invite = payload.As<InviteModel>().Data;
                                State.DiscordAPI.Servers[invite.GuildId].Invites[invite.Code] = new DiscordInvite(invite);

                                Sync(() =>
                                {
                                    interfaces.InviteCreated(State.DiscordAPI.Servers[invite.GuildId], State.DiscordAPI.Servers[invite.GuildId].Invites[invite.Code]);
                                });
                            }
                            break;
                        case "invite_delete":
                            {
                                var invite = payload.As<InviteModel>().Data;
                                State.DiscordAPI.Servers[invite.GuildId].Invites.Remove(invite.Code);

                                Sync(() =>
                                {
                                    interfaces.InviteDeleted(State.DiscordAPI.Servers[invite.GuildId], State.DiscordAPI.Servers[invite.GuildId].Invites[invite.Code]);
                                });
                            }
                            break;

                        //  ------ Messages ------ 

                        case "message_create":
                            {
                                var message = payload.As<MessageModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageCreated(new DiscordMessage(message));
                                });
                            }
                            break;
                        case "message_update":
                            {
                                var message = payload.As<MessageModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageUpdated(new DiscordMessage(message));
                                });
                            }
                            break;
                        case "message_delete":
                            {
                                var message = payload.As<MessageModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageDeleted(new DiscordMessage(message));
                                });
                            }
                            break;
                        case "message_delete_bulk":
                            {
                                var messageBulk = payload.As<MessageBulkModel>().Data;

                                //need to add
                                Sync(() =>
                                {
                                    interfaces.OnMessageDeletedBulk(messageBulk.Ids);
                                });
                            }
                            break;
                        case "message_reaction_add":
                            {
                                var messageReaction = payload.As<MessageReactionModel>().Data;
                                var message = payload.As<MessageModel>().Data;
                                var reaction = payload.As<ReactionModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageReactionAdded(new DiscordMessageReaction (messageReaction));
                                });
                            }
                            break;
                        case "message_reaction_remove":
                            {
                                var messageReaction = payload.As<MessageReactionModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageReactionRemoved(new DiscordMessageReaction(messageReaction));
                                });
                            }
                            break;
                        case "message_reaction_remove_all":
                            {
                                var messageReaction = payload.As<MessageReactionModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageAllReactionsRemoved(new DiscordMessageReaction(messageReaction));
                                });
                            }
                            break;
                        case "message_reaction_remove_emoji":
                            {
                                var messageReaction = payload.As<MessageReactionModel>().Data;

                                Sync(() =>
                                {
                                    interfaces.OnMessageEmojiReactionRemoved(new DiscordMessageReaction(messageReaction));
                                });
                            }
                            break;

                        //  ------ Status ------ 

                        case "presence_update":
                            {
                                var presence = payload.As<PresenceModel>().Data;
                            }
                            break;

                        case "typing_start":
                            {
                                var typing = payload.As<TypingModel>().Data;
                            }
                            break;

                        case "user_update":
                            {
                                var user = payload.As<UserModel>().Data;
                            }
                            break;

                        //  ------ Voice ------ 

                        case "voice_state_update":
                            {
                                var voiceState = payload.As<VoiceStateModel>().Data;
                            }
                            break;
                        case "voice_server_update":
                            {
                                var voiceServer = payload.As<VoiceServerModel>().Data;
                            }
                            break;

                        //  ------ Webhooks ------ 

                        case "webhooks_update":
                            {
                                var webhook = payload.As<ServerWebhookModel>().Data;
                            }
                            break;
                    }
                }
            }

            catch (Exception exception)
            {
                Logger.LogError("Exception occured while processing message.", exception);
            }
        }
    }
}
