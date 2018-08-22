using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YouLikeHits
{
    class Pinterest
    {
        static string acc;
        ChromeDriver driver;
        public Pinterest(ChromeDriver driver)
        {
            this.driver = driver;
        }


        public void Login()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("login:passwod");
                    string result = Console.ReadLine().Trim();

                    string[] emailPass = result.Split(':');

                    driver.Url = "https://pinterest.com/login";
                    driver.FindElementById("email").SendKeys(emailPass[0]);
                    driver.FindElementById("password").SendKeys(emailPass[1]);
                    driver.FindElementByCssSelector("button.red").Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    Console.WriteLine("ok?");
                    if (Console.ReadLine().Contains("y"))
                        break;
                }
                catch
                {

                }

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

