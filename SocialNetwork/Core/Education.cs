using SocialNetwork.Core.Scope;
using System;

namespace SocialNetwork.Core
{
    /// <summary>
    /// Инкапсулирует данные об образовании пользователя.
    /// Данный класс является зависимым свойством класса <see cref="Account"/>.
    /// </summary>
    public class Education
    {
        #region Вложенные структуры

        /// <summary>
        /// Инкапсулирует значимые данные содержащего типа для представления в бинарном виде.
        /// </summary>
        [Serializable]
        public struct Buffer
        {
            public string OwnerId;
            public string School;
            public string University;
        }

        #endregion

        #region Поля

        /// <summary>
        /// Перед изменением свойств типа вызывается данное событие.
        /// </summary>
        public event Action<Account, IFeedableNote> PropertyHasChanged;

        /// <summary>
        /// Значение по умолчанию для <see cref="School"/>, <see cref="University"/>.
        /// </summary>
        public const string DefaultName = "Null";

        string school, university;

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
        /// Школа пользователя.
        /// </summary>
        public string School {
            get => school;
            set {
                if (!IsValidName(value)) {
                    throw new ArgumentException();
                } else if (school == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение школы.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} изменил(-а) школу на {value}.");
                school = value;
            }
        }

        /// <summary>
        /// ВУЗ пользователя.
        /// </summary>
        public string University {
            get => university;
            set {
                if (!IsValidName(value)) {
                    throw new ArgumentException();
                } else if (university == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение ВУЗа.", $"{Owner.Passport.Middlename} {Owner.Passport.Name} изменил(-а) ВУЗ на {value}.");
                university = value;
            }
        }

        #endregion

        #region Конструкторы, On-методы

        /// <summary>
        /// Полный конструктор.
        /// </summary>
        /// <param name="owner">Экземпляр класса <see cref="Account"/>, на который будет ссылаться данный.</param>
        /// <param name="school">Школа пользователя.</param>
        /// <param name="university">ВУЗ пользователя.</param>
        public Education(Account owner, string school, string university)
        {
            Owner = owner ?? throw new ArgumentNullException();
            School = school ?? DefaultName;
            University = university ?? DefaultName;
        }

        /// <summary>
        /// Конструктор с установкой значений свйоств по умолчанию.
        /// </summary>
        /// <param name="owner">Экземпляр класса <see cref="Account"/>, на который будет ссылаться данный.</param>
        public Education(Account owner) : this(owner, null, null) { }

        protected void OnPropertyHasChanged(string title, string description)
        {
            PropertyHasChanged?.Invoke(Owner, new Note(Owner.Id, title, description));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Указывает можно ли использовать входную строку как значение поля <see cref="School"/>, <see cref="University"/>.
        /// </summary>
        /// <param name="name">Входная строка.</param>
        /// <returns>Валидность имени.</returns>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Owner.ToString();
        }

        /// <summary>
        /// Возвращает <see cref="Education.Buffer"/> с копиями значений текущего экземпляра.
        /// </summary>
        /// <returns><see cref="Education.Buffer"/> с копиями значений.</returns>
        public Buffer GetBuffer()
        {
            return new Buffer {
                OwnerId = Owner.Id,
                School = school,
                University = university
            };
        }
        
        #endregion
    }
}
