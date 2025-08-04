using System.Runtime.CompilerServices;

public class KeyValueTextException : Exception
{
    public KeyValueTextException()
        : base()
    { }

    public KeyValueTextException(string? message)
        : base(message)
    { }

    public KeyValueTextException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
public enum KeyValueTextTokenType : byte
{
    None = 0,
    Key = 1,
    Value = 2,
    End = 3,
}
public struct KeyValueTextReader
{
    Stream stream;
    bool isReadAllStream;

    byte[] buffer;
    int readBufferIndex;
    int usedBufferSize;

    KeyValueTextTokenType curTokenType;
    int curTokenStartIndex;
    int curTokenReadLength;


    public ArraySlice<byte> CurTokenSlice
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new ArraySlice<byte>(buffer, curTokenStartIndex, curTokenReadLength);
    }
    public ReadOnlyMemory<byte> CurTokenMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new ReadOnlyMemory<byte>(buffer, curTokenStartIndex, curTokenReadLength);
    }


    public KeyValueTextReader(Stream stream, byte[] buffer)
    {
        this.stream = stream;
        this.isReadAllStream = false;

        this.buffer = buffer;
        this.readBufferIndex = 0;
        this.usedBufferSize = 0;

        this.curTokenType = KeyValueTextTokenType.None;
        this.curTokenStartIndex = 0;
        this.curTokenReadLength = 0;
    }


    public bool ReadKey()
    {
        switch (curTokenType)
        {
            case KeyValueTextTokenType.Value:
            {
                // 키 읽기
                if (false == ReadKeyToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                curTokenType = KeyValueTextTokenType.Key;
                return true;
            }
            case KeyValueTextTokenType.Key:
            {
                // 값 이전임: 값까지 스킵
                if (false == SkipToNextValue())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 키 읽기
                if (false == ReadKeyToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                return true;
            }
            case KeyValueTextTokenType.None:
            {
                if (false == ReadKeyToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                curTokenType = KeyValueTextTokenType.Key;
                return true;
            }
            case KeyValueTextTokenType.End:
            {
                return false;
            }
            default: throw new KeyValueTextException($"예상하지 못한 토큰 타입: {curTokenType}");
        }
    }

    public bool ReadValue()
    {
        switch (curTokenType)
        {
            case KeyValueTextTokenType.Key:
            {
                // 공백 건너뛰기
                if (false == SkipToNextValue())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 값 읽기
                if (false == ReadValueToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                curTokenType = KeyValueTextTokenType.Value;
                return true;
            }
            case KeyValueTextTokenType.Value:
            {
                // 키 읽기
                if (false == ReadKeyToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 공백 건너뛰기
                if (false == SkipToNextValue())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 값 읽기
                if (false == ReadValueToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                return true;
            }
            case KeyValueTextTokenType.None:
            {
                // 키 건너뛰기
                if (false == SkipKey())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 공백 건너뛰기
                if (false == SkipToNextValue())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                // 값 읽기
                if (false == ReadValueToEnd())
                {
                    curTokenType = KeyValueTextTokenType.End;
                    return false;
                }

                return true;
            }
            case KeyValueTextTokenType.End:
            {
                return false;
            }
            default: throw new KeyValueTextException($"예상하지 못한 토큰 타입: {curTokenType}");
        }
    }


    bool ReadKeyToEnd()
    {
        for (; ; )
        {
            // 현재 버퍼를 끝까지 읽음
            if (readBufferIndex >= usedBufferSize)
            {
                if (false == ReadStreamWithKeepToken())
                    return false;
            }

            byte curByte = buffer[readBufferIndex];

            // 쌍점임: 키 읽기 종료
            if (curByte == ':')
            {
                curTokenReadLength = readBufferIndex - curTokenStartIndex;
                return true;
            }

            continue;
        }
    }

    bool ReadValueToEnd()
    {
        for (; ; )
        {
            // 현재 버퍼를 끝까지 읽음
            if (readBufferIndex >= usedBufferSize)
            {
                if (false == ReadStreamWithKeepToken())
                    return false;
            }

            byte curByte = buffer[readBufferIndex];

            // 줄바꿈임: 값 읽기 종료
            if (curByte == '\n')
            {
                curTokenReadLength = readBufferIndex - curTokenStartIndex;
                return true;
            }

            readBufferIndex++;
        }
    }


    bool SkipKey()
    {
        byte curByte;

        for (; ; )
        {
            // 현재 버퍼를 끝까지 읽음
            if (readBufferIndex >= usedBufferSize)
            {
                if (false == ReadStream())
                {
                    return false;
                }
            }

            curByte = buffer[readBufferIndex];

            // 쌍점임: 키 스킵 종료
            if (curByte == ':')
            {
                readBufferIndex++;
                curTokenType = KeyValueTextTokenType.Key;
                return true;
            }

            // 줄바꿈임: 오류 (키 뒤에는 쌍점이 와야 합니다.)
            if (curByte == '\n')
                throw new KeyValueTextException("키 뒤에는 쌍점이 와야 합니다.");

            readBufferIndex++;
        }
    }
    bool SkipToNextKey()
    {
        byte curByte;

        for (; ; )
        {
            readBufferIndex++;

            // 현재 버퍼를 끝까지 읽음
            if (readBufferIndex >= usedBufferSize)
            {
                if (false == ReadStream())
                {
                    return false;
                }
            }

            curByte = buffer[readBufferIndex];

            // 줄바꿈: 값 토큰 종료
            if (curByte == '\n')
            {
                curTokenReadLength = 0;
                return true;
            }
        }
    }

    bool SkipToNextValue()
    {
        byte curByte;

        for (; ; )
        {
            // 현재 버퍼를 끝까지 읽음
            if (readBufferIndex >= usedBufferSize)
            {
                if (false == ReadStream())
                {
                    return false;
                }
            }

            curByte = buffer[readBufferIndex];

            // 띄어쓰기임: 계속 탐색
            if (curByte == ' ')
            {
                readBufferIndex++;
                continue;
            }

            // 줄바꿈: 오류 (키 뒤에는 줄바꿈 문자가 올 수 없습니다.)
            if (curByte == '\n')
                throw new KeyValueTextException("키 뒤에는 줄바꿈 문자가 올 수 없습니다.");

            // 띄어쓰기가 아님: 현재 토큰 시작 위치로 설정
            curTokenStartIndex = readBufferIndex;
            curTokenReadLength = 1;
            return true;
        }
    }


    bool ReadStream()
    {
        if (isReadAllStream)
        {
            return false;
        }

        // 스트림 읽기
        usedBufferSize = stream.Read(buffer, 0, buffer.Length);
        readBufferIndex = 0;

        // 스트림을 모두 읽었음
        if (usedBufferSize != buffer.Length)
        {
            isReadAllStream = true;
        }

        // 더 이상 읽을 데이터가 없음
        if (usedBufferSize == 0)
        {
            return false;
        }

        return true;
    }

    bool ReadStreamWithKeepToken()
    {
        if (curTokenStartIndex == 0)
        {

        }

        // 현재 토큰 당기기
        buffer.AsSpan(curTokenStartIndex, curTokenReadLength).CopyTo(buffer);

        if (isReadAllStream)
        {
            return false;
        }

        // 스트림 읽기
        usedBufferSize = stream.Read(buffer, curTokenReadLength, buffer.Length - curTokenReadLength) + curTokenReadLength;
        readBufferIndex = curTokenReadLength;

        // 스트림을 모두 읽었음
        if (usedBufferSize != buffer.Length)
        {
            isReadAllStream = true;
        }

        // 더 이상 읽을 데이터가 없음
        if (usedBufferSize == curTokenReadLength)
        {
            return false;
        }

        return true;
    }
}
