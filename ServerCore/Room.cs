using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 방 클래스
public class Room
{
    public int RoomId { get; private set; }
    public string RoomName { get; private set; }
    public List<ClientSession> Participants { get; private set; } = new List<ClientSession>();

    public Room(int id, string name)
    {
        RoomId = id;
        RoomName = name;
    }

    public void AddParticipant(ClientSession session)
    {
        if (!Participants.Contains(session))
        {
            Participants.Add(session);
            session.CurrentRoomId = RoomId; // 세션에 현재 방 ID 저장
        }
    }

    public void RemoveParticipant(ClientSession session)
    {
        if (Participants.Remove(session))
        {
            session.CurrentRoomId = null;
        }
    }
}

