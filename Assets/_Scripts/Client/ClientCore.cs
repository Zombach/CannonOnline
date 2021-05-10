using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Client
{
    public class ClientCore : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _button;
        [SerializeField] private Text _viewText;

        private bool _isInputNameUser = true;
        static string _userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        

        private void Start()
        {
            _viewText.text = "Клиент \n";
            _viewText.text += "Введите свое имя: \n";
        }

        public void InputNameUser()
        {
            if (_isInputNameUser)
            {
                if (_inputField.text.Length != 0)
                {
                    _isInputNameUser = false;
                    _userName = _inputField.text;
                    Connection();
                }
            }
            else
            {
                SendMessage();
            }
        }



        private void CreateThreadReceive()
        {
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start(); //старт потока
        }

        private void Connection()
        {
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                
                stream = client.GetStream(); // получаем поток
                
                byte[] data = Encoding.Unicode.GetBytes(_userName);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                
                string Greeting = "║ Добро пожаловать: " + _userName + " ║";
                
                var line = "╔" + new string('═', Greeting.Length - 2) + "╗";
                var line2 = "╚" + new string('═', Greeting.Length - 2) + "╝";
                _viewText.text += line + "\n";
                _viewText.text += Greeting + "\n";
                _viewText.text += line2 + "\n";
            }
            catch (Exception e)
            {
                Disconnect();
                Time.timeScale = 0;
            }
        }

        private static void Log(string v)
        {
            throw new NotImplementedException();
        }

        // отправка сообщений
        void SendMessage()
        {
            _viewText.text += "Введите сообщение: \n";

            while (true)
            {
                string message = _inputField.text;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        // получение сообщений
        void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    _viewText.text += message + "\n";//вывод сообщения
                }
                catch
                {
                    _viewText.text += "Подключение прервано! \n"; //соединение было прервано
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

       
    }
}

