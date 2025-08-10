namespace ManagerBot.Utils.PriorityMethod;

public enum PriorityMethodOption : byte
{
    /// <summary>
    /// 반환형이 ValutTask, Task 등인 경우 await하고, <br/>
    /// 반환형이 그 외인 경우에는 동기적으로 실행합니다.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// 코드를 비동기적으로 실행합니다. (ValueTask, Task 등의 반환형이 아니면 예외 발생)
    /// </summary>
    Await,

    /// <summary>
    /// 코드를 동기적으로 실행합니다.
    /// </summary>
    NonAwait,
}