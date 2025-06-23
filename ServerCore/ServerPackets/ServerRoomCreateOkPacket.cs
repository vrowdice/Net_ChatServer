using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServerRoomCreateOkPacket
{
    public string RoomId;

    public ArraySegment<byte> ToBytes()
    {
        byte[] roomBytes = Encoding.UTF8.GetBytes(RoomId);
        ushort size = (ushort)(2 + 2 + 2 + roomBytes.Length);
        ushort packetId = 1013;

        List<byte> buffer = new();
        buffer.AddRange(BitConverter.GetBytes(size));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)roomBytes.Length));
        buffer.AddRange(roomBytes);
        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ServerRoomCreateOkPacket FromBytes(ArraySegment<byte> buffer)
    {
        int offset = 4;
        ushort roomLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string roomId = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, roomLen);
        return new ServerRoomCreateOkPacket { RoomId = roomId };
    }
}

