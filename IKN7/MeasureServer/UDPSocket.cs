using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace MeasurementServer
{
    class UDPSocket
    {
		const int PORT = 9000;
		private UdpClient Server;
        private const int buffSize = 8 * 1024;
        private IPEndPoint epFrom = new IPEndPoint(IPAddress.Any, PORT);
        private string commandReceived;
                                                
        public void SendText(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
			Server.Send(data, data.Length);
        }

        public void Receive()
        {
			Server = new UdpClient(PORT);
           
			byte[] dataReceived = Server.Receive(ref epFrom);
			commandReceived = Encoding.ASCII.GetString(dataReceived);
            
            switch (commandReceived)
            {
                case "U":
                case "u":
                    var uptimeText = File.ReadAllText("/proc/uptime");
                    string[] uptimes = uptimeText.Split(' ');
                    Server.Connect(IPAddress.Parse("10.0.0.2"), 9000);
					SendText("Server uptime in seconds: " + uptimes[0]);
					Server.Close();
                    break;

                case "L":
                case "l":
                    var loadText = File.ReadAllText("/proc/loadavg");
                    string[] loads = loadText.Split(' ');
                    Server.Connect(IPAddress.Parse("10.0.0.2"), 9000);
                    string toSend = "Load average: \t Last minut: " + loads[0] + "\t Last 5 minutes: " + loads[1] +
                                    "\t Last 15 minutes: " + loads[2] +
                                    "\t Currently running kernel scheduling entities/existing kernel scheduling entities: " +
                                    loads[3];
                    SendText(toSend);
					Server.Close();
                    break;
                default:
                    break;
            }
        }

    }


}
