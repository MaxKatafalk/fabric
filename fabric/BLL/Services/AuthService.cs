using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.BLL.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public AuthService()
        {
            _db = new AppDbContext();
            _db.Database.EnsureCreated();
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            string hash = ComputeHash(password);
            User user = _db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash && u.IsActive);
            return user;
        }

        public bool Register(string username, string password, string fullName, int roleId)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            bool exists = _db.Users.Any(u => u.Username == username);
            if (exists)
            {
                return false;
            }

            User user = new User
            {
                Username = username,
                PasswordHash = ComputeHash(password),
                FullName = fullName,
                IsActive = true,
                RoleId = roleId
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return true;
        }

        private string ComputeHash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public bool DeleteUser(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return false;

                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
        }
    }
}

