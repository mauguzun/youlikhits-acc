﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace YouLikeHits
{
    public class AccountManager
    {
        private string _dirPath = "cookies";
        private List<Account> _accounts;
        RemoteWebDriver driver;

        public AccountManager()
        {
            _accounts = new AccRepo().Accounts;
            if (!Directory.Exists(this._dirPath))
            {
                Directory.CreateDirectory(this._dirPath);
            }
        }

        public void LoginAll()
        {
            foreach (var acc in _accounts)
            {
                CheckAccount(acc);
                Console.WriteLine("correct y? / n ?");
                if (Console.ReadLine().Trim() == "y")
                {
                    this.Save(acc);
                }
                driver.Quit();

            }
        }

        private void CheckAccount(Account acc)
        {
            this.driver = new ChromeDriver();
            driver.Url = "https://www.youlikehits.com/login.php";
            driver.FindElementById("username").SendKeys(acc.Login);
            driver.FindElementById("password").SendKeys(acc.Password);
        }

        private void Save(Account acc)
        {
            var xs = driver.Manage().Cookies.GetCookieNamed("_auth");
            var cookies = driver.Manage().Cookies.AllCookies;

            List<DCookie> listDc = new List<DCookie>();
            foreach (OpenQA.Selenium.Cookie cookie in cookies)
            {
                //_auth=1
                var dCookie = new DCookie();
                dCookie.Domain = cookie.Domain;
                dCookie.Expiry = cookie.Expiry;
                dCookie.Name = cookie.Name;
                dCookie.Path = cookie.Path;
                dCookie.Value = cookie.Value;
                dCookie.Secure = cookie.Secure;

                listDc.Add(dCookie);
            }
            XmlSerializer ser = new XmlSerializer(typeof(List<DCookie>),new XmlRootAttribute("list"));
 
            using (FileStream fs = new FileStream(this._dirPath + "/" + this.FileName(acc) , FileMode.Create))
            {
                ser.Serialize(fs, listDc);
            }
        }

        private string FileName(Account acc )
        {
            return acc.Login + ".xml";
        }

        public List<string> Accounts()
        {
            List<string> res = new List<string>();
            foreach(string name in  Directory.GetFiles(this._dirPath))
            {
              
                res.Add(Path.GetFileNameWithoutExtension(name));
            }
            return res;
        }

         

        public RemoteWebDriver GetLoginedDriver(Account acc)
        {


            //   RemoteWebDriver phantomJs = new PhantomJSDriver();
            RemoteWebDriver phantomJs = ChromeInstance.Driver();
            phantomJs.Url = "http://youlikehits.com/";
            List<DCookie> dCookie;
            using (var reader = new StreamReader(this._dirPath + "/" + acc.Login + ".xml"))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<DCookie>),
                    new XmlRootAttribute("list"));
                dCookie = (List<DCookie>)deserializer.Deserialize(reader);
            }
          
            foreach (var cookie in dCookie)
            {
                phantomJs.Manage().Cookies.AddCookie(cookie.GetCookie());
            }

          

         
            phantomJs.Url = "https://youlikehits.com/";
               OpenQA.Selenium.Screenshot screenshot = ((ITakesScreenshot)phantomJs).GetScreenshot();
            screenshot.SaveAsFile("cookie.jpg", ImageFormat.Jpeg);
            return phantomJs;

        }

        internal bool Logined(Account selectedAcc)
        {
            return true;
        }
    }
}
