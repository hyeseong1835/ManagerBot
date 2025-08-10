using Discord.WebSocket;

public class TemporaryChannel
{
    public SocketThreadChannel thread;
    public SocketVoiceChannel? voiceChannel;


    public TemporaryChannel(SocketThreadChannel thread, SocketVoiceChannel? voiceChannel)
    {
        this.thread = thread;
        this.voiceChannel = voiceChannel;
    }
}