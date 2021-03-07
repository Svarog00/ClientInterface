using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
//server
namespace SocketTcpServer
{
    class Server
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            ServerClass serverClass = new ServerClass();
        }
    }
}