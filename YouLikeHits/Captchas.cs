using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{


    class Captcha
    {

        public string Result { get; set; }
        public ObjectId Id { get; set; }

        public byte[] Hash { get; set; }
    }


}
