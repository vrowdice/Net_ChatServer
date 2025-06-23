// ServerCore 프로젝트에 포함
// PacketCode.cs
public static class ConstPacketId
{
    public const ushort S_CHAT = 1001; // 서버 -> 클라이언트 채팅 메시지
    public const ushort C_CHAT = 1002; // 클라이언트 -> 서버 채팅 메시지
    public const ushort C_LOGIN = 1003; // 클라이언트 -> 서버 로그인 요청 (추가 기능 확장용)
    public const ushort S_LOGIN_OK = 1004; // 서버 -> 클라이언트 로그인 성공 (추가 기능 확장용)
    public const ushort C_REGISTER = 1005; // 클라이언트 -> 서버 회원가입 요청 (추가 기능 확장용)
    public const ushort C_WHISPER = 1006; // 클라이언트 -> 서버 귓속말
    public const ushort S_WHISPER = 1007; // 서버 -> 클라이언트 귓속말 수신
    public const ushort S_USER_LIST = 1008; // 서버 -> 클라이언트 사용자 목록 전송 (추가 기능 확장용)
}