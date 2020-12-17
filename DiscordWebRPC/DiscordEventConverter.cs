using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordWebRPC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal class DiscordEventAttribute : Attribute
    {
        public string Command;

        public DiscordEventAttribute(string command)
        {
            Command = command;
        }
    }

    internal class DiscordEventConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RPCEvent);

        public override bool CanWrite { get { return false; } }


        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) return null;
            JObject obj = JObject.Load(reader);
            RPCEvent rpcEvent = new RPCEvent();
            rpcEvent.Cmd = obj["cmd"].ToString();
            rpcEvent.Event = obj["evt"].ToString();
            if(obj["nonce"].Type == JTokenType.String && Guid.TryParse(obj["nonce"].ToString(), out var nonce))
                rpcEvent.Nonce = nonce;
            if(obj["data"].Type == JTokenType.Object)
                rpcEvent.Data = GetData(rpcEvent.Cmd, obj["data"]);
            return rpcEvent;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static object GetData(string cmd, JToken obj)
        {
            if (!obj.HasValues) return null;
            if (EventClasses.TryGetValue(cmd, out var type))
                return obj.ToObject(type);

            return obj.ToObject<object>();
        }


        private static readonly Dictionary<string, Type> EventClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .ToList()
            .Where(type => type.GetCustomAttributes(typeof(DiscordEventAttribute), true).Length > 0)
            .ToDictionary(x => (x.GetCustomAttributes(typeof(DiscordEventAttribute), true)[0] as DiscordEventAttribute).Command);
            

        public static readonly DiscordEventConverter Singleton = new DiscordEventConverter();
    }

}
