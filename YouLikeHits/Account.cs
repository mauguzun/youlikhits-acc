using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{
    class Account
    {

        public string Login { get; set; }
        public string Password { get; set; }

        public int Number { get; set; }

        public bool Selected { get; set; } = false;

        public Account(string login,string password,int number )
        {
            this.Login = login;
            this.Password = password;
            Number = number;
        }
    }
}
