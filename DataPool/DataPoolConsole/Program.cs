using BenchmarkDotNet.Running;
using DataPoolLib;
using System.IO.Compression;
using System.Text;

namespace DataPoolConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ////BenchmarkRunner.Run<DeserializationBenchmark>();

            //var container = RandomObjectGenerator.GenerateRandomObjects(100);

            var actor = new Actor()
            {
                Name = "Ryan",
                Surname = "Gosling",
                Age = 40,
                IsAlive = true,
                Budget = [0, 244, -333.3, 100000.4, 0, -10, 10000000.2132313, 123123123, -123123.12312]
            };

            var s = DataPoolSerializer.Serialize(actor, false);
            var ss = DataPoolSerializer.Serialize(actor, true);
            var json = System.Text.Json.JsonSerializer.Serialize(actor);
            var d = DataPoolSerializer.Deserialize<Actor>(s);

            Console.WriteLine("No downgrade DATAPOOL/JSON: " + s.Length + "/" + json.Length + " Ratio: " + (Math.Round((double)json.Length / (double)s.Length, 2)));
            Console.WriteLine("With downgrade DATAPOOL/JSON: " + ss.Length + "/" + json.Length + " Ratio: " + (Math.Round((double)json.Length / (double)ss.Length, 2)));


        }

        public static byte[] Compress(string input)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    using (var writer = new StreamWriter(gzipStream, Encoding.UTF8))
                    {
                        writer.Write(input);
                    }
                }
                return memoryStream.ToArray();
            }
        }

        [DataPoolObject(2, 0)]
        public class Actor
        {
            [DataPoolProperty(0)]
            public string Name { get; set; }

            [DataPoolProperty(1)]
            public string Surname { get; set; }

            [DataPoolProperty(2)]
            public int Age { get; set; }

            [DataPoolProperty(3)]
            public bool IsAlive { get; set; }

            [DataPoolProperty(4)]
            public double[] Budget { get; set; }
        }
    }

}
