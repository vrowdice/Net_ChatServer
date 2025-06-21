// ChatClientWinForms/Form1.cs
using ServerCore; // ServerCore 라이브러리 참조
using System;
// 필요한 using 지시문들을 여기에 모두 추가합니다.
// 만약 프로젝트가 .NET 6+이고 ImplicitUsings가 enable이면 일부는 생략 가능합니다.
// .NET 5 이하라면 아래 using들은 필수입니다.
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace ChatClientWinForms
{
    public partial class Form1 : Form
    {
        private Connector _connector = new Connector();
        // _currentSession은 ClientSessionWinForms 클래스 내부에서 접근할 필요가 없습니다.
        // Form1 내부에서만 관리하면 됩니다.
        private ClientSessionWinForms? _currentSession;

        public Form1()
        {
            InitializeComponent(); // UI 컴포넌트 초기화

            // 초기값 설정 (디자이너에서 설정해도 됨)
            ipTextBox.Text = "127.0.0.1";
            portTextBox.Text = "7777";
        }

        public void SendLoginPacket()
        {
            if (_currentSession == null || !_currentSession.IsConnected)
                return;

            string userId = userIdTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userId))
            {
                DisplayMessage("사용자 ID를 입력하세요.");
                return;
            }

            ClientLoginPacket loginPacket = new ClientLoginPacket();
            loginPacket.UserId = userId;

            ArraySegment<byte> sendBuff = loginPacket.ToBytes();
            _currentSession.Send(sendBuff);

            DisplayMessage($"로그인 요청 전송: {userId}");
        }

        public void DisplayMessage(string message)
        {
            // UI 스레드가 아닌 다른 스레드에서 호출될 수 있으므로 Invoke 필요
            if (chatListBox.InvokeRequired)
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
                // _connector.Connect 메서드에서 새로운 세션 인스턴스를 반환하도록 람다식 사용
                _connector.Connect(endPoint, () =>
                {
                    // 새로운 ClientSessionWinForms 인스턴스를 생성하고 현재 Form1 인스턴스를 전달합니다.
                    _currentSession = new ClientSessionWinForms(this);
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
                ClientChatPacket chatPacket = new ClientChatPacket { Message = message };
                _currentSession.Send(chatPacket.ToBytes());
                messageInputTextBox.Text = ""; // 입력 필드 초기화
            }
            catch (Exception ex)
            {
                DisplayMessage($"메시지 전송 실패: {ex.Message}");
                _currentSession.Disconnect(); // 전송 실패 시 연결 끊기
            }
        }
    }
}