using System;
using Globals.NET.RabbitMQ;  // the package

namespace Sender
{
    class Program
    {
        static void Main()
        {
            using var SomeText = new Global<string>("SomeText");

            // assign a value, and we are done!
            SomeText.Value = "Hello, World!";     

            Console.ReadLine();
            Console.WriteLine("Stopping...");
        }
    }
}