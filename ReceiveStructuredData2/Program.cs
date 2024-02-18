using System;
using Globals.NET.RabbitMQ;

namespace ReceiveStructuredData
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
        private static GlobalReader<PlantData> gPlantData;
        static void Main()
        {
            Guid myId = Guid.NewGuid();

            using (gPlantData = new GlobalReader<PlantData>(myId.ToString(), handler: GPlantData_DataChanged))
            using (Global<Guid> MyId = new Global<Guid>("MyId"))
            {
                MyId.Value = myId;
                Console.ReadLine();
            }
        }

        private static void GPlantData_DataChanged(object sender, GlobalEventData<PlantData> e)
        {
            if (!e.isInitialValue)
            {
                Console.WriteLine("Received: " + e.Data);
            }
        }
    }
}
