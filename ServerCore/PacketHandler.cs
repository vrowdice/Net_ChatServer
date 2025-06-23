// ServerCore 프로젝트에 포함
// PacketHandler.cs
using System;
using ServerCore;

public class PacketHandler
{
    // 각 패킷 ID에 해당하는 처리 함수들을 등록할 딕셔너리
    public static Action<PacketSession, ArraySegment<byte>>[] Handler { get; private set; }

    public static void Init()
    {
        Console.WriteLine("PacketHandler Init() called.");
        Handler = new Action<PacketSession, ArraySegment<byte>>[2000];
        Handler[ConstPacketId.C_CHAT] = HandleCChat;
        Handler[ConstPacketId.C_LOGIN] = HandleCLogin;
    }

    public static void HandlePacket(PacketSession session, ArraySegment<byte> buffer)
    {
        if (Handler == null)
        {
            Console.WriteLine("Handler array is null.");
            return;
        }
        if (buffer.Array == null || buffer.Count < PacketSession.HeaderSize)
        {
            Console.WriteLine("Invalid buffer data.");
            return;
        }

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        if (packetId >= Handler.Length || Handler[packetId] == null)
        {
            Console.WriteLine($"No handler for Packet ID: {packetId}");
            return;
        }

        Handler[packetId].Invoke(session, buffer);
    }


    private static void HandleCChat(PacketSession session, ArraySegment<byte> buffer)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null)
            return;

        ClientChatPacket chatPacket = ClientChatPacket.FromBytes(buffer);
        string userId = clientSession.UserId ?? "Unknown";

        Console.WriteLine($"Received chat message from [{userId}]: {chatPacket.Message}");

        ServerChatPacket broadcastPacket = new ServerChatPacket { Message = $"[{userId}] {chatPacket.Message}" };
        ArraySegment<byte> sendBuff = broadcastPacket.ToBytes();

        SessionManager.Instance.Broadcast(sendBuff);
    }


    public static void HandleCLogin(PacketSession session, ArraySegment<byte> buffer)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null)
            return;

        ClientLoginPacket loginPacket = ClientLoginPacket.FromBytes(buffer);

        clientSession.UserId = loginPacket.UserId;

        Console.WriteLine($"User '{clientSession.UserId}' logged in.");

        // 로그인 성공 패킷 생성
        ServerLoginOkPacket okPacket = new ServerLoginOkPacket();
        okPacket.Message = $"Welcome, {clientSession.UserId}!";
        ArraySegment<byte> sendBuff = okPacket.ToBytes();

        clientSession.Send(sendBuff);

        // 모든 클라이언트에게 최신 유저 리스트 브로드캐스트
        SessionManager.Instance.BroadcastUserList();
    }
}