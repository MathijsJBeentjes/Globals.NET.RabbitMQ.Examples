# Globals.NET.RabbitMQ.Examples

## The Base
The Globals Communication Pattern defines named communication objects, enforcing the rule:  

#### If the names (and type) are the same, then the data is the same.  

Assigning a value to a named communication object should be enough to send its value.  
Declaring that same named communication object should be enough to receive its value.

This rule is valid as long as the programs are connected to the same (RabbitMQ) server. 
As long as at least one Global with a certain name is alive, then this named Global has a value that is the same for all the same named globals connected to the framework. Only when the last named Global gets out of scope, the value is gone.  
For more information see [this article](https://www.beentjessoftware.nl/globals-design-pattern). 
To go to the package, click [here](https://github.com/MathijsJBeentjes/Globals.NET.RabbitMQ)  

## Examples using Globals.NET.RabbitMQ:

### Hello World!

[Hello World! Sender](#hello-world-sender)  
[Hello World! Receiver](#hello-world-receiver)  
 
### Chat
[Simplistic Chat Program 1](#simplistic-chat-program-1)  
[Simplistic Chat Program 2](#simplistic-chat-program-2)

### Send / Receive Structured Data
[Send Structured Data 1](#the-sender-1)  
[Receive Structured Data 1](#the-receiver-1)  

[Send Structured Data 2](#the-sender-2)  
[Receive Structured Data 2](#the-receiver-2)  

[Send Structured Data 3](#the-sender-3)  
[Receive Structured Data 3](#the-receiver-3)

### Guaranteed Delivery  

[Guaranteed Delivery Sender](#guaranteed-delivery-sender)  
[Guaranteed Delivery Receiver](#guaranteed-delivery-receiver)  

### Other (undocumented)
[Simple File Transfer](#simple-file-transfer)  

[Vote Server](#vote-server)  
[Vote Client](#vote-client)

## Hello World! Sender

#### Program.cs:
~~~csharp
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
~~~

</h4>
<h4 id="app-config">
App.config:
</h4>

~~~csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="HostName" value="localhost"/>
    <add key="Port" value="5672"/>
    <add key="UserName" value="guest"/>
    <add key="Password" value="guest"/>
    <add key="VirtualHost" value="/"/>
  </appSettings>
</configuration>
~~~
Explanation:

In the using section, the package Globals.NET.RabbitMQ is added, containing the Globals communication object.  

In the Main section, a Global with the name "SomeText" (first parameter in the constructor) is instantiated and the Value "Hello, World!" is assigned to it. 
All other Globals with the same name and connected to the framework will get the value "Hello, World!".  

As Globals are in practice heavy duty objects, they should be disposed by the user. In this example, the C# 8 using statement is implemented, assuring the object is disposed automatically.

In order to make communication via RabbitMQ possible, you should have a RabbitMQ server somewhere running. In addition, 5 RabbitMQ parameters should be set, in this example via the App.Config file. Please note that on a local (default) system, these parameters are ok. In a production environment, these should be adapted according to the [RabbitMQ](https://www.rabbitmq.com/documentation.html) documentation.


## Hello World! Receiver

#### Program.cs:
~~~csharp
using System;
using Globals.NET.RabbitMQ;  // the package

namespace Receiver
{
    class Program
    {
        static void Main()
        {
            // A declaration plus event handler
            using var SomeText = new Global<string>("SomeText", handler: SomeText_DataChanged);

            Console.ReadLine();
            Console.WriteLine("Stopping...");
        }

        private static void SomeText_DataChanged(object sender, GlobalEventData<string> e)
        {
            // The data is received here!
            if (!e.isDefault)
            {
                Console.WriteLine(e.Data);
            }
        }
    }    
}
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:

In the Receiver, a Global is instantiated with the same name, "SomeText", as in the Sender. During instantiation, an event handler is added. On the moment both Sender and Receiver is running, the DataChanged event fires and the value "Hello, World!" is displayed in the console.

In this example, it does not matter which program is started first, the Sender or the Receiver. As long as they are both running at the same time, the "Hello, World!" text is transfered.

In the SomeText_DataChanged handler, you see a check on !e.isDefault. The reason for this check is the fact that on the moment the Global is instantiated and there are no other same-named Globals alive, a first DataChanged event is raised containing a default value (in this case the default value of a string, which is null). In this situation, we don't want to display this data so it is filtered away.


## Simplistic Chat Program 1
In the example below, you can type in your name and after that, you can communicate with everybody running the same chat program as well.

#### Program.cs:
~~~csharp
namespace Chat1
{
    class Program
    {
        private static string _name;

        // The Heavy Duty communication object
        static Global<string> MyText;

        static void Main()
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

        private static void MyText_DataChanged(object sender, GlobalEventData<string> e)
        {
            if (!e.isInitialValue && !e.fromSelf)
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}
~~~

#### App.config:
Same as in the [Sender](#app-config)  

Explanation:  
  
  
In the main section, first your name is asked and stored in a private string. Then the Global<string> MyText is instantiated and a first value is assigned: “\<name> has entered the building”. Every chat program connected to the same world will receive this text via a DataChanged event and will display it in the console. Now a loop is started, reading the text a user types, and assigns it to MyText: “\<name> says: \<the text you just typed>”. If you just hit enter, it is interpreted you are leaving, so a last message will be sent: “\<name>  has left the building” .

## Simplistic Chat Program 2
In the previous example, you could not see who else is connected on the moment you entered the chat room. In the next chat example this problem is solved, by added some automatic greeting behavior:

#### Program.cs:

~~~csharp
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:  
  
In the MyText_DataChanged handler automatic greeting behavior is added, by checking on the presence of the string: " has entered the building". In that situation, an automatic response is created. Suppose four persons are in the chat, and a fifth person enters the room, then it receives 4 greetings from the other members.  
Another automatic response is created on the moment someone leaves the chat.

## Send / Receive structured data
In the next example, a continuous array of simulated plant data is sent over the line. We start with the Sender:

### The Sender (1):

#### Program.cs:

~~~csharp
using System;
using Globals.NET.RabbitMQ;

namespace SendStructuredData0
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
            using (gPlantData = new Global<PlantData>("PlantData"))
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

            Console.ReadLine();
        }
    }
}
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:  

Most of the code is about generating the simulated plant data. There are only three lines that are involved in the communication:  
A using statement importing the Globals class:
~~~csharp
    using Globals.NET.RabbitMQ;
~~~
A declaration of the named communication object:
~~~csharp
    using (gPlantData = new Global<PlantData>("PlantData"))
~~~
And the actual sending:
~~~csharp
    gPlantData.Value = pd;
~~~
### The Receiver (1):

####Program.cs:
~~~csharp
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:  

In Main the only thing that is done is instantiating a "PlantData" Global including an event handler. The Global keeps alive as long as no Enter has been hit. In the mean time, every time a value is assigned to PlantData by the sender, a DataChanged event is raised on the receiving side and the data is displayed in the console.  

In the DataChanged event handler, we see a check on !e.isInitialValue. e.isInitialValue is true in cases that the data was assigned some time ago. So if the receiver is running, and the value of the Global is changed, then e.isInitialValue is false. If a Global is instantiated, receiving the current value of the Global (which might be a default value as well), then e.isInitialValue is true. Because we only want to receive new values we filter the initial values away.

As long as the Receiver is started first, and then the Sender, everything goes fine. After starting the Sender it immediatly start to send an array of plant data to the receiver, which displays its data in the console.

However, if you first start the sender, and the the receiver, nothing seems to happen: The sender sent its data allready over the line before the receiver got into live, so the Global only contained the last value sent. This value was filtered out by the !e.isInitialValue check. So there was nothing to receive. In the next version of the program this issue is solved, as the sender will wait for a start event before it sends its data:


### The Sender (2):

#### Program.cs:

~~~csharp
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
            // Define the PlantData global 
            using (gPlantData = new Global<PlantData>("PlantData"))
            using (var start = new Global<bool>("Start", handler: Start_DataChanged))
            {
                Console.ReadLine();
            }
        }

        private static void Start_DataChanged(object sender, GlobalEventData<bool> e)
        {
            if (e.isDefault)
            {   // initialization completed.
                // No other Globals with the same name yet
                return;
            }

            if (e.Data)
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:

In Main a Global with the name "Start" is declared including a DataChanged event handler. Only when this value is assigned the value true by a Global then the sending of data will take place.

### The Receiver (2):

#### Program.cs:
~~~csharp
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
        private static Global<PlantData> gPlantData;
        static void Main()
        {
            // Define the PlantData global 
            using (gPlantData = new Global<PlantData>("PlantData", handler: GPlantData_DataChanged))
            using (var start = new Global<bool>("Start"))
            {
                start.Value = true;

                Console.ReadLine();
            }
        }

        private static void GPlantData_DataChanged(object sender,   GlobalEventData<PlantData> e)
        {
            if (!e.isInitialValue)
            {
                Console.WriteLine("Received: " + e.Data);
            }
        }
    }
}
~~~
#### App.config:
Same as in the [Sender](#app-config)  

Explanation:

In the Receiver the same "Start" Global is declared, and a value of true is assigned to it. The Sender will now receive a Start-DataChanged event and the Sender will send its array of data.

If the Receiver is started first, it will set the Start Global to true. When the Sender is brought into live, it will receive a DataChanged event on the Start Global and will send its data.

This works well if we have a one-to-one configuration, but suppose we have one sender and multiple receivers.
If we continue with our previous example, and start a second receiver, then the new Receiver assigns true to start, triggering the DataChanged event of the start on the Sender, with the result the Sender sends another bunch of 100 records – to alle receivers that are listening.
What we actually want in a one-to-many configuration, is that every Receiver only gets one bunch of data, and not with every receiver that has come to life. Instead of assigning true to the Global<bool> Start variable, we let the receiver send some unique value to the sender and use that unique value to receive the data privately. This will be worked out in the next section.

One remark about assigning the value of true to the Start Global for the second time: A DataChanged event is raised, while the data isn't actually changed. However, the rule is that if you assign a value to the Value property of a Global, a DataChanged event will be fired. The important reason for this is the fact that if you want to send a continuous array of data over the line, you don't want the DataChanged event to be skipped in situations that the previous value happens to be the same as the next value.

### The Sender (3):

#### Program.cs:

~~~csharp
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
        private static GlobalWriter<PlantData> gPlantData;

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

            using (gPlantData = new GlobalWriter<PlantData>(e.Data.ToString()))
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

### The Receiver (3):

#### Program.cs:

~~~csharp
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

            using (gPlantData = new GlobalReader<PlantData>(myId.ToString(), 
handler: GPlantData_DataChanged))
            using (Global<Guid> MyId = new Global<Guid>("MyId"))
            {
                MyId.Value = myId;
                Console.ReadLine();
            }
        }

  	 private static void GPlantData_DataChanged(object sender, 
    GlobalEventData<PlantData> e)
        {
            if (!e.isInitialValue)
            {
                Console.WriteLine("Received: " + e.Data);
            }
        }
    }
}
~~~
#### App.config:
Same as in the [Sender](#app-config)  

The Receiver first creates a Guid myId, then creates a Global with that identifier stringified as the name, and then send this original Guid to the sender via assignment of the myId Global. The sender then creates a Global with the stringified identifier as its name, being the same Global as defined on the receiving side, and use that Global to communicate the data.


## Guaranteed Delivery

With the current implementation, assigning a value to a Global says nothing about its delivery. You don’t know if data is delivered after assigning a value to that global. In that case some two-way communication has to be created, which can be on many different ways. Below one of the possible solutions is given, where a message is send back to the sender after receiving its data. A one-to-one configuration is assumed here.
### Guaranteed Delivery Sender

#### Program.cs:
~~~csharp
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
            using (var Id = new Global<Guid>("IdCheck", handler: 
Id_DataChanged))
            using (var Message = new Global<Envellope>("message"))
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

### Guaranteed Delivery Receiver
#### Program.cs:

~~~csharp
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
            using (var Message = new Global<Envellope>("message", 
   handler: Message_DataChanged))
            {
                // Waiting for the message until enter is h
                Console.ReadLine();
            }
        }

       private static void Message_DataChanged(object sender, 
    GlobalEventData<Envellope> e)
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

What happens here: The sender puts its data in another class, the Envellope, containing the data to be send, together with an Id field, in this case a Guid. The Receiver sends the Id back to the sender after processing. Now the sender is assured (guaranteed) that the data is delivered.

## Other (undocumented)

### Simple File Transfer
#### Program.cs:
~~~csharp
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
~~~
#### App.config:
Same as in the [Sender](#app-config)  

### Voting

### Vote Server
#### Program.cs:
~~~csharp
using System;
using System.Windows.Forms;

namespace VoteFormServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VoteFormServer());
        }
    }
}
~~~
#### VoteFormServer.cs:
~~~csharp
using Globals.NET.RabbitMQ;
using System.Windows.Forms;

namespace VoteFormServer
{
    public partial class VoteFormServer : Form
    {
        readonly GlobalReader<bool> Yes;
        readonly Global<int> TotalYes;
        readonly Global<int> TotalNo;
        readonly Global<bool> ServerOnline;
        private int _totalYes = 0;
        private int _totalNo = 0;

        public VoteFormServer()
        {
            InitializeComponent();

            Yes = new GlobalReader<bool>("Yes", handler: Yes_DataChanged);
            TotalYes = new Global<int>("TotalYes") { Value = 0 };
            TotalNo = new Global<int>("TotalNo") { Value = 0 };

            ServerOnline = new Global<bool>("ServerOnline")
            {
                Value = true
            };
        }

        private delegate void HandleInMainThreadDelegate(Control c, int i);

        // Some UI trouble
        private void HandleInMainThread(Control c, int i)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate(HandleInMainThread), new object[] { c, i });
            }
            else
            {
                c.Text = i.ToString();
            }
        }

        private void Yes_DataChanged(object sender, GlobalEventData<bool> e)
        {
            if (e.isInitialValue)
            {
                return;
            }

            if (e.Data)
            {
                TotalYes.Value = ++_totalYes;
                HandleInMainThread(lbYesCnt, _totalYes);
            }
            else
            {
                TotalNo.Value = ++_totalNo;
                HandleInMainThread(lbNoCnt, _totalNo);
            }
        }

        private void VoteFormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServerOnline.Value = false;

            Yes.Dispose();
            TotalYes.Dispose();
            TotalNo.Dispose();
            ServerOnline.Dispose();
        }
    }
}
~~~
#### App.config:
Same as in the [Sender](#app-config)  

### Vote Client
#### Program.cs:
~~~csharp
using System;
using System.Windows.Forms;

namespace Vote
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormVote());
        }
    }
}
~~~
#### VoteForm.cs:
~~~csharp
using Globals.NET.RabbitMQ;
using System;
using System.Windows.Forms;

namespace Vote
{
    public partial class FormVote : Form
    {
        // true if yes, false if no
        readonly Global<bool> Yes;
        readonly GlobalReader<int> TotalYes;
        readonly GlobalReader<int> TotalNo;
        readonly GlobalReader<bool> ServerOnline;

        public FormVote()
        {
            InitializeComponent();

            Yes = new Global<bool>("Yes");
            TotalYes = new GlobalReader<int>("TotalYes", handler: TotalYes_DataChanged);
            TotalNo = new GlobalReader<int>("TotalNo", handler: TotalNo_DataChanged);
            ServerOnline = new GlobalReader<bool>("ServerOnline", false, ServerOnline_DataChanged);
        }

        private delegate void HandleInMainThreadDelegate2(Control c, GlobalEventData<bool> e);

        private void HandleInMainThread2(Control c, GlobalEventData<bool> e)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate2(HandleInMainThread2), new object[] { c, e });
            }
            else
            {
                Enable(e.Data);
            }
        }

        private void ServerOnline_DataChanged(object sender, GlobalEventData<bool> e)
        {
            if (e.isDefault)
            {
                return;
            }

            HandleInMainThread2(lbYes, e);
        }

        private delegate void HandleInMainThreadDelegate(Control c, GlobalEventData<int> e);

        private void HandleInMainThread(Control c, GlobalEventData<int> e)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate(HandleInMainThread), new object[] { c, e });
            }
            else
            {
                c.Text = e.Data.ToString();
            }
        }

        private void TotalNo_DataChanged(object sender, GlobalEventData<int> e)
        {
            HandleInMainThread(lbNoCnt, e);            
        }

        private void TotalYes_DataChanged(object sender, GlobalEventData<int> e)
        {
            HandleInMainThread(lbYesCnt, e);
        }

        private void Enable(bool doEnable)
        {
            btnYes.Enabled = doEnable;
            btnNo.Enabled = doEnable;
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            Yes.Value = true;
            Enable(false);
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            Yes.Value = false;
            Enable(false);            
        }
    }
}
~~~
