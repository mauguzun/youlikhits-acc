using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManager.Models
{
    public class YoulikeHits
    {
        public int Id { get; set; }
        public string Point { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

         
        public ICollection<YouPinterest> YouPinterests { get; set; }

        public YoulikeHits ()
        {
            YouPinterests = new List<YouPinterest>();
        }
          
    }
}