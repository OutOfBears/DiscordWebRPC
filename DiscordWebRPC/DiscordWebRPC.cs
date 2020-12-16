using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Newtonsoft.Json;
using WebSocket4Net;

namespace DiscordWebRPC
{
    using TaskQueue = ConcurrentDictionary<Guid, TaskCompletionSource<object>>;

    public class DiscordWebRPC
    {
        // Constant values, change at will with disocrd
        // How to connect to RPC
        private const short RPC_VERSION = 1;
        private const short RPC_PORT_RANGE = 10;
        private const short RPC_STARTING_PORT = 6463;

        // This is required
        private const string RPC_ORIGIN = "https://discord.com";

        // private members
        private WebSocket _socket;
        private TaskQueue _taskQueue;
        private readonly JsonSerializerSettings _jsonSettings;

        // public events
        public event EventHandler<DispatchEvent> OnDispatch;

        public DiscordWebRPC()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            _taskQueue = new TaskQueue();
            _socket = null;
        }

        public async Task<InviteEvent> SendInvite(string code)
        {
            if (!Connected())
                throw new InvalidOperationException();

            var task = new TaskCompletionSource<object>();
            var task_id = Guid.NewGuid();
            _taskQueue[task_id] = task;

            SendEvent(task_id, "INVITE_BROWSER", new InviteEvent
            {
                Code = code
            });

            return await task.Task as InviteEvent;
        }

        public async Task<bool> Connect()
        {
            if (Connected())
                return true;

            var rpcPort = RPC_STARTING_PORT;
            _taskQueue.Clear();

            do
            {
                _socket = new WebSocket($"ws://127.0.0.1:{rpcPort--}?v={RPC_VERSION}", origin: RPC_ORIGIN);
                _socket.MessageReceived += _socket_OnMessage;
                _socket.NoDelay = true;

                if (await _socket.OpenAsync())
                    return true;

            } while (rpcPort >= RPC_STARTING_PORT - RPC_PORT_RANGE);

            return false;
        }


        private void _socket_OnMessage(object sender, MessageReceivedEventArgs e)
        {
            var obj = JsonConvert.DeserializeObject<RPCEvent>(e.Message);
            if (obj == null || obj.Nonce == null || !_taskQueue.TryGetValue(obj.Nonce, out var task))
            {
                if (obj.Cmd == "DISPATCH")
                    OnDispatch(this, obj.Data as DispatchEvent);
                
                return;
            }

            task.TrySetResult(obj.Data);
        }

        private void SendEvent<T>(Guid id, string name, T event_obj)
        {
            var obj = new
            {
                cmd = name,
                nonce = id.ToString(),
                args = event_obj as object
            };

            var serializedObj = JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
            _socket.Send(serializedObj);
        }

        private bool Connected()
        {
            if (_socket != null && _socket.State == WebSocketState.Open)
                return true;
            return false;
        }
    }
}
