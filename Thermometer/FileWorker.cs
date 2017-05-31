using System.IO;
using System.Text;

namespace Thermometer
{
    public static class FileWorker
    {
        private static string _filename = "temperature.txt";

        public static void Write(Measurement measurement)
        {
            using (var writer = new StreamWriter(_filename, true, Encoding.UTF8))
            {
                writer.WriteLine($"{measurement.Temperature}\t{measurement.DateAndTime:HH:mm:ss}");
            }
        }
    }
}
