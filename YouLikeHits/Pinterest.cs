using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YouLikeHits
{
    class Pinterest
    {
       
        static string acc;
        RemoteWebDriver driver;

        public Pinterest(RemoteWebDriver driver)
        {
            this.driver = driver;
        }


        public bool Login(string accountPath)
        {
            try
            {
                driver.Url = "http://pinterest.com";
                List<DCookie> dCookie;
                using (var reader = new StreamReader(accountPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(List<DCookie>),
                        new XmlRootAttribute("list"));
                    dCookie = (List<DCookie>)deserializer.Deserialize(reader);
                }

                foreach (var cookie in dCookie)
                {
                    driver.Manage().Cookies.AddCookie(cookie.GetCookie());
                }



                Console.WriteLine("logined");
                Thread.Sleep(new TimeSpan(0, 0, 5));

                if (driver.Manage().Cookies.GetCookieNamed("_auth").Value.ToString() == "1")
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch
            {
                return false;
            }


        }

        public void Follow()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            driver.Url = "https://youlikehits.com/pinterest.php";

            var follows = driver.FindElementsByClassName("followbutton");
            follows[0].Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));




            while (true)
            {
                try

                {
                    driver.FindElementByCssSelector(".likebutton").Click();


                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    if (driver.FindElementsByCssSelector(".CreatorHeaderContainer").Count != 0)
                    {
                        var divs = driver.FindElementsByTagName("div");
                        foreach (var div in divs)
                        {

                            if (div.GetAttribute("class").Contains("CreatorFollowButton"))
                            {
                                div.Click();
                                break;
                            }

                        }
                    }


                    else
                    {
                        var buttons = driver.FindElementsByTagName("button");
                        foreach (var button in buttons)
                        {
                            if (button.Text.ToLower().Contains("follow"))
                            {
                                button.Click();
                                break;
                            }
                        }
                    }

                    driver.SwitchTo().Window(driver.WindowHandles.First());
                    driver.FindElementById("hidethis").Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch
                {

                }

            }
        }

    }
}

