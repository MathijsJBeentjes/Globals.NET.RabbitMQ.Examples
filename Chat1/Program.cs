using System;
using Globals.NET.RabbitMQ;

namespace Chat1
{
    class Program
    {
        private static string _name;

        static void Main()
        {
            Console.Write("Your Name: ");
            _name = Console.ReadLine();

            using (var MyText = new Global<string>("MyText", handler: MyText_DataChanged))
            {
                MyText.Value = _name + " has entered the building";

                string txt;
                do
                {
                    txt = Console.ReadLine();

                    // Send text
                    if (txt == "")
                    {
                        MyText.Value = _name + " has left the building";
                    }
                    else
                    {
                        MyText.Value = _name + " says: " + txt;
                    }
                }
                while (!string.IsNullOrEmpty(txt));

            }
        }

        private static void MyText_DataChanged(object sender, GlobalEventData<string> e)
        {
            if (!e.isInitialValue && !e.fromSelf)
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}
