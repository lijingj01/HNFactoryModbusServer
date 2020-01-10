using HNFactoryModbusServer.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HNFactoryModbusServer.Modbus
{
    /// <summary>
    /// PLC服务器相关信息
    /// </summary>
    public class PLCNode
    {
        #region 相关配置属性

        private Thread _thread;
        private TcpListener _listener;
        private IPAddress _ip;
        public string _status { get; set; } //状态
        public bool _isRunning = false;
        public int _id { get; set; }
        /// <summary>
        /// PLC服务器IP
        /// </summary>
        public string _plcip { get; set; }
        public int _port { get; set; }
        public string _name { get; set; }
        // public TurbineType _type{ get; set; }
        public string _typeStr { get; set; }
        /// <summary>
        /// 刷新频率（秒）
        /// </summary>
        public int _rate { get; set; }
        /// <summary>
        /// 设备id,默认为1
        /// </summary>
        private byte _slaveId = 1;
        /// <summary>
        /// 保存配置文件
        /// </summary>
        private DataTable _dt = new DataTable();
        /// <summary>
        /// 保存ioName 到在datable中的索引
        /// </summary>
        private Dictionary<string, int> _ioName2index = new Dictionary<string, int>();

        /// <summary>
        /// 毫秒量
        /// </summary>
        private int _Milliseconds = 1000;

        /// <summary>
        /// PLC连接对象
        /// </summary>
        public ModBusWrapper Wrapper = null;
        #endregion

        public PLCNode(int id,string ip,int port, string typeStr, string comments,int rate)
        {
            _id = id;
            _plcip = ip;
            _port = port;
            _typeStr = typeStr;
            _name = comments;
            _status = "服务未连接";
            _rate = rate;

            Wrapper = ModBusWrapper.CreateInstance(Protocol.TCPIP, ip, port);
        }

        #region 连接关闭服务器操作
        public void open()
        {
            Wrapper.Connect();

            try
            {
                _thread = new Thread(Receive) { Name = _id.ToString() };
                _thread.Start();
                _status = "服务连接中";
                _isRunning = true;
            }
            catch (Exception ex)
            {
                string errorStr = "[" + _name + "]" + "服务启动失败!" + ex.Message;
                MessageBox.Show(errorStr, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void close()
        {

            if (_isRunning == false)
            {
                return;
            }
            _status = "连接断开";
            _isRunning = false;
            try
            {

                Wrapper.Close();
                //_listener.Stop();
                _thread.Abort();
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 按定义的间隔循环提取PLC数据
        /// </summary>
        public void Receive()
        {
            while (_isRunning)
            {
                byte[] plcData = Wrapper.Receive();
                //保存数据
                RequestToDB(plcData);

                //暂停指定的时间提取数据
                Thread.Sleep(_Milliseconds * _rate);
            }
        }

        /// <summary>
        /// 将数据保存起来并发送给后台服务
        /// </summary>
        /// <param name="data"></param>
        private void RequestToDB(byte[] data)
        {
            string strMessage = Encoding.ASCII.GetString(data);
            ModbusMessageLog logitem = new ModbusMessageLog()
            {
                LogTime = DateTime.Now,
                LogMessage = strMessage,
                FunctionCode = FunctionCode.Read.ToString(),
                MessageFrame = data,
                ProtocolDataUnit = data,
                SlaveAddress = ModBusTCPIPWrapper.StartingAddress.ToString(),
                TransactionId = Wrapper.CurrentDataIndex.ToString()
            };

            DataLogHelper.AddModbusMessageLog(logitem);

            //将数据传入后台处理
            string strToText = logitem.ToJsonString();

            SendToSocketServer(strToText);
        }

        #region Socket相关配置
        /// <summary>
        /// 缓存字节最大长度
        /// </summary>
        private int BufferMax = 1024 * 1024 * 2;
        private string SocketServerIP;
        private int SocketServerPoint;
        Socket socketSend;
        private bool IsConnectServer = false;
        private void ConnectSocketServer()
        {
            SocketServerIP = appString.GetAppsettingStr("StockServerIP");
            SocketServerPoint = Convert.ToInt32(appString.GetAppsettingStr("StockServerPort"));

            IPAddress ip = IPAddress.Parse(SocketServerIP);
            IPEndPoint point = new IPEndPoint(ip, SocketServerPoint);

            try
            {
                //创建负责通信的Socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //获得要连接的远程服务器应用程序的IP地址和端口号
                socketSend.Connect(point);

                //开启一个新的线程不停的接收服务端发来的消息
                Thread th = new Thread(SocketRecive);
                th.IsBackground = true;
                th.Start();

                IsConnectServer = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 客户端给服务器发送消息
        /// </summary>
        /// <param name="strText"></param>
        private void SendToSocketServer(string strText)
        {
            if (IsConnectServer)
            {
                strText += "|";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strText);
                socketSend.Send(buffer);
            }
        }

        /// <summary>
        /// 不停的接受服务器发来的消息
        /// </summary>
        private void SocketRecive()
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

                }
                catch (Exception ex)
                {

                }

            }
        }
        #endregion


        public void Dispose()
        {
            close();
        }
        #endregion


        #region 初始化操作

        /// <summary>
        /// initData
        /// </summary>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        public bool initNodeData(ref string errorStr)
        {
            _dt = null;

            try
            {
                if (_typeStr.Length == 0)
                { return false; }

                string configDirStr = MAppConfig.getValueByName("defaultCfgDir");
                string csvFileName = configDirStr + "/" + _typeStr + ".csv";// _typeStr是MY1500， 采用MY1500.csv作为模型名
                bool ret = CSVReader.readCSV(csvFileName, out _dt);
                if (!ret)
                {
                    return false;
                }

                for (int i = 0; i < _dt.Rows.Count; i++) //写入各行数据
                {
                    {
                        string ioName = _dt.Rows[i]["path"].ToString();
                        if (ioName.Length == 0)
                        {
                            errorStr = csvFileName + "[path] 列出现空值";
                            return false;
                        }

                        _ioName2index[ioName] = i;
                    }

                    string groupindexStr = _dt.Rows[i]["groupIndex"].ToString();
                    if (groupindexStr.Length == 0)
                    {
                        errorStr = csvFileName + "[groupIndex] 列出现空值";
                        return false;
                    }
                    int groupindex = int.Parse(groupindexStr);     //功能码

                    string offsetStr = _dt.Rows[i]["offs"].ToString();
                    if (offsetStr.Length == 0)
                    {
                        errorStr = csvFileName + "[offs] 列出现空值";
                        return false;
                    }
                    if (offsetStr.Contains(':'))
                    {
                        offsetStr = offsetStr.Substring(0, offsetStr.IndexOf(":"));
                    }
                    int offset = int.Parse(offsetStr);

                    string dataTypeStr = _dt.Rows[i]["dataType"].ToString();
                    if (dataTypeStr.Length == 0)
                    {
                        errorStr = csvFileName + "[dataType] 列出现空值";
                        return false;
                    }

                    float coe = float.Parse(_dt.Rows[i]["coe"].ToString());
                    int coe_reverse = floatToInt(1.00000000f / coe);//1.0除以0.1得到0.9
                    string valueStr = "0";
                    if (_dt.Columns.Contains("value"))
                    {
                        valueStr = _dt.Rows[i]["value"].ToString();//如果有value这一列就赋值，否则默认是0
                    }

                    bool ret1 = setValueUniverse(groupindex, offset, dataTypeStr, coe_reverse, valueStr);
                    if (ret1 != true)
                    {
                        return false;
                    }
                }//for

                return true;
            }
            catch (Exception e)
            {
                errorStr = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public int floatToInt(float f)
        {
            int i = 0;
            if (f > 0) //正数
                i = (int)((f * 10 + 5) / 10);
            else if (f < 0) //负数
                i = (int)((f * 10 - 5) / 10);
            else i = 0;

            return i;
        }

        bool setValueUniverse(int groupindex, int offset, string dataTypeStr, int coe_reverse, string valueStr)
        {
            float value_f;
            string offsetAddOne = MAppConfig.getValueByName("offsetAddOne");
            if (offsetAddOne != "0")
                offset += 1;//此处的内存对应的是modbus协议中的地址，比offset要多1。
            else
                offset += 0;//只在配置为0时才不加1

            try
            {
                if (valueStr.Contains('.'))//有些点虽为INT型，但最终的值是float。风速为INT，modbus值为988这样。
                {
                    value_f = float.Parse(valueStr);
                }
                else
                {
                    value_f = int.Parse(valueStr);
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        #endregion
    }

    public static class PLCNodeMgr
    {
        public static List<PLCNode> _nodeList = new List<PLCNode>();
        private static bool _initFlag = false;//初始化标识


        static public List<PLCNode> getNodeList()
        {
            return _nodeList;
        }
        static public bool init(string cfgFile)
        {
            if (_initFlag)
            {
                openAll();
                _nodeList.Clear();
            }
            //读csv文件，初始化每个node
            DataTable dt;
            bool ret = CSVReader.readCSV(cfgFile, out dt);
            if (!ret)
                return false;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
                {

                    int id = int.Parse(dt.Rows[i][0].ToString());
                    string ip =  dt.Rows[i][1].ToString();
                    int port = int.Parse(dt.Rows[i][2].ToString());
                    string typeStr = dt.Rows[i][3].ToString();
                    string comments = dt.Rows[i][4].ToString();
                    int rate = int.Parse(dt.Rows[i][5].ToString());

                    PLCNode node = new PLCNode(id, ip, port, typeStr, comments, rate);
                    _nodeList.Add(node);
                }
                _initFlag = true;//已初始化
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        static public bool openCfgFile(string cfgFile)//打开一个配置文件
        {
            if (String.IsNullOrEmpty(cfgFile))
                return false;



            init(cfgFile);


            return true;
        }

        static public void closeAll()
        {
            foreach (PLCNode node in _nodeList)
            {
                node.close();
            }
        }

        static public void openAll()
        {
            foreach (PLCNode node in _nodeList)
            {
                node.open();
            }
        }

        static public void openNode(int id)
        {
            foreach (PLCNode node in _nodeList)
            {
                if (node._id == id)
                {
                    node.open();
                }
            }
        }

        static public void closeNode(int id)
        {
            foreach (PLCNode node in _nodeList)
            {
                if (node._id == id)
                {
                    node.close();
                }
            }
        }
    }
}
