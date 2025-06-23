using System;
using System.Collections.Generic;
using System.Text;

public class ClientRoomCreatePacket
{
    public string RoomName;

    public ArraySegment<byte> ToBytes()
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(RoomName);
        ushort packetSize = (ushort)(2 + 2 + 2 + nameBytes.Length);
        ushort packetId = ConstPacketId.C_ROOM_CREATE;

        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(packetSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)nameBytes.Length));
        buffer.AddRange(nameBytes);

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ClientRoomCreatePacket FromBytes(ArraySegment<byte> buffer)
    {
        int offset = 4;

        ushort nameLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;

        string roomName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, nameLen);

        return new ClientRoomCreatePacket { RoomName = roomName };
    }
}
