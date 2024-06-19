using DataPoolLib;

namespace DataPoolConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var objs = RandomObjectGenerator.GenerateRandomObjects(100); 
            var container = new ObjContainer() { Objects = objs };


            var o = new Obj()
            {
                Name = "Pavel",
                Age = 10,
                Values = new int[] { 1, 2, 3, 4, 5 },
            };

            var s = DataPoolSerializer.Serialize(container, true);

            var json = System.Text.Json.JsonSerializer.Serialize(container);
            Console.WriteLine(s.Length + "/" + json.Length + " " + ((double)json.Length / (double)s.Length));

            //var d = DataPoolSerializer.Deserialize<ObjContainer>(s);

        }

        [DataPoolObject("1.0.0")]
        public class ObjContainer
        {
            [DataPoolProperty(0)]
            public Obj[] Objects { get; set; }
        }

        [DataPoolObject("1.0.0")]
        public class Obj
        {
            [DataPoolProperty(1)]
            public bool IsAlive { get; set; }

            [DataPoolProperty(0)]
            public string Name { get; set;}

            [DataPoolProperty(3)]
            public int Age { get; set; }

            [DataPoolProperty(2)]
            public double[][] Matrix { get; set; }

            [DataPoolProperty(6)]
            public int[][] Matrix1 { get; set; }

            [DataPoolProperty(4)]
            public int[] Values { get; set; }
        }



        public class RandomObjectGenerator
        {
            private static readonly Random random = new Random();

            public static Obj[] GenerateRandomObjects(int count)
            {
                Obj[] objects = new Obj[count];

                for (int i = 0; i < count; i++)
                {
                    Obj obj = new Obj
                    {
                        Name = GenerateRandomString(),
                        Age = random.Next(1, 100), // Random age between 1 and 100
                        IsAlive = random.Next(2) == 0, // Random boolean value

                        // Generate random matrix of doubles
                        Matrix = GenerateRandomDoubleMatrix(),

                        // Generate random matrix of integers
                        Matrix1 = GenerateRandomIntMatrix(),

                        // Generate random array of integers
                        Values = GenerateRandomIntArray()
                    };

                    objects[i] = obj;
                }

                return objects;
            }

            private static string GenerateRandomString()
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                return new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            private static double[][] GenerateRandomDoubleMatrix()
            {
                const int rows = 50;
                const int cols = 50;
                double[][] matrix = new double[rows][];

                for (int i = 0; i < rows; i++)
                {
                    matrix[i] = new double[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        matrix[i][j] = random.NextDouble() * 100; // Random double value
                    }
                }

                return matrix;
            }

            private static int[][] GenerateRandomIntMatrix()
            {
                const int rows = 20;
                const int cols = 40;
                int[][] matrix = new int[rows][];

                for (int i = 0; i < rows; i++)
                {
                    matrix[i] = new int[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        matrix[i][j] = random.Next(int.MinValue, int.MaxValue); // Random integer value
                    }
                }

                return matrix;
            }

            private static int[] GenerateRandomIntArray()
            {
                const int length = 100;
                int[] array = new int[length];

                for (int i = 0; i < length; i++)
                {
                    array[i] = random.Next(int.MinValue, int.MaxValue); // Random integer value
                }

                return array;
            }
        }

    }

}
