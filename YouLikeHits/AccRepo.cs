using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{
    class AccRepo
    {
        public List<Account> Accounts { get; set; }

        public AccRepo()
        {
            this.Accounts = new List<Account>();

            string[] lines = File.ReadAllLines("Acc.txt");
            int num   = 1; 
            foreach(string line in lines)
            {
                string[] acc = line.Split(':');
                Accounts.Add(new Account(acc[0], acc[1], num));
                num++;

            }

        }
    }
}
