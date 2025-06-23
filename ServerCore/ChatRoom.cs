using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Server/ChatRoom.cs (일부)

public class ChatRoom
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private List<ClientSession> _sessions = new List<ClientSession>();
    private object _lock = new object();

    public ChatRoom(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddSession(ClientSession session)
    {
        lock (_lock)
        {
            _sessions.Add(session);
            // 새로운 유저 입장 메시지 브로드캐스트
            Broadcast(new ServerChatPacket { Message = $"[시스템] {session.UserId}님이 방에 입장했습니다." }.ToBytes());
            BroadcastUserList(); // 사용자 목록 갱신
        }
    }

    public void RemoveSession(ClientSession session)
    {
        lock (_lock)
        {
            _sessions.Remove(session);
            // 유저 퇴장 메시지 브로드캐스트
            Broadcast(new ServerChatPacket { Message = $"[시스템] {session.UserId}님이 방을 떠났습니다." }.ToBytes());
            BroadcastUserList(); // 사용자 목록 갱신
        }
    }

    public void Broadcast(ArraySegment<byte> buffer)
    {
        lock (_lock)
        {
            foreach (ClientSession session in _sessions)
            {
                session.Send(buffer);
            }
        }
    }

    // 방에 속한 모든 사용자 ID를 담은 리스트를 브로드캐스트
    public void BroadcastUserList()
    {
        lock (_lock)
        {
            List<string> userIds = _sessions.Select(s => s.UserId ?? "Unknown").ToList();
            var userListPacket = new ServerUserListPacket { UserIds = userIds };
            Broadcast(userListPacket.ToBytes());
        }
    }

    // ... 기존 메서드 ...
}
