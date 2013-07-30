using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TimeMangement.Models
{
    public enum UserRole
    {
        Orders,
        Admin,
        Stats
    }

    /// <summary>
    /// A site-wide user
    /// </summary>
    public class User
    {
        public string Id { get; set; }
        public List<UserRole> Roles { get; set; }

        private const string ConstantSalt = "t2fy^cerv3f4#";

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        protected string HashedPassword { get; private set; }

        public bool Enabled { get; set; }
        public string SendOutKey { get; set; }

        public DateTimeOffset DateJoined { get; set; }
        public DateTimeOffset LastSeen { get; set; }

        private Guid _passwordSalt;
        private Guid PasswordSalt
        {
            get
            {
                if (_passwordSalt == Guid.Empty)
                    _passwordSalt = Guid.NewGuid();
                return _passwordSalt;
            }
            // ReSharper disable UnusedMember.Local
            set { _passwordSalt = value; }
            // ReSharper restore UnusedMember.Local
        }

        public User()
        {
            Roles = new List<UserRole>();
        }

        public User SetPassword(string pwd)
        {
            HashedPassword = GetHashedPassword(pwd);
            return this;
        }

        private string GetHashedPassword(string pwd)
        {
            string hashedPassword;
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(
                    PasswordSalt.ToByteArray().Concat(
                        Encoding.Unicode.GetBytes(PasswordSalt + pwd + ConstantSalt)
                        ).ToArray()
                    );

                hashedPassword = Convert.ToBase64String(computedHash);
            }
            return hashedPassword;
        }

        public bool ValidatePassword(string maybePwd)
        {
            return HashedPassword == GetHashedPassword(maybePwd);
        }

        public static string FullId(string login)
        {
            if (login == null)
                return null;

            return "users/" + login;
        }
    }
}