using System;
using Globals.NET.RabbitMQ;

namespace Chat2
{
    class Program
    {
        private static string _name;
        static Global<string> MyText;

        static void Main()
        {
            try
            {
                Console.Write("Your Name: ");
                _name = Console.ReadLine();

                using (MyText = new Global<string>("MyText", handler: MyText_DataChanged))
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine();
                Console.WriteLine(e.StackTrace.ToString());
                Console.ReadLine();
            }
        }

        private static void MyText_DataChanged(object sender, GlobalEventData<string> e)
        {
            if (e.isInitialValue || e.fromSelf)
            {
                return;
            }

            Console.WriteLine(e.Data);

            // Automatic Hi! response for new members:
            int index = e.Data.IndexOf(" has entered the building");
            if (index > -1)
            {
                string newName = e.Data.Substring(0, e.Data.Length - " has entered the building".Length);
                if (newName != _name)
                {
                    MyText.Value = _name + " says: Hi " + newName + "!";
                }
            }

            // Automatic Bey! response for left members:
            index = e.Data.IndexOf(" has left the building");
            if (index > -1)
            {
                string newName = e.Data.Substring(0, e.Data.Length - " has left the building".Length);
                if (newName != _name)
                {
                    MyText.Value = _name + " says: Bye " + newName + "!";
                }
            }
        }
    }
}
