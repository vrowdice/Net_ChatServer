// Server
// ClientSession.cs
using System;
using System.Net;
using System.Net.Sockets;
using ServerCore;
using System.Collections.Generic;

public class ClientSession : PacketSession
{
    public string UserId { get; set; } = null;

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"[ClientSession] Connected: {endPoint}");
        SessionManager.Instance.Add(this); // SessionManager에 세션 추가
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"[ClientSession] Disconnected: {endPoint}");
        SessionManager.Instance.Remove(this); // SessionManager에서 세션 제거
    }

    // PacketSession에서 패킷을 받은 후 호출되는 추상 메서드
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketHandler.HandlePacket(this, buffer); // PacketHandler에게 패킷 처리 위임
    }

    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"[ClientSession] Sent {numOfBytes} bytes.");
    }
}