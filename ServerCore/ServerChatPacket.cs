// ServerCore 프로젝트에 포함
// ServerChatPacket.cs
using System;
using System.Text;
using ServerCore;

public class ServerChatPacket
{
    public ushort Size;
    public ushort PacketId;
    public string Message;

    public ServerChatPacket()
    {
        PacketId = ConstPacketId.S_CHAT; // PacketId.S_CHAT 상수 사용
    }

    public ArraySegment<byte> ToBytes()
    {
        byte[] msgBytes = Encoding.UTF8.GetBytes(Message);
        Size = (ushort)(PacketSession.HeaderSize + msgBytes.Length);

        ArraySegment<byte> sendBuffer = SendBufferHelper.Open(Size);
        Buffer.BlockCopy(BitConverter.GetBytes(Size), 0, sendBuffer.Array, sendBuffer.Offset, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(PacketId), 0, sendBuffer.Array, sendBuffer.Offset + 2, 2);
        Buffer.BlockCopy(msgBytes, 0, sendBuffer.Array, sendBuffer.Offset + 4, msgBytes.Length);

        return SendBufferHelper.Close(Size);
    }

    public static ServerChatPacket FromBytes(ArraySegment<byte> buffer)
    {
        ServerChatPacket chatPacket = new ServerChatPacket();
        chatPacket.Size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        chatPacket.PacketId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        if (buffer.Count > PacketSession.HeaderSize)
        {
            chatPacket.Message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + PacketSession.HeaderSize, buffer.Count - PacketSession.HeaderSize);
        }
        else
        {
            chatPacket.Message = string.Empty;
        }
        return chatPacket;
    }
}