# DiscordWebRPC
Discord RPC via WebSockets in C#

## What is DiscordWebRPC?

DiscordWebRPC is a C# library which handles the dispatchment of Discord browser events to the native client.

## Currently implemented dispatch

- [x] Browser Invite (sends browser style invite request to Discord client)
- [x] Dispatch (undocumented - TODO)

## Examples

```cs
class Program
{
        static void Main(string[] args) => RunAsync(args).GetAwaiter().GetResult();

        static async Task RunAsync(string[] args)
        {
            var client = new DiscordWebRPC(); // create client instance
            client.OnDispatch += Client_OnDispatch; // attach dispatch event

            if(!await client.Connect()) // attempt to connect to 
            {
                Console.WriteLine("Error: failed to connect to discord");
                return;
            }

            var invite = await client.SendInvite("roblox");
            Console.WriteLine("Response: {0}", invite);
        }

        private static void Client_OnDispatch(object sender, DispatchEvent e)
        {
            Console.WriteLine("Got dispatch: {0}", e); // debug output
        }
}
```
