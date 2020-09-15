// SC20100D_Modbus_Client
// SC20100D_Modbus_Client using tcp
// The example came up in response to this thread of @WombatWonder
// He noticed that the a Modbus Slave created with TiniClr library v2.0.0 answered only on the first modbus request, not on succeeding requests
// https://forums.ghielectronics.com/t/modbustcp-device/23249


using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Networking;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using System.Diagnostics;

namespace SC20100D_Modbus_Client
{
    class Program
    {
        private static NetworkController networkController;
        private static NetworkInterfaceSettings networkInterfaceSetting;
        private static SpiNetworkCommunicationInterfaceSettings networkCommunicationInterfaceSettings;
        private static GpioPin cs;
        private static SpiConnectionSettings settings;

        

        static void Main()
        {
            initializeNetwork();


            var myModbusTcpSlave = new ModbusTcpSlave((byte)248);
           
            myModbusTcpSlave.Start();

            Thread.Sleep(-1);
            

        }
        #region InitializeNetwork
        static void initializeNetwork()
        {

            networkController = GHIElectronics.TinyCLR.Devices.Network.NetworkController.FromName("GHIElectronics.TinyCLR.NativeApis.ENC28J60.NetworkController");
            networkInterfaceSetting = new GHIElectronics.TinyCLR.Devices.Network.EthernetNetworkInterfaceSettings();
            networkCommunicationInterfaceSettings = new GHIElectronics.TinyCLR.Devices.Network.SpiNetworkCommunicationInterfaceSettings();
            cs = GHIElectronics.TinyCLR.Devices.Gpio.GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20100.GpioPin.PD3);
            settings = new GHIElectronics.TinyCLR.Devices.Spi.SpiConnectionSettings()
            {
                ChipSelectLine = cs,
                ClockFrequency = 4000000,
                Mode = GHIElectronics.TinyCLR.Devices.Spi.SpiMode.Mode0,
                ChipSelectType = GHIElectronics.TinyCLR.Devices.Spi.SpiChipSelectType.Gpio,
                ChipSelectHoldTime = TimeSpan.FromTicks(10),
                ChipSelectSetupTime = TimeSpan.FromTicks(10)
            };
            networkCommunicationInterfaceSettings.SpiApiName = GHIElectronics.TinyCLR.Pins.SC20100.SpiBus.Spi3;
            networkCommunicationInterfaceSettings.GpioApiName = GHIElectronics.TinyCLR.Pins.SC20100.GpioPin.Id;

            networkCommunicationInterfaceSettings.SpiSettings = settings;
            networkCommunicationInterfaceSettings.InterruptPin = GHIElectronics.TinyCLR.Devices.Gpio.GpioController.GetDefault().OpenPin
                                                                (GHIElectronics.TinyCLR.Pins.SC20100.GpioPin.PC5);
            networkCommunicationInterfaceSettings.InterruptEdge = GpioPinEdge.FallingEdge;
            networkCommunicationInterfaceSettings.InterruptDriveMode = GpioPinDriveMode.InputPullUp;

            networkCommunicationInterfaceSettings.ResetPin = GHIElectronics.TinyCLR.Devices.Gpio.GpioController.GetDefault().OpenPin
                                                            (GHIElectronics.TinyCLR.Pins.SC20100.GpioPin.PD4);
            networkCommunicationInterfaceSettings.ResetActiveState = GpioPinValue.Low;

            //networkInterfaceSetting.Address = new System.Net.IPAddress(new byte[] { 10, 1, 1, 1 });
            networkInterfaceSetting.Address = new System.Net.IPAddress(new byte[] { 192, 168, 1, 32 });

            networkInterfaceSetting.SubnetMask = new System.Net.IPAddress(new byte[] { 255, 255, 255, 0 });
            // networkInterfaceSetting.GatewayAddress = new System.Net.IPAddress(new byte[] { 10, 1, 1, 1 });
            // networkInterfaceSetting.DnsAddresses = new System.Net.IPAddress[] { new System.Net.IPAddress(new byte[]
            // { 75, 75, 75, 75 }), new System.Net.IPAddress(new byte[] { 75, 75, 75, 76 }) };

            networkInterfaceSetting.MacAddress = new byte[] { 0xEC, 0xAD, 0xB8, 0x74, 0xF1, 0x7C };
            networkInterfaceSetting.IsDhcpEnabled = false;
            networkInterfaceSetting.IsDynamicDnsEnabled = false;

            networkInterfaceSetting.TlsEntropy = new byte[] { 0, 1, 2, 3 };

            networkController.SetInterfaceSettings(networkInterfaceSetting);
            networkController.SetCommunicationInterfaceSettings(networkCommunicationInterfaceSettings);

            networkController.SetAsDefaultController();

            networkController.NetworkAddressChanged += NetworkController_NetworkAddressChanged;
            networkController.NetworkLinkConnectedChanged += NetworkController_NetworkLinkConnectedChanged;

            networkController.Enable();
            System.Diagnostics.Debug.WriteLine("Network is ready to use:   ");

            Thread.Sleep(100);

        }
        #endregion

        private static void NetworkController_NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs e)
        {
            Debug.WriteLine("connection changed");

            // Raise event connect/disconnect
        }

        private static void NetworkController_NetworkAddressChanged(NetworkController sender, NetworkAddressChangedEventArgs e)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();

            // linkReady = address[0] != 0;
        }
    }


}
