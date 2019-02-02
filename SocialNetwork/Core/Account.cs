using SocialNetwork.Core.Scope;
using System;

namespace SocialNetwork.Core
{
    /// <summary>
    /// Инкапсулирует пользователя в социальной сети.
    /// </summary>
    public class Account
    {
        #region Вложенные структуры

        /// <summary>
        /// Инкапсулирует значимые данные содержащего типа для представления в бинарном виде.
        /// </summary>
        [Serializable]
        public struct Buffer
        {
            public string Id;
            public Passport.Buffer PassportBuffer;
            public Education.Buffer EducationBuffer;
            public Profile.Buffer ProfileBuffer;
        }

        #endregion

        #region Поля

        /// <summary>
        /// При изменении свойств <see cref="Passport"/>, <see cref="Education"/> пользователей, на которых подписан данные, вызывается данное событие.
        /// </summary>
        public event Action<Account, IFeedableNote> FollowigPropertyHasChanged;

        /// <summary>
        /// При изменени <see cref="Profile.Photo"/> пользователей, на которых подписан данные, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingPhotoHasChanged;

        /// <summary>
        /// При удалении <see cref="Profile.Photo"/> пользователями, на которых подписан данный, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingPhotoHasRemoved;

        /// <summary>
        /// При добавлении новых новостей методом <see cref="Profile.AddNews(Note)"/> пользователями, на которых подписан данные, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingPublishedNews;

        /// <summary>
        /// При удалении новости методом <see cref="Profile.RemoveNews(Note)"/> пользователями, на которых подписан данные, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingRemovedNews;

        /// <summary>
        /// При подписке других пользователей на пользователей, на которых подписан данный, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingGotFollower;

        /// <summary>
        /// При подписке пользователей, на которых подписан данный, на других пользователей, вызывается данное событие.
        /// </summary>
        public event Action<Account, Note> FollowingGotFollowing;

        string id;

        #endregion

        #region Свойства

        /// <summary>
        /// Поле для однозначной идентификации пользователя.
        /// Однозначность данного поля необходимо поддерживать самостоятельно.
        /// </summary>
        public string Id {
            get => id;
            set {
                if (!IsValidId(value)) {
                    throw new ArgumentException();
                }

                id = value;
            }
        }

        /// <summary>
        /// Личный данные.
        /// </summary>
        public Passport Passport {
            get;
            set;
        }

        /// <summary>
        /// Данные об образовании.
        /// </summary>
        public Education Education {
            get;
            set;
        }

        /// <summary>
        /// Профильные данные пользователя.
        /// </summary>
        public Profile Profile {
            get;
            set;
        }

        #endregion

        #region Конструкторы, On-методы

        /// <summary>
        /// Полный конструктор.
        /// </summary>
        /// <param name="id">Однозначный идентификатор пользователя.</param>
        public Account(string id)
        {
            Id = id;
            Passport = new Passport(this);
            Education = new Education(this);
            Profile = new Profile(this);
        }
        
        protected void OnFollowingPropertyHasChanged(Account account, IFeedableNote note)
        {
            FollowigPropertyHasChanged?.Invoke(account, note);
        }

        protected void OnFollowingPhotoHasChanged(Account account, Note note)
        {
            FollowingPhotoHasChanged?.Invoke(account, note);
        }

        protected void OnFollowingPhotoHasRemoved(Account account, Note note)
        {
            FollowingPhotoHasRemoved?.Invoke(account, note);
        }

        protected void OnFollowingPublishNews(Account account, Note note)
        {
            FollowingPublishedNews?.Invoke(account, note);
        }

        protected void OnFollowingRemovedNews(Account account, Note note)
        {
            FollowingRemovedNews?.Invoke(account, note);
        }

        protected void OnFollowingGotFollower(Account account, Note note)
        {
            FollowingGotFollower?.Invoke(account, note);
        }

        protected void OnFollowingGotFollowing(Account account, Note note)
        {
            FollowingGotFollowing?.Invoke(account, note);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Указывает можно ли использовать входную строку как значение поля <see cref="Id"/>.
        /// </summary>
        /// <param name="id">Входная строка.</param>
        /// <returns>Валидность идентификатора.</returns>
        public static bool IsValidId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Id;
        }

        #endregion
    }
}
