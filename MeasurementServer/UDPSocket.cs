using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace MeasurementServer
{
    class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
        private const int buffSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 9000);
        private AsyncCallback recv = null;
        private string commandReceived;

        public class State
        {
            public byte[] buffer = new byte[buffSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.ReuseAddress,true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address),port ));
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
        }

        public void SendText(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State) ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine($"SEND: {0}, {1}", bytes, text);
            }, state);
        }

        public void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, buffSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State) ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, buffSize, SocketFlags.None, ref epFrom, recv, so);
                Console.WriteLine($"RECV: {0}: {1}, {2}",epFrom.ToString(),bytes,Encoding.ASCII.GetString(so.buffer,0,bytes));
                commandReceived = Encoding.ASCII.GetString(so.buffer, 0, bytes);
            }, state);

            switch (commandReceived)
            {
                case "U":
                case "u":
                    var uptimeText = File.ReadAllText("/proc/uptime");
                    string[] uptimes = uptimeText.Split(',');
                    _socket.Connect(IPAddress.Parse("10.0.0.2"),9000);
                    SendText(uptimes[0]);
                    _socket.Disconnect(true);
                    break;

                case "L":
                case "l":
                    var loadText = File.ReadAllText("/proc/loadavg");
                    string[] loads = loadText.Split(' ', 5);
                    _socket.Connect(IPAddress.Parse("10.0.0.2"),9000);
                    string toSend = "Load average: \t Last minut: " + loads[0] + "\t Last 5 minutes: " + loads[1] +
                                    "\t Last 15 minutes: " + loads[2] +
                                    "\t Currently running kernel scheduling entities/existing kernel scheduling entities: " +
                                    loads[3];
                    SendText(toSend);
                    _socket.Disconnect(true);
                    break;
                default:
                    break;
            }
        }

    }

    
}
