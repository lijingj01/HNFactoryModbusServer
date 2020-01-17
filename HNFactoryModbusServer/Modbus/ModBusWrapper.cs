using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNFactoryModbusServer.Modbus
{
    public enum Protocol
    {
        TCPIP,
        SerialPort
    }

    public abstract class ModBusWrapper : IDisposable
    {
        private static ModBusWrapper _Instance;

        public static ModBusWrapper CreateInstance(Protocol protocol, string strPLCServerIP, int iPLCServerPort, short startAddress)
        {
            if (_Instance == null)
            {
                switch (protocol)
                {
                    case Protocol.TCPIP:
                        _Instance = new ModBusTCPIPWrapper(strPLCServerIP,iPLCServerPort,startAddress);
                        break;
                    case Protocol.SerialPort:
                        _Instance = new ModBusSerialPortWrapper(strPLCServerIP, iPLCServerPort,startAddress);
                        break;
                    default:
                        break;
                }
            }
            return _Instance;
        }


        #region Transaction Identifier
        /// <summary>
        /// 数据序号标识
        /// </summary>
        private byte dataIndex = 0;

        public byte CurrentDataIndex
        {
            get { return this.dataIndex; }
        }

        public byte NextDataIndex()
        {
            return ++this.dataIndex;
        }
        #endregion

        /// <summary>
        /// PLC服务器IP
        /// </summary>
        public string PLCServerIP { get; set; }
        /// <summary>
        /// PLC服务器端口
        /// </summary>
        public int PLCServerPort { get; set; }
        /// <summary>
        /// 读取寄存器地址起点
        /// </summary>
        public short StartingAddress { get; set; }
        /// <summary>
        /// 读取寄存器数量
        /// </summary>
        public short RegCount { get; set; }

        public ModBusWrapper(string strPLCServerIP,int iPLCServerPort,short startAddress)
        {
            this.PLCServerIP = strPLCServerIP;
            this.PLCServerPort = iPLCServerPort;
            this.StartingAddress = startAddress;
        }

        public ILog Logger { get; set; }

        public abstract void Connect();

        public abstract void Close();

        public abstract byte[] Receive();

        public abstract void Send(byte[] data);

        #region IDisposable 成员
        public virtual void Dispose()
        {
        }
        #endregion
    }
}
