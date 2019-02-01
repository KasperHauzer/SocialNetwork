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
            protected set;
        }

        #endregion

        #region Конструкторы, On-методы

        public Account(string id)
        {
            Id = id;
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
