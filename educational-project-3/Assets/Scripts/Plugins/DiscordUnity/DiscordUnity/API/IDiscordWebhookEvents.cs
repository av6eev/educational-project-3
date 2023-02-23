using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity.API
{
    public interface IDiscordWebhookEvents : IDiscordInterface
    {
        void OnWebhooksUpdated(DiscordChannel channel);
    }
}
