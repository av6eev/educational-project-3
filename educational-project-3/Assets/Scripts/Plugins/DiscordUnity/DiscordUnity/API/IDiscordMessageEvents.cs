using Plugins.DiscordUnity.DiscordUnity.State;

namespace Plugins.DiscordUnity.DiscordUnity.API
{
    public interface IDiscordMessageEvents : IDiscordInterface
    {
        void OnMessageCreated(DiscordMessage message);
        void OnMessageUpdated(DiscordMessage message);
        void OnMessageDeleted(DiscordMessage message);

        void OnMessageDeletedBulk(string[] messageIds);

        void OnMessageReactionAdded(DiscordMessageReaction reactionMessage);
        void OnMessageReactionRemoved(DiscordMessageReaction reactionMessage);

        void OnMessageAllReactionsRemoved(DiscordMessageReaction reactionMessage);
        void OnMessageEmojiReactionRemoved(DiscordMessageReaction reactionMessage);
    }
}
