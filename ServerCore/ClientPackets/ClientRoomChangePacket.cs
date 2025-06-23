// ClientRoomChangePacket.cs
using System;
using System.Collections.Generic;
using System.Text;

public class ClientRoomChangePacket
{
    public int RoomId { get; set; } // int로 변경!

    public ArraySegment<byte> ToBytes()
    {
        List<byte> buffer = new List<byte>();

        ushort packetId = ConstPacketId.C_ROOM_CHANGE; // 적절한 PacketId 상수 사용
        // totalSize 계산: size(2) + packetId(2) + RoomId(4) = 8
        ushort totalSize = (ushort)(sizeof(ushort) + sizeof(ushort) + sizeof(int));

        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes(RoomId)); // int를 바이트로 변환

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ClientRoomChangePacket FromBytes(ArraySegment<byte> buffer)
    {
        ClientRoomChangePacket packet = new ClientRoomChangePacket();

        // size(2) + packetId(2) = 4
        int currentOffset = buffer.Offset + sizeof(ushort) + sizeof(ushort);

        packet.RoomId = BitConverter.ToInt32(buffer.Array!, currentOffset); // int로 파싱
        // currentOffset += sizeof(int); // 더 이상 읽을 데이터가 없으므로 필요 없음

        return packet;
    }
}