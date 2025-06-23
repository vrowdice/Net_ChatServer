//ServerCore/PacketHandler.cs
using ServerCore;
using System;
using System.Collections.Generic;

public static class PacketHandler
{
    // 패킷 ID별 처리 함수 등록용 배열
    public static Action<PacketSession, ArraySegment<byte>>[] Handler { get; private set; }

    public static void Init()
    {
        Handler = new Action<PacketSession, ArraySegment<byte>>[2000];

        // 패킷 ID 등록
        Handler[ConstPacketId.C_CHAT] = HandleCChat;
        Handler[ConstPacketId.C_LOGIN] = HandleCLogin;
        Handler[ConstPacketId.C_WHISPER] = HandleCWhisper;
        Handler[ConstPacketId.C_ROOM_CHANGE] = HandleCRoomChange;
        Handler[ConstPacketId.C_ROOM_CREATE] = HandleCRoomCreate;
        Handler[ConstPacketId.C_ROOM_LIST] = HandleCRoomList;
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

            if (clientSession.CurrentRoom != null)
            {
                clientSession.CurrentRoom.Broadcast(broadcast.ToBytes());
                Console.WriteLine($"[PacketHandler] Chat broadcasted in Room ID: {clientSession.CurrentRoom.Id}");
            }
            else
            {
                Console.WriteLine($"[PacketHandler] User '{userId}' is not in any room. Chat not broadcasted.");
            }
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

    // ServerCore/PacketHandler.cs (일부)

    private static void HandleCRoomChange(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
            return;

        try
        {
            var roomChangePacket = ClientRoomChangePacket.FromBytes(buffer);
            int requestedRoomId = roomChangePacket.RoomId;

            Console.WriteLine($"User '{clientSession.UserId}' requests room change to room ID: {requestedRoomId}");

            // 현재 방에서 세션 제거
            if (clientSession.CurrentRoom != null)
            {
                clientSession.CurrentRoom.RemoveSession(clientSession);
                Console.WriteLine($"User '{clientSession.UserId}' left room ID: {clientSession.CurrentRoom.Id}");
            }

            // 요청된 방으로 세션 추가
            ChatRoom targetRoom = RoomManager.Instance.FindRoom(requestedRoomId);

            if (targetRoom != null)
            {
                targetRoom.AddSession(clientSession);
                clientSession.CurrentRoom = targetRoom;
                Console.WriteLine($"User '{clientSession.UserId}' joined room ID: {targetRoom.Id} (Name: {targetRoom.Name})");

                // 클라이언트에 방 변경 성공 응답 전송
                var resultPacket = new ServerRoomChangeResultPacket
                {
                    Success = true,
                    RoomId = targetRoom.Id
                };
                clientSession.Send(resultPacket.ToBytes());
            }
            else
            {
                Console.WriteLine($"[PacketHandler] Room ID {requestedRoomId} not found. User '{clientSession.UserId}' remains in previous state.");
                // 클라이언트에 방 변경 실패 응답 전송
                var resultPacket = new ServerRoomChangeResultPacket
                {
                    Success = false,
                    RoomId = requestedRoomId
                };
                clientSession.Send(resultPacket.ToBytes());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] HandleCRoomChange error for user '{clientSession.UserId}': {ex.Message}");
        }
    }

    private static void HandleCRoomCreate(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
            return;

        try
        {
            var packet = ClientRoomCreatePacket.FromBytes(buffer);
            string roomName = packet.RoomName;

            Console.WriteLine($"[PacketHandler] Room create request: {roomName}");

            RoomManager.Instance.CreateRoomIfNotExist(roomName);

            var okPacket = new ServerRoomCreateOkPacket { RoomId = roomName };
            clientSession.Send(okPacket.ToBytes());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] HandleCRoomCreate error: {ex.Message}");
        }
    }

    private static void HandleCRoomList(PacketSession session, ArraySegment<byte> buffer)
    {
        if (session is not ClientSession clientSession)
            return;

        try
        {
            Console.WriteLine($"[PacketHandler] Room list request");

            var roomIds = RoomManager.Instance.GetAllRoomIds();

            if (roomIds == null || roomIds.Count == 0)
            {
                Console.WriteLine("[PacketHandler] No rooms available.");
                roomIds = new List<int>();
            }

            var listPacket = new ServerRoomListPacket
            {
                RoomIds = roomIds
            };

            clientSession.Send(listPacket.ToBytes());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PacketHandler] HandleCRoomList error: {ex.Message}");
        }
    }

}
