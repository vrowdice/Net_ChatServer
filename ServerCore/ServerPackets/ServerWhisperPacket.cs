using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServerWhisperPacket
{
    public string SenderUserId;
    public string Message;

    public ArraySegment<byte> ToBytes()
    {
        byte[] userIdBytes = System.Text.Encoding.UTF8.GetBytes(SenderUserId);
        byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(Message);

        ushort totalSize = (ushort)(2 + 2 + 2 + userIdBytes.Length + 2 + msgBytes.Length);
        ushort packetId = ConstPacketId.S_WHISPER;

        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(totalSize));
        buffer.AddRange(BitConverter.GetBytes(packetId));
        buffer.AddRange(BitConverter.GetBytes((ushort)userIdBytes.Length));
        buffer.AddRange(userIdBytes);
        buffer.AddRange(BitConverter.GetBytes((ushort)msgBytes.Length));
        buffer.AddRange(msgBytes);

        return new ArraySegment<byte>(buffer.ToArray());
    }

    public static ServerWhisperPacket FromBytes(ArraySegment<byte> buffer)
    {
        int offset = 4;
        ushort senderLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string sender = System.Text.Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, senderLen);
        offset += senderLen;

        ushort msgLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + offset);
        offset += 2;
        string msg = System.Text.Encoding.UTF8.GetString(buffer.Array, buffer.Offset + offset, msgLen);

        return new ServerWhisperPacket { SenderUserId = sender, Message = msg };
    }
}

