using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Modbus;
using GHIElectronics.TinyCLR.Devices.Modbus.Interface;
using GHIElectronics.TinyCLR.Devices.Network;

namespace SC20100D_Modbus_Client
{
    public class ModbusTcpSlave
    {
        byte address;
        
        ModbusTcpListener mbListener;

        ModbusDevice ModbusTCP_Device;

        bool link_required = true;
        bool isRunning = false;
        public ModbusTcpSlave(byte address)
        {
            this.address = address;
        }
        public void Start()
        {
            if (!isRunning)
            {
                link_required = true;
                var Ethernet_Thread = new Thread(new ThreadStart(runThread));
                Ethernet_Thread.Start();
            }
        }
        public void Stop()
        {
            if (link_required)
            {
                link_required = false;
            }
        }
        private void runThread()
        {
            ModbusTCP_Device = new MyModbusDevice(address);
            
            mbListener = new ModbusTcpListener(ModbusTCP_Device, 502, 5, 1000);
            Thread.Sleep(100);

            isRunning = true;
            ModbusTCP_Device.Start();

            while (link_required == true)
            {
                if (ModbusTCP_Device.IsRunning == true)
                {
                    //System.Diagnostics.Debug.WriteLine("Running   ");                 
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Modbus Devic is Stopped ");
                }

                Thread.Sleep(1000);
            }
            isRunning = false;
        }
    }
}
