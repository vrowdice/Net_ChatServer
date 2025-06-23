using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServerUserListPacket
{
    public List<string> UserIds = new();

    public ArraySegment<byte> ToBytes()
    {
        List<byte> buffer = new();
        ushort packetId = ConstPacketId.S_USER_LIST;

        buffer.AddRange(BitConverter.GetBytes((ushort)0)); // Placeholder for size
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)UserIds.Count));
        foreach (string userId in UserIds)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(userId);
            buffer.AddRange(BitConverter.GetBytes((ushort)strBytes.Length));
            buffer.AddRange(strBytes);
        }

        ushort totalSize = (ushort)buffer.Count;
        Array.Copy(BitConverter.GetBytes(totalSize), 0, buffer.ToArray(), 0, 2);
        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ServerUserListPacket FromBytes(ArraySegment<byte> buffer)
    {
        var result = new ServerUserListPacket();
        int offset = 4;

        ushort userCount = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;

        for (int i = 0; i < userCount; i++)
        {
            ushort len = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
            offset += 2;

            string id = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, len);
            offset += len;

            result.UserIds.Add(id);
        }

        return result;
    }
}

