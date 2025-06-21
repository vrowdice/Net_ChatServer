// ChatClientWinForms/PacketHandler.cs
using System;
using ServerCore;

public class PacketHandler
{
    public static Action<ClientSessionWinForms, ArraySegment<byte>>[] Handler { get; private set; }

    public static void Init()
    {
        Handler = new Action<ClientSessionWinForms, ArraySegment<byte>>[2000];
        Handler[ConstPacketId.S_CHAT] = HandleSChat;
        Handler[ConstPacketId.S_LOGIN_OK] = HandleSLoginOk;
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
}
