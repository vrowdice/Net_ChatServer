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
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        if (packetId == ConstPacketId.C_LOGIN)
        {
            ClientLoginPacket loginPacket = ClientLoginPacket.FromBytes(buffer);
            UserId = loginPacket.UserId;
            Console.WriteLine($"User logged in: {UserId}");
        }
        else if (packetId == ConstPacketId.C_CHAT)
        {
            PacketHandler.HandlePacket(this, buffer);
        }
        else
        {
            Console.WriteLine($"Unknown packet received: {packetId}");
        }
    }

    public override void OnSend(int numOfBytes)
    {
        // 필요하면 구현
    }
}
