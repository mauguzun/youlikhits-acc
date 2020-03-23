using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GUI
{
    public class Account

    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }


        public int? Followers { get; set; } = null;
        public int? Follow { get; set; } = null;
        public string Boards { get; set; } = null;

        private string proxie;
        public string Proxie
        {
            get { return proxie; }
            set
            {
                if (value != null)
                    proxie = value.Replace(':', '_');
            }
        }

        public string Cookie { get; set; } = null;

        public string Group { get; set; } = null;

        public string Status { get; set; } = null;

        public override string ToString()
        {
            return this.Email + ':' + this.Password + ':' + this.UserName + ":" + this.FullName + ":" + this.Proxie + ":" + this.Followers + ":" + this.Follow + ":" + this.Boards + ":" + this.Group + ":" + this.Status;
        }

        public static List<Account> GetAccountExtraInfo()
        {
            List<Account> acounts = new List<Account>();

            foreach (string line in File.ReadAllLines(@"C:\my_work_files\pinterest\full_info_copy.txt"))
            {
                string[] splited = line.Split(':');
                var acc = new Account()
                {
                    Email = splited[0],
                    Password = splited[1],
                    UserName = splited[2],


                };
                if (splited.Count() > 3)
                {

                    acc.Followers = Int32.Parse(splited[6] );

                }
                acounts.Add(acc);
            }


            return acounts;
        }


    }



}
