using LF.Toolkit.Data;
using LF.Toolkit.Test.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = StorageProvider<TestStorage>.Factory.Get();
            Console.WriteLine(i);

            Console.ReadKey();
        }
    }
}
