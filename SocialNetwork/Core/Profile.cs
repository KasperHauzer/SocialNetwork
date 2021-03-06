﻿using SocialNetwork.Core.Scope;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SocialNetwork.Core
{
    /// <summary>
    /// Инкапсулирует профильные функции пользователя.
    /// Данный класс является зависимым свойством класса <see cref="Account"/>.
    /// </summary>
    public class Profile
    {
        #region Встроенные структуры

        /// <summary>
        /// Инкапсулирует значимые данные содержащего типа для представления в бинарном виде.
        /// </summary>
        [Serializable]
        public struct Buffer
        {
            public string OwnerId;
            public Bitmap Photo;
            public List<string> FollowersId;
            public List<string> FollowingId;
            public List<IFeedableNote> Journal;
            public List<Note> News;
        }

        #endregion

        #region Поля

        /// <summary>
        /// При изменение фота профиля вызывается данное событие.
        /// </summary>
        public event Action<Note> PhotoHasChanged;

        /// <summary>
        /// При удалении фота профиля вызывается данное событие.
        /// </summary>
        public event Action<Note> PhotoHasRemoved;

        /// <summary>
        /// При новой подписке на данного пользователя вызывается данное событие.
        /// </summary>
        public event Action<Note> GotFollower;

        /// <summary>
        /// При подписке данного пользователя на другого вызывается данное событие.
        /// </summary>
        public event Action<Note> GotFollowing;

        /// <summary>
        /// При добавлении новости вызывается данное событие.
        /// </summary>
        public event Action<Note> PublishedNews;

        /// <summary>
        /// При удалении новости вызывается данное событие.
        /// </summary>
        public event Action<Note> NewsHasRemoved;

        /// <summary>
        /// Фото профиля по умолчанию.
        /// </summary>
        protected static Bitmap DefaultPhoto = new Bitmap(200, 300);

        Bitmap photo;

        #endregion

        #region Свойства

        /// <summary>
        /// Ссылка на <see cref="Account"/>, которому принадлежит данный экземпляр класса.
        /// </summary>
        public Account Owner {
            get;
            protected set;
        }

        /// <summary>
        /// Список <see cref="Account.Id"/>, которые подписаны на события данного пользователя.
        /// </summary>
        public List<string> FollowersId {
            get;
            protected set;
        } = new List<string>();

        /// <summary>
        /// Список <see cref="Account.Id"/>, на которых подписан данный пользователя.
        /// </summary>
        public List<string> FollowingId {
            get;
            protected set;
        } = new List<string>();
        
        /// <summary>
        /// Журнал уведомлений, составляющийся из описаний действий других пользователей.
        /// </summary>
        public List<IFeedableNote> Journal {
            get;
            protected set;
        } = new List<IFeedableNote>();

        /// <summary>
        /// Список новостей пользователя.
        /// Для того, чтобы вызвалось событие <see cref="PublishedNews"/> и <see cref="NewsHasRemoved"/> 
        /// необходимо изменять данный список методами <see cref="AddNews(Note)"/> и <see cref="RemoveNews(Note)"/> соответственно.
        /// </summary>
        public List<Note> News {
            get;
            protected set;
        } = new List<Note>();

        /// <summary>
        /// Фото профиля, при удалении присвоить ссылку <see cref="Nullable"/>.
        /// Если фото было удалено, то возвращает фото по умолчанию.
        /// </summary>
        public Bitmap Photo {
            get {
                if (photo == null) {
                    return DefaultPhoto;
                }

                return photo;
            }
            set {
                if (value == null) {
                    OnPhotoRemoved("Удалено фото пользователя.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} удалил(-а) фото профиля.", DefaultPhoto);
                    return;
                }

                OnPhotoRemoved("Изменено фото пользователя.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} изменил(-а) фото профиля.", DefaultPhoto);
                photo = value;
            }
        }

        #endregion

        #region Конструкторы, On-методы

        /// <summary>
        /// Полный конструктор.
        /// </summary>
        /// <param name="owner">Экземпляр класса <see cref="Account"/>, на который будет ссылаться данный.</param>
        public Profile(Account owner)
        {
            Owner = owner ?? throw new ArgumentNullException();
        }

        protected void OnPhotoHasChanged(string title, string description, Bitmap photo)
        {
            PhotoHasChanged?.Invoke(new Note(Owner.Id, title, description, photo));
        }

        protected void OnPhotoRemoved(string title, string description, Bitmap photo)
        {
            PhotoHasRemoved?.Invoke(new Note(Owner.Id, title, description, photo));
        }

        protected void OnGotFollower(string title, string description, string followerId)
        {
            GotFollower?.Invoke(new Note(Owner.Id, title, description, followerId));
        }

        protected void OnGotFollowing(string title, string description, string followingId)
        {
            GotFollowing?.Invoke(new Note(Owner.Id, title, description, followingId));
        }

        protected void OnPublishedNews(string title, string description, Note note)
        {
            PublishedNews?.Invoke(new Note(Owner.Id, title, description, note));
        }

        protected void OnNewsHasRemoved(string title, string description, Note note)
        {
            NewsHasRemoved?.Invoke(new Note(Owner.Id, title, description, note));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавляет в список <see cref="Profile.FollowingId"/> <paramref name="follower"/> <see cref="Account.Id"/> <paramref name="following"/> 
        /// и добавляет в список <see cref="Profile.FollowersId"/> <paramref name="following"/> <see cref="Account.Id"/> <paramref name="follower"/>.
        /// Вызывает событие <see cref="Profile.GotFollowing"/> для <paramref name="follower"/> и событие <see cref="Profile.GotFollower"/> для <paramref name="following"/>.
        /// </summary>
        /// <param name="follower">Экземпляр класса <see cref="Account"/>, который подписывается.</param>
        /// <param name="following">Экземпляр класса <see cref="Account"/>, на которого подписываются.</param>
        /// <returns>Успешность свзяки двух пользователей.</returns>
        public static bool Bind(Account follower, Account following)
        {
            if (follower == null || following == null) {
                throw new ArgumentNullException();
            } else if (following.Profile.FollowersId.Contains(follower.Id) || follower.Profile.FollowingId.Contains(following.Id)) {
                return false;
            }

            follower.Profile.FollowingId.Add(following.Id);
            follower.Profile.OnGotFollowing("Новая подписка.", $"Пользователь {follower.Passport.Middlename} {follower.Passport.Name} подписался на {following.Passport.Middlename} {following.Passport.Name}.", following.Id);

            following.Profile.FollowersId.Add(follower.Id);
            following.Profile.OnGotFollower("Новый подписчик.", $"На пользователя {following.Passport.Middlename} {following.Passport.Name} подписался(-лась) {follower.Passport.Middlename} {follower.Passport.Name}.", follower.Id);

            return true;
        }

        /// <summary>
        /// Удаляет <see cref="Account.Id"/> <paramref name="following"/> из списка <see cref="Profile.FollowingId"/> и
        /// удаляет <see cref="Account.Id"/> <paramref name="follower"/> из списка <see cref="Profile.FollowersId"/>.
        /// </summary>
        /// <param name="follower">Экземпляр класса <see cref="Account"/>, который является подписчиком.</param>
        /// <param name="following">Экземпляр класса <see cref="Account"/>, на который подписан <paramref name="follower"/>.</param>
        public static void Untie(Account follower, Account following)
        {
            if (following == null || following == null) {
                throw new ArgumentNullException();
            }

            follower.Profile.FollowingId.Remove(following.Id);
            following.Profile.FollowersId.Remove(follower.Id);
        }

        public override string ToString()
        {
            return Owner.ToString();
        }

        /// <summary>
        /// Возвращает <see cref="Profile.Buffer"/> с копиями значений текущего экземпляра.
        /// </summary>
        /// <returns><see cref="Profile.Buffer"/> с копиями значений.</returns>
        public Buffer GetBuffer()
        {
            return new Buffer {
                OwnerId = Owner.Id,
                Photo = Photo,
                FollowersId = FollowersId,
                FollowingId = FollowingId,
                Journal = Journal,
                News = News,
            };
        }

        /// <summary>
        /// Добавляет новость в список <see cref="News"/> и вызывает событие <see cref="PublishedNews"/>.
        /// </summary>
        /// <param name="note">Добавляемая новость.</param>
        public void AddNews(Note note)
        {
            News.Add(note);
            OnPublishedNews("Добавлена новая запись.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} добавил(-а) новую запись.", note);
        }

        /// <summary>
        /// Удаляет новость из списка <see cref="News"/> и вызывает событие <see cref="NewsHasRemoved"/>.
        /// </summary>
        /// <param name="note">Удаляемая новость.</param>
        public void RemoveNews(Note note)
        {
            News.Remove(note);
            OnNewsHasRemoved("Удалена запись.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} удалил(-а) запись.", note);
        }

        #endregion
    }
}
