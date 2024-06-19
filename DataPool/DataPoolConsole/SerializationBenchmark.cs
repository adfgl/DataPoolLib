using BenchmarkDotNet.Attributes;

namespace DataPoolConsole
{
    public class SerializationBenchmark
    {
        static ObjContainer s_container = RandomObjectGenerator.GenerateRandomObjects(100);

        [Benchmark]
        public string JsonApproach()
        {
            string json = System.Text.Json.JsonSerializer.Serialize(s_container);
            return json;
        }

        [Benchmark]
        public byte[] DataPoolApproach()
        {
            byte[] dp = DataPoolLib.DataPoolSerializer.Serialize(s_container, true);
            return dp;
        }
    }
}
