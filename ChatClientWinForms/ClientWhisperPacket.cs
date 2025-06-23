using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ClientWhisperPacket
{
    public string TargetUserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ArraySegment<byte> ToBytes()
    {
        byte[] userIdBytes = System.Text.Encoding.UTF8.GetBytes(TargetUserId);
        byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(Message);

        ushort totalSize = (ushort)(2 + 2 + 2 + userIdBytes.Length + 2 + msgBytes.Length);
        ushort packetId = ConstPacketId.C_WHISPER;

        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)userIdBytes.Length));
        buffer.AddRange(userIdBytes);
        buffer.AddRange(BitConverter.GetBytes((ushort)msgBytes.Length));
        buffer.AddRange(msgBytes);

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ClientWhisperPacket FromBytes(ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            throw new ArgumentNullException(nameof(buffer.Array), "Buffer array is null.");

        int offset = 4; // size + packetId
        ushort userIdLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string userId = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, userIdLen);
        offset += userIdLen;

        ushort msgLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string msg = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, msgLen);

        return new ClientWhisperPacket { TargetUserId = userId, Message = msg };
    }

}

