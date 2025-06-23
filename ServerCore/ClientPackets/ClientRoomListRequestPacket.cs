using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ClientRoomListRequestPacket
{
    public ArraySegment<byte> ToBytes()
    {
        ushort totalSize = 4;
        ushort packetId = ConstPacketId.C_ROOM_LIST;

        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));

        return new ArraySegment<byte>(buffer.ToArray());
    }
}
