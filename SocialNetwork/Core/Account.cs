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
        
        protected void OnFollowingPropertyHasChanged(IFeedableNote note)
        {
            FollowigPropertyHasChanged?.Invoke(this, note);
        }

        protected void OnFollowingPhotoHasChanged(Note note)
        {
            FollowingPhotoHasChanged?.Invoke(this, note);
        }

        protected void OnFollowingPhotoHasRemoved(Note note)
        {
            FollowingPhotoHasRemoved?.Invoke(this, note);
        }

        protected void OnFollowingPublishNews(Note note)
        {
            FollowingPublishedNews?.Invoke(this, note);
        }

        protected void OnFollowingRemovedNews(Note note)
        {
            FollowingRemovedNews?.Invoke(this, note);
        }

        protected void OnFollowingGotFollower(Note note)
        {
            FollowingGotFollower?.Invoke(this, note);
        }

        protected void OnFollowingGotFollowing(Note note)
        {
            FollowingGotFollowing?.Invoke(this, note);
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

        /// <summary>
        /// Возвращает <see cref="Account.Buffer"/> с копиями значений текущего экземпляра.
        /// </summary>
        /// <returns><see cref="Account.Buffer"/> с копиями значений.</returns>
        public Buffer GetBuffer()
        {
            return new Buffer {
                Id = Id,
                PassportBuffer = Passport.GetBuffer(),
                EducationBuffer = Education.GetBuffer(),
                ProfileBuffer = Profile.GetBuffer()
            };
        }

        public void EatBuffer(Buffer buffer)
        {
            Passport.Name = buffer.PassportBuffer.Name;
            Passport.Middlename = buffer.PassportBuffer.Middlename;
            Passport.Lastname = buffer.PassportBuffer.Lastname;
            Passport.Birthday = buffer.PassportBuffer.Birthday;
            Passport.Gender = buffer.PassportBuffer.Gender;
            Passport.Status = buffer.PassportBuffer.Status;
            Education.School = buffer.EducationBuffer.School;
            Education.University = buffer.EducationBuffer.University;
            Profile.Photo = buffer.ProfileBuffer.Photo;
        }

        /// <summary>
        /// Подписывает текущего пользователя на <paramref name="following"/>, для получени уведомлений об изменениях на стороне <paramref name="following"/>.
        /// </summary>
        /// <param name="following">Пользователь, на которого необходимо подписать текущего.</param>
        /// <returns>Успешность подписания.</returns>
        public bool SubscribeTo(Account following)
        {
            if (following == null) {
                throw new ArgumentNullException();
            }

            Profile.Bind(this, following);
            following.Passport.PropertyHasChanged += OnFollowingPropertyHasChanged;
            following.Education.PropertyHasChanged += OnFollowingPropertyHasChanged;
            following.Profile.PhotoHasChanged += OnFollowingPhotoHasChanged;
            following.Profile.PhotoHasRemoved += OnFollowingPhotoHasRemoved;
            following.Profile.PublishedNews += OnFollowingPublishNews;
            following.Profile.NewsHasRemoved += OnFollowingRemovedNews;
            following.Profile.GotFollower += OnFollowingGotFollower;
            following.Profile.GotFollowing += OnFollowingGotFollowing;

            if (following.Passport.Birthday.DayOfYear == DateTime.Now.DayOfYear) {
                following.OnFollowingPropertyHasChanged(new Note(following.Id, "День рождение.", $"Сегодня день рождения пользователя {following.Passport.Middlename} {following.Passport.Name}"));
            }

            return true;
        }

        public void Unsubscribe(Account following)
        {
            if (following == null) {
                throw new ArgumentNullException();
            }

            Profile.Untie(this, following);
            following.Passport.PropertyHasChanged -= OnFollowingPropertyHasChanged;
            following.Education.PropertyHasChanged -= OnFollowingPropertyHasChanged;
            following.Profile.PhotoHasChanged -= OnFollowingPhotoHasChanged;
            following.Profile.PhotoHasRemoved -= OnFollowingPhotoHasRemoved;
            following.Profile.PublishedNews -= OnFollowingPublishNews;
            following.Profile.NewsHasRemoved -= OnFollowingRemovedNews;
            following.Profile.GotFollower -= OnFollowingGotFollower;
            following.Profile.GotFollowing -= OnFollowingGotFollowing;
        }

        #endregion
    }
}
