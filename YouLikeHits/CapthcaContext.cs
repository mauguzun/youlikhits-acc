using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{
    

        class CaptchaContext : DbContext
        {
            public CaptchaContext()
            : base("Captchas")
            { }

            public DbSet<Captcha> Captchas { get; set; }
        }

}
