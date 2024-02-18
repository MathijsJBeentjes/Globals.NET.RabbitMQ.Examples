using System;
using Globals.NET.RabbitMQ;

namespace ReceiveStructuredData0
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
            // Define the PlantData global 
            using (gPlantData = new Global<PlantData>("PlantData", handler: GPlantData_DataChanged))
            {
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
