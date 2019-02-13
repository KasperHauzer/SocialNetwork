using SocialNetwork.Core;
using System;
using System.Net;
using System.Net.Sockets;

namespace SocialNetwork.Net
{
    /// <summary>
    /// Инкапсулирует логику клиентской части.
    /// </summary>
    public class Client
    {
        #region Свойства

        protected Socket Socket {
            get;
            set;
        }

        public Account Account {
            get;
            set;
        }

        public IPEndPoint RemoteEP {
            get;
            set;
        }

        public IPEndPoint EndPoint {
            get;
            set;
        }

        #endregion

        #region Конструкторы

        public Client(IPEndPoint remoteEP, IPEndPoint myEP)
        {
            RemoteEP = remoteEP;
            EndPoint = myEP;
        }

        #endregion

        #region Методы

        protected void AcceptCallback(IAsyncResult result)
        {

        }

        protected byte[] Send(byte[] buffer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var answerBuffer = new byte[short.MaxValue];

            socket.Connect(RemoteEP);
            socket.Send(buffer);
            socket.Receive(answerBuffer);

            socket.Close();
            return answerBuffer;
        }

        #endregion

        #region Методы запросов

        public Response ConvertResponse(Response request)
        {
            switch (request.Description) {
                case "run": {
                    return Run();
                }

                case "stop": {
                    return Stop();
                }

                case "register" when request.Data is string: {
                    var id = (string)request.Data;
                    return Register(id);
                }

                case "update" when request.Data is Account.Buffer: {
                    var buffer = (Account.Buffer)request.Data;
                    return Update(buffer);
                }

                default: return new Response(false, "Команда не распознана.");
            }
        }

        protected Response Run()
        {
            if (Socket != null) {
                return new Response("Клиент был активирован ранее.");
            }

            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(EndPoint);
            Socket.Listen(2);
            Socket.BeginAccept(AcceptCallback, null);
            return new Response("Клиент успешно активирован");
        }

        protected Response Stop()
        {
            if (Socket == null) {
                return new Response(false, "Клиент был дективирован ранее.");
            }

            Socket.Close();
            Socket = null;
            return new Response(true, "Сервер успешно деактивирован.");
        }

        protected Response Register(string id)
        {
            var request = new Response("register", id);
            var answer = Send(request.GetBuffer());

            return Response.Parse(answer);
        }

        protected Response Login(string id)
        {
            var request = new Response("login", id);
            var answer = Send(request.GetBuffer());
            var response = Response.Parse(answer);

            if (response.IsSuccessful) {
                Account.Buffer buffer = (Account.Buffer)response.Data;
                Account.EatBuffer(buffer);
            }

            return response;
        }

        protected Response Update(Account.Buffer buffer)
        {
            var request = new Response("update", buffer);
            var answer = Send(request.GetBuffer());

            return Response.Parse(answer);
        }

        #endregion
    }
}
