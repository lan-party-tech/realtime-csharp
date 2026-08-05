using Newtonsoft.Json;
namespace Supabase.Realtime.Presence
{
/// <summary>
/// Options used to initialize Realtime Presence
/// </summary>
public class PresenceOptions
{
    /// <summary>
    /// key option is used to track presence payload across clients
    /// </summary>
    [JsonProperty("key")]
    public string PresenceKey { get; set; }

    /// <summary>
    /// Opts this client into presence. Current Supabase Realtime servers only send the
    /// initial presence_state roster (and full presence sync) to clients that join with
    /// enabled=true — matching realtime-js, which sets it when presence listeners are
    /// bound. Set automatically by RealtimeChannel.Register for presence.
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Presence options.
    /// </summary>
    /// <param name="presenceKey"></param>
    public PresenceOptions(string presenceKey)
    {
        PresenceKey = presenceKey;
    }
}
}
