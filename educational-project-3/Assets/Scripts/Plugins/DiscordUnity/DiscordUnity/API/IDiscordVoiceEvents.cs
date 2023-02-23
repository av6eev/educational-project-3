using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity.API
{
    public interface IDiscordVoiceEvents : IDiscordInterface
    {
        void OnVoiceStateUpdated(DiscordVoiceState voiceState);
        void OnVoiceServerUpdated(DiscordServer server, string token, string endpoint);
    }
}
