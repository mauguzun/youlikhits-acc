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
        const string Path = "Acc.txt";
        public List<Account> Accounts { get; set; }

        public AccRepo()
        {
            this.Accounts = new List<Account>();

           
            string[] lines = File.ReadAllLines(Path);
            int num   = 1; 
            foreach(string line in lines)
            {
                string[] acc = line.Split(':');
                Accounts.Add(new Account(acc[0], acc[1], num));
                num++;

            }

        }
        public void Save()
        {
            if (this.Accounts != null)
            {
                File.Delete(Path);
                foreach(Account acc in Accounts)
                {
                    File.AppendAllText(Path, acc.Login + ":" + acc.Password + Environment.NewLine);
                }
            }
        }
    }
}
