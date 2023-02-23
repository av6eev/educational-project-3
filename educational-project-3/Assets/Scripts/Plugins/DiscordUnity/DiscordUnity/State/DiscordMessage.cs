using System;
using Plugins.DiscordUnity.DiscordUnity.Models;

namespace Plugins.DiscordUnity.DiscordUnity.State
{
    public class DiscordMessage
    {
        public string Id { get; internal set; }
        public DiscordServer Server => string.IsNullOrEmpty(GuildId) ? null : DiscordAPI.Servers[GuildId];
        public DiscordChannel Channel => string.IsNullOrEmpty(GuildId) ? DiscordAPI.PrivateChannels[ChannelId] : Server.Channels[ChannelId];
        public DiscordUser Author { get; internal set; }
        public string Content { get; internal set; }
        public DateTime Timestamp { get; internal set; }
        public DateTime? EditedTimestamp { get; internal set; }
        public bool Tts { get; internal set; }
        public bool MentionEveryone { get; internal set; }
        public MessageType Type { get; internal set; }

        public string GuildId;
        public string ChannelId;

        internal DiscordMessage(MessageModel model)
        {
            Id = model.Id;
            GuildId = model.GuildId;
            ChannelId = model.ChannelId;
            if (model.Author != null) Author = new DiscordUser(model.Author);
            Content = model.Content;
            Timestamp = model.Timestamp;
            EditedTimestamp = model.EditedTimestamp;
            Tts = model.Tts;
            MentionEveryone = model.MentionEveryone;
            Type = model.Type;
        }
    }

    public class DiscordReaction
    {
        public int Count { get; internal set; }
        public bool Me { get; internal set; }
        public DiscordEmoji Emoji { get; internal set; }

        internal DiscordReaction(ReactionModel model)
        {
            Count = model.Count;
            Me = model.Me;
            if (model.Emoji != null) Emoji = new DiscordEmoji(model.Emoji);
        }
    }
    //edit
    public class DiscordMessageReaction
    {
        public string UserId { get; internal set; }
        public string MessageId { get; internal set; }
        public DiscordServerMember Member { get; internal set; }
        public DiscordEmoji Emoji { get; internal set; }
        public string ChannelId { get; set; }
        public string GuildId { get; set; }

        internal DiscordMessageReaction(MessageReactionModel model)
        {
            UserId = model.UserId;
            MessageId = model.MessageId;

            if (model.Member != null) Member = new DiscordServerMember(model.Member);
            if (model.Emoji != null) Emoji = new DiscordEmoji(model.Emoji);

            ChannelId = model.ChannelId;
            GuildId = model.GuildId;

        }
    }
    //edit
}
