using System;
using System.IO.Ports;
using System.Windows;

namespace Thermometer
{
    public class DataReader
    {
        private readonly SerialPort _serialPort;
        public bool IsConnected => _serialPort.IsOpen;

        public DataReader(string portName)
        {
            _serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = 9600,
                DtrEnable = true,
                RtsEnable = true
            };
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Connect()
        {
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot connect", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot disconnect", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public float ReadValue()
        {
            var readData = _serialPort.ReadLine().Replace('.', ',');
            var success = float.TryParse(readData, out float value);

            return success ? value : 0;
        }
    }
}
