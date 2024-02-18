using System;
using Globals.NET.RabbitMQ;

namespace GuaranteedDeliveryReceiver
{
    class Program
    {
        public class Envellope
        {
            public Guid ID;
            public string Data;
        }

        static void Main(string[] args)
        {
            using (Global<Envellope> Message = new Global<Envellope>("message", handler: Message_DataChanged))
            {
                // Waiting for the message until enter is h
                Console.ReadLine();
            }
        }

        private static void Message_DataChanged(object sender, GlobalEventData<Envellope> e)
        {
            if (!e.isDefault)
            {
                // Received!
                Console.WriteLine(e.Data.Data);

                // Confirming...
                using (var Id = new Global<Guid>("IdCheck"))
                {
                    Id.Value = e.Data.ID;
                };
            }
        }
    }
}
