using System;

namespace SocialNetwork.Core.Scope
{
    /// <summary>
    /// Инкапсулирует запись с неизменяемыми свойствами после создания.
    /// </summary>
    [Serializable]
    public struct Note : IFeedableNote
    {
        #region Свойства

        /// <summary>
        /// Id владельца записи.
        /// </summary>
        public string OwnerId {
            get;
        }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title {
            get;
        }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description {
            get;
        }

        /// <summary>
        /// Время создания.
        /// </summary>
        public DateTime Time {
            get;
        }

        /// <summary>
        /// Дополнительные данные.
        /// </summary>
        public object Data {
            get;
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Полный конструтор записи.
        /// </summary>
        /// <param name="ownerId">Id владельца записи.</param>
        /// <param name="title">Заголовок.</param>
        /// <param name="description">Описание.</param>
        /// <param name="time">Время записи.</param>
        /// <param name="data">Дополнительные данные записи.</param>
        public Note(string ownerId, string title, string description, DateTime time, object data)
        {
            OwnerId = ownerId;
            Title = title;
            Description = description;
            Time = time;
            Data = data;
        }

        /// <summary>
        /// Конструктор записи с установкой свойтсва <see cref="Time"/> равное времени создания записи.
        /// </summary>
        /// <param name="ownerId">Id владельца записи.</param>
        /// <param name="title">Заголовок.</param>
        /// <param name="description">Описание.</param>
        /// <param name="data">Дополнительные данные записи.</param>
        public Note(string ownerId, string title, string description, object data) : this(ownerId, title, description, DateTime.Now, data) { }

        /// <summary>
        /// Конструктор записи с установкой свойства <see cref="Time"/> равное времени создания записи и без дополнительных данных.
        /// </summary>
        /// <param name="ownerId">Id владельца записи.</param>
        /// <param name="title">Заголовок.</param>
        /// <param name="description">Описание.</param>
        public Note(string ownerId, string title, string description) : this(ownerId, title, description, DateTime.Now, null) { }

        #endregion
    }
}
