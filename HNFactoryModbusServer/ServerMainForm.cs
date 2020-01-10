using HNFactoryModbusServer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HNFactoryModbusServer
{
    public partial class ServerMainForm : Form
    {
        private DataTable _dt = new DataTable();

        private Dictionary<int, Modbus.ModBusWrapper> Wrappers = null;
        public ServerMainForm()
        {
            InitializeComponent();
            readConfigFile();

            Wrappers = new Dictionary<int, Modbus.ModBusWrapper>();

            initUI();
            initPLC();
            //TestDB();

            timer1.Start();
            timer2.Start();
        }

        private void initUI()
        {
            dataGridView1.Rows.Clear();
            List<Node> nodeList = NodeMgr.getNodeList();
            foreach (Node node in nodeList)
            {
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = node._id;
                this.dataGridView1.Rows[index].Cells[1].Value = node._name;
                this.dataGridView1.Rows[index].Cells[2].Value = node._port;

                this.dataGridView1.Rows[index].Cells[4].Value = node._status;
            }
        }

        private void initPLC()
        {
            //加载PLC服务器集合
            dvPLCServer.Rows.Clear();
            List<Modbus.PLCNode> nodeList = Modbus.PLCNodeMgr.getNodeList();

            foreach (Modbus.PLCNode node in nodeList)
            {
                int index = this.dvPLCServer.Rows.Add();
                this.dvPLCServer.Rows[index].Cells[0].Value = node._id;
                this.dvPLCServer.Rows[index].Cells[1].Value = node._name;
                this.dvPLCServer.Rows[index].Cells[2].Value = node._plcip;
                this.dvPLCServer.Rows[index].Cells[3].Value = node._port;

                this.dvPLCServer.Rows[index].Cells[4].Value = node._status;
                this.dvPLCServer.Rows[index].Cells[5].Value = node._rate;
            }
        }

        private void updateUI()
        {
            try
            {
                List<Node> nodeList = NodeMgr.getNodeList();
                for (int index = 0; index < nodeList.Count; index++)
                {
                    this.dataGridView1.Rows[index].Cells[0].Value = nodeList[index]._id;
                    this.dataGridView1.Rows[index].Cells[1].Value = nodeList[index]._name;
                    this.dataGridView1.Rows[index].Cells[2].Value = nodeList[index]._port;
                    this.dataGridView1.Rows[index].Cells[3].Value = nodeList[index]._typeStr;
                    this.dataGridView1.Rows[index].Cells[4].Value = nodeList[index]._status;

                    if (nodeList[index]._isRunning == true)
                    {
                        this.dataGridView1.Rows[index].Cells[4].Style.BackColor = Color.DeepSkyBlue;
                    }
                    else { this.dataGridView1.Rows[index].Cells[4].Style.BackColor = Color.White; }
                }
            }
            catch
            { }

        }
        private void readConfigFile()
        {
            if (MAppConfig.InitFromFile())
            {
                string defaultCfgFile = MAppConfig.getValueByName("defaultCfgFile");
                string defaultPLCCfgFile = MAppConfig.getValueByName("defaultPLCCfgFile");
                string configDirStr = MAppConfig.getValueByName("defaultCfgDir");
                string fileName = configDirStr + "/" + defaultCfgFile;
                //PLC服务器配置
                string plcFileName = configDirStr + "/" + defaultPLCCfgFile;

                if (!String.IsNullOrEmpty(fileName))
                {
                    if (!NodeMgr.init(fileName))//
                    {
                        MessageBox.Show(fileName + "模型解析失败!检查配置文件", "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (!String.IsNullOrEmpty(plcFileName))
                {
                    if (!Modbus.PLCNodeMgr.init(plcFileName))//
                    {
                        MessageBox.Show(plcFileName + " PLC服务器配置解析失败!检查配置文件", "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }


        private void ServerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("是否要退出程序？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            NodeMgr.stopAll();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            NodeMgr.startAll();


        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            NodeMgr.stopAll();
        }

        private void button_play_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program._logForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
                return;
            string selectIdStr = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            RegisterInspector regWindow = new RegisterInspector(selectIdStr);
            regWindow.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count <= 0)
                    return;

                foreach (DataGridViewRow Row in dataGridView1.SelectedRows)
                {
                    string selectIdStr = Row.Cells[0].Value.ToString();//查找第0列
                    int id = int.Parse(selectIdStr);
                    NodeMgr.stopNode(id);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateUI();
            
        }

        #region 针对Stock的连接操作

        Socket socketSend;

        private static byte[] result = new byte[1024];
        private void JoinStockServer()
        {
            //设定服务器IP地址

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint point = new IPEndPoint(ip, 1510);

            //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                //创建负责通信的Socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //获得要连接的远程服务器应用程序的IP地址和端口号
                socketSend.Connect(point);
                //statusStrip1.Text = "连接成功";

                //开启一个新的线程不停的接收服务端发来的消息
                Thread th = new Thread(Recive);
                th.IsBackground = true;
                th.Start();

                //clientSocket.Connect(new IPEndPoint(ip, 1510)); //配置服务器IP与端口

                //statusStrip1.Text = "连接服务器成功";
                MessageBox.Show("连接服务器成功");
            }
            catch
            {
                MessageBox.Show("连接服务器失败，请按回车键退出！");

                return;

            }

            //通过clientSocket接收数据

            //int receiveLength = clientSocket.Receive(result);

            //MessageBox.Show("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));

            //通过 clientSocket 发送数据
            //for (int i = 0; i < 10; i++)
            //{

            //    try
            //    {
            //        Thread.Sleep(1000);    //等待1秒钟

            //        string sendMessage = "client send Message Hellp" + i.ToString() + "|";

            //        SendToSocketServer(sendMessage);

            //        //MessageBox.Show("向服务器发送消息：{0}" + sendMessage);
            //    }
            //    catch
            //    {
            //        if (socketSend != null)
            //        {
            //            socketSend.Shutdown(SocketShutdown.Both);
            //            socketSend.Close();
            //        }
            //        break;
            //    }

            //}

            //MessageBox.Show("发送完毕，按回车键退出");

            //Console.ReadLine();

        }

        /// <summary>
        /// 客户端给服务器发送消息
        /// </summary>
        /// <param name="strText"></param>
        private void SendToSocketServer(string strText)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strText);
            socketSend.Send(buffer);
        }

        /// <summary>
        /// 不停的接受服务器发来的消息
        /// </summary>
        private void Recive()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int r = socketSend.Receive(buffer);
                    //实际接收到的有效字节数
                    if (r == 0)
                    {
                        break;
                    }
                    //表示发送的文字消息

                    string s = Encoding.UTF8.GetString(buffer, 0, r);
                    //ShowMsg(socketSend.RemoteEndPoint + ":" + s);
                    //statusStrip1.Text = s;
                    //MessageBox.Show(s);

                }
                catch(Exception ex) { 

                }

            }
        }


        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            //JoinStockServer();
            for(int i = 0; i < 10; i++)
            {
                NodeMgr.TestServer();
            }
        }

        private void btnConAll_Click(object sender, EventArgs e)
        {
            Modbus.PLCNodeMgr.openAll();
        }

        private void updatePLCUI()
        {
            try
            {
                List<Modbus.PLCNode> nodeList = Modbus.PLCNodeMgr.getNodeList();

                for (int index = 0; index < nodeList.Count; index++)
                {
                    this.dvPLCServer.Rows[index].Cells[0].Value = nodeList[index]._id;
                    this.dvPLCServer.Rows[index].Cells[1].Value = nodeList[index]._name;
                    this.dvPLCServer.Rows[index].Cells[2].Value = nodeList[index]._plcip;
                    this.dvPLCServer.Rows[index].Cells[3].Value = nodeList[index]._port;

                    this.dvPLCServer.Rows[index].Cells[4].Value = nodeList[index]._status;
                    this.dvPLCServer.Rows[index].Cells[5].Value = nodeList[index]._rate;

                    if (nodeList[index]._isRunning == true)
                    {
                        this.dataGridView1.Rows[index].Cells[4].Style.BackColor = Color.DeepSkyBlue;
                    }
                    else { this.dataGridView1.Rows[index].Cells[4].Style.BackColor = Color.White; }
                }
            }
            catch
            { }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            updatePLCUI();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Modbus.PLCNodeMgr.closeAll();
        }
    }
}
