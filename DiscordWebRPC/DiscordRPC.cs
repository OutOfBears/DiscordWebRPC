using System.Collections.Generic;
using System.Linq;

namespace DiscordWebRPC
{
    // Constants used for discord RPC
    internal static class DiscordRPC
    {
        // Constant values, change at will with disocrd
        // How to connect to RPC
        public const short RPC_VERSION = 1;
        public const short RPC_PORT_RANGE = 10;
        public const short RPC_STARTING_PORT = 6463;
        public const short RPC_ENDING_PORT = RPC_STARTING_PORT - RPC_PORT_RANGE;

        // This is required
        public const string RPC_ORIGIN = "https://discord.com";

        // Dont change these methods
        public static IList<int> CreatePortRange(int portStart)
        {
            var list = Enumerable.Range(RPC_ENDING_PORT + 1, RPC_PORT_RANGE - (RPC_STARTING_PORT - portStart))
                .Reverse()
                .ToList();

            if (portStart < RPC_STARTING_PORT)
                list.AddRange(Enumerable.Range(portStart + 1, RPC_STARTING_PORT - portStart).Reverse());

           return list;
        }
    }
}
