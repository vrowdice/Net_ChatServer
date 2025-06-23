using ServerCore;
using System;
using System.Text;

public class ServerLoginOkPacket
{
    public ushort Size;
    public ushort PacketId;
    public string Message;

    public ServerLoginOkPacket()
    {
        PacketId = ConstPacketId.S_LOGIN_OK; // 1004
    }

    public ArraySegment<byte> ToBytes()
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(Message ?? "");
        Size = (ushort)(PacketSession.HeaderSize + messageBytes.Length);

        byte[] buffer = new byte[Size];
        Buffer.BlockCopy(BitConverter.GetBytes(Size), 0, buffer, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(PacketId), 0, buffer, 2, 2);
        Buffer.BlockCopy(messageBytes, 0, buffer, PacketSession.HeaderSize, messageBytes.Length);

        return new ArraySegment<byte>(buffer, 0, Size);
    }

    public static ServerLoginOkPacket FromBytes(ArraySegment<byte> buffer)
    {
        ServerLoginOkPacket packet = new ServerLoginOkPacket();
        packet.Size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        packet.PacketId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        int msgLen = buffer.Count - PacketSession.HeaderSize;
        if (msgLen > 0)
        {
            packet.Message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + PacketSession.HeaderSize, msgLen);
        }
        else
        {
            packet.Message = string.Empty;
        }
        return packet;
    }
}
