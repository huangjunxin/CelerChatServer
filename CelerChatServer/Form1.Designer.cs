namespace CelerChatServer {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.chatHistoryTextBox = new System.Windows.Forms.TextBox();
            this.clientConnectionList = new System.Windows.Forms.ListBox();
            this.startButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chatHistoryTextBox
            // 
            this.chatHistoryTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.chatHistoryTextBox.Font = new System.Drawing.Font("更纱黑体 SC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chatHistoryTextBox.Location = new System.Drawing.Point(12, 12);
            this.chatHistoryTextBox.Multiline = true;
            this.chatHistoryTextBox.Name = "chatHistoryTextBox";
            this.chatHistoryTextBox.ReadOnly = true;
            this.chatHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatHistoryTextBox.Size = new System.Drawing.Size(432, 402);
            this.chatHistoryTextBox.TabIndex = 0;
            // 
            // clientConnectionList
            // 
            this.clientConnectionList.Font = new System.Drawing.Font("更纱黑体 SC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.clientConnectionList.FormattingEnabled = true;
            this.clientConnectionList.ItemHeight = 22;
            this.clientConnectionList.Location = new System.Drawing.Point(450, 12);
            this.clientConnectionList.Name = "clientConnectionList";
            this.clientConnectionList.Size = new System.Drawing.Size(152, 422);
            this.clientConnectionList.TabIndex = 1;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(369, 420);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(614, 455);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.clientConnectionList);
            this.Controls.Add(this.chatHistoryTextBox);
            this.Name = "Form1";
            this.Text = "CelerChatServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatHistoryTextBox;
        private System.Windows.Forms.ListBox clientConnectionList;
        private System.Windows.Forms.Button startButton;
    }
}

