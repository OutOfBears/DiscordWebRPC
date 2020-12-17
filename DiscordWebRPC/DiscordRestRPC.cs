using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordWebRPC
{
    public class DiscordRestRPC
    {
        private readonly HttpClient _client;
        private short _lastUsedPort;

        public DiscordRestRPC()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Connection.Clear();
            _client.DefaultRequestHeaders.ConnectionClose = true;
            _client.DefaultRequestHeaders.Add("Origin", DiscordRPC.RPC_ORIGIN);
            _client.Timeout = TimeSpan.FromSeconds(1);

            _lastUsedPort = DiscordRPC.RPC_STARTING_PORT;
        }

        public async Task<InviteEvent> SendInvite(string code)
        {
            return await RPCRequest(HttpMethod.Post, "INVITE_BROWSER", new InviteEvent
            {
                Code = code
            }) as InviteEvent;
        }

        private async Task<object> RPCRequest<T>(HttpMethod method, string cmd, T rpcRequestArgs)
        {
            var currentPort = _lastUsedPort;
            var startingPort = currentPort;
            var nonce = Guid.NewGuid();

            HttpResponseMessage response = null;
            var json = JsonConvert.SerializeObject(new
            {
                cmd,
                nonce,
                args = rpcRequestArgs as object
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType.CharSet = null;

            do
            {
                if (currentPort < DiscordRPC.RPC_ENDING_PORT)
                    currentPort = DiscordRPC.RPC_STARTING_PORT;

                try
                {
                    var url = $"http://127.0.0.1:{currentPort}/rpc?v={DiscordRPC.RPC_VERSION}";
                    response = await _client.PostAsync(url, content).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                        break;
                } catch // timeouts cause exceptions to be thrown
                {
                    continue;
                }
            } while (--currentPort != startingPort);

            if (response != null && response.StatusCode == HttpStatusCode.OK) 
            {
                _lastUsedPort = currentPort;
                try
                {
                    var rpcResponse = JsonConvert.DeserializeObject<RPCEvent>(await response.Content.ReadAsStringAsync());
                    if (rpcResponse != null && rpcResponse.Data != null)
                        return rpcResponse.Data;
                }
                catch
                {
                    return default;
                }
            }
            
            return default;
        }
    }
}
