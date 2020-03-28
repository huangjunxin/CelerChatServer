using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CelerChatServer {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        static Socket serverSocket = null;
        static Dictionary<string, Socket> clientConnectionDict = new Dictionary<string, Socket> { };

        private void startButton_Click(object sender, EventArgs e) {
            // 禁用Button
            startButton.Enabled = false;

            IPAddress ip = IPAddress.Parse("127.0.0.1");

            // 定义套接字用于监听客户端发来的消息
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // 绑定监听的网络节点
            serverSocket.Bind(new IPEndPoint(ip, 10086));

            // 将套接字的监听队列长度限制为10
            serverSocket.Listen(10);

            // 创建一个监听线程
            Thread threadLisenConnect = new Thread(ListenConnect);
            threadLisenConnect.IsBackground = true;
            threadLisenConnect.Start();
        }

        // 客户端连接后的socket
        static Socket connection = null;

        void ListenConnect() {
            // 不断监听客户端发来的请求
            while (true) {
                try {
                    connection = serverSocket.Accept();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    break;
                }

                // 获取客户端网络节点号
                string remoteEndPoint = connection.RemoteEndPoint.ToString();

                // 增加至clientConnectionList
                BeginInvoke(new Action(() => {
                    clientConnectionList.Items.Insert(0, remoteEndPoint);
                }));

                // 增加至clientConnectionDict
                clientConnectionDict.Add(remoteEndPoint, connection);

                // 创建一个通信线程，用于接收客户端发来的信息
                Thread thread = new Thread(Recv);
                thread.IsBackground = true;
                thread.Start(connection);
            }
        }

        void Recv(object sc) {
            Socket socketServer = sc as Socket;

            while (true) {
                // 创建内存缓冲区
                byte[] srcMsgBuffer = new byte[1024 * 1024];

                try {
                    int len = socketServer.Receive(srcMsgBuffer);
                    
                    // 将byte转换为string
                    string srcMsgString = Encoding.UTF8.GetString(srcMsgBuffer, 0, len);

                    // 获取当前时间
                    string nowDateTime = DateTime.Now.ToString();

                    // 将收到的字符串拼接成一条新消息
                    string newMsg = socketServer.RemoteEndPoint + " " + nowDateTime;
                    newMsg += Environment.NewLine;
                    newMsg += "　" + srcMsgString;
                    newMsg += Environment.NewLine;

                    // 将新消息增加到chatHistory
                    string chatHistory = chatHistoryTextBox.Text;
                    chatHistory += newMsg;

                    // 将增加新消息后的chatHistory更新到chatHistoryTextBox
                    BeginInvoke(new Action(() => {
                        chatHistoryTextBox.Text = chatHistory;

                        // 滚动到最底部
                        chatHistoryTextBox.SelectionStart = chatHistoryTextBox.Text.Length;
                        chatHistoryTextBox.ScrollToCaret();
                    }));

                    // 将新消息发回给所有已连接的客户端
                    if (clientConnectionDict.Count > 0) {
                        string sendNewMsg = newMsg;
                        byte[] sendNewMsgBuffer = Encoding.UTF8.GetBytes(sendNewMsg);
                        foreach (var clientConnection in clientConnectionDict) {
                            clientConnection.Value.Send(sendNewMsgBuffer);
                        }
                    }

                } catch (Exception ex) {
                    string toRemoveIP = socketServer.RemoteEndPoint.ToString();

                    // 当发现连接断开时，应删除clientConnectionDict中的项
                    clientConnectionDict.Remove(toRemoveIP);

                    // 当发现连接断开时，应删除chatHistoryTextBox中的项
                    BeginInvoke(new Action(() => {
                        clientConnectionList.Items.Remove(toRemoveIP);
                    }));

                    Console.WriteLine(ex.Message);
                    socketServer.Close();
                    break;
                }
            }
        }
    }
}
