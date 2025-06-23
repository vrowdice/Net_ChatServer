// ServerCore 프로젝트에 포함
// PacketCode.cs

public static class ConstPacketId
{
    // ================================
    // ① 클라이언트 → 서버 패킷 ID
    // ================================

    public const ushort C_CHAT = 1002;          // 채팅 메시지 전송
    public const ushort C_LOGIN = 1003;         // 로그인 요청
    public const ushort C_REGISTER = 1005;      // 회원가입 요청 (미사용 가능)
    public const ushort C_WHISPER = 1006;       // 귓속말 전송
    public const ushort C_ROOM_CHANGE = 1009;   // 방 전환 요청
    public const ushort C_ROOM_CREATE = 1010;   // 방 생성 요청
    public const ushort C_ROOM_LIST = 1011;     // 방 목록 요청

    // ================================
    // ② 서버 → 클라이언트 패킷 ID
    // ================================

    public const ushort S_CHAT = 1001;          // 채팅 메시지 수신
    public const ushort S_LOGIN_OK = 1004;      // 로그인 성공 응답
    public const ushort S_WHISPER = 1007;       // 귓속말 수신
    public const ushort S_USER_LIST = 1008;     // 사용자 목록 전달
    public const ushort S_ROOM_LIST = 1012;  // 방 목록 응답
    public const ushort S_ROOM_CREATE_OK = 1013; // 방 생성 성공 응답
    public const ushort S_ROOM_CHANGE_RESULT = 1014; // 방 전환 결과 응답
}
