using SocialNetwork.Core.Scope;
using System;

namespace SocialNetwork.Core
{
    /// <summary>
    /// Инкапсулирует личные данные пользователя.
    /// Данный класс является зависимым свойством класса <see cref="Account"/>.
    /// </summary>
    public class Passport
    {
        #region Вложенные структуры

        /// <summary>
        /// Инкапсулирует значимые данные содержащего типа для представления в бинарном виде.
        /// </summary>
        [Serializable]
        public struct Buffer
        {
            public string OwnerId;
            public string Name;
            public string Middlename;
            public string Lastname;
            public DateTime Birthday;
            public Gender Gender;
            public MarritalStatus Status;
        }

        #endregion

        #region Поля

        /// <summary>
        /// Перед изменением свойств типа вызывается данное событие.
        /// </summary>
        public event Action<IFeedableNote> PropertyHasChanged;

        /// <summary>
        /// Значение по умолчанию для <see cref="Name"/>, <see cref="Middlename"/>, <see cref="Lastname"/>.
        /// </summary>
        public const string DefaultName = "Null";

        string name, middlename, lastname;
        DateTime birthday;
        Gender gender;
        MarritalStatus status;

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
        /// Имя пользователя.
        /// </summary>
        public string Name {
            get => name;
            set {
                if (!IsValidName(value)) {
                    throw new ArgumentException();
                } else if (name == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение имени.", $"{Middlename} {Name} изменил(-а) имя на {value}.");
                name = value;
            }
        }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string Middlename {
            get => middlename;
            set {
                if (!IsValidName(value)) {
                    throw new ArgumentException();
                } else if (middlename == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение фамилии.", $"{Middlename} {Name} изменил(-а) фамилию на {value}.");
                middlename = value;
            }
        }

        /// <summary>
        /// Отчество пользователя.
        /// </summary>
        public string Lastname {
            get => lastname;
            set {
                if (!IsValidName(value)) {
                    throw new ArgumentException();
                } else if (lastname == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение отчества.", $"{Middlename} {Name} изменил(-а) отчество на {value}.");
                lastname = value;
            }
        }

        /// <summary>
        /// Дата и время рождения пользователя.
        /// </summary>
        public DateTime Birthday {
            get => birthday;
            set {
                if (birthday == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение дня рождения.", $"{Middlename} {Name} изменил(-а) день рождения на {value.ToShortDateString()}.");
                birthday = value;

                if (value.DayOfYear == DateTime.Now.DayOfYear) {
                    OnPropertyHasChanged("День рождения.", $"Сегодня день рождения пользователя {Middlename} {Name}");
                }
            }
        }

        /// <summary>
        /// Пол пользователя.
        /// </summary>
        public Gender Gender {
            get => gender;
            set {
                if (gender == value) {
                    return;
                }

                OnPropertyHasChanged("Смена пола.", $"{Middlename} {Name} сменил(-а) пол на {value.Descripte()}.");
                gender = value;
            }
        }

        /// <summary>
        /// Семейное положение пользователя.
        /// </summary>
        public MarritalStatus Status {
            get => status;
            set {
                if (status == value) {
                    return;
                }

                OnPropertyHasChanged("Изменение семейного положения.", $"{Middlename} {Name} изменил(-а) семейное положение на {value.Descripte()}.");
                status = value;
            }
        }

        #endregion

        #region Конструкторы, On-методы

        /// <summary>
        /// Полный конструктор.
        /// </summary>
        /// <param name="owner">Экземпляр класса <see cref="Account"/>, на который будет ссылаться данный.</param>
        /// <param name="name">Имя пользователя.</param>
        /// <param name="middlename">Фамилия пользователя.</param>
        /// <param name="lastname">Отчество пользователя.</param>
        /// <param name="birthday">День рождения пользователя.</param>
        /// <param name="gender">Пол пользователя.</param>
        /// <param name="status">Семейное положение пользователя.</param>
        public Passport(Account owner, string name, string middlename, string lastname, DateTime birthday, Gender gender, MarritalStatus status)
        {
            Owner = owner ?? throw new ArgumentNullException();
            Name = name ?? DefaultName;
            Middlename = middlename ?? DefaultName;
            Lastname = lastname ?? DefaultName;
            Birthday = birthday;
            Gender = gender;
            Status = status;
        }

        /// <summary>
        /// Консруктор с установкой значений свйоств по умолчанию.
        /// </summary>
        /// <param name="owner">Экземпляр класса <see cref="Account"/>, на который будет ссылаться данный.</param>
        public Passport(Account owner) : this(owner, null, null, null, DateTime.MinValue, Gender.Female, MarritalStatus.ActivelyLooking) { }

        protected void OnPropertyHasChanged(string title, string description)
        {
            PropertyHasChanged?.Invoke(new Note(Owner.Id, title, description));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Указывает можно ли использовать входную строку как значение поля <see cref="Name"/>, <see cref="Middlename"/> или <see cref="Lastname"/>.
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
            return Middlename + " " + Name;
        }

        /// <summary>
        /// Возвращает <see cref="Passport.Buffer"/> с копиями значений текущего экземпляра.
        /// </summary>
        /// <returns><see cref="Passport.Buffer"/> с копиями значений.</returns>
        public Buffer GetBuffer()
        {
            return new Buffer {
                OwnerId = (string)Owner.Id.Clone(),
                Name = (string)Name.Clone(),
                Middlename = (string)Middlename.Clone(),
                Birthday = Birthday,
                Gender = Gender,
                Status = Status
            };
        }
        
        #endregion
    }
}
