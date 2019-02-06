using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperfightBot.Game
{
    static class GameContexts
    {
        private static Dictionary<ulong, GameContext> contexts = new Dictionary<ulong, GameContext>();

        public static GameContext getContext(ulong guildId)
        {
            if (!contexts.TryGetValue(guildId, out GameContext result))
            {
                result = new GameContext(guildId);

                contexts.Add(guildId, result);
            }

            return result;
        }
    }
}
