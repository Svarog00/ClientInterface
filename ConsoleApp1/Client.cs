using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SocketTcpServer
{
    class Client
    {
        Socket clientSocket;
        string nickname;

        public Client(string nickname, Socket clientSocket)
        {
            this.nickname = nickname;
            this.clientSocket = clientSocket;
        }

        public Socket CSocket
        {
            get { return clientSocket; }
        }

        public string Nickname
        {
            get { return nickname; }
        }
    }
}
