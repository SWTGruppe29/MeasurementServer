using System;
using System.Net.Sockets;

namespace MeasurementServer
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPSocket s = new UDPSocket();
            //s.Server("10.0.0.1", 9000);
			Console.WriteLine("Server started");
            while (true)
            {
                s.Receive();
            }
        }
    }
}
