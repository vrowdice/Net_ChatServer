// ChatClientWinForms/Form1.cs
using ServerCore; // ServerCore ���̺귯�� ����
using System;
using System.Collections.Generic;
using System.Drawing; // �ʿ��� ���
using System.IO;    // �ʿ��� ���
using System.Linq;
using System.Net;
using System.Net.Http; // �ʿ��� ���
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// using static System.Windows.Forms.VisualStyles.VisualStyleElement; // ���ʿ�� ���� ����


namespace ChatClientWinForms
{
    public partial class Form1 : Form
    {
        private Connector _connector = new Connector();
        private ClientSessionWinForms? _currentSession;

        public Form1()
        {
            InitializeComponent(); // UI ������Ʈ �ʱ�ȭ

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
                DisplayMessage("����� ID�� �Է��ϼ���.");
                return;
            }

            ClientLoginPacket loginPacket = new ClientLoginPacket();
            loginPacket.UserId = userId;

            ArraySegment<byte> sendBuff = loginPacket.ToBytes();
            _currentSession.Send(sendBuff);

            DisplayMessage($"�α��� ��û ����: {userId}");
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
                DisplayMessage("�̹� ������ ����Ǿ� �ֽ��ϴ�.");
                return;
            }

            try
            {
                string ip = ipTextBox.Text;
                int port = int.Parse(portTextBox.Text);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                DisplayMessage($"���� ���� �õ� ��: {ip}:{port}...");
                _connector.Connect(endPoint, () =>
                {
                    _currentSession = new ClientSessionWinForms(this);
                    return _currentSession;
                });
            }
            catch (Exception ex)
            {
                DisplayMessage($"���� ����: {ex.Message}");
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
                DisplayMessage("������ ����Ǿ� ���� �ʽ��ϴ�. ���� �����ϼ���.");
                return;
            }

            string message = messageInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
            {
                DisplayMessage("���� �޽����� �Է��ϼ���.");
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
                        DisplayMessage("�ӼӸ� ����: /���̵� �޽���");
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
                DisplayMessage($"�޽��� ���� ����: {ex.Message}");
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

        // !!! �� UpdateRoomList �޼��常 ����� �ٸ� UpdateRoomList �����ε�� ��� �����ϼ��� !!!
        public void UpdateRoomList(List<int> roomIds)
        {
            // !!! �ٽ� ����: UI �����尡 �ƴ� ��� Invoke�� ����Ͽ� UI �����忡�� ���� !!!
            if (roomListBox.InvokeRequired)
            {
                roomListBox.Invoke(new Action<List<int>>(UpdateRoomList), new object[] { roomIds });
                return;
            }

            // UI �������� ���� ���� UI ��Ʈ�� ����
            roomListBox.Items.Clear();
            foreach (var id in roomIds)
            {
                roomListBox.Items.Add($"Room {id}"); // int ID�� string���� ��ȯ�Ͽ� ǥ��
            }
        }

        private void switchRoomButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("������ ����Ǿ� ���� �ʽ��ϴ�.");
                return;
            }

            if (roomListBox.SelectedItem == null)
            {
                DisplayMessage("��ȯ�� ���� �����ϼ���.");
                return;
            }

            // ListBox�� "Room {id}" ���·� ���������Ƿ�, string���� id�� �Ľ��ؾ� �մϴ�.
            if (roomListBox.SelectedItem is string selectedRoomText)
            {
                // "Room 123" -> 123 �Ľ�
                if (int.TryParse(selectedRoomText.Replace("Room ", ""), out int roomId))
                {
                    var roomChangePacket = new ClientRoomChangePacket { RoomId = roomId }; // int Ÿ������ ����
                    _currentSession.Send(roomChangePacket.ToBytes());
                    DisplayMessage($"�� ��ȯ ��û: {selectedRoomText} (ID: {roomId})");
                }
                else
                {
                    DisplayMessage("���õ� �� ID�� �Ľ��� �� �����ϴ�.");
                }
            }
            else
            {
                DisplayMessage("���õ� ���� ��ȿ���� �ʽ��ϴ�.");
            }
        }

        private void createRoomButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("������ ����Ǿ� ���� �ʽ��ϴ�.");
                return;
            }

            string newRoomName = Microsoft.VisualBasic.Interaction.InputBox("�� �̸��� �Է��ϼ���", "�� ����", "NewRoom");
            if (string.IsNullOrWhiteSpace(newRoomName))
            {
                DisplayMessage("�� �̸��� ��� �ֽ��ϴ�.");
                return;
            }

            var packet = new ClientRoomCreatePacket { RoomName = newRoomName };
            _currentSession.Send(packet.ToBytes());
            DisplayMessage($"�� ���� ��û: {newRoomName}");
        }

        private void refreshRoomListButton_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("������ ����Ǿ� ���� �ʽ��ϴ�.");
                return;
            }

            var packet = new ClientRoomListRequestPacket();
            _currentSession.Send(packet.ToBytes());
            DisplayMessage("�� ��� ��û ���۵�");
        }

        public void DisplayRoomCreateResult(string roomId)
        {
            // MessageBox�� UI �����忡�� ȣ��Ǿ�� �ϹǷ� InvokeRequired �˻� �߰� (���� �ٸ� �����忡�� ȣ��� �� �ִٸ�)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => DisplayRoomCreateResult(roomId)));
                return;
            }
            MessageBox.Show($"�� ���� ����: {roomId}", "���� �޽���", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ipTextBox_TextChanged(object sender, EventArgs e)
        {
            // �� �̺�Ʈ �ڵ鷯�� �ʿ��ϴٸ� �����ϼ���. ����� ����ֽ��ϴ�.
        }

        private void messageInputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        public void UpdateCurrentRoomDisplay(string roomName)
        {
            // UI �����尡 �ƴ� �ٸ� �����忡�� ȣ��� ��츦 ����� Invoke ó��
            if (currentRoomLabel.InvokeRequired)
            {
                currentRoomLabel.Invoke(new Action(() => UpdateCurrentRoomDisplay(roomName)));
                return;
            }
            // UI ������Ʈ
            currentRoomLabel.Text = $"���� ��: {roomName}";
            DisplayMessage($"���� {roomName}���� ����Ǿ����ϴ�.");
        }
    }
}