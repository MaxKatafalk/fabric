using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using fabric.DAL;
using fabric.DAL.Models;
using fabric.Forms;

namespace fabric
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                if (!db.Users.Any(u => u.Username == "admin"))
                {
                    string hash;
                    using (SHA256 sha = SHA256.Create())
                    {
                        byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes("admin"));
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in data) sb.Append(b.ToString("x2"));
                        hash = sb.ToString();
                    }

                    db.Users.Add(new User
                    {
                        Username = "admin",
                        PasswordHash = hash,
                        FullName = "Administrator",
                        IsActive = true,
                        RoleId = 4
                    });
                    db.SaveChanges();
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
