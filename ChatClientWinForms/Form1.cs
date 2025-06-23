// ChatClientWinForms/Form1.cs
using ServerCore; // ServerCore 라이브러리 참조
using System;
using System.Collections.Generic;
using System.Drawing; // 필요한 경우
using System.IO;    // 필요한 경우
using System.Linq;
using System.Net;
using System.Net.Http; // 필요한 경우
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// using static System.Windows.Forms.VisualStyles.VisualStyleElement; // 불필요시 제거 가능


namespace ChatClientWinForms
{
    public partial class Form1 : Form
    {
        private Connector _connector = new Connector();
        private ClientSessionWinForms? _currentSession;

        public Form1()
        {
            InitializeComponent(); // UI 컴포넌트 초기화

            ipTextBox.Text = "192.168.219.110";
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
            if (chatListBox.InvokeRequired)
            {
                chatListBox.Invoke(new Action(() =>
                {
                    chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                    chatListBox.TopIndex = chatListBox.Items.Count - 1;
                }));
            }
            else
            {
                chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                chatListBox.TopIndex = chatListBox.Items.Count - 1;
            }
        }

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
                    _currentSession = new ClientSessionWinForms(this);
                    return _currentSession;
                });
            }
            catch (Exception ex)
            {
                DisplayMessage($"연결 실패: {ex.Message}");
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void messageInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void SendChatMessage()
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("서버에 연결되어 있지 않습니다. 먼저 연결하세요.");
                return;
            }

            string message = messageInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
            {
                DisplayMessage("보낼 메시지를 입력하세요.");
                return;
            }

            try
            {
                if (message.StartsWith("/"))
                {
                    int spaceIdx = message.IndexOf(' ');
                    if (spaceIdx > 1)
                    {
                        string targetId = message.Substring(1, spaceIdx - 1);
                        string whisperMsg = message.Substring(spaceIdx + 1);

                        ClientWhisperPacket whisper = new ClientWhisperPacket
                        {
                            TargetUserId = targetId,
                            Message = whisperMsg
                        };
                        _currentSession.Send(whisper.ToBytes());
                        DisplayMessage($"[To {targetId}] {whisperMsg}");
                    }
                    else
                    {
                        DisplayMessage("귓속말 형식: /아이디 메시지");
                    }
                }
                else
                {
                    ClientChatPacket chatPacket = new ClientChatPacket { Message = message };
                    _currentSession.Send(chatPacket.ToBytes());
                }

                messageInputTextBox.Text = "";
            }
            catch (Exception ex)
            {
                DisplayMessage($"메시지 전송 실패: {ex.Message}");
                _currentSession.Disconnect();
            }
        }

        public void UpdateUserList(List<string> users)
        {
            if (userListBox.InvokeRequired)
            {
                userListBox.Invoke(new Action(() => UpdateUserList(users)));
                return;
            }

            userListBox.Items.Clear();
            foreach (string user in users)
            {
                userListBox.Items.Add(user);
            }
        }

        // !!! 이 UpdateRoomList 메서드만 남기고 다른 UpdateRoomList 오버로드는 모두 삭제하세요 !!!
        public void UpdateRoomList(List<int> roomIds)
        {
            // !!! 핵심 수정: UI 스레드가 아닌 경우 Invoke를 사용하여 UI 스레드에서 실행 !!!
            if (roomListBox.InvokeRequired)
            {
                roomListBox.Invoke(new Action<List<int>>(UpdateRoomList), new object[] { roomIds });
                return;
            }

            // UI 스레드일 때만 직접 UI 컨트롤 접근
            roomListBox.Items.Clear();
            foreach (var id in roomIds)
            {
                roomListBox.Items.Add($"Room {id}"); // int ID를 string으로 변환하여 표시
            }
        }

        private void switchRoomButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("서버에 연결되어 있지 않습니다.");
                return;
            }

            if (roomListBox.SelectedItem == null)
            {
                DisplayMessage("전환할 방을 선택하세요.");
                return;
            }

            // ListBox에 "Room {id}" 형태로 저장했으므로, string에서 id를 파싱해야 합니다.
            if (roomListBox.SelectedItem is string selectedRoomText)
            {
                // "Room 123" -> 123 파싱
                if (int.TryParse(selectedRoomText.Replace("Room ", ""), out int roomId))
                {
                    var roomChangePacket = new ClientRoomChangePacket { RoomId = roomId }; // int 타입으로 변경
                    _currentSession.Send(roomChangePacket.ToBytes());
                    DisplayMessage($"방 전환 요청: {selectedRoomText} (ID: {roomId})");
                }
                else
                {
                    DisplayMessage("선택된 방 ID를 파싱할 수 없습니다.");
                }
            }
            else
            {
                DisplayMessage("선택된 방이 유효하지 않습니다.");
            }
        }

        private void createRoomButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("서버에 연결되어 있지 않습니다.");
                return;
            }

            string newRoomName = Microsoft.VisualBasic.Interaction.InputBox("방 이름을 입력하세요", "방 생성", "NewRoom");
            if (string.IsNullOrWhiteSpace(newRoomName))
            {
                DisplayMessage("방 이름이 비어 있습니다.");
                return;
            }

            var packet = new ClientRoomCreatePacket { RoomName = newRoomName };
            _currentSession.Send(packet.ToBytes());
            DisplayMessage($"방 생성 요청: {newRoomName}");
        }

        private void refreshRoomListButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("서버에 연결되어 있지 않습니다.");
                return;
            }

            var packet = new ClientRoomListRequestPacket();
            _currentSession.Send(packet.ToBytes());
            DisplayMessage("방 목록 요청 전송됨");
        }

        public void DisplayRoomCreateResult(string roomId)
        {
            // MessageBox는 UI 스레드에서 호출되어야 하므로 InvokeRequired 검사 추가 (만약 다른 스레드에서 호출될 수 있다면)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => DisplayRoomCreateResult(roomId)));
                return;
            }
            MessageBox.Show($"방 생성 성공: {roomId}", "서버 메시지", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ipTextBox_TextChanged(object sender, EventArgs e)
        {
            // 이 이벤트 핸들러는 필요하다면 구현하세요. 현재는 비어있습니다.
        }

        private void messageInputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        public void UpdateCurrentRoomDisplay(string roomName)
        {
            // UI 스레드가 아닌 다른 스레드에서 호출될 경우를 대비한 Invoke 처리
            if (currentRoomLabel.InvokeRequired)
            {
                currentRoomLabel.Invoke(new Action(() => UpdateCurrentRoomDisplay(roomName)));
                return;
            }
            // UI 업데이트
            currentRoomLabel.Text = $"현재 방: {roomName}";
            DisplayMessage($"방이 {roomName}으로 변경되었습니다.");
        }
    }
}