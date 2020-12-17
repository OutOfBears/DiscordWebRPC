using System;

namespace DiscordWebRPC
{
    using Newtonsoft.Json;

    // Main Event
    [JsonConverter(typeof(DiscordEventConverter))]
    internal class RPCEvent
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("evt")]
        public object Event { get; set; }

        [JsonProperty("nonce")]
        public Guid Nonce { get; set; }
    }

    // Events
    [DiscordEvent("INVITE_BROWSER")]
    public class InviteEvent
    {
        [JsonProperty("invite", NullValueHandling = NullValueHandling.Include)]
        public InviteEventData.Invite Invite { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }


    [DiscordEvent("DISPATCH")]
    public class DispatchEvent
    {
        [JsonProperty("v")]
        public long Version { get; set; }

        [JsonProperty("config")]
        public DispatchEventData.Config Config { get; set; }
    }

    // Event Subclasses
    namespace InviteEventData
    {

        public partial class Invite
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("guild")]
            public Guild Guild { get; set; }

            [JsonProperty("channel")]
            public Channel Channel { get; set; }

            [JsonProperty("inviter")]
            public Inviter Inviter { get; set; }

            [JsonProperty("approximate_member_count")]
            public long ApproximateMemberCount { get; set; }

            [JsonProperty("approximate_presence_count")]
            public long ApproximatePresenceCount { get; set; }
        }

        public partial class Channel
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public long Type { get; set; }
        }

        public partial class Guild
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("splash")]
            public object Splash { get; set; }

            [JsonProperty("banner")]
            public object Banner { get; set; }

            [JsonProperty("description")]
            public object Description { get; set; }

            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("features")]
            public object[] Features { get; set; }

            [JsonProperty("verification_level")]
            public long VerificationLevel { get; set; }

            [JsonProperty("vanity_url_code")]
            public object VanityUrlCode { get; set; }
        }

        public partial class Inviter
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("discriminator")]
            public string Discriminator { get; set; }

            [JsonProperty("public_flags")]
            public long PublicFlags { get; set; }
        }
    }

    namespace DispatchEventData
    {
        public partial class Config
        {
            [JsonProperty("cdn_host")]
            public string CdnHost { get; set; }

            [JsonProperty("api_endpoint")]
            public string ApiEndpoint { get; set; }

            [JsonProperty("environment")]
            public string Environment { get; set; }
        }
    }
}
