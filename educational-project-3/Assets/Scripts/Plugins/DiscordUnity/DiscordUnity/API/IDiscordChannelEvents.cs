using System;
using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity.API
{
    public interface IDiscordChannelEvents : IDiscordInterface
    {
        void OnChannelCreated(DiscordChannel channel);
        void OnChannelUpdated(DiscordChannel channel);
        void OnChannelDeleted(DiscordChannel channel);

        void OnChannelPinsUpdated(DiscordChannel channel, DateTime? lastPinTimestamp);
    }
}
