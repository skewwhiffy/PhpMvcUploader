using System;

namespace PhpMvcUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Called with the following args.");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
        }
    }
}
