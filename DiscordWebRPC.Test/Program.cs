using System;
using System.Threading.Tasks;

namespace DiscordWebRPC.Test
{
    class Program
    {
        static void Main(string[] args) => RunAsync(args).GetAwaiter().GetResult();

        static async Task RunAsync(string[] args)
        {
            var client = new DiscordWebRPC();
            client.OnDispatch += Client_OnDispatch;

            if(!await client.Connect())
            {
                Console.WriteLine("Error: failed to connect to discord");
                return;
            }

            client.GetGuilds();
            await Task.Delay(-1);
            //var invite = await client.SendInvite("roblox");
            //Console.WriteLine("Response: {0}", invite);
        }

        private static void Client_OnDispatch(object sender, DispatchEvent e)
        {
            Console.WriteLine("Got dispatch: {0}", e);
        }
    }
}
