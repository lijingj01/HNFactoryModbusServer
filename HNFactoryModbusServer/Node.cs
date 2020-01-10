using HNFactoryModbusServer.Data;
using Modbus.Data;
using Modbus.Device;
using Modbus.Message;
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

namespace HNFactoryModbusServer
{
    public class Node : IDisposable
    {

        #region 相关配置属性

        private Thread _thread;
        private ModbusSlave _modbusSlave;
        private TcpListener _listener;
        private IPAddress _ip;
        public string _status { get; set; } //状态
        public bool _isRunning = false;
        public int _id { get; set; }
        public int _port { get; set; }
        public string _name { get; set; }
        // public TurbineType _type{ get; set; }
        public string _typeStr { get; set; }
        private DataStore _dataStore;
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


        #endregion

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


        public void TestToDB()
        {
            ModbusMessageLog item = new ModbusMessageLog();
            item.LogTime = DateTime.Now;
            item.LogMessage = "Test111";
            item.FunctionCode = "12";
            //item.MessageFrame = "MessageFrame";
            //item.ProtocolDataUnit = "ProtocolDataUnit";
            item.SlaveAddress = "SlaveAddress";
            item.TransactionId = "TransactionId";

            //将数据传入后台处理
            string strToText = item.ToJsonString();

            SendToSocketServer(strToText);
        }
        #endregion

        public DataStore getDataStore()
        {
            return _dataStore;
        }

        public Node(int id, int port, string typeStr)
        {
            _id = id;
            _port = port;
            _typeStr = typeStr;
            _name = id.ToString() + "#设备";
            _dataStore = DataStoreFactory.CreateDefaultDataStore();
            _status = "服务未启动";
            string errStr = "";
            if (!initNodeData(ref errStr))
            {
                string outStr = "[" + _name + "]" + "CSV数据解析失败! \n请检查" + typeStr + ".CSV \n";
                outStr += errStr;
                MessageBox.Show(outStr, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);  //用风机类型初始化数据
            }

        }

        public void start()
        {
            if (_isRunning)
                return;
            if (IsPortUsed(_port))
            {
                string errorStr = "TCP端口：[" + _port.ToString() + "]" + "被占用";
                MessageBox.Show(errorStr, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _ip = new IPAddress(new byte[] { 127, 0, 0, 1 }); // new IPAddress(new byte[] { 0, 0, 0, 0 }); //
                _listener = new TcpListener(_ip, _port);

                _modbusSlave = ModbusTcpSlave.CreateTcp(_slaveId, _listener);
                _modbusSlave.DataStore = _dataStore;
                _modbusSlave.ModbusSlaveRequestReceived += requestReceiveHandler;

                _thread = new Thread(_modbusSlave.Listen) { Name = _port.ToString() };
                _thread.Start();
                _status = "服务运行中";
                _isRunning = true;

                ConnectSocketServer();
            }
            catch (Exception ex)
            {
                string errorStr = "[" + _name + "]" + "服务启动失败!" + ex.Message;
                MessageBox.Show(errorStr, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 处理PLC发送的请求数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void requestReceiveHandler(object sender, ModbusSlaveRequestEventArgs e)
        {
            IModbusMessage message = e.Message;

            #region 读取的日志数据写入数据库
            //Thread tdb = new Thread(RequestToDB);
            //tdb.IsBackground = true;
            //tdb.Start(message);
            RequestToDB(message);

            #endregion

            string writeLogStr = _name + ": " + message;

            if (message.FunctionCode == 6)//6是写单个寄存器
            {
                Program._logForm.addWriteLog(writeLogStr);
                return;
            }
            else if (message.FunctionCode == 16)//16是写多个模拟量寄存器
            {
                Program._logForm.addWriteLog(writeLogStr);
                return;
            }
            string logStr = _name + " Receive Request：" + message;
            Program._logForm.addLog(logStr);
        }

        private void RequestToDB(IModbusMessage message)
        {
            WriteMultipleRegistersRequest typedResponse = (WriteMultipleRegistersRequest)message;

            ModbusMessageLog logitem = new ModbusMessageLog()
            {
                LogTime = DateTime.Now,
                LogMessage = message.ToString(),
                FunctionCode = message.FunctionCode.ToString(),
                MessageFrame = message.MessageFrame,
                ProtocolDataUnit = message.ProtocolDataUnit,
                SlaveAddress = message.SlaveAddress.ToString(),
                TransactionId = message.TransactionId.ToString()
            };

            int iStartIndex = typedResponse.MinimumFrameSize;
            int iLen = message.MessageFrame.Length - iStartIndex;
            byte[] bdata = new byte[iLen];
            Array.Copy(message.MessageFrame, iStartIndex, bdata, 0, iLen);

            string strFrame = System.Text.Encoding.UTF8.GetString(bdata).TrimEnd('\0');
            string strUnit = Modbus.ConvertTools.BytesToHexString(message.ProtocolDataUnit);

            DataLogHelper.AddModbusMessageLog(logitem);

            //将数据传入后台处理
            string strToText = logitem.ToJsonString();

            SendToSocketServer(strToText);
        }

        public void stop()
        {
            if (_isRunning == false)
            {
                return;
            }
            _status = "服务停止";
            _isRunning = false;
            try
            {

                _modbusSlave.Dispose();
                //_listener.Stop();
                _thread.Abort();
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            stop();
        }

        #region PLC的数据解码

        /// <summary>
        /// 根据3或4返回适合的寄存器
        /// </summary>
        /// <param name="groupindex"></param>
        /// <returns></returns>
        ModbusDataCollection<ushort> getRegisterGroup(int groupindex)//
        {
            switch (groupindex)
            {
                case 3: return _dataStore.HoldingRegisters; //可由moddbus修改
                case 4: return _dataStore.InputRegisters;   //不可通过modbus修改
                default: return _dataStore.InputRegisters;
            }
        }
        public void setValue16(int groupindex, int offset, ushort value)
        {
            ModbusDataCollection<ushort> data = getRegisterGroup(groupindex);
            data[offset] = value;
        }
        public void setValue32(int groupindex, int offset, int value)
        {
            byte[] valueBuf = BitConverter.GetBytes(value);
            ushort lowOrderValue = BitConverter.ToUInt16(valueBuf, 0);
            ushort highOrderValue = BitConverter.ToUInt16(valueBuf, 2);

            ModbusDataCollection<ushort> data = getRegisterGroup(groupindex);
            data[offset] = lowOrderValue;
            data[offset + 1] = highOrderValue;
        }
        public void setValue32(int groupindex, int offset, float value)
        {
            ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            ModbusDataCollection<ushort> data = getRegisterGroup(groupindex);
            data[offset] = lowOrderValue;
            data[offset + 1] = highOrderValue;
        }
        public void setValue16(int groupindex, int offset, bool value)
        {
            byte[] valueBuf = BitConverter.GetBytes(value);//用1代替true
            ushort lowOrderValue = BitConverter.ToUInt16(valueBuf, 0);

            ModbusDataCollection<ushort> data = getRegisterGroup(groupindex);
            data[offset] = lowOrderValue;

        }
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



                switch (dataTypeStr.ToUpper())
                {
                    case "INT16":
                    case "WORD":
                    case "INT"://目前主控把INT当作16位
                        setValue16(groupindex, offset, (ushort)(value_f * coe_reverse));
                        break;
                    case "INT32":
                    case "DINT":
                    case "DWORD":
                        setValue32(groupindex, offset, (int)value_f * coe_reverse);
                        break;
                    case "REAL":
                    case "FLOAT":
                        value_f = float.Parse(valueStr);
                        setValue32(groupindex, offset, value_f * coe_reverse);
                        break;
                    case "BIT"://先不管
                        return true;
                    default:
                        setValue16(groupindex, offset, (ushort)(value_f * coe_reverse));
                        break;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public void setValueByName(string ioName, string valueStr)
        {
            if (ioName.Length == 0 || valueStr.Length == 0)
            {
                return;
            }

            int ioNameIndex = 0;
            if (!fetch(ioName, ref ioNameIndex))
            {
                return;
            }
            try
            {
                int groupindex = int.Parse(_dt.Rows[ioNameIndex]["groupIndex"].ToString());              //功能码
                int offset = int.Parse(_dt.Rows[ioNameIndex]["offs"].ToString());

                string dataTypeStr = _dt.Rows[ioNameIndex]["dataType"].ToString();
                float coe = float.Parse(_dt.Rows[ioNameIndex]["coe"].ToString());
                int coe_reverse = floatToInt(1.0000f / coe);

                bool ret = setValueUniverse(groupindex, offset, dataTypeStr, coe_reverse, valueStr); //coe在这不起作用
            }
            catch (Exception e)
            {
                return;
            }

        }

        private bool fetch(string ioName, ref int index)//查找ioName 在_dt中的index
        {
            if (_ioName2index.ContainsKey(ioName))
            {
                index = _ioName2index[ioName];
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        /// <summary>
        /// 判断指定端口号是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        internal static Boolean IsPortUsed(Int32 port)
        {
            Boolean result = false;
            try
            {
                System.Net.NetworkInformation.IPGlobalProperties iproperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                System.Net.IPEndPoint[] ipEndPoints = iproperties.GetActiveTcpListeners();
                //System.Net.NetworkInformation.TcpConnectionInformation[] conns = iproperties.GetActiveTcpConnections();

                //foreach (var con in conns)
                foreach (var con in ipEndPoints)
                {
                    // if (con.LocalEndPoint.Port == port)
                    if (con.Port == port)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

    }

    public static class NodeMgr
    {
        public static List<Node> _nodeList = new List<Node>();
        private static bool _initFlag = false;//初始化标识

        static public List<Node> getNodeList()
        {
            return _nodeList;
        }
        static public bool init(string cfgFile)
        {
            if (_initFlag)
            {
                stopAll();
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
                    int port = int.Parse(dt.Rows[i][1].ToString());
                    string typeStr = dt.Rows[i][2].ToString();

                    Node node = new Node(id, port, typeStr);
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

        static public void stopAll()
        {
            foreach (Node node in _nodeList)
            {
                node.stop();
            }
        }

        static public void startAll()
        {
            foreach (Node node in _nodeList)
            {
                node.start();
            }
        }

        static public void startNode(int id)
        {
            foreach (Node node in _nodeList)
            {
                if (node._id == id)
                {
                    node.start();
                }
            }
        }

        static public void stopNode(int id)
        {
            foreach (Node node in _nodeList)
            {
                if (node._id == id)
                {
                    node.stop();
                }
            }
        }


        public static void TestServer()
        {
            foreach (Node node in _nodeList)
            {
                node.TestToDB();
            }
        }
    }
}
