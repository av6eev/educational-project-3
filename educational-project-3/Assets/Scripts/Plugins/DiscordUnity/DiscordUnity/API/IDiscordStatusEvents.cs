using System;
using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity.API
{
    public interface IDiscordStatusEvents : IDiscordInterface
    {
        void OnPresenceUpdated(DiscordPresence presence);
        void OnTypingStarted(DiscordChannel channel, DiscordUser user, DateTime timestamp);
        void OnServerTypingStarted(DiscordChannel channel, DiscordServerMember member, DateTime timestamp);
        void OnUserUpdated(DiscordUser user);
    }
}
