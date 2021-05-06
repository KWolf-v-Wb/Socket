using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
    //Nuove librerie aggiunte
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int port;
        public MainWindow()
        {
            InitializeComponent();

            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()), port);
            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketRecieve));
            ricezione.Start(sourceSocket);
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = txtIp.Text;

            //Necessari controlli sul contenuto delle textbox
            //if(CorrectIp(ipAddress) && CorrectPort(port))

            SocketSend(IPAddress.Parse(ipAddress), port, txtMessage.Text);
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtPort.Text.Length > 0)
            {
                try
                {
                    port = int.Parse(txtPort.Text);
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.Message, "Errore");
                }
            }
        }

        public async void SocketRecieve(object socketSource)
        {
            IPEndPoint ipEndP = (IPEndPoint)socketSource;

            Socket t = new Socket(ipEndP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            t.Bind(ipEndP);
            
            Byte[] bytesRicevuti = new Byte[256];
            string message;
            int bytes = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if(t.Available > 0)
                    {
                        message = "";
                        bytes = t.Receive(bytesRicevuti, bytesRicevuti.Length, 0);
                        message += Encoding.ASCII.GetString(bytesRicevuti, 0, bytes);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lstRicevi.Items.Add(message);
                        }));
                    }
                }
            });
        }

        public void SocketSend(IPAddress dest, int destPort, string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);
            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint remoteEndPoint = new IPEndPoint(dest, destPort);

            s.SendTo(byteInviati, remoteEndPoint);
        }
    }
}
