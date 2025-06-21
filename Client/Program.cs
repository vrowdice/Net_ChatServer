// ChatClientWinForms ������Ʈ (Windows Forms ��)
// Form1.cs
using System;
using System.Net;
using System.Net.Sockets; // IPEndPoint ���� ���� �ʿ�
using System.Text;
using System.Windows.Forms;
using ServerCore; // ServerCore ���̺귯�� ����

namespace ChatClientWinForms
{
    public partial class Form1 : Form
    {
        private Connector _connector = new Connector();
        private ClientSessionWinForms _currentSession; // ���� ����� ����

        public Form1()
        {
            InitializeComponent(); // UI ������Ʈ �ʱ�ȭ

            // �ʱⰪ ���� (�����̳ʿ��� �����ص� ��)
            ipTextBox.Text = "127.0.0.1";
            portTextBox.Text = "7777";
        }

        // UI�� �޽����� ǥ���ϴ� �޼��� (������ ����)
        public void DisplayMessage(string message)
        {
            // UI �����尡 �ƴ� �ٸ� �����忡�� ȣ��� �� �����Ƿ� Invoke �ʿ�
            if (chatListBox.InvokeRequired) // Ȥ�� chatDisplayArea.InvokeRequired (TextBox ����ϴ� ���)
            {
                chatListBox.Invoke(new Action(() =>
                {
                    chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                    chatListBox.TopIndex = chatListBox.Items.Count - 1; // ��ũ�� ���ϴ�����
                }));
            }
            else
            {
                chatListBox.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} - {message}");
                chatListBox.TopIndex = chatListBox.Items.Count - 1; // ��ũ�� ���ϴ�����
            }
        }

        // '����' ��ư Ŭ�� �̺�Ʈ �ڵ鷯
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
                    _currentSession = new ClientSessionWinForms(this); // ���� Form1 �ν��Ͻ� ����
                    return _currentSession;
                });
            }
            catch (Exception ex)
            {
                DisplayMessage($"���� ����: {ex.Message}");
            }
        }

        // '����' ��ư Ŭ�� �̺�Ʈ �ڵ鷯
        private void sendButton_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        // �޽��� �Է�â���� Enter Ű ������ ��
        private void messageInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true; // Enter Ű �Է� �� ���� �ٷ� �Ѿ�� �ʵ��� ó��
                e.SuppressKeyPress = true; // �Ҹ� ����
            }
        }

        // ä�� �޽��� ���� ����
        private void SendChatMessage()
        {
            if (_currentSession == null || !_currentSession.IsConnected)
            {
                DisplayMessage("������ ����Ǿ� ���� �ʽ��ϴ�. ���� �����ϼ���.");
                return;
            }

            string message = messageInputTextBox.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                DisplayMessage("���� �޽����� �Է��ϼ���.");
                return;
            }

            try
            {
                // PacketId.C_CHAT�� ServerCore ������Ʈ�� PacketCode.cs�� �־�� �մϴ�.
                ClientChatPacket chatPacket = new ClientChatPacket { Message = message };
                _currentSession.Send(chatPacket.ToBytes());
                messageInputTextBox.Text = ""; // �Է� �ʵ� �ʱ�ȭ
                // DisplayMessage($"[��] {message}"); // ���� ���� �޽����� �ٷ� ǥ�� (���� ����)
            }
            catch (Exception ex)
            {
                DisplayMessage($"�޽��� ���� ����: {ex.Message}");
                _currentSession.Disconnect(); // ���� ���� �� ���� ����
            }
        }

        // Ŭ���̾�Ʈ ���� ���� Ŭ���� (Windows Forms UI�� ��ȣ�ۿ�)
        private class ClientSessionWinForms : PacketSession
        {
            private Form1 _form; // Form1 �ν��Ͻ� ����
            public bool IsConnected { get; private set; } = false; // ���� ���� ����

            public ClientSessionWinForms(Form1 form)
            {
                _form = form;
            }

            public override void OnConnected(EndPoint endPoint)
            {
                IsConnected = true;
                _form.DisplayMessage($"���� ���� ����: {endPoint}");
            }

            public override void OnDisconnected(EndPoint endPoint)
            {
                IsConnected = false;
                _form.DisplayMessage($"���� ���� ����: {endPoint}");
                // ���� ���� �� _form._currentSession�� null�� �����ϰų� �ٽ� ���� �õ�
                if (_form._currentSession == this)
                {
                    _form._currentSession = null;
                }
            }

            public override void OnRecvPacket(ArraySegment<byte> buffer)
            {
                ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

                if (packetId == ConstPacketId.S_CHAT) // PacketId.S_CHAT�� ServerCore�� ����
                {
                    ServerChatPacket chatPacket = ServerChatPacket.FromBytes(buffer);
                    _form.DisplayMessage($"[�޽���] {chatPacket.Message}");
                }
                else
                {
                    _form.DisplayMessage($"�� �� ���� ��Ŷ ����: ID={packetId}");
                }
            }

            public override void OnSend(int numOfBytes)
            {
                // _form.DisplayMessage($"Sent {numOfBytes} bytes."); // �ʹ� ���� �޽����� ���� �� ����
            }
        }
    }
}