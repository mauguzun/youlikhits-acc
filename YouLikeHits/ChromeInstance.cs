using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouLikeHits
{
    public class ChromeInstance
    {

        public static RemoteWebDriver Driver(bool visible = false)
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();

            if (!visible)
                options.AddArguments("headless");

            /*            148.217.94.54:3128
            132.145.89.166:3128
            163.172.190.160:8811
            5.189.134.87:3128
            5.189.134.87:3128
            51.158.172.165:8811
            51.158.172.165:8811*/

            options.AddArgument("--proxy-server=51.158.172.165:8811");  //
            //

            options.AddArgument("--window-size=1920,4080");
            ChromeDriver driver = new ChromeDriver(driverService, options);
        
           // PhantomJSOptions op = new PhantomJSOptions();
           // op.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:36.0) Gecko/20100101 Firefox/36.0 WebKit");

           //// RemoteWebDriver driver = new PhantomJSDriver(op);
      
            return driver;
        }
    }
}
