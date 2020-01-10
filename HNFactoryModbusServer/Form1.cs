using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
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

namespace HNFactoryModbusServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Simple Modbus TCP slave example.
        /// </summary>
        public static void StartModbusTcpSlave()
        {

            byte slaveId = 1;
            int port = 502;

            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });
            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);

            slaveTcpListener.Start();

            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener);

            slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

            //object p = slave.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);

        }
    }
}
