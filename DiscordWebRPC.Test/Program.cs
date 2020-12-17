using System;
using System.Threading.Tasks;

namespace DiscordWebRPC.Test
{
    class Program
    {
        // switch me between rest and socket
        static void Main(string[] args) => 
            RunRestAsync(args).GetAwaiter().GetResult();

        static async Task RunRestAsync(string[] args)
        {
            var client = new DiscordRestRPC();
            var invite = await client.SendInvite("roblox");

            Console.WriteLine("Response: {0}", invite);
            await Task.Delay(-1);
        }

        static async Task RunSocketAsync(string[] args)
        {
            var client = new DiscordSocketRPC();
            client.OnDispatch += Client_OnDispatch;

            if(!await client.Connect())
            {
                Console.WriteLine("Error: failed to connect to discord");
                return;
            }

            var invite = await client.SendInvite("roblox");
            Console.WriteLine("Response: {0}", invite);
            await Task.Delay(-1);
        }

        private static void Client_OnDispatch(object sender, DispatchEvent e)
        {
            Console.WriteLine("Got dispatch: {0}", e);
        }
    }
}
