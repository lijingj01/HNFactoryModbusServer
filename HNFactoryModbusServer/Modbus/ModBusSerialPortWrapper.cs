using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNFactoryModbusServer.Modbus
{
    internal class ModBusSerialPortWrapper : ModBusWrapper, IDisposable
    {
        public ModBusSerialPortWrapper(string strPLCServerIP, int iPLCServerPort, short startAddress) : base(strPLCServerIP, iPLCServerPort,startAddress) { }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public override void Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        #region IDisposable 成员
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
