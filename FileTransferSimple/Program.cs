using Globals.NET.RabbitMQ;
using System;
using System.IO;

namespace FileTransferSimple
{
    public class FileData
    {
        public byte[] data;
        public string filename;
        public string Name;
    }

    class Program
    {
        // If you try this at home: please make sure these directories exist,
        // or rename the strings below.
        static readonly string ToSend = @"D:\ToSend\";
        static readonly string ToReceive = @"D:\ToReceive\";
        static readonly string Name = Environment.MachineName;

        // Reads contents of file 'filename' and puts it in a byte array
        private static byte[] ReadFile(string filename)
        {
            var sr = new StreamReader(filename);

            byte[] result;
            using (var streamReader = new MemoryStream())
            {
                sr.BaseStream.CopyTo(streamReader);
                result = streamReader.ToArray();
            }
            sr.Close();
            return result;
        }

        static void Main()
        {
            // Declare the Global
            using (Global<FileData> FileTransfer = new Global<FileData>("FileTransfer", handler: FileTransfer_DataChanged))
            {
                // Send all the files present in the D:\ToSend directory
                if (Directory.Exists(ToSend))
                {
                    foreach (string file in Directory.GetFiles(ToSend))
                    {
                        Console.WriteLine("Sending " + file);

                        // Fill the Global
                        FileData fd = new FileData()
                        {
                            filename = Path.GetFileName(file), // removing the path
                            Name = Name,                       // computer name
                            data = ReadFile(file)              // the file data
                        };

                        // And send it!
                        FileTransfer.Value = fd;
                    }
                }
                else
                {
                    Console.WriteLine("No files to send!");
                }

                Console.ReadLine();
            }
        }

        private static void FileTransfer_DataChanged(object sender, GlobalEventData<FileData> e)
        {
            if (e.isDefault)
            {
                // In that case: no other Globals yet, object gets the default value (is this case: null)
                return;
            }

            if (e.fromSelf)
            {
                Console.WriteLine(e.Data.filename + " sent!");
            }
            else
            {
                Console.WriteLine("File " + e.Data.filename + " received from " + e.Data.Name);

                // Create dest dir, if non existent
                if (!Directory.Exists(ToReceive))
                {
                    Directory.CreateDirectory(ToReceive);
                }

                // Write file
                string destFile = (Path.Combine(ToReceive, e.Data.filename));
                using (var fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(e.Data.data, 0, e.Data.data.Length);
                }

                Console.WriteLine("File " + e.Data.filename + " written to " + destFile);
            }
        }
    }
}
