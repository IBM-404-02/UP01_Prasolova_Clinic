using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;

namespace UP01_Prasolova_Clinic
{
    internal class Core
    {
        public static DB_Prasolova_ClinicEntities Context = new DB_Prasolova_ClinicEntities();
        public static User CurrentUser;

        public static string GetHash(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return string.Concat(bytes.Select(b => b.ToString("X2")));
            }
        }

        public static User Authenticate(string login, string password)
        {
            string hash = GetHash(password);
            return Context.Users.FirstOrDefault(u => u.Login == login && u.Password == hash);
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static string CheckPassword(string password)
        {
            if (password.Length < 6)
                return "Пароль должен содержать не менее 6 символов";

            bool hasDigit = false;

            foreach (char c in password)
            {
                if (c >= '0' && c <= '9')
                    hasDigit = true;
                else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return "Пароль может содержать только английские буквы и цифры";
            }

            if (!hasDigit)
                return "Пароль должен содержать хотя бы одну цифру";

            return null;
        }

    }

}
