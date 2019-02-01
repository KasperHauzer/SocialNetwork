namespace SocialNetwork.Core.Scope
{
    public enum MarritalStatus
    {
        ActivelyLooking,
        InLove,
        ItsComplicated,
        Married,
        Meeting
    }

    public static class MarritalStatusExtansion
    {
        public const string DefaultDescription = "NULL";

        /// <summary>
        /// Возвращает описание элемента перечислителя.
        /// </summary>
        /// <param name="status">Элемент перечилителя.</param>
        /// <returns>Описание элемента перечислителя.</returns>
        public static string Descripte(this MarritalStatus status)
        {
            switch (status) {
                case MarritalStatus.ActivelyLooking: return "в активном поиске";
                case MarritalStatus.InLove: return "влюблен";
                case MarritalStatus.ItsComplicated: return "все сложно";
                case MarritalStatus.Married: return "в браке";
                case MarritalStatus.Meeting: return "в отношениях";
                default: return DefaultDescription;
            }
        }
    }
}
