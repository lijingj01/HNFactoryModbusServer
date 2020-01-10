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

        public static ModBusWrapper CreateInstance(Protocol protocol, string strPLCServerIP, int iPLCServerPort)
        {
            if (_Instance == null)
            {
                switch (protocol)
                {
                    case Protocol.TCPIP:
                        _Instance = new ModBusTCPIPWrapper(strPLCServerIP,iPLCServerPort);
                        break;
                    case Protocol.SerialPort:
                        _Instance = new ModBusSerialPortWrapper(strPLCServerIP, iPLCServerPort);
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

        public string PLCServerIP { get; set; }
        public int PLCServerPort { get; set; }

        public ModBusWrapper(string strPLCServerIP,int iPLCServerPort)
        {
            this.PLCServerIP = strPLCServerIP;
            this.PLCServerPort = iPLCServerPort;
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
