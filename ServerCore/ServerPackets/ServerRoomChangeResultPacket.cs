// Common/ServerRoomChangeResultPacket.cs

using System;
using System.Collections.Generic;
using System.Text;

public class ServerRoomChangeResultPacket
{
    public bool Success { get; set; }
    public int RoomId { get; set; } // 변경된 방의 ID (실패 시 요청했던 ID)

    public ArraySegment<byte> ToBytes()
    {
        List<byte> buffer = new List<byte>();

        ushort packetId = ConstPacketId.S_ROOM_CHANGE_RESULT;
        ushort totalSize = (ushort)(sizeof(ushort) + sizeof(ushort) + sizeof(bool) + sizeof(int));

        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes(Success));
        buffer.AddRange(BitConverter.GetBytes(RoomId));

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ServerRoomChangeResultPacket FromBytes(ArraySegment<byte> buffer)
    {
        ServerRoomChangeResultPacket packet = new ServerRoomChangeResultPacket();

        int currentOffset = buffer.Offset + sizeof(ushort) + sizeof(ushort); // size(2) + packetId(2) = 4

        packet.Success = BitConverter.ToBoolean(buffer.Array!, currentOffset);
        currentOffset += sizeof(bool);

        packet.RoomId = BitConverter.ToInt32(buffer.Array!, currentOffset);

        return packet;
    }
}