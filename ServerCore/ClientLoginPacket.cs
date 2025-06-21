using System;
using System.Text;
using ServerCore;

public class ClientLoginPacket
{
    public ushort Size;
    public ushort PacketId;
    public string UserId;

    public ClientLoginPacket()
    {
        PacketId = ConstPacketId.C_LOGIN; // 로그인 패킷 ID
    }

    public ArraySegment<byte> ToBytes()
    {
        byte[] userIdBytes = Encoding.UTF8.GetBytes(UserId);
        Size = (ushort)(PacketSession.HeaderSize + userIdBytes.Length);

        ArraySegment<byte> sendBuffer = SendBufferHelper.Open(Size);
        Buffer.BlockCopy(BitConverter.GetBytes(Size), 0, sendBuffer.Array, sendBuffer.Offset, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(PacketId), 0, sendBuffer.Array, sendBuffer.Offset + 2, 2);
        Buffer.BlockCopy(userIdBytes, 0, sendBuffer.Array, sendBuffer.Offset + PacketSession.HeaderSize, userIdBytes.Length);

        return SendBufferHelper.Close(Size);
    }

    public static ClientLoginPacket FromBytes(ArraySegment<byte> buffer)
    {
        ClientLoginPacket loginPacket = new ClientLoginPacket();
        loginPacket.Size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        loginPacket.PacketId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        int userIdLength = loginPacket.Size - PacketSession.HeaderSize;
        if (userIdLength > 0 && buffer.Count >= loginPacket.Size)
        {
            loginPacket.UserId = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + PacketSession.HeaderSize, userIdLength);
        }
        else
        {
            loginPacket.UserId = string.Empty;
        }

        return loginPacket;
    }
}
