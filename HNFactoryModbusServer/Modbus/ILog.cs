using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNFactoryModbusServer.Modbus
{
    public interface ILog
    {
        void Write(string log);
    }
}
