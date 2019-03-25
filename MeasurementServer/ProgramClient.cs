using System;

namespace UdpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientSocket _Client = new ClientSocket();
            _Client.Client("10.0.0.2",9000);
            _Client.Send("u");

            Console.ReadKey();
        }
    }
}
