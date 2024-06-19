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
}
