using Newtonsoft.Json;
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
        static Dictionary<string, string> clientNicknameDict = new Dictionary<string, string> { };

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
                byte[] srcObjectBuffer = new byte[1024 * 1024];

                try {
                    int len = socketServer.Receive(srcObjectBuffer);

                    // 将byte转换为string
                    string srcObjectString = Encoding.UTF8.GetString(srcObjectBuffer, 0, len);

                    // 将string转换为Json
                    ContactObject co = JsonConvert.DeserializeObject<ContactObject>(srcObjectString);

                    // 获取客户端网络节点号
                    string remoteEndPoint = socketServer.RemoteEndPoint.ToString();

                    // 声明返回用变量
                    string newMsg = null;

                    if (co != null) {
                        // 若nickname不为空，则说明是首次加入
                        if (co.nickname != null) {
                            // 将nickname增加至clientNicknameDict
                            clientNicknameDict.Add(remoteEndPoint, co.nickname);

                            // 增加至clientConnectionList
                            BeginInvoke(new Action(() => {
                                clientConnectionList.Items.Insert(0, co.nickname);
                            }));

                            // 加入聊天室提示
                            string newInfo = clientNicknameDict[remoteEndPoint] + " entered the chat room.";

                            // 根据nickname与newInfo创建新消息
                            newMsg = MakeNewMsg(clientNicknameDict[remoteEndPoint], newInfo);
                        } else {
                            // 根据nickname与co.msg创建新消息
                            newMsg = MakeNewMsg(clientNicknameDict[remoteEndPoint], co.msg);
                        }
                    }

                    // 将新消息发回给所有已连接的客户端
                    if (clientConnectionDict.Count > 0) {
                        string sendNewMsg = newMsg;
                        byte[] sendNewMsgBuffer = Encoding.UTF8.GetBytes(sendNewMsg);
                        foreach (var clientConnection in clientConnectionDict) {
                            // 不必发回给发送该消息的客户端
                            if (clientConnection.Key != socketServer.RemoteEndPoint.ToString()) {
                                clientConnection.Value.Send(sendNewMsgBuffer);
                            }
                        }
                    }

                } catch (Exception ex) {
                    string toRemoveIP = socketServer.RemoteEndPoint.ToString();

                    // 当发现连接断开时，应删除clientConnectionDict中的项
                    if (clientConnectionDict.ContainsKey(toRemoveIP)) {
                        clientConnectionDict.Remove(toRemoveIP);
                    }

                    // 当发现连接断开时，应删除chatHistoryTextBox中的项
                    BeginInvoke(new Action(() => {
                        if (clientNicknameDict.ContainsKey(toRemoveIP) && clientConnectionList.Items.Contains(clientNicknameDict[toRemoveIP])) {
                            clientConnectionList.Items.Remove(clientNicknameDict[toRemoveIP]);
                        }
                    }));

                    if (clientNicknameDict.ContainsKey(toRemoveIP)) {
                        // 当发现连接断开时，应删除clientNicknameDict中的项
                        clientNicknameDict.Remove(clientNicknameDict[toRemoveIP]);
                    }

                    Console.WriteLine(ex.Message);
                    socketServer.Close();
                    break;
                }
            }
        }

        public string MakeNewMsg(string nickname, string msg) {
            // 获取当前时间
            string nowDateTime = DateTime.Now.ToString();

            // 即将发送的字符串拼接成一条新消息
            string newMsg = nickname + " " + nowDateTime;
            newMsg += Environment.NewLine;
            newMsg += "　" + msg;
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

            return newMsg;
        }
    }

    class ContactObject {
        public string nickname = null;
        public string msg = null;
    }
}
