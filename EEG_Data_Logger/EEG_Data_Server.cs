using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EEG_Data_Logger
{
    class EEG_Data_Server
    {
        EEG_Logger mLogger;

        IPEndPoint ipep;
        Socket server;
        Socket client;
        IPEndPoint ip;

        String _buf = "연결됨";
        Byte[] _data;

        string key;
        bool is_running;

        public EEG_Data_Server(EEG_Logger logger)
        {
            mLogger = logger;
            is_running = false;
        }

        public void Open()
        {
            ipep = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(ipep);
            server.Listen(20);

            Console.WriteLine("Server Start... Listen port 9999...");

            client = server.Accept();
            ip = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("주소 {0} 에서 접속", ip.Address);

            //_buf = "연결됨";
            //_data = Encoding.Default.GetBytes(_buf);
            //client.Send(_data);

            is_running = true;
            Thread t = new Thread(new ThreadStart(Loop));
            t.Start();
        }

        public void Loop()
        {
            while (is_running)
            {
                _data = new Byte[32];
                client.Receive(_data);
                _buf = Encoding.Default.GetString(_data).Split('\0')[0];
                //Console.WriteLine(_buf);
                //client = server.Accept();

                // Click event listener
                if (_buf.Equals("1"))
                {
                    //Console.WriteLine("Start");
                    mLogger.Start();
                }
                else if (_buf.Equals("2"))
                {
                    string selected_item = mLogger.Analysis(key);
                    SendMessage(selected_item);
                }
                //else if(_buf.Equals("MP7"))
                //{
                //    string selected_item = mLogger.Analysis("MP7");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("MP8"))
                //{
                //    string selected_item = mLogger.Analysis("MP8");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("MN7"))
                //{
                //    string selected_item = mLogger.Analysis("MN7");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("MN8"))
                //{
                //    string selected_item = mLogger.Analysis("MN8");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("NP7"))
                //{
                //    string selected_item = mLogger.Analysis("NP7");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("NP8"))
                //{
                //    string selected_item = mLogger.Analysis("NP8");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("NN7"))
                //{
                //    string selected_item = mLogger.Analysis("NN7");
                //    SendMessage(selected_item);
                //}
                //else if (_buf.Equals("NN8"))
                //{
                //    string selected_item = mLogger.Analysis("NN8");
                //    SendMessage(selected_item);
                //}
                else if (_buf.Equals("T"))
                {
                    string selected_item = mLogger.Analysis("T");
                    key = selected_item;
                    SendMessage(selected_item);
                }
            }
        }

        public void Close()
        {
            is_running = false;
            client.Close();
            server.Close();
        }

        public void SendMessage(string message)
        {
            //Console.WriteLine("SendMessage " + message);
            Byte[] StrByte = Encoding.Default.GetBytes(message);
            client.Send(StrByte);
        }
    }
}