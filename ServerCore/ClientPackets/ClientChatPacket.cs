// ServerCore 프로젝트에 포함
// ClientChatPacket.cs
using System;
using System.Text;
using ServerCore;

public class ClientChatPacket
{
    public ushort Size;
    public ushort PacketId;
    public string Message;

    public ClientChatPacket()
    {
        PacketId = ConstPacketId.C_CHAT; // PacketId.C_CHAT 상수 사용
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

    public static ClientChatPacket FromBytes(ArraySegment<byte> buffer)
    {
        ClientChatPacket chatPacket = new ClientChatPacket();
        chatPacket.Size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        chatPacket.PacketId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        int messageLength = chatPacket.Size - PacketSession.HeaderSize;

        if (messageLength > 0 && buffer.Count >= chatPacket.Size)
        {
            chatPacket.Message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + PacketSession.HeaderSize, messageLength);
        }
        else
        {
            chatPacket.Message = string.Empty;
        }
        return chatPacket;
    }
}