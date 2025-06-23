using System;
using System.Net;
using ServerCore;

public class ClientSession : PacketSession
{
    public string UserId { get; set; } = null;
    public int? CurrentRoomId { get; set; } = null;
    public ChatRoom CurrentRoom { get; set; }
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
        ushort packetId = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + 2);
        PacketHandler.HandlePacket(this, buffer);
    }


    public override void OnSend(int numOfBytes)
    {
        // 필요하면 구현
    }
}
