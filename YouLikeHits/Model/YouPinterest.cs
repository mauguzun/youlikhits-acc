using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AccountManager.Models
{
    public class YouPinterest
    {

        [Key]
        public int AccountUserNameID { get; set; }
        public string AccountUserName { get; set; }

        public YoulikeHits YoulikeHits { get; set; }
        public  int ? YoulikeHitsID { get; set; }


    }
}