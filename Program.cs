using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;
using System.Drawing.Imaging;
using OpenQA.Selenium.Support.UI;
using System.Text;

class Program
{

    //static void Main()
    //{
    //    string imagePath = @"D:\Memes\Sytuacyjne\pepohappy.jpg"; // Path to your image file
    //    try
    //    {
    //        using (Bitmap image = new Bitmap(imagePath))
    //        {
    //            string asciiArt = ConvertImageToAscii(image, 100); // Adjust the width as needed for your console window
    //            Console.WriteLine(asciiArt);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Error loading or processing image:");
    //        Console.WriteLine(ex.Message);
    //    }

    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

    //static void Main()
    //{
    //    ChromeOptions options = new ChromeOptions();
    //    options.AddArguments("headless");
    //    options.AddArguments("--start-maximized"); // Maximizes the window

    //    using (IWebDriver driver = new ChromeDriver(options))
    //    {
    //        try
    //        {
    //            driver.Navigate().GoToUrl("https://www.twitch.tv/moistcr1tikal");
    //            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    //            IWebElement videoPlayer = wait.Until(d => d.FindElement(By.ClassName("video-ref")));

    //            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, arguments[0].getBoundingClientRect().top + window.pageYOffset - window.innerHeight / 2);", videoPlayer);
    //            System.Threading.Thread.Sleep(1000);  // Allow time for scrolling.

    //            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
    //            using (Bitmap image = new Bitmap(new MemoryStream(screenshot.AsByteArray)))
    //            {
    //                string asciiArt = ConvertImageToAscii(image, 150); // Width of 100 characters
    //                Console.WriteLine(asciiArt);
    //            }
    //        }
    //        catch (NoSuchElementException ex)
    //        {
    //            Console.WriteLine("The element could not be found.");
    //            Console.WriteLine(ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("An error occurred:");
    //            Console.WriteLine(ex.Message);
    //        }
    //    }

    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

    static void Main()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArguments("headless");
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
        options.AddArgument("--autoplay-policy=no-user-gesture-required");
        options.AddArguments("window-size=800,1500"); // Adjust this based on the size of the element

        //options.AddArguments("--start-maximized"); // Maximizes the window

        using (IWebDriver driver = new ChromeDriver(options))
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.twitch.tv/moistcr1tikal");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement videoPlayer = wait.Until(d => d.FindElement(By.ClassName("persistent-player")));

                // Ensure the element is in the middle of the view
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, arguments[0].getBoundingClientRect().top + window.pageYOffset - window.innerHeight / 2);", videoPlayer);
                System.Threading.Thread.Sleep(1000);  // Allow time for scrolling.

                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                using (Image fullImg = Image.FromStream(new MemoryStream(screenshot.AsByteArray)))
                {
                    Rectangle cropArea = new Rectangle(videoPlayer.Location.X, videoPlayer.Location.Y, videoPlayer.Size.Width, videoPlayer.Size.Height);
                    Console.WriteLine($"Screenshot dimensions: {fullImg.Width}x{fullImg.Height}");
                    Console.WriteLine($"Element location: {videoPlayer.Location.X}, {videoPlayer.Location.Y}");
                    Console.WriteLine($"Element size: {videoPlayer.Size.Width}x{videoPlayer.Size.Height}");

                    if (cropArea.Right <= fullImg.Width && cropArea.Bottom <= fullImg.Height)
                    {
                        using (Bitmap bmpImage = new Bitmap(fullImg))
                        {
                            using (Bitmap croppedImage = bmpImage.Clone(cropArea, bmpImage.PixelFormat))
                            {
                                //string filePath = "video_player_screenshot.png";
                                //croppedImage.Save(filePath, ImageFormat.Png);
                                //Console.WriteLine($"Screenshot saved as {filePath}");
                                string asciiArt = ConvertImageToAscii(croppedImage, 200); // Adjust the width as needed for your console window
                                Console.WriteLine(asciiArt);
                            }
                            //using (Bitmap image = new Bitmap(fullImg))
                            //{
                            //    string asciiArt = ConvertImageToAscii(image, 100); // Adjust the width as needed for your console window
                            //    Console.WriteLine(asciiArt);
                            //}

                        }
                    }
                    else
                    {
                        Console.WriteLine("Crop area is out of image bounds!");
                    }
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("The element could not be found.");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }



    public static string ConvertImageToAscii(Bitmap image, int outputWidth)
    {
        // Charset with characters representing different shades from dark to light
        const string chars = "@%#*+=-:. ";

        // Calculate output height based on the aspect ratio
        int outputHeight = (int)(outputWidth * image.Height / (double)image.Width / 1.65);

        // Resize the image
        Bitmap resizedImage = new Bitmap(outputWidth, outputHeight);
        using (Graphics g = Graphics.FromImage(resizedImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, 0, 0, outputWidth, outputHeight);
        }

        // Convert the resized image to ASCII art
        StringBuilder asciiArt = new StringBuilder();
        for (int y = 0; y < resizedImage.Height; y++)
        {
            for (int x = 0; x < resizedImage.Width; x++)
            {
                // Get the color of the pixel
                Color pixelColor = resizedImage.GetPixel(x, y);

                // Calculate luminance
                double luminance = pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114;

                // Determine the character index based on luminance
                int index = (int)(luminance / 255 * (chars.Length - 1));

                // Append the corresponding character to the ASCII art
                asciiArt.Append(chars[index]);
            }
            asciiArt.AppendLine();  // Add newline at the end of each row
        }

        // Return the ASCII art
        return asciiArt.ToString();
    }





}



//static void Main()
//{
//    ChromeOptions options = new ChromeOptions();
//    options.AddArguments("headless");
//    options.AddArguments("--start-maximized"); // Maximizes the window

//    using (IWebDriver driver = new ChromeDriver(options))
//    {
//        try
//        {
//            driver.Navigate().GoToUrl("https://www.id-design.pl/");
//            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
//            IWebElement videoPlayer = wait.Until(d => d.FindElement(By.ClassName("kayden-logo")));

//            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, arguments[0].getBoundingClientRect().top + window.pageYOffset - window.innerHeight / 2);", videoPlayer);
//            System.Threading.Thread.Sleep(1000);  // Allow time for scrolling.

//            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
//            using (Bitmap image = new Bitmap(new MemoryStream(screenshot.AsByteArray)))
//            {
//                string asciiArt = ConvertImageToAscii(image, 100); // Width of 100 characters
//                Console.WriteLine(asciiArt);
//            }
//        }
//        catch (NoSuchElementException ex)
//        {
//            Console.WriteLine("The element could not be found.");
//            Console.WriteLine(ex.Message);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("An error occurred:");
//            Console.WriteLine(ex.Message);
//        }
//    }

//    Console.WriteLine("Press any key to exit...");
//    Console.ReadKey();
//}