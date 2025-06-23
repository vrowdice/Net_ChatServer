using System;
using System.Collections.Generic;
using System.Text;

public class ClientWhisperPacket
{
    public string TargetUserId;
    public string Message;

    public ArraySegment<byte> ToBytes()
    {
        byte[] targetBytes = Encoding.UTF8.GetBytes(TargetUserId);
        byte[] msgBytes = Encoding.UTF8.GetBytes(Message);

        ushort totalSize = (ushort)(2 + 2 + 2 + targetBytes.Length + 2 + msgBytes.Length);
        ushort packetId = ConstPacketId.C_WHISPER;

        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)targetBytes.Length));
        buffer.AddRange(targetBytes);
        buffer.AddRange(BitConverter.GetBytes((ushort)msgBytes.Length));
        buffer.AddRange(msgBytes);

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ClientWhisperPacket FromBytes(ArraySegment<byte> buffer)
    {
        int offset = 4; // skip size(2) + packetId(2)

        ushort targetLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string targetUserId = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, targetLen);
        offset += targetLen;

        ushort msgLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, msgLen);

        return new ClientWhisperPacket
        {
            TargetUserId = targetUserId,
            Message = message
        };
    }
}
