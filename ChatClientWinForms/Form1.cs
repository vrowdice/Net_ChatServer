// ChatClientWinForms/Form1.cs
using ServerCore; // ServerCore ���̺귯�� ����
using System;
// �ʿ��� using ���ù����� ���⿡ ��� �߰��մϴ�.
// ���� ������Ʈ�� .NET 6+�̰� ImplicitUsings�� enable�̸� �Ϻδ� ���� �����մϴ�.
// .NET 5 ���϶�� �Ʒ� using���� �ʼ��Դϴ�.
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
        // _currentSession�� ClientSessionWinForms Ŭ���� ���ο��� ������ �ʿ䰡 �����ϴ�.
        // Form1 ���ο����� �����ϸ� �˴ϴ�.
        private ClientSessionWinForms? _currentSession;

        public Form1()
        {
            InitializeComponent(); // UI ������Ʈ �ʱ�ȭ

            // �ʱⰪ ���� (�����̳ʿ��� �����ص� ��)
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
            // UI �����尡 �ƴ� �ٸ� �����忡�� ȣ��� �� �����Ƿ� Invoke �ʿ�
            if (chatListBox.InvokeRequired)
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
                // _connector.Connect �޼��忡�� ���ο� ���� �ν��Ͻ��� ��ȯ�ϵ��� ���ٽ� ���
                _connector.Connect(endPoint, () =>
                {
                    // ���ο� ClientSessionWinForms �ν��Ͻ��� �����ϰ� ���� Form1 �ν��Ͻ��� �����մϴ�.
                    _currentSession = new ClientSessionWinForms(this);
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
                ClientChatPacket chatPacket = new ClientChatPacket { Message = message };
                _currentSession.Send(chatPacket.ToBytes());
                messageInputTextBox.Text = ""; // �Է� �ʵ� �ʱ�ȭ
            }
            catch (Exception ex)
            {
                DisplayMessage($"�޽��� ���� ����: {ex.Message}");
                _currentSession.Disconnect(); // ���� ���� �� ���� ����
            }
        }
    }
}