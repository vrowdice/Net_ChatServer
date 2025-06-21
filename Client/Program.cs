// ChatClientWinForms 프로젝트 (Windows Forms 앱)
// Form1.cs
using System;
using System.Net;
using System.Net.Sockets; // IPEndPoint 등을 위해 필요
using System.Text;
using System.Windows.Forms;
using ServerCore; // ServerCore 라이브러리 참조

namespace ChatClientWinForms
{
    public partial class Form1 : Form
    {
        private Connector _connector = new Connector();
        private ClientSessionWinForms _currentSession; // 현재 연결된 세션

        public Form1()
        {
            InitializeComponent(); // UI 컴포넌트 초기화

            // 초기값 설정 (디자이너에서 설정해도 됨)
            ipTextBox.Text = "127.0.0.1";
            portTextBox.Text = "7777";
        }

        // UI에 메시지를 표시하는 메서드 (스레드 안전)
        public void DisplayMessage(string message)
        {
            // UI 스레드가 아닌 다른 스레드에서 호출될 수 있으므로 Invoke 필요
            if (chatListBox.InvokeRequired) // 혹은 chatDisplayArea.InvokeRequired (TextBox 사용하는 경우)
            {
                chatListBox.Invoke(new Action(() =>
                {
                    chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                    chatListBox.TopIndex = chatListBox.Items.Count - 1; // 스크롤 최하단으로
                }));
            }
            else
            {
                chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                chatListBox.TopIndex = chatListBox.Items.Count - 1; // 스크롤 최하단으로
            }
        }

        // '연결' 버튼 클릭 이벤트 핸들러
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (_currentSession != null && _currentSession.IsConnected)
            {
                DisplayMessage("이미 서버에 연결되어 있습니다.");
                return;
            }

            try
            {
                string ip = ipTextBox.Text;
                int port = int.Parse(portTextBox.Text);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                DisplayMessage($"서버 연결 시도 중: {ip}:{port}...");
                _connector.Connect(endPoint, () =>
                {
                    _currentSession = new ClientSessionWinForms(this); // 현재 Form1 인스턴스 전달
                    return _currentSession;
                });
            }
            catch (Exception ex)
            {
                DisplayMessage($"연결 실패: {ex.Message}");
            }
        }

        // '전송' 버튼 클릭 이벤트 핸들러
        private void sendButton_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        // 메시지 입력창에서 Enter 키 눌렀을 때
        private void messageInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true; // Enter 키 입력 후 다음 줄로 넘어가지 않도록 처리
                e.SuppressKeyPress = true; // 소리 방지
            }
        }

        // 채팅 메시지 전송 로직
        private void SendChatMessage()
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("서버에 연결되어 있지 않습니다. 먼저 연결하세요.");
                return;
            }

            string message = messageInputTextBox.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                DisplayMessage("보낼 메시지를 입력하세요.");
                return;
            }

            try
            {
                // PacketId.C_CHAT은 ServerCore 프로젝트의 PacketCode.cs에 있어야 합니다.
                ClientChatPacket chatPacket = new ClientChatPacket { Message = message };
                _currentSession.Send(chatPacket.ToBytes());
                messageInputTextBox.Text = ""; // 입력 필드 초기화
                // DisplayMessage($"[나] {message}"); // 내가 보낸 메시지는 바로 표시 (선택 사항)
            }
            catch (Exception ex)
            {
                DisplayMessage($"메시지 전송 실패: {ex.Message}");
                _currentSession.Disconnect(); // 전송 실패 시 연결 끊기
            }
        }

        // 클라이언트 전용 세션 클래스 (Windows Forms UI와 상호작용)
        private class ClientSessionWinForms : PacketSession
        {
            private Form1 _form; // Form1 인스턴스 참조
            public bool IsConnected { get; private set; } = false; // 연결 상태 추적

            public ClientSessionWinForms(Form1 form)
            {
                _form = form;
            }

            public override void OnConnected(EndPoint endPoint)
            {
                IsConnected = true;
                _form.DisplayMessage($"서버 연결 성공: {endPoint}");
            }

            public override void OnDisconnected(EndPoint endPoint)
            {
                IsConnected = false;
                _form.DisplayMessage($"서버 연결 해제: {endPoint}");
                // 연결 해제 시 _form._currentSession을 null로 설정하거나 다시 연결 시도
                if (_form._currentSession == this)
                {
                    _form._currentSession = null;
                }
            }

            public override void OnRecvPacket(ArraySegment<byte> buffer)
            {
                ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

                if (packetId == ConstPacketId.S_CHAT) // PacketId.S_CHAT은 ServerCore에 정의
                {
                    ServerChatPacket chatPacket = ServerChatPacket.FromBytes(buffer);
                    _form.DisplayMessage($"[메시지] {chatPacket.Message}");
                }
                else
                {
                    _form.DisplayMessage($"알 수 없는 패킷 수신: ID={packetId}");
                }
            }

            public override void OnSend(int numOfBytes)
            {
                // _form.DisplayMessage($"Sent {numOfBytes} bytes."); // 너무 많은 메시지가 나올 수 있음
            }
        }
    }
}