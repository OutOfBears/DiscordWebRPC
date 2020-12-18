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
            return await RPCRequest("INVITE_BROWSER", new InviteEvent
            {
                Code = code
            }) as InviteEvent;
        }

        private async Task<object> RPCRequest<T>(string cmd, T rpcRequestArgs)
        {
            var currentPort = _lastUsedPort;
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

            var portList = DiscordRPC.CreatePortRange(currentPort);
            foreach(var item in portList)
            {
                try
                {
                    var url = $"http://127.0.0.1:{currentPort}/rpc?v={DiscordRPC.RPC_VERSION}";
                    response = await _client.PostAsync(url, content).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                        break;
                }
                catch // timeouts cause exceptions to be thrown
                {
                    continue;
                }
            }

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                _lastUsedPort = currentPort;
                try
                {
                    var responseStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rpcResponse = JsonConvert.DeserializeObject<RPCEvent>(responseStr);
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
