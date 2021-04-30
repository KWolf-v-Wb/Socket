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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.21"), 56000);

            btnInvia.IsEnabled = true;

            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketRecieve));
            ricezione.Start(sourceSocket);
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = txtIp.Text;
            int port = int.Parse(txtport.Text);

            //Necessari controlli sul contenuto delle textbox
            //if(CorrectIp(ipAddress) && CorrectPort(port))

            SocketSend(IPAddress.Parse(ipAddress), port, txtMessage.Text);
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
                            lblRicevi.Content = message;
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
