
namespace ManagerBot.SlashCommandSystem.InfoCommandGroup;
public class InfoCommands : SlashCommandGroup
{
    public override string Name => "정보";
    public override string Description => "정보 관련 명령어입니다.";

    public override SlashCommandGroupElement[] SlashCommandGroupDefine()
    {
        return new SlashCommandGroupElement[]
        {
            new Command("명령어", "봇의 명령어 목록을 확인합니다."),
            new User("유저", "유저 정보를 확인합니다."),
        };
    }
}
