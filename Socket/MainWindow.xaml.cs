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
        string recieverIP;

        public MainWindow()
        {
            InitializeComponent();

            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()), 56000);
            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketRecieve));
            ricezione.Start(sourceSocket);
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            if(txtMessage.Text.Length > 0)
            {
                SocketSend(IPAddress.Parse(recieverIP), port, txtMessage.Text);
                lstContact.Items.Add(recieverIP + ":" + port);
                txtMessage.Text = "";
            }
        }

        private void txtIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtIp.Text.Split('.').Length == 4 && txtIp.Text.Split('.')[3] != "")
            {
                btnInvia.IsEnabled = false;
                try
                {
                    IPAddress.Parse(txtIp.Text);
                    recieverIP = txtIp.Text;
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.Message, "Errore");
                }

                if (port != 0)
                    btnInvia.IsEnabled = true;
            }
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtPort.Text.Length > 0)
            {
                btnInvia.IsEnabled = false;
                try
                {
                    port = int.Parse(txtPort.Text);
                    if (port > 65535)
                    {
                        MessageBox.Show("Valore della porta non valido");
                        port = 0;
                        return;
                    }
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.Message, "Errore");
                    txtPort.Text = txtPort.Text.Remove(txtPort.Text.Length - 1);
                }

                if (recieverIP != null)
                    btnInvia.IsEnabled = true;
            }
        }

        private void lstContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] contact = lstContact.SelectedItem.ToString().Split(':');

            txtIp.Text = contact[0];
            txtPort.Text = contact[1];
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
                        message += Encoding.UTF8.GetString(bytesRicevuti, 0, bytes);

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
            Byte[] byteInviati = Encoding.UTF8.GetBytes(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString() + ": " + message);
            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint remoteEndPoint = new IPEndPoint(dest, destPort);

            s.SendTo(byteInviati, remoteEndPoint);
        }
    }
}
