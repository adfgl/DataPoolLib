using DataPoolLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataPoolConsole.Program;

namespace DataPoolConsole
{
    [DataPoolObject(1,0)]
    public class ObjContainer
    {
        [DataPoolProperty(0)]
        public Obj[] Objects { get; set; }
    }

    [DataPoolObject(1, 0)]
    public class Obj
    {
        [DataPoolProperty(1)]
        public bool IsAlive { get; set; }

        [DataPoolProperty(0)]
        public string Name { get; set; }

        [DataPoolProperty(3)]
        public int Age { get; set; }

        [DataPoolProperty(2)]
        public double[][] Matrix { get; set; }

        [DataPoolProperty(6)]
        public int[][] Matrix1 { get; set; }

        [DataPoolProperty(4)]
        public int[] Values { get; set; }
    }
}
