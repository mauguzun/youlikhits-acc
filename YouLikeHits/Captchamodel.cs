using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{


    class CaptchaModel
    {
        private IMongoCollection<Captcha> captchas;
        public CaptchaModel()
        {

         
            this.LoadCollection();
        }

        public Captcha FindCaptcha(byte[] hash)
        {
            return this.captchas.Find(x => x.Hash == hash).FirstOrDefault();
          
        }
        public bool Add(Captcha cap)
        {
            try
            {
                this.captchas.InsertOne(cap);
                this.LoadCollection();
                return true;
            }
            catch
            {
                return false;
            }
          
        }

        private void LoadCollection()
        {
            string connectionString = "mongodb+srv://denis:penis@denispenis-gv66s.mongodb.net/test?retryWrites=true";

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("captcha");
            captchas = database.GetCollection<Captcha>("captchas");

            
        }
    }

}
