using System.Collections.Generic;

namespace Plugins.DiscordUnity.DiscordUnity.Models
{
    internal class IdentityModel
    {
        public string Token { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
