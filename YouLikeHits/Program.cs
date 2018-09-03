using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YouLikeHits
{
    class Program
    {
        public const string IMG = "img";
        public static Guid guid = Guid.NewGuid();
        public static string acc = null;
        public static Account selectedAcc;
        static RemoteWebDriver driver;


        static void Main(string[] args)
        {
            Clear();

          

            var serviceJs = PhantomJSDriverService.CreateDefaultService();
            serviceJs.HideCommandPromptWindow = true;

            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--mute-audio");
            //options.AddArgument("--window-position=-32000,-32000");
            try
            {
                driver = new PhantomJSDriver(serviceJs);
            }
            catch
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--mute-audio");
                options.AddArgument("--window-position=-32000,-32000");
                driver = new ChromeDriver(options);
            }


            AccRepo repo = new AccRepo();



         
            int defaultNumber = 1;
            if (args.Count() != 0)
            {
                if(args[0] == "all")
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
                    OpenAll(repo);
                }
                defaultNumber = 1;
                Int32.TryParse(number, out defaultNumber);

            }


            selectedAcc = repo.Accounts.Where(y => y.Number == defaultNumber).FirstOrDefault();

            driver.Url = "http://youlikehits.com";
            driver.FindElementById("username").SendKeys(selectedAcc.Login);
            driver.FindElementById("password").SendKeys(selectedAcc.Password);
            driver.FindElementByCssSelector("input[value=Login]").Click();

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

        private static void OpenAll(AccRepo repo)
        {
            foreach (var line in repo.Accounts)
            {
                System.Diagnostics.Process.Start("YouLikeHits.exe", line.Number.ToString());

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




    }
}
