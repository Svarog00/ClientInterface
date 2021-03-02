using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientInterface
{
	public partial class Form1 : Form
	{
		bool isConnected;
		int port = 8005;
		string address;
		byte[] passB;

		StringBuilder pass = new StringBuilder();
		IPEndPoint ipPoint;
		Socket server;

		public Form1()
		{
			InitializeComponent();
			isConnected = false;
		}

		private void button1_Click(object sender, EventArgs e) //Connect button
		{
			try
			{
				address = textBox1.Text;
				ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

				server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				//подключаемся к удаленному хосту
				server.Connect(ipPoint);
				//получаем ключ дешифровки и шифровки
				int bytesOfPass = 0;
				passB = new byte[256];
				do
				{
					bytesOfPass = server.Receive(passB, passB.Length, 0);
					pass.Append(Encoding.Unicode.GetString(passB, 0, bytesOfPass));
				}
				while (server.Available > 0);
				textBox1.Clear();
				richTextBox1.AppendText($"Completed! Welcome {server.LocalEndPoint} \n");
				isConnected = true;
				GetMessagesAsync(server, pass.ToString());
			}
			catch(Exception exception)
			{
				richTextBox1.AppendText(exception.Message);
			}
		}

		private void button2_Click(object sender, EventArgs e) //Send button
		{
			if(isConnected)
			{
				string message = textBox2.Text;
				string codedMessage = Cryptography.Encode(message, pass.ToString()); //кодирование сообщения
				byte[] data = Encoding.Unicode.GetBytes(codedMessage);
				server.Send(data); //отправка на сервер
			}
			textBox2.Clear();
		}

		private async void GetMessagesAsync(Socket server, string pass)
        {
			await Task.Run(() => GettingMessages(server, pass));
        }

		private void GettingMessages(Socket server, string pass)
		{
			while (isConnected)
			{
				// получаем ответ
				byte[] data = new byte[256]; // буфер для ответа
				StringBuilder builder = new StringBuilder();
				do
				{
					int bytes = server.Receive(data, data.Length, 0);
					builder.Append(Encoding.Unicode.GetString(data, 0, bytes)); //получение закодированного сообщения
				}
				while (server.Available > 0);
				string tmpStr;
				tmpStr = Cryptography.Decode(builder.ToString(), pass); //декодирование полученного сообщения
				Invoke(new MethodInvoker(() => { richTextBox1.AppendText($"\n{tmpStr}"); }));
				//richTextBox1.AppendText($"\n{tmpStr}");
			}
		}

		private void button3_Click(object sender, EventArgs e) //Disconnect button
		{
			if(isConnected)
			{
				string message = "/disconnect";
				string codedMessage = Cryptography.Encode(message, pass.ToString()); //кодирование сообщения
				byte[] data = Encoding.Unicode.GetBytes(codedMessage);
				server.Send(data); //отправка на сервер
				richTextBox1.AppendText($"Goodbye Y_Y");
				isConnected = false;
			}
		}
	}
}
