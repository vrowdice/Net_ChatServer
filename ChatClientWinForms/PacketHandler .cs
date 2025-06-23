// ChatClientWinForms/PacketHandler.cs
using System;
using ServerCore;

public class PacketHandler
{
    public static Action<ClientSessionWinForms, ArraySegment<byte>>[]? Handler { get; private set; }

    public static void Init()
    {
        Handler = new Action<ClientSessionWinForms, ArraySegment<byte>>[2000];
        Handler[ConstPacketId.S_CHAT] = HandleSChat;
        Handler[ConstPacketId.S_LOGIN_OK] = HandleSLoginOk;
        Handler[ConstPacketId.C_WHISPER] = HandleCWhisper;
        Handler[ConstPacketId.S_USER_LIST] = HandleSUserList;
        Handler[ConstPacketId.S_ROOM_CREATE_OK] = HandleSRoomCreateOk;
        Handler[ConstPacketId.S_ROOM_LIST] = HandleSRoomList;
    }

    private static void HandleSChat(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        var chatPacket = ServerChatPacket.FromBytes(buffer);
        session.FormInvoke(() => session._form.DisplayMessage($"[서버 메시지] {chatPacket.Message}"));
    }

    private static void HandleSLoginOk(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        var loginOkPacket = ServerLoginOkPacket.FromBytes(buffer);
        session.OnLoginOk(loginOkPacket);
    }

    public static void HandleCWhisper(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
            return;

        ClientWhisperPacket whisperPacket = ClientWhisperPacket.FromBytes(buffer);
        string senderId = clientSession.UserId ?? "Unknown";

        ClientSession targetSession = SessionManager.Instance.FindByUserId(whisperPacket.TargetUserId);
        if (targetSession == null)
        {
            Console.WriteLine($"[Whisper] 대상 사용자 없음: {whisperPacket.TargetUserId}");
            return;
        }

        ServerWhisperPacket response = new ServerWhisperPacket
        {
            SenderUserId = senderId,
            Message = whisperPacket.Message
        };

        targetSession.Send(response.ToBytes());

        Console.WriteLine($"[Whisper] {senderId} -> {whisperPacket.TargetUserId}: {whisperPacket.Message}");
    }

    private static void HandleSUserList(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        var userListPacket = ServerUserListPacket.FromBytes(buffer);
        session.FormInvoke(() => session._form.UpdateUserList(userListPacket.UserIds));
    }

    public static void HandleLogin(ClientSession session, ClientLoginPacket loginPacket)
    {
        session.UserId = loginPacket.UserId;

        var ok = new ServerLoginOkPacket { Message = $"로그인 성공: {loginPacket.UserId}" };
        session.Send(ok.ToBytes());

        Task.Run(async () =>
        {
            await Task.Delay(100);
            SessionManager.Instance.BroadcastUserList();
        });
    }

    private static void HandleSRoomCreateOk(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        var roomCreateOkPacket = ServerRoomCreateOkPacket.FromBytes(buffer);
        session.FormInvoke(() =>
        {
            session._form.DisplayRoomCreateResult(roomCreateOkPacket.RoomId);
        });
    }

    private static void HandleSRoomList(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        var roomListPacket = ServerRoomListPacket.FromBytes(buffer);
        session.FormInvoke(() =>
        {
            session._form.UpdateRoomList(roomListPacket.RoomIds);
        });
    }

    private static void HandleSRoomChangeResult(ClientSessionWinForms session, ArraySegment<byte> buffer)
    {
        ClientSessionWinForms clientSession = session as ClientSessionWinForms;
        if (clientSession == null) return;

        ServerRoomChangeResultPacket resultPacket = ServerRoomChangeResultPacket.FromBytes(buffer);

        clientSession.FormInvoke(() =>
        {
            if (resultPacket.Success)
            {
                session._form.DisplayMessage($"방 변경 성공! 새로운 방 ID: {resultPacket.RoomId}");
                session._form.UpdateCurrentRoomDisplay($"Room {resultPacket.RoomId}"); // 여기에 '현재 방' 라벨 업데이트
            }
            else
            {
                session._form.DisplayMessage($"방 변경 실패! 요청한 방 ID: {resultPacket.RoomId}");
            }
        });
    }
}
