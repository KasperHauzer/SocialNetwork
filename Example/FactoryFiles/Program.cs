using SocialNetwork.Core;
using SocialNetwork.Core.Scope;
using System.Drawing;
using static System.Console;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Подготовительная часть

            Title = "Пример использования библиотеки SocialNetwork.";

            Account first = new Account("killmonger");
            Account second = new Account("superUser");
            Account third = new Account("axelerot");

            first.FollowigPropertyHasChanged += ShowAction;
            first.FollowingPhotoHasChanged += ShowAction;
            first.FollowingPhotoHasRemoved += ShowAction;
            first.FollowingGotFollower += ShowAction;
            first.FollowingGotFollowing += ShowAction;
            first.FollowingPublishedNews += ShowAction;
            first.FollowingRemovedNews += ShowAction;

            second.FollowigPropertyHasChanged += ShowAction;
            second.FollowingPhotoHasChanged += ShowAction;
            second.FollowingPhotoHasRemoved += ShowAction;
            second.FollowingGotFollower += ShowAction;
            second.FollowingGotFollowing += ShowAction;
            second.FollowingPublishedNews += ShowAction;
            second.FollowingRemovedNews += ShowAction;

            third.FollowigPropertyHasChanged += ShowAction;
            third.FollowingPhotoHasChanged += ShowAction;
            third.FollowingPhotoHasRemoved += ShowAction;
            third.FollowingGotFollower += ShowAction;
            third.FollowingGotFollowing += ShowAction;
            third.FollowingPublishedNews += ShowAction;
            third.FollowingRemovedNews += ShowAction;

            #endregion Подготовительная часть

            first.Passport.Name = "Макалистер";
            first.Passport.Middlename = "Алистер";

            second.Passport.Name = "Вонка";
            second.Passport.Middlename = "Вилли";

            third.Passport.Name = "Валентина";
            third.Passport.Middlename = "Терешкова";

            //axelerot подписывается на killmonger & superUser.
            //superUser подписывается на killmonger.
            third.SubscribeTo(first);
            third.SubscribeTo(second);
            second.SubscribeTo(first);

            first.Education.University = "HSE Perm";
            var news = new Note(
                first.Id,
                "Здесь могла бы быть умная мысль.",
                "Поешь еще этих вкусных французских булочек. ;)"
            );
            first.Profile.AddNews(news);

            second.Profile.Photo = (Bitmap)Image.FromFile(@"C:\Users\Arsanukaev\OneDrive\Pictures\php.jpg");
            second.Passport.Birthday = System.DateTime.Now;

            first.Profile.RemoveNews(news);

            third.Unsubscribe(first);

            first.Passport.Status = MarritalStatus.Married;

            first.Passport.Name = "Степа";

            ReadLine();
        }

        static void ShowAction(Account sender, IFeedableNote note)
        {
            ForegroundColor = System.ConsoleColor.Green;
            WriteLine($"Оповещение для {sender}");
            ForegroundColor = System.ConsoleColor.White;

            WriteLine($"{note.Title}\n{note.Description}\n{note.Time.ToShortTimeString()}");

            WriteLine();
        }

        static void ShowAction(Account sender, Note note)
        {
            ForegroundColor = System.ConsoleColor.Green;
            WriteLine($"Оповещение для {sender}");
            ForegroundColor = System.ConsoleColor.White;

            WriteLine($"{note.Title}\n{note.Description}\n{note.Time.ToShortTimeString()}");

            if (note.Data != null) {
                WriteLine($"\t***\n{note.Data.ToString()}\n\t***");
            }

            WriteLine();
        }
    }
}
