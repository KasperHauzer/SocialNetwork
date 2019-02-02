using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SocialNetwork.Net
{
    /// <summary>
    /// Инкапсулирует сообщения для общения клиентской и серверной частей.
    /// </summary>
    [Serializable]
    public struct Response
    {
        #region Поля

        /// <summary>
        /// Успешность исполнения запроса (контекстно зависимое поле).
        /// </summary>
        public bool IsSuccessful;

        /// <summary>
        /// Описание запроса, команда запроса, описание ответаю (контекстно зависимое поле).
        /// </summary>
        public string Description;

        /// <summary>
        /// Дополнительные данные необходимые для выполнения запроса или дополнительные данные ответа (контекстно зависимое поле).
        /// Данное поле должно хранить ссылку только на обхекты помеченные атрибутом <see cref="SerializableAttribute"/>.
        /// </summary>
        public object Data;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Полный конструткор запросов и ответов.
        /// </summary>
        /// <param name="isSuccess">Успешность запроса или ответа.</param>
        /// <param name="description">Описание запроса или ответа.</param>
        /// <param name="data">Дополнительные данные запроса или ответа.</param>
        public Response(bool isSuccess, string description, object data)
        {
            IsSuccessful = isSuccess;
            Description = description ?? "NULL";
            Data = data ?? new object();
        }

        /// <summary>
        /// Конструктор ответов без дополнительных данных.
        /// </summary>
        /// <param name="isSuccess">Успешность выполнения запроса.</param>
        /// <param name="description">Описание ответа.</param>
        public Response(bool isSuccess, string description) : this(isSuccess, description, null) { }

        /// <summary>
        /// Конструктор ответов без описаний.
        /// </summary>
        /// <param name="isSuccess">Успешность выполнеиня запроса.</param>
        /// <param name="data">Дополнительные данные.</param>
        public Response(bool isSuccess, object data) : this(isSuccess, null, data) { }

        /// <summary>
        /// Констуктор запросов с дополнительными данными.
        /// </summary>
        /// <param name="description">Описание или команда запроса.</param>
        /// <param name="data">Дополнительные данный для выполнения запроса.</param>
        public Response(string description, object data) : this(true, description, data) { }

        /// <summary>
        /// Конструктор запросов без дополнительных данных.
        /// </summary>
        /// <param name="description">Описание или команда запроса.</param>
        public Response(string description) : this(true, description, null) { }

        #endregion

        #region Методы

        public static Response Parse(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer)) {
                return (Response)new BinaryFormatter().Deserialize(stream);
            }
        }

        public override string ToString()
        {
            return Description;
        }

        public byte[] ToBuffer()
        {
            using (var stream = new MemoryStream()) {
                new BinaryFormatter().Serialize(stream, this);
                return stream.GetBuffer();
            }
        }

        #endregion
    }
}
 