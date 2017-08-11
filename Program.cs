using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace QuizupBot_3._0
{
    class Program
    {
        const string SCREENSHOT_DIRECTORY = @"D:\_programming\Quizupbot 3.0\SCREENSHOTS\";
        const string PROBLEM_DIRECTORY = @"D:\_programming\Quizupbot 3.0\Problems\";


        static Point emulatorTopLeft = new Point(1138, 45); //1765, 896


        static Size emulatorSize = new Size(533, 948);

        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        static void Main(string[] args)
        {
            //SCREENCAP MODE
            /*
            ScreencapStream screencapper = new ScreencapStream(new Rectangle(emulatorTopLeft, emulatorSize));
            screencapper.beginSnapshotStream();

            Display disp = new Display();
            disp.Visible = true;

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Looping.");
                Bitmap sc = screencapper.getCurrentSnapshot();
                Random rd = new Random();
                int i = rd.Next(1, 99);
                sc.Save(@"D:\_programming\Quizupbot 3.0\SCREENSHOTS\SCREENCAPPER_" + i + ".bmp");
                disp.updateDisplay(sc);

            }

            */

            //MANUAL LOOP MODE
            
             while (true)
            {
                Console.WriteLine("Ready. Press enter to execute...");
                Console.ReadLine(); Console.Clear();

                Bitmap problem = takeSnapshotOfEmulator();

                problem.Save(SCREENSHOT_DIRECTORY + "PREPROCESSING_SCREENSHOT_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".bmp");
                Console.WriteLine("Screenshot taken.");

                MathResult result = MathPlayer.solveProblem(problem);

                Point clickPoint = result.answers.answerPoint;
                float SCALE_FACTOR_TEMP = 0.497222f;
                Point adjustedPoint = new Point((int)(clickPoint.X * SCALE_FACTOR_TEMP), (int)(clickPoint.Y * SCALE_FACTOR_TEMP));
                adjustedPoint = new Point(adjustedPoint.X + emulatorTopLeft.X, adjustedPoint.Y + emulatorTopLeft.Y);

                Console.WriteLine("Clicking at {0}, {1}", adjustedPoint.X, adjustedPoint.Y);
                LeftMouseClick(adjustedPoint.X, adjustedPoint.Y);
                

                Console.WriteLine("Finished.");
                result.answerScreenshot.Save(SCREENSHOT_DIRECTORY + "POSTPROCESSING_SCREENSHOT_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".bmp");
                Console.WriteLine("LOG:\n" + result.log);

            }


            //FOLDER WATCH MODE
            /*
            FileSystemWatcher folderWatcher = new FileSystemWatcher(@"D:\_programming\Quizupbot 3.0\INPUT\");
            folderWatcher.Created += new FileSystemEventHandler(onNewScreenshot);

            folderWatcher.EnableRaisingEvents = true;
            Console.WriteLine("Running.");
            while (true)
            {
                Thread.Sleep(100);
            }
            */

        }

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public static Bitmap takeSnapshotOfEmulator()
        {
            Bitmap screenShot = new Bitmap(emulatorSize.Width, emulatorSize.Height);
            using (Graphics g = Graphics.FromImage(screenShot))
            {
                g.CopyFromScreen(emulatorTopLeft, new Point(0, 0), emulatorSize);
            }

            return (new AForge.Imaging.Filters.ResizeNearestNeighbor(1080, 1920).Apply(screenShot));
        }

        public static void onNewScreenshot(object sender, FileSystemEventArgs e)
        {
            //When the event is triggered, it is too soon to access the file. We must wait until a file appears in the directory.
            DateTime eventTriggered = DateTime.Now;

            string[] files;
            while (true)
            {
                files = Directory.GetFiles(@"D:\_programming\Quizupbot 3.0\INPUT\");
                if (files.Length == 0)
                    Thread.Sleep(100);
                else
                    break;
            }
            string file = files[0];
            Console.WriteLine("Time from event to execution", (DateTime.Now - eventTriggered).TotalSeconds);

            Bitmap problem = new Bitmap(file);
            Console.WriteLine("New screenshot detected: " + file);

            MathResult result = MathPlayer.solveProblem(problem);

            result.answerScreenshot.Save(SCREENSHOT_DIRECTORY + string.Format("FINAL_ANSWER_{0}.bmp", result.answers.answer));
            Console.Write("\nLOG:\n" + result.log);
            problem.Dispose();
            File.Delete(file);
            Console.WriteLine("Operation completed in {0:F4} seconds", (DateTime.Now - eventTriggered).TotalSeconds);
        }
    }
}