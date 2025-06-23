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
        if (buffer.Array == null)
        {
            _form.DisplayMessage("수신 버퍼 오류: 버퍼가 null입니다.");
            return;
        }

        if (buffer.Count < 4) // 최소 헤더 크기 (2바이트 size + 2바이트 packetId)
        {
            _form.DisplayMessage("수신 버퍼 오류: 패킷 데이터가 너무 짧습니다.");
            return;
        }

        ushort packetSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        if (buffer.Count < packetSize)
        {
            _form.DisplayMessage($"수신 버퍼 오류: 전체 패킷 크기({packetSize}바이트)에 못 미치는 데이터입니다.");
            return;
        }

        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        // UI 업데이트는 FormInvoke를 통해 UI 스레드에서 실행
        FormInvoke(() =>
        {
            switch (packetId)
            {
                case ConstPacketId.S_CHAT:
                    {
                        var chatPacket = ServerChatPacket.FromBytes(buffer);
                        _form.DisplayMessage($"[메시지] {chatPacket.Message}");
                        break;
                    }
                case ConstPacketId.S_LOGIN_OK:
                    {
                        var loginOkPacket = ServerLoginOkPacket.FromBytes(buffer);
                        OnLoginOk(loginOkPacket);
                        break;
                    }
                case ConstPacketId.S_WHISPER:
                    {
                        var whisperPacket = ServerWhisperPacket.FromBytes(buffer);
                        _form.DisplayMessage($"[Whisper from {whisperPacket.SenderUserId}] {whisperPacket.Message}");
                        break;
                    }
                case ConstPacketId.S_USER_LIST:
                    {
                        var userListPacket = ServerUserListPacket.FromBytes(buffer);
                        _form.UpdateUserList(userListPacket.UserIds);
                        break;
                    }
                case ConstPacketId.S_ROOM_CREATE_OK:
                    {
                        var roomCreateOkPacket = ServerRoomCreateOkPacket.FromBytes(buffer);
                        _form.DisplayMessage($"방 생성 성공: {roomCreateOkPacket.RoomId}");
                        break;
                    }
                case ConstPacketId.S_ROOM_LIST:
                    {
                        var roomListPacket = ServerRoomListPacket.FromBytes(buffer);
                        _form.UpdateRoomList(roomListPacket.RoomIds);
                        break;
                    }
                case ConstPacketId.S_ROOM_CHANGE_RESULT:
                    {
                        var resultPacket = ServerRoomChangeResultPacket.FromBytes(buffer);
                        if (resultPacket.Success)
                        {
                            _form.DisplayMessage($"방 변경 성공! 새로운 방 ID: {resultPacket.RoomId}");
                            _form.UpdateCurrentRoomDisplay($"Room {resultPacket.RoomId}"); // '현재 방' 라벨 업데이트
                        }
                        else
                        {
                            _form.DisplayMessage($"방 변경 실패! 요청한 방 ID: {resultPacket.RoomId}");
                        }
                        break;
                    }
                default:
                    _form.DisplayMessage($"알 수 없는 패킷 수신: ID={packetId}");
                    break;
            }
        });
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