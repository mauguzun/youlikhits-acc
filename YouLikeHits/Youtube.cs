using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using OpenQA.Selenium;
using System.IO;
using System.Drawing.Imaging;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;

namespace YouLikeHits
{
    class Youtube
    {
        PhantomJSDriver driver;
        string last = null;
        CaptchaContext db;
        bool bonus = false;
        byte[] ms;
        int attemps = 0;


        System.Drawing.Bitmap bit;
        public Youtube(PhantomJSDriver driver)
        {

            this.driver = driver;
            db = new CaptchaContext();
            if (bonus == false)
            {
                try
                {
                    driver.Url = "https://youlikehits.com/bonuspoints.php";
                    driver.FindElementByCssSelector(".buybutton").Click();

                    Console.WriteLine("Bonuses");

                    bonus = true;
                }
                catch
                {
                    Console.WriteLine("Bonuses error");
                }

            }
        }

        public void Follow()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));

            driver.Url = "https://youlikehits.com/youtubenew2.php";
            if (driver.FindElementsByName("answer").Count != 0)
                SetAnswer();

            try
            {
                var follows = driver.FindElementsByClassName("followbutton");
                follows[0].Click();
                Thread.Sleep(TimeSpan.FromSeconds(2));

            }
            catch (Exception ex)
            {

                driver.Navigate().Refresh();
            }




            while (true)
            {
                try

                {
                    //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    //string count = (string)js.ExecuteScript("return document.getElementById('currentpoints')");

                    Console.Title = Program.selectedAcc.Login + "/" + driver.FindElementById("currentpoints").Text + "/" + bonus.ToString();

                    Console.WriteLine(Program.acc);
                    if (driver.FindElementsByName("answer").Count != 0)
                        SetAnswer();

                    string now = driver.FindElementByCssSelector(".followbutton").GetAttribute("onclick");

                    if (now == last)
                    {
                        driver.Navigate().Refresh();
                        continue;
                    }

                    driver.FindElementByCssSelector(".followbutton").Click();
                    last = driver.FindElementByCssSelector(".followbutton").GetAttribute("onclick");
                    Console.WriteLine("Sleep 61");

                    for (int i = 0; i < 62; i++)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        Console.Write($"{i}");
                        if (i % 10 == 0)
                            Console.Clear();

                    }


                    driver.SwitchTo().Window(driver.WindowHandles.First());
                    // Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    Failed();
                    driver.Navigate().Refresh();
                    // 

                    driver.SwitchTo().Window(driver.WindowHandles.First());
                }

            }
        }

        private void CloseAndBonus()
        {
            try
            {
                // 

                if (driver.WindowHandles.Count > 1)
                {

                    foreach (string window in driver.WindowHandles)
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        if (driver.WindowHandles.Last() != driver.WindowHandles.First())
                        {

                            if (bonus == false)
                            {

                                driver.Url = "https://youlikehits.com/bonuspoints.php";
                                driver.FindElementByCssSelector(".buybutton").Click();

                                Console.WriteLine("Bonuses");
                                driver.Url = "https://youlikehits.com/youtubenew2.php";

                                driver.Close();
                                Console.WriteLine("Windows close");

                            }

                        }
                    }



                }





            }
            catch
            {

            }
        }

        private void Failed(string answer = null)
        {
            bool haveError = false;
            var fonts = driver.FindElementsByTagName("font");
            foreach (var font in fonts)
            {
                if (font.Text.Contains("Failed. You did not successfully solve the problem"))
                {
                    driver.FindElementById("loadmore").Click();
                    haveError = true;
                }
            }

            if (!haveError)
            {
                string hash = bit.GetHashCode().ToString();
                if (db.Captchas.FirstOrDefault(x => x.Hash == ms) != null)
                {
                    // ok we have already 

                }
                else if (answer != null)
                {
                    db.Captchas.Add(new Captcha() { Hash = ms, Result = answer });
                    db.SaveChanges();
                    Console.WriteLine("*\n saved * \n");
                }





            }


            // save answer
        }
        private void SetAnswer()
        {
            //CloseAndBonus();
            var images = driver.FindElementsByTagName("img");
            foreach (var item in images)
            {
                if (item.GetAttribute("src").Contains("captchayt"))
                {
                    bit = CaptureElementScreenShot(item) as System.Drawing.Bitmap;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        bit.Save(memoryStream, ImageFormat.Jpeg);
                        ms = memoryStream.ToArray();
                    }
                    ConsoleWriteImage(bit);
                }
            }



            Console.WriteLine("answer :");
            string hash = bit.GetHashCode().ToString();
            var result = db.Captchas.FirstOrDefault(x => x.Hash == ms);
            string answer = null;
            if (result != null && attemps < 4)
            {
                answer = result.Result;
                Console.WriteLine($"finded in db{answer}");
                driver.FindElementByName("answer").SendKeys(answer);
                driver.FindElementByName("submit").Click();
                attemps++;

            }
            else
            {
               //  Console.Beep(333, 333);
                //answer = Console.ReadLine();
                answer = new Random().Next(1, 10).ToString();
                Console.WriteLine(answer);
                driver.FindElementByName("answer").SendKeys(answer);
                driver.FindElementByName("submit").Click();

            }
            Console.WriteLine("Yahoo");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Failed(answer);

        }

        public Image CaptureElementScreenShot(IWebElement element)
        {
            string filename = $"{Program.IMG}\\{Program.guid}.jpg";
            Screenshot screenshot = ((ITakesScreenshot)this.driver).GetScreenshot();
            screenshot.SaveAsFile(filename, ImageFormat.Jpeg);

            Image img = Bitmap.FromFile(filename);
            Rectangle rect = new Rectangle();

            if (element != null)
            {
                // Get the Width and Height of the WebElement using
                int width = element.Size.Width;
                int height = element.Size.Height;

                // Get the Location of WebElement in a Point.
                // This will provide X & Y co-ordinates of the WebElement
                Point p = element.Location;

                // Create a rectangle using Width, Height and element location
                rect = new Rectangle(p.X, p.Y, width, height);
            }

            // croping the image based on rect.
            Bitmap bmpImage = new Bitmap(img);
            var cropedImag = bmpImage.Clone(rect, bmpImage.PixelFormat);
            try
            { File.Delete(filename); }
            catch { }
            return cropedImag;
        }
        public static void ConsoleWriteImage(Bitmap bmpSrc)
        {
            Bitmap newBitmap = new Bitmap(bmpSrc.Width, bmpSrc.Height,
                               PixelFormat.Format8bppIndexed);
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, bmpSrc.Width), decimal.Divide(sMax, bmpSrc.Height));
            Size resSize = new Size((int)(bmpSrc.Width * percent), (int)(bmpSrc.Height * percent));
            Func<System.Drawing.Color, int> ToConsoleColor = c =>
            {
                int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
                index |= (c.R > 64) ? 4 : 0;
                index |= (c.G > 64) ? 2 : 0;
                index |= (c.B > 64) ? 1 : 0;
                return index;
            };
            Bitmap bmpMin = new Bitmap(bmpSrc, resSize);
            for (int i = 0; i < resSize.Height; i++)
            {
                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMin.GetPixel(j, i));
                    Console.Write("██");
                }
                System.Console.WriteLine();
            }
        }
    }
}
