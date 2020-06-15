using System;
using Globals.NET.RabbitMQ;  // the package

namespace Receiver
{
    class Program
    {
        static void Main()
        {
            // A declaration plus event handler
            using var SomeText = new Global<string>("SomeText", handler: SomeText_DataChanged);

            Console.ReadLine();
            Console.WriteLine("Stopping...");
        }

        private static void SomeText_DataChanged(object sender, GlobalEventData<string> e)
        {
            // The data is received here!
            Console.WriteLine(e.Data);
        }
    }    
}
