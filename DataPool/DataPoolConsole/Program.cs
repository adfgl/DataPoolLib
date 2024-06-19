using BenchmarkDotNet.Running;
using DataPoolLib;

namespace DataPoolConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ////BenchmarkRunner.Run<DeserializationBenchmark>();

            var container = RandomObjectGenerator.GenerateRandomObjects(100);

            var o = new Obj()
            {
                Name = "Pavel",
                Age = 10,
                Values = new int[] { 1, 2, 3, 4, 5 },
            };

            var s = DataPoolSerializer.Serialize(o, false);
            var ss = DataPoolSerializer.Serialize(o, true);


            var json = System.Text.Json.JsonSerializer.Serialize(o);


            Console.WriteLine("No downgrade JSON/DATAPOOL: " + s.Length + "/" + json.Length + " Ratio: " + (Math.Round((double)json.Length / (double)s.Length, 2)));
            Console.WriteLine("With downgrade JSON/DATAPOOL: " + ss.Length + "/" + json.Length + " Ratio: " + (Math.Round((double)json.Length / (double)ss.Length, 2)));


        }
    }

}
