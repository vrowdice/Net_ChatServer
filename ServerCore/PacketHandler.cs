using System;
using ServerCore;

public static class PacketHandler
{
    // 패킷 ID별 처리 함수 등록용 배열
    public static Action<PacketSession, ArraySegment<byte>>[] Handler { get; private set; }

    public static void Init()
    {
        Console.WriteLine("[PacketHandler] Init() called.");
        Handler = new Action<PacketSession, ArraySegment<byte>>[2000];

        // 패킷 ID 등록
        Handler[ConstPacketId.C_CHAT] = HandleCChat;
        Handler[ConstPacketId.C_LOGIN] = HandleCLogin;
        Handler[ConstPacketId.C_WHISPER] = HandleCWhisper;
    }

    public static void HandlePacket(PacketSession session, ArraySegment<byte> buffer)
    {
        if (buffer.Array == null || buffer.Count < PacketSession.HeaderSize)
        {
            Console.WriteLine("[PacketHandler] Invalid buffer data.");
            return;
        }

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        if (packetId >= Handler.Length || Handler[packetId] == null)
        {
            Console.WriteLine($"[PacketHandler] No handler for Packet ID: {packetId}");
            return;
        }

        try
        {
            Handler[packetId].Invoke(session, buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] Error handling packet {packetId}: {ex.Message}");
        }
    }

    private static void HandleCChat(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
        {
            Console.WriteLine("[PacketHandler] Invalid session type for C_CHAT.");
            return;
        }

        try
        {
            var chatPacket = ClientChatPacket.FromBytes(buffer);
            string userId = clientSession.UserId ?? "Unknown";

            Console.WriteLine($"[PacketHandler] Chat from [{userId}]: {chatPacket.Message}");

            var broadcast = new ServerChatPacket
            {
                Message = $"[{userId}] {chatPacket.Message}"
            };

            SessionManager.Instance.Broadcast(broadcast.ToBytes());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] HandleCChat error: {ex.Message}");
        }
    }

    private static void HandleCLogin(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
        {
            Console.WriteLine("[PacketHandler] Invalid session type for C_LOGIN.");
            return;
        }

        try
        {
            var loginPacket = ClientLoginPacket.FromBytes(buffer);
            clientSession.UserId = loginPacket.UserId;

            Console.WriteLine($"[PacketHandler] User '{clientSession.UserId}' logged in.");

            var okPacket = new ServerLoginOkPacket
            {
                Message = $"로그인 성공: {clientSession.UserId}"
            };
            clientSession.Send(okPacket.ToBytes());

            // 로그인 응답 먼저 보내고 사용자 리스트는 약간의 지연 후 전송
            System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(100);
                SessionManager.Instance.BroadcastUserList();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] HandleCLogin error: {ex.Message}");
        }
    }

    private static void HandleCWhisper(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
            return;

        var whisperPacket = ClientWhisperPacket.FromBytes(buffer);

        string senderId = clientSession.UserId ?? "Unknown";
        string targetId = whisperPacket.TargetUserId;

        var serverWhisperPacket = new ServerWhisperPacket
        {
            SenderUserId = senderId,
            Message = whisperPacket.Message
        };

        var targetSession = SessionManager.Instance.FindByUserId(targetId);
        if (targetSession != null)
        {
            targetSession.Send(serverWhisperPacket.ToBytes());
        }
        else
        {
            Console.WriteLine($"[PacketHandler] Whisper target '{targetId}' not found.");
        }
    }
}
