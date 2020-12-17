using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordWebRPC
{
    // Constants used for discord RPC
    public static class DiscordRPC
    {
        // Constant values, change at will with disocrd
        // How to connect to RPC
        public const short RPC_VERSION = 1;
        public const short RPC_PORT_RANGE = 10;
        public const short RPC_STARTING_PORT = 6463;
        public const short RPC_ENDING_PORT = RPC_STARTING_PORT - RPC_PORT_RANGE;

        // This is required
        public const string RPC_ORIGIN = "https://discord.com";
    }
}
