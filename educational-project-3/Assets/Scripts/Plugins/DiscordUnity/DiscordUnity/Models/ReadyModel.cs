﻿using Newtonsoft.Json;

namespace Plugins.DiscordUnity.DiscordUnity.Models
{
    internal class ReadyModel
    {
        [JsonProperty("v")]
        public int Version { get; set; }
        public UserModel User { get; set; }
        public ChannelModel[] PrivateChannels { get; set; }
        public GuildModel[] Guilds { get; set; }
        public string SessionId { get; set; }
        public int[] Shard { get; set; }
    }
}
