using System;

namespace fabric.DAL.Models
{
    public class User
    {
        private int _id;
        private string _username;
        private string _passwordHash;
        private string _fullName;
        private bool _isActive;
        private int _roleId;
        private Role _role;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string PasswordHash
        {
            get => _passwordHash;
            set => _passwordHash = value;
        }

        public string FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public int RoleId
        {
            get => _roleId;
            set => _roleId = value;
        }

        public Role Role
        {
            get => _role;
            set => _role = value;
        }
    }
}
