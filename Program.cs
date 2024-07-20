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
            int titleWidth = Console.WindowWidth; 
            int scrollSpeed = 200; 

            
            Thread titleThread = new Thread(() => ScrollTitle(baseTitle, titleWidth, scrollSpeed));
            titleThread.IsBackground = true; 
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
                    
                    messageShown = false;

                    if (File.Exists(logpath))
                    {
                        FileInfo fileInfo = new FileInfo(logpath);
                        long currentFileSize = fileInfo.Length;

                        
                        if (fileStream == null || currentFileSize < lastFileSize)
                        {
                            fileStream?.Close();
                            streamReader?.Close();

                            Console.WriteLine("File cleared or reinitialized. Restarting log reading.");
                            Console.Clear();
                            fileStream = new FileStream(logpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            streamReader = new StreamReader(fileStream);
                            fileStream.Seek(0, SeekOrigin.End);
                        }

                        lastFileSize = currentFileSize;

                      
                        ReadLogFile(streamReader);
                    }
                    else
                    {
                        Console.WriteLine($"Could not find Fortnite log files at {logpath}");
                    }
                }
                else
                {
                   
                    if (!messageShown)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Fortnite client is not running.");
                        Console.ResetColor();
                        messageShown = true;
                    }
                }

              
                Thread.Sleep(1000);
            }
        }

        static void ScrollTitle(string baseTitle, int titleWidth, int scrollSpeed)
        {
           
            string title = baseTitle.PadLeft(titleWidth + baseTitle.Length);

            
            for (int i = 0; i < title.Length - titleWidth; i++)
            {
                Console.Title = title.Substring(i, titleWidth);
                Thread.Sleep(scrollSpeed);
            }

           
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
            
            string urlPattern = @"(https?:\/\/[^\s]+)";
            string blueColor = "\x1b[94m"; 
            string resetColor = "\x1b[0m"; 

           
            string highlightedText = Regex.Replace(text, urlPattern, match =>
                $"{blueColor}{match.Value}{resetColor}");

            return highlightedText;
        }
    }
}
