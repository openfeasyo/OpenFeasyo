using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenFeasyo.Platform.Controls;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;

namespace TrignoAvantiCustom;

public class TrignoAvanti : IDiscoverable, OpenFeasyo.Platform.Controls.Drivers.IDevice
{
    
   public class BufferAssembler
{
    private const int MaxSize = 128;
    private readonly byte[][] _buffers;
    private int _bufferIndex;

    private byte[]? _currentBuffer;
    private int _expectedSize;
    private int _offset;

    private int? _lastPacketId; // for sequence check
    public int LastPacketId => _lastPacketId ?? 0;
    
    /// <summary>
    /// Largely chatGPT generated code to assemble the data packets comming from Bluetooth and putting them together
    /// into Delsys messages.
    /// Known issues: When two sensors are connected, there is a frequent data loss.
    /// TODO: Check the performance of this code. Possible speed to be gained (possibly reducing data loss).
    /// </summary>
    public BufferAssembler(int bufferCount = 3)
    {
        _buffers = new byte[bufferCount][];
        for (int i = 0; i < _buffers.Length; i++)
            _buffers[i] = new byte[MaxSize];
    }

    /// <summary>
    /// Add a chunk of bytes. Returns completed, valid packets (may be zero, one, or more).
    /// </summary>
    public IEnumerable<byte[]> AddChunk(byte[] chunk)
    {
        int index = 0;

        while (index < chunk.Length)
        {
            // Start of a new packet
            if (_expectedSize == 0)
            {
                if (index >= chunk.Length)
                    break;

                int size = chunk[index];
                if (size != 59) //size <= 0 || size > MaxSize)
                {
                    Console.WriteLine($"⚠ Invalid packet size {size}, skipping byte.");
                    index++;
                    continue; // try next byte to resync
                }

                // Ensure we have at least first 4 bytes (size + ID + marker)
                if (chunk.Length - index < 4)
                {
                    // not enough bytes to validate header yet — wait for next chunk
                    _expectedSize = size;
                    _currentBuffer = _buffers[_bufferIndex];
                    _bufferIndex = (_bufferIndex + 1) % _buffers.Length;
                    _offset = 0;
                }
                else
                {
                    // Validate immediately
                    int packetId = (chunk[index + 1] << 8) | chunk[index + 2];
                    byte marker = chunk[index + 3];

                    if (marker != 49)
                    {
                        Console.WriteLine($"⚠ Invalid marker {marker}, skipping potential packet.");
                        index++;
                        continue; // resync attempt
                    }

                    if (_lastPacketId.HasValue && packetId != _lastPacketId.Value + 1)
                    {
                        Console.WriteLine($"⚠ Packet drop? Expected {_lastPacketId.Value + 1}, got {packetId}");
                    }

                    _lastPacketId = packetId;

                    // prepare buffer
                    _expectedSize = size;
                    _currentBuffer = _buffers[_bufferIndex];
                    _bufferIndex = (_bufferIndex + 1) % _buffers.Length;
                    _offset = 0;
                }

                // Copy size byte
                _currentBuffer![_offset++] = (byte)size;
                index++;
                continue;
            }

            // Copy payload
            int remaining = _expectedSize - _offset;
            int available = chunk.Length - index;
            int toCopy = (remaining < available) ? remaining : available;

            Buffer.BlockCopy(chunk, index, _currentBuffer!, _offset, toCopy);
            _offset += toCopy;
            index += toCopy;

            // Completed packet
            if (_offset >= _expectedSize)
            {
                if (ValidateChecksum(_currentBuffer!, _expectedSize))
                {
                    yield return _currentBuffer!;
                }
                else
                {
                    Console.WriteLine("⚠ Invalid checksum, discarding packet.");
                }
                
                _expectedSize = 0;
                _offset = 0;
                _currentBuffer = null;
            }
        }
    }
    
    
    private bool ValidateChecksum(byte[] buffer, int length)
    {
        if (length < 2) return false; // must have at least 1 byte data + checksum

        byte expected = buffer[length - 1];
        byte calculated = CalculateChecksum(buffer, length - 1);
        return expected == calculated;
    }

    private static byte CalculateChecksum(byte[] data, int numBytes)
    {
        uint checksum = 0;

        for (int i = 0; i < numBytes; i++)
            checksum += data[i];

        checksum = (checksum >> 8) + (checksum & 0xFF);
        checksum = (checksum >> 8) + (checksum & 0xFF);
        checksum = (checksum >> 8) + (checksum & 0xFF);

        checksum ^= 0xFF;
        return (byte)(checksum & 0xFF);
    }
}
    
    
    private readonly Guid _uuidBattery = Guid.Parse("e736de0C-c1d3-4b97-9d56-b730260d7517");
    private readonly Guid _uuidDeviceMode = Guid.Parse("e736de02-c1d3-4b97-9d56-b730260d7517");
    private readonly Guid _uuidConfigSelection = Guid.Parse("e736de20-c1d3-4b97-9d56-b730260d7517");
    private readonly Guid _uuidAnalogPowerStatus = Guid.Parse("e736de0A-c1d3-4b97-9d56-b730260d7517");
    private readonly Guid _uuidSensorData = Guid.Parse("e736de01-c1d3-4b97-9d56-b730260d7517");

    private IEmgSensorInput _gamingInput;
    private IAdapter _adapter;
    private List<Plugin.BLE.Abstractions.Contracts.IDevice> _foundDevices = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();
    private List<Sensor> _connectedSensors = new List<Sensor>();
    private List<Plugin.BLE.Abstractions.Contracts.IDevice> _devicesToStart = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();
    public TrignoAvanti()
    {
        _adapter = CrossBluetoothLE.Current.Adapter;
        _adapter.DeviceConnected += async (s, a) =>
        {
            if (a.Device.Name != null && a.Device.Name.Length > 4)
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(500));
                // Read the information from sensor
                Console.WriteLine("CONNECTED TO " + a.Device.Name);

                _devicesToStart.Add(a.Device);
                
            }
        };
        _adapter.DeviceConnectionLost += (s, a) =>
        {
            if (a.Device.Name != null && a.Device.Name.Length > 4)
                OnConnectionFailed(new ConnectionEventArgs(a.Device.Name,"ConnectionLost: " + a.ErrorMessage));
        };
        _adapter.DeviceConnectionError += (s, a) =>
        {
            if (a.Device.Name != null && a.Device.Name.Length > 4)
                OnConnectionFailed(new ConnectionEventArgs(a.Device.Name,"ConnectionError: " + a.ErrorMessage));

        };
        _adapter.DeviceDisconnected += (s, a) =>
        {
            if (a.Device.Name != null && a.Device.Name.Length > 4)
                OnConnectionFailed(new ConnectionEventArgs(a.Device.Name,"NoError: Just disconnected."));

        };
    }
        

    #region IDevice
    
    #region Properties

    /// <summary>
    /// Name of the driver. </summary>
    public string Name {
        get { return "Trigno Avanti"; }
    }

    /// <summary>
    /// Vendor of the device. </summary>
    public string Vendor 
    {
        get {  return "Delsys"; }
    }

    /// <summary>
    /// Description of the driver. </summary>
    public string Description
    {
        get {  return "Bluetooth sEMG sensor"; }
    }

    /// <summary>
    /// Flag that signals whether the driver is loaded. </summary>
    public bool IsLoaded 
    { 
        get  { return _gamingInput != null; }
    }

    /// <summary>
    /// Flag that signals whether the driver is loaded. </summary>
    public OpenFeasyo.Platform.Controls.Drivers.DeviceType DeviceType 
    { 
        get { return OpenFeasyo.Platform.Controls.Drivers.DeviceType.Electromyography; }
    }

    /// <summary>
    /// GamingInput property. </summary>
    /// <exception cref="Vub.Etro.Gaming.GamingControls.Drivers.UninitializedDriverException" >
    /// Throws an exception if the driver was not previously loaded. </exception>
    public IGamingInput GamingInput
    {
        get { return _gamingInput; }
    }

    #endregion

    #region Methods for initialization and deinitialization

    public void LoadDriver(Dictionary<string, string> parameters)
    {
        _gamingInput = new TrignoEmgInput(this);
    }

    
    public void UnloadDriver()
    {
        
    }
    #endregion Methods for initialization and deinitialization
    
    #endregion IDevice
    
    #region IDiscoverable implementation

    public void ScanAsync()
    {
        //var state = ble.State;
        Plugin.BLE.Abstractions.Contracts.IDevice? foundDevice = null;
        _adapter.ScanTimeout = 5000;
        _foundDevices.Clear();
        _adapter.DeviceDiscovered += async (s, a) =>
        { 
            if (a.Device.Name != null && a.Device.Name.Contains("Trigno Avanti") && !_foundDevices.Contains(a.Device))
            {
                
                _foundDevices.Add((a.Device));
            }
        };
        _adapter.ScanTimeoutElapsed += async (s, a) =>
        {
            List<string> names = _foundDevices
                .Where(d => !string.IsNullOrEmpty(d.Name) && d.Name.Length >= 4)
                .Select(d => d.Name[^4..])   // take last 4 chars - code of the sensor
                .ToList();
            OnScanFinished(new ScanResultsEventArgs(names));
        };
        _adapter.StartScanningForDevicesAsync(); 
        
    }

    public event EventHandler<ScanResultsEventArgs> ScanFinished;

    public void OnScanFinished(ScanResultsEventArgs e)
    {
        ScanFinished?.Invoke(this, e);
    }

    public async void ConnectAsync(string strDevices)
    {
        string[] devices = strDevices.Split(';');
        for (int i = 0; i < devices.Length; i++)
        {
            var strDev = devices[i];
                Plugin.BLE.Abstractions.Contracts.IDevice? device = _foundDevices
                .FirstOrDefault(d => !string.IsNullOrEmpty(d.Name) && d.Name.EndsWith(strDev));
            if (device == null) throw new ArgumentNullException(nameof(device));
            
            var parameters = new ConnectParameters(
                autoConnect: false,
                forceBleTransport: true
            );
            await _adapter.ConnectToDeviceAsync(device,parameters);
            await device.RequestMtuAsync(247);
            
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            
            var sensor = await ReadCharacteristics(device, i);
            Console.WriteLine("STARTING stream to  " + sensor.Name);
            _connectedSensors.Add(sensor);
            OnConnectionEstablished(
                new ConnectionEventArgs(device.Name.Substring(device.Name.Length - 4), sensor.ToString(),sensor));
            await Task.Delay(TimeSpan.FromMilliseconds(500));

        }
    }

    public event EventHandler<ConnectionEventArgs> ConnectionEstablished;

    public void OnConnectionEstablished(ConnectionEventArgs e)
    {
        ConnectionEstablished?.Invoke(this, e);
    }

    public event EventHandler<ConnectionEventArgs> ConnectionFailed;

    public void OnConnectionFailed(ConnectionEventArgs e)
    {
        ConnectionFailed?.Invoke(this, e);
    }

    #endregion
    
    #region Sensor connection
    
    public async Task<Sensor> ReadCharacteristics(Plugin.BLE.Abstractions.Contracts.IDevice device, int channel)
    {
        Sensor sensor = new Sensor(device.Name,channel);
        var services = await device.GetServicesAsync();

        foreach (var service in services)
            sensor.Services.Add(service.Id);
          
        // Example: read battery level
        var batteryChar = await FindCharacteristicAsync(_uuidBattery,device);
        if (batteryChar != null)
        {
            var data = await batteryChar.ReadAsync();
            Console.WriteLine($"DEV:  Battery level: {data.data[0]}%");
            sensor.BatteryStatus = data.data;
            
        }

        // Select config
        var configChar = await FindCharacteristicAsync(_uuidConfigSelection,device);
        if (configChar != null)
        {
            var original = await configChar.ReadAsync();
            Console.WriteLine($"DEV: Original config: {original.data[0]}");
            sensor.OriginalConfig = original.data;
            
            await configChar.WriteAsync(new byte[] { 0x01 });
            var updated = await configChar.ReadAsync();
            Console.WriteLine($"DEV: Updated config: {updated.data[0]}");
        }

        // Check analog power status
        var powerChar = await FindCharacteristicAsync(_uuidAnalogPowerStatus,device);
        if (powerChar != null)
        {
            var power = await powerChar.ReadAsync();
            Console.WriteLine($"Analog power status: {power.data[0]}");
            sensor.PowerStatus = power.data;
        }

        // Subscribe to EMG + ACC + GYRO notifications
        var dataChar = await FindCharacteristicAsync(_uuidSensorData,device);
        if (dataChar != null)
        {
            sensor.DataStreamer = dataChar;
            dataChar.ValueUpdated += (s, e) => sensor.ParseSensorData(e.Characteristic.Value);
            for(int i = 0; i < 5; i++){
                try
                {
                    await dataChar.StartUpdatesAsync();
                    break;
                }
                catch
                {  
                    Console.WriteLine("DEV ERROR: Unable to start data streaming. This was try n. " +i);
                    await Task.Delay(300);
                }
                
            }
            Console.WriteLine("DEV: Starting updates from :" + sensor.Name);
        }

        // Set device mode = data collection
        var modeChar = await FindCharacteristicAsync(_uuidDeviceMode,device);
        if (modeChar != null)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var mode = await modeChar.ReadAsync();
                    Console.WriteLine($"DEV: Original device mode: {mode.data[0]}");

                    await modeChar.WriteAsync(new byte[] { 0x01 });
                    var newMode = await modeChar.ReadAsync();
                    Console.WriteLine($"DEV: New device mode: {newMode.data[0]}");
                    if (newMode.data[0] == 0x01)
                        break;
                    else
                        Console.WriteLine("DEV ERROR: Unable to set the mode. This was try n. " + i);
                } catch (Exception e)
                {
                    Console.WriteLine("DEV ERROR: " + e.Message);
                    Console.WriteLine("DEV ERROR: Unable to set the mode. This was try n. " + i);
                    await Task.Delay(300);
                }
            }
        }
        return sensor;
    }

   

    
    

  //  private BufferAssembler buffer = new BufferAssembler();
  //  private void ParseSensorData(byte[] data)
  //  {
  //      byte[]? completedData = buffer.AddChunk(data);
  //      if (completedData != null)
  //      {
  //          Console.WriteLine( "Buffer: " + completedData[0] + ", "+ completedData[1] + ", "+ completedData[2] + ", "+ completedData[3] + ", ");

            
  //          _framesReceived++;
  //          // FPS Updates
  //          if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
  //          {
  //              _fps = _framesReceived;
  //              _framesReceived = 0;
  //              _lastTime = DateTime.Now;
  //              Console.WriteLine( "Update freq.: " + _fps + " fps " + data.Length);
  //          }
  //      }
  // }

    private async Task<ICharacteristic?> FindCharacteristicAsync(Guid uuid, Plugin.BLE.Abstractions.Contracts.IDevice? device)
    {
        if (device == null) return null;
        foreach (var service in await device.GetServicesAsync())
        {
            var chars = await service.GetCharacteristicsAsync();
            foreach (var ch in chars)
            {
                if (ch.Id == uuid) return ch;
            }
        }
        return null;
    }
    
    #endregion Sensor connection
    
}