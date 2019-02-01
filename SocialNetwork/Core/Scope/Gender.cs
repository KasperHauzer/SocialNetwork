namespace SocialNetwork.Core.Scope
{
    public enum Gender
    {
        Female,
        Male
    }

    public static class GenderExtansion
    {
        public const string DefaultDescription = "NULL";

        /// <summary>
        /// Возвращает описание элемента перечислителя.
        /// </summary>
        /// <param name="gender">Элемент перечислителя.</param>
        /// <returns>Описание элемента перечислителя.</returns>
        public static string Descripte(this Gender gender)
        {
            switch (gender) {
                case Gender.Female: return "женский пол";
                case Gender.Male: return "мужской пол";
                default: return DefaultDescription;
            }
        }
    }
}
