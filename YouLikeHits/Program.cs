using AccountManager.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using OpenQA.Selenium.Support.UI;
namespace YouLikeHits
{
    class Program
    {
        public static string password = File.ReadAllText("Pass.txt").Trim();


        public const string IMG = "img";
        private const string accFile = "account_in_account.txt";
        public static Guid guid = Guid.NewGuid();
        public static string acc = null;
        public static Account selectedAcc;
        static RemoteWebDriver driver;
        public static int defaultNumber = 1;
        static  List<GUI.Account> already = new List<GUI.Account>();


        static void Main(string[] args)
        {


            // Clear();
            AccRepo repo = new AccRepo();
            //var cookiesAcc = new AccountManager().Accounts();
            //List<Account> newList = new List<Account>();

            //foreach (Account acc in repo.Accounts)
            //{
            //    if (cookiesAcc.Contains(acc.Login))
            //    {
            //        newList.Add(acc);
            //    }
            //}


            //repo.Accounts = newList;
            //repo.Save();

            if (args.Count() != 0)
            {


                defaultNumber = Int32.Parse(args[0]);

            }
            else
            {
                foreach (var line in repo.Accounts)
                {
                    Console.WriteLine($"{line.Number},{line.Login},{line.Password}");
                }
                Console.WriteLine("pls choose login, grab, login,clear,account");

                string number = Console.ReadLine().Trim();

                if (number == "login")
                {
                    AccountManager manager = new AccountManager();
                    manager.LoginAll();
                }
                else if (number == "account")
                {
                    ColectPinterest(repo);
                    Console.ReadLine();
                }
                else if (number == "grab")
                {
                    Grab(repo);
                    Console.ReadLine();
                }

                else if (number == "clear")
                {
                    Clear();
                }

                defaultNumber = 1;
                Int32.TryParse(number, out defaultNumber);

            }
            Console.WriteLine("en  moment");

            while (true)
            {
                AccountManager accountManager = new AccountManager();
                selectedAcc = repo.Accounts.Where(y => y.Number == defaultNumber).FirstOrDefault();
                driver = accountManager.GetLoginedDriver(selectedAcc);
                if (accountManager.Logined(selectedAcc))
                {
                    Console.Title = selectedAcc.Login;
                    Youtube youtube = new Youtube(driver);
                    try
                    {
                        youtube.Follow();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                else
                {
                    Console.Title = "user can`t login" + selectedAcc.Login;
                    break;
                }
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            Console.ReadKey();
            driver.Quit();
            Console.ReadKey();




        }


        private static void MakeTimer(object state)
        {

            driver = ChromeInstance.Driver();
            AccRepo repo = (AccRepo)state;
            Grab(repo);
            Console.ReadLine();
        }



        private static bool Login()
        {
            driver.Url = "https://www.youlikehits.com/login.php";
            driver.FindElementById("username").SendKeys(selectedAcc.Login.Trim());
            driver.FindElementById("password").SendKeys(password);
            driver.FindElementByCssSelector("input[value=Login]").Click();


            driver.Url = "https://www.youlikehits.com/stats.php";

            var cookies = driver.Manage().Cookies.AllCookies;
            if (cookies.Where(x => x.Name == "tfuser").FirstOrDefault() != null)
                return true;


            return false;

        }

        private static void Grab(AccRepo repo)
        {


            foreach (Account acc in repo.Accounts)
            {


                try
                {
                    Console.WriteLine($"check {acc.Login}");

                    YoulikeHits sendAccount = new YoulikeHits();
                    driver = new AccountManager().GetLoginedDriver(acc);
                    driver.Url = "https://youlikehits.com/addpinterest.php";

                    var cardDivs = driver.FindElementsByCssSelector(".cards");
                    foreach (RemoteWebElement item in cardDivs)
                    {
                        var userName = item.FindElementsByCssSelector("b");
                        sendAccount.YouPinterests.Add(new YouPinterest() { AccountUserName = userName[0].Text });

                    }
                    driver.Url = "https://youlikehits.com/stats.php";

                    if (driver.FindElementByCssSelector("center a[href = 'buypoints.php'] font") != null)
                    {
                        Console.WriteLine($"{acc.Login}:{driver.FindElementByCssSelector("center a[href = 'buypoints.php'] font").Text}");
                        sendAccount.Point = driver.FindElementByCssSelector("center a[href = 'buypoints.php'] font").Text;
                    }





                    var x = driver.FindElementsByCssSelector("center a[href = 'bonuspoints.php']");
                    System.Diagnostics.Process.Start("YouLikeHits.exe", acc.Number.ToString());


                    driver.Url = "http://youlikehits.com/logout.php";

                }

                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    Console.WriteLine("omg");
                }
                finally
                {
                    driver.Url = "http://youlikehits.com/logout.php";
                }

            }
        }
        private static void ColectPinterest(AccRepo repo)
        {
           
            while (true)
            {
                Console.WriteLine("Select account");
                string input = Console.ReadLine();


                AccountManager accountManager = new AccountManager();

                if (input == "all")
                {
                    for (int i = 1; i < repo.Accounts.Count() ; i++)
                    {
                        selectedAcc = repo.Accounts.Where(y => y.Number == i).FirstOrDefault();
                        AddPinterestAcciunt(selectedAcc);
                    }
                    Console.WriteLine("end");
                    Console.ReadKey();
                }

              
                defaultNumber = int.Parse(input);
                selectedAcc = repo.Accounts.Where(y => y.Number == defaultNumber).FirstOrDefault();

                AddPinterestAcciunt(selectedAcc); 
            }
         
        }
        private static void AddPinterestAcciunt(Account selectedAcc)
        {
            AccountManager accountManager = new AccountManager();
            driver = accountManager.GetLoginedDriver(selectedAcc);

            if (accountManager.Logined(selectedAcc))
            {
                driver.Url = "https://www.youlikehits.com/addpinterest.php";

                var links = driver.FindElementsByCssSelector(".cards a");
                for (int i = 0; i < links.Count; i++)
                {
                    try
                    {
                        driver.FindElementByCssSelector(".cards a").Click();


                        driver.Url = "https://www.youlikehits.com/addpinterest.php";
                    }

                    catch { }
                }


                var account = GUI.Account.GetAccountExtraInfo();



                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                string title = (string)js.ExecuteScript("document.getElementById('addpage').setAttribute('style', '')");
                //   driver.FindElementByCssSelector(".mainfocusbody a").Click();



                int addedAccount = 0;
                var noobies = account.Where(x => x.Followers == 0);
                int count = 0;
                foreach (var item in noobies)
                {
                    try
                    {
                        if (!already.Contains(item) && item.Followers != null)
                        {
                            already.Add(item);
                            driver.FindElementByCssSelector("#url").Clear();
                            driver.FindElementByCssSelector("#url").SendKeys(item.UserName);
                            driver.FindElementByCssSelector("#verifybutton").Click();
                            var x = driver.FindElementByCssSelector("#verify .mainfocusheader").Text;
                            if (!driver.FindElementByCssSelector("#verify .mainfocusheader").Text.ToLower().Contains("ops"))
                            {
                                addedAccount++;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("catch" + ex.Message);
                    }
                    count++;

                    if (addedAccount == 10 | count > 40)
                        break;
                }
                driver.Url = "https://www.youlikehits.com/addpinterest.php";
                var select = driver.FindElementsByCssSelector(".cards select");
                foreach (var item in select)
                {
                    var selectElement = new SelectElement(item);
                    selectElement.SelectByValue("10");
                    Console.WriteLine("done");
                }
            }
            else
            {
                Console.Title = "user can`t login" + selectedAcc.Login;

            }
        }


        private static void OpenAll(AccRepo repo)
        {
            foreach (Account acc in repo.Accounts)
            {
                Thread.Sleep(new TimeSpan(0, 0, 7));
                System.Diagnostics.Process.Start("YouLikeHits.exe", acc.Number.ToString());

            }
        }

        private static void Clear()
        {
            if (!Directory.Exists(IMG))
                Directory.CreateDirectory(IMG);

            foreach (var file in Directory.GetFiles(IMG))
            {
                try
                {
                    File.Delete(file);
                    Console.WriteLine("file deleted");
                }
                catch
                {
                    Console.WriteLine("whoops ");
                }


            }
        }

        //else
        //{

        //}
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                driver.Quit();
                Console.WriteLine("exit");
            }
            catch
            {

            }


        }

        public static bool UpdateServer(string json)
        {

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://denisacc.somee.com/Api/YouApi/1");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine("post response");
                    Console.WriteLine(responseText);
                    //Now you have your response.
                    //or false depending on information in the response
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
    }


}
