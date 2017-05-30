using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Thermometer
{
    public class MeasurementRepository
    {
        public static ObservableCollection<Measurement> Measurements { get; set; } = new ObservableCollection<Measurement>();

        public float Minimun => (float)Math.Round(Measurements.Min(x => x.Temperature), 2);
        public float Maximun => (float)Math.Round(Measurements.Max(x => x.Temperature), 2);
        public float Average => (float)Math.Round(Measurements.Average(x => x.Temperature), 2);

        public void AddMeasurement(Measurement measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException(nameof(measurement), @"Measurement cannot be null");
            }

            Measurements.Add(measurement);
        }
    }
}
