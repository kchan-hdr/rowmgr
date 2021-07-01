using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHillsDocumentToSharePoint
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = new Mover();
            h.Copy();

            Console.WriteLine("That's all folks");
            Console.ReadKey();
        }
    }
}
