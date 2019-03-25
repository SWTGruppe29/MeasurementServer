using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClient
{
    class ClientSocket
    {
        private Socket _ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int _BufferSize = 8 * 1024;
        private State _State = new State();
        private EndPoint _EPFrom = new IPEndPoint(IPAddress.Any, 9000);
        private AsyncCallback _Receive = null;

        public class State
        {
            public byte[] _Buffer = new byte[_BufferSize];
        }

        public void Client(string address, int port)
        {
            _ClientSocket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string send)
        {
            byte[] _Data = Encoding.ASCII.GetBytes(send);
            _ClientSocket.BeginSend(_Data, 0, _Data.Length, SocketFlags.None, (ar) =>
            {
                State _SO = (State) ar.AsyncState;
                int Bytes = _ClientSocket.EndSend(ar);
                Console.WriteLine("Send: {0}, {1}", Bytes, send);
            }, _State);
        }

        private void Receive()
        {
            _ClientSocket.BeginReceiveFrom(_State._Buffer, 0, _BufferSize, SocketFlags.None, ref _EPFrom, _Receive = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _ClientSocket.EndReceiveFrom(ar, ref _EPFrom);
                _ClientSocket.BeginReceiveFrom(so._Buffer, 0, _BufferSize, SocketFlags.None, ref _EPFrom, _Receive, so);
                Console.WriteLine("RECV: {0}: {1}, {2}", _EPFrom.ToString(), bytes, Encoding.ASCII.GetString(so._Buffer, 0, bytes));
            }, _State);
        }
    }
}
