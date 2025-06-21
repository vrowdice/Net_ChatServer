// Server/Program.cs
using System;
using System.Linq; // LINQ를 사용하기 위해 추가
using System.Net;
using System.Net.Sockets;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            PacketHandler.Init();

            IPAddress ipAddr = null;
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);

            // IPv4 주소 중에서 루프백(127.0.0.1)이 아닌 실제 네트워크 주소를 찾습니다.
            ipAddr = ipHost.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));

            // 만약 실제 IP를 찾지 못했다면, 첫 번째 IPv4 주소를 사용하거나 (IPv6가 먼저 올 수 있음),
            // 최후의 수단으로 127.0.0.1을 사용합니다.
            if (ipAddr == null)
            {
                ipAddr = ipHost.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
            if (ipAddr == null)
            {
                ipAddr = IPAddress.Parse("127.0.0.1"); // 폴백: 127.0.0.1 사용
            }

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening...");
            Console.WriteLine($"Server listening on {endPoint.Address}:{endPoint.Port}"); // 확인용 출력

            while (true)
            {
                ;
            }
        }
    }
}