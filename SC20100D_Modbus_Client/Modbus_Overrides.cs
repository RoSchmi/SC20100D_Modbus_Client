 
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Modbus;
using GHIElectronics.TinyCLR.Devices.Modbus.Interface;

// implement slave device
//public class MyModbusDevice : ModbusDevice, IModbusInterface
public class MyModbusDevice : ModbusDevice

{
    static ushort ctr = 0;
    public MyModbusDevice(byte deviceAddress, object syncObject = null)
       : base(deviceAddress, syncObject)
    { }

    public MyModbusDevice(IModbusInterface intf, byte deviceAddress, object syncObject = null)
       : base(intf, deviceAddress, syncObject)
    { }

    protected override string OnGetDeviceIdentification(ModbusObjectId objectId)
    {
        switch (objectId)
        {
            case ModbusObjectId.VendorName:
                return "Vendor Name";
            case ModbusObjectId.ProductCode:
                return "101";
            case ModbusObjectId.MajorMinorRevision:
                return "1.0";
            case ModbusObjectId.VendorUrl:
                return "www.test.com.au";
            case ModbusObjectId.ProductName:
                return "Test Modbus";
            case ModbusObjectId.ModelName:
                return "1";
            case ModbusObjectId.UserApplicationName:
                return "SC100";
        }
        return null;
    }

    protected override ModbusConformityLevel GetConformityLevel()
    {
        return ModbusConformityLevel.Regular;
    }
 
    protected override ModbusErrorCode OnReadHoldingRegisters(bool isBroadcast, ushort startAddress, ushort[] registers)
    {       
        
        for (int n = 0; n < registers.Length; ++n)
        {
            // just set a number in each register to test
            registers[n] = ctr;       
        }
        ctr++;

           //Debug.WriteLine("Read Response Fired");
       
        return ModbusErrorCode.NoError;
       
    }

    // override any On<ModusFunction> methods here
}