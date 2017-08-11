using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Imaging;

namespace QuizupBot3_0
{

    class PokemonPlayer
    {
        Dictionary<string, string> bitmapPathDictionary = new Dictionary<string, string>
        {
            ["K"] = @"C:\Users\evanc\Desktop\prototypeLetters\Upper\K.bmp",
            ["L"] = @"C:\Users\evanc\Desktop\prototypeLetters\Upper\L.bmp",
            ["M"] = @"C:\Users\evanc\Desktop\prototypeLetters\Upper\M.bmp",
            ["W"] = @"C:\Users\evanc\Desktop\prototypeLetters\Upper\W.bmp",
            ["a"] = @"C:\Users\evanc\Desktop\prototypeLetters\a.bmp",
            ["b"] = @"C:\Users\evanc\Desktop\prototypeLetters\b.bmp",
            ["d"] = @"C:\Users\evanc\Desktop\prototypeLetters\d.bmp",
            ["e"] = @"C:\Users\evanc\Desktop\prototypeLetters\e.bmp",
            ["i"] = @"C:\Users\evanc\Desktop\prototypeLetters\i.bmp",
            ["l"] = @"C:\Users\evanc\Desktop\prototypeLetters\l.bmp",
            ["o"] = @"C:\Users\evanc\Desktop\prototypeLetters\o.bmp",
            ["r"] = @"C:\Users\evanc\Desktop\prototypeLetters\r.bmp",
            ["t"] = @"C:\Users\evanc\Desktop\prototypeLetters\t.bmp",
            ["y"] = @"C:\Users\evanc\Desktop\prototypeLetters\y.bmp"
        };


        static Bitmap letterL;
        const float TEMPLATE_MATCHING_SIMILARITY = 0.932f;
        const int BUTTON_RESIZE_FACTOR = 2;
        const int LETTER_RESIZE_FACTOR = 2;

        public string readButton(Bitmap button)
        {
            DateTime methodCall = DateTime.Now;
            string total = "";

            foreach (string letter in bitmapPathDictionary.Keys)
            {
                total += scanAreaForLetter(button, letter);
            }

            TimeSpan totalOperationTime = DateTime.Now - methodCall;

            Console.WriteLine("\nTotal operation time to read button: {0:F4} seconds", totalOperationTime.TotalSeconds);

            return total;
        }

        public string scanAreaForLetter(Bitmap area, string letter)
        {
            DateTime methodCall = DateTime.Now;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(TEMPLATE_MATCHING_SIMILARITY);

            Bitmap letterPic = getBitmapForLetter(letter);

            //Resizing
            letterPic = new AForge.Imaging.Filters.ResizeNearestNeighbor(letterPic.Width / LETTER_RESIZE_FACTOR, letterPic.Height / LETTER_RESIZE_FACTOR).Apply(letterPic);
            area = new AForge.Imaging.Filters.ResizeNearestNeighbor(area.Width / BUTTON_RESIZE_FACTOR, area.Height / BUTTON_RESIZE_FACTOR).Apply(area);

            DateTime beforeProcessing = DateTime.Now;
            TemplateMatch[] matches = tm.ProcessImage(area, letterPic);

            TimeSpan resizingTime = beforeProcessing - methodCall;
            TimeSpan processingTime = DateTime.Now - beforeProcessing;
            TimeSpan totalOperationTime = DateTime.Now - methodCall;

            Console.WriteLine("Resizing completed in {0:F4} seconds", resizingTime.TotalSeconds);
            Console.WriteLine("Image processing completed in {0:F4} seconds", processingTime.TotalSeconds);
            Console.WriteLine("Operation completed in {0:F4} seconds", totalOperationTime.TotalSeconds);

            string result = "";

            if (matches.Length > 0)
            {
                Console.WriteLine("\nMatch was made for letter {0}!\n", letter);
                foreach (TemplateMatch match in matches)
                    result += letter;
            }
            else
            {
                Console.WriteLine("\nNo matches found for letter {0}.\n", letter);
            }

            return result;

        }

        public Bitmap getBitmapForLetter(string letter)
        {
            string bitmapPath;
            bool success = bitmapPathDictionary.TryGetValue(letter, out bitmapPath);
            if (success)
                return new Bitmap(bitmapPath);

            else
            {
                Console.WriteLine("The letter {0} could not be found in the dictionary table for letters to Bitmaps.", letter);
                string bitmapLPath; bitmapPathDictionary.TryGetValue("L", out bitmapLPath);
                return new Bitmap(bitmapLPath);
            }
        }

        public PokemonPlayer()
        {
            letterL = new Bitmap(@"C:\Users\evanc\Desktop\prototypeLetters\L.bmp");
        }

    }
}
