
namespace ManagerBot.SlashCommandSystem.DebugCommandGroup;
public class DebugCommands : SlashCommandGroup
{
    public override string Name => "디버그";
    public override string Description => "개발자를 위한 디버그 명령어입니다.";

    public override SlashCommandGroupElement[] SlashCommandGroupDefine()
    {
        return new SlashCommandGroupElement[]
        {
            new SubCommandGroup(
                "인코딩",
                "인코딩 명령어입니다.",
                new Encoding.Base64("base64", "Base64 형식으로 변환합니다."),
                new Encoding.KoreanString("koreanstring", "KoreanString 형식으로 변환합니다.")
            ),
            new SubCommandGroup(
                "디코딩",
                "디코딩 명령어입니다.",
                new Decoding.Base64("base64", "Base64 형식을 디코딩합니다."),
                new Decoding.KoreanString("koreanstring", "KoreanString 형식을 디코딩합니다.")
            ),
            new User("유저", "유저 정보를 확인합니다."),
            new Channel("채널", "채널 정보를 확인합니다."),
            new Category("카테고리", "카테고리 정보를 확인합니다."),
            new Message("메시지", "메시지 정보를 확인합니다.")
        };
    }
}
