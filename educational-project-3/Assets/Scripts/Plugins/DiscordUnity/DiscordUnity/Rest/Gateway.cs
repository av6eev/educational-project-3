using System.Threading.Tasks;
using Plugins.DiscordUnity.DiscordUnity.Models;

namespace Plugins.DiscordUnity.DiscordUnity.Rest
{
    public static partial class DiscordAPI
    {
        internal static Task<RestResult<GatewayModel>> GetBotGateway()
            => Get<GatewayModel>("/gateway/bot");
    }
}
