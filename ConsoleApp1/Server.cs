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
        static Thread Messages;
        static int currentClients = 0;
        static List<Client> Handlers = new List<Client>();
        static int port = 8005; // порт для приема входящих запросов

        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                Messages = new Thread(() => Chatting());
                Messages.Start();

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[256]; // буфер для получаемых данных
                    do
                    {
                        int bytes = handler.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listenSocket.Available > 0);

                    Handlers.Add(new Client(builder.ToString(), handler));
                    //Console.WriteLine(handlers[currentClients].RemoteEndPoint + " connected"); //вывод о том, что кто-то подключился
                    Console.WriteLine(Handlers[currentClients].Nickname + " connected");
                    currentClients++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Chatting()
        {
            while(true)
            {
                // получаем сообщение
                StringBuilder builder = new StringBuilder();
                byte[] data = new byte[256]; // буфер для получаемых данных

                for (int i = 0; i < currentClients; i++)
                {
                    do
                    {
                        int bytes = Handlers[i].CSocket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Handlers[i].CSocket.Available > 0);

                    Console.WriteLine(Handlers[i].Nickname + ": " + builder);

                    string str = Handlers[i].Nickname + ": " + builder; //создание строки "Клиент: (сообщение)"
                    byte[] strB = Encoding.Unicode.GetBytes(str);

                    for (int j = 0; j < Handlers.Count; j++)
                    {
                        Handlers[j].CSocket.Send(strB);
                    }

                    if (builder.ToString() == "/disconnect")
                    {
                        // отключение клиента
                        Handlers[i].CSocket.Disconnect(true);
                        Handlers[i].CSocket.Close();
                        Handlers.Remove(Handlers[i]);
                        currentClients--;
                    }
                    builder.Clear(); //очистка буфера StringBuilder
                }
            }
        }
    }
}