using System;
using Globals.NET.RabbitMQ;

namespace Receiver
{
    class Program
    {
        static void Main()
        {


    Global<string> SomeText = new Global<string>("SomeText");
    Console.WriteLine(SomeText.WaitForRealValue());


            using (Global<string> AddressMJB = new Global<string>("AddressMJB"))
            {
                Console.WriteLine(AddressMJB.WaitForRealValue());
                Console.ReadLine();
                Console.WriteLine("Stopping...");
            }
        }
    }
}
