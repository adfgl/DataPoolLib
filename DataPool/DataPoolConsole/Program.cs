using DataPoolLib;

namespace DataPoolConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in PropertyLoader.LoadProperties(typeof(Obj)))
            {
                Console.WriteLine(item.Name);
            }
        }

        public class Obj
        {
            [DataPoolProperty(0)]
            public string Name { get; set;}

            [DataPoolProperty(3)]
            public int Age { get; set; }

            [DataPoolProperty(1)]
            public bool IsAlive { get; set; }
        }
    }
}
