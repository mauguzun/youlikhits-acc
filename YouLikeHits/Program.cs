using AccountManager.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
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

namespace YouLikeHits
{
    class Program
    {
        public const string IMG = "img";
        public static Guid guid = Guid.NewGuid();
        public static string acc = null;
        public static Account selectedAcc;
        static RemoteWebDriver driver;
        public static int defaultNumber = 1;

        static void Main(string[] args)
        {



            CaptchaContext captchaContext = new CaptchaContext();
            Console.WriteLine(captchaContext.Database.Connection.ConnectionString); ;

            Clear();



            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--mute-audio");
                //options.AddArgument("--window-position=-32000,-32000");
                // driver = new ChromeDriver(options);
                driver = new PhantomJSDriver(GetJsSettingsPhantom());
            }
            catch
            {

                //ChromeOptions options = new ChromeOptions();
                //options.AddArgument("--mute-audio");
                //options.AddArgument("--window-position=-32000,-32000");
                //driver = new ChromeDriver(options);
            }


            AccRepo repo = new AccRepo();

            if (args.Count() != 0)
            {
                if (args[0] == "all")
                {
                    // run new instance :)
                    OpenAll(repo);

                }
                else
                {

                    defaultNumber = Int32.Parse(args[0]);
                }


            }
            else
            {
                foreach (var line in repo.Accounts)
                {
                    Console.WriteLine($"{line.Number},{line.Login},{line.Password}");
                }
                Console.WriteLine("pls choose account");

                string number = Console.ReadLine().Trim();


                if (number == "all")
                {
                    OpenAll(repo); Console.ReadLine();

                }
                else if (number == "grab")
                {
                    Grab(repo); Console.ReadLine();
                }
                else if (number == "timer")
                {
                    Timer timer = new Timer(MakeTimer, repo, new TimeSpan(0, 0, 0), new TimeSpan(0, 15, 0));
                    Console.ReadLine();
                }

                defaultNumber = 1;
                Int32.TryParse(number, out defaultNumber);

            }


            selectedAcc = repo.Accounts.Where(y => y.Number == defaultNumber).FirstOrDefault();
            Login();
            Console.Title = selectedAcc.Login;


            //  Console.WriteLine("1.pinterest \n 2.youtube");

            //if (Console.ReadLine().Trim() == "1")
            //{
            //Pinterest p = new Pinterest(driver);
            //p.Login();
            //p.Follow();

            Youtube youtube = new Youtube(driver);
            youtube.Follow();
        }

        private static PhantomJSDriverService GetJsSettingsPhantom()
        {
            var serviceJs = PhantomJSDriverService.CreateDefaultService();
            serviceJs.HideCommandPromptWindow = true;
            serviceJs.IgnoreSslErrors = true;
            return serviceJs;
        }

        private static void MakeTimer(object state)
        {
            KillAllPhantom();
            driver = new PhantomJSDriver(GetJsSettingsPhantom());
            AccRepo repo = (AccRepo)state;
            Grab(repo);
            Console.ReadLine();
        }

        private static void KillAllPhantom()
        {
            var proccess = Process.GetProcesses();
            foreach (Process pr in proccess)
            {

                var x = pr.ProcessName;
                if (pr.ProcessName.ToLower().Contains("phantom"))
                    pr.Kill();

            }

        }

        private static void Login()
        {
            driver.Url = "http://youlikehits.com";
            driver.FindElementById("username").SendKeys(selectedAcc.Login);
            driver.FindElementById("password").SendKeys(selectedAcc.Password);
            driver.FindElementByCssSelector("input[value=Login]").Click();
        }

        private static void Grab(AccRepo repo)
        {
            foreach (Account acc in repo.Accounts)
            {
                try
                {
                    selectedAcc = acc;
                    Login();

                    YoulikeHits sendAccount = new YoulikeHits();
                    sendAccount.Login = acc.Login;
                    sendAccount.Password = acc.Password;




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
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string result = js.Serialize(sendAccount);

                    UpdateServer(result);



                    if (driver.FindElementsByCssSelector("center a[href = 'bonuspoints.php']") != null && driver.FindElementsByCssSelector("center a[href = 'bonuspoints.php']").Count != 0)
                    {
                        var x = driver.FindElementsByCssSelector("center a[href = 'bonuspoints.php']");
                        System.Diagnostics.Process.Start("YouLikeHits.exe", acc.Number.ToString());
                    }

                    driver.Url = "http://youlikehits.com/logout.php";

                }

                catch (Exception e)
                {

                    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    screenshot.SaveAsFile("cantlogin.jpg", ImageFormat.Jpeg);

                    Console.WriteLine(e.Message);
                    Console.WriteLine("omg");
                }
                finally
                {
                    driver.Url = "http://youlikehits.com/logout.php";
                }

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
                File.Delete($"{Program.IMG}\\{Program.guid}.jpg");
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
