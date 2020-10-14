using System;
using System.Threading.Tasks;

namespace BabylonTools.XMap
{
    class Program
    {
        static void Main(string[] args)
        {
            MainOperation();
        }

        static void MainOperation()
        {
            var xmap = new XMap();
            xmap.Start();

            Console.WriteLine("Listening on Xbox 360 controller input...");
            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey().KeyChar != 'q') ;

            xmap.Stop();
        }
    }
}
