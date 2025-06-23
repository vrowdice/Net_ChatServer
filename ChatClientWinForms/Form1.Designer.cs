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
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.chatListBox = new System.Windows.Forms.ListBox(); // <--- 이 부분이 추가됩니다.
            this.messageInputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // ipTextBox
            //
            this.ipTextBox.Location = new System.Drawing.Point(12, 12);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(150, 21);
            this.ipTextBox.TabIndex = 0;
            this.ipTextBox.Text = "127.0.0.1"; // 기본 IP 주소
            //
            // portTextBox
            //
            this.portTextBox.Location = new System.Drawing.Point(168, 12);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(80, 21);
            this.portTextBox.TabIndex = 1;
            this.portTextBox.Text = "7777"; // 기본 포트 번호
            //
            // connectButton
            //
            this.connectButton.Location = new System.Drawing.Point(254, 10);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "연결";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            //
            // chatListBox
            //
            this.chatListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatListBox.FormattingEnabled = true;
            this.chatListBox.ItemHeight = 12;
            this.chatListBox.Location = new System.Drawing.Point(12, 40);
            this.chatListBox.Name = "chatListBox";
            this.chatListBox.Size = new System.Drawing.Size(460, 268);
            this.chatListBox.TabIndex = 3;
            //
            // messageInputTextBox
            //
            this.messageInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageInputTextBox.Location = new System.Drawing.Point(12, 317);
            this.messageInputTextBox.Name = "messageInputTextBox";
            this.messageInputTextBox.Size = new System.Drawing.Size(379, 21);
            this.messageInputTextBox.TabIndex = 4;
            this.messageInputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.messageInputTextBox_KeyDown);
            //
            // sendButton
            //
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(397, 315);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "전송";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.messageInputTextBox);
            this.Controls.Add(this.chatListBox); // <--- Controls에 추가하는 부분
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.ipTextBox);
            this.MinimumSize = new System.Drawing.Size(400, 300); // 최소 크기 설정
            this.Name = "Form1";
            this.Text = "채팅 클라이언트";
            this.ResumeLayout(false);
            this.PerformLayout();

            this.userIdTextBox = new System.Windows.Forms.TextBox();
            this.userIdTextBox.Location = new System.Drawing.Point(340, 12);
            this.userIdTextBox.Name = "userIdTextBox";
            this.userIdTextBox.Size = new System.Drawing.Size(100, 21);
            this.userIdTextBox.TabIndex = 6;
            this.userIdTextBox.Text = "User123";
            this.Controls.Add(this.userIdTextBox);
            // 
            // userListBox
            // 
            this.userListBox = new System.Windows.Forms.ListBox();
            this.userListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.userListBox.FormattingEnabled = true;
            this.userListBox.ItemHeight = 12;
            this.userListBox.Location = new System.Drawing.Point(12, 40);  // 기존 chatListBox의 왼쪽
            this.userListBox.Name = "userListBox";
            this.userListBox.Size = new System.Drawing.Size(120, 268);
            this.userListBox.TabIndex = 7;
            this.Controls.Add(this.userListBox);

            this.chatListBox.Location = new System.Drawing.Point(138, 40);
            this.chatListBox.Size = new System.Drawing.Size(334, 268);

            this.messageInputTextBox.Location = new System.Drawing.Point(138, 317);
            this.messageInputTextBox.Size = new System.Drawing.Size(253, 21);

            this.sendButton.Location = new System.Drawing.Point(397, 315);
        }

        #endregion

        private System.Windows.Forms.ListBox userListBox;
        private System.Windows.Forms.TextBox userIdTextBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ListBox chatListBox;
        private System.Windows.Forms.TextBox messageInputTextBox;
        private System.Windows.Forms.Button sendButton;
    }
}