using System.Collections.Generic;
using System.Linq;

public class RoomManager
{
    private static RoomManager _instance = new RoomManager();
    public static RoomManager Instance => _instance;

    private Dictionary<int, ChatRoom> _rooms = new Dictionary<int, ChatRoom>();
    private int _nextRoomId = 1;

    public ChatRoom FindRoom(int roomId)
    {
        ChatRoom room;
        _rooms.TryGetValue(roomId, out room);
        return room;
    }

    public ChatRoom CreateRoom(string roomName)
    {
        var room = new ChatRoom(_nextRoomId++, roomName);
        _rooms.Add(room.Id, room);
        return room;
    }

    public bool RemoveRoom(int roomId)
    {
        return _rooms.Remove(roomId);
    }

    public ChatRoom GetRoom(int roomId)
    {
        ChatRoom room;
        _rooms.TryGetValue(roomId, out room);
        return room;
    }

    public List<int> GetAllRoomIds()
    {
        return _rooms.Keys.ToList();
    }
    public List<string> GetAllRoomNames()
    {
        return _rooms.Values.Select(r => r.Name).ToList();
    }

    public ChatRoom CreateRoomIfNotExist(string roomName)
    {
        var existingRoom = _rooms.Values.FirstOrDefault(r => r.Name == roomName);
        if (existingRoom != null)
            return existingRoom;

        var newRoom = new ChatRoom(_nextRoomId++, roomName);
        _rooms.Add(newRoom.Id, newRoom);
        return newRoom;
    }
}