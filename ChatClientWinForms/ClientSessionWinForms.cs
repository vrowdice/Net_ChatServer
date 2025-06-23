// ChatClientWinForms/ClientSessionWinForms.cs
using ChatClientWinForms;
using ServerCore; // ServerCore 라이브러리 참조
using System;
using System.Net;
using System.Net.Sockets;

// UWP의 ClientSessionUnity 대신 Windows Forms용 ClientSession
public class ClientSessionWinForms : PacketSession
{
    public Form1 _form; // Form1 인스턴스 참조 (UI 업데이트용)
    public bool IsConnected { get; private set; } = false; // 연결 상태 추적

    // 생성자를 통해 Form1 인스턴스를 받습니다.
    public ClientSessionWinForms(Form1 form)
    {
        _form = form;
    }

    public override void OnConnected(EndPoint endPoint)
    {
        IsConnected = true;
        _form.DisplayMessage($"서버 연결 성공: {endPoint}");
        _form.SendLoginPacket();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        IsConnected = false;
        _form.DisplayMessage($"서버 연결 해제: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        if (buffer.Array == null || buffer.Offset < 0 || buffer.Offset + 2 > buffer.Count || buffer.Offset + 2 > buffer.Array.Length)
        {
            _form.DisplayMessage($"수신 버퍼 오류: 유효하지 않은 패킷 데이터.");
            return;
        }

        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        if (packetId == ConstPacketId.S_CHAT)
        {
            ServerChatPacket chatPacket = ServerChatPacket.FromBytes(buffer);
            _form.DisplayMessage($"[메시지] {chatPacket.Message}");
        }
        else if (packetId == ConstPacketId.S_LOGIN_OK)
        {
            ServerLoginOkPacket loginOkPacket = ServerLoginOkPacket.FromBytes(buffer);
            OnLoginOk(loginOkPacket);
        }
        else if (packetId == ConstPacketId.S_WHISPER)
        {
            ServerWhisperPacket whisper = ServerWhisperPacket.FromBytes(buffer);
            _form.DisplayMessage($"[Whisper from {whisper.SenderUserId}] {whisper.Message}");
        }
        else
        {
            _form.DisplayMessage($"알 수 없는 패킷 수신: ID={packetId}");
        }
    }

    public void OnLoginOk(ServerLoginOkPacket packet)
    {
        FormInvoke(() => _form.DisplayMessage(packet.Message));
    }

    public override void OnSend(int numOfBytes)
    {
        // _form.DisplayMessage($"Sent {numOfBytes} bytes."); // 디버깅용, 실제 앱에서는 주석 처리
    }

    public void FormInvoke(Action action)
    {
        if (_form.InvokeRequired)
        {
            _form.Invoke(action);
        }
        else
        {
            action();
        }
    }

}