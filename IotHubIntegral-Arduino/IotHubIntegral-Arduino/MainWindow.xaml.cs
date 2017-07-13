using System.Windows;
using System.IO.Ports;
using System.Threading;
using System;

namespace IotHubIntegral_Arduino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _sp;
        Thread _trhead;

        bool flag;
        string message;


        public MainWindow()
        {
            InitializeComponent();
            _sp = new SerialPort();

            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_sp.IsOpen) {
                button.Content = "Start";
                flag = false;
                _sp.DataReceived -= ReadIn;


                _trhead.Join();
                _sp.Close();
            }
            else
            {
                button.Content = "stop";
                _sp.PortName = "COM3";
                _sp.BaudRate = 9600;
                
                _trhead = new Thread(new ThreadStart(Handle));
                _sp.Open();
                _sp.DataReceived += new SerialDataReceivedEventHandler(ReadIn);
                textBox.Text += _sp.ReadLine();

            }
        }

        private void ReadIn(object sender, SerialDataReceivedEventArgs e)
        {
            if (!_trhead.IsAlive)
            {
                flag = true;
                _trhead.Start();
            }
        }

        private void Handle()
        {
            while (flag)
            {
                if (_sp.IsOpen)
                {
                    try
                    {
                        //message = _sp.ReadByte().ToString("X2");
                        message = _sp.ReadLine().ToString();
                        Dispatcher.BeginInvoke((Action)(() =>
                            { textBox.Text += message + " - ";}
                        ));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
    }
}
