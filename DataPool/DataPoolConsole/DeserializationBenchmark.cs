using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPoolConsole
{
    public class DeserializationBenchmark
    {
        static ObjContainer s_container = RandomObjectGenerator.GenerateRandomObjects(100);
        static string json = System.Text.Json.JsonSerializer.Serialize(s_container);
        static byte[] dp = DataPoolLib.DataPoolSerializer.Serialize(s_container, true);

        [Benchmark]
        public ObjContainer JsonApproach()
        {
            ObjContainer objContainer = System.Text.Json.JsonSerializer.Deserialize<ObjContainer>(json);
            return objContainer;
        }

        [Benchmark]
        public ObjContainer DataPoolApproach()
        {
            ObjContainer objContainer = DataPoolLib.DataPoolSerializer.Deserialize<ObjContainer>(dp);
            return objContainer;
        }
    }
}
