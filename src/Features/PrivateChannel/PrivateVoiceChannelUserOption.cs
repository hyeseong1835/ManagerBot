public class PrivateVoiceChannelUserOption
{
    public ulong UserId  { get; set; }
    public PrivateVoiceChannelPreset[] Presets  { get; set; }

    public PrivateVoiceChannelUserOption(ulong userId, PrivateVoiceChannelPreset[] presets)
    {
        this.UserId = userId;
        this.Presets = presets;
    }
}