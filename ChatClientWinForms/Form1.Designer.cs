// ChatClientWinForms/Form1.Designer.cs
namespace ChatClientWinForms
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            ipTextBox = new TextBox();
            portTextBox = new TextBox();
            connectButton = new Button();
            chatListBox = new ListBox();
            messageInputTextBox = new TextBox();
            sendButton = new Button();
            userIdTextBox = new TextBox();
            userListBox = new ListBox();
            roomListBox = new ListBox();
            switchRoomButton = new Button();
            createRoomButton = new Button();
            refreshRoomListButton = new Button();
            currentRoomLabel = new Label(); // 새로 추가: currentRoomLabel 변수 선언
            SuspendLayout();
            //
            // ipTextBox
            //
            ipTextBox.Location = new Point(12, 13);
            ipTextBox.Margin = new Padding(3, 4, 3, 4);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.Size = new Size(150, 23);
            ipTextBox.TabIndex = 0;
            ipTextBox.Text = "127.0.0.1";
            ipTextBox.TextChanged += ipTextBox_TextChanged;
            //
            // portTextBox
            //
            portTextBox.Location = new Point(168, 13);
            portTextBox.Margin = new Padding(3, 4, 3, 4);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(80, 23);
            portTextBox.TabIndex = 1;
            portTextBox.Text = "7777";
            //
            // connectButton
            //
            connectButton.Location = new Point(397, 9);
            connectButton.Margin = new Padding(3, 4, 3, 4);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(75, 29);
            connectButton.TabIndex = 2;
            connectButton.Text = "연결";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            //
            // chatListBox
            //
            chatListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatListBox.FormattingEnabled = true;
            chatListBox.ItemHeight = 15;
            chatListBox.Location = new Point(138, 50);
            chatListBox.Margin = new Padding(3, 4, 3, 4);
            chatListBox.Name = "chatListBox";
            chatListBox.Size = new Size(334, 319);
            chatListBox.TabIndex = 3;
            //
            // messageInputTextBox
            //
            messageInputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageInputTextBox.Location = new Point(93, 388);
            messageInputTextBox.Margin = new Padding(3, 4, 3, 4);
            messageInputTextBox.Name = "messageInputTextBox";
            messageInputTextBox.Size = new Size(298, 23);
            messageInputTextBox.TabIndex = 4;
            messageInputTextBox.KeyDown += messageInputTextBox_KeyDown;
            //
            // sendButton
            //
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.Location = new Point(397, 384);
            sendButton.Margin = new Padding(3, 4, 3, 4);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(75, 29);
            sendButton.TabIndex = 5;
            sendButton.Text = "전송";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            //
            // userIdTextBox
            //
            userIdTextBox.Location = new Point(254, 13);
            userIdTextBox.Margin = new Padding(3, 4, 3, 4);
            userIdTextBox.Name = "userIdTextBox";
            userIdTextBox.Size = new Size(132, 23);
            userIdTextBox.TabIndex = 6;
            userIdTextBox.Text = "User123";
            //
            // userListBox
            //
            userListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            userListBox.FormattingEnabled = true;
            userListBox.ItemHeight = 15;
            userListBox.Location = new Point(12, 50);
            userListBox.Margin = new Padding(3, 4, 3, 4);
            userListBox.Name = "userListBox";
            userListBox.Size = new Size(120, 319);
            userListBox.TabIndex = 7;
            //
            // roomListBox
            //
            roomListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            roomListBox.FormattingEnabled = true;
            roomListBox.ItemHeight = 15;
            roomListBox.Location = new Point(93, 419);
            roomListBox.Margin = new Padding(3, 4, 3, 4);
            roomListBox.Name = "roomListBox";
            roomListBox.Size = new Size(379, 79);
            roomListBox.TabIndex = 8;
            //
            // switchRoomButton
            //
            switchRoomButton.Location = new Point(12, 429);
            switchRoomButton.Margin = new Padding(3, 4, 3, 4);
            switchRoomButton.Name = "switchRoomButton";
            switchRoomButton.Size = new Size(75, 29);
            switchRoomButton.TabIndex = 9;
            switchRoomButton.Text = "방 전환";
            switchRoomButton.UseVisualStyleBackColor = true;
            switchRoomButton.Click += switchRoomButton_Click;
            //
            // createRoomButton
            //
            createRoomButton.Location = new Point(12, 392);
            createRoomButton.Margin = new Padding(3, 4, 3, 4);
            createRoomButton.Name = "createRoomButton";
            createRoomButton.Size = new Size(75, 29);
            createRoomButton.TabIndex = 8;
            createRoomButton.Text = "방 생성";
            createRoomButton.UseVisualStyleBackColor = true;
            createRoomButton.Click += createRoomButton_Click;
            //
            // refreshRoomListButton
            //
            refreshRoomListButton.Location = new Point(12, 466);
            refreshRoomListButton.Margin = new Padding(3, 4, 3, 4);
            refreshRoomListButton.Name = "refreshRoomListButton";
            refreshRoomListButton.Size = new Size(75, 29);
            refreshRoomListButton.TabIndex = 9;
            refreshRoomListButton.Text = "갱신";
            refreshRoomListButton.UseVisualStyleBackColor = true;
            refreshRoomListButton.Click += refreshRoomListButton_Click;
            //
            // currentRoomLabel
            //
            currentRoomLabel.AutoSize = true; // 텍스트 길이에 따라 자동으로 크기 조절
            currentRoomLabel.Location = new Point(12, 370); // 적절한 위치로 조정
            currentRoomLabel.Name = "currentRoomLabel";
            currentRoomLabel.Size = new Size(100, 15); // 적절한 초기 크기
            currentRoomLabel.TabIndex = 10; // 기존 컨트롤들의 TabIndex와 겹치지 않게 조정
            currentRoomLabel.Text = "현재 방: 로비"; // 초기 텍스트 설정
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 508);
            Controls.Add(sendButton);
            Controls.Add(messageInputTextBox);
            Controls.Add(chatListBox);
            Controls.Add(connectButton);
            Controls.Add(portTextBox);
            Controls.Add(ipTextBox);
            Controls.Add(userIdTextBox);
            Controls.Add(userListBox);
            Controls.Add(roomListBox);
            Controls.Add(switchRoomButton);
            Controls.Add(createRoomButton);
            Controls.Add(refreshRoomListButton);
            Controls.Add(currentRoomLabel); // 새로 추가: currentRoomLabel을 폼에 추가
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(400, 365);
            Name = "Form1";
            Text = "채팅 클라이언트";
            ResumeLayout(false);
            PerformLayout(); // PerformLayout() 호출은 SuspendLayout() 다음에 와야 합니다.

        }

        #endregion

        private System.Windows.Forms.Button createRoomButton;
        private System.Windows.Forms.Button refreshRoomListButton;
        private System.Windows.Forms.ListBox roomListBox;
        private System.Windows.Forms.Button switchRoomButton;
        private System.Windows.Forms.ListBox userListBox;
        private System.Windows.Forms.TextBox userIdTextBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ListBox chatListBox;
        private System.Windows.Forms.TextBox messageInputTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label currentRoomLabel; // 새로 추가: currentRoomLabel 멤버 변수 선언
    }
}