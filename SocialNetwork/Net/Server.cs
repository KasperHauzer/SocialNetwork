using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SocialNetwork.Core;
using SocialNetwork.Core.Scope;

namespace SocialNetwork.Net
{
    public class Server
    {
        #region Свойства

        protected Socket Socket {
            get;
            set;
        }

        public IPHostEntry HostEntry {
            get;
        } = Dns.GetHostEntry("localhost");

        public IPAddress Address {
            get;
            set;
        }

        public IPEndPoint EndPoint {
            get;
            set;
        }

        public List<Account> Accounts {
            get;
        } = new List<Account>();

        #endregion

        #region Конструкторы

        public Server(int port)
        {
            Address = HostEntry.AddressList[0];
            EndPoint = new IPEndPoint(Address, port);
        }

        #endregion

        #region Методы

        protected void AcceptCallback(IAsyncResult result)
        {
            Socket socket = Socket.EndAccept(result);
            var buffer = new byte[short.MaxValue];

            socket.Receive(buffer);
            var request = Response.Parse(buffer);

            var answer = ConvertResponse(request);
            socket.Send(answer.GetBuffer());

            socket.Close();
            Socket.BeginAccept(AcceptCallback, null);
        }

        protected void AccountPropertyHasChanged(Account account, IFeedableNote e)
        {
             
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

                case "info": {
                    return GetServerInfo();
                }

                case "login" when request.Data is string: {
                    var id = (string)request.Data;
                    return Login(id);
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

        /// <summary>
        /// Устанавливает свойство <see cref="Server.Socket"/>, <see cref="System.Net.Sockets.Socket"/> которой прослушивает сеть для принятия и обработки запросов.
        /// </summary>
        /// <returns>Ответ сервера.</returns>
        protected Response Run()
        {
            if (Socket != null) {
                return new Response(false, "Сервер был активирован ранее.");
            }

            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(EndPoint);
            Socket.Listen(16);
            Socket.BeginAccept(AcceptCallback, null);
            return new Response(true, "Сервер успешно активирован.");
        }

        /// <summary>
        /// Вызывает метод <see cref="System.Net.Sockets.Socket.Close"/> для значения свойства <see cref="Socket"/>, после чего устанавливает данное свйоства равное <see cref="null"/>.
        /// </summary>
        /// <returns>Ответ сервера.</returns>
        protected Response Stop()
        {
            if (Socket == null) {
                return new Response(false, "Сервер был диактивирован ранее.");
            }

            Socket.Close();
            Socket = null;
            return new Response(true, "Сервер успешно диактивирован.");
        }

        /// <summary>
        /// Возвращает основную информацию о состоянии сервера.
        /// </summary>
        /// <returns>Ответ сервера.</returns>
        protected Response GetServerInfo()
        {
            return new Response(
                (Socket != null ?
                "Сервер активирован и принимает входяшие запросы.\n" :
                "Сервер дизактивирован и не может принимать запросы.\n") +
                $"Число пользователей на сервере: {Accounts.Count}.\n" +
                $"Название хоста: {HostEntry.HostName}.\n" +
                $"Ip-адресс: {Address}.\n" +
                $"Номер порта конечной точки подключения: {EndPoint.Port}."
                );
        }

        /// <summary>
        /// Если входная строка не распознана как <see cref="Core.Account.Id"/> метод возвращает соответствующий ответ,
        /// если пользователь с таким <paramref name="id"/> существует, метод возвращает соответствующий ответ и ссылку на пользователя,
        /// если <paramref name="id"/> не используется, метод возвращает соответствующий ответ.
        /// </summary>
        /// <param name="id">Входная строка, которую необходимо проверить.</param>
        /// <returns>Ответ сервера.</returns>
        protected Response IsAvailableId(string id)
        {
            if (!Account.IsValidId(id)) {
                return new Response(false, "Входная строка не распознана как Id", id);
            }

            var account = Accounts.FirstOrDefault(x => x.Id == id);

            if (account != null) {
                return new Response(false, $"Данный Id ({id}) уже занят пользователем {account.Passport.Middlename} {account.Passport.Name}.", account);
            }

            return new Response(true, $"Данный Id ({id}) не занят и может быть использован.", id);
        }

        /// <summary>
        /// Ищет пользователя с указанным <paramref name="id"/>, если находит, то возвращает <see cref="Account"/> найденного пользователя.
        /// </summary>
        /// <param name="id"><see cref="Account.Id"/> пользователя, которого необходимо найти.</param>
        /// <returns>Ответ сервера.</returns>
        protected Response Search(string id)
        {
            if (!Account.IsValidId(id)) {
                return new Response(false, "Входная строка не распознана как Id", id);
            }

            var account = Accounts.FirstOrDefault(x => x.Id == id);

            if (account == null) {
                return new Response(false, $"Пользователь с таким Id ({id}) не найден.", id);
            }

            return new Response(true, $"Пользователь найден: {account.Passport.Middlename} {account.Passport.Name}.", account);
        }
       
        /// <summary>
        /// Возврощает <see cref="Account.Buffer"/> для искомого пользователя, если находится пользователь с <see cref="Account.Id"/> равным <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="Account.Id"/> искомого пользователя.</param>
        /// <returns>ответ сервера.</returns>
        protected Response Login(string id)
        {
            var x = Search(id);

            if (!x.IsSuccessful) {
                return x;
            }

            Account account = (Account)x.Data;
            return new Response(true, x.Description, account.GetBuffer());
        }

        /// <summary>
        /// Создает новый экхемпляр класса <see cref="Account"/> и добавляет его в список <see cref="Accounts"/>,
        /// если пользователь с таким <paramref name="id"/> уже существует или входная строка не распознана как
        /// <see cref="Account.Id"/>, то возвращает ответ метода <see cref="IsAvailableId(string)"/> без дополнительных данных.
        /// </summary>
        /// <param name="id">Входная строка.</param>
        /// <returns>Ответ сервера.</returns>
        protected Response Register(string id)
        {
            var x = IsAvailableId(id);

            if (!x.IsSuccessful) {
                return new Response(false, x.Description);
            }

            Account account = new Account(id);
            Accounts.Add(account);
            account.FollowigPropertyHasChanged += AccountPropertyHasChanged;
            return new Response(true, $"Новый аккаунт ({account}) успешно зарегестрирован.", account.GetBuffer());
        }

        /// <summary>
        /// Переносит значения полей <paramref name="buffer"/> на свойства аккаунта пользователя, у которого <see cref="Account.Id"/> равен <see cref="Account.Buffer.Id"/>.
        /// </summary>
        /// <param name="buffer">Экземпляр <see cref="Account.Buffer"/>, данные полей которого необходимо перенести.</param>
        /// <returns>Ответ сервера.</returns>
        protected Response Update(Account.Buffer buffer)
        {
            var x = Search(buffer.Id);

            if (!x.IsSuccessful) {
                return x;
            }

            var account = (Account)x.Data;
            account.EatBuffer(buffer);
            return new Response(true, "Данные пользователя успешно обновлены.");
        }

        #endregion
    }
}
