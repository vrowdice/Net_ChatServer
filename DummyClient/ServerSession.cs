using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{
	class Packet
	{
		public ushort size;
		public ushort packetId;
	}

	class PlayerInfoReq : Packet
	{
		public long playerId;
	}

	class PlayerInfoOk : Packet
	{
		public int hp;
		public int attack;
	}

	class MakeChattingRoom : Packet
	{
		public string roomName;
		public int maxPlayers;

		public MakeChattingRoom()
		{
			packetId = (ushort)PacketID.MakeChattingRoom;
		}

		public ArraySegment<byte> Serialize()
		{
			ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort size = 0;
            bool success = true;

            size += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + size, s.Count - size), this.packetId);
			size += sizeof(ushort);
			ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.roomName, 0, this.roomName.Length, s.Array, s.Offset + size + sizeof(ushort));
			success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + size, s.Count - size), nameLen);
			size += sizeof(ushort);
			size += nameLen;
			success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + size, s.Count - size), maxPlayers);
			size += sizeof(int);

			success &= BitConverter.TryWriteBytes(s.Array, size);
			if (success == true)
				return SendBufferHelper.Close(size);
			else
			{
				SendBufferHelper.Close(0);
				return null;
			}
        }

		public void Deserialize(ArraySegment<byte> buffer)
		{
            int pos = 0;

            this.size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += sizeof(ushort);
            this.packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += sizeof(ushort);
			int nameLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
			pos += sizeof(ushort);
			this.roomName = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + pos, nameLen);
			pos += nameLen;
			this.maxPlayers = BitConverter.ToInt32(buffer.Array, buffer.Offset + pos);
        }
	}

	public enum PacketID
	{
		PlayerInfoReq = 1,
		PlayerInfoOk = 2,
		MakeChattingRoom = 3,
	}

	class ServerSession : Session
	{
		static unsafe void ToBytes(byte[] array, int offset, ulong value)
		{
			fixed (byte* ptr = &array[offset])
				*(ulong*)ptr = value;
		}

		static unsafe void ToBytes<T>(byte[] array, int offset, T value) where T : unmanaged
		{
			fixed (byte* ptr = &array[offset])
				*(T*)ptr = value;
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			//PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 };


			//// 보낸다
			//for (int i = 0; i < 5; i++)
			//{
			//	ArraySegment<byte> s = SendBufferHelper.Open(4096);
			//	//byte[] size = BitConverter.GetBytes(packet.size);
			//	//byte[] packetId = BitConverter.GetBytes(packet.packetId);
			//	//byte[] playerId = BitConverter.GetBytes(packet.playerId);

			//	ushort size = 0;
			//	bool success = true;
			
			//	size += 2;
			//	success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + size, s.Count - size), packet.packetId);
			//	size += 2;
			//	success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + size, s.Count - size), packet.playerId);
			//	size += 8;
			//	success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), size);

			//	ArraySegment<byte> sendBuff = SendBufferHelper.Close(size);

			//	if (success)
			//		Send(sendBuff);
			//}

			MakeChattingRoom packet = new MakeChattingRoom() { roomName = "TestRoom", maxPlayers = 10 };
			ArraySegment<byte> serializedPacket = packet.Serialize();
			if (serializedPacket != null)
				Send(serializedPacket);
		}

	

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override int OnRecv(ArraySegment<byte> buffer)
		{
			string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			Console.WriteLine($"[From Server] {recvData}");
			return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}

}
