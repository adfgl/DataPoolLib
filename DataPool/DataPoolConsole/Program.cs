using DataPoolLib;

namespace DataPoolConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var obj = new Obj<int>()
            {
                Name = "John",
                Age = 30,
                IsAlive = true,
                Matrix = new int[][]
                {
                    [ 1, 2, 3 ],
                    [ 4, 5, 6 ],
                    [ 7, 8, 9 ]
                },
            };
            var s = DataPoolSerializer.Serialize(obj, true);
            var json = System.Text.Json.JsonSerializer.Serialize(obj);

            Console.WriteLine(s.Length + "/" + json.Length);

            var obj2 = DataPoolSerializer.Deserialize<Obj<int>>(s);
        }

        [DataPoolObject("1.0.0")]
        public class Obj<T>
        {
            [DataPoolProperty(0)]
            public string Name { get; set;}

            [DataPoolProperty(3)]
            public int Age { get; set; }

            [DataPoolProperty(1)]
            public bool IsAlive { get; set; }

            [DataPoolProperty(2)]
            public T[][] Matrix { get; set; }
        }

    }
}
