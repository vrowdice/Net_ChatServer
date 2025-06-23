using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServerRoomListPacket
{
    public List<int> RoomIds = new List<int>();

    // ToBytes()
    public ArraySegment<byte> ToBytes()
    {
        List<byte> buffer = new List<byte>();

        ushort packetId = ConstPacketId.S_ROOM_LIST;
        ushort roomCount = (ushort)RoomIds.Count;
        ushort totalSize = (ushort)(sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + (sizeof(int) * roomCount));

        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes(roomCount));

        foreach (int id in RoomIds)
        {
            buffer.AddRange(BitConverter.GetBytes(id));
        }

        return new ArraySegment<byte>(buffer.ToArray());
    }

    // FromBytes()
    public static ServerRoomListPacket FromBytes(ArraySegment<byte> buffer)
    {
        ServerRoomListPacket packet = new ServerRoomListPacket();
        int currentOffset = buffer.Offset + 2 + 2;

        ushort count = BitConverter.ToUInt16(buffer.Array!, currentOffset);
        currentOffset += sizeof(ushort);

        packet.RoomIds = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int id = BitConverter.ToInt32(buffer.Array!, currentOffset);
            currentOffset += sizeof(int);
            packet.RoomIds.Add(id);
        }

        return packet;
    }
}

