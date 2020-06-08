using System;
using Globals.NET.RabbitMQ;

namespace GuaranteedDeliverySender
{
    class Program
    {
        public class Envellope
        {
            public Guid ID;
            public string Data;
        }

        static Guid messageId = Guid.NewGuid();
        static string data = "Hello there! I sent you a message!";

        static void Main(string[] args)
        {
            using (Global<Guid> Id = new Global<Guid>("IdCheck", handler: Id_DataChanged))
            using (Global<Envellope> Message = new Global<Envellope>("message"))
            {
                Message.Value = new Envellope() { ID = messageId, Data = data };
                Console.WriteLine("Just sent my message!");
                Console.ReadLine();
            }
        }

        private static void Id_DataChanged(object sender, GlobalEventData<Guid> e)
        {
            if (!e.isInitialValue && e.Data == messageId)
            {
                Console.WriteLine("Message delivery is confirmed!");
            }
        }
    }
}

