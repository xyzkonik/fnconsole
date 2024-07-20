using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace FortniteExternalConsole
{
    class Program
    {
        static void Main()
        {
            string baseTitle = "FortConsole Made By KyeOnDiscord And Revamped by XyzKonik";
            int titleWidth = Console.WindowWidth; // Get the console window width
            int scrollSpeed = 200; // Adjust the scroll speed (milliseconds)

            // Start the title scrolling effect in a separate thread
            Thread titleThread = new Thread(() => ScrollTitle(baseTitle, titleWidth, scrollSpeed));
            titleThread.IsBackground = true; // Make it a background thread so it exits when the main thread exits
            titleThread.Start();

            string logpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"FortniteGame\Saved\Logs\FortniteGame.log");

            FileStream fileStream = null;
            StreamReader streamReader = null;
            long lastFileSize = 0;

            bool messageShown = false;

            while (true)
            {
                if (IsFortniteRunning())
                {
                    // Reset the message shown flag when Fortnite is running
                    messageShown = false;

                    if (File.Exists(logpath))
                    {
                        FileInfo fileInfo = new FileInfo(logpath);
                        long currentFileSize = fileInfo.Length;

                        // Check if the file has been cleared or reinitialized
                        if (fileStream == null || currentFileSize < lastFileSize)
                        {
                            fileStream?.Close();
                            streamReader?.Close();

                            Console.WriteLine("File cleared or reinitialized. Restarting log reading.");
                            Console.Clear();
                            fileStream = new FileStream(logpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            streamReader = new StreamReader(fileStream);
                            fileStream.Seek(0, SeekOrigin.End); // Start reading from the end of the file
                        }

                        lastFileSize = currentFileSize;

                        // Continuously read new logs appended to the file
                        ReadLogFile(streamReader);
                    }
                    else
                    {
                        Console.WriteLine($"Could not find Fortnite log files at {logpath}");
                    }
                }
                else
                {
                    // Print the message only once in blue if it's not already shown
                    if (!messageShown)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Fortnite client is not running.");
                        Console.ResetColor();
                        messageShown = true;
                    }
                }

                // Sleep for a short period before rechecking
                Thread.Sleep(1000);
            }
        }

        static void ScrollTitle(string baseTitle, int titleWidth, int scrollSpeed)
        {
            // Pad the baseTitle with spaces to ensure smooth scrolling
            string title = baseTitle.PadLeft(titleWidth + baseTitle.Length);

            // Scroll the title from right to left
            for (int i = 0; i < title.Length - titleWidth; i++)
            {
                Console.Title = title.Substring(i, titleWidth);
                Thread.Sleep(scrollSpeed);
            }

            // Set the final title position
            Console.Title = baseTitle;
        }

        static bool IsFortniteRunning()
        {
            Process[] processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
            return processes.Length > 0;
        }

        static void ReadLogFile(StreamReader streamReader)
        {
            try
            {
                string logLine;
                while ((logLine = streamReader.ReadLine()) != null)
                {
                    // Highlight URLs in blue
                    string highlightedLog = HighlightUrls(logLine);
                    Console.WriteLine(highlightedLog);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while reading the log file: {ex.Message}");
            }
        }

        static string HighlightUrls(string text)
        {
            // Define a regex pattern to match URLs
            string urlPattern = @"(https?:\/\/[^\s]+)";
            string blueColor = "\x1b[94m"; // ANSI escape code for blue text
            string resetColor = "\x1b[0m"; // ANSI escape code to reset text color

            // Replace URLs with highlighted versions
            string highlightedText = Regex.Replace(text, urlPattern, match =>
                $"{blueColor}{match.Value}{resetColor}");

            return highlightedText;
        }
    }
}
