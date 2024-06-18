using DataPoolLib;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataPoolConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var obj = new Obj<int>()
            {
                Name = "Jdasd ",
                Age = 30,
                IsAliveASdasdads = true,
                Matrix = new int[][]
                {
                    [ 1, 2, 3 ],
                    [ 4, 5, 6 ],
                    [ 7, 8, 9 ]
                },

                V = [ "a", "b", "c"]
            };
            var s = DataPoolSerializer.Serialize(obj, true);
            var json = System.Text.Json.JsonSerializer.Serialize(obj);

            Console.WriteLine(s.Length + "/" + json.Length);

            var obj2 = DataPoolSerializer.Deserialize<Obj<int>>(s);

            Console.WriteLine(short.MinValue);
            Console.WriteLine(int.MinValue);
        }

        [DataPoolObject("1.0.0", true)]
        public class Obj<T>
        {
            [DataPoolProperty(0)]
            public string Name { get; set;}

            [DataPoolProperty(3)]
            public int Age { get; set; }

            [DataPoolProperty(1)]
            public bool IsAliveASdasdads { get; set; }

            [DataPoolProperty(2, true)]
            public T[][] Matrix { get; set; }

            [DataPoolProperty(4)]
            public string[] V { get; set; }
        }

    }

}
