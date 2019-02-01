namespace SocialNetwork.Core
{
    /// <summary>
    /// Инкапсулирует пользователя в социальной сети.
    /// </summary>
    public class Account
    {
        #region Вложенные структуры

        #endregion

        #region Поля

        #endregion

        #region Свойства

        public string Id {
            get;
            set;
        }

        public Passport Passport {
            get;
            set;
        }

        #endregion

        #region Конструкторы, On-методы

        public Account(string id)
        {
            Id = id;
            Passport = new Passport(this);
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Id;
        }

        #endregion
    }
}
