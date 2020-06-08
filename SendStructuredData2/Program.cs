using System;
using Globals.NET.RabbitMQ;

namespace SendStructuredData
{
    public class PlantData
    {
        public int ID { get; set; }
        public double LA { get; set; }
        public int Class { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}, Class: {Class}, LA: {LA}";
        }
    }

    class Program
    {
        private static Global<PlantData> gPlantData;

        static void Main()
        {
            // When the receiver is ready to receive, it sends its ID.
            using var MyId = new GlobalReader<Guid>("MyId", handler: MyId_DataChanged);
            // And now sit and wait
            Console.ReadLine();
        }

        private static void MyId_DataChanged(object sender, GlobalEventData<Guid> e)
        {
            if (e.isDefault)
            {
                // Initialized, no other Globals with this name yet
                return;
            }

            using (gPlantData = new Global<PlantData>(e.Data.ToString()))
            {
                // Create a random generator helping to simulate the data
                Random r = new Random();
                for (int i = 0; i < 100; i++)
                {
                    var pd = new PlantData()
                    {
                        ID = i + 1,
                        Class = (int)(r.NextDouble() * 4),
                        LA = r.NextDouble() * 100
                    };

                    Console.WriteLine("Sending: " + pd);

                    // The actual sending
                    gPlantData.Value = pd;
                }
            }
        }
    }
}
