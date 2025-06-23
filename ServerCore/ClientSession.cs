using System;
using System.Net;
using ServerCore;

public class ClientSession : PacketSession
{
    public string UserId { get; set; } = null;

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"[ClientSession] Connected: {endPoint}");
        SessionManager.Instance.Add(this);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"[ClientSession] Disconnected: {endPoint} (User: {UserId ?? "Unknown"})");
        SessionManager.Instance.Remove(this);

        // 유저 목록 갱신 브로드캐스트
        SessionManager.Instance.BroadcastUserList();
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        switch (packetId)
        {
            case ConstPacketId.C_LOGIN:
            case ConstPacketId.C_CHAT:
            case ConstPacketId.C_WHISPER:
                PacketHandler.HandlePacket(this, buffer);
                break;

            default:
                Console.WriteLine($"Unknown packet received: {packetId}");
                break;
        }
    }


    public override void OnSend(int numOfBytes)
    {
        // 필요하면 구현
    }
}
