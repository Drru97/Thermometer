using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Thermometer.Annotations;

namespace Thermometer
{
    public class ThermometerViewModel : INotifyPropertyChanged
    {
        private string _selectedPort;
        private DataReader _reader;
        private bool _connected;

        private float _min;
        private float _max;
        private float _avg;
        private float _current;

        private Task _readDataTask;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _token;

        public List<string> Ports { get; set; }
        public MeasurementRepository Repository { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public List<KeyValuePair<int, int>> Data { get; set; } = new List<KeyValuePair<int, int>>();

        public List<int> Ints { get; set; } = new List<int> { 3, 6, 2, 5, 5 };
        public List<int> Ints2 { get; set; } = new List<int> { 3, 7, 2, 1, 6 };

        private RelayCommand _connectCommand;
        private RelayCommand _disconnectCommand;
        private RelayCommand _startCommand;
        private RelayCommand _stopCommand;

        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand ??
                       (_connectCommand = new RelayCommand(x =>
                       {
                           _reader = new DataReader(SelectedPort);
                           _reader.Connect();
                           Connected = true;
                       }, x => !Connected));
            }
        }

        public RelayCommand DisconnectCommand
        {
            get
            {
                return _disconnectCommand ??
                       (_disconnectCommand = new RelayCommand(x =>
                       {
                           _tokenSource.Cancel();
                           _reader?.Disconnect();
                           Connected = false;
                       }, x => Connected));
            }
        }

        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand ??
                       (_startCommand = new RelayCommand(x =>
                       {
                           _readDataTask = new Task(() =>
                           {
                               while (true)
                               {
                                   if (_token.IsCancellationRequested)
                                   {
                                       try
                                       {
                                           _token.ThrowIfCancellationRequested();
                                       }
                                       catch
                                       {
                                           break;
                                       }
                                   }
                                   var temp = _reader.ReadValue();
                                   var measurement = new Measurement
                                   {
                                       DateAndTime = DateTime.Now,
                                       Temperature = temp
                                   };
                                   Repository.AddMeasurement(measurement);
                                   Minimum = Repository.Minimun;
                                   Maximum = Repository.Maximun;
                                   Average = Repository.Average;
                                   Current = temp;
                               }
                           }, _tokenSource.Token);
                           if (_readDataTask.Status == TaskStatus.Created)
                               _readDataTask.Start();
                           else if (_readDataTask.Status == TaskStatus.Canceled)
                               _readDataTask.ContinueWith(task => ReadData(), TaskContinuationOptions.OnlyOnCanceled);
                       }, x => Connected && _readDataTask?.Status != TaskStatus.Running));
            }
        }

        private void ReadData()
        {
            while (true)
            {
                var temp = _reader.ReadValue();
                var measurement = new Measurement
                {
                    DateAndTime = DateTime.Now,
                    Temperature = temp
                };
                Repository.AddMeasurement(measurement);
                Minimum = Repository.Minimun;
                Maximum = Repository.Maximun;
                Average = Repository.Average;
                Current = temp;
            }
        }

        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand ??
                       (_stopCommand = new RelayCommand(x =>
                       {
                           _tokenSource.Cancel();
                       }));
            }
        }

        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _reader?.Disconnect();
                Connected = false;
                _selectedPort = value;
                OnPropertyChanged(nameof(SelectedPort));
            }
        }

        public float Minimum
        {
            get { return _min; }
            set
            {
                _min = value;
                OnPropertyChanged(nameof(Minimum));
            }
        }

        public float Maximum
        {
            get { return _max; }
            set
            {
                _max = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }

        public float Average
        {
            get { return _avg; }
            set
            {
                _avg = value;
                OnPropertyChanged(nameof(Average));
            }
        }

        public float Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged(nameof(Current));
            }
        }

        private bool Connected
        {
            get
            {
                _connected = _reader != null && _reader.IsConnected;
                return _connected;
            }
            set { _connected = value; }
        }

        public ThermometerViewModel()
        {
            var task = new Task<List<string>>(() => Ports = DataReader.GetAvailablePorts().ToList());
            task.Start();
            SelectedPort = task.Result.FirstOrDefault();
            Repository = new MeasurementRepository();
            _token = _tokenSource.Token;


            Data.Add(new KeyValuePair<int, int>(2, 5));
            Data.Add(new KeyValuePair<int, int>(6, 44));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
