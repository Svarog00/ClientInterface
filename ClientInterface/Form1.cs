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
		int port = 8005;
		Client client;
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) //Connect button
		{
			try
			{
				client = new Client(textBox1.Text, port, richTextBox1, textBox3);
			}
			catch(Exception exception)
			{
				richTextBox1.AppendText(exception.Message);
			}
		}

		private void button2_Click(object sender, EventArgs e) //Send button
		{
			client.SendMessage(textBox2);
			textBox2.Clear();
		}

		private void button3_Click(object sender, EventArgs e) //Disconnect button
		{
			client.Disconnect(richTextBox1);
		}
	}
}
