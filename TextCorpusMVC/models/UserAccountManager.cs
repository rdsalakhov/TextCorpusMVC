using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Data.Entity;

namespace TextCorpusMVC.Models
{
    public class UserAccountManager
    {
        static public void SignUpUser(string login, string password)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var users = db.UserSet.Where(x => x.Login == login);
                if (users.Count() > 0)
                    throw new UserAlreadyExistException();
                string saltedPassword = HashPassword(password + GetUserSalt(login));
                var newUser = new User();
                newUser.Login = login;
                newUser.Password = saltedPassword;
                newUser.IsAdmin = false;
                db.UserSet.Add(newUser);
                db.SaveChanges();
            }
        }

        public static int SignInUser(string login, string password)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                string saltedPassword = HashPassword(password + GetUserSalt(login));               
                var users = db.UserSet.Where(x => x.Login == login && x.Password == saltedPassword);
                if (users.Count() != 0)
                {
                    return users.First().Id;
                }
                else
                {
                    throw new InvalidPasswordException();
                }
            }
        }

        private static string HashPassword(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(password));
            string result = BitConverter.ToString(bytes).Replace("-", string.Empty);
            return result.ToLower();
        }

        private static string GetUserSalt(string login)
        {
            Random rnd = new Random(login.GetHashCode());

            string salt = (Math.Round(rnd.NextDouble(), 5) * 10).ToString();
            return salt;
        }

        public static void GrantAdminStatus(int userId)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var user = db.UserSet.Where(x => x.Id == userId).First();
                user.IsAdmin = true;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();              
            }
        }

        public static bool GrantAccessToText(int userId, int textId)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var user = db.UserSet.Where(x => x.Id == userId).First();
                var text = db.TextSet.Where(x => x.Id == textId).First();

                var newAccess = new UserAccess();
                newAccess.User = user;
                newAccess.Text = text;

                if (db.UserAccessSet.Where(x => x.User == user && x.Text == text).Count() > 0)
                {
                    return false;
                }
                db.UserAccessSet.Add(newAccess);
                db.SaveChanges();
                return true;
            }
        }

        public static bool RemoveAccessToText(int userId, int textId)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var user = db.UserSet.Where(x => x.Id == userId).First();
                var text = db.TextSet.Where(x => x.Id == textId).First();

                if (db.UserAccessSet.Where(x => x.User == user && x.Text == text).Count() == 0)
                {
                    return false;
                }
                var access = db.UserAccessSet.Where(x => x.User == user && x.Text == text).First();
                db.UserAccessSet.Remove(access);
                db.SaveChanges();
                return true;
            }
        }

        public static void DeleteAccount(int userId)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var userToRemove = db.UserSet.Where(x => x.Id == userId).First();
                db.UserSet.Remove(userToRemove);
                db.SaveChanges();               
            }
        }
    }

    [Serializable]
    internal class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException()
        {
        }

        public UserAlreadyExistException(string message) : base(message)
        {
        }

        public UserAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
        {
        }

        public InvalidPasswordException(string message) : base(message)
        {
        }

        public InvalidPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}